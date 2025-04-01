using Entity.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTOs
{
    class PaymentUserDto
    {
        public int Id { get; set; }
        public int PaymentAgreementId { get; set; }
        public string paymentAgreementName { get; set; }
        public bool IsAgreement { get; set; }
        public int TypePaymentId { get; set; }
        public string TypePaymentName { get; set; }
        public int BillId { get; set; }
        public string BillName { get; set; }
    }
}
