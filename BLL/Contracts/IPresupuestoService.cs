using BLL.DTOs;
using System;
using System.Collections.Generic;

namespace BLL.Contracts
{
    /// <summary>
    /// Contrato que define las operaciones de lógica de negocio disponibles para la gestión de presupuestos.
    /// 
    /// Los presupuestos (también llamados cotizaciones) son propuestas de venta detalladas que se presentan
    /// a los clientes. Transitan por diferentes estados durante su ciclo de vida hasta llegar a convertirse
    /// en facturas.
    /// 
    /// ESTADOS DEL PRESUPUESTO:
    /// - 1 = Borrado: Presupuesto marcado como eliminado (eliminación lógica)
    /// - 2 = Emitido: Estado inicial, recién creado, disponible para modificaciones
    /// - 3 = Aprobado: El cliente aceptó el presupuesto, listo para facturar
    /// - 4 = Rechazado: El cliente rechazó el presupuesto
    /// - 5 = Vencido: Presupuesto cuya fecha de validez ha expirado (estado implícito basado en fecha)
    /// - 6 = Facturado: Convertido en factura legal
    /// </summary>
    public interface IPresupuestoService
    {
        /// <summary>
        /// Obtiene todos los presupuestos registrados en el sistema,
        /// independientemente de su estado o antigüedad.
        /// </summary>
        /// <returns>
        /// Una colección enumerable de objetos PresupuestoDTO que representan todos los presupuestos.
        /// Si no existen presupuestos, retorna una colección vacía.
        /// </returns>
        IEnumerable<PresupuestoDTO> GetAll();

        /// <summary>
        /// Obtiene un presupuesto específico por su identificador único (ID),
        /// incluyendo todos los detalles (líneas de artículos) del presupuesto.
        /// </summary>
        /// <param name="id">
        /// El GUID (identificador único global) del presupuesto que se desea recuperar.
        /// </param>
        /// <returns>
        /// Un objeto PresupuestoDTO que contiene la información completa del presupuesto
        /// y sus detalles, o null si no existe presupuesto con el ID proporcionado.
        /// </returns>
        PresupuestoDTO GetById(Guid id);

        /// <summary>
        /// Obtiene todos los presupuestos asociados a un cliente específico.
        /// </summary>
        /// <param name="idCliente">
        /// El GUID del cliente cuyos presupuestos se desean recuperar.
        /// </param>
        /// <returns>
        /// Una colección enumerable de objetos PresupuestoDTO correspondientes al cliente especificado.
        /// Si el cliente no tiene presupuestos, retorna una colección vacía.
        /// </returns>
        IEnumerable<PresupuestoDTO> GetByCliente(Guid idCliente);

        /// <summary>
        /// Obtiene todos los presupuestos creados por un vendedor específico.
        /// </summary>
        /// <param name="idVendedor">
        /// El GUID del vendedor cuyos presupuestos se desean recuperar.
        /// </param>
        /// <returns>
        /// Una colección enumerable de objetos PresupuestoDTO creados por el vendedor especificado.
        /// Si el vendedor no tiene presupuestos, retorna una colección vacía.
        /// </returns>
        IEnumerable<PresupuestoDTO> GetByVendedor(Guid idVendedor);

        /// <summary>
        /// Obtiene todos los presupuestos que se encuentran en un estado específico.
        /// </summary>
        /// <param name="estado">
        /// El código del estado que se desea filtrar (1=Borrado, 2=Emitido, 3=Aprobado, 4=Rechazado, 5=Vencido, 6=Facturado).
        /// </param>
        /// <returns>
        /// Una colección enumerable de objetos PresupuestoDTO que están en el estado especificado.
        /// Si no hay presupuestos en ese estado, retorna una colección vacía.
        /// </returns>
        IEnumerable<PresupuestoDTO> GetByEstado(int estado);

