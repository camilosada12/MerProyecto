namespace Entity.Model
{
    public class Module
    {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public bool statu { get; set; }
        public List<ModuleForm> ModuleForm { get; set; } = new List<ModuleForm>();
    }
}
