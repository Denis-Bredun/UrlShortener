using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrlShortener.Infrastructure.Exceptions
{
    public class ShortUrlException : Exception
    {
        public ShortUrlException(string message) : base(message) { }
    }

    public class ShortUrlNotFoundException : ShortUrlException
    {
        public ShortUrlNotFoundException() : base("Short URL not found.") { }
    }

    public class ShortUrlForbiddenDeletionException : ShortUrlException
    {
        public ShortUrlForbiddenDeletionException() : base("You can only delete your own short URLs.") { }
    }

    public class ShortUrlCreationConflictException : ShortUrlException
    {
        public ShortUrlCreationConflictException() : base("This URL already exists.") { }
    }

    public class ShortCodeGenerationException : ShortUrlException
    {
        public ShortCodeGenerationException() : base("Unable to generate unique short code after 10 attempts.") { }
    }

    public class ShortCodeEmptyException : ShortUrlException
    {
        public ShortCodeEmptyException() : base("Empty short code passed.") { }
    }

    public class ShortCodeInvalidLengthException : ShortUrlException
    {
        public ShortCodeInvalidLengthException(int length) : base($"Short code length must be greater than 0, but was {length}.") { }
    }
}