        /// <summary>
        /// Obtiene todos los presupuestos que se encuentran en cualquiera de los estados especificados.
        /// Permite filtrar presupuestos por múltiples estados en una sola consulta.
        /// </summary>
        /// <param name="estados">
        /// Códigos de los estados que se desean filtrar (ej: 2, 3 para obtener Emitidos y Aprobados).
        /// </param>
        /// <returns>
        /// Una colección enumerable de objetos PresupuestoDTO que están en cualquiera de los estados especificados.
        /// Si no hay presupuestos en esos estados, retorna una colección vacía.
        /// </returns>
        IEnumerable<PresupuestoDTO> GetByEstados(params int[] estados);

        /// <summary>
        /// Crea un nuevo presupuesto en el sistema.
        /// 
        /// Esta operación realiza validaciones de negocio necesarias, como verificar que el cliente
        /// existe, que hay detalles válidos, etc., antes de persistir el nuevo presupuesto.
        /// El presupuesto se crea en estado Emitido (estado 2).
        /// </summary>
        /// <param name="presupuestoDto">
        /// Objeto PresupuestoDTO que contiene los datos del nuevo presupuesto, incluyendo cliente,
        /// vendedor, detalles (líneas de artículos) y condiciones comerciales.
        /// </param>
        /// <returns>
        /// true si el presupuesto fue creado exitosamente;
        /// false si ocurrió un error durante la operación (ej: validación fallida).
        /// </returns>
        bool Add(PresupuestoDTO presupuestoDto);

        /// <summary>
        /// Actualiza un presupuesto existente en el sistema.
        /// 
        /// Solo se pueden modificar presupuestos en estado Emitido.
        /// Esta operación permite cambiar datos como cliente, vendedor, detalles, etc.
        /// </summary>
        /// <param name="presupuestoDto">
        /// Objeto PresupuestoDTO que contiene los datos actualizados del presupuesto.
        /// El ID debe corresponder a un presupuesto existente en estado Emitido.
        /// </param>
        /// <returns>
        /// true si el presupuesto fue actualizado exitosamente;
        /// false si ocurrió un error (ej: presupuesto no encontrado, no está en estado Emitido).
        /// </returns>
        bool Update(PresupuestoDTO presupuestoDto);

        /// <summary>
        /// Marca un presupuesto como borrado (eliminación lógica).
        /// 
        /// Los presupuestos solo pueden ser borrados si están en estado Emitido.
        /// Este método no elimina físicamente el registro de la base de datos, sino que
        /// cambia su estado a Borrado (estado 1) para preservar la integridad histórica.
        /// </summary>
        /// <param name="id">
        /// El GUID del presupuesto que se desea eliminar.
        /// </param>
        /// <returns>
        /// true si el presupuesto fue marcado como borrado exitosamente;
        /// false si ocurrió un error (ej: presupuesto no encontrado, no está en estado Emitido).
        /// </returns>
        bool Delete(Guid id);

        /// <summary>
        /// Genera y retorna el siguiente número de presupuesto disponible en el sistema.
        /// 
        /// Este método es responsable de mantener la secuencia de números de presupuestos únicos,
        /// garantizando que no existan duplicados. Típicamente implementa una lógica de contador
        /// secuencial o usa formatos específicos de la empresa.
        /// </summary>
        /// <returns>
        /// Una cadena de texto que representa el siguiente número de presupuesto disponible.
        /// </returns>
        string GetNextNumero();

        /// <summary>
        /// Cambia el estado de un presupuesto existente.
        /// 
        /// Este es un método genérico que permite transicionar un presupuesto a cualquier estado.
        /// Las implementaciones específicas (Aprobar, Rechazar, etc.) usan este método internamente
        /// después de validar que la transición de estado es válida.
        /// </summary>
        /// <param name="id">
        /// El GUID del presupuesto cuyo estado se desea cambiar.
        /// </param>
        /// <param name="nuevoEstado">
        /// El código del nuevo estado (1=Borrado, 2=Emitido, 3=Aprobado, 4=Rechazado, 5=Vencido, 6=Facturado).
        /// </param>
        /// <returns>
        /// true si el estado fue cambiado exitosamente;
        /// false si ocurrió un error (ej: presupuesto no encontrado, estado inválido).
        /// </returns>
        bool CambiarEstado(Guid id, int nuevoEstado);

