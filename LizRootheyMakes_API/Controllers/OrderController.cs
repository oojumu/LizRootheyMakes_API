using Azure;
using LizRootheyMakes_API.Data;
using LizRootheyMakes_API.Models;
using LizRootheyMakes_API.Models.DTO;
using LizRootheyMakes_API.Services;
using LizRootheyMakes_API.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace LizRootheyMakes_API.Controllers
{
	
	[Route("api/[controller]")]
	[ApiController]
	public class OrderController : ControllerBase
	{
		private readonly ApplicationDbContext _db;
		private readonly IBlobService _blobService;

		private ApiResponse _response;
		public OrderController(ApplicationDbContext db, IBlobService blobService)
		{
			_db = db;
			_response = new ApiResponse();
			_blobService = blobService;
		}

		[HttpGet]
		public async Task<ActionResult<ApiResponse>> GetOrders(string? userId)
		{
			try
			{
				var orderHeaders =  _db.OrderHeaders.Include(k => k.OrderDetails)
					.ThenInclude(k => k.MenuItem)
					.OrderByDescending(k => k.OrderHeaderId);

				if (!string.IsNullOrEmpty(userId))
				{
					_response.Result = orderHeaders.Where(u => u.ApplicationUserId == userId);//.ToList();
				}
				else
				{
					_response.Result = orderHeaders;
				}
				_response.StatusCode = HttpStatusCode.OK;
				return Ok(_response);
			}
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.ErrorMessages = new List<string>() { ex.ToString() };
			}

			return _response;

		}

		[HttpGet("{id:int}")]
		public async Task<ActionResult<ApiResponse>> GetOrder(int Id)
		{
			try
			{
				if (Id == 0)
				{
					_response.StatusCode = HttpStatusCode.BadRequest;
					_response.IsSuccess = false;
					return BadRequest(_response);
				}

				var orderHeaders =  _db.OrderHeaders.Include(k => k.OrderDetails)
					.ThenInclude(k => k.MenuItem)
					.Where(k => k.OrderHeaderId == Id);

				if (orderHeaders == null)
				{
					_response.StatusCode = HttpStatusCode.NotFound;
					return NotFound(_response);
				}

				_response.Result = orderHeaders;
				_response.StatusCode = HttpStatusCode.OK;
				return Ok(_response);

			}
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.ErrorMessages = new List<string>() { ex.ToString() };
			}

			return _response;

		}


		[HttpPost]
		public async Task<ActionResult<ApiResponse>> CreateOrder([FromBody] OrderHeaderCreateDTO orderHeaderDTO)
		{
			try
			{
                OrderHeader order = new()
				{
					ApplicationUserId = orderHeaderDTO.ApplicationUserId,
					PickupEmail = orderHeaderDTO.PickupEmail,
					PickupName 	= orderHeaderDTO.PickupName,
					PickupPhoneNumber = orderHeaderDTO.PickupPhoneNumber,
					OrderTotal = orderHeaderDTO.OrderTotal,
					OrderDate = DateTime.Now,
					StripePaymentIntentID = orderHeaderDTO.StripePaymentIntentID,	
					TotalItems = orderHeaderDTO.TotalItems,
					Status = String.IsNullOrEmpty(orderHeaderDTO.Status)? SD.status_pending : orderHeaderDTO.Status,
				};

				if (ModelState.IsValid) 
				{
					_db.OrderHeaders.Add(order);
					_db.SaveChanges();

					foreach(var orderDetailDTO in orderHeaderDTO.OrderDetailsDTO)
					{
						OrderDetails orderDetails = new()
						{
							OrderHeaderId = order.OrderHeaderId,
							ItemName	  = orderDetailDTO.ItemName,
							MenuItemId    = orderDetailDTO.MenuItemId,
							Price		  = orderDetailDTO.Price,
							Quantity = orderDetailDTO.Quantity,

						};

						_db.OrderDetails.Add(orderDetails);						
					}
					_db.SaveChanges();

					order.OrderDetails = null;
					_response.Result = order;
					_response.StatusCode = HttpStatusCode.Created;
					return Ok(_response);
				}
			}
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.ErrorMessages = new List<string>() { ex.ToString() };
			}

			return _response;
		}
	}

	
}
