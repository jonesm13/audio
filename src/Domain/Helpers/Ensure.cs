namespace Domain.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class Ensure
    {
        public static void IsPositiveInteger(int value, string argumentName)
        {
            if (value < 1)
            {
                throw new ArgumentException($"{argumentName} must be a " +
                                            "positive integer.");
            }
        }

        public static void IsNotNullOrEmptyOrWhitespace(
            string value,
            string argumentName)
        {
            IsNotNull(value, argumentName);
            IsNotNullOrEmpty(value, argumentName);

            if (value.All(char.IsWhiteSpace))
            {
                throw new ArgumentException($"{argumentName} may not " +
                                            "be all whitespace.");
            }
        }

        public static void IsNotNullOrEmpty(string value, string argumentName)
        {
            IsNotNull(value, argumentName);

            if (value == string.Empty)
            {
                throw new ArgumentException($"{argumentName} may not be an " +
                                            "empty string.");
            }
        }

        public static void ContainsMembers<T>(
            IEnumerable<T> items,
            string argumentName)
        {
            if (items == null)
            {
                throw new ArgumentNullException(argumentName);
            }

            if (!items.Any())
            {
                throw new ArgumentException($"{argumentName} contains no members.");
            }
        }

        public static void IsNotNull(object o, string argumentName)
        {
            if (o == null)
            {
                throw new ArgumentException(argumentName);
            }
        }
    }
}
