using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace PlaceAnimal.Enums;

public static class ImageProcessingContextExtensions
{
    /// <param name="aspectRatio">The target image aspect ratio</param>
    public static IImageProcessingContext CropToAspectRatio(this IImageProcessingContext context, decimal aspectRatio)
    {
        Size size = context.GetCurrentSize();
            
        decimal sourceAspectRatio = (decimal) size.Width / size.Height;

        if (sourceAspectRatio > aspectRatio)
        {
            // Source image is wider than desired. Crop the width to the correct aspect ratio
            decimal newWidth = size.Height * aspectRatio;

            decimal x = (size.Width - newWidth) / 2;
            int y = 0;

            context.Crop(new Rectangle((int) x, y, (int) newWidth, size.Height));
        }
        else if (sourceAspectRatio < aspectRatio)
        {
            // Source image is higher than desired. Crop the height to the correct aspect ratio
            decimal newHeight = size.Width / aspectRatio;

            decimal y = (size.Height - newHeight) / 2;
                
            context.Crop(new Rectangle(0, (int) y, size.Width, (int) newHeight));
        }

        return context;
    }
}