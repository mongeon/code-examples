using System.Net.Http.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Shared;

namespace Client.Auth;

public class StaticWebAppsAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly HttpClient _http;

    public StaticWebAppsAuthenticationStateProvider(HttpClient http)
    {
        _http = http;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var response = await _http.GetFromJsonAsync<AuthResponse>("/.auth/me");
            var principal = response?.ClientPrincipal;

            if (principal == null)
                return new AuthenticationState(new ClaimsPrincipal());

            var identity = new ClaimsIdentity(principal.IdentityProvider);
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, principal.UserId));
            identity.AddClaim(new Claim(ClaimTypes.Name, principal.UserDetails));

            foreach (var role in principal.UserRoles)
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, role));
            }

            return new AuthenticationState(new ClaimsPrincipal(identity));
        }
        catch
        {
            return new AuthenticationState(new ClaimsPrincipal());
        }
    }
}
