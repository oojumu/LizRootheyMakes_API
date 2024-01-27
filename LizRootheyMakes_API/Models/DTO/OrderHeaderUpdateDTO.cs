﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LizRootheyMakes_API.Models.DTO
{
	public class OrderHeaderUpdateDTO
	{

		
		public string PickupName { get; set; }
		public string PickupPhoneNumber { get; set; }
		public string PickupEmail { get; set; }
		public string ApplicationUserId { get; set; }
		
		public DateTime OrderDate { get; set; }
		public string StripePaymentIntentID { get; set; }
		public string Status { get; set; }


	}
}
