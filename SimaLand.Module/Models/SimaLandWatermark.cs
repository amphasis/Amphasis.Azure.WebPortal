using SkiaSharp;

namespace Amphasis.Azure.SimaLand.Models;

public static class SimaLandWatermark
{
	private const int ImageSize = 1600;

	private const int X = 40;
	private const int Y = 40;
	private const int Width = 480;
	private const int Height = 56;

	private const float ReducedX = (float) X / ImageSize;
	private const float ReducedY = (float) Y / ImageSize;
	private const float ReducedWidth = (float) Width / ImageSize;
	private const float ReducedHeight = (float) Height / ImageSize;

	public static SKRect GetSkRect(int imageSize)
	{
		var x = ReducedX * imageSize;
		var y = ReducedY * imageSize;
		var width = ReducedWidth * imageSize;
		var height = ReducedHeight * imageSize;
		return SKRect.Create(x, y, width, height);
	}
}