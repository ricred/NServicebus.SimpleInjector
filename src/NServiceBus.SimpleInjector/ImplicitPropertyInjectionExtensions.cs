using SimpleInjector;
using SimpleInjector.Advanced;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NServiceBus.SimpleInjector
{
    /// <summary>
    /// 
    /// </summary>
    public static class ImplicitPropertyInjectionExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        public static void AutoWirePropertiesImplicitly(this ContainerOptions options)
        {
            options.PropertySelectionBehavior = new ImplicitPropertyInjectionBehavior(
                options.PropertySelectionBehavior, options);
        }

        internal sealed class ImplicitPropertyInjectionBehavior
            : IPropertySelectionBehavior
        {
            private readonly IPropertySelectionBehavior core;
            private readonly ContainerOptions options;

            internal ImplicitPropertyInjectionBehavior(IPropertySelectionBehavior core,
                ContainerOptions options)
            {
                this.core = core;
                this.options = options;
            }

            public bool SelectProperty(Type type, PropertyInfo property)
            {
                return this.IsImplicitInjectable(property) ||
                    this.core.SelectProperty(type, property);
            }

            private bool IsImplicitInjectable(PropertyInfo property)
            {
                return IsInjectableProperty(property) &&
                    this.IsAvailableService(property.PropertyType);
            }

            private static bool IsInjectableProperty(PropertyInfo property)
            {
                MethodInfo setMethod = property.GetSetMethod(nonPublic: false);

                return setMethod != null && !setMethod.IsStatic && property.CanWrite;
            }

            private bool IsAvailableService(Type serviceType)
            {
                return this.options.Container.GetRegistration(serviceType) != null;
            }
        }
    }
}
