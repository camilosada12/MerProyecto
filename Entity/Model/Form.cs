namespace Entity.Model
{
    public class Form
    {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public DateTime datacreation { get; set; }
        public bool statu { get; set; }
        public List<ModuleForm> ModuleForm { get; set; } = new List<ModuleForm>();

        public List<RolFormPermission> RolFormPermission { get; set; } = new List<RolFormPermission>();
    }
}
