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
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _vendedorRepository = vendedorRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<VendedorDTO>> GetAllAsync()
        {
            var entidades = await _vendedorRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<VendedorDTO>>(entidades);
        }

        public async Task<IEnumerable<VendedorDTO>> GetActivosAsync()
        {
            var entidades = await _vendedorRepository.GetActivosAsync();
            return _mapper.Map<IEnumerable<VendedorDTO>>(entidades);
        }

        public async Task<VendedorDTO> GetByIdAsync(Guid id)
        {
            var entidad = await _vendedorRepository.GetByIdAsync(id);
            return _mapper.Map<VendedorDTO>(entidad);
        }

        public async Task<VendedorDTO> GetByCodigoAsync(string codigoVendedor)
        {
            var entidad = await _vendedorRepository.GetByCodigoAsync(codigoVendedor);
            return _mapper.Map<VendedorDTO>(entidad);
        }

        public async Task<VendedorDTO> GetByCUITAsync(string cuit)
        {
            var entidad = await _vendedorRepository.GetByCUITAsync(cuit);
            return _mapper.Map<VendedorDTO>(entidad);
        }

        public async Task<bool> AddAsync(VendedorDTO vendedorDTO)
        {
            try
            {
                // Validar que no exista el código
                if (await _vendedorRepository.ExisteCodigoAsync(vendedorDTO.CodigoVendedor))
                    throw new InvalidOperationException($"Ya existe un vendedor con el código '{vendedorDTO.CodigoVendedor}'");

                // Validar que no exista el CUIT
                if (await _vendedorRepository.ExisteCUITAsync(vendedorDTO.CUIT))
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

        public async Task<bool> UpdateAsync(VendedorDTO vendedorDTO)
        {
            try
            {
                // Obtener la entidad existente
                var entidadExistente = await _vendedorRepository.GetByIdAsync(vendedorDTO.Id);
                
                if (entidadExistente == null)
                    throw new InvalidOperationException($"No se encontró el vendedor con ID '{vendedorDTO.Id}'");

                // Validar que no exista otro vendedor con el mismo CUIT
                if (await _vendedorRepository.ExisteCUITAsync(vendedorDTO.CUIT, vendedorDTO.Id))
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

        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                var entidad = await _vendedorRepository.GetByIdAsync(id);
                
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

        public async Task<bool> ReactivarAsync(Guid id)
        {
            try
            {
                var entidad = await _vendedorRepository.GetByIdAsync(id);
                
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
