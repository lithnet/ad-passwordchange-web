using System.Text;

namespace Lithnet.ActiveDirectory.PasswordChange.Web
{
    public static class Extensions
    {
        public static string ToHexString(this byte[] hash)
        {
            return hash.ToHexString(0, hash.Length);
        }

        public static string ToHexString(this byte[] hash, int offset, int count)
        {
            StringBuilder sb = new StringBuilder(hash.Length * 2);

            for (int i = offset; i < count; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }

            return sb.ToString();
        }
    }
}