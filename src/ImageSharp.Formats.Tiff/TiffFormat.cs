using System.Collections.Generic;

namespace ImageSharp.Formats.Tiff
{
    public class TiffFormat : IImageFormat
    {
        /// <inheritdoc/>
        public string MimeType => "image/tiff";

        /// <inheritdoc/>
        public string Extension => "tif";

        /// <inheritdoc/>
        public IEnumerable<string> SupportedExtensions => new string[] { "tif" };

        /// <inheritdoc/>
        public IImageDecoder Decoder => new TiffDecoder();

        /// <inheritdoc/>
        public IImageEncoder Encoder => new TiffEncoder();

        /// <inheritdoc/>
        public int HeaderSize => 4;

        /// <inheritdoc/>
        public bool IsSupportedFileFormat(byte[] header)
        {

            return header.Length >= this.HeaderSize &&
                (
                   (header[0] == 0x49 && //I
                   header[1] == 0x49 &&  //I
                   header[2] == 0x2A &&
                   header[3] == 0x00)
                   ||
                   (header[0] == 0x4D && //M
                   header[1] == 0x4D &&  //M
                   header[2] == 0x00 &&
                   header[3] == 0x2A)
                );
        }
    }
}
