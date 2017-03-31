using NServiceBus.SimpleInjector;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NServiceBus.ObjectBuilder
{
    /// <summary>
    /// Extension methods to allow cloning of a SimpleInjector container
    /// </summary>
    public static class ContainerExtensionMethods
    {
        /// <summary>
        /// Creates a clone of an existing container by copying all the registrations.
        /// </summary>
        /// <param name="parentContainer">The container to clone</param>
        /// <returns>A new container with all the same registrations</returns>
        public static global::SimpleInjector.Container Clone(this global::SimpleInjector.Container parentContainer)
        {
            var clonedContainer = new global::SimpleInjector.Container();
            clonedContainer.AllowToResolveArraysAndLists();
            clonedContainer.Options.AllowOverridingRegistrations = true;
            clonedContainer.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
            clonedContainer.Options.AutoWirePropertiesImplicitly();

            AsyncScopedLifestyle.BeginScope(clonedContainer);

            foreach (var reg in parentContainer.GetCurrentRegistrations())
            {
                if (reg.Lifestyle == Lifestyle.Singleton && !HasComponent(clonedContainer, reg.ServiceType))
                {
                    clonedContainer.Register(reg.ServiceType, reg.GetInstance, reg.Lifestyle);
                }
                else
                {
                    var registration = RegistrationOptions(reg, clonedContainer).First(r => r != null);
                    clonedContainer.AddRegistration(reg.ServiceType, registration);
                }
            }

            return clonedContainer;
        }

        static IEnumerable<Registration> RegistrationOptions(InstanceProducer registrationToCopy, global::SimpleInjector.Container container)
        {
            yield return CreateRegistrationFromPrivateField(registrationToCopy, container, "instanceCreator");
            yield return CreateRegistrationFromPrivateField(registrationToCopy, container, "userSuppliedInstanceCreator");
            yield return registrationToCopy.Lifestyle.CreateRegistration(registrationToCopy.ServiceType, container);
        }

        static Registration CreateRegistrationFromPrivateField(InstanceProducer instanceProducer, global::SimpleInjector.Container container, string privateFieldName)
        {
            var instanceCreator = (Func<object>)GetPrivateField(instanceProducer.Registration, privateFieldName);

            if (instanceCreator != null)
            {
                return instanceProducer.Lifestyle.CreateRegistration(instanceProducer.ServiceType, instanceCreator, container);
            }

            return null;
        }

        static object GetPrivateField(object obj, string fieldName)
        {
            var field = obj.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);
            var fieldValue = field?.GetValue(obj);

            return fieldValue;
        }

        static bool HasComponent(global::SimpleInjector.Container container, Type componentType)
        {
            return container.GetCurrentRegistrations().Any(r => r.ServiceType == componentType);
        }

        /// <summary>
        /// Checks if 2 registrations have the same lifestyle and implementation type
        /// </summary>
        /// <param name="registration">The current registration</param>
        /// <param name="otherRegistration">The registration to compare to</param>
        /// <returns>True if the registrations share the same lifestyle and implementation type</returns>
        public static bool IsEqualTo(this Registration registration, Registration otherRegistration)
        {
            return registration.Lifestyle == otherRegistration.Lifestyle
                && registration.ImplementationType == registration.ImplementationType;
        }
    }
}