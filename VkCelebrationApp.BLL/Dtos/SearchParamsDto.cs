using System.Collections.Generic;

namespace VkCelebrationApp.BLL.Dtos
{
    public class SearchParamsDto
    {
        public ushort? AgeFrom { get; set; }

        public ushort? AgeTo { get; set; }

        public LastSeenMode LastSeenMode { get; set; }

        public SexDto Sex { get; set; }

        public IEnumerable<RelationTypeDto> RelationTypes { get; set; }

        public long? CityId { get; set; }

        public long? UniversityId { get; set; }

        public bool CanWritePrivateMessage { get; set; }

        public bool IsOpened { get; set; }
    }

    public enum LastSeenMode
    {
        Online = 0,
        Last24Hours = 1
    }

    public enum RelationTypeDto
    {
        Unknown = 0,
        NotMarried = 1,
        HasFriend = 2,
        Engaged = 3,
        Married = 4,
        ItsComplex = 5,
        InActiveSearch = 6,
        Amorous = 7,
        CivilMarriage = 8
    }

    public enum SexDto
    {
        All = 0,
        Female = 1,
        Male = 2
    }
}
