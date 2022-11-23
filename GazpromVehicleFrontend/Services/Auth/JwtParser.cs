using System.Security.Claims;
using System.Text.Json;

namespace SB.WebApp.Services;

public static class JwtParser
{
    public static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var claims = new List<Claim>();
        var payload = jwt.Split('.')[1];

        var jsonBytes = ParseBase64WithoutPadding(payload);
            
        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

        ExtractRolesFromJwt(claims, keyValuePairs);

        claims.AddRange((keyValuePairs ?? throw new InvalidOperationException()).Select(kvp => new Claim(kvp.Key, kvp.Value.ToString() ?? throw new InvalidOperationException())));

        return claims;
    }

    private static void ExtractRolesFromJwt(List<Claim> claims, IDictionary<string, object> keyValuePairs)
    {
        keyValuePairs.TryGetValue(ClaimTypes.Role, out var roles);

        if (roles is null) return;
            
        var parsedRoles = roles.ToString()?.Trim().TrimStart('[').TrimEnd(']').Split(',');

        if (parsedRoles is {Length: > 1})
        {
            claims.AddRange(parsedRoles.Select(parsedRole => new Claim(ClaimTypes.Role, parsedRole.Trim('"'))));
        }
        else
        {
            claims.Add(new Claim(ClaimTypes.Role, parsedRoles?[0] ?? throw new InvalidOperationException()));
        }

        keyValuePairs.Remove(ClaimTypes.Role);
    }

    private static byte[] ParseBase64WithoutPadding(string base64)
    {
        switch (base64.Length % 4)
        {
            case 2: base64 += "==";
                break;
            case 3: base64 += "=";
                break;
        }

        return Convert.FromBase64String(base64);
    }
}