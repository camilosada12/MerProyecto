using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class PaymentUser
    {
        public int Id { get; set; }
        public int PaymentAgreementId { get; set; }
        public PaymentAgreement paymentAgreement { get; set; }
        public bool IsAgreement { get; set; }
        public int TypePaymentId { get; set; }
        public TypePayment TypePayment { get; set; }
        public int BillId { get; set; }
        public Bill Bill { get; set; }
    }
}
