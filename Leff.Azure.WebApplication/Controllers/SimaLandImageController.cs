using System.Net.Mime;
using System.Threading.Tasks;
using Leff.Azure.WebApplication.Models.Enums;
using Leff.Azure.WebApplication.Services;
using Microsoft.AspNetCore.Mvc;
using SkiaSharp;

namespace Leff.Azure.WebApplication.Controllers
{
    [Route("simaland/image")]
    public class SimaLandImageController : Controller
    {
        private const int Left = 32;
        private const int Top = 32;
        private const int Width = 512;
        private const int Height = 64;

        private const int Right = Left + Width;
        private const int Bottom = Top + Height;

        private const float Ratio = 700.0f / 1600;

        private static readonly SKRect DestRect = new SKRect(Left, Top, Right, Bottom);
        private static readonly SKRect SourceRect = new SKRect(Left * Ratio, Top * Ratio, Right * Ratio, Bottom * Ratio);

        private static readonly SKPaint Paint = new SKPaint {FilterQuality = SKFilterQuality.High};

        private static readonly SKJpegEncoderOptions JpegEncoderOptions =
            new SKJpegEncoderOptions(95, SKJpegEncoderDownsample.Downsample422, SKJpegEncoderAlphaOption.Ignore);

        private readonly SimaLandService _simaLandService;
        private readonly ImageProcessingService _imageProcessingService;

        public SimaLandImageController(
            SimaLandService simaLandService,
            ImageProcessingService imageProcessingService)
        {
            _simaLandService = simaLandService;
            _imageProcessingService = imageProcessingService;
        }

        [HttpGet]
        [Route("{goodId}/{imageIndex}")]
        [ResponseCache(Duration = 86400, Location = ResponseCacheLocation.Any)]
        public async Task<IActionResult> DecodeImageAsync(int goodId, int imageIndex)
        {
            using var image700 = await GetImageAsync(goodId, imageIndex, SimaLandImageSize.Quad700);
            using var image1600 = await GetImageAsync(goodId, imageIndex, SimaLandImageSize.Quad1600Watermark);
            using var canvas = new SKCanvas(image1600);
            canvas.DrawBitmap(image700, SourceRect, DestRect, Paint);
            return ImageToFileStream(image1600);
        }

        private IActionResult ImageToFileStream(SKBitmap bitmap)
        {
            using var pixels = bitmap.PeekPixels();
            var data = pixels.Encode(JpegEncoderOptions);
            return File(data.AsStream(true), MediaTypeNames.Image.Jpeg);
        }

        private async Task<SKBitmap> GetImageAsync(int itemId, int imageIndex, SimaLandImageSize imageSize)
        {
            await using var stream = await _simaLandService.DownloadImageAsync(itemId, imageIndex, imageSize);
            return SKBitmap.Decode(stream);
        }
    }
}