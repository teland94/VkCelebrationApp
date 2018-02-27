using System;

namespace VkCelebrationApp.BLL.Dtos
{
    public class UserDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string BirthDate { get; set; }
        public ushort Age { get; set; }
        public Uri Photo50 { get; set; }
        public Uri Photo100 { get; set; }
        public Uri PhotoMax { get; set; }

        public int? TimeZone { get; set; }
        public long? CityId { get; set; }
        public long? CountryId { get; set; }
    }
}