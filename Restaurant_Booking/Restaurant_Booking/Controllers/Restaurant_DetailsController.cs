//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Restaurant_Booking.Data;
//using Restaurant_Booking.Models;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace Restaurant_Booking.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class Restaurant_DetailsController : ControllerBase
//    {
//        private readonly Restaurant_BookingDbContext _context;

//        public Restaurant_DetailsController(Restaurant_BookingDbContext context)
//        {
//            _context = context;
//        }

//        // GET: api/restaurant_details
//        [HttpGet]
//        public async Task<ActionResult<IEnumerable<Restaurant>>> GetRestaurantDetails()
//        {
//            return await _context.Restaurant.ToListAsync();
//        }

//        // GET: api/restaurant_details/5
//        [HttpGet("{id}")]
//        public async Task<ActionResult<Restaurant>> GetRestaurantDetails(int id)
//        {
//            var restaurantDetails = await _context.Restaurant.FindAsync(id);

//            if (restaurantDetails == null)
//            {
//                return NotFound();
//            }

//            return restaurantDetails;
//        }

//        // POST: api/restaurant_details
//        [HttpPost]
//        public async Task<ActionResult<Restaurant>> PostRestaurantDetails(Restaurant restaurantDetails)
//        {
//            if (await _context.Restaurant.AnyAsync(c => c.Email_Id == restaurantDetails.Email_Id))
//            {
//                return Conflict("Email Id already exists");
//            }
//            _context.Restaurant.Add(restaurantDetails);
//            await _context.SaveChangesAsync();

//            return CreatedAtAction("GetRestaurantDetails", new { id = restaurantDetails.Restaurant_Id }, restaurantDetails);
//        }

//        // PUT: api/restaurant_details/5
//        [HttpPut("{id}")]
//        public async Task<IActionResult> PutRestaurantDetails(int id, Restaurant restaurantDetails)
//        {
//            if (id != restaurantDetails.Restaurant_Id)
//            {
//                return BadRequest();
//            }

//            _context.Entry(restaurantDetails).State = EntityState.Modified;

//            try
//            {
//                await _context.SaveChangesAsync();
//            }
//            catch (DbUpdateConcurrencyException)
//            {
//                if (!RestaurantDetailsExists(id))
//                {
//                    return NotFound();
//                }
//                else
//                {
//                    throw;
//                }
//            }

//            return NoContent();
//        }

//        // DELETE: api/restaurant_details/5
//        [HttpDelete("{id}")]
//        public async Task<ActionResult<Restaurant>> DeleteRestaurantDetails(int id)
//        {
//            var restaurantDetails = await _context.Restaurant.FindAsync(id);
//            if (restaurantDetails == null)
//            {
//                return NotFound();
//            }

//            _context.Restaurant.Remove(restaurantDetails);
//            await _context.SaveChangesAsync();

//            return restaurantDetails;
//        }

