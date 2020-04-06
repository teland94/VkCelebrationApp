using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace VkCelebrationApp.BLL.Helpers
{
    public static class ImageHelpers
    {
        public static async Task<byte[]> DownloadAsync(Uri uri)
        {
            if (uri == null) return null;
            using var client = new HttpClient();
            return await client.GetByteArrayAsync(uri);
        }

        public static async Task<Stream> DownloadStreamAsync(Uri uri)
        {
            if (uri == null) return null;
            using var client = new HttpClient();
            return await client.GetStreamAsync(uri);
        }
    }
}
