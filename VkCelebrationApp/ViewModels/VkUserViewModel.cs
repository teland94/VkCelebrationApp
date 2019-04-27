namespace VkCelebrationApp.ViewModels
{
    public class VkUserViewModel
    {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string BirthDate { get; set; }

        public ushort Age { get; set; }

        public byte[] Photo50 { get; set; }
        public byte[] Photo100 { get; set; }

        public bool CanWritePrivateMessage { get; set; }

        public int? TimeZone { get; set; }
        public long? CityId { get; set; }
        public long? CountryId { get; set; }
    }
}
