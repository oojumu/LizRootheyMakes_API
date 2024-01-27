using LizRootheyMakes_API.Data;
using LizRootheyMakes_API.Models;
using LizRootheyMakes_API.Models.DTO;
using LizRootheyMakes_API.Services;
using LizRootheyMakes_API.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace LizRootheyMakes_API.Controllers
{
	[Route("api/auth")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly ApplicationDbContext _db;
		private string secretKey;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;


		//private readonly IBlobService _blobService;

		private ApiResponse _response;
		public AuthController(ApplicationDbContext db, IConfiguration configuration, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
		{
			_db = db;
			_response = new ApiResponse();
			secretKey = configuration.GetValue<string>("ApiSettings:Secret");
			_userManager = userManager;
			_roleManager = roleManager;
			//_blobService = blobService;
		}

		[HttpPost("register")]
		public async Task <IActionResult> RegisterUser([FromBody] RegisterationRequestDTO model)
		{
			ApplicationUser user = _db.ApplicationUsers.FirstOrDefault(u => u.UserName.ToLower() == model.UserName.ToLower());

			if (user != null) 
			{
				_response.StatusCode = HttpStatusCode.BadRequest;
				_response.IsSuccess = false;
				_response.ErrorMessages.Add("Username already exists");
				return BadRequest(_response);
			}

			ApplicationUser newUser = new()
			{
				UserName = model.UserName,
				Email =	model.UserName,
				NormalizedEmail = model.UserName.ToUpper(),
				Name = model.Name
			};

			try
			{
				var result = await _userManager.CreateAsync(newUser, model.Password);

				if (result.Succeeded)
				{

					if (!_roleManager.RoleExistsAsync(SD.Role_Admin).GetAwaiter().GetResult())
					{
						await _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin));
						await _roleManager.CreateAsync(new IdentityRole(SD.Role_Customer));
					}

					if (model.Role == SD.Role_Admin)
					{
						await _userManager.AddToRoleAsync(newUser, SD.Role_Admin);
					}
					else
					{

						await _userManager.AddToRoleAsync(newUser, SD.Role_Customer);

					}

					_response.StatusCode = HttpStatusCode.OK;
					_response.IsSuccess = true;
					return Ok(_response);
				}
				else
				{
					_response.StatusCode = HttpStatusCode.BadRequest;
					_response.IsSuccess = false;
				}
			}
			catch (Exception ex)
			{
				_response.StatusCode = HttpStatusCode.BadRequest;
				_response.IsSuccess = false;
			}
			


			_response.StatusCode = HttpStatusCode.OK;
			_response.IsSuccess = true;
			return  Ok(_response);
		}


		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginRequestDTO model)
		{

			if (model.UserName == null || model.Password == null)
			{
				_response.IsSuccess = false;
				_response.StatusCode = HttpStatusCode.BadRequest;
			}

			var userExists = _db.ApplicationUsers.FirstOrDefault(b => b.UserName.ToLower() == model.UserName.ToLower());

			if (userExists == null)
			{
				_response.IsSuccess = false;
				_response.StatusCode = HttpStatusCode.BadRequest;
				_response.ErrorMessages.Add("User does not exist");
				return BadRequest(_response);

			}
			else
			{
				bool userValidated = await _userManager.CheckPasswordAsync(userExists, model.Password);
				
				if (userValidated == false) 
				{
					_response.Result = new LoginResponseDTO();
					_response.IsSuccess = false;
					_response.StatusCode = HttpStatusCode.BadRequest;
					_response.ErrorMessages.Add("invalid username or password");
					return BadRequest(_response);
				}
				else 
				{
					//generate JWT Token
					var role = await _userManager.GetRolesAsync(userExists);

					JwtSecurityTokenHandler tokenHandler = new ();
					byte[] key = Encoding.ASCII.GetBytes(secretKey);

					SecurityTokenDescriptor tokenDescriptor = new()
					{
						Subject = new ClaimsIdentity(new Claim[]
						{
							new Claim("fullName", userExists.Name),
							new Claim("id", userExists.Id.ToString()),
							new Claim(ClaimTypes.Email, userExists.UserName),
							new Claim(ClaimTypes.Role, userExists.UserName)
						}),

						//how long is it valid is defined here
						Expires = DateTime.UtcNow.AddDays(7),
						SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
					};

					SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

					LoginResponseDTO loginResponse = new()
					{
						Email = userExists.Email,
						Token = tokenHandler.WriteToken(token)
					};

					if (loginResponse.Email == null || string.IsNullOrEmpty(loginResponse.Email)) 
					{
						_response.IsSuccess = false;
						_response.StatusCode = HttpStatusCode.BadRequest;
						_response.ErrorMessages.Add("invalid username or password");
						return BadRequest(_response);
					}

					_response.StatusCode = HttpStatusCode.OK;
					_response.IsSuccess = true;
					_response.Result = loginResponse;
					return Ok(_response);
				}

			}

			//&&)


			return Ok(_response);
		}
	}
}
