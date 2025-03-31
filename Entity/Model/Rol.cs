namespace Entity.Model
{
    public class Rol
    {
        public int Id { get; set; }

        public string Role { get; set; }

        public string Description { get; set; }
        public bool IsDeleted { get; set; }
        public List<RolUser> RolUser { get; set; } =  new List<RolUser>();

        public List<RolFormPermission> RolFormPermission { get; set; }= new List<RolFormPermission>();
    }
}
