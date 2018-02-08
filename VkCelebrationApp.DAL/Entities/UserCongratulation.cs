namespace VkCelebrationApp.DAL.Entities
{
    public class UserCongratulation
    {
        public int Id { get; set; }

        public string Text { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }
}
