namespace Entity.Model
{
    public class Module
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string description { get; set; }
        public bool statu { get; set; }
        public bool IsDeleted { get; set; }
        public List<ModuleForm> ModuleForm { get; set; } = new List<ModuleForm>();
    }
}
