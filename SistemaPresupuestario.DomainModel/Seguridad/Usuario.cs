using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SistemaPresupuestario.DomainModel.Seguridad.Base;

namespace SistemaPresupuestario.DomainModel.Seguridad
{
    /// <summary>
    /// Represents a user in the system with assigned families and patents
    /// </summary>
    public class Usuario : EntityBase
    {
        /// <summary>
        /// Display name of the user
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; }

        /// <summary>
        /// Unique username for login
        /// </summary>
        [Required]
        [MaxLength(50)]
        [Index("IX_Usuario_Username", IsUnique = true)]
        public string NombreUsuario { get; set; }

        /// <summary>
        /// Hashed password
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string ClaveHash { get; set; }

        /// <summary>
        /// Salt used for password hashing
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Salt { get; set; }

        /// <summary>
        /// Whether user must change password on next login
        /// </summary>
        public bool DebeRenovarClave { get; set; }

        /// <summary>
        /// Whether the user account is active
        /// </summary>
        public bool Activo { get; set; }

        /// <summary>
        /// Last login timestamp
        /// </summary>
        public DateTime? UltimoLogin { get; set; }

        /// <summary>
        /// Number of failed login attempts
        /// </summary>
        public int IntentosLoginFallidos { get; set; }

        /// <summary>
        /// When the account was locked due to failed attempts
        /// </summary>
        public DateTime? CuentaBloqueadaHasta { get; set; }

        /// <summary>
        /// Email address for notifications
        /// </summary>
        [MaxLength(255)]
        public string Email { get; set; }

        /// <summary>
        /// Families directly assigned to this user
        /// </summary>
        public virtual ICollection<Familia> Familias { get; set; }

        /// <summary>
        /// Patents directly assigned to this user
        /// </summary>
        public virtual ICollection<Patente> Patentes { get; set; }

        public Usuario()
        {
            Familias = new HashSet<Familia>();
            Patentes = new HashSet<Patente>();
            Activo = true;
            DebeRenovarClave = false;
            IntentosLoginFallidos = 0;
        }

        public Usuario(string nombre, string nombreUsuario, string claveHash, string salt) : this()
        {
            Nombre = nombre ?? throw new ArgumentNullException(nameof(nombre));
            NombreUsuario = nombreUsuario ?? throw new ArgumentNullException(nameof(nombreUsuario));
            ClaveHash = claveHash ?? throw new ArgumentNullException(nameof(claveHash));
            Salt = salt ?? throw new ArgumentNullException(nameof(salt));
        }

        #region Permission Management

        /// <summary>
        /// Gets all effective patents for this user (direct patents + patents from families)
        /// </summary>
        public IEnumerable<Patente> ObtenerPatentesEfectivas()
        {
            var patentesEfectivas = new HashSet<Patente>();

            // Add direct patents
            if (Patentes != null)
            {
                foreach (var patente in Patentes)
                {
                    patentesEfectivas.Add(patente);
                }
            }

            // Add patents from families (including inherited from child families)
            if (Familias != null)
            {
                foreach (var familia in Familias)
                {
                    foreach (var patente in familia.ObtenerPatentesEfectivas())
                    {
                        patentesEfectivas.Add(patente);
                    }
                }
            }

            return patentesEfectivas;
        }

        /// <summary>
        /// Gets all direct patents assigned to this user
        /// </summary>
        public IEnumerable<Patente> ObtenerPatentesDirectas()
        {
            return Patentes?.ToList() ?? Enumerable.Empty<Patente>();
        }

        /// <summary>
        /// Gets all patents inherited from families (not direct assignments)
        /// </summary>
        public IEnumerable<Patente> ObtenerPatentesHeredadas()
        {
            var patentesDirectas = new HashSet<Patente>(ObtenerPatentesDirectas());
            var patentesEfectivas = ObtenerPatentesEfectivas();

            return patentesEfectivas.Where(p => !patentesDirectas.Contains(p));
        }

        /// <summary>
        /// Checks if user has a specific patent (direct or inherited)
        /// </summary>
        public bool TienePatente(string nombrePatente)
        {
            return ObtenerPatentesEfectivas().Any(p => 
                string.Equals(p.Nombre, nombrePatente, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Checks if user has a specific patent by ID
        /// </summary>
        public bool TienePatente(Guid patenteId)
        {
            return ObtenerPatentesEfectivas().Any(p => p.Id == patenteId);
        }

        /// <summary>
        /// Assigns a family to this user
        /// </summary>
        public void AsignarFamilia(Familia familia)
        {
            if (familia == null)
                throw new ArgumentNullException(nameof(familia));

            if (!Familias.Contains(familia))
            {
                Familias.Add(familia);
            }
        }

        /// <summary>
        /// Removes a family from this user
        /// </summary>
        public void DesasignarFamilia(Familia familia)
        {
            if (familia != null)
            {
                Familias.Remove(familia);
            }
        }

        /// <summary>
        /// Assigns a patent directly to this user
        /// </summary>
        public void AsignarPatente(Patente patente)
        {
            if (patente == null)
                throw new ArgumentNullException(nameof(patente));

            if (!Patentes.Contains(patente))
            {
                Patentes.Add(patente);
            }
        }

        /// <summary>
        /// Removes a direct patent assignment from this user
        /// </summary>
        public void DesasignarPatente(Patente patente)
        {
            if (patente != null)
            {
                Patentes.Remove(patente);
            }
        }

        /// <summary>
        /// Validates that user has at least one permission (family or direct patent)
        /// </summary>
        public bool TienePermisos()
        {
            return (Familias?.Any() == true) || (Patentes?.Any() == true);
        }

        #endregion

        #region Account Management

        /// <summary>
        /// Checks if account is currently locked
        /// </summary>
        public bool EstaBloqueado()
        {
            return CuentaBloqueadaHasta.HasValue && CuentaBloqueadaHasta.Value > DateTime.Now;
        }

        /// <summary>
        /// Locks the account for a specified duration
        /// </summary>
        public void BloquearCuenta(TimeSpan duracion)
        {
            CuentaBloqueadaHasta = DateTime.Now.Add(duracion);
        }

        /// <summary>
        /// Unlocks the account
        /// </summary>
        public void DesbloquearCuenta()
        {
            CuentaBloqueadaHasta = null;
            IntentosLoginFallidos = 0;
        }

        /// <summary>
        /// Records a successful login
        /// </summary>
        public void RegistrarLoginExitoso()
        {
            UltimoLogin = DateTime.Now;
            IntentosLoginFallidos = 0;
            CuentaBloqueadaHasta = null;
        }

        /// <summary>
        /// Records a failed login attempt
        /// </summary>
        public void RegistrarLoginFallido()
        {
            IntentosLoginFallidos++;
        }

        /// <summary>
        /// Updates the password hash and salt
        /// </summary>
        public void ActualizarClave(string nuevoHash, string nuevoSalt, bool debeRenovar = false)
        {
            ClaveHash = nuevoHash ?? throw new ArgumentNullException(nameof(nuevoHash));
            Salt = nuevoSalt ?? throw new ArgumentNullException(nameof(nuevoSalt));
            DebeRenovarClave = debeRenovar;
            ModifiedAt = DateTime.Now;
        }

        #endregion

        public override string ToString()
        {
            return $"{Nombre} ({NombreUsuario})";
        }

        public override bool Equals(object obj)
        {
            if (obj is Usuario other)
                return Id == other.Id;
            return false;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}