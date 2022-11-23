using GazpromVehicleFrontend.Models;

namespace GazpromVehicleFrontend.Services.Auth;

public interface IJwtAuthService
{
    Task<AuthenticatedUserModel> Login(AuthenticationUserModel userForAuthentication);
    Task LogOut();
}