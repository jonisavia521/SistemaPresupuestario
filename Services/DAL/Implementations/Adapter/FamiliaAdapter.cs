using Services.DAL.Contracts;
using Services.DAL.Factory;
using Services.DAL.Implementations.Joins;
using Services.DomainModel.Security.Composite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DAL.Implementations.Adapter
{

    internal sealed class FamiliaAdapter : IAdapter<Familia>
    {

        //IJoinRepository<Familia> _familiaFamiliaRepository;
        //IJoinRepository<Familia> _familiaPatenteRepository;
        public FamiliaAdapter( /*IEnumerable<IJoinRepository<Familia>> iJoinRepository*/)
        {
            //_familiaFamiliaRepository = iJoinRepository.SingleOrDefault(s => s.GetType() == typeof(FamiliaFamiliaRepository)); 
            //_familiaPatenteRepository =  iJoinRepository.SingleOrDefault(s => s.GetType() == typeof(FamiliaPatenteRepository)); 
          
        }
      
        public Familia Adapt(DataRow row)
        {
            //Hidratar el objeto familia -> Nivel 1
            //Familia familia = new Familia()
            //{
            //    IdComponent = Guid.Parse(values[0].ToString()),
            //    Nombre = values[1].ToString()
            //};
            Familia familia = new Familia { 
                IdComponent = new Guid(row.Field<string>("IdFamilia")), 
                Nombre = row.Field<string>("Nombre") 
            };

            //Nivel 2 de hidratación...
            LoginFactory.familiaFamiliaRepository.GetChildren(familia);
            LoginFactory.familiaPatenteRepository.GetChildren(familia);
           
            return familia;
        }
    }
}