        /// <summary>
        /// Emite un presupuesto (cambia su estado a Emitido = 2).
        /// 
        /// Este método es principalmente documentativo, ya que los presupuestos se crean
        /// directamente en estado Emitido. Se proporciona para mantener una API simétrica.
        /// </summary>
        /// <param name="id">
        /// El GUID del presupuesto que se desea emitir.
        /// </param>
        /// <returns>
        /// true si el presupuesto fue emitido exitosamente;
        /// false si ocurrió un error.
        /// </returns>
        bool Emitir(Guid id);

        /// <summary>
        /// Marca un presupuesto como borrado (estado 1).
        /// 
        /// Los presupuestos solo pueden ser borrados si están en estado Emitido.
        /// Esta es una eliminación lógica que preserva los datos históricos.
        /// </summary>
        /// <param name="id">
        /// El GUID del presupuesto que se desea borrar.
        /// </param>
        /// <returns>
        /// true si el presupuesto fue marcado como borrado exitosamente;
        /// false si ocurrió un error (ej: presupuesto no encontrado, no está en estado Emitido).
        /// </returns>
        bool Borrar(Guid id);

        /// <summary>
        /// Aprueba un presupuesto (cambia su estado a Aprobado = 3).
        /// 
        /// Los presupuestos solo pueden ser aprobados si están en estado Emitido.
        /// Una vez aprobado, el presupuesto está listo para ser convertido en factura.
        /// </summary>
        /// <param name="id">
        /// El GUID del presupuesto que se desea aprobar.
        /// </param>
        /// <returns>
        /// true si el presupuesto fue aprobado exitosamente;
        /// false si ocurrió un error (ej: presupuesto no encontrado, no está en estado Emitido).
        /// </returns>
        bool Aprobar(Guid id);

        /// <summary>
        /// Rechaza un presupuesto (cambia su estado a Rechazado = 4).
        /// 
        /// Los presupuestos solo pueden ser rechazados si están en estado Emitido.
        /// Un presupuesto rechazado no puede ser convertido en factura.
        /// </summary>
        /// <param name="id">
        /// El GUID del presupuesto que se desea rechazar.
        /// </param>
        /// <returns>
        /// true si el presupuesto fue rechazado exitosamente;
        /// false si ocurrió un error (ej: presupuesto no encontrado, no está en estado Emitido).
        /// </returns>
        bool Rechazar(Guid id);

        /// <summary>
        /// Marca un presupuesto aprobado como facturado (cambia su estado a Facturado = 6).
        /// 
        /// Los presupuestos solo pueden ser facturados si están en estado Aprobado.
        /// Esta operación indica que el presupuesto ha sido convertido en una factura legal emitida
        /// y registrada en el sistema de facturación.
        /// </summary>
        /// <param name="id">
        /// El GUID del presupuesto que se desea marcar como facturado.
        /// </param>
        /// <returns>
        /// true si el presupuesto fue marcado como facturado exitosamente;
        /// false si ocurrió un error (ej: presupuesto no encontrado, no está en estado Aprobado).
        /// </returns>
        bool Facturar(Guid id);

        /// <summary>
        /// Copia un presupuesto existente, creando uno nuevo basado en el original.
        /// 
        /// Esta operación duplica todos los datos del presupuesto original (cliente, vendedor,
        /// detalles, condiciones) creando un nuevo presupuesto en estado Emitido con un número único.
        /// Es útil cuando se desea generar una nueva cotización similar a una anterior.
        /// </summary>
        /// <param name="idPresupuestoOriginal">
        /// El GUID del presupuesto que se desea copiar.
        /// </param>
        /// <returns>
        /// Un objeto PresupuestoDTO que representa el presupuesto copiado con datos idénticos al original
        /// pero con un ID y número único. Retorna null si el presupuesto original no existe.
        /// </returns>
        PresupuestoDTO Copiar(Guid idPresupuestoOriginal);
    }
}
