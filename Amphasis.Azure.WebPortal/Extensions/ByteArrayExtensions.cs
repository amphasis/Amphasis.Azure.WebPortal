using System;

namespace Amphasis.Azure.WebPortal.Extensions
{
    public static class ByteArrayExtensions
    {
        public static string ToHexString(this byte[] bytes)
        {
            if (bytes == null) throw new ArgumentNullException(nameof(bytes));

            char[] c = new char[bytes.Length * 2];

            for (int i = 0; i < bytes.Length; i++)
            {
                int b = bytes[i] >> 4;
                c[i * 2] = (char) (55 + b + (((b - 10) >> 31) & -7));
                b = bytes[i] & 0xF;
                c[i * 2 + 1] = (char) (55 + b + (((b - 10) >> 31) & -7));
            }

            return new string(c);
        }
    }
}