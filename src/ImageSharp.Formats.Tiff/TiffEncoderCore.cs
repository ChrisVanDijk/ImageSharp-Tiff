using BitMiracle.LibTiff.Classic;
using System.IO;

namespace ImageSharp.Formats.Tiff
{
    internal sealed class TiffEncoderCore
    {
        public void Encode<TColor>(Image<TColor> image, Stream stream)
            where TColor : struct, IPixel<TColor>
        {

            using (var tif = BitMiracle.LibTiff.Classic.Tiff.Open(stream, "w"))
            {
                tif.SetField(TiffTag.IMAGEWIDTH, image.Width);
                tif.SetField(TiffTag.IMAGELENGTH, image.Height);
                tif.SetField(TiffTag.COMPRESSION, Compression.LZW);
                tif.SetField(TiffTag.PHOTOMETRIC, Photometric.RGB);

                tif.SetField(TiffTag.ROWSPERSTRIP, image.Height);

                tif.SetField(TiffTag.XRESOLUTION, image.MetaData.HorizontalResolution);
                tif.SetField(TiffTag.YRESOLUTION, image.MetaData.VerticalResolution);

                tif.SetField(TiffTag.BITSPERSAMPLE, 8);
                tif.SetField(TiffTag.SAMPLESPERPIXEL, 4);

                tif.SetField(TiffTag.PLANARCONFIG, PlanarConfig.CONTIG);

                int height = image.Height;
                int width = image.Width;
                using (var pixels = image.Lock())
                {
                    byte[] color_ptr = new byte[pixels.RowStride];

                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            var color = pixels[x, y];
                            color.ToXyzwBytes(color_ptr, x * 4);
                        }

                        tif.WriteScanline(color_ptr, y, 0);
                    }

                    tif.FlushData();
                }
            }
        }
    }
}
