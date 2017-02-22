using BitMiracle.LibTiff.Classic;
using System;
using System.IO;

namespace ImageSharp.Formats.Tiff
{
    internal class TiffDecoderCore
    {
        public void Decode<TColor>(Image<TColor> image, Stream stream)
            where TColor : struct, IPixel<TColor>
        {
            using (var sourceImage = BitMiracle.LibTiff.Classic.Tiff.Open(stream, "r"))
            {
                // Find the width and height of the image
                FieldValue[] value = sourceImage.GetField(TiffTag.IMAGEWIDTH);
                int width = value[0].ToInt();

                value = sourceImage.GetField(TiffTag.IMAGELENGTH);
                int height = value[0].ToInt();

                value = sourceImage.GetField(TiffTag.XRESOLUTION);
                int xRes = value[0].ToInt();

                value = sourceImage.GetField(TiffTag.YRESOLUTION);
                int yRes = value[0].ToInt();

                if (width > image.MaxWidth || height > image.MaxHeight)
                {
                    throw new ArgumentOutOfRangeException($"The input png '{width}x{height}' is bigger than the max allowed size '{image.MaxWidth}x{image.MaxHeight}'");
                }

                image.InitPixels(width, height);

                int imageSize = height * width;
                int[] raster = new int[imageSize];

                // Read the image into the memory buffer
                if (!sourceImage.ReadRGBAImage(width, height, raster))
                {
                    throw new Exception("Cannot read image into raster");
                }

                image.MetaData.HorizontalResolution = xRes;
                image.MetaData.VerticalResolution = yRes;

                TColor color = default(TColor);

                using (var pixels = image.Lock())
                {
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            int offset = (height - y - 1) * width + x;
                            int r = BitMiracle.LibTiff.Classic.Tiff.GetR(raster[offset]);
                            int g = BitMiracle.LibTiff.Classic.Tiff.GetG(raster[offset]);
                            int b = BitMiracle.LibTiff.Classic.Tiff.GetB(raster[offset]);
                            int a = BitMiracle.LibTiff.Classic.Tiff.GetA(raster[offset]);

                            color.PackFromBytes((byte)(r), (byte)(g), (byte)(b), (byte)a);

                            pixels[x, y] = color;
                        }
                    }
                }
            }
        }
    }
}
