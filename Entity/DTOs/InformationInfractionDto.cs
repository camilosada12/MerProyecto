using Entity.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTOs
{
    class InformationInfractionDto
    {
        public int Id { get; set; }
        public int Number_smldv { get; set; }
        public decimal MinimunWage { get; set; }
        public decimal value_smldv { get; set; }
        public decimal TotalValue { get; set; }
    }
}
