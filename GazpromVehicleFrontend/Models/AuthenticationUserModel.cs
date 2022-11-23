using System.ComponentModel.DataAnnotations;

namespace GazpromVehicleFrontend.Models;

public class AuthenticationUserModel
{
    [Required(ErrorMessage = "Email Address is required.")]
    public string UserName { get; set; }
    [Required(ErrorMessage = "Password is required.")]
    public string Password { get; set; }
}