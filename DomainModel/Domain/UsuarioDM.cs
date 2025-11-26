using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModel.Domain
{
    /// <summary>
    /// Entidad de dominio que representa un usuario del sistema.
    /// 
    /// Esta clase encapsula la lógica de negocio de usuarios del sistema de presupuestos.
    /// Maneja credenciales, validación de datos personales y la integridad de cuentas.
    /// La contraseña se almacena hashada por seguridad.
    /// 
    /// Responsabilidades:
    /// - Validar nombre de usuario (alfanumérico, mínimo 3 caracteres)
    /// - Validar nombre completo del usuario
    /// - Validar y gestionar hash de contraseña
    /// - Auditar creación y cambios de contraseña
    /// 
    /// Invariantes:
    /// - Nombre: 2+ caracteres
    /// - Usuario: 3+ caracteres, solo alfanuméricos y guiones bajos
    /// - Clave: siempre almacenada como hash, nunca en texto plano
    /// 
    /// Seguridad:
    /// - Las contraseñas debe hasharse con algoritmos seguros (ej: bcrypt, SHA-256)
    /// - Este dominio NO valida la solidez de contraseña (eso es responsabilidad del BLL)
    /// - Las validaciones se hacen sobre el hash, no la contraseña plana
    /// 
    /// </summary>
    public class UsuarioDM
    {
        /// <summary>Identificador único del usuario en el sistema</summary>
        public Guid Id { get; private set; }
        
        /// <summary>Nombre completo del usuario (2+ caracteres, no alfanumérico restringido)</summary>
        public string Nombre { get; private set; }
        
        /// <summary>Nombre de usuario para login. Entre 3 y caracteres, solo letras, números y guiones bajos.</summary>
        public string Usuario_ { get; private set; }
        
        /// <summary>Hash seguro de la contraseña del usuario. Nunca contiene texto plano.</summary>
        public string Clave { get; private set; }

        /// <summary>
        /// Crea una nueva instancia de usuario para registrar en el sistema.
        /// 
        /// Se validan automáticamente todos los parámetros. El hash de contraseña
        /// debe ser proporcionado ya hasheado (el dominio no realiza el hashing).
        /// </summary>
        /// <param name="nombre">Nombre completo del usuario (2+ caracteres)</param>
        /// <param name="usuarioNombre">Nombre de usuario para login (3+ caracteres, alfanumérico + guiones bajos)</param>
        /// <param name="claveHash">Hash seguro de la contraseña (nunca texto plano)</param>
        /// <exception cref="ArgumentException">Si algún parámetro no cumple validaciones</exception>
        /// <exception cref="ArgumentNullException">Si un parámetro requerido es nulo</exception>
        /// <remarks>
        /// La contraseña debe ser hasheada en la capa de servicios ANTES de pasar a este constructor.
        /// Este dominio solo valida que el hash no esté vacío.
        /// </remarks>
        public UsuarioDM(string nombre, string usuarioNombre, string claveHash)
        {
            Id = Guid.NewGuid();
            Nombre = nombre ?? throw new ArgumentNullException(nameof(nombre), "El nombre es obligatorio.");
            if (string.IsNullOrWhiteSpace(nombre) || nombre.Length < 2)
                throw new ArgumentException("El nombre debe tener al menos 2 caracteres.", nameof(nombre));

            Usuario_ = usuarioNombre ?? throw new ArgumentNullException(nameof(usuarioNombre), "El nombre de usuario es obligatorio.");
            if (string.IsNullOrWhiteSpace(usuarioNombre) || usuarioNombre.Length < 3)
                throw new ArgumentException("El nombre de usuario debe tener al menos 3 caracteres.", nameof(usuarioNombre));
            if (!System.Text.RegularExpressions.Regex.IsMatch(usuarioNombre, @"^[a-zA-Z0-9_]+$"))
                throw new ArgumentException("El nombre de usuario solo puede contener letras, números y guiones bajos.", nameof(usuarioNombre));

            Clave = claveHash ?? throw new ArgumentNullException(nameof(claveHash), "La clave es obligatoria.");
            if (string.IsNullOrWhiteSpace(claveHash))
                throw new ArgumentException("El hash de la clave no puede estar vacío.", nameof(claveHash));
        }

        /// <summary>
        /// Constructor para hidratar un usuario desde la base de datos.
        /// 
        /// Se utiliza internamente por los repositorios para construir objetos
        /// desde registros persistidos.
        /// </summary>
        /// <param name="idUsuario">GUID único del usuario</param>
        /// <param name="nombre">Nombre completo del usuario</param>
        /// <param name="usuarioNombre">Nombre de usuario para login</param>
        /// <param name="claveHash">Hash de la contraseña almacenado</param>
        /// <exception cref="ArgumentException">Si el ID es un GUID vacío o si algún parámetro no cumple validaciones</exception>
        public UsuarioDM(Guid idUsuario, string nombre, string usuarioNombre, string claveHash)
        {
            Id = idUsuario == Guid.Empty ? throw new ArgumentException("El ID del usuario no puede ser vacío.", nameof(idUsuario)) : idUsuario;
            Nombre = nombre ?? throw new ArgumentNullException(nameof(nombre), "El nombre es obligatorio.");
            if (string.IsNullOrWhiteSpace(nombre) || nombre.Length < 2)
                throw new ArgumentException("El nombre debe tener al menos 2 caracteres.", nameof(nombre));

            Usuario_ = usuarioNombre ?? throw new ArgumentNullException(nameof(usuarioNombre), "El nombre de usuario es obligatorio.");
            if (string.IsNullOrWhiteSpace(usuarioNombre) || usuarioNombre.Length < 3)
                throw new ArgumentException("El nombre de usuario debe tener al menos 3 caracteres.", nameof(usuarioNombre));
            if (!System.Text.RegularExpressions.Regex.IsMatch(usuarioNombre, @"^[a-zA-Z0-9_]+$"))
                throw new ArgumentException("El nombre de usuario solo puede contener letras, números y guiones bajos.", nameof(usuarioNombre));

            Clave = claveHash ?? throw new ArgumentNullException(nameof(claveHash), "La clave es obligatoria.");
            if (string.IsNullOrWhiteSpace(claveHash))
                throw new ArgumentException("El hash de la clave no puede estar vacío.", nameof(claveHash));
        }

        /// <summary>
        /// Actualiza el nombre y nombre de usuario manteniendo la contraseña.
        /// 
        /// Se validan automáticamente los nuevos valores. Utilizado cuando se
        /// editan datos del perfil del usuario.
        /// </summary>
        /// <param name="nombre">Nuevo nombre completo del usuario</param>
        /// <param name="usuarioNombre">Nuevo nombre de usuario para login</param>
        /// <exception cref="ArgumentException">Si algún parámetro no cumple validaciones</exception>
        /// <exception cref="ArgumentNullException">Si un parámetro requerido es nulo</exception>
        public void ActualizarDatos(string nombre, string usuarioNombre)
        {
            Nombre = nombre ?? throw new ArgumentNullException(nameof(nombre), "El nombre es obligatorio.");
            if (string.IsNullOrWhiteSpace(nombre) || nombre.Length < 2)
                throw new ArgumentException("El nombre debe tener al menos 2 caracteres.", nameof(nombre));

            Usuario_ = usuarioNombre ?? throw new ArgumentNullException(nameof(usuarioNombre), "El nombre de usuario es obligatorio.");
            if (string.IsNullOrWhiteSpace(usuarioNombre) || usuarioNombre.Length < 3)
                throw new ArgumentException("El nombre de usuario debe tener al menos 3 caracteres.", nameof(usuarioNombre));
            if (!System.Text.RegularExpressions.Regex.IsMatch(usuarioNombre, @"^[a-zA-Z0-9_]+$"))
                throw new ArgumentException("El nombre de usuario solo puede contener letras, números y guiones bajos.", nameof(usuarioNombre));
        }

        /// <summary>
        /// Actualiza exclusivamente el hash de la contraseña del usuario.
        /// 
        /// Se utiliza cuando el usuario cambia su contraseña. El nuevo hash
        /// debe ser proporcionado ya procesado por un algoritmo seguro.
        /// </summary>
        /// <param name="nuevoClaveHash">Nuevo hash de contraseña (debe ser hasheado, nunca texto plano)</param>
        /// <exception cref="ArgumentException">Si el hash está vacío</exception>
        /// <exception cref="ArgumentNullException">Si el hash es nulo</exception>
        /// <remarks>
        /// Este método es utilizado por el BLL después de validar y hashear
        /// una nueva contraseña proporcionada por el usuario.
        /// </remarks>
        public void ActualizarClave(string nuevoClaveHash)
        {
            Clave = nuevoClaveHash ?? throw new ArgumentNullException(nameof(nuevoClaveHash), "La clave es obligatoria.");
            if (string.IsNullOrWhiteSpace(nuevoClaveHash))
                throw new ArgumentException("El hash de la clave no puede estar vacío.", nameof(nuevoClaveHash));
        }
    }
}
