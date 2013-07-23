using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Catwalk
{
    internal sealed class ObservablePropertyExpressionVisitor : ExpressionVisitor
    {
        //public properties
        public Action NotifyAction { get; set; }
        public IList<ModelProperty> Properties { get; private set; }

        //private variables
        private IList<INotifyPropertyChanged> observed = new List<INotifyPropertyChanged>();
        private Type declaringType;

        public ObservablePropertyExpressionVisitor(Type declaringType)
        {
            if (null == declaringType)
            {
                throw new ArgumentNullException("declaringType");
            }

            this.declaringType = declaringType;
            this.Properties = new List<ModelProperty>();
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            //filter for Properties only, skip all fields
            if (node.Member is PropertyInfo)
            {
                //does the owner of the property implement INotifyPropertyChanged?
                var typeInfo = node.Member.DeclaringType.GetTypeInfo();
                if (typeInfo.ImplementedInterfaces.Contains(typeof(INotifyPropertyChanged)))
                {
                    //is the owner, the type that invoked the search
                    if (node.Member.DeclaringType == this.declaringType)
                    {
                        this.Properties.Add(new ModelProperty(node.Member.Name, node.Type, node.Member.DeclaringType));
                    }
                    else
                    {
                        //define our property changed event handler
                        PropertyChangedEventHandler onPropertyChanged = (sender, e) =>
                        {
                            if (e.PropertyName == node.Member.Name)
                            {
                                if (null != NotifyAction)
                                {
                                    NotifyAction();
                                }
                            }
                        };

                        //Get a reference to the owner instance by creating a new lambda expression
                        var lam = Expression.Lambda<Func<INotifyPropertyChanged>>(node.Expression);
                        var src = lam.Compile()();

                        //add the event handler
                        src.PropertyChanged += onPropertyChanged;

                        //todo: figure out how to remove the event handler so we don't leak. Should
                        //probably make this a weak event listener, but weak events seem to be missing from PCL.
                    }
                }

            }

            return base.VisitMember(node);
        }
    }
}
