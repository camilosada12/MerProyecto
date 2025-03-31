namespace Entity.DTOs
{
    public class ModuleFormDto
    {
        public int Id { get; set; }

        public int FormId { get; set; }
        public string FormName { get; set; }

        public int ModuleId { get; set; }
        public string ModuleName { get; set; }

        public bool IsDeleted { get; set; }
    }
}
