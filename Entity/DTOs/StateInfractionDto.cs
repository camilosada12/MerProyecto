using Entity.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTOs
{
    class StateInfractionDto
    {
        public int Id { get; set; }
        public DateTime DateViolation { get; set; }
        public decimal FineValue { get; set; }
        public bool State { get; set; }
        public decimal ToltalValue { get; set; }
        public int PaymentHistoryId { get; set; }
        public string PaymentHistoryName { get; set; }
    }
}
