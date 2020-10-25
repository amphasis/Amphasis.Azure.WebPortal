using SkiaSharp;

namespace Amphasis.Azure.WebPortal.Models
{
    public static class SimaLandWatermark
    {
        private const int ImageSize = 1600;

        private const int X = 32;
        private const int Y = 32;
        private const int Width = 512;
        private const int Height = 64;

        private const float ReducedX = (float) X / ImageSize;
        private const float ReducedY = (float) Y / ImageSize;
        private const float ReducedWidth = (float) Width / ImageSize;
        private const float ReducedHeight = (float) Height / ImageSize;

        public static SKRect GetSkRect(int imageSize)
        {
            float x = ReducedX * imageSize;
            float y = ReducedY * imageSize;
            float width = ReducedWidth * imageSize;
            float height = ReducedHeight * imageSize;
            return SKRect.Create(x, y, width, height);
        }
    }
}