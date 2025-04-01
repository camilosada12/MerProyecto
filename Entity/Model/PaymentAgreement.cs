using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class PaymentAgreement
    {
        public int Id { get; set; }
        public string Address { get; set; }
        public string Neighborhood { get; set; }
        public string FinanceAmount { get; set; }
        public string Agreementdiscount { get; set; }
        public bool state { get; set; }
        public List<PaymentUser> PaymentUser { get; set; } = new List<PaymentUser>();
    }
}
