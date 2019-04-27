namespace VkCelebrationApp.BLL.Dtos
{
    public class UserDto
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public long VkUserId { get; set; }

        public string AccessToken { get; set; }
    }
}
