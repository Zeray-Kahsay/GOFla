using System;
using GoFla.API.Commons;

namespace GoFla.API.Services;

public interface IImageUploadService
{
    Task<string> UploadRestaurantImageAsync(
        int restaurantId,
        IFormFile file,
        CancellationToken cancellationToken
    );

    Task<string> UploadMenuItemImageAsync(
        int menuItemId,
        IFormFile file,
        CancellationToken cancellationToken
    );

    Task<Result<bool>> DeleteImageAsync(
        string publicId,
        CancellationToken cancellationToken = default
    );

}
