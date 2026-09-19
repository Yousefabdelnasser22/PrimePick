using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PrimePick.Core.Services.Contract;
using Stripe;
using System.Threading.Tasks;

namespace PrimePick.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController(IPaymentService paymentService ,IConfiguration _configuration) : ControllerBase
    {
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateIntentId(string cartId)
        {
            var cart = await paymentService.CreateOrUpdatePaymentIntentIdAsync(cartId);
            if (cart is null )
            {
                return BadRequest();
            }
            return Ok(cart);
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body)
                .ReadToEndAsync();

            var stripeSignature = Request.Headers["Stripe-Signature"];

            Event stripeEvent;

            try
            {
                stripeEvent = EventUtility.ConstructEvent(
                    json,
                    stripeSignature,
                    _configuration["Strip:WebhookSecret"]
                );
            }
            catch
            {
                return BadRequest();
            }


            switch (stripeEvent.Type)
            {
                case "payment_intent.succeeded":

                    var paymentIntent =
                        stripeEvent.Data.Object as PaymentIntent;


                   await paymentService.UpdatePaymentIntentForSucceededOrFailed (paymentIntent.Id , true);

                    Console.WriteLine(
                        $"Payment succeeded {paymentIntent.Id}"
                    );

                    break;


                case "payment_intent.payment_failed":

                    var failedPayment =
                        stripeEvent.Data.Object as PaymentIntent;

                    await paymentService.UpdatePaymentIntentForSucceededOrFailed(failedPayment.Id, false);
                    Console.WriteLine(
                        $"Payment failed {failedPayment.Id}"
                    );

                    break;
            }


            return Ok();
        }

    }
}
