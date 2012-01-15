using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Prelude
{   
    public class SuperString : IEnumerable<char>, IEquatable<SuperString>, IEquatable<string>
    {
        string s;

        public SuperString(string s)
        {
            this.s = s;            
        }

        public SuperString ToUpper()
        {
            return new SuperString(s.ToUpper());
        }

        public SuperString ToLower()
        {
            return new SuperString(s.ToLower());
        }

        public Char[] ToCharArray()
        {
            return s.ToCharArray();            
        }

        public SuperString Substring(int index)
        {
            return new SuperString(s.Substring(index));
        }

        public SuperString Substring(int index, int length)
        {
            return new SuperString(s.Substring(index, length));
        }

        public SuperString[] Split(params char[] separator)
        {
            return s.Split(separator).Select(x => new SuperString(x)).ToArray();
        }

        public SuperString(char[] chars)
        {
            this.s = new string(chars);
        }

        public static SuperString operator *(int n, SuperString s)
        {
            return s.s.Multiply(n);
        }

        public static SuperString operator +(SuperString t, SuperString s)
        {
            return new SuperString(t.s + s.s);
        }

        public IEnumerator<char> GetEnumerator()
        {
            return s.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public static implicit operator SuperString(string s)
        {
            return new SuperString(s);
        }

        public static implicit operator string(SuperString s)
        {
            return s.s;
        }

        public SuperString Format(string format, object arg0)
        {
            return new SuperString (string.Format (s, arg0));
        }

        public SuperString Format(string format, params object[] args)
        {
            return new SuperString (string.Format (s, args));
        }

        public IEnumerable<SuperString> FindAll(string pattern)
        {
            foreach (Match match in Regex.Matches(this.s, pattern))
                yield return new SuperString(match.Value);
        }

        public SuperString Find(string pattern)
        {
            return new SuperString (Regex.Match (this.s, pattern).Value);
        }

        bool IEquatable<SuperString>.Equals(SuperString other)
        {
            return other.s.Equals(other.s);
        }

        bool IEquatable<string>.Equals(string other)
        {
            return other.Equals(this.s);
        }

        public bool ParseableAsInt()
        {
            return 0.Becomes (this.s).SuccessfulParse;
        }

        public bool ParseableAsDate()
        {
            return DateTime.MinValue.Becomes (this.s).SuccessfulParse;
        }        
    }    
    
    public static class Strings
    {
        public static string Multiply(this string s, int n)
        {
            var sb = new StringBuilder(n);

            for (var i = 0; i < n; i++)
            {
                sb.Append(s);
            }

            return s.ToString();
        }

        public static string Capitalize(this string s)
        {
            if (s != null & s.Length >= 1)
            {
                var chars = s.ToCharArray();
                
                chars[0] = char.ToUpper(chars[0]);

                s = new string(chars);
            }

            return s;
        }

        public static string CreateString(this char[] xs)
        {
            return new string(xs);
        }

        public static string Substring(this string s, int n)
        {
            return new string (s.Skip (n).ToArray ());
        }

        public static string CamelCase(this string s)
        {
            Func<char, bool> f = x => x == '_';

            var leading_underscore = new string(s.TakeWhile(f).ToArray());
            var trailing_underscore = new string(s.Reverse().TakeWhile(f).ToArray());

            return leading_underscore + s.Split('_').Select(x => x.Capitalize()).Join("") + trailing_underscore;
        }

        public static string SurroundWith(this string s, string fst, string lst)
        {
            return fst + s + lst;
        }

        public static string Repeat(this string s, uint n)
        {
            var sb = new StringBuilder((int)n);            

            for (var i = 0; i < n; i++)
                sb.Append(s);

            return sb.ToString();
        }

        public static string Repeat(this char c, uint n)
        {
            var chars = new char[n];           

            for (var i = 0; i < n; i++)
                chars[i] = c;

            return new string(chars);
        }

        public static string Join(this IEnumerable<string> xs, string separator)
        {
            return string.Join(separator, xs);
        }

        public static string Join(this string[] xs, string separator)
        {
            return string.Join(separator, xs);
        }

        public static bool IsNumeric(this string s)
        {
            return Microsoft.VisualBasic.Information.IsNumeric(s);
        }

        public static bool IsDate(this string s)
        {
            return Microsoft.VisualBasic.Information.IsDate(s);
        }

        public static bool TryParseAsBoolean(this string s, out bool @bool)
        {
            if (s == null)
            {
                @bool = false;
                return false;
            }            
            
            s = s.ToUpper();

            switch(s)
            {
                case "TRUE":
                case "YES":
                case "1":
                    @bool = true;
                    return true;
                
                case "FALSE":
                case "NO":
                case "0":
                    @bool = false;
                    return true;
                
            }

            @bool = false;

            return false;

        }


        public static bool TryParseAsBoolean(this string s, out bool? @bool)
        {
            if (s == null)
            {
                @bool = null;
                return false;
            }


            s = s.ToUpper();

            switch (s)
            {
                case "TRUE":
                case "YES":
                case "1":
                    @bool = true;
                    return true;

                case "FALSE":
                case "NO":
                case "0":
                    @bool = false;
                    return true;
            }

            @bool = null;

            return false;

        }

       
        public static bool IsNullOrEmpty(this string s)
        {
            return s == null || s.Length == 0;
        }
    }
}
