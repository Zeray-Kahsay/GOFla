using System;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using GoFla.API.Commons;
using GoFla.API.Configuration;
using Microsoft.Extensions.Options;

namespace GoFla.API.Services;

public class CloudinaryService : ICloudinaryService
{
    private readonly ILogger<CloudinaryService> _logger;
    private readonly Cloudinary _cloudinary;
    public CloudinaryService(IOptions<CloudinarySettings> cloudinarySettings, ILogger<CloudinaryService> logger)
    {
        _logger = logger;
        var settings = cloudinarySettings.Value;
        var account = new Account(
            settings.CloudName,
            settings.ApiKey,
            settings.ApiSecret
        );
        _cloudinary = new Cloudinary(account);
    }

    public async Task<Result<string>> UploadImageAsync(IFormFile file, string folder, CancellationToken cancellationToken = default)
    {
        if (file is null || file.Length == 0)
        {
            return Result<string>.Failure("No file provided", "NO_FILE");
        }

        // Validate file type 
        var allowedExtensions = new[]{".jpg", ".jpeg", ".png", ".gif", ".webp"};
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(extension))
        {
            return Result<string>.Failure("Invalid file type. Only images are allowed (jpg, jpeg, png, gif, webp)", "INVALID_FILE_TYPE");
        }

        // Validate file size (max 5MB)
        if (file.Length > 5 * 1024 * 1024)
        {
            return Result<string>.Failure("File size exceeds the maximum limit of 5MB", "FILE_TOO_LARGE");
        }

        try
        {
            using var stream = file.OpenReadStream();

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = folder,
                Transformation = new Transformation().Quality("auto").FetchFormat("auto"),
                UseFilename = true,
                UniqueFilename = true,
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams, cancellationToken);

            if (uploadResult.Error is not null)
            {
                _logger.LogError("Cloudinary upload error: {ErrorMessage}", uploadResult.Error.Message);
                return Result<string>.Failure("Image upload failed", "UPLOAD_FAILED");
            }

            return Result<string>.Success(uploadResult.SecureUrl.ToString());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading image to Cloudinary");
            return Result<string>.Failure("Failed to upload image", "UPLOAD_ERROR");
        }
    }


    public async Task<Result<bool>> DeleteImageAsync(string publicId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(publicId))
        {
            return Result<bool>.Failure("Public ID is required", "NO_PUBLIC_ID");
        }

        try
        {
            var deleteParams = new DeletionParams(publicId);
            var result = await _cloudinary.DestroyAsync(deleteParams);

            if (result.Result == "ok")
            {
                return Result<bool>.Success(true);
            }

            _logger.LogError("Cloudinary deletion error: {Result}", result.Result);
            return Result<bool>.Failure("Image deletion failed", "DELETION_FAILED");
        }

        
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting image from Cloudinary");
            return Result<bool>.Failure("Failed to delete image", "DELETION_ERROR");
        }
    }

    public async Task<Result<string>> UploadImageWithTransformationAsync(IFormFile file, string folder, int width, int height, CancellationToken cancellationToken = default)
    {
        if (file is null || file.Length == 0)
        {
            return Result<string>.Failure("No file provided", "NO_FILE");
        }

        try
        {
            using var stream = file.OpenReadStream();

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = folder,
                Transformation = new Transformation()
                    .Width(width)
                    .Height(height)
                    .Crop("fill")
                    .Gravity("auto")
                    .Quality("auto")
                    .FetchFormat("auto"),
                UseFilename = true,
                UniqueFilename = true,

            };

            var UploadResult = await _cloudinary.UploadAsync(uploadParams, cancellationToken);

            if (UploadResult.Error != null)
            {
                _logger.LogError("Cloudinary upload error: {ErrorMessage}", UploadResult.Error.Message);
                return Result<string>.Failure("Image upload failed", "UPLOAD_FAILED");
            }

            return Result<string>.Success(UploadResult.SecureUrl.ToString());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading image to Cloudinary with transformation");
            return Result<string>.Failure("Failed to upload image", "UPLOAD_ERROR");
        }
    }

    public string GetPublicIdFromUrl(string imageUrl)
    {
        // Extract public_id from Cloudinary URL
        // Example: https://res.cloudinary.com/demo/image/upload/v1234567890/folder/filename.jpg
        // Returns: folder/filename

        try
        {
            var uri = new Uri(imageUrl);
            var segments = uri.Segments;

            // Find the index of "upload/"
            var uploadIndex = Array.FindIndex(segments, s => s.Contains("upload"));

            if (uploadIndex >= 0 && uploadIndex < segments.Length)
            {
                // Skip "upload/" and version "Vxxxxxx/"
                var publicIdParts = segments.Skip(uploadIndex + 2).ToArray();
                var publicId = string.Join("", publicIdParts).TrimEnd('/');

                // Remove file extension
                var lastDotIndex = publicId.LastIndexOf('.');
                if (lastDotIndex > 0)
                {
                    publicId = publicId.Substring(0, lastDotIndex);
                }

                return publicId;
            }

            return string.Empty;
        }
        catch 
        {
            return string.Empty;
        }
    }
}
