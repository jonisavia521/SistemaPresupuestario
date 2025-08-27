using DAL.Implementation.EntityFramework.Context;
using DAL.Repository.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        readonly SistemaPresupuestarioContext _context;
        //private IDbContextTransaction _transaction;
        public UnitOfWork(SistemaPresupuestarioContext context)
        {
            this._context = context;
        }

        public void BeginTransaction()
        {
            //_transaction = _context.Database.BeginTransaction();
        }

        public void Commit()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Rollback()
        {
            throw new NotImplementedException();
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
    }
}
