using LizRootheyMakes_API.Data;
using LizRootheyMakes_API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using System.Net;

namespace LizRootheyMakes_API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class PaymentController : ControllerBase
	{
		protected ApiResponse _response;
		private readonly ApplicationDbContext _db;
		private readonly IConfiguration _configuration;

        public PaymentController(IConfiguration configuration, ApplicationDbContext db)
        {
			_configuration = configuration;
			_db = db;
			_response = null;

		}


		[HttpPost]
        public async Task<ActionResult<ApiResponse>> MakePayment(string userID) 
		{

			ShoppingCart shoppingCart = _db.ShoppingCarts
				.Include(u => u.CartItems)
				.ThenInclude(u => u.MenuItem).FirstOrDefault(u => u.UserId == userID);

			if (shoppingCart == null || shoppingCart.CartItems == null || shoppingCart.CartItems.Count == 0) 
			{
				_response.IsSuccess  = false;
				_response.StatusCode = HttpStatusCode.BadRequest;
				return BadRequest(_response);
			}


			#region Create Payment Intent

			StripeConfiguration.ApiKey = _configuration["StripeSettings:SecretKey"];

			//get the cart total value
			shoppingCart.CartTotal = shoppingCart.CartItems.Sum( u=> u.Quantity * u.MenuItem.Price);



			var options = new PaymentIntentCreateOptions
			{
				Amount = (int)(shoppingCart.CartTotal * 100),
				Currency = "usd",
				PaymentMethodTypes = new List<string>
				{
					"card",
				},
				//AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
				//{
				//	Enabled = true,
				//},
			};

			PaymentIntentService service = new PaymentIntentService();
			PaymentIntent response = service.Create(options);
			shoppingCart.StripePaymentIntentId = response.Id;
			shoppingCart.ClientSecret = response.ClientSecret;

			#endregion


			_response.IsSuccess = true;
			_response.StatusCode = HttpStatusCode.OK;
			return Ok(_response);
		
		}
    }
}
