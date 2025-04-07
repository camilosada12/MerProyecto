namespace Entity.Model
{
    public class Permission
    {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public bool? isdelete { get; set; } = false;
        public List<RolFormPermission> RolFormPermission { get; set; } = new List<RolFormPermission>();
    }
}
