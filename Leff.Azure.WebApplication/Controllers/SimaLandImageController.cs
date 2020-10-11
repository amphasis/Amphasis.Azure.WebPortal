using System.Net.Http;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SkiaSharp;

namespace Leff.Azure.WebApplication.Controllers
{
    [Route("simaland/image")]
    public class SimaLandImageController : Controller
    {
        private const string Image700 = "700-nw.jpg";
        private const string Image1600 = "1600.jpg";

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

        //private static readonly HttpClient HttpClient = new HttpClient {BaseAddress = new Uri("https://cdn2.static1-sima-land.com/items/")};

        private readonly HttpClient _httpClient;

        public SimaLandImageController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient(nameof(SimaLandImageController));
        }

        [HttpGet]
        [Route("{goodId}/{imageIndex}")]
        public async Task<IActionResult> DecodeImageAsync(int goodId, int imageIndex)
        {
            using var image700 = await GetImageAsync(goodId, imageIndex, Image700);
            using var image1600 = await GetImageAsync(goodId, imageIndex, Image1600);
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

        private async Task<SKBitmap> GetImageAsync(int goodId, int imageIndex, string name)
        {
            var uri = $"{goodId}/{imageIndex}/{name}";
            await using var stream = await _httpClient.GetStreamAsync(uri);
            return SKBitmap.Decode(stream);
        }
    }
}