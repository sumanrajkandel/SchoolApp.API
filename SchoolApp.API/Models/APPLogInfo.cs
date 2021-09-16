
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SchoolApp.API.Models;
public class APPLogInfo
{


    [Key]
    public int Id { get; set; }
    public string? LogCode { get; set; }
    public string? LogDescription { get; set; }
    [MaxLength(5000)]
    public string? Headers { get; set; }
    [MaxLength(5000)]
    public string? Body { get; set; }
    [DefaultValue(true)]
    public DateTime CreatedOn { get; set; }
    [MaxLength(5000)]
    public string UserAgent { get; set; }


}
