using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NServiceBus.ObjectBuilder;
using SimpleInjector;
using SimpleInjector.Extensions.ExecutionContextScoping;

namespace NServiceBus.SimpleInjector.Tests
{
    public class ContainerExtensionTests
    {
        [Test]
        public void Cloning_A_Container_Does_Not_Give_The_Same_Container_Instance()
        {
            var container = new global::SimpleInjector.Container();

            var clonedContainer = container.Clone();

            Assert.AreNotSame(container, clonedContainer);
        }

        [Test]
        public void Cloning_A_Container_Allows_Resolving_Same_SingletonInstance()
        {
            var container = new global::SimpleInjector.Container();

            container.Register(typeof(TestType), () => new TestType(), Lifestyle.Singleton);
            var originalResolve = container.GetInstance<TestType>();

            var clonedContainer = container.Clone();
            var clonedResolve = clonedContainer.GetInstance<TestType>();

            Assert.AreSame(originalResolve, clonedResolve);
        }

        [Test]
        public void Cloning_A_Container_Allows_Resolving_Transient_Types_Configured_In_ParentContainer()
        {
            var container = new global::SimpleInjector.Container();

            var clonedContainer = container.Clone();
            var clonedResolve = clonedContainer.GetInstance<TestType>();

            Assert.IsNotNull(clonedResolve);
        }

        [Test]
        public void Cloning_A_Container_Allows_Resolving_Child_Scoped_Variables()
        {
            var container = new global::SimpleInjector.Container();
            container.Options.DefaultScopedLifestyle = new ExecutionContextScopeLifestyle();
            container.BeginExecutionContextScope();

            container.Register<TestType>(Lifestyle.Scoped);

            var cloned = container.Clone();

            var clonedResolve = cloned.GetInstance<TestType>();
        }
    }

    class TestType
    {

    }
}