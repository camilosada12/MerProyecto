using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class InformartionInfraction
    {
        public int Id { get; set; }
        public int Number_smldv { get; set; }
        public decimal MinimunWage { get; set; }
        public decimal value_smldv { get; set; }
        public decimal TotalValue { get; set; }
        public List<TypeInfraction> TypeInfraction { get; set; } = new List<TypeInfraction>();
    }
}
