using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.ProjectOxford.Face.Contract;

namespace VkCelebrationApp.BLL.Interfaces
{
    public interface IFaceApiService
    {
        Task<Face> DetectAsync(string imageData);

        Task<IList<Face>> DetectAsync(Uri imageUri);

        Task<string> IdentifyAsync(string personGroupId, string imageData);

        Task<PersonGroup[]> GetPersonGroupsAsync(int top = 1000);

        Task<string> CreatePersonGroupAsync(string name);

        Task UpdatePersonGroupAsync(string personGroupId, string name);

        Task DeletePersonGroupAsync(string personGroupId);

        Task<TrainingStatus> GetPersonGroupTrainingStatusAsync(string personGroupId);

        Task TrainPersonGroupAsync(string personGroupId);

        Task<IEnumerable<string>> GetPersonFaceIdsAsync(string personGroupId, string personId);

        Task<string> CreatePersonAsync(string personGroupId, string name);

        Task UpdatePersonAsync(string personGroupId, string personId, string name);

        Task DeletePersonAsync(string personGroupId, string personId);

        Task<string> AddPersonFaceAsync(string personGroupId, string personId, Stream imageStream);

        Task DeletePersonFaceAsync(string personGroupId, string personId, string faceId);
    }
}
