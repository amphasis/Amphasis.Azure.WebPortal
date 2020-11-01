using System.Net.Mime;
using System.Threading.Tasks;
using Amphasis.Azure.WebPortal.SimaLand.Models;
using Amphasis.Azure.WebPortal.SimaLand.Models.Enums;
using Amphasis.Azure.WebPortal.SimaLand.Services;
using Microsoft.AspNetCore.Mvc;
using SkiaSharp;

namespace Amphasis.Azure.WebPortal.Controllers
{
    [Route("simaland/image")]
    public class SimaLandImageController : Controller
    {
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

#if !DEBUG
        [ResponseCache(Duration = 86400, Location = ResponseCacheLocation.Any)]
#endif
        [HttpGet("{goodId}/{imageIndex}")]
        public async Task<IActionResult> DeWatermark([FromRoute] int goodId, [FromRoute] int imageIndex)
        {
            using var image700 = await GetImageAsync(goodId, imageIndex, SimaLandImageSize.Quad700);
            using var image1600 = await GetImageAsync(goodId, imageIndex, SimaLandImageSize.Quad1600Watermark);

            using var canvas = new SKCanvas(image1600);
            var sourceRect = SimaLandWatermark.GetSkRect(700);
            var destRect = SimaLandWatermark.GetSkRect(1600);
            canvas.DrawBitmap(image700, sourceRect, destRect, Paint);

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