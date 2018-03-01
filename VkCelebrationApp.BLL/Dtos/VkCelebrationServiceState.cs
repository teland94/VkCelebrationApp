namespace VkCelebrationApp.BLL.Dtos
{
    public class VkCelebrationServiceState
    {
        public VkCollectionDto<VkUserDto> CurrentUsers { get; set; }

        public ulong Offset { get; set; }
    }
}
