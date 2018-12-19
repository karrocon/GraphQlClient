namespace GraphQlClient.Extensions
{
    static class StringExtensions
    {
        public static string ToLowerFirst(this string str)
        {
            if (str == null)
            {
                throw new System.ArgumentNullException(nameof(str));
            }

            if (str == string.Empty)
            {
                return str;
            }

            return $"{char.ToLower(str[0])}{str.Substring(1, str.Length - 1)}";
        }

        public static string ToLowerInvariantFirst(this string str)
        {
            if (str == null)
            {
                throw new System.ArgumentNullException(nameof(str));
            }

            if (str == string.Empty)
            {
                return str;
            }

            return $"{char.ToLowerInvariant(str[0])}{str.Substring(1, str.Length - 1)}";
        }

        public static string ToUpperFirst(this string str)
        {
            if (str == null)
            {
                throw new System.ArgumentNullException(nameof(str));
            }

            if (str == string.Empty)
            {
                return str;
            }

            return $"{char.ToUpper(str[0])}{str.Substring(1, str.Length - 1)}";
        }

        public static string ToUpperInvariantFirst(this string str)
        {
            if (str == null)
            {
                throw new System.ArgumentNullException(nameof(str));
            }

            if (str == string.Empty)
            {
                return str;
            }

            return $"{char.ToUpperInvariant(str[0])}{str.Substring(1, str.Length - 1)}";
        }
    }
}
