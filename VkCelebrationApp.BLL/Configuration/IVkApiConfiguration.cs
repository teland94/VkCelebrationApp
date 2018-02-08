namespace VkCelebrationApp.BLL.Configuration
{
    public interface IVkApiConfiguration
    {
        ulong AppId { get; set; }

        string Host { get; set; }

        int? Port { get; set; }
    }
}
