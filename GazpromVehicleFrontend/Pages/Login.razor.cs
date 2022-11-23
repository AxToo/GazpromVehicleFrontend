using GazpromVehicleFrontend.Models;
using GazpromVehicleFrontend.Services.Auth;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace GazpromVehicleFrontend.Pages;

public partial class Login
{
    [Inject] private IJwtAuthService AuthService { get; set; }
    [Inject] private NavigationManager NavManager { get; set; }
    [CascadingParameter] private AuthenticationUserModel Model { get; set; } = new();
    
    private readonly InputType passwordInput = InputType.Password;
        
    private async void OnValidSubmit()
    {
        var result = await AuthService.Login(Model);
            
        if (result is not null)
        {
            NavManager.NavigateTo("/");
        }
        else
        {
            StateHasChanged();
        }
    }
}