using NServiceBus.ContainerTests;
using NServiceBus.ObjectBuilder.SimpleInjector;
using NUnit.Framework;

[SetUpFixture]
public class SetUpFixture
{
    public SetUpFixture()
    {
        TestContainerBuilder.ConstructBuilder = () => new SimpleInjectorObjectBuilder();
    }
}