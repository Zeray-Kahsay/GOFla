using System;
using GoFla.API.Commons;

namespace GoFla.API.Services;

public interface ICloudinaryService
{
    Task<Result<string>> UploadImageAsync(IFormFile file, string folder, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteImageAsync(string publicId, CancellationToken cancellationToken = default);
}
