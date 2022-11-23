using System.Net.Http.Headers;
using System.Security.Claims;
using Blazored.LocalStorage;
using GazpromVehicleFrontend.Models;
using Microsoft.AspNetCore.Components.Authorization;
using SB.WebApp.Services;

namespace GazpromVehicleFrontend.Services.Auth;

public class JwtStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorage;
    private readonly HttpClient _httpClient;
    private readonly AuthenticationState _anonymous;

    public JwtStateProvider(ILocalStorageService localStorage, HttpClient httpClient)
    {
        _localStorage = localStorage;
        _httpClient = httpClient;
        _anonymous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
    }
    
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var userTokens = await _localStorage.GetItemAsync<TokenResult>("userTokens");
        
        if (userTokens is not { })
        {
            return _anonymous;
        }
        
        var isAuthenticated = await NotifyUserAuthentication(userTokens.AccessToken);

        if (isAuthenticated is false)
        {
            return _anonymous;
        }

        return new AuthenticationState(
            new ClaimsPrincipal(
                new ClaimsIdentity(JwtParser.ParseClaimsFromJwt(userTokens.AccessToken), "jwtAuthType")));
    }
    
    public async Task<bool> NotifyUserAuthentication(string token)
    {
        bool isAuthenticatedOutput;
        
        try
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(JwtParser.ParseClaimsFromJwt(token), "jwtAuthType"));
            var authState = Task.FromResult(new AuthenticationState(authenticatedUser));

            NotifyAuthenticationStateChanged(authState);
            isAuthenticatedOutput = true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            await NotifyUserLogout();
            isAuthenticatedOutput = false;
        }

        return isAuthenticatedOutput;
    }

    public async Task NotifyUserLogout()
    {
        var authState = Task.FromResult(_anonymous);
        
        await _localStorage.RemoveItemAsync("userTokens");

        NotifyAuthenticationStateChanged(authState);
    }
}