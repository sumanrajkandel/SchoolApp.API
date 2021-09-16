
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolApp.API.Models;
public class RefreshToken
{
    [Key]
    public int Id { get; set; }
    public string Token { get; set; }
    public string JwtId { get; set; }
    public bool IsResolved { get; set; }
    public DateTime DateAdded { get; set; }
    public DateTime DateExpire { get; set; }

    public string UserId { get; set; } //FK
    [ForeignKey(nameof(UserId))]
    public Applicationuser User { get; set; }
}
