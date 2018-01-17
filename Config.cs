using System.Collections.Generic;
using System.Security.Claims;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;

namespace IdentityServer
{
    public class Config
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            var customProfile = new IdentityResource(
            name: "custom.profile",
            displayName: "Custom profile",
            claimTypes: new[] { "name", "email", "website" });
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                customProfile,
                new IdentityResource("roles", "Your role(s)", new List<string>() {"role"})
            };
        }

        //Deve retornar uma lista de todos os recursos que uma api pode ter
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                //new ApiResource("imagegalleryapi", "image gallery api")
                new ApiResource("api1", "My API")
                {
                    UserClaims = new [] {"custom.profile"}
                }
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                // OpenID Connect implicit flow client (MVC)
                new Client
                {
                    RequireClientSecret = false,
                    ClientId = "angular_spa",
                    ClientName = "Angular Cliente",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowedCorsOrigins = new List<string> { "http://localhost:4200" },
                    // where to redirect to after login,
                    RedirectUris = new List<string> {  "http://localhost:4200/auth-callback", "http://localhost:4200/silent-refresh.html" },
                    // where to redirect to after logout
                    PostLogoutRedirectUris = { "http://localhost:4200/" },
                    AllowAccessTokensViaBrowser = true,
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "api1", "custom.profile", "roles"
                    },
                    //AccessTokenLifetime = 90,
                    RequireConsent=false,
                }
            };
        }


        public static List<TestUser> GetUsers()
        {
            return new List<TestUser>
            {
                new TestUser
                {
                    SubjectId = "1",
                    Username = "louise",
                    Password = "louise*A1",
                    Claims = new List<Claim>
                    {
                        new Claim("email", "louise@hotmail.com"),
                        new Claim("website", "louise.com"),
                        new Claim("roles", "PayingUser")
                    }
                },
                new TestUser
                {
                    SubjectId = "2",
                    Username = "jackson",
                    Password = "jackson*A1",
                    Claims = new List<Claim>
                    {
                        new Claim("email", "jackson@hotmail.com"),
                        new Claim("website", "jackson.com")
                    },
  
                },
                new TestUser
                {
                    SubjectId = "3",
                    Username = "daiana@hotmail.com",
                    Password = "daiana",
                    Claims = new List<Claim>
                    {
                        new Claim("email", "daiana@hotmail.com"),
                        new Claim("website", "daiana.com")
                    }
                }
            };
        }
    }
}