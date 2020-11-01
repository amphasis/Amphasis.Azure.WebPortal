using System;
using System.Security.Cryptography;
using System.Text;
using Amphasis.Azure.WebPortal.Extensions;
using Amphasis.Azure.WebPortal.Yandex.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Amphasis.Azure.WebPortal.Controllers
{
    [Route("payment/yandex")]
    public class YandexMoneyController : Controller
    {
        private readonly YandexConfiguration _yandexConfiguration;

        public YandexMoneyController(IOptions<YandexConfiguration> yandexConfigurationOptions)
        {
            _yandexConfiguration = yandexConfigurationOptions.Value;
        }

        [HttpPost("confirm")]
        public IActionResult Confirm([FromForm] PaymentConfirmationModel paymentConfirmation)
        {
            if (!IsSecretHashValid(paymentConfirmation)) return StatusCode(StatusCodes.Status403Forbidden);
            if (!ModelState.IsValid) return BadRequest(ModelState);

            return Ok();
        }

        private bool IsSecretHashValid(PaymentConfirmationModel paymentConfirmation)
        {
            string[] parameters =
            {
                paymentConfirmation.NotificationType,
                paymentConfirmation.OperationId,
                paymentConfirmation.AmountString,
                paymentConfirmation.Currency,
                paymentConfirmation.DateTimeString,
                paymentConfirmation.Sender,
                paymentConfirmation.ProtectionCodeSetString,
                _yandexConfiguration.PaymentConfirmationSecret,
                paymentConfirmation.Label
            };

            const char separator = '&';
            var hashString = string.Join(separator, parameters);
            byte[] stringBytesBuffer = Encoding.ASCII.GetBytes(hashString);
            byte[] hashBytes = SHA1.Create().ComputeHash(stringBytesBuffer);
            var hashHexString = hashBytes.ToHexString();

            return string.Equals(paymentConfirmation.Hash, hashHexString, StringComparison.OrdinalIgnoreCase);
        }
    }
}
