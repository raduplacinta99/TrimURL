namespace TrimUrlApi.Exceptions
{
    public class UsernameNotFoundException : ApiException
    {
        public UsernameNotFoundException(string username)
            : base($"Username not found: {username}", StatusCodes.Status404NotFound) 
        {
        }
    }
}
