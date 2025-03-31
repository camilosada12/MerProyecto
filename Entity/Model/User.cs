

namespace Entity.Model
{
    public class User
    { 
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string gmail { get; set; }
        public DateTime RegistrationDate { get; set; }

        public string UserNotificationId { get; set; }

        public bool IsDeleted { get; set; }

        public List<RolUser> RolUser { get; set; } = new List<RolUser>();
    }
}
