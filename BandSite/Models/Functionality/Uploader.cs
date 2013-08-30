using System.Collections.Generic;
using System.IO;

namespace BandSite.Models.Functionality
{
    public class Uploader
    {
        public IEnumerable<double> UploadPartial(byte[] buffer, Stream stream, string userName)
        {
            var offset = 0;
            while (stream.Position < stream.Length)
            {
                stream.Read(buffer, offset, 4096);
                offset += 4096;
                var percentage = (double)stream.Position / stream.Length * 100;
                yield return System.Math.Round(percentage, 2);
            }
        }
    }
}