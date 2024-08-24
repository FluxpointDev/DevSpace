using SkiaSharp;

namespace DevSpaceWeb.Services;

public class ImageBuilder
{
    public string output { get; set; } = "jpg";


    public CanvasImage image { get; set; } = new CanvasImage();

    public class CanvasImage
    {
        public string type { get; set; }

        public string path { get; set; }
    }

    public async Task<SKSurface> Build()
    {
        SKSurface surface = null;

        SKCanvas myCanvas = null;



        switch (image.type)
        {
            case "file":
                string file = "";
                if (File.Exists(file))
                {
                    using (SKFileStream stream = new SKFileStream(file))
                    using (SKBitmap bitmap = SKBitmap.Decode(stream))
                    {
                        surface = SKSurface.Create(new SKImageInfo(bitmap.Width, bitmap.Height));
                        myCanvas = surface.Canvas;

                        myCanvas.DrawBitmap(bitmap, 0, 0);
                    }
                }
                else
                {
                    throw new CanvasException("provided file does not exist.");
                }
                break;
        };

        if (myCanvas == null)
            throw new CanvasException("failed to parse base.");

        return surface;
    }


}


public class CanvasException : Exception
{
    public CanvasException(string message) : base(message)
    {

    }
}
