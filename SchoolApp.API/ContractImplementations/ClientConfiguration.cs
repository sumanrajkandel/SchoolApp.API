
using SchoolApp.API.Contracts;

namespace SchoolApp.API.ContractImplementations;
public class ClientConfiguration : IClientConfiguration
{
    public string ClientName { get; set; }
    public DateTime InvokedDateTime { get; set; }
}
