using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Edument.CQRS.EntityFramework
{
    class Event
    {
        [Key, DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public Guid EventId { get; set; }

        public Guid AggregateId { get; set; }
        [Column(TypeName = "varchar(MAX)")]
        public string Type { get; set; }
        [Column(TypeName = "varchar(MAX)")]
        public string Body { get; set; }
        public int SequenceNumber { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
