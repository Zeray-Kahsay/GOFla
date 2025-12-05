using System;
using GoFla.API.DTOs.Auth;

namespace GoFla.API.Services;

public interface IExternalAuthService
{
    Task<ExternalUserInfo?> GetUserInfoAsync(string provider, string accessToken, CancellationToken cancellationToken = default);
}
