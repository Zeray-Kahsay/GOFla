using System;
using System.Net;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using GoFla.API.Commons;
using GoFla.API.Configuration;
using Microsoft.Extensions.Options;

namespace GoFla.API.Services;

public class ImageStorage : IImageStorage
{
    private readonly Cloudinary _cloudinary;

    public ImageStorage(IOptions<CloudinarySettings> options)
    {
        var account = new Account(
            options.Value.CloudName,
            options.Value.ApiKey,
            options.Value.ApiSecret
        );

        _cloudinary = new Cloudinary(account);
    }


    public async Task<string> UploadImageAsync(int restaurantId, IFormFile file, CancellationToken cancellationToken)
    {
        if (!file.ContentType.StartsWith("image/"))
            throw new InvalidOperationException("Invalid image type");
        
        await using var stream = file.OpenReadStream();

        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(file.FileName, stream),
            Folder = "restaurants",
            PublicId = $"restaurant_{restaurantId}",
            Overwrite = true,
            Transformation = new Transformation()
                .Width(1200)
                .Height(800)
                .Crop("fill")
                .Quality("auto")
                .FetchFormat("auto")
        };

        var result = await _cloudinary.UploadAsync(uploadParams, cancellationToken);

        if (result.StatusCode != HttpStatusCode.OK)
        {
                throw new Exception("Cloudinary upload failed");
        }

        return result.SecureUrl.ToString();
    }


    public async Task<Result<bool>> DeleteImageAsync(string publicId, CancellationToken cancellationToken = default)
    {
        var deleteParams = new DeletionParams(publicId);

        var result = await _cloudinary.DestroyAsync(deleteParams);

        if (result.Result != "ok" && result.Result != "not found")
        {
            return Result<bool>.Failure("Failed to delete image from storage", "DELETE_FAILED");
        }

        return Result<bool>.Success(true);
    }
}
