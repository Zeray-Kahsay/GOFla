using System;
using GoFla.API.Commons;

namespace GoFla.API.Services;

public class CloudinaryService : ICloudinaryService
{
    public Task<Result<bool>> DeleteImageAsync(string publicId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<string>> UploadImageAsync(IFormFile file, string folder, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
