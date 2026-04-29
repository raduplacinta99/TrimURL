namespace TrimUrlApi.Exceptions
{
    public class ShortUrlsNotFoundException : ApiException
    {
        public ShortUrlsNotFoundException()
        : base($"No URLs found", StatusCodes.Status404NotFound)
        {
        }
    }
}
