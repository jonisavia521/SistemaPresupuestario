using DAL.Implementation.EntityFramework;
using DomainModel.Contract;
using System;
using System.Data.Entity;

namespace DAL.Implementation.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SistemaPresupuestario _context;
        private DbContextTransaction _transaction;

        // Repositorios
        private IClienteRepository _clienteRepository;
        private IVendedorRepository _vendedorRepository;
        private IProductoRepository _productoRepository;
        private IPresupuestoRepository _presupuestoRepository;
        private IListaPrecioRepository _listaPrecioRepository;
        private IProvinciaRepository _provinciaRepository;
        private IConfiguracionRepository _configuracionRepository; // NUEVO

        public UnitOfWork(SistemaPresupuestario context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // Propiedades de repositorios con inicialización lazy
        public IClienteRepository ClienteRepository
        {
            get { return _clienteRepository ?? (_clienteRepository = new ClienteRepository(_context)); }
        }

        public IVendedorRepository VendedorRepository
        {
            get { return _vendedorRepository ?? (_vendedorRepository = new VendedorRepository(_context)); }
        }

        public IProductoRepository ProductoRepository
        {
            get { return _productoRepository ?? (_productoRepository = new ProductoRepository(_context)); }
        }

        public IPresupuestoRepository PresupuestoRepository
        {
            get { return _presupuestoRepository ?? (_presupuestoRepository = new PresupuestoRepository(_context)); }
        }

        public IListaPrecioRepository ListaPrecioRepository
        {
            get { return _listaPrecioRepository ?? (_listaPrecioRepository = new ListaPrecioRepository(_context)); }
        }

        public IProvinciaRepository ProvinciaRepository
        {
            get { return _provinciaRepository ?? (_provinciaRepository = new ProvinciaRepository(_context)); }
        }

        // NUEVO: Repositorio de Configuracion
        public IConfiguracionRepository ConfiguracionRepository
        {
            get { return _configuracionRepository ?? (_configuracionRepository = new ConfiguracionRepository(_context)); }
        }

        public void BeginTransaction()
        {
            _transaction = _context.Database.BeginTransaction();
        }

        public void Commit()
        {
            try
            {
                _context.SaveChanges();
                _transaction?.Commit();
            }
            catch
            {
                _transaction?.Rollback();
                throw;
            }
        }

        public void Rollback()
        {
            _transaction?.Rollback();
        }

        public int SaveChanges()
        {
            try
            {
                return _context.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context?.Dispose();
        }
    }
}
