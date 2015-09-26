using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Edument.CQRS.EntityFramework
{
    internal class EventStoreContext : DbContext
    {
        public EventStoreContext() : this("DefaultConnection") { }
        public EventStoreContext(string connectionStringName) : base(connectionStringName) { }

        #region For the Event Store
        public DbSet<Aggregate> Aggregates { get; set; }
        public DbSet<Event> Events { get; set; }
        #endregion

    }
}
