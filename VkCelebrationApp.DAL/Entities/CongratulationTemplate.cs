namespace VkCelebrationApp.DAL.Entities
{
    public class CongratulationTemplate : EntityBase
    {
        public string Text { get; set; }

        public int? CreatedById { get; set; }
        public User CreatedBy { get; set; }
    }
}