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

        public IEnumerable<ClienteDTO> GetAll()
        {
            var entidades = _clienteRepository.GetAll();
            return _mapper.Map<IEnumerable<ClienteDTO>>(entidades);
        }

        public IEnumerable<ClienteDTO> GetActivos()
        {
            var entidades = _clienteRepository.GetActivos();
            return _mapper.Map<IEnumerable<ClienteDTO>>(entidades);
        }

        public ClienteDTO GetById(Guid id)
        {
            var entidad = _clienteRepository.GetById(id);
            return _mapper.Map<ClienteDTO>(entidad);
        }

        public ClienteDTO GetByCodigo(string codigoCliente)
        {
            var entidad = _clienteRepository.GetByCodigo(codigoCliente);
            return _mapper.Map<ClienteDTO>(entidad);
        }

        public ClienteDTO GetByDocumento(string numeroDocumento)
        {
            var entidad = _clienteRepository.GetByDocumento(numeroDocumento);
            return _mapper.Map<ClienteDTO>(entidad);
        }

        public bool Add(ClienteDTO clienteDTO)
        {
            try
            {
                // Validar que no exista el código
                if (_clienteRepository.ExisteCodigo(clienteDTO.CodigoCliente))
                    throw new InvalidOperationException($"Ya existe un cliente con el código '{clienteDTO.CodigoCliente}'");

                // Validar que no exista el documento
                if (_clienteRepository.ExisteDocumento(clienteDTO.NumeroDocumento))
                    throw new InvalidOperationException($"Ya existe un cliente con el documento '{clienteDTO.NumeroDocumento}'");

                // Crear la entidad de dominio (las validaciones de negocio se ejecutan en el constructor)
                var entidad = new ClienteDM(
                    clienteDTO.CodigoCliente,
                    clienteDTO.RazonSocial,
                    clienteDTO.TipoDocumento,
                    clienteDTO.NumeroDocumento,
                    clienteDTO.IdVendedor,
                    clienteDTO.TipoIva,
                    clienteDTO.CondicionPago,
                    clienteDTO.IdProvincia,
                    clienteDTO.Email,
                    clienteDTO.Telefono,
                    clienteDTO.Direccion,
                    clienteDTO.Localidad
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

        public bool Update(ClienteDTO clienteDTO)
        {
            try
            {
                // Obtener la entidad existente
                var entidadExistente = _clienteRepository.GetById(clienteDTO.Id);
                
                if (entidadExistente == null)
                    throw new InvalidOperationException($"No se encontró el cliente con ID '{clienteDTO.Id}'");

                // Validar que no exista otro cliente con el mismo documento
                if (_clienteRepository.ExisteDocumento(clienteDTO.NumeroDocumento, clienteDTO.Id))
                    throw new InvalidOperationException($"Ya existe otro cliente con el documento '{clienteDTO.NumeroDocumento}'");

                // Actualizar datos (las validaciones de negocio se ejecutan en el método)
                entidadExistente.ActualizarDatos(
                    clienteDTO.RazonSocial,
                    clienteDTO.TipoDocumento,
                    clienteDTO.NumeroDocumento,
                    clienteDTO.IdVendedor,
                    clienteDTO.TipoIva,
                    clienteDTO.CondicionPago,
                    clienteDTO.IdProvincia,
                    clienteDTO.Email,
                    clienteDTO.Telefono,
                    clienteDTO.Direccion,
                    clienteDTO.Localidad
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

        public bool Delete(Guid id)
        {
            try
            {
                var entidad = _clienteRepository.GetById(id);
                
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

        public bool Reactivar(Guid id)
        {
            try
            {
                var entidad = _clienteRepository.GetById(id);
                
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
