using System;
using NServiceBus.SimpleInjector;
using SimpleInjector.Lifestyles;

namespace NServiceBus
{
    using NServiceBus.Container;

    /// <summary>
    /// Extension method class to allow configuration of existing container
    /// </summary>
    public static class ConfigurationExtensionMethods
    {
        /// <summary>
        /// Use the a pre-configured SimpleInjector contains
        /// </summary>
        /// <param name="customizations"></param>
        /// <param name="container">The existing container to use.</param>
        public static void UseExistingContainer(this ContainerCustomizations customizations, global::SimpleInjector.Container container)
        {
            if (!container.Options.AllowOverridingRegistrations)
                throw new ArgumentException("Invalid container configuration - must set Options.AllowOverridingRegistrations to true");
            if (!(container.Options.DefaultScopedLifestyle is AsyncScopedLifestyle))
                throw new ArgumentException("Invalid container configuration - must set DefaultScopedLifestyle to AsyncScopedLifestyle");
            if (!(container.Options.PropertySelectionBehavior is ImplicitPropertyInjectionExtensions.ImplicitPropertyInjectionBehavior))
                throw new ArgumentException("Invalid container - must set properties to be autowired (by calling Options.AutoWirePropertiesImplicitly())");

            customizations.Settings.Set<ContainerHolder>(new ContainerHolder(container));
        }
    }
}