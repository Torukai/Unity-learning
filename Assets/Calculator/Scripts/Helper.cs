using System.Collections;
using System.Collections.Generic;

namespace Utility{
    public static class Helper
    {
        public static double ReadNumber(this string s, ref int startPos)
        {
            int i;


            for (i = startPos; i != s.Length && (char.IsDigit(s[i]) || s[i] == '.' || (i == startPos && s[i] == '-')); i++) ;

            var num = s.Substring(startPos, i - startPos);
            startPos += i - startPos - 1;
            return double.Parse(num);
        }
        public static string ReadFunction(this string s, ref int startPos)
        {
            int i;


            for (i = startPos; i != s.Length && char.IsLetter(s[i]); i++) ;

            var func = s.Substring(startPos, i - startPos);
            startPos += i - startPos - 1;
            return func;
        }
    }
}

