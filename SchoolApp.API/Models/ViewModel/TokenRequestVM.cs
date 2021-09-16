
using System.ComponentModel.DataAnnotations;

namespace SchoolApp.API.Models.ViewModel;
public class TokenRequestVM
{
    [Required]
    public string Token { get; set; }
    [Required]
    public string RefreshToken { get; set; }
}
