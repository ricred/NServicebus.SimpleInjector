using System;
using NUnit.Framework;
using NServiceBus;
using SimpleInjector.Lifestyles;

namespace NServiceBus.SimpleInjector.Tests
{
    class ExistingContainerConfigurationTests
    {
        [Test]
        public void Should_throw_argument_exception_if_not_allow_override()
        {
            var container = new global::SimpleInjector.Container();

            var endpointConfig = new EndpointConfiguration("Test");
            try
            {
                endpointConfig.UseContainer<SimpleInjectorBuilder>(customization =>
                {
                    customization.UseExistingContainer(container);
                });
            }
            catch (ArgumentException ex) when (ex.Message == "Invalid container configuration - must set Options.AllowOverridingRegistrations to true")
            {
                return;
            }

            Assert.Fail("Expected exception not thrown");
        }

        [Test]
        public void Should_throw_argument_exception_if_not_async_scoped()
        {
            var container = new global::SimpleInjector.Container();
            container.Options.AllowOverridingRegistrations = true;

            var endpointConfig = new EndpointConfiguration("Test");
            try
            {
                endpointConfig.UseContainer<SimpleInjectorBuilder>(customization =>
                {
                    customization.UseExistingContainer(container);
                });
            }
            catch (ArgumentException ex) when (ex.Message == "Invalid container configuration - must set DefaultScopedLifestyle to AsyncScopedLifestyle")
            {
                return;
            }

            Assert.Fail("Expected exception not thrown");
        }

        [Test]
        public void Should_throw_argument_exception_if_not_properties_autowired()
        {
            var container = new global::SimpleInjector.Container();
            container.Options.AllowOverridingRegistrations = true;
            container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

            var endpointConfig = new EndpointConfiguration("Test");
            try
            {
                endpointConfig.UseContainer<SimpleInjectorBuilder>(customization =>
                {
                    customization.UseExistingContainer(container);
                });
            }
            catch (ArgumentException ex) when (ex.Message == "Invalid container - must set properties to be autowired (by calling Options.AutoWirePropertiesImplicitly())")
            {
                return;
            }

            Assert.Fail("Expected exception not thrown");
        }

        public void Should_not_throw_when_configured_correctly()
        {
            var container = new global::SimpleInjector.Container();
            container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
            container.Options.AutoWirePropertiesImplicitly();
            container.Options.AllowOverridingRegistrations = true;

            var endpointConfig = new EndpointConfiguration("Test");
            endpointConfig.UseContainer<SimpleInjectorBuilder>(customization =>
            {
                customization.UseExistingContainer(container);
            });
        }
    }
}
