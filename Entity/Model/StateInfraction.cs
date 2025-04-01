using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class StateInfraction
    {
        public int Id { get; set; }
        public DateTime DateViolation { get; set; }
        public decimal FineValue { get; set; }
        public bool State { get; set; }
        public decimal ToltalValue { get; set; }
        public int PaymentHistoryId { get; set; }
        public PaymentHistory PaymentHistory { get; set; }
        public List<TypeInfraction> TypeInfraction { get; set; } = new List<TypeInfraction>();
    }
}
