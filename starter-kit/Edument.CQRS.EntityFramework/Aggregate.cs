using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Edument.CQRS.EntityFramework
{
    class Aggregate
    {
        [Key]
        public Guid Id { get; set; }
        [Column(TypeName = "varchar(MAX)")]
        public string Type { get; set; }
    }
}
