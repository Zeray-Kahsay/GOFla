using System;
using System.Security;
using GoFla.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace GoFla.API.Controllers;

public class ImagesController (ICloudinaryService cloudinaryService) : BaseController
{
    [HttpPost("upload")]
    public async Task<IActionResult> UploadImage(IFormFile file, [FromQuery] string folder, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new {Message = "No file provided.", code = "NO_FILE"});
        }

        var result = await cloudinaryService.UploadImageAsync(file, folder, cancellationToken);

        return Ok(result);
    }

    [HttpDelete("{publicId}")]
    public async Task<IActionResult> DeleteImage(string publicId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(publicId))
        {
            return BadRequest(new {Message = "No publicId provided.", code = "NO_PUBLIC_ID"});
        }

        var result = await cloudinaryService.DeleteImageAsync(publicId, cancellationToken);

        return Ok(result);
    }
}
