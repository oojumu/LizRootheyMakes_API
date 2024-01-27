using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LizRootheyMakes_API.Controllers
{
	[Route("api/AuthTest")]
	[ApiController]
	public class AuthTestController : ControllerBase
	{
		[HttpGet]
		[Authorize]
		public async Task<ActionResult<string>> GetSemething()
		{
			return "You are Authenticated";
		}

		[HttpGet("{id:int}")]
		public async Task<ActionResult<string>> GetSemething(int smthg)
		{

			return "You are Authorized with Admin role";
		}
	}
}
