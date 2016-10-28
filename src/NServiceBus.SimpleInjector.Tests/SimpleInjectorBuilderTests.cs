using NServiceBus.ObjectBuilder.SimpleInjector;
using NUnit.Framework;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NServiceBus.SimpleInjector.Tests
{
    public class SimpleInjectorBuilderTests
    {
        [Test]
        public void Passing_A_Func_Uses_The_Func_To_Resolve()
        {
            var builder = new SimpleInjectorObjectBuilder();
            var funcCalled = false;

            Func<TestType> reg = () =>
            {
                funcCalled = true;
                return new TestType();
            };
            builder.Configure(reg, DependencyLifecycle.InstancePerCall);

            var resolve = builder.Build(typeof(TestType));
            Assert.True(funcCalled);
        }

        [Test]
        public void Passing_A_Func_Uses_The_Func_To_Resolve_In_Child_Container()
        {
            var builder = new SimpleInjectorObjectBuilder();
            var funcCalled = false;

            Func<TestType> reg = () =>
            {
                funcCalled = true;
                return new TestType();
            };
            builder.Configure(reg, DependencyLifecycle.InstancePerCall);

            var childContainer = builder.BuildChildContainer();

            var resolve = childContainer.Build(typeof(TestType));
            Assert.True(funcCalled);
        }
    }
}