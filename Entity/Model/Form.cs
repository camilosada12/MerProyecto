namespace Entity.Model
{
    public class Form
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime DateCreation { get; set; }

        public bool statu { get; set; }

        public bool IsDeleted { get; set; }
        public List<ModuleForm> ModuleForm { get; set; } = new List<ModuleForm>();

        public List<RolFormPermission> RolFormPermission { get; set; } = new List<RolFormPermission>();
    }
}
