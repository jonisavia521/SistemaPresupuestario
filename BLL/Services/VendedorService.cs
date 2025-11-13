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
    /// <summary>
    /// Servicio de lógica de negocio para Vendedor
    /// Coordina operaciones entre UI y DAL, aplicando reglas de negocio
    /// </summary>
    public class VendedorService : IVendedorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVendedorRepository _vendedorRepository;
        private readonly IMapper _mapper;

        public VendedorService(
            IUnitOfWork unitOfWork,
            IVendedorRepository vendedorRepository,
            IMapper _mapper)
        {
            _unitOfWork = unitOfWork;
            _vendedorRepository = vendedorRepository;
            this._mapper = _mapper;
        }

        public IEnumerable<VendedorDTO> GetAll()
        {
            var entidades = _vendedorRepository.GetAll();
            return _mapper.Map<IEnumerable<VendedorDTO>>(entidades);
        }

        public IEnumerable<VendedorDTO> GetActivos()
        {
            var entidades = _vendedorRepository.GetActivos();
            return _mapper.Map<IEnumerable<VendedorDTO>>(entidades);
        }

        public VendedorDTO GetById(Guid id)
        {
            var entidad = _vendedorRepository.GetById(id);
            return _mapper.Map<VendedorDTO>(entidad);
        }

        public VendedorDTO GetByCodigo(string codigoVendedor)
        {
            var entidad = _vendedorRepository.GetByCodigo(codigoVendedor);
            return _mapper.Map<VendedorDTO>(entidad);
        }

        public VendedorDTO GetByCUIT(string cuit)
        {
            var entidad = _vendedorRepository.GetByCUIT(cuit);
            return _mapper.Map<VendedorDTO>(entidad);
        }

        public bool Add(VendedorDTO vendedorDTO)
        {
            try
            {
                // Validar que no exista el código
                if (_vendedorRepository.ExisteCodigo(vendedorDTO.CodigoVendedor))
                    throw new InvalidOperationException($"Ya existe un vendedor con el código '{vendedorDTO.CodigoVendedor}'");

                // Validar que no exista el CUIT
                if (_vendedorRepository.ExisteCUIT(vendedorDTO.CUIT))
                    throw new InvalidOperationException($"Ya existe un vendedor con el CUIT '{vendedorDTO.CUIT}'");

                // Crear la entidad de dominio (las validaciones de negocio se ejecutan en el constructor)
                var entidad = new VendedorDM(
                    vendedorDTO.CodigoVendedor,
                    vendedorDTO.Nombre,
                    vendedorDTO.CUIT,
                    vendedorDTO.PorcentajeComision,
                    vendedorDTO.Email,
                    vendedorDTO.Telefono,
                    vendedorDTO.Direccion
                );

                // Validación adicional de negocio
                entidad.ValidarNegocio();

                // Iniciar transacción
                _unitOfWork.BeginTransaction();

                // Agregar a través del repositorio
                _vendedorRepository.Add(entidad);

                // Confirmar cambios
                _unitOfWork.Commit();

                return true;
            }
            catch
            {
                _unitOfWork.Rollback();
                throw;
            }
        }

        public bool Update(VendedorDTO vendedorDTO)
        {
            try
            {
                // Obtener la entidad existente
                var entidadExistente = _vendedorRepository.GetById(vendedorDTO.Id);
                
                if (entidadExistente == null)
                    throw new InvalidOperationException($"No se encontró el vendedor con ID '{vendedorDTO.Id}'");

                // Validar que no exista otro vendedor con el mismo CUIT
                if (_vendedorRepository.ExisteCUIT(vendedorDTO.CUIT, vendedorDTO.Id))
                    throw new InvalidOperationException($"Ya existe otro vendedor con el CUIT '{vendedorDTO.CUIT}'");

                // Actualizar datos (las validaciones de negocio se ejecutan en el método)
                entidadExistente.ActualizarDatos(
                    vendedorDTO.Nombre,
                    vendedorDTO.CUIT,
                    vendedorDTO.PorcentajeComision,
                    vendedorDTO.Email,
                    vendedorDTO.Telefono,
                    vendedorDTO.Direccion
                );

                // Validación adicional de negocio
                entidadExistente.ValidarNegocio();

                // Iniciar transacción
                _unitOfWork.BeginTransaction();

                // Actualizar a través del repositorio
                _vendedorRepository.Update(entidadExistente);

                // Confirmar cambios
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
                var entidad = _vendedorRepository.GetById(id);
                
                if (entidad == null)
                    throw new InvalidOperationException($"No se encontró el vendedor con ID '{id}'");

                // Iniciar transacción
                _unitOfWork.BeginTransaction();

                // Eliminar lógicamente (desactivar)
                entidad.Desactivar();
                _vendedorRepository.Update(entidad);

                // Confirmar cambios
                _unitOfWork.Commit();

                return true;
            }
            catch
            {
                _unitOfWork.Rollback();
                throw;
            }
        }

        public bool Reactivar(Guid id)
        {
            try
            {
                var entidad = _vendedorRepository.GetById(id);
                
                if (entidad == null)
                    throw new InvalidOperationException($"No se encontró el vendedor con ID '{id}'");

                // Iniciar transacción
                _unitOfWork.BeginTransaction();

                // Reactivar
                entidad.Reactivar();
                _vendedorRepository.Update(entidad);

                // Confirmar cambios
                _unitOfWork.Commit();

                return true;
            }
            catch
            {
                _unitOfWork.Rollback();
                throw;
            }
        }
    }
}
