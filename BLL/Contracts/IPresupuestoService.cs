using BLL.DTOs;
using System;
using System.Collections.Generic;

namespace BLL.Contracts
{
    /// <summary>
    /// Contrato para el servicio de lógica de negocio de Presupuesto
    /// Define las operaciones disponibles desde la UI
    /// </summary>
    public interface IPresupuestoService
    {
        /// <summary>
        /// Obtiene todos los presupuestos
        /// </summary>
        IEnumerable<PresupuestoDTO> GetAll();

        /// <summary>
        /// Obtiene un presupuesto por su ID con sus detalles
        /// </summary>
        PresupuestoDTO GetById(Guid id);

        /// <summary>
        /// Obtiene presupuestos por cliente
        /// </summary>
        IEnumerable<PresupuestoDTO> GetByCliente(Guid idCliente);

        /// <summary>
        /// Obtiene presupuestos por vendedor
        /// </summary>
        IEnumerable<PresupuestoDTO> GetByVendedor(Guid idVendedor);

        /// <summary>
        /// Obtiene presupuestos por estado
        /// </summary>
        IEnumerable<PresupuestoDTO> GetByEstado(int estado);

        /// <summary>
        /// Obtiene presupuestos por estados (permite múltiples estados)
        /// </summary>
        IEnumerable<PresupuestoDTO> GetByEstados(params int[] estados);

        /// <summary>
        /// Crea un nuevo presupuesto
        /// </summary>
        bool Add(PresupuestoDTO presupuestoDto);

        /// <summary>
        /// Actualiza un presupuesto existente
        /// </summary>
        bool Update(PresupuestoDTO presupuestoDto);

        /// <summary>
        /// Elimina un presupuesto
        /// </summary>
        bool Delete(Guid id);

        /// <summary>
        /// Obtiene el siguiente número de presupuesto disponible
        /// </summary>
        string GetNextNumero();

        /// <summary>
        /// Cambia el estado de un presupuesto
        /// </summary>
        bool CambiarEstado(Guid id, int nuevoEstado);

        /// <summary>
        /// Emite un presupuesto (cambia su estado a Emitido)
        /// </summary>
        bool Emitir(Guid id);

        /// <summary>
        /// Aprueba un presupuesto
        /// </summary>
        bool Aprobar(Guid id);

        /// <summary>
        /// Rechaza un presupuesto
        /// </summary>
        bool Rechazar(Guid id);

        /// <summary>
        /// Copia un presupuesto existente
        /// </summary>
        PresupuestoDTO Copiar(Guid idPresupuestoOriginal);
    }
}
