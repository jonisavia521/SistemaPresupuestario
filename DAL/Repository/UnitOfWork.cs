using DAL.Implementation.EntityFramework.Context;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repository
{
    public class UnitOfWork //: IUnitOfWork
    {
        readonly SistemaPresupuestarioContext _context;
        public UnitOfWork(SistemaPresupuestarioContext context)
        {
            this._context = context;
        }
        public Task<int> SaveChanges()
        {
            try
            {
                return _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
            
        }
    }
}
