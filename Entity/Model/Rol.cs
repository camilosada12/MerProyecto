namespace Entity.Model
{
    public class Rol
    {
        public int id { get; set; }
        public string role { get; set; }
        public string description { get; set; }
        public bool isdelete { get; set; } = false;
        public List<RolUser> RolUser { get; set; } =  new List<RolUser>();

        public List<RolFormPermission> RolFormPermission { get; set; }= new List<RolFormPermission>();
    }
}
