using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Edument.CQRS;
using Events.Something;
using System.Collections;

namespace YourDomain.Something
{
    public class SomethingAggregate : Aggregate,
        IHandleCommand<MakeSomethingHappen>,
        IApplyEvent<SomethingHappened>,
        IHandleCommand<MakeASubsequentThingHappen>,
        IApplyEvent<SomethingExtraHappened>
    {
        private string What { get; set; }
        private bool alreadyHappened;

        public IEnumerable Handle(MakeSomethingHappen c)
        {
            if (alreadyHappened)
                throw new SomethingCanOnlyHappenOnce();

            yield return new SomethingHappened
            {
                Id = c.Id,
                What = c.What
            };
        }

        public void Apply(SomethingHappened e)
        {
            alreadyHappened = true;
            What = e.What;
        }

        public IEnumerable Handle(MakeASubsequentThingHappen c)
        {
            if (!alreadyHappened)
                throw new SomethingMustHappenFirst();

            yield return new SomethingExtraHappened
            {
                Id = c.Id,
                Summary = What + " then " + c.Next
            };
        }

        public void Apply(SomethingExtraHappened e)
        {
            What = e.Summary;
        }
    }
}
