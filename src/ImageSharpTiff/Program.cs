namespace ImageSharpTiff
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ImageSharp.Configuration.Default.AddImageFormat(new ImageSharp.Formats.Tiff.TiffFormat());

            using (var image = new ImageSharp.Image("MARBLES.TIF"))
            {
                image.Save("MARBLES.png");
            }
        }
    }
}
