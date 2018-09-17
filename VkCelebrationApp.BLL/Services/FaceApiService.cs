using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using VkCelebrationApp.BLL.Configuration;
using VkCelebrationApp.BLL.Interfaces;

namespace VkCelebrationApp.BLL.Services
{
    public class FaceApiService : IFaceApiService
    {
        private IFaceServiceClient FaceClient { get; }

        public FaceApiService(IFaceApiConfiguration faceApiConfiguration)
        {
            FaceClient = new FaceServiceClient(faceApiConfiguration.Key, faceApiConfiguration.Endpoint);
        }

        public async Task<Face> DetectAsync(string imageData)
        {
            return (await UploadAndDetectFacesAsync(imageData))?.FirstOrDefault();
        }

        public async Task<IList<Face>> DetectAsync(Uri imageUri)
        {
            return await UploadAndDetectFacesAsync(imageUri);
        }

        public async Task<string> IdentifyAsync(string personGroupId, string imageData)
        {
            var delectedFaces = await UploadAndDetectFacesAsync(imageData);
            var results = await FaceClient.IdentifyAsync(personGroupId, delectedFaces.Select(f => f.FaceId).ToArray());
            return results.SelectMany(res => res.Candidates).FirstOrDefault()?.PersonId.ToString();
        }

        public async Task<PersonGroup[]> GetPersonGroupsAsync(int top = 1000)
        {
            return await FaceClient.ListPersonGroupsAsync(top: top);
        }

        public async Task<string> CreatePersonGroupAsync(string name)
        {
            var guid = Guid.NewGuid().ToString();
            await FaceClient.CreatePersonGroupAsync(guid, name);
            return guid;
        }

        public async Task UpdatePersonGroupAsync(string personGroupId, string name)
        {
            await FaceClient.UpdatePersonGroupAsync(personGroupId, name);
        }

        public async Task DeletePersonGroupAsync(string personGroupId)
        {
            await FaceClient.DeletePersonGroupAsync(personGroupId);
        }

        public async Task<TrainingStatus> GetPersonGroupTrainingStatusAsync(string personGroupId)
        {
            return await FaceClient.GetPersonGroupTrainingStatusAsync(personGroupId);
        }

        public async Task TrainPersonGroupAsync(string personGroupId)
        {
            await FaceClient.TrainPersonGroupAsync(personGroupId);
        }

        public async Task<IEnumerable<string>> GetPersonFaceIdsAsync(string personGroupId, string personId)
        {
            var person = await FaceClient.GetPersonAsync(personGroupId, new Guid(personId));
            return person.PersistedFaceIds.Select(f => f.ToString());
        }

        public async Task<string> CreatePersonAsync(string personGroupId, string name)
        {
            var res = await FaceClient.CreatePersonAsync(personGroupId, name);
            return res.PersonId.ToString();
        }

        public async Task UpdatePersonAsync(string personGroupId, string personId, string name)
        {
            await FaceClient.UpdatePersonAsync(personGroupId, new Guid(personId), name);
        }

        public async Task DeletePersonAsync(string personGroupId, string personId)
        {
            await FaceClient.DeletePersonAsync(personGroupId, new Guid(personId));
        }

        public async Task<string> AddPersonFaceAsync(string personGroupId, string personId, Stream imageStream)
        {
            var res = await FaceClient.AddPersonFaceAsync(personGroupId, new Guid(personId), imageStream);
            return res.PersistedFaceId.ToString();
        }

        public async Task DeletePersonFaceAsync(string personGroupId, string personId, string faceId)
        {
            await FaceClient.DeletePersonFaceAsync(personGroupId, new Guid(personId), new Guid(faceId));
        }

        private async Task<IList<Face>> UploadAndDetectFacesAsync(string imageData)
        {
            var data = Convert.FromBase64String(imageData);

            using (var ms = new MemoryStream(data))
            {
                return await FaceClient.DetectAsync(ms);
            }
        }

        private async Task<IList<Face>> UploadAndDetectFacesAsync(Uri imageUri)
        {
            IList<FaceAttributeType> faceAttributes =
                new[]
                {
                    FaceAttributeType.Gender, FaceAttributeType.Age,
                    FaceAttributeType.Smile, FaceAttributeType.Emotion,
                    FaceAttributeType.Glasses, FaceAttributeType.FacialHair
                };
            return await FaceClient.DetectAsync(imageUri.AbsoluteUri, returnFaceAttributes: faceAttributes);
        }
    }
}
