namespace TrimUrlApi.Exceptions
{
    public class UnavailableEmailException : ApiException
    {
        public UnavailableEmailException(string email)
        : base($"E-mail address is already in use: {email}", StatusCodes.Status400BadRequest)
        {
        }
    }
}
