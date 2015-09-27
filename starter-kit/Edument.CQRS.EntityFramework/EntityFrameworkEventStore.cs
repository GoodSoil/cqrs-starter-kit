using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Edument.CQRS.EntityFramework
{
    /// <summary>
    /// This is a simple example implementation of an event store, using Entity Framework
    /// to provide the storage. Tested and known to work with SQL Server.
    /// </summary>
    public class EntityFrameworkEventStore : IEventStore
    {
        private string connectionString;

        public EntityFrameworkEventStore(string connectionString)
        {
            this.connectionString = connectionString;
        }

        private object DeserializeEvent(string typeName, string data)
        {
            var ser = new XmlSerializer(Type.GetType(typeName));
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(data));
            ms.Seek(0, SeekOrigin.Begin);
            return ser.Deserialize(ms);
        }

        private string SerializeEvent(object obj)
        {
            var ser = new XmlSerializer(obj.GetType());
            var ms = new MemoryStream();
            ser.Serialize(ms, obj);
            ms.Seek(0, SeekOrigin.Begin);
            return new StreamReader(ms).ReadToEnd();
        }

        #region IEventStore Implementations
        public System.Collections.IEnumerable LoadEventsFor<TAggregate>(Guid id)
        {
            using (var context = new EventStoreContext(connectionString))
            {
                var events = from data in context.Events
                             where data.AggregateId == id
                             orderby data.SequenceNumber
                             select data;
                foreach (var item in events)
                {
                    yield return DeserializeEvent(item.Type, item.Body);
                }
            }
        }

        public void SaveEventsFor<TAggregate>(Guid id, int eventsLoaded, System.Collections.ArrayList newEvents)
        {
            using (var context = new EventStoreContext(connectionString))
            {
                var existingAggregate = context.Aggregates.Find(id);
                if (existingAggregate == null)
                    context.Aggregates.Add(new Aggregate { Id = id, Type = typeof(TAggregate).AssemblyQualifiedName });
                foreach (var item in newEvents)
                    context.Events.Add(new Event
                    {
                        AggregateId = id,
                        SequenceNumber = ++eventsLoaded,
                        Type = item.GetType().AssemblyQualifiedName,
                        Body = SerializeEvent(item),
                        Timestamp = DateTime.Now
                    });
                context.SaveChanges();
            }
        }
        #endregion
    }
}