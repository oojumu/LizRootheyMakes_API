﻿using LizRootheyMakes_API.Data;
using LizRootheyMakes_API.Models;
using LizRootheyMakes_API.Models.DTO;
using LizRootheyMakes_API.Services;
using LizRootheyMakes_API.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace LizRootheyMakes_API.Controllers
{
	[Route("api/MenuItem")]
	[ApiController]

	
	public class MenuItemController : ControllerBase
	{
		private readonly ApplicationDbContext _db;
		private readonly IBlobService _blobService;

		private ApiResponse _response;
		public MenuItemController(ApplicationDbContext db, IBlobService blobService)
		{
			_db = db;
			_response = new ApiResponse();
			_blobService = blobService;
		}

		[HttpGet]
		[Authorize]
		public async Task<IActionResult> GetMenuItem()
		{
			_response.Result = _db.MenuItems;
			_response.StatusCode = HttpStatusCode.OK;
			return Ok(_response);
		}

		[HttpGet("{id:int}", Name = "GetMenuItem")]
		public async Task<IActionResult> GetMenuById(int id)
		{
			if (id == 0)
			{
				_response.StatusCode = HttpStatusCode.BadRequest;
				_response.IsSuccess = false;
				return BadRequest(_response);
			}

			MenuItem menuItems = _db.MenuItems.FirstOrDefault(v => v.Id == id);

			if (menuItems == null)
			{
				_response.StatusCode = HttpStatusCode.NotFound;
				_response.IsSuccess = false;
				return NotFound(_response);
			}
			_response.Result = menuItems;
			_response.StatusCode = HttpStatusCode.OK;
			return Ok(_response);
		}


		[HttpPost]
		public async Task<ActionResult<ApiResponse>> CreateMenuItem([FromForm] MenuItemCreateDTO menuItemCreateDTO)
		{			

			try
			{
				if (ModelState.IsValid)
				{
					if (menuItemCreateDTO.File == null || menuItemCreateDTO.File.Length == 0)
					{
						_response.StatusCode = HttpStatusCode.BadRequest;
						_response.IsSuccess = false;
						return BadRequest();
					}

					string fileName = $"{Guid.NewGuid()}{Path.GetExtension(menuItemCreateDTO.File.FileName)}";

					MenuItem menuItemToCreate = new()
					{
						Name = menuItemCreateDTO.Name,
						Price = menuItemCreateDTO.Price,
						Category = menuItemCreateDTO.Category,
						SpecialTag = menuItemCreateDTO.SpecialTag,
						Description = menuItemCreateDTO.Description,
						Image = await _blobService.UpdateBlob(fileName,SD.SD_Storage_Container,menuItemCreateDTO.File)

					};
					_db.MenuItems.Add(menuItemToCreate);
					_db.SaveChanges();

					_response.Result = menuItemToCreate;
					_response.StatusCode = HttpStatusCode.Created;
					return CreatedAtRoute("GetMenuItem", new { id = menuItemToCreate.Id} , _response);
				}
				else
                {
					_response.IsSuccess = false;
                }


			}
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.ErrorMessages = new List<string>() { ex.ToString()  };
			}

			return _response;

		}


		[HttpPut]
		public async Task<ActionResult<ApiResponse>> UpdateMenuItem(int id, [FromForm] MenuItemUpdateDTO menuItemUpdateDTO)
		{

			try
			{
				if (ModelState.IsValid)
				{
					if (menuItemUpdateDTO == null || id != menuItemUpdateDTO.Id)
					{	
						_response.StatusCode = HttpStatusCode.BadRequest;
						_response.IsSuccess = false;
						return BadRequest();
					}
					MenuItem menuItemFromDb = await _db.MenuItems.FindAsync(id);

					if (menuItemFromDb == null)
					{
						return BadRequest();
					}

					
					menuItemFromDb.Name = menuItemUpdateDTO.Name;
					menuItemFromDb.Price = menuItemUpdateDTO.Price;
					menuItemFromDb.Category = menuItemUpdateDTO.Category;
					menuItemFromDb.SpecialTag = menuItemUpdateDTO.SpecialTag;
					menuItemFromDb.Description = menuItemUpdateDTO.Description;
					 
					if (menuItemUpdateDTO.File != null || menuItemUpdateDTO.File.Length > 0)
					{
						string fileName = $"{Guid.NewGuid()}{Path.GetExtension(menuItemUpdateDTO.File.FileName)}";
						await _blobService.DeleteBlob(menuItemFromDb.Image.Split('/').Last(), SD.SD_Storage_Container);
						menuItemFromDb.Image = await _blobService.UpdateBlob(fileName, SD.SD_Storage_Container, menuItemUpdateDTO.File);
					}

					_db.MenuItems.Update(menuItemFromDb);
					_db.SaveChanges();

					//_response.Result = menuItemFromDb;
					//_response.StatusCode = HttpStatusCode.OK;
					//return CreatedAtRoute("GetMenuItem", new { id = menuItemFromDb.Id }, _response);

					_db.MenuItems.Update(menuItemFromDb);
					_db.SaveChanges();
					_response.StatusCode = HttpStatusCode.NoContent;
					return Ok(_response); // CreatedAtRoute("GetMenuItem", new { id = menuItemFromDB.Id }, _response);
				}
				else
				{
					_response.IsSuccess = false;
				}


			}
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.ErrorMessages = new List<string>() { ex.ToString() };
			}

			return _response;

		}


		[HttpDelete]
		public async Task<ActionResult<ApiResponse>> DeleteMenuItem(int id)
		{
			try
			{				
					if (id == 0) //menuItemCreateDTO.File == null || menuItemCreateDTO.File.Length == 0)
					{
						_response.StatusCode = HttpStatusCode.BadRequest;
						_response.IsSuccess = false;
						return BadRequest();
					}

					MenuItem menuItemForDelete =  _db.MenuItems.Find(id);

					if (menuItemForDelete == null)
					{
						_response.StatusCode = HttpStatusCode.BadRequest;
						_response.IsSuccess = false;
						return BadRequest();

					}
					
					 await _blobService.DeleteBlob(menuItemForDelete.Image.Split('/').Last(), SD.SD_Storage_Container);
					
					//delay for a few seconds
					int milliseconds = 2000;
					Thread.Sleep(milliseconds) ;
					
					_db.MenuItems.Remove(menuItemForDelete);
					_db.SaveChanges();
					_response.StatusCode = HttpStatusCode.NoContent;
					return Ok(_response); // CreatedAtRoute("GetMenuItem", new { id = menuItemToCreate.Id }, _response);				

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
