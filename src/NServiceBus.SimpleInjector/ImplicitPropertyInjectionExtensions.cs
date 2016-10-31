namespace NServiceBus.SimpleInjector
{
    using global::SimpleInjector;
    using global::SimpleInjector.Advanced;
    using System;
    using System.Reflection;

    /// <summary>
    /// Extension methods for configuring property injection on a SimpleInjector container
    /// </summary>
    public static class ImplicitPropertyInjectionExtensions
    {
        /// <summary>
        /// Configures the options object's PropertySelectionBehaviour to use Implicit property injection
        /// </summary>
        /// <param name="options"></param>
        public static void AutoWirePropertiesImplicitly(this ContainerOptions options)
        {
            options.PropertySelectionBehavior = new ImplicitPropertyInjectionBehavior(options.PropertySelectionBehavior, options);
        }

        internal sealed class ImplicitPropertyInjectionBehavior
            : IPropertySelectionBehavior
        {
            private readonly IPropertySelectionBehavior core;
            private readonly ContainerOptions options;

            internal ImplicitPropertyInjectionBehavior(IPropertySelectionBehavior core, ContainerOptions options)
            {
                this.core = core;
                this.options = options;
            }

            public bool SelectProperty(Type type, PropertyInfo property)
            {
                return IsImplicitInjectable(property) || core.SelectProperty(type, property);
            }

            private bool IsImplicitInjectable(PropertyInfo property)
            {
                return IsInjectableProperty(property) && IsAvailableService(property.PropertyType);
            }

            private static bool IsInjectableProperty(PropertyInfo property)
            {
                var setMethod = property.GetSetMethod(nonPublic: false);

                return setMethod != null && !setMethod.IsStatic && property.CanWrite;
            }

            private bool IsAvailableService(Type serviceType)
            {
                return options.Container.GetRegistration(serviceType) != null;
            }
        }
    }
}