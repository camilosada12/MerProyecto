using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class UserNotification
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public DateTime ShippingDate { get; set; }
        public bool state { get; set; }
        public List<TypeInfraction> typeInfraction { get; set; } = new List<TypeInfraction>();
    }
}
