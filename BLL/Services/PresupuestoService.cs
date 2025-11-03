using AutoMapper;
using BLL.Contracts;
using BLL.DTOs;
using DomainModel.Contract;
using DomainModel.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BLL.Services
{
    public class PresupuestoService : IPresupuestoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PresupuestoService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public IEnumerable<PresupuestoDTO> GetAll()
        {
            var presupuestos = _unitOfWork.PresupuestoRepository.GetAllWithDetails();
            var dtos = presupuestos.Select(p => _mapper.Map<PresupuestoDTO>(p)).ToList();

            // Enriquecer con datos de navegación
            EnriquecerDatosNavegacion(dtos);

            return dtos;
        }

        public PresupuestoDTO GetById(Guid id)
        {
            var presupuesto = _unitOfWork.PresupuestoRepository.GetByIdWithDetails(id);
            
            if (presupuesto == null)
                return null;

            var dto = _mapper.Map<PresupuestoDTO>(presupuesto);

            // Enriquecer con datos de navegación
            EnriquecerDatosNavegacion(new List<PresupuestoDTO> { dto });

            return dto;
        }

        public IEnumerable<PresupuestoDTO> GetByCliente(Guid idCliente)
        {
            var presupuestos = _unitOfWork.PresupuestoRepository.GetByCliente(idCliente);
            var dtos = presupuestos.Select(p => _mapper.Map<PresupuestoDTO>(p)).ToList();

            EnriquecerDatosNavegacion(dtos);

            return dtos;
        }

        public IEnumerable<PresupuestoDTO> GetByVendedor(Guid idVendedor)
        {
            var presupuestos = _unitOfWork.PresupuestoRepository.GetByVendedor(idVendedor);
            var dtos = presupuestos.Select(p => _mapper.Map<PresupuestoDTO>(p)).ToList();

            EnriquecerDatosNavegacion(dtos);

            return dtos;
        }

        public IEnumerable<PresupuestoDTO> GetByEstado(int estado)
        {
            var presupuestos = _unitOfWork.PresupuestoRepository.GetByEstado(estado);
            var dtos = presupuestos.Select(p => _mapper.Map<PresupuestoDTO>(p)).ToList();

            EnriquecerDatosNavegacion(dtos);

            return dtos;
        }

        public bool Add(PresupuestoDTO presupuestoDto)
        {
            try
            {
                // Crear entidad de dominio
                var presupuesto = new PresupuestoDM(
                    presupuestoDto.Numero,
                    presupuestoDto.IdCliente,
                    presupuestoDto.FechaEmision,
                    presupuestoDto.FechaVencimiento,
                    presupuestoDto.IdVendedor,
                    presupuestoDto.IdPresupuestoPadre
                );

                // Agregar detalles
                if (presupuestoDto.Detalles != null && presupuestoDto.Detalles.Any())
                {
                    foreach (var detalleDto in presupuestoDto.Detalles)
                    {
                        var detalle = new PresupuestoDetalleDM(
                            detalleDto.IdProducto,
                            detalleDto.Cantidad,
                            detalleDto.Precio,
                            detalleDto.PorcentajeIVA,
                            detalleDto.Descuento
                        );

                        presupuesto.AgregarDetalle(detalle);
                    }
                }

                // Validar reglas de negocio
                presupuesto.ValidarNegocio();

                // Persistir
                _unitOfWork.PresupuestoRepository.Add(presupuesto);
                _unitOfWork.Commit();

                // Actualizar el ID en el DTO
                presupuestoDto.Id = presupuesto.Id;

                return true;
            }
            catch
            {
                _unitOfWork.Rollback();
                throw;
            }
        }

        public bool Update(PresupuestoDTO presupuestoDto)
        {
            try
            {
                // Obtener entidad existente
                var presupuesto = _unitOfWork.PresupuestoRepository.GetByIdWithDetails(presupuestoDto.Id);

                if (presupuesto == null)
                    throw new InvalidOperationException("El presupuesto no existe.");

                // Actualizar datos del presupuesto
                presupuesto.ActualizarDatos(
                    presupuestoDto.IdCliente,
                    presupuestoDto.FechaEmision,
                    presupuestoDto.FechaVencimiento,
                    presupuestoDto.IdVendedor
                );

                // Actualizar detalles
                presupuesto.LimpiarDetalles();

                if (presupuestoDto.Detalles != null && presupuestoDto.Detalles.Any())
                {
                    foreach (var detalleDto in presupuestoDto.Detalles)
                    {
                        var detalle = new PresupuestoDetalleDM(
                            detalleDto.IdProducto,
                            detalleDto.Cantidad,
                            detalleDto.Precio,
                            detalleDto.PorcentajeIVA,
                            detalleDto.Descuento
                        );

                        presupuesto.AgregarDetalle(detalle);
                    }
                }

                // Validar reglas de negocio
                presupuesto.ValidarNegocio();

                // Persistir
                _unitOfWork.PresupuestoRepository.Update(presupuesto);
                _unitOfWork.Commit();

                return true;
            }
            catch
            {
                _unitOfWork.Rollback();
                throw;
            }
        }

        public bool Delete(Guid id)
        {
            try
            {
                var presupuesto = _unitOfWork.PresupuestoRepository.GetById(id);

                if (presupuesto == null)
                    throw new InvalidOperationException("El presupuesto no existe.");

                if (presupuesto.Estado == 2) // Aprobado
                    throw new InvalidOperationException("No se puede eliminar un presupuesto aprobado.");

                _unitOfWork.PresupuestoRepository.Delete(presupuesto);
                _unitOfWork.Commit();

                return true;
            }
            catch
            {
                _unitOfWork.Rollback();
                throw;
            }
        }

        public string GetNextNumero()
        {
            return _unitOfWork.PresupuestoRepository.GetNextNumero();
        }

        public bool CambiarEstado(Guid id, int nuevoEstado)
        {
            try
            {
                var presupuesto = _unitOfWork.PresupuestoRepository.GetById(id);

                if (presupuesto == null)
                    throw new InvalidOperationException("El presupuesto no existe.");

                presupuesto.CambiarEstado(nuevoEstado);
                presupuesto.ValidarNegocio();

                _unitOfWork.PresupuestoRepository.Update(presupuesto);
                _unitOfWork.Commit();

                return true;
            }
            catch
            {
                _unitOfWork.Rollback();
                throw;
            }
        }

        public bool Emitir(Guid id)
        {
            try
            {
                var presupuesto = _unitOfWork.PresupuestoRepository.GetByIdWithDetails(id);

                if (presupuesto == null)
                    throw new InvalidOperationException("El presupuesto no existe.");

                presupuesto.Emitir();

                _unitOfWork.PresupuestoRepository.Update(presupuesto);
                _unitOfWork.Commit();

                return true;
            }
            catch
            {
                _unitOfWork.Rollback();
                throw;
            }
        }

        public bool Aprobar(Guid id)
        {
            try
            {
                var presupuesto = _unitOfWork.PresupuestoRepository.GetById(id);

                if (presupuesto == null)
                    throw new InvalidOperationException("El presupuesto no existe.");

                presupuesto.Aprobar();

                _unitOfWork.PresupuestoRepository.Update(presupuesto);
                _unitOfWork.Commit();

                return true;
            }
            catch
            {
                _unitOfWork.Rollback();
                throw;
            }
        }

        public bool Rechazar(Guid id)
        {
            try
            {
                var presupuesto = _unitOfWork.PresupuestoRepository.GetById(id);

                if (presupuesto == null)
                    throw new InvalidOperationException("El presupuesto no existe.");

                presupuesto.Rechazar();

                _unitOfWork.PresupuestoRepository.Update(presupuesto);
                _unitOfWork.Commit();

                return true;
            }
            catch
            {
                _unitOfWork.Rollback();
                throw;
            }
        }

        public IEnumerable<PresupuestoDTO> GetByEstados(params int[] estados)
        {
            if (estados == null || estados.Length == 0)
                return Enumerable.Empty<PresupuestoDTO>();

            var todosPresupuestos = _unitOfWork.PresupuestoRepository.GetAllWithDetails();
            var filtrados = todosPresupuestos.Where(p => estados.Contains(p.Estado));
            var dtos = filtrados.Select(p => _mapper.Map<PresupuestoDTO>(p)).ToList();

            EnriquecerDatosNavegacion(dtos);

            return dtos;
        }

        public PresupuestoDTO Copiar(Guid idPresupuestoOriginal)
        {
            try
            {
                var original = _unitOfWork.PresupuestoRepository.GetByIdWithDetails(idPresupuestoOriginal);

                if (original == null)
                    throw new InvalidOperationException("El presupuesto original no existe.");

                // Crear nuevo presupuesto
                var nuevoNumero = GetNextNumero();
                var nuevo = new PresupuestoDM(
                    nuevoNumero,
                    original.IdCliente,
                    DateTime.Now,
                    original.FechaVencimiento,
                    original.IdVendedor,
                    original.Id // Establecer como presupuesto padre
                );

                // Copiar detalles
                foreach (var detalleOriginal in original.Detalles)
                {
                    var detalle = new PresupuestoDetalleDM(
                        detalleOriginal.IdProducto,
                        detalleOriginal.Cantidad,
                        detalleOriginal.Precio,
                        detalleOriginal.PorcentajeIVA,
                        detalleOriginal.Descuento
                    );

                    nuevo.AgregarDetalle(detalle);
                }

                // Validar y persistir
                nuevo.ValidarNegocio();
                _unitOfWork.PresupuestoRepository.Add(nuevo);
                _unitOfWork.Commit();

                // Retornar DTO
                return GetById(nuevo.Id);
            }
            catch
            {
                _unitOfWork.Rollback();
                throw;
            }
        }

        private void EnriquecerDatosNavegacion(List<PresupuestoDTO> dtos)
        {
            foreach (var dto in dtos)
            {
                // Obtener datos del cliente
                if (dto.IdCliente != Guid.Empty)
                {
                    var cliente = _unitOfWork.ClienteRepository.GetById(dto.IdCliente);
                    if (cliente != null)
                    {
                        dto.ClienteRazonSocial = cliente.RazonSocial;
                    }
                }

                // Obtener datos del vendedor
                if (dto.IdVendedor.HasValue && dto.IdVendedor.Value != Guid.Empty)
                {
                    var vendedor = _unitOfWork.VendedorRepository.GetById(dto.IdVendedor.Value);
                    if (vendedor != null)
                    {
                        dto.VendedorNombre = vendedor.Nombre;
                    }
                }

                // Enriquecer detalles
                if (dto.Detalles != null && dto.Detalles.Any())
                {
                    foreach (var detalle in dto.Detalles)
                    {
                        if (detalle.IdProducto.HasValue && detalle.IdProducto.Value != Guid.Empty)
                        {
                            var producto = _unitOfWork.ProductoRepository.GetById(detalle.IdProducto.Value);
                            if (producto != null)
                            {
                                detalle.Codigo = producto.Codigo;
                                detalle.Descripcion = producto.Descripcion;
                            }
                        }
                    }
                }
            }
        }
    }
}
