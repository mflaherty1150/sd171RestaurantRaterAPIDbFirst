using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantRaterAPI.Data;
using RestaurantRaterAPI.Models;

namespace RestaurantRaterAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class RestaurantController : ControllerBase
{
    private readonly RestaurantDbContext _context;
    public RestaurantController(RestaurantDbContext context)
    {
        _context = context;
    }

    // Async POST Endpoint
    [HttpPost]
    public async Task<IActionResult> PostRestaurantAsync([FromBody] Restaurant request)
    {
        if (ModelState.IsValid)
        {
            _context.Restaurants.Add(request);
            await _context.SaveChangesAsync();
            return Ok();
        }

        return BadRequest(ModelState);
    }

    // Async GET Endpoint
    [HttpGet]
    public async Task<IActionResult> GetRestaurantsAsync()
    {
        var restaurants = await _context.Restaurants.Include(r => r.Ratings).ToListAsync();
        List<RestaurantListItem> restaurantList = restaurants.Select(r => new RestaurantListItem() {
            Id = r.Id,
            Name = r.Name,
            Location = r.Location,
            AverageScore = r.AverageRating,
        }).ToList();
        return Ok(restaurantList);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetRestaurantByIdAsync(int id)
    {
        Restaurant? restaurant = await _context.Restaurants.Include(r => r.Ratings).FirstOrDefaultAsync(r => r.Id == id);

        if (restaurant is null)
        {
            return NotFound();
        }

        return Ok(restaurant);
    }

    [HttpPut]
    [Route("{id}")]
    public async Task<IActionResult> UpdateRestaurantAsync([FromForm] RestaurantEdit model, [FromRoute] int id)
    {
        var oldRestaurant = await _context.Restaurants.FindAsync(id);

        if (oldRestaurant == null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        if (!string.IsNullOrEmpty(model.Name))
        {
            oldRestaurant.Name = model.Name;
        }
        if (!string.IsNullOrEmpty(model.Location))
        {
            oldRestaurant.Location = model.Location;
        }

        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpDelete]
    [Route("{id}")]
    public async Task<IActionResult> DeleteRestaurantAsync([FromRoute] int id)
    {
        var restaurant = await _context.Restaurants.FindAsync(id);
        if (restaurant == null)
        {
            return NotFound();
        }

        _context.Restaurants.Remove(restaurant);
        await _context.SaveChangesAsync();
        return Ok();
    }
}