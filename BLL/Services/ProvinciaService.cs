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
    /// Servicio de lógica de negocio para Provincia
    /// Intermediario entre UI (que usa DTOs) y DAL (que usa entidades de dominio)
    /// </summary>
    public class ProvinciaService : IProvinciaService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProvinciaService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public ProvinciaDTO GetById(Guid id)
        {
            var provincia = _unitOfWork.ProvinciaRepository.GetById(id);
            return _mapper.Map<ProvinciaDTO>(provincia);
        }

        public ProvinciaDTO GetByCodigoAFIP(string codigoAFIP)
        {
            var provincia = _unitOfWork.ProvinciaRepository.GetByCodigoAFIP(codigoAFIP);
            return _mapper.Map<ProvinciaDTO>(provincia);
        }

        public IEnumerable<ProvinciaDTO> GetAll()
        {
            var provincias = _unitOfWork.ProvinciaRepository.GetAll();
            return _mapper.Map<IEnumerable<ProvinciaDTO>>(provincias);
        }

        public IEnumerable<ProvinciaDTO> GetAllOrdenadas()
        {
            var provincias = _unitOfWork.ProvinciaRepository.GetAllOrdenadas();
            return _mapper.Map<IEnumerable<ProvinciaDTO>>(provincias);
        }
    }
}
