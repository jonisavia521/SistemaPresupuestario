using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Contracts
{
    /// <summary>
    /// Contrato genérico que define las operaciones básicas de lógica de negocio (CRUD).
    /// 
    /// Esta interfaz genérica proporciona un contrato base para todos los servicios de negocio
    /// que implementan operaciones estándar de Crear, Leer, Actualizar y Eliminar (CRUD).
    /// Los servicios específicos pueden heredar de esta interfaz o implementarla directamente,
    /// aunque la mayoría implementan interfaces específicas más ricas con métodos adicionales.
    /// 
    /// Este es un patrón genérico que reduce la duplicación de código en servicios similares.
    /// </summary>
    /// <typeparam name="T">
    /// El tipo de objeto genérico sobre el cual operará este servicio de negocio.
    /// Típicamente es un DTO (Data Transfer Object) que representa una entidad de dominio.
    /// </typeparam>
    public interface IGenericBusinessService<T>
    {
        /// <summary>
        /// Agrega un nuevo objeto del tipo especificado al sistema.
        /// 
        /// Este método es responsable de realizar la lógica de negocio necesaria
        /// para crear y persistir un nuevo registro en la base de datos.
        /// </summary>
        /// <param name="obj">
        /// El objeto del tipo T que se desea agregar al sistema.
        /// No debe ser null.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Se lanza si obj es null.
        /// </exception>
        void Add(T obj);

        /// <summary>
        /// Actualiza un objeto existente en el sistema.
        /// 
        /// Este método es responsable de realizar la lógica de negocio necesaria
        /// para modificar y persistir los cambios de un registro existente en la base de datos.
        /// </summary>
        /// <param name="obj">
        /// El objeto del tipo T que se desea actualizar.
        /// El objeto debe existir previamente en el sistema (debe tener un ID válido).
        /// No debe ser null.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Se lanza si obj es null.
        /// </exception>
        void Update(T obj);

        /// <summary>
        /// Elimina un objeto existente en el sistema.
        /// 
        /// Este método es responsable de realizar la lógica de negocio necesaria
        /// para eliminar (típicamente de forma lógica) un registro de la base de datos.
        /// La mayoría de implementaciones realizan eliminación lógica en lugar de física.
        /// </summary>
        /// <param name="id">
        /// El GUID (identificador único global) del objeto que se desea eliminar.
        /// </param>
        void Delete(Guid id);

        /// <summary>
        /// Obtiene todos los objetos del tipo especificado registrados en el sistema.
        /// 
        /// Este método recupera la colección completa sin filtros.
        /// </summary>
        /// <returns>
        /// Una colección enumerable de objetos del tipo T que representan todos los registros.
        /// Si no existen registros, retorna una colección vacía (nunca null).
        /// </returns>
        IEnumerable<T> SelectAll();

        /// <summary>
        /// Obtiene un objeto específico del tipo T por su identificador único.
        /// 
        /// Este método es utilizado para recuperar un registro específico cuando se conoce su ID.
        /// </summary>
        /// <param name="id">
        /// El GUID (identificador único global) del objeto que se desea recuperar.
        /// </param>
        /// <returns>
        /// Un objeto del tipo T que corresponde al ID proporcionado,
        /// o null si no existe un objeto con ese ID en el sistema.
        /// </returns>
        T SelectOne(Guid id);
    }
}
