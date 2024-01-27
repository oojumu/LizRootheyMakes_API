using System.ComponentModel.DataAnnotations;

namespace LizRootheyMakes_API.Models.DTO
{
	public class MenuItemCreateDTO
	{		
		[Required]
		public string Name { get; set; }
		public string Description { get; set; }
		public string SpecialTag { get; set; }
		public string Category { get; set; }

		[Range(1, int.MaxValue)]
		public double Price { get; set; }

		[Required]
		public IFormFile File { get; set; }
		//public int Id { get; set; }
		public DateTime DateCreated { get; set; }
		public DateTime CreatedBy { get; set; }
	}
}
