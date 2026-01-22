using System;
using System.Net;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using GoFla.API.Commons;
using GoFla.API.Configuration;
using Microsoft.Extensions.Options;

namespace GoFla.API.Services;

public class ImageUploadServie : IImageUploadService
{
    private readonly Cloudinary _cloudinary;

    public ImageUploadServie(IOptions<CloudinarySettings> options)
    {
        var account = new Account(
            options.Value.CloudName,
            options.Value.ApiKey,
            options.Value.ApiSecret
        );

        _cloudinary = new Cloudinary(account);
    }


    public async Task<string> UploadRestaurantImageAsync(int restaurantId, IFormFile file, CancellationToken cancellationToken)
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

        EnsureSuccess(result);

        return result.SecureUrl.ToString();
    }


    public async Task<string> UploadMenuItemImageAsync( int menuItemId, IFormFile file)
    {
        // Validate image
        ValidateImage(file);

        await using var stream = file.OpenReadStream();

        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(file.FileName, stream),
            Folder = $"menu-items",
            PublicId = $"menu_item_{menuItemId}",
            Overwrite = true,
            Transformation = new Transformation()
                .Width(600)
                .Height(600)
                .Crop("fill")
                .Quality("auto")
                .FetchFormat("auto")
        };

        var result = await _cloudinary.UploadAsync(uploadParams);

        EnsureSuccess(result);

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

    private static void ValidateImage(IFormFile file)
    {
        if (file == null || !file.ContentType.StartsWith("image/"))
            throw new InvalidOperationException("Invalid image type");
    }

    private static void EnsureSuccess(ImageUploadResult result)
    {
        if (result.StatusCode != HttpStatusCode.OK)
            throw new Exception("Cloudinary upload failed");
    }

}
