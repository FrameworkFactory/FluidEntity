using System;

namespace FWF.FluidEntity.ComponentModel
{
    public static class Base64Url
    {
        public static string Encode(byte[] arg)
        {
            string s = Convert.ToBase64String(arg);

            s = s.Split(new char[]
            {
                '='
            })[0];

            s = s.Replace('+', '-');

            return s.Replace('/', '_');
        }

        public static byte[] Decode(string arg)
        {
            string s = arg.Replace('-', '+');

            s = s.Replace('_', '/');

            switch (s.Length % 4)
            {
                case 0:
                    goto IL_69;
                case 2:
                    s += "==";
                    goto IL_69;
                case 3:
                    s += "=";
                    goto IL_69;
            }

            throw new Exception("Illegal base64url string!");

            IL_69:

            return Convert.FromBase64String(s);
        }
    }
}
