using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace FinanceManager.Models

{
    [Table("EnvelopeEntry")]
    public class EnvelopeEntry
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed]
        public int EnvelopeId { get; set; }

        [NotNull]
        public decimal Amount { get; set; }

        [NotNull]
        public EntryType Type { get; set; }

        [NotNull]
        public DateTime OccurredAtUtc { get; set; }

        [Indexed]
        public int YearMonth { get; set; }

        public string? Note { get; set; }
    }
}
