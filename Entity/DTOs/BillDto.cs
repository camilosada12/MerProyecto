using Entity.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTOs
{
    class BillDto
    {
        public int Id { get; set; }
        public int Barcode { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public decimal ToltalValue { get; set; }
        public bool state { get; set; }
    }
}
