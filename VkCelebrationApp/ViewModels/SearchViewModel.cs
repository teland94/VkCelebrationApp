using System.Collections.Generic;
using VkCelebrationApp.BLL.Dtos;

namespace VkCelebrationApp.ViewModels
{
    public class SearchViewModel
    {
        public SearchUserParamsViewModel SearchParams { get; set; }

        public uint? PageNumber = 1;

        public uint? PageSize = 20;
    }

    public class SearchUserParamsViewModel : SearchParamsViewModel
    {
        public bool CanWritePrivateMessage { get; set; }
    }

    public class SearchParamsViewModel
    {
        public ushort? AgeFrom { get; set; }

        public ushort? AgeTo { get; set; }

        public bool Online { get; set; }

        public ushort Sex { get; set; }

        public IEnumerable<RelationTypeDto> RelationTypes { get; set; }

        public long? CityId { get; set; }

        public long? UniversityId { get; set; }

        public bool IsOpened { get; set; }
    }
}
