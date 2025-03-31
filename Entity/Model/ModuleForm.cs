namespace Entity.Model
{
   public class ModuleForm
    {
        public int Id { get; set; }
        public int FormId { get; set; }
        public Form Form { get; set; }
        public int ModuleId { get; set; }
        public Module Module { get; set; }
        public bool IsDeleted { get; set; }
    }
}
