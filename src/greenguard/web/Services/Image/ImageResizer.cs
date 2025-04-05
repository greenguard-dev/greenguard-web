using ImageMagick;

namespace web.Services.Image;

public static class ImageResizer
{
    public static async Task CreateThumbnailAsync(IFormFile imageStream, uint width, uint height, string folder,
        string fileName)
    {
        await using var stream = imageStream.OpenReadStream();
        using var image = new MagickImage(stream);

        var centerX = image.Width / 2;
        var centerY = image.Height / 2;
        var cropWidth = Math.Min(image.Width, image.Height);

        image.Crop(new MagickGeometry((int)(centerX - cropWidth / 2), (int)(centerY - cropWidth / 2),
            cropWidth, cropWidth));

        image.Resize(width, height);

        if (Directory.Exists(folder) == false)
            Directory.CreateDirectory(folder);

        await image.WriteAsync(Path.Combine(folder, fileName), MagickFormat.WebP);
    }
}