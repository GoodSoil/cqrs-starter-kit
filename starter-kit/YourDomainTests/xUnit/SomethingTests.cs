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

        [Fact]
        public void ASubsequentThingHapensAfterSomething()
        {
            Test(
                Given(new SomethingHappened
                {
                    Id = testId,
                    What = "Boom!"
                }),
                When(new MakeASubsequentThingHappen
                {
                    Id = testId,
                    Next = "{smoke}"
                }),
                Then(new SomethingExtraHappened
                {
                    Id = testId,
                    Summary = "Boom! then {smoke}"
                }));
        }

        [Fact]
        public void ASubsequentThingCanHappenMoreThanOnceAfterSomething()
        {
            Test(
                Given(new SomethingHappened
                {
                    Id = testId,
                    What = "Boom!"
                },
                new SomethingExtraHappened
                {
                    Id = testId,
                    Summary = "Boom! then {smoke}"
                }),
                When(new MakeASubsequentThingHappen
                {
                    Id = testId,
                    Next = "--silence--"
                }),
                Then(new SomethingExtraHappened
                {
                    Id = testId,
                    Summary = "Boom! then {smoke} then --silence--"
                }));
        }

        [Fact]
        public void SomethingMustHappenBeforeASubsequentThing()
        {
            Test(
                Given(),
                When(new MakeASubsequentThingHappen
                {
                    Id = testId,
                    Next = "{smoke}"
                }),
                ThenFailWith<SomethingMustHappenFirst>());
        }
    }
}
