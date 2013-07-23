using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catwalk
{
    public sealed class ModelProperty
    {
        private List<ModelProperty> depends = new List<ModelProperty>();
        private readonly string name;
        private readonly Type type;
        private readonly Type owner;
        private readonly int hashCode;

        public ModelProperty(string name, Type type, Type owner)
        {
            if (null == name)
            {
                throw new ArgumentNullException("name");
            }

            if (null == type)
            {
                throw new ArgumentNullException("type");
            }

            if (null == owner)
            {
                throw new ArgumentNullException("owner");
            }

            this.name = name;
            this.type = type;
            this.owner = owner;

            //compute the hashcode once and cache it
            this.hashCode = this.name.GetHashCode() ^ this.owner.GetHashCode() ^ this.type.GetHashCode();
        }

        public override int GetHashCode()
        {
            return this.hashCode;
        }

        public override bool Equals(object obj)
        {
            var mp = obj as ModelProperty;
            if (null == mp)
            {
                return false;
            }

            return String.Equals(this.name, mp.name, StringComparison.Ordinal)
                && mp.type == this.type
                && mp.owner == this.owner;
        }

        public string Name { get { return this.name; } }
        public Type Type { get { return this.type; } }
        public Type Owner { get { return this.owner; } }

        public IEnumerable<ModelProperty> Dependencies
        {
            get
            {
                return this.depends.AsEnumerable();
            }
        }

        internal void AddDependency(ModelProperty prop)
        {
            if (!this.Dependencies.Contains(prop))
            {
                this.depends.Add(prop);
            }
        }
    }
}
