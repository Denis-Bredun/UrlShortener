using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrlShortener.Infrastructure.Exceptions
{
    public class AuthException : Exception
    {
        public AuthException(string message) : base(message) { }
    }

    public class UserNotFoundException : AuthException
    {
        public UserNotFoundException() : base("User not found.") { }
    }

    public class AdminNotFoundException : AuthException
    {
        public AdminNotFoundException() : base("Admin not found.") { }
    }

    public class InvalidCredentialsException : AuthException
    {
        public InvalidCredentialsException() : base("Invalid email or password.") { }
    }

    public class UserCreationException : AuthException
    {
        public UserCreationException(string details) : base($"User creation failed: {details}") { }
    }

    public class RoleCreationException : AuthException
    {
        public RoleCreationException(string details) : base($"Role creation failed: {details}") { }
    }

    public class RoleAssignmentException : AuthException
    {
        public RoleAssignmentException(string details): base($"Role assignment failed: {details}") { }
    }
}
