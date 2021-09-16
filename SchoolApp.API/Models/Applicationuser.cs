
using Microsoft.AspNetCore.Identity; // IdentityUser present on it

namespace SchoolApp.API.Models;
public class Applicationuser: IdentityUser // We're creating Applicationuser as custom class for already existing IdentityUser class.
{
    public string Firstname { get; set; }
    public string Lastname { get; set; }
    public string? Custom { get; set; }
}
