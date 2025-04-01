using Entity.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTOs
{
    class TypeInfractionDto
    {
        public int Id { get; set; }
        public string Type_Violation { get; set; }
        public decimal ValueInfraction { get; set; }
        public string Description { get; set; }
        public string InformationFine { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int TypeInfractionId { get; set; }
        public string typeInfractionName { get; set; }
        public int UserNotificationId { get; set; }
        public string userNotification { get; set; }
        public int StateInfractionId { get; set; }
        public string StateInfractionName { get; set; }
    }
}
