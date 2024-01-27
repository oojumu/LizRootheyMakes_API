using LizRootheyMakes_API.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace LizRootheyMakes_API.Models
{
	public class ShoppingCart
	{
        public int Id { get; set; }
		public string UserId { get; set; }
		public string ClientSecret { get; set; }
        public ICollection<CartItem> CartItems { get; set; }

		[NotMapped]
        public double CartTotal { get; set; }
        [NotMapped]
		public string StripePaymentIntentId { get; set; }
	}



}


