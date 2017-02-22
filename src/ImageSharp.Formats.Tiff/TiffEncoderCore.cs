using BitMiracle.LibTiff.Classic;
using System.IO;

namespace ImageSharp.Formats.Tiff
{
    internal sealed class TiffEncoderCore
    {
        private int width;
        private int height;
        private int bytesPerPixel = 4;

        public void Encode<TColor>(Image<TColor> image, Stream stream)
            where TColor : struct, IPixel<TColor>
        {
            using (var tif = BitMiracle.LibTiff.Classic.Tiff.Open(stream, "w"))
            {
                this.width = image.Width;
                this.height = image.Height;

                tif.SetField(TiffTag.IMAGEWIDTH, this.width);
                tif.SetField(TiffTag.IMAGELENGTH, this.height);
                tif.SetField(TiffTag.COMPRESSION, Compression.LZW);
                tif.SetField(TiffTag.PHOTOMETRIC, Photometric.RGB);

                tif.SetField(TiffTag.ROWSPERSTRIP, image.Height);

                tif.SetField(TiffTag.XRESOLUTION, image.MetaData.HorizontalResolution);
                tif.SetField(TiffTag.YRESOLUTION, image.MetaData.VerticalResolution);

                tif.SetField(TiffTag.BITSPERSAMPLE, 8);
                tif.SetField(TiffTag.SAMPLESPERPIXEL, this.bytesPerPixel);

                tif.SetField(TiffTag.PLANARCONFIG, PlanarConfig.CONTIG);


                using (var pixels = image.Lock())
                {
                    byte[] color_ptr = new byte[pixels.RowStride];

                    for (int y = 0; y < this.height; y++)
                    {
                        CollectColorBytes(pixels, y, color_ptr);
                        tif.WriteScanline(color_ptr, y, 0);
                    }

                    tif.FlushData();
                }
            }
        }

        /// <summary>
        /// Collects a row of true color pixel data.
        /// </summary>
        /// <typeparam name="TColor">The pixel format.</typeparam>
        /// <param name="pixels">The image pixel accessor.</param>
        /// <param name="row">The row index.</param>
        /// <param name="rawScanline">The raw scanline.</param>
        private void CollectColorBytes<TColor>(PixelAccessor<TColor> pixels, int row, byte[] rawScanline)
            where TColor : struct, IPixel<TColor>
        {
            // We can use the optimized PixelAccessor here and copy the bytes in unmanaged memory.
            using (PixelArea<TColor> pixelRow = new PixelArea<TColor>(this.width, rawScanline, this.bytesPerPixel == 4 ? ComponentOrder.Xyzw : ComponentOrder.Xyz))
            {
                pixels.CopyTo(pixelRow, row);
            }
        }
    }
}
