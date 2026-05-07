namespace TrimUrlApi.Exceptions
{
    public class MissingUserUpdateFieldsException : ApiException
    {
        public MissingUserUpdateFieldsException(string email)
        : base("At least one field must be provided: password, emailAddress.", StatusCodes.Status400BadRequest)
        {
        }
    }
}
