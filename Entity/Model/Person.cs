namespace Entity.Model
{
    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public int Phone { get; set; }
        public bool IsDeleted { get; set; }
        public List<User> User { get; set; } = new List<User>();
    }
}
