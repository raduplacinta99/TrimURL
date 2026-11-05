namespace TrimUrlApi.Exceptions
{
    public class IncorrectPasswordException : ApiException
    {
        public IncorrectPasswordException()
            : base("Incorrect password.", StatusCodes.Status401Unauthorized)
        {
        }
    }
}
