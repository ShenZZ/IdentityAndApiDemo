using System.ComponentModel.DataAnnotations;

namespace IdentityServer.Controllers;

public class LoginModel
{
    [Required]
    public string Username { get; set; }
        
    [Required]
    public string Password { get; set; }
        
    public bool RememberLogin { get; set; }
        
    public string ReturnUrl { get; set; }

    public bool EnableLocalLogin { get; set; }
}