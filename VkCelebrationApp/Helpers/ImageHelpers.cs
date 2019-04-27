using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace VkCelebrationApp.Helpers
{
    public static class ImageHelpers
    {
        public async static Task<byte[]> Download(Uri uri)
        {
            if (uri == null) return null;
            using (var client = new HttpClient())
            {
                return await client.GetByteArrayAsync(uri);
            }
        }
    }
}
