﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Prelude
{
    public struct ParseAttemptResult<T>
    {
        public T Value { get; private set; }
        public bool SuccessfulParse { get; private set; }

        public ParseAttemptResult(T Value, bool SuccessfulParse)
            : this()
        {
            this.Value = Value;
            this.SuccessfulParse = SuccessfulParse;
        }

        //We override the true and false operators because we want to use this in branching statements; e.g. if, while, and the ternary operator.
        //Normally we would just implicitly cast to bool but that doesn't work since T might be a bool resulting in an ambiguity error.

        public static bool operator true(ParseAttemptResult<T> obj)
        {
            return obj.SuccessfulParse;
        }

        public static bool operator false(ParseAttemptResult<T> obj)
        {
            return obj.SuccessfulParse;
        }

        public static implicit operator T(ParseAttemptResult<T> obj)
        {
            return obj.Value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

    }

    public static class ParseUtils
    {

        //These methods allow a more functional style of creating other types from string parsing.

        //Example1 2.Becomes("4") creates a ParseAttemptResult<int> with Value = 4 and SuccessfulParse = true.
        //Example2 (-1).Becomes("foo") creates a ParseAttemptResult<int> with Value = -1 and SuccessfulParse = false.
        //Example3 Suppse we have a method Foo(int x); We can call it with Foo( 2.Becomes(str) )

        public static ParseAttemptResult<Boolean> Becomes(this Boolean n, string StringToParse)
        {
            bool k;
            
            var result = Boolean.TryParse(StringToParse, out k);

            return new ParseAttemptResult<Boolean>(result ? k : n, result);
        }

        public static ParseAttemptResult<Int16> Becomes(this Int16 n, string StringToParse)
        {
            Int16 k;
            
            var result = Int16.TryParse(StringToParse, out k);

            return new ParseAttemptResult<Int16>(result ? k : n, result);
        }

        public static ParseAttemptResult<Int32> Becomes(this Int32 n, string StringToParse)
        {
            int k;
            
            var result = Int32.TryParse(StringToParse, out k);

            return new ParseAttemptResult<Int32>(result ? k : n, result);
        }

        public static ParseAttemptResult<Int64> Becomes(this Int64 n, string StringToParse)
        {
            long k;
            
            var result = Int64.TryParse(StringToParse, out k);

            return new ParseAttemptResult<Int64>(result ? k : n, result);
        }

        public static ParseAttemptResult<UInt16> Becomes(this UInt16 n, string StringToParse)
        {
            ushort k;
            
            var result = UInt16.TryParse(StringToParse, out k);

            return new ParseAttemptResult<UInt16>(result ? k : n, result);
        }

        public static ParseAttemptResult<UInt32> Becomes(this UInt32 n, string StringToParse)
        {
            uint k;
            
            var result = UInt32.TryParse(StringToParse, out k);

            return new ParseAttemptResult<UInt32>(result ? k : n, result);
        }

        public static ParseAttemptResult<UInt64> Becomes(this UInt64 n, string StringToParse)
        {
            ulong k;
            
            var result = UInt64.TryParse(StringToParse, out k);

            return new ParseAttemptResult<UInt64>(result ? k : n, result);
        }

        public static ParseAttemptResult<Double> Becomes(this Double n, string StringToParse)
        {
            double k;
            
            var result = Double.TryParse(StringToParse, out k);

            return new ParseAttemptResult<Double>(result ? k : n, result);
        }

        public static ParseAttemptResult<Decimal> Becomes(this Decimal n, string StringToParse)
        {
            decimal k;
            
            var result = Decimal.TryParse(StringToParse, out k);

            return new ParseAttemptResult<Decimal>(result ? k : n, result);
        }

        public static ParseAttemptResult<Single> Becomes(this Single n, string StringToParse)
        {
            float k;
            
            var result = Single.TryParse(StringToParse, out k);

            return new ParseAttemptResult<Single>(result ? k : n, result);
        }

        public static ParseAttemptResult<DateTime> Becomes(this DateTime n, string StringToParse)
        {
            DateTime whenever;

            var result = DateTime.TryParse(StringToParse, out whenever);

            return new ParseAttemptResult<DateTime>(result ? whenever : n, result);
        }

        public static ParseAttemptResult<Guid> Becomes(this Guid guid, string StringToParse)
        {
            Guid k;

            var result = Guid.TryParse(StringToParse, out k);

            return new ParseAttemptResult<Guid>(result ? k : guid, result);
        }

        public static int? TryParseInt32(this string s)
        {
            int? result = null;
            int n;

            if (int.TryParse(s, out n))
                result = n;

            return result;
        }
    }
}
