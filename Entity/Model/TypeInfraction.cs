using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Model
{
    public class TypeInfraction
    {
        public int Id { get; set; }
        public string Type_Violation { get; set; }
        public decimal ValueInfraction { get; set; }
        public string Description { get; set; }
        public string InformationFine { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int TypeInfractionId { get; set; }
        public TypeInfraction typeInfraction { get; set; }
        public int UserNotificationId { get; set; }
        public UserNotification userNotification { get; set; }
        public int StateInfractionId { get; set; }
        public StateInfraction StateInfraction { get; set; }
    }
}
