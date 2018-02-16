namespace VkCelebrationApp.BLL.Configuration
{
    public interface IVkSearchConfiguration
    {
        ushort AgeFrom { get; set; }

        ushort? AgeTo { get; set; }

        ushort? Sex { get; set; }
    }
}