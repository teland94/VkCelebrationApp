namespace VkCelebrationApp.DAL.Entities
{
    public class CongratulationTemplate
    {
        public int Id { get; set; }

        public string Text { get; set; }

        public int? CreatedById { get; set; }
        public User CreatedBy { get; set; }
    }
}