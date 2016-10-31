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
            customizations.Settings.Set<ContainerHolder>(new ContainerHolder(container));
        }
    }
}