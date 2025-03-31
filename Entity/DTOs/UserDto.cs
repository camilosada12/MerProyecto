namespace Entity.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string gmail { get; set; }
        public string UserNotificationId { get; set; }
        public bool IsDeleted { get; set; }
    }
}
