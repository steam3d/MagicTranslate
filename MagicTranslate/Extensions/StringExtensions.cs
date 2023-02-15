namespace MagicTranslate.Extensions
{
    public static class StringExtensions
    {
        public static string ToFirstCharUpper(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            char[] a = str.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }

        public static string ToFirstCharLower(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            char[] a = str.ToCharArray();
            a[0] = char.ToLower(a[0]);
            return new string(a);
        }
    }
}
