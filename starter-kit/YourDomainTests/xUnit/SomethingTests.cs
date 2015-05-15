using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YourDomain.Something;
using Edument.CQRS;
using Events.Something;
using Xunit;

namespace YourDomainTests.xUnit
{
    public class SomethingTests : BDDTest<SomethingAggregate>
    {
        public SomethingTests() : base(Assert.Equal, message => Assert.True(false, message))
        {
            BDDTestSetup();
            testId = Guid.NewGuid();
        }

        private Guid testId;


        [Fact]
        public void SomethingCanHappen()
        {
            Test(
                Given(),
                When(new MakeSomethingHappen
                {
                    Id = testId,
                    What = "Boom!"
                }),
                Then(new SomethingHappened
                {
                    Id = testId,
                    What = "Boom!"
                }));
        }

        [Fact]
        public void SomethingCanHappenOnlyOnce()
        {
            Test(
                Given(new SomethingHappened
                {
                    Id = testId,
                    What = "Boom!"
                }),
                When(new MakeSomethingHappen
                {
                    Id = testId,
                    What = "Boom!"
                }),
                ThenFailWith<SomethingCanOnlyHappenOnce>());
        }
    }
}
