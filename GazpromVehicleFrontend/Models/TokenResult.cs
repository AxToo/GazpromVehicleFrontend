namespace GazpromVehicleFrontend.Models;

public class TokenResult
{
    public string UserName { get; set; }
    public string AccessToken { get; set; }
    public DateTime AccessTokenExpirationTime { get; set; }
}