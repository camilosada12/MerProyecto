using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class PaymentHistory
    {
        public int Id { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal DiscountDate { get; set; }
        public List<StateInfraction> StateInfraction { get; set; } = new List<StateInfraction>();
    }
}
