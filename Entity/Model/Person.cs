namespace Entity.Model
{
    public class Person
    {
        public int id { get; set; }
        public string name { get; set; }
        public string lastname { get; set; }
        public string phone { get; set; }
        public List<User> User { get; set; } = new List<User>();
    }
}
