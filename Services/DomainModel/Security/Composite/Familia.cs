using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DomainModel.Security.Composite
{
    public class Familia:Component
    {
        private List<Component> childrens = new List<Component>();

        public Familia()
        {

        }
        public Familia(Component component, string nombre)
        {
            childrens.Add(component);
            Nombre = nombre;
        }

        public string Nombre { get; set; }


        public List<Component> GetChildrens()
        {
            return childrens;
        }
        /// 
        /// <param name="component"></param>
        public override void Add(Component component)
        {
            //Validar que no tenga referencias circulares...
            childrens.Add(component);
        }

        public override int ChildrenCount()
        {
            return childrens.Count;
        }

        /// 
        /// <param name="component"></param>
        public override void Remove(Component component)
        {
            childrens.RemoveAll(o => o.IdComponent == component.IdComponent);
        }

        public override void Set(List<Component> components)
        {
            this.childrens.AddRange(components);            
        }
    }
}
