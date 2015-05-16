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
        [Test]
        public void ShouldAddEventHandlers()
        {
            // Arrange
            Guid testId = Guid.NewGuid();
            var command = new OpenTab()
            {
                Id = testId,
                TableNumber = 5,
                Waiter = "Bob"
            };
            var expectedEvent = new TabOpened()
            {
                Id = testId,
                TableNumber = 5,
                Waiter = "Bob"
            };
            ISubscribeTo<TabOpened> handler = new EventHandler();

            // Act
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
