

namespace Entity.Model
{
    public class User
    { 
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string gmail { get; set; }
        public DateTime RegistrationDate { get; set; }
        public bool IsDeleted { get; set; }
        public List<RolUser> rolUser { get; set; } = new List<RolUser>();
        public List<AccessLog> accessLogs { get; set; } = new List<AccessLog>();
        public List<TypePayment> typePayment { get; set; } = new List<TypePayment>();
    }
}
