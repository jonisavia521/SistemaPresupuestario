using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModel.Domain
{
    public class UsuarioDM
    {
        public Guid Id { get; private set; }
        public string Nombre { get; private set; }
        public string Usuario_ { get; private set; } // Renombrado para claridad (evitar confusión con la clase)
        public string Clave { get; private set; } // Almacena el hash de la clave

        // Constructor para creación inicial
        public UsuarioDM(string nombre, string usuarioNombre, string claveHash)
        {
            Id = Guid.NewGuid(); // Generar nuevo ID
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

        // Constructor para cargar desde la base de datos
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

        // Método para actualizar datos
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

        // Método para actualizar la clave
        public void ActualizarClave(string nuevoClaveHash)
        {
            Clave = nuevoClaveHash ?? throw new ArgumentNullException(nameof(nuevoClaveHash), "La clave es obligatoria.");
            if (string.IsNullOrWhiteSpace(nuevoClaveHash))
                throw new ArgumentException("El hash de la clave no puede estar vacío.", nameof(nuevoClaveHash));
        }
    }
}
