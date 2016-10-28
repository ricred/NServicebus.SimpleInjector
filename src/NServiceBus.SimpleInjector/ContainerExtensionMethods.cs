using NServiceBus.SimpleInjector;
using SimpleInjector;
using SimpleInjector.Extensions.ExecutionContextScoping;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NServiceBus.ObjectBuilder
{
    public static class ContainerExtensionMethods
    {
        public static global::SimpleInjector.Container Clone(this global::SimpleInjector.Container parentContainer)
        {
            var clonedContainer = new global::SimpleInjector.Container();
            clonedContainer.AllowToResolveArraysAndLists();
            clonedContainer.Options.AllowOverridingRegistrations = true;
            clonedContainer.Options.DefaultScopedLifestyle = new ExecutionContextScopeLifestyle();
            clonedContainer.Options.AutoWirePropertiesImplicitly();

            clonedContainer.BeginExecutionContextScope();

            foreach (var reg in parentContainer.GetCurrentRegistrations())//.Where(r => !typeof(IEnumerable).IsAssignableFrom(r.Registration.ImplementationType)))
            {
                if (reg.Lifestyle == Lifestyle.Singleton && !HasComponent(clonedContainer, reg.ServiceType))
                {
                    // Use the parent container to resolve singletons. This could be problematic :(
                    clonedContainer.Register(reg.ServiceType, () => parentContainer.GetInstance(reg.ServiceType), reg.Lifestyle);
                }
                else
                {
                    var field = reg.Registration.GetType().GetField("instanceCreator", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);
                    var instanceCreator = (Func<object>)field?.GetValue(reg.Registration);

                    if (instanceCreator != null)
                    {
                        var newReg = reg.Lifestyle.CreateRegistration(reg.ServiceType, instanceCreator, clonedContainer);
                        clonedContainer.AddRegistration(reg.ServiceType, newReg);
                    }
                    else
                    {
                        var newReg = reg.Lifestyle.CreateRegistration(reg.ServiceType, reg.Registration.ImplementationType, clonedContainer);
                        clonedContainer.AddRegistration(reg.ServiceType, newReg);
                    }
                }
            }

            return clonedContainer;
        }

        static bool HasComponent(global::SimpleInjector.Container container, Type componentType)
        {
            return container.GetCurrentRegistrations().Any(r => r.ServiceType == componentType);
        }

        public static bool IsEqualTo(this Registration registration, Registration otherRegistration)
        {
            return registration.Lifestyle == otherRegistration.Lifestyle
                && registration.ImplementationType == registration.ImplementationType;
        }
    }
}