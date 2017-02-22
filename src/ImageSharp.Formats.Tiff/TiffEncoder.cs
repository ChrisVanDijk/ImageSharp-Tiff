using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ImageSharp.Formats.Tiff
{
    public class TiffEncoder : IImageEncoder
    {
        public void Encode<TColor>(Image<TColor> image, Stream stream) where TColor : struct, IPixel<TColor>
        {
            new TiffEncoderCore().Encode(image, stream);
        }
    }
}
