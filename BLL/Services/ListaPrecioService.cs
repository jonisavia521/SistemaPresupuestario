using AutoMapper;
using BLL.Contracts;
using BLL.DTOs;
using DomainModel.Contract;
using DomainModel.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BLL.Services
{
    /// <summary>
    /// Servicio de lógica de negocio para Lista de Precios
    /// Maneja la orquestación entre UI, dominio y persistencia
    /// IMPORTANTE: Este servicio NO debe referenciar clases concretas de DAL
    /// </summary>
    public class ListaPrecioService : IListaPrecioService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ListaPrecioService(
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public ListaPrecioDTO GetById(Guid id)
        {
            try
            {
                var listaPrecioObj = _unitOfWork.ListaPrecioRepository.GetByIdWithDetails(id);
                if (listaPrecioObj == null)
                    return null;

                return MapearDesdeEF(listaPrecioObj);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener lista de precios: {ex.Message}", ex);
            }
        }

        public ListaPrecioDTO GetByCodigo(string codigo)
        {
            try
            {
                var listaPrecioObj = _unitOfWork.ListaPrecioRepository.GetByCodigo(codigo);
                if (listaPrecioObj == null)
                    return null;

                return MapearDesdeEF(listaPrecioObj);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener lista de precios por código: {ex.Message}", ex);
            }
        }

        public IEnumerable<ListaPrecioDTO> GetAll()
        {
            try
            {
                var listasObj = _unitOfWork.ListaPrecioRepository.GetAllWithDetails();
                var listasDTOs = new List<ListaPrecioDTO>();

                foreach (var listaObj in listasObj)
                {
                    listasDTOs.Add(MapearDesdeEF(listaObj));
                }

                return listasDTOs;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener listas de precios: {ex.Message}", ex);
            }
        }

        public IEnumerable<ListaPrecioDTO> GetActivas()
        {
            try
            {
                var listasObj = _unitOfWork.ListaPrecioRepository.GetActivas();
                var listasDTOs = new List<ListaPrecioDTO>();

                foreach (var listaObj in listasObj)
                {
                    listasDTOs.Add(MapearDesdeEF(listaObj));
                }

                return listasDTOs;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener listas de precios activas: {ex.Message}", ex);
            }
        }

        public bool Add(ListaPrecioDTO listaPrecioDTO)
        {
            try
            {
                // Validar que no exista el código
                var existe = _unitOfWork.ListaPrecioRepository.ExisteCodigo(listaPrecioDTO.Codigo);
                if (existe)
                    throw new InvalidOperationException($"Ya existe una lista de precios con el código '{listaPrecioDTO.Codigo}'");

                // Convertir DTO a dominio para validaciones
                var domainModel = new ListaPrecioDM(
                    listaPrecioDTO.Codigo,
                    listaPrecioDTO.Nombre,
                    listaPrecioDTO.IncluyeIva
                );

                // Agregar detalles al dominio
                if (listaPrecioDTO.Detalles != null)
                {
                    foreach (var detalleDTO in listaPrecioDTO.Detalles)
                    {
                        var detalleDM = new ListaPrecioDetalleDM(
                            detalleDTO.IdProducto,
                            detalleDTO.Precio
                        );
                        domainModel.AgregarDetalle(detalleDM);
                    }
                }

                // Validar lógica de negocio
                domainModel.ValidarNegocio();

                // Crear entidad EF usando reflection
                var listaPrecioEF = CrearEntidadListaPrecio(domainModel);

                // Guardar usando IUnitOfWork
                _unitOfWork.ListaPrecioRepository.Add(listaPrecioEF);
                _unitOfWork.SaveChanges();

                // Actualizar el ID en el DTO
                listaPrecioDTO.Id = domainModel.Id;

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al crear lista de precios: {ex.Message}", ex);
            }
        }

        public bool Update(ListaPrecioDTO listaPrecioDTO)
        {
            try
            {
                // Obtener la lista existente
                var listaPrecioExistenteObj = _unitOfWork.ListaPrecioRepository.GetByIdWithDetails(listaPrecioDTO.Id);
                if (listaPrecioExistenteObj == null)
                    throw new InvalidOperationException("Lista de precios no encontrada");

                // Validar que no exista el código en otro registro
                var existe = _unitOfWork.ListaPrecioRepository.ExisteCodigo(listaPrecioDTO.Codigo, listaPrecioDTO.Id);
                if (existe)
                    throw new InvalidOperationException($"Ya existe otra lista de precios con el código '{listaPrecioDTO.Codigo}'");

                // Convertir DTO a dominio para validaciones
                var domainModel = new ListaPrecioDM(
                    listaPrecioDTO.Id,
                    listaPrecioDTO.Codigo,
                    listaPrecioDTO.Nombre,
                    listaPrecioDTO.Activo,
                    listaPrecioDTO.FechaAlta,
                    listaPrecioDTO.FechaModificacion,
                    null,
                    listaPrecioDTO.IncluyeIva
                );

                // Limpiar y agregar detalles nuevos
                domainModel.LimpiarDetalles();
                if (listaPrecioDTO.Detalles != null)
                {
                    foreach (var detalleDTO in listaPrecioDTO.Detalles)
                    {
                        var detalleDM = new ListaPrecioDetalleDM(
                            detalleDTO.IdProducto,
                            detalleDTO.Precio
                        );
                        domainModel.AgregarDetalle(detalleDM);
                    }
                }

                // Validar lógica de negocio
                domainModel.ValidarNegocio();

                // Actualizar la entidad EF usando reflection
                ActualizarEntidadListaPrecio(listaPrecioExistenteObj, domainModel);

                // Guardar cambios
                _unitOfWork.ListaPrecioRepository.Update(listaPrecioExistenteObj);
                _unitOfWork.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al actualizar lista de precios: {ex.Message}", ex);
            }
        }

        public bool Delete(Guid id)
        {
            try
            {
                // Obtener la lista existente
                var listaPrecioObj = _unitOfWork.ListaPrecioRepository.GetByIdWithDetails(id);
                if (listaPrecioObj == null)
                    throw new InvalidOperationException("Lista de precios no encontrada");

                // Realizar eliminación lógica usando reflection
                var tipo = listaPrecioObj.GetType();
                tipo.GetProperty("Activo").SetValue(listaPrecioObj, false);
                tipo.GetProperty("FechaModificacion").SetValue(listaPrecioObj, DateTime.Now);

                // Guardar cambios
                _unitOfWork.ListaPrecioRepository.Update(listaPrecioObj);
                _unitOfWork.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al desactivar lista de precios: {ex.Message}", ex);
            }
        }

        public bool Reactivar(Guid id)
        {
            try
            {
                // Obtener la lista existente
                var listaPrecioObj = _unitOfWork.ListaPrecioRepository.GetByIdWithDetails(id);
                if (listaPrecioObj == null)
                    throw new InvalidOperationException("Lista de precios no encontrada");

                // Reactivar usando reflection
                var tipo = listaPrecioObj.GetType();
                tipo.GetProperty("Activo").SetValue(listaPrecioObj, true);
                tipo.GetProperty("FechaModificacion").SetValue(listaPrecioObj, DateTime.Now);

                // Guardar cambios
                _unitOfWork.ListaPrecioRepository.Update(listaPrecioObj);
                _unitOfWork.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al reactivar lista de precios: {ex.Message}", ex);
            }
        }

        public bool ExisteCodigo(string codigo, Guid? excludeId = null)
        {
            return _unitOfWork.ListaPrecioRepository.ExisteCodigo(codigo, excludeId);
        }

        public decimal? ObtenerPrecioProducto(Guid idListaPrecio, Guid idProducto)
        {
            return _unitOfWork.ListaPrecioRepository.ObtenerPrecioProducto(idListaPrecio, idProducto);
        }

        // Métodos privados para mapeo usando reflection
        private ListaPrecioDTO MapearDesdeEF(object listaPrecioEF)
        {
            var tipo = listaPrecioEF.GetType();

            var dto = new ListaPrecioDTO
            {
                Id = (Guid)tipo.GetProperty("ID").GetValue(listaPrecioEF),
                Codigo = (string)tipo.GetProperty("Codigo").GetValue(listaPrecioEF),
                Nombre = (string)tipo.GetProperty("Nombre").GetValue(listaPrecioEF),
                Activo = (bool)tipo.GetProperty("Activo").GetValue(listaPrecioEF),
                IncluyeIva = (bool)tipo.GetProperty("IncluyeIva").GetValue(listaPrecioEF),
                FechaAlta = (DateTime)tipo.GetProperty("FechaAlta").GetValue(listaPrecioEF),
                FechaModificacion = (DateTime?)tipo.GetProperty("FechaModificacion").GetValue(listaPrecioEF)
            };

            // Mapear detalles
            var detallesEF = tipo.GetProperty("ListaPrecio_Detalle").GetValue(listaPrecioEF);
            if (detallesEF != null)
            {
                var detallesLista = (System.Collections.IEnumerable)detallesEF;
                foreach (var detalleEF in detallesLista)
                {
                    dto.Detalles.Add(MapearDetalleDesdeEF(detalleEF));
                }
            }

            return dto;
        }

        private ListaPrecioDetalleDTO MapearDetalleDesdeEF(object detalleEF)
        {
            var tipo = detalleEF.GetType();

            var dto = new ListaPrecioDetalleDTO
            {
                Id = (Guid)tipo.GetProperty("ID").GetValue(detalleEF),
                IdListaPrecio = (Guid)tipo.GetProperty("IdListaPrecio").GetValue(detalleEF),
                IdProducto = (Guid)tipo.GetProperty("IdProducto").GetValue(detalleEF),
                Precio = (decimal)tipo.GetProperty("Precio").GetValue(detalleEF)
            };

            // Obtener producto si existe
            var productoEF = tipo.GetProperty("Producto").GetValue(detalleEF);
            if (productoEF != null)
            {
                var tipoProducto = productoEF.GetType();
                dto.Codigo = (string)tipoProducto.GetProperty("Codigo").GetValue(productoEF);
                dto.Descripcion = (string)tipoProducto.GetProperty("Descripcion").GetValue(productoEF);
            }

            return dto;
        }

        private object CrearEntidadListaPrecio(ListaPrecioDM domainModel)
        {
            // Usar reflection para crear la instancia sin referenciar DAL
            var assembly = Assembly.Load("DAL");
            var tipoListaPrecio = assembly.GetType("DAL.Implementation.EntityFramework.ListaPrecio");
            var tipoDetalle = assembly.GetType("DAL.Implementation.EntityFramework.ListaPrecio_Detalle");

            // Crear instancia de ListaPrecio
            var listaPrecioEF = Activator.CreateInstance(tipoListaPrecio);

            // Establecer propiedades
            tipoListaPrecio.GetProperty("ID").SetValue(listaPrecioEF, domainModel.Id);
            tipoListaPrecio.GetProperty("Codigo").SetValue(listaPrecioEF, domainModel.Codigo);
            tipoListaPrecio.GetProperty("Nombre").SetValue(listaPrecioEF, domainModel.Nombre);
            tipoListaPrecio.GetProperty("Activo").SetValue(listaPrecioEF, domainModel.Activo);
            tipoListaPrecio.GetProperty("IncluyeIva").SetValue(listaPrecioEF, domainModel.IncluyeIva);
            tipoListaPrecio.GetProperty("FechaAlta").SetValue(listaPrecioEF, domainModel.FechaAlta);

            // Crear colección de detalles
            var detallesEF = Activator.CreateInstance(typeof(List<>).MakeGenericType(tipoDetalle));
            var addMethod = detallesEF.GetType().GetMethod("Add");

            foreach (var detalleDM in domainModel.Detalles)
            {
                var detalleEF = Activator.CreateInstance(tipoDetalle);
                tipoDetalle.GetProperty("ID").SetValue(detalleEF, detalleDM.Id);
                tipoDetalle.GetProperty("IdListaPrecio").SetValue(detalleEF, domainModel.Id);
                tipoDetalle.GetProperty("IdProducto").SetValue(detalleEF, detalleDM.IdProducto);
                tipoDetalle.GetProperty("Precio").SetValue(detalleEF, detalleDM.Precio);

                addMethod.Invoke(detallesEF, new[] { detalleEF });
            }

            tipoListaPrecio.GetProperty("ListaPrecio_Detalle").SetValue(listaPrecioEF, detallesEF);

            return listaPrecioEF;
        }

        private void ActualizarEntidadListaPrecio(object listaPrecioEF, ListaPrecioDM domainModel)
        {
            var tipo = listaPrecioEF.GetType();

            // Actualizar propiedades principales
            tipo.GetProperty("Codigo").SetValue(listaPrecioEF, domainModel.Codigo);
            tipo.GetProperty("Nombre").SetValue(listaPrecioEF, domainModel.Nombre);
            tipo.GetProperty("Activo").SetValue(listaPrecioEF, domainModel.Activo);
            tipo.GetProperty("IncluyeIva").SetValue(listaPrecioEF, domainModel.IncluyeIva);
            tipo.GetProperty("FechaModificacion").SetValue(listaPrecioEF, DateTime.Now);

            // Obtener la colección actual de detalles
            var detallesEF = tipo.GetProperty("ListaPrecio_Detalle").GetValue(listaPrecioEF);
            
            // Convertir a lista para poder iterar de forma segura
            var detallesActuales = new List<object>();
            foreach (var detalle in (System.Collections.IEnumerable)detallesEF)
            {
                detallesActuales.Add(detalle);
            }

            // Obtener el contexto de Entity Framework desde la colección
            var assembly = Assembly.Load("DAL");
            var tipoDetalle = assembly.GetType("DAL.Implementation.EntityFramework.ListaPrecio_Detalle");
            var removeMethod = detallesEF.GetType().GetMethod("Remove");

            // Remover cada detalle existente de la colección
            // Entity Framework los marcará como "Deleted" en el contexto
            foreach (var detalleActual in detallesActuales)
            {
                removeMethod.Invoke(detallesEF, new[] { detalleActual });
            }

            // Agregar los nuevos detalles
            var addMethod = detallesEF.GetType().GetMethod("Add");

            foreach (var detalleDM in domainModel.Detalles)
            {
                // Crear nuevo detalle
                var detalleEF = Activator.CreateInstance(tipoDetalle);
                
                // Establecer propiedades del detalle
                tipoDetalle.GetProperty("ID").SetValue(detalleEF, Guid.NewGuid()); // Nuevo ID para cada detalle
                tipoDetalle.GetProperty("IdListaPrecio").SetValue(detalleEF, domainModel.Id);
                tipoDetalle.GetProperty("IdProducto").SetValue(detalleEF, detalleDM.IdProducto);
                tipoDetalle.GetProperty("Precio").SetValue(detalleEF, detalleDM.Precio);

                // CRÍTICO: Establecer la referencia a la entidad padre
                // Esto asegura que Entity Framework reconozca la relación correctamente
                var propListaPrecio = tipoDetalle.GetProperty("ListaPrecio");
                if (propListaPrecio != null)
                {
                    propListaPrecio.SetValue(detalleEF, listaPrecioEF);
                }

                // Agregar a la colección
                addMethod.Invoke(detallesEF, new[] { detalleEF });
            }
        }
    }
}
