using GoFla.API.Domain;
using GoFla.API.Repositories;
using GoFla.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace GoFla.API.Controllers;

public class ImagesController(ICloudinaryService cloudinaryService, IRestaurantRepository restaurantRepository, IRepository<MenuItem> menuItemRepository) : BaseController
{
    [HttpPost("upload")]
    public async Task<IActionResult> UploadImage(IFormFile file, [FromQuery] string folder, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new { Message = "No file provided.", code = "NO_FILE" });
        }

        var result = await cloudinaryService.UploadImageAsync(file, folder, cancellationToken);

        return Ok(result);
    }



    [HttpPost("upload-with-size")]
    public async Task<IActionResult> UploadImageWithSize(
        IFormFile file,
        [FromQuery] string folder,
        [FromQuery] int width,
        [FromQuery] int height,
        CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new { Message = "No file provided.", code = "NO_FILE" });
        }

        var result = await cloudinaryService.UploadImageWithTransformationAsync(file, folder, width, height, cancellationToken);

        return Ok(result);
    }




    [HttpPost("restaurant/{restaurantId}")]
    public async Task<IActionResult> UploadRestaurantImage(IFormFile file, int restaurantId, CancellationToken cancellationToken)
    {
        var restaurant = await restaurantRepository.GetByIdAsync(restaurantId, cancellationToken);
        if (restaurant is null)
        {
            return NotFound(new { Message = "Restaurant not found.", code = "RESTAURANT_NOT_FOUND" });
        }

        // Delete old image if exists
        if (!string.IsNullOrEmpty(restaurant.ImageUrl))
        {
            var publicId = cloudinaryService.GetPublicIdFromUrl(restaurant.ImageUrl);
            if (!string.IsNullOrEmpty(publicId))
            {
                await cloudinaryService.DeleteImageAsync(publicId, cancellationToken);
            }

        }

        // Upload new image
        var uploadResult = await cloudinaryService.UploadImageWithTransformationAsync(file, "restaurants", 800, 600, cancellationToken);

        if (!uploadResult.IsSuccess)
        {
            return BadRequest(new { Message = uploadResult.ErrorMessage, code = uploadResult.ErrorCode });
        }

        // Update restaurant image URL
        restaurant.ImageUrl = uploadResult.Data!;
        restaurant.UpdatedAt = DateTime.UtcNow;
        await restaurantRepository.UpdateAsync(restaurant, cancellationToken);

        return Ok(new { imageUrl = uploadResult.Data });
    }



    [HttpPost("menu-item/{menuItemId}")]
    public async Task<IActionResult> UploadMenuItemImage(int menuItemId, IFormFile file, CancellationToken cancellationToken)
    {
        var menuItem = await menuItemRepository.GetByIdAsync(menuItemId, cancellationToken);
        if (menuItem is null)
        {
            return NotFound(new { Message = "Menu item not found.", code = "MENU_ITEM_NOT_FOUND" });
        }

        // Deleteold old image if exists
        if (!string.IsNullOrEmpty(menuItem.ImageUrl))
        {
            var publicId = cloudinaryService.GetPublicIdFromUrl(menuItem.ImageUrl);
            if (!string.IsNullOrEmpty(publicId))
            {
                await cloudinaryService.DeleteImageAsync(publicId, cancellationToken);
            }
        }

        // Upload new image
        var uploadResult = await cloudinaryService.UploadImageWithTransformationAsync(file, "menu-items", 800, 600, cancellationToken);

        if (!uploadResult.IsSuccess)
        {
            return BadRequest(new { Message = uploadResult.ErrorMessage, code = uploadResult.ErrorCode });
        }

        // Update menu item image URL
        menuItem.ImageUrl = uploadResult.Data!;
        menuItem.UpdatedAt = DateTime.UtcNow;
        await menuItemRepository.UpdateAsync(menuItem, cancellationToken);

        return Ok(new { imageUrl = uploadResult.Data });
    }



    [HttpDelete("{publicId}")]
    public async Task<IActionResult> DeleteImage(string publicId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(publicId))
        {
            return BadRequest(new { Message = "No publicId provided.", code = "NO_PUBLIC_ID" });
        }

        var result = await cloudinaryService.DeleteImageAsync(publicId, cancellationToken);

        return Ok(result);
    }
}
