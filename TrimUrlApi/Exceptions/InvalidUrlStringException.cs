namespace TrimUrlApi.Exceptions
{
    public class InvalidUrlStringException : ApiException
    {
        public InvalidUrlStringException(string url)
            : base($"Invalid URL string: {url}", StatusCodes.Status400BadRequest)
        {
        }
    }
}
