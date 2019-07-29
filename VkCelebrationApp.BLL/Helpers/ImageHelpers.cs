using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace VkCelebrationApp.BLL.Helpers
{
    public static class ImageHelpers
    {
        public async static Task<byte[]> DownloadAsync(Uri uri)
        {
            if (uri == null) return null;
            using (var client = new HttpClient())
            {
                return await client.GetByteArrayAsync(uri);
            }
        }

        public async static Task<Stream> DownloadStreamAsync(Uri uri)
        {
            if (uri == null) return null;
            using (var client = new HttpClient())
            {
                return await client.GetStreamAsync(uri);
            }
        }
    }
}
