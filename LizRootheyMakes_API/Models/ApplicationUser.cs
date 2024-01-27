using Microsoft.AspNetCore.Identity;

namespace LizRootheyMakes_API.Models
{
	public class ApplicationUser : IdentityUser
	{
		public string Name { get; set;}
	}
}
