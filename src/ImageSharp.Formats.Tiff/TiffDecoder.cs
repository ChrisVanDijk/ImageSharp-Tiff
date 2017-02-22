using System.IO;

namespace ImageSharp.Formats.Tiff
{
    public class TiffDecoder : IImageDecoder
    {
        public void Decode<TColor>(Image<TColor> image, Stream stream) where TColor : struct, IPixel<TColor>
        {
            new TiffDecoderCore().Decode(image, stream);
        }
    }
}
