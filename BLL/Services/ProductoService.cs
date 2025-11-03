using AutoMapper;
using BLL.Contracts;
using BLL.DTOs;
using DomainModel.Contract;
using DomainModel.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Services
{
    /// <summary>
    /// Servicio de lógica de negocio para Producto
    /// Coordina operaciones entre UI y DAL, aplicando reglas de negocio
    /// </summary>
    public class ProductoService : IProductoService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProductoRepository _productoRepository;
        private readonly IMapper _mapper;

        public ProductoService(
            IUnitOfWork unitOfWork,
            IProductoRepository productoRepository,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _productoRepository = productoRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductoDTO>> GetAllAsync()
        {
            var entidades = await _productoRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ProductoDTO>>(entidades);
        }

        public async Task<IEnumerable<ProductoDTO>> GetActivosAsync()
        {
            var entidades = await _productoRepository.GetActivosAsync();
            return _mapper.Map<IEnumerable<ProductoDTO>>(entidades);
        }

        public async Task<ProductoDTO> GetByIdAsync(Guid id)
        {
            var entidad = await _productoRepository.GetByIdAsync(id);
            return _mapper.Map<ProductoDTO>(entidad);
        }

        public async Task<ProductoDTO> GetByCodigoAsync(string codigo)
        {
            var entidad = await _productoRepository.GetByCodigoAsync(codigo);
            return _mapper.Map<ProductoDTO>(entidad);
        }

        public async Task<bool> AddAsync(ProductoDTO productoDto)
        {
            try
            {
                // Validar que no exista el código
                if (await _productoRepository.ExisteCodigoAsync(productoDto.Codigo))
                    throw new InvalidOperationException($"Ya existe un producto con el código '{productoDto.Codigo}'");

                // Mapear DTO a entidad de dominio
                var entidad = _mapper.Map<ProductoDM>(productoDto);
                
                // Asignar valores de auditoría
                entidad.ID = Guid.NewGuid();
                entidad.FechaAlta = DateTime.Now;
                entidad.UsuarioAlta = 1; // TODO: Obtener del contexto de usuario actual

                // Validación de negocio
                var errores = entidad.ValidarNegocio();
                if (errores.Any())
                    throw new InvalidOperationException(string.Join(Environment.NewLine, errores));

                // Iniciar transacción
                _unitOfWork.BeginTransaction();

                // Agregar a través del repositorio
                _productoRepository.Add(entidad);

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

        public async Task<bool> UpdateAsync(ProductoDTO productoDto)
        {
            try
            {
                // Obtener la entidad existente
                var entidadExistente = await _productoRepository.GetByIdAsync(productoDto.Id);
                
                if (entidadExistente == null)
                    throw new InvalidOperationException($"No se encontró el producto con ID '{productoDto.Id}'");

                // Validar que no exista otro producto con el mismo código
                if (await _productoRepository.ExisteCodigoAsync(productoDto.Codigo, productoDto.Id))
                    throw new InvalidOperationException($"Ya existe otro producto con el código '{productoDto.Codigo}'");

                // Actualizar datos
                entidadExistente.Codigo = productoDto.Codigo;
                entidadExistente.Descripcion = productoDto.Descripcion;
                entidadExistente.Inhabilitado = productoDto.Inhabilitado;
                entidadExistente.PorcentajeIVA = productoDto.PorcentajeIVA; // AGREGADO

                // Validación de negocio
                var errores = entidadExistente.ValidarNegocio();
                if (errores.Any())
                    throw new InvalidOperationException(string.Join(Environment.NewLine, errores));

                // Iniciar transacción
                _unitOfWork.BeginTransaction();

                // Actualizar a través del repositorio
                _productoRepository.Update(entidadExistente);

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

        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                var entidad = await _productoRepository.GetByIdAsync(id);
                
                if (entidad == null)
                    throw new InvalidOperationException($"No se encontró el producto con ID '{id}'");

                // Iniciar transacción
                _unitOfWork.BeginTransaction();

                // Eliminar lógicamente (inhabilitar)
                entidad.Inhabilitado = true;
                _productoRepository.Update(entidad);

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

        public async Task<bool> ExisteCodigoAsync(string codigo, Guid? excludeId = null)
        {
            return await _productoRepository.ExisteCodigoAsync(codigo, excludeId);
        }
    }
}
