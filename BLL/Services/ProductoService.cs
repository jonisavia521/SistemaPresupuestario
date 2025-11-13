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
    /// Servicio de lógica de negocio para Producto
    /// Coordina operaciones entre UI y DAL, aplicando reglas de negocio
    /// TODOS LOS MÉTODOS SON SÍNCRONOS para evitar deadlocks en Windows Forms
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

        /// <summary>
        /// Obtiene todos los productos
        /// </summary>
        public IEnumerable<ProductoDTO> GetAll()
        {
            var entidades = _productoRepository.GetAll();
            return _mapper.Map<IEnumerable<ProductoDTO>>(entidades);
        }

        /// <summary>
        /// Obtiene todos los productos activos
        /// </summary>
        public IEnumerable<ProductoDTO> GetActivos()
        {
            var entidades = _productoRepository.GetActivos();
            return _mapper.Map<IEnumerable<ProductoDTO>>(entidades);
        }

        /// <summary>
        /// Obtiene un producto por ID
        /// </summary>
        public ProductoDTO GetById(Guid id)
        {
            var entidad = _productoRepository.GetById(id);
            return _mapper.Map<ProductoDTO>(entidad);
        }

        /// <summary>
        /// Busca un producto por código
        /// </summary>
        public ProductoDTO GetByCodigo(string codigo)
        {
            var entidad = _productoRepository.GetByCodigo(codigo);
            return _mapper.Map<ProductoDTO>(entidad);
        }

        /// <summary>
        /// Agrega un nuevo producto
        /// </summary>
        public bool Add(ProductoDTO productoDto)
        {
            try
            {
                // Validar que no exista el código
                if (_productoRepository.ExisteCodigo(productoDto.Codigo))
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

        /// <summary>
        /// Actualiza un producto existente
        /// </summary>
        public bool Update(ProductoDTO productoDto)
        {
            try
            {
                // Obtener la entidad existente
                var entidadExistente = _productoRepository.GetById(productoDto.Id);
                
                if (entidadExistente == null)
                    throw new InvalidOperationException($"No se encontró el producto con ID '{productoDto.Id}'");

                // Validar que no exista otro producto con el mismo código
                if (_productoRepository.ExisteCodigo(productoDto.Codigo, productoDto.Id))
                    throw new InvalidOperationException($"Ya existe otro producto con el código '{productoDto.Codigo}'");

                // Actualizar datos
                entidadExistente.Codigo = productoDto.Codigo;
                entidadExistente.Descripcion = productoDto.Descripcion;
                entidadExistente.Inhabilitado = productoDto.Inhabilitado;
                entidadExistente.PorcentajeIVA = productoDto.PorcentajeIVA;

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

        /// <summary>
        /// Elimina un producto (eliminación lógica)
        /// </summary>
        public bool Delete(Guid id)
        {
            try
            {
                var entidad = _productoRepository.GetById(id);
                
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

        /// <summary>
        /// Verifica si un código de producto ya existe
        /// </summary>
        public bool ExisteCodigo(string codigo, Guid? excludeId = null)
        {
            return _productoRepository.ExisteCodigo(codigo, excludeId);
        }
    }
}
