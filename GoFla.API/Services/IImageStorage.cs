using System;
using GoFla.API.Commons;

namespace GoFla.API.Services;

public interface IImageStorage
{
    Task<string> UploadImageAsync(
        int restaurantId,
        IFormFile file,
        CancellationToken cancellationToken
    );
    Task<Result<bool>> DeleteImageAsync(
        string publicId,
        CancellationToken cancellationToken = default
    );

}
