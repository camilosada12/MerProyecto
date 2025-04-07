

namespace Entity.Model
{
    public class User
    { 
        public int id { get; set; }
        public string user_per { get; set; }
        public string password { get; set; }
        public string gmail { get; set; }
        public DateTime registrationdate { get; set; }
        public bool isdelete { get; set; } = false;
        public List<RolUser> rolUser { get; set; } = new List<RolUser>();
        public List<AccessLog> accessLogs { get; set; } = new List<AccessLog>();
        public List<TypePayment> typePayment { get; set; } = new List<TypePayment>();
    }
}
