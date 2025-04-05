using ImageMagick;

namespace web.Services.Image;

public static class ImageResizer
{
    public static async Task CreateThumbnailAsync(IFormFile imageStream, uint width, uint height, string folder,
        string fileName)
    {
        await using var stream = imageStream.OpenReadStream();
        using var image = new MagickImage(stream);

        image.AutoOrient();

        var cropSize = Math.Min(image.Width, image.Height);
        var xOffset = (image.Width - cropSize) / 2;
        var yOffset = (image.Height - cropSize) / 2;
        image.Crop(new MagickGeometry((int)xOffset, (int)yOffset, cropSize, cropSize));

        image.Resize(width, height);
        
        await image.WriteAsync(Path.Combine(folder, fileName), MagickFormat.WebP);
    }
}