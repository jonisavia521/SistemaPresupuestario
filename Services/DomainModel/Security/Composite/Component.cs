using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DomainModel.Security.Composite
{
    public abstract class Component
    {
        public Guid IdComponent { get; set; }
        public abstract void Add(Component component);
        public abstract void Remove(Component component);
        /// <summary>
		/// Retorna la cantidad de hijos del elemento:
		/// Patente: 0
		/// Familia: >0
		/// </summary>
        public abstract int ChildrenCount();

        public abstract void Set(List<Component> components);
    }
}
