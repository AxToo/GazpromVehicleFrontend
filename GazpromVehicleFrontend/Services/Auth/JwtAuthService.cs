using Blazored.LocalStorage;
using GazpromVehicleFrontend.Models;
using Microsoft.AspNetCore.Components.Authorization;

namespace GazpromVehicleFrontend.Services.Auth;

public class JwtAuthService : IJwtAuthService
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;
    private readonly AuthenticationStateProvider _authenticationStateProvider;

    public JwtAuthService(HttpClient httpClient, ILocalStorageService localStorage, AuthenticationStateProvider authenticationStateProvider)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
        _authenticationStateProvider = authenticationStateProvider;
    }
    
    public async Task<AuthenticatedUserModel> Login(AuthenticationUserModel userForAuthentication)
    {
        var request = await _httpClient.PostAsJsonAsync("/api/auth", userForAuthentication);
        
        if (!request.IsSuccessStatusCode)
        {
            throw new Exception(await request.Content.ReadAsStringAsync());
        }
        
        var authenticatedUser = await request.Content.ReadFromJsonAsync<TokenResult>();
        
        await _localStorage.SetItemAsync("userTokens", authenticatedUser);

        await ((JwtStateProvider) _authenticationStateProvider).NotifyUserAuthentication(authenticatedUser.AccessToken);

        return new AuthenticatedUserModel
        {
            UserName = authenticatedUser.UserName,
            AccessToken = authenticatedUser.AccessToken
        };
    }
    
    public async Task LogOut()
    {
        await _localStorage.RemoveItemAsync("userTokens");
        
        await ((JwtStateProvider) _authenticationStateProvider).NotifyUserLogout();
    }
}