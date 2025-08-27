using Services.DAL.Contracts;
using Services.DomainModel.Security.Composite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DAL.Implementations.Adapter
{

    internal sealed class PatenteAdapter : IAdapter<Patente>
    {

        public Patente Adapt(DataRow row)
        {
            //Hidratar el objeto patente
            Patente patente = new Patente()
            {
                IdComponent = row.Field<Guid>("IdPatente"),
                MenuItemName = row.Field<string>("Nombre"),
                FormName = row.Field<string>("Vista")
            };
            return patente;
        }
    }

}
