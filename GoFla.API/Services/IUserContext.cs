using System;

namespace GoFla.API.Services;

public interface IUserContext
{
    string? UserId {get; }
    bool IsAuthenticated {get; }
}
