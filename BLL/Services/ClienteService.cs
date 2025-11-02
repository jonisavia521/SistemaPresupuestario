using AutoMapper;
using BLL.Contracts;
using BLL.DTOs;
using DomainModel.Contract;
using DomainModel.Contracts;
using DomainModel.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Services
{
    /// <summary>
    /// Servicio de lógica de negocio para Cliente
    /// Coordina operaciones entre UI y DAL, aplicando reglas de negocio
    /// </summary>
    public class ClienteService : IClienteService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IClienteRepository _clienteRepository;
        private readonly IMapper _mapper;

        public ClienteService(
            IUnitOfWork unitOfWork,
            IClienteRepository clienteRepository,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _clienteRepository = clienteRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ClienteDTO>> GetAllAsync()
        {
            var entidades = await _clienteRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ClienteDTO>>(entidades);
        }

        public async Task<IEnumerable<ClienteDTO>> GetActivosAsync()
        {
            var entidades = await _clienteRepository.GetActivosAsync();
            return _mapper.Map<IEnumerable<ClienteDTO>>(entidades);
        }

        public async Task<ClienteDTO> GetByIdAsync(Guid id)
        {
            var entidad = await _clienteRepository.GetByIdAsync(id);
            return _mapper.Map<ClienteDTO>(entidad);
        }

        public async Task<ClienteDTO> GetByCodigoAsync(string codigoCliente)
        {
            var entidad = await _clienteRepository.GetByCodigoAsync(codigoCliente);
            return _mapper.Map<ClienteDTO>(entidad);
        }

        public async Task<ClienteDTO> GetByDocumentoAsync(string numeroDocumento)
        {
            var entidad = await _clienteRepository.GetByDocumentoAsync(numeroDocumento);
            return _mapper.Map<ClienteDTO>(entidad);
        }

        public async Task<bool> AddAsync(ClienteDTO clienteDTO)
        {
            try
            {
                // Validar que no exista el código
                if (await _clienteRepository.ExisteCodigoAsync(clienteDTO.CodigoCliente))
                    throw new InvalidOperationException($"Ya existe un cliente con el código '{clienteDTO.CodigoCliente}'");

                // Validar que no exista el documento
                if (await _clienteRepository.ExisteDocumentoAsync(clienteDTO.NumeroDocumento))
                    throw new InvalidOperationException($"Ya existe un cliente con el documento '{clienteDTO.NumeroDocumento}'");

                // Crear la entidad de dominio (las validaciones de negocio se ejecutan en el constructor)
                var entidad = new ClienteDM(
                    clienteDTO.CodigoCliente,
                    clienteDTO.RazonSocial,
                    clienteDTO.TipoDocumento,
                    clienteDTO.NumeroDocumento,
                    clienteDTO.CodigoVendedor,
                    clienteDTO.TipoIva,
                    clienteDTO.CondicionPago,
                    clienteDTO.Email,
                    clienteDTO.Telefono,
                    clienteDTO.Direccion
                );

                // Validación adicional de negocio
                entidad.ValidarNegocio();

                // Iniciar transacción
                _unitOfWork.BeginTransaction();

                // Agregar a través del repositorio
                _clienteRepository.Add(entidad);

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

        public async Task<bool> UpdateAsync(ClienteDTO clienteDTO)
        {
            try
            {
                // Obtener la entidad existente
                var entidadExistente = await _clienteRepository.GetByIdAsync(clienteDTO.Id);
                
                if (entidadExistente == null)
                    throw new InvalidOperationException($"No se encontró el cliente con ID '{clienteDTO.Id}'");

                // Validar que no exista otro cliente con el mismo documento
                if (await _clienteRepository.ExisteDocumentoAsync(clienteDTO.NumeroDocumento, clienteDTO.Id))
                    throw new InvalidOperationException($"Ya existe otro cliente con el documento '{clienteDTO.NumeroDocumento}'");

                // Actualizar datos (las validaciones de negocio se ejecutan en el método)
                entidadExistente.ActualizarDatos(
                    clienteDTO.RazonSocial,
                    clienteDTO.TipoDocumento,
                    clienteDTO.NumeroDocumento,
                    clienteDTO.CodigoVendedor,
                    clienteDTO.TipoIva,
                    clienteDTO.CondicionPago,
                    clienteDTO.Email,
                    clienteDTO.Telefono,
                    clienteDTO.Direccion
                );

                // Validación adicional de negocio
                entidadExistente.ValidarNegocio();

                // Iniciar transacción
                _unitOfWork.BeginTransaction();

                // Actualizar a través del repositorio
                _clienteRepository.Update(entidadExistente);

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
                var entidad = await _clienteRepository.GetByIdAsync(id);
                
                if (entidad == null)
                    throw new InvalidOperationException($"No se encontró el cliente con ID '{id}'");

                // Iniciar transacción
                _unitOfWork.BeginTransaction();

                // Eliminar lógicamente (desactivar)
                entidad.Desactivar();
                _clienteRepository.Update(entidad);

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
                var entidad = await _clienteRepository.GetByIdAsync(id);
                
                if (entidad == null)
                    throw new InvalidOperationException($"No se encontró el cliente con ID '{id}'");

                // Iniciar transacción
                _unitOfWork.BeginTransaction();

                // Reactivar
                entidad.Reactivar();
                _clienteRepository.Update(entidad);

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
