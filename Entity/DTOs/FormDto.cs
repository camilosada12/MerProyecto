namespace Entity.DTOs
{
    public class FormDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime DateCreation { get; set; }

        public bool statu { get; set; }

        public bool IsDeleted { get; set; }
    }
}
