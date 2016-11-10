using NServiceBus.AcceptanceTesting;
using NServiceBus.AcceptanceTests.EndpointTemplates;
using NUnit.Framework;
using SimpleInjector.Extensions.ExecutionContextScoping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NServiceBus.SimpleInjector.AcceptanceTests
{
    class When_using_sagas
    {
        [Test]
        public async Task Should_be_able_to_load_sagadata()
        {
            var context = await Scenario.Define<Context>()
                .WithEndpoint<Endpoint>(b => b.When((bus, c) => bus.SendLocal(new MyMessage())))
                .Done(c => c.WasCalled)
                .Run();

            //Assert.IsTrue(context.PropertyWasInjected);
        }

        public class Context : ScenarioContext
        {
            public bool WasCalled { get; set; }
        }

        public class Endpoint : EndpointConfigurationBuilder
        {
            public Endpoint()
            {
                EndpointSetup<DefaultServer>(config => config.UseContainer<SimpleInjectorBuilder>());
            }

            public class MyMessageHandler : Saga<TestSagaData>,
                                            IAmStartedByMessages<MyMessage>
            {
                public MyMessageHandler(Context testContext)
                {
                    this.testContext = testContext;
                }

                public Task Handle(MyMessage message, IMessageHandlerContext context)
                {
                    testContext.WasCalled = true;

                    return Task.FromResult(0);
                }

                protected override void ConfigureHowToFindSaga(SagaPropertyMapper<TestSagaData> mapper)
                {
                    mapper.ConfigureMapping<MyMessage>(message => message.SagaId).ToSaga(sagaData => sagaData.Id);
                }

                Context testContext = new Context();
            }
        }

        public class MyMessage : ICommand
        {
            public Guid SagaId { get; set; }
        }

        public class TestSagaData : IContainSagaData
        {
            public Guid Id { get; set; }
            public string OriginalMessageId { get; set; }
            public string Originator { get; set; }
        }
    }
}
