using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DomainModel.Security.Composite
{
    public class Patente: Component
    {
        public string FormName { get; set; }

        public string MenuItemName { get; set; }

        public override void Add(Component component)
        {
            throw new Exception("No se pueden agregar elementos sobre primitivos");
        }

        public override int ChildrenCount()
        {
            return 0;
        }

        public override void Remove(Component component)
        {
            throw new Exception("No se pueden quitar elementos sobre primitivos");
        }

        public override void Set(List<Component> components)
        {
            throw new Exception("No se pueden cargar un grupo de elementos sobre primitivos");
        }
    }
}
