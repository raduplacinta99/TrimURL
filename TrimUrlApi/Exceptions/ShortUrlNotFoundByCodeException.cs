namespace TrimUrlApi.Exceptions
{
    public class ShortUrlNotFoundByCodeException : ApiException
    {
        public ShortUrlNotFoundByCodeException(string code)
        : base($"No URL found with code: {code}", StatusCodes.Status404NotFound)
        {
        }
    }
}
