using LizRootheyMakes_API.Data;
using LizRootheyMakes_API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace LizRootheyMakes_API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ShoppingCartController : ControllerBase
	{
        protected ApiResponse _response;
        private readonly ApplicationDbContext _db;
        public ShoppingCartController(ApplicationDbContext db)
        {
            _response = new ();
            _db = db;
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse>> AddOrUpdateItemInCart(string userID, int menuItemId, int updateQtyBy)
        {
			/* Some Ground Rules to be laid from this point
             * 
             * The Shopping cart would have only one entry per userId, even if a user has many items in the cart.
             * Cart Items will have all the items in shoppong cart for a user
             * Update Qty by will have count by which an items qty needs to be updated
             * If it is -1, that implies we are reducing a count; if it is 5, it implies, we are increasing existing count by 5
             * If updateQtyBy is set to zero, the item would be removed
             */
			//_db.shopp


			// Scenarios

			// when a user adds a new item to a new shopping cart for the first time
			// when a user adds a new item to an existing shopping cart (basically user has other items in cart)
			// when a user updates an existing item count
			// when a user removes an item

            ShoppingCart shoppingCart = _db.ShoppingCarts.FirstOrDefault( v=> v.UserId == userID );
            MenuItem menuItem = _db.MenuItems.FirstOrDefault(h => h.Id == menuItemId);

            if (menuItem == null) 
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;                
            }

            if (shoppingCart == null && updateQtyBy > 0)
            { 
                //this is a new shoping cart entry
                ShoppingCart newShoppingCart = new()
                {
                    UserId = userID
                };

                _db.ShoppingCarts.Add(newShoppingCart);
                _db.SaveChanges();

                CartItem newCartItem = new()
                {
                    MenuItemId = menuItemId,
                    Quantity = updateQtyBy,
                    ShoppingCartId = newShoppingCart.Id,
                    MenuItem = null
                };
            
                _db.CartItems.Add(newCartItem);
                _db.SaveChanges();
				_response.StatusCode = HttpStatusCode.OK;
				_response.IsSuccess = true;
			}
            else 
            {
                //check if the menu item(s) exist in the shopping cart
                CartItem cartIteminCart = shoppingCart.CartItems.FirstOrDefault(f=> f.MenuItemId == menuItemId);

                if (cartIteminCart == null) 
                {
					// cartitem does not exist, create it afresh

					CartItem newCartItem = new()
					{
						MenuItemId = menuItemId,
						Quantity = updateQtyBy,
						ShoppingCartId = shoppingCart.Id,
						MenuItem = null
					};

					_db.CartItems.Add(newCartItem);
					_db.SaveChanges();
				}
                else
                {
                    int newQuantity = cartIteminCart.Quantity + updateQtyBy;

                    if (updateQtyBy == 0  || newQuantity <= 0)
                    {
                        // remove cart item from cart and if it is the only item then remove cart
                        _db.CartItems.Remove(cartIteminCart);

                        if(shoppingCart.CartItems.Count() == 1)
                        {
                            _db.ShoppingCarts.Remove(shoppingCart);
                        }

						_db.SaveChanges();

					}
                    else
                    {

                        cartIteminCart.Quantity = newQuantity;
						_db.SaveChanges();

					}

					//CartItem newCartItem = new()
					//{
					//	MenuItemId = menuItemId,
					//	Quantity = updateQtyBy,
					//	ShoppingCartId = shoppingCart.Id,
					//	MenuItem = null
					//};

					//_db.CartItems.Update(newCartItem);
					//_db.SaveChanges();
				}


            
            }
			return Ok(_response);
        }

        //public async Task<ActionResult> GetShoppingCart(string userId)
        //{
        //    if(string.IsNullOrEmpty(userId))
        //    { }



        //}
    }
}
