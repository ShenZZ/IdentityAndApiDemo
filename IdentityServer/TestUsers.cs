using IdentityModel;
using System.Collections.Generic;
using System.Security.Claims;
using Duende.IdentityServer.Test;

public class TestUsers
{
    public static List<TestUser> Users
    {
        get
        {
            return new List<TestUser>
            {
                new TestUser
                {
                    SubjectId = "1",
                    Username = "sam",
                    Password = "sam",
                    Claims =
                    {
                        new Claim(JwtClaimTypes.Name, "sam"),
                        new Claim(JwtClaimTypes.Email, "sam@email.com"),
                        new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean)
                    }
                }
            };
        }
    }
}