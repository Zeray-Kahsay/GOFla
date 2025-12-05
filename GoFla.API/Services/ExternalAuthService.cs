using System;
using System.Text.Json;
using GoFla.API.DTOs.Auth;

namespace GoFla.API.Services;

public class ExternalAuthService : IExternalAuthService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ExternalAuthService> _logger;

    public ExternalAuthService(HttpClient httpClient, ILogger<ExternalAuthService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }
    public async Task<ExternalUserInfo?> GetUserInfoAsync(string provider, string accessToken, CancellationToken cancellationToken = default)
    {
       try
       {
            return provider.ToLower() switch
            {
                "google" => await GetGoogleUserInfoAsync(accessToken),
                "facebook" => await GetFacebookUserInfoAsync(accessToken),
                "github" => await GetGitHubUserInfoAsync(accessToken),
                "linkedin" => await GetLinkedInUserInfoAsync(accessToken),
                _ => null
            };
        }
       catch (Exception ex)
       {
            _logger.LogError(ex, "Error getting user info from {Provider}", provider);
            return null;
        }
    }

    private async Task<ExternalUserInfo?> GetGoogleUserInfoAsync(string accessToken)
    {
        var response = await _httpClient.GetAsync($"https://www.googleapis.com/oauth2/v2/userinfo?access_token={accessToken}");
        if (!response.IsSuccessStatusCode) return null;

        var json = await response.Content.ReadAsStringAsync();
        var data = JsonSerializer.Deserialize<JsonElement>(json);

        return new ExternalUserInfo
        {
            ProviderId = data.GetProperty("id").GetString() ?? string.Empty,
            Email = data.GetProperty("email").GetString() ?? string.Empty,
            FirstName = data.GetProperty("given_name").GetString() ?? string.Empty,
            LastName = data.GetProperty("family_name").GetString() ?? string.Empty,
            ProfileImageUrl = data.GetProperty("picture").GetString()
        };
    }

    private async Task<ExternalUserInfo?> GetFacebookUserInfoAsync(string accessToken)
    {
        var response = await _httpClient.GetAsync($"https://graph.facebook.com/me?fields=id,email,first_name,last_name,picture&access_token={accessToken}");
        if (!response.IsSuccessStatusCode) return null;

        var json = await response.Content.ReadAsStringAsync();
        var data = JsonSerializer.Deserialize<JsonElement>(json);

        return new ExternalUserInfo
        {
            ProviderId = data.GetProperty("id").GetString() ?? string.Empty,
            Email = data.GetProperty("email").GetString() ?? string.Empty,
            FirstName = data.GetProperty("first_name").GetString() ?? string.Empty,
            LastName = data.GetProperty("last_name").GetString() ?? string.Empty,
            ProfileImageUrl = data.GetProperty("picture").GetProperty("data").GetProperty("url").GetString()
        };
    }

    private async Task<ExternalUserInfo?> GetLinkedInUserInfoAsync(string accessToken)
    {
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

        var response = await _httpClient.GetAsync("https://api.linkedin.com/v2/me");
        if (!response.IsSuccessStatusCode) return null;

        var json = await response.Content.ReadAsStringAsync();
        var data = JsonSerializer.Deserialize<JsonElement>(json);

        // Get email
        var emailResponse = await _httpClient.GetAsync("https://api.linkedin.com/v2/emailAddress?q=members&projection=(elements*(handle~))");
        var emailJson = await emailResponse.Content.ReadAsStringAsync();
        var emailData = JsonSerializer.Deserialize<JsonElement>(emailJson);
        var email = emailData.GetProperty("elements")[0].GetProperty("handle~").GetProperty("emailAddress").GetString();

        return new ExternalUserInfo
        {
            ProviderId = data.GetProperty("id").GetString() ?? string.Empty,
            Email = email ?? string.Empty,
            FirstName = data.GetProperty("localizedFirstName").GetString() ?? string.Empty,
            LastName = data.GetProperty("localizedLastName").GetString() ?? string.Empty,
            ProfileImageUrl = null
        };
    }

    private async Task<ExternalUserInfo?> GetGitHubUserInfoAsync(string accessToken)
    {
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "FoodOrderApp");

        var response = await _httpClient.GetAsync("https://api.github.com/user");
        if (!response.IsSuccessStatusCode) return null;

        var json = await response.Content.ReadAsStringAsync();
        var data = JsonSerializer.Deserialize<JsonElement>(json);

        var name = data.GetProperty("name").GetString() ?? string.Empty;
        var nameParts = name.Split(' ', 2);

        // Get email from separate endpoint
        var emailResponse = await _httpClient.GetAsync("https://api.github.com/user/emails");
        var emailJson = await emailResponse.Content.ReadAsStringAsync();
        var emails = JsonSerializer.Deserialize<JsonElement[]>(emailJson);
        var primaryEmail = emails?.FirstOrDefault(e => e.GetProperty("primary").GetBoolean());

        return new ExternalUserInfo
        {
            ProviderId = data.GetProperty("id").GetInt32().ToString(),
            Email = primaryEmail?.GetProperty("email").GetString() ?? string.Empty,
            FirstName = nameParts.Length > 0 ? nameParts[0] : string.Empty,
            LastName = nameParts.Length > 1 ? nameParts[1] : string.Empty,
            ProfileImageUrl = data.GetProperty("avatar_url").GetString()
        };
    }
}
