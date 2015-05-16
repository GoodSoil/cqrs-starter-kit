using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;

namespace Edument.CQRS
{
    public delegate void AreEqualDelegate(object expected, object actual);
    public delegate void FailDelegate(string message);
    public delegate void PassDelegate(string message);
    /// <summary>
    /// Provides infrastructure for a set of tests on a given aggregate.
    /// </summary>
    /// <typeparam name="TAggregate"></typeparam>
    public class BDDTest<TAggregate>
        where TAggregate : Aggregate, new()
    {

        private TAggregate sut;
        private AreEqualDelegate AssertAreEqual;
        private FailDelegate AssertFail;
        private PassDelegate AssertPass;

        /// <summary>
        /// This constructor is most compatible with NUnit
        /// </summary>
        /// <param name="assertAreEqual">For the assertAreEqual nUnit assertion, use <code>Assert.AreEqual</code></param>
        /// <param name="assertFail">For the assertFail nUnit assertion, use <code>Assert.Fail</code></param>
        /// <param name="assertPass">For the assertPass nUnit assertion, use <code>Assert.Pass</code></param>
        public BDDTest(AreEqualDelegate assertAreEqual, FailDelegate assertFail, PassDelegate assertPass)
        {
            AssertAreEqual = assertAreEqual;
            AssertFail = assertFail;
            AssertPass = assertPass;
        }

        /// <summary>
        /// This constructor is most compatible with xUnit.
        /// </summary>
        /// <param name="assertAreEqual">For the assertAreEqual xUnit equivalent, use <code>Assert.Equal</code></param>
        /// <param name="assertFail">For the assertFail xUnit equivalent, use <code>message => Assert.True(false, message)</code></param>
        /// <remarks>
        /// The following is a sample of how to call this constructor for xUnit:
        /// <code>
        ///         public SomethingTests() : base(Assert.Equal, message => Assert.True(false, message))
        ///         {
        ///             BDDTestSetup();
        ///             testId = Guid.NewGuid();
        ///         }
        /// </code>
        /// </remarks>
        public BDDTest(AreEqualDelegate assertAreEqual, FailDelegate assertFail)
            : this(assertAreEqual, assertFail, delegate { return; })
        {
        }

        public virtual void BDDTestSetup()
        {
            sut = new TAggregate();
        }

        protected void Test(IEnumerable given, Func<TAggregate, object> when, Action<object> then)
        {
            then(when(ApplyEvents(sut, given)));
        }

        protected IEnumerable Given(params object[] events)
        {
            return events;
        }

        protected Func<TAggregate, object> When<TCommand>(TCommand command)
        {
            return agg =>
            {
                try
                {
                    return DispatchCommand(command).Cast<object>().ToArray();
                }
                catch (Exception e)
                {
                    return e;
                }
            };
        }

        protected Action<object> Then(params object[] expectedEvents)
        {
            return got =>
            {
                var gotEvents = got as object[];
                if (gotEvents != null)
                {
                    if (gotEvents.Length == expectedEvents.Length)
                        for (var i = 0; i < gotEvents.Length; i++)
                            if (gotEvents[i].GetType() == expectedEvents[i].GetType())
                                AssertAreEqual(Serialize(expectedEvents[i]), Serialize(gotEvents[i]));
                            else
                                AssertFail(string.Format(
                                    "Incorrect event in results; expected a {0} but got a {1}",
                                    expectedEvents[i].GetType().Name, gotEvents[i].GetType().Name));
                    else if (gotEvents.Length < expectedEvents.Length)
                        AssertFail(string.Format("Expected event(s) missing: {0}",
                            string.Join(", ", EventDiff(expectedEvents, gotEvents))));
                    else
                        AssertFail(string.Format("Unexpected event(s) emitted: {0}",
                            string.Join(", ", EventDiff(gotEvents, expectedEvents))));
                }
                else if (got is CommandHandlerNotDefiendException)
                    AssertFail((got as Exception).Message);
                else
                    AssertFail(string.Format("Expected events, but got exception {0}",
                        got.GetType().Name));
            };
        }

        private string[] EventDiff(object[] a, object[] b)
        {
            var diff = a.Select(e => e.GetType().Name).ToList();
            foreach (var remove in b.Select(e => e.GetType().Name))
                diff.Remove(remove);
            return diff.ToArray();
        }

        protected Action<object> ThenFailWith<TException>()
        {
            return got =>
            {
                if (got is TException)
                    AssertPass("Got correct exception type");
                else if (got is CommandHandlerNotDefiendException)
                    AssertFail((got as Exception).Message);
                else if (got is Exception)
                    AssertFail(string.Format(
                        "Expected exception {0}, but got exception {1}",
                        typeof(TException).Name, got.GetType().Name));
                else
                    AssertFail(string.Format(
                        "Expected exception {0}, but got event result",
                        typeof(TException).Name));
            };
        }

        private IEnumerable DispatchCommand<TCommand>(TCommand c)
        {
            var handler = sut as IHandleCommand<TCommand>;
            if (handler == null)
                throw new CommandHandlerNotDefiendException(string.Format(
                    "Aggregate {0} does not yet handle command {1}",
                    sut.GetType().Name, c.GetType().Name));
            return handler.Handle(c);
        }

        private TAggregate ApplyEvents(TAggregate agg, IEnumerable events)
        {
            agg.ApplyEvents(events);
            return agg;
        }

        private string Serialize(object obj)
        {
            var ser = new XmlSerializer(obj.GetType());
            var ms = new MemoryStream();
            ser.Serialize(ms, obj);
            ms.Seek(0, SeekOrigin.Begin);
            return new StreamReader(ms).ReadToEnd();
        }

        private class CommandHandlerNotDefiendException : Exception
        {
            public CommandHandlerNotDefiendException(string msg) : base(msg) { }
        }
    }
}
