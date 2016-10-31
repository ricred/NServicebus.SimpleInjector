using System;
using System.Collections.Generic;
using System.Linq;
using global::SimpleInjector;
using System.Linq.Expressions;

namespace NServiceBus.SimpleInjector
{
    /// <summary>
    /// Extension methods for configuration collection defaults on the SimpleInjector container
    /// </summary>
    public static class CollectionRegistrationExtensions
    {
        /// <summary>
        /// Adds support to the container to resolve arrays and lists
        /// </summary>
        /// <param name="container"></param>
        public static void AllowToResolveArraysAndLists(this global::SimpleInjector.Container container)
        {
            container.ResolveUnregisteredType += (sender, e) => {
                var serviceType = e.UnregisteredServiceType;

                if (serviceType.IsArray)
                {
                    RegisterArrayResolver(e, container,
                        serviceType.GetElementType());
                }
                else if (serviceType.IsGenericType &&
                  serviceType.GetGenericTypeDefinition() == typeof(IList<>))
                {
                    RegisterArrayResolver(e, container,
                        serviceType.GetGenericArguments()[0]);
                }
            };
        }

        private static void RegisterArrayResolver(UnregisteredTypeEventArgs e,
            global::SimpleInjector.Container container, Type elementType)
        {
            var producer = container.GetRegistration(typeof(IEnumerable<>)
                .MakeGenericType(elementType));
            var enumerableExpression = producer.BuildExpression();
            var arrayMethod = typeof(Enumerable).GetMethod("ToArray")
                .MakeGenericMethod(elementType);
            var arrayExpression =
                Expression.Call(arrayMethod, enumerableExpression);

            e.Register(arrayExpression);
        }
    }
}
