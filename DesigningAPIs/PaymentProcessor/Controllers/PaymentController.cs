using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PaymentProcessor.Models;
using PaymentProcessor.Services;

namespace PaymentProcessor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            this.paymentService = paymentService;
        }

        [HttpGet]
        public IActionResult Get(OrderCheckoutModel model)
        {
            //validate
            //transform to call external
            //await external call to WackyPayments. if successfull

                //save payment trace in PaymentProcessorDB either as failed or successfull
            //Ok ();
            //NotOK()
            //}
            //notify OrderApi and change OrderStatus in a PUT- or call ORders Endpoint-external servie


            //
            return Ok("Payment Processor API");
        }
    }
}
