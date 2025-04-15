namespace Proz_WebApi.Exceptions
{
    public class RoleNotFoundException : InvalidOperationException
    {
        public DateTime ErrorOccurredAt { get; }
        public string RoleName {  get; }
        public string UserName {  get; }
        public RoleNotFoundException(string role, string username)
            : base($"Role {role} does not exist to be assigned to the user {username}. The error occurred at: {DateTime.UtcNow}.") //the custom exception RoleNotFoundException was inherited by the base InvalidOperationException class, this " public RoleNotFoundException(string role, string username)" is our constructor of this custom exception, and by doing this ": base()" we are passing this string value "$"Role {role} does not exist to be assigned to the user {username}. The error occurred at: {DateTime.UtcNow}."" to the base class constructor.
        {
            ErrorOccurredAt = DateTime.UtcNow;
            RoleName= role;
            UserName= username;
        }
    }
}
