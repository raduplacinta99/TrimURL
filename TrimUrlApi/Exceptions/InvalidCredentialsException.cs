namespace TrimUrlApi.Exceptions
{
    public class InvalidCredentialsException : ApiException
    {
        public InvalidCredentialsException()
            : base("Invalid username or password.", StatusCodes.Status401Unauthorized)
        {
        }
    }
}
