namespace VkCelebrationApp.BLL.Dtos
{
    public class VkCelebrationServiceState
    {
        public VkCollectionDto<UserDto> CurrentUsers { get; set; }

        public ulong Offset { get; set; }
    }
}
