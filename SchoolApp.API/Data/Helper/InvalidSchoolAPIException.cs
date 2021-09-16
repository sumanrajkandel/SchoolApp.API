
namespace SchoolApp.API.Data.Helper;
public class InvalidSchoolAPIException : Exception
{
    public InvalidSchoolAPIException(string message) : base(message)
    {

    }

    public InvalidSchoolAPIException(string message, Exception ex) : base(message, ex)
    {

    }
}
