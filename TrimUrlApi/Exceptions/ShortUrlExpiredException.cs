namespace TrimUrlApi.Exceptions
{
    public class ShortUrlExpiredException : ApiException
    {
        public ShortUrlExpiredException()
        : base($"URL expired", StatusCodes.Status410Gone)
        {
        }
    }
}
