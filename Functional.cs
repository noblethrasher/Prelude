using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Prelude
{
    public static class Functional
    {
        public static Func<X, Y> Memoize<X, Y>(this Func<X, Y> f)
        {
            var memo = new Dictionary<X, Y>();

            return x =>
                {
                    var y = default(Y);

                    if (!memo.TryGetValue(x, out y))
                    {
                        y = f(x);
                        memo.Add(x, y);
                    }

                    return y;
                };
        }

        public static Func<X, Z> Compose<X, Y, Z>(this Func<X, Y> f, Func<Y, Z> g)
        {
            return x => g(f(x));
        }

        public static Func<A, Func<B, C>> Curry<A, B, C>(this Func<A, B, C> f)
        {
            return a => b => f(a, b);
        }

        public static Func<A, Func<B, Func<C, D>>> Curry<A, B, C, D>(this Func<A, B, C, D> f)
        {
            return a => b => c => f(a, b, c);
        }
    }

    public abstract class Unit
    {
        public static readonly Unit Value = new _Unit();

        private Unit()
        {

        }

        class _Unit : Unit
        {

        }
    }
}
