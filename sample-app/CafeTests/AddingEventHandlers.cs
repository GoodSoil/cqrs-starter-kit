using Cafe.Tab;
using Edument.CQRS;
using Events.Cafe;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFrontend;

namespace CafeTests
{
    [TestFixture]
    public class AddingEventHandlers
    {
        private static void Arrange(out OpenTab command, out TabOpened expectedEvent, out ISubscribeTo<TabOpened> handler)
        {
            // Arrange
            Guid testId = Guid.NewGuid();
            command = new OpenTab()
            {
                Id = testId,
                TableNumber = 5,
                Waiter = "Bob"
            };
            expectedEvent = new TabOpened()
            {
                Id = testId,
                TableNumber = 5,
                Waiter = "Bob"
            };
            handler = new EventHandler();
        }

        [Test]
        public void ShouldAddEventHandlers()
        {
            ISubscribeTo<TabOpened> handler;
            OpenTab command;
            TabOpened expectedEvent;
            Arrange(out command, out expectedEvent, out handler);

            // Act
            Domain.Setup();
            Domain.Dispatcher.AddSubscriberFor<TabOpened>(handler);
            Domain.Dispatcher.SendCommand(command);

            // Assert
            Assert.AreEqual(expectedEvent.Id, (handler as EventHandler).Actual.Id);
        }

        public class EventHandler : ISubscribeTo<TabOpened>
        {
            public TabOpened Actual { get; private set; }
            public void Handle(TabOpened e)
            {
                Actual = e;
            }
        }
    }
}
