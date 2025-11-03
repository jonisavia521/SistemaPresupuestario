using DomainModel.Domain;
using System;
using System.Collections.Generic;

namespace DomainModel.Contract
{
    public interface IPresupuestoRepository : IRepository<PresupuestoDM>
    {
        /// <summary>
        /// Obtiene un presupuesto por ID (versión síncrona)
        /// </summary>
        PresupuestoDM GetById(Guid id);

        /// <summary>
        /// Obtiene un presupuesto con sus detalles
        /// </summary>
        PresupuestoDM GetByIdWithDetails(Guid id);

        /// <summary>
        /// Obtiene todos los presupuestos sin detalles
        /// </summary>
        IEnumerable<PresupuestoDM> GetAll();

        /// <summary>
        /// Obtiene todos los presupuestos con sus detalles
        /// </summary>
        IEnumerable<PresupuestoDM> GetAllWithDetails();

        /// <summary>
        /// Obtiene presupuestos por cliente
        /// </summary>
        IEnumerable<PresupuestoDM> GetByCliente(Guid idCliente);

        /// <summary>
        /// Obtiene presupuestos por vendedor
        /// </summary>
        IEnumerable<PresupuestoDM> GetByVendedor(Guid idVendedor);

        /// <summary>
        /// Obtiene presupuestos por estado
        /// </summary>
        IEnumerable<PresupuestoDM> GetByEstado(int estado);

        /// <summary>
        /// Obtiene el siguiente número de presupuesto disponible
        /// </summary>
        string GetNextNumero();

        /// <summary>
        /// Verifica si existe un presupuesto con el número indicado
        /// </summary>
        bool ExisteNumero(string numero, Guid? idExcluir = null);
    }
}
