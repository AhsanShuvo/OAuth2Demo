using Microsoft.AspNetCore.Authentication;
using System.Collections.Generic;

namespace OAuth2Demo.Models
{
    public class LoginViewModel
    {
        public string ReturnUrl { get; set; }
        public IList<string> ExternalLogins { get; set; }
    }
}
