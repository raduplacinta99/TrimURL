namespace TrimUrlApi.Exceptions
{
    public class UnavailableUsernameException : ApiException
    {
        public UnavailableUsernameException(string username)
        : base($"Username is already in use: {username}", StatusCodes.Status400BadRequest)
        {
        }
    }
}