//        private bool RestaurantDetailsExists(int id)
//        {
//            return _context.Restaurant.Any(e => e.Restaurant_Id == id);
//        }
//    }
//}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using Restaurant_Booking.Data;
using Restaurant_Booking.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Restaurant_Booking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Restaurant_DetailsController : ControllerBase
    {

        private readonly Restaurant_BookingDbContext _restaurantdetails;

        private readonly IWebHostEnvironment _environment;

        private readonly IConfiguration _configuration;





        public Restaurant_DetailsController(Restaurant_BookingDbContext restaurantdetails, IWebHostEnvironment environment, IConfiguration configuration)

        {

            _restaurantdetails = restaurantdetails;

            _environment = environment;

            _configuration = configuration;

        }
        [HttpGet]

        public IActionResult GetAllCart()
        {
            var carts = _restaurantdetails.Restaurant.ToList();

            var cartList = new List<object>();

            foreach (var menu in carts)
            {
                var cartData = new
                {

                    Restaurant_Id = menu.Restaurant_Id,
                    Restaurant_Name = menu.Restaurant_Name,
                    Email_Id = menu.Email_Id,
                    ContactNumber = menu.ContactNumber,
                    Location = menu.Location,
                    Type = menu.Type,
                    Cuisine = menu.Cuisine,
                    TotalTables = menu.TotalTables,
                    Status = menu.Status,
                    Personal_Email = menu.Personal_Email,
                    UniqueFileName = menu.UniqueFileName,
                    imageUrl = String.Format("{0}://{1}{2}/wwwroot/images/{3}", Request.Scheme, Request.Host, Request.PathBase, menu.UniqueFileName)
                };

                cartList.Add(cartData);
            }

            return Ok(cartList);
        }

        // GET: api/menu

        [HttpGet("{Item_Id}")]

        public async Task<Restaurant> GetById(int id)

        {

            return await _restaurantdetails.Restaurant.FindAsync(id);

        }

        [HttpGet("{id}/Image")]

        public IActionResult GetImage(int id)

        {

            var request = _restaurantdetails.Restaurant.Find(id);

            if (request == null)

            {

                return NotFound(); // User not found 

            }



            // Construct the full path to the image file 

            var imagePath = Path.Combine(_environment.WebRootPath, "images", request.UniqueFileName);



            // Check if the image file exists 

            if (!System.IO.File.Exists(imagePath))

            {

                return NotFound(); // Image file not found 

            }



            // Serve the image file 

            return PhysicalFile(imagePath, "image/jpeg");

        }

        //// GET: api/menu/5
        //[HttpGet("{id}")]
        //public async Task<ActionResult<Menu>> GetMenu(int id)
        //{
        //    var menu = await _menudetails.Menu.FindAsync(id);

        //    if (menu == null)
        //    {
        //        return NotFound();
        //    }

        //    return menu;
        //}


        // POST: api/menu
        [HttpPost]
        public async Task<ActionResult<Restaurant>> PostMenu(Restaurant restaurant)
        {
            var uniqueFileName = $"{Guid.NewGuid()}_{restaurant.RestaurantImage.FileName}";



            // Save the image to a designated folder (e.g., wwwroot/images) 
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "images");

            var filePath = Path.Combine(uploadsFolder, uniqueFileName);



            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await restaurant.RestaurantImage.CopyToAsync(stream);
            }



            // Store the file path in the database 
            restaurant.UniqueFileName = uniqueFileName;
            Restaurant food = new()
            {
                   Restaurant_Id = restaurant.Restaurant_Id,
                    Restaurant_Name = restaurant.Restaurant_Name,
                    Email_Id = restaurant.Email_Id,
                    ContactNumber = restaurant.ContactNumber,
                    Location = restaurant.Location,
                    Type = restaurant.Type,
                    Cuisine = restaurant.Cuisine,
                    TotalTables = restaurant.TotalTables,
                    Status = restaurant.Status,
                    Personal_Email = restaurant.Personal_Email,
                    UniqueFileName = restaurant.UniqueFileName,
               


            };



            _restaurantdetails.Restaurant.Add(food);
            await _restaurantdetails.SaveChangesAsync();



            return Ok();



        }

        // PUT: api/menu/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMenu(int id, Menu menu)
        {
            if (id != menu.MenuID)
            {
                return BadRequest();
            }

            _restaurantdetails.Entry(menu).State = EntityState.Modified;

            try
            {
                await _restaurantdetails.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MenuExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/menu/5
        [HttpDelete("{id}")]



        public async Task<IActionResult> DeleteItemDetails(int id)



        {



            var mobdetails = _restaurantdetails.Restaurant.Find(id);



            if (mobdetails == null)



            {



                return NotFound(); // PetAccessory not found 



            }



            _restaurantdetails.Restaurant.Remove(mobdetails);



            await _restaurantdetails.SaveChangesAsync();



            return NoContent(); // Successfully deleted 



        }
        private bool MenuExists(int id)
        {
            return _restaurantdetails.Restaurant.Any(e => e.Restaurant_Id == id);
        }
    }
}
