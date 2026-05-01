namespace TrimUrlApi.Exceptions
{
    public class ForbiddenShortUrlAccessException : ApiException
    {
        public ForbiddenShortUrlAccessException()
        : base($"You do not have permission to modify this Short URL.", StatusCodes.Status400BadRequest)
        {
        }
    }
}
