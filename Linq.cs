using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Prelude
{
    public static class Linq
    {
        public static bool CountIs<T>(this IEnumerable<T> xs, Func<int, bool> pred)
        {
            var list = xs as IList<T>;
            var array = xs as T[];

            if (list != null)
                return pred(list.Count);

            if (array != null)
                return pred(array.Length);

            return pred(xs.Count());
        }

        public static bool CountIs<T>(this IEnumerable<T> xs, int n)
        {
            var list = xs as IList<T>;
            var array = xs as T[];

            if (list != null)
                return list.Count == n;

            if (array != null)
                return array.Length == n;

            int i = 0;

            foreach (var x in xs)
                if (i++ > n)
                    return false;

            return i == n;

        }
        
        public static IEnumerable<TResult> MapLast<T, TResult>(this IEnumerable<T> xs, Func<T, TResult> init, Func<T, TResult> tail)
        {
            var enumerator = xs.GetEnumerator();
            
            if (enumerator.MoveNext())
            {                
                loop:
                var x = enumerator.Current;

                while (enumerator.MoveNext())
                {
                    yield return init(x);
                    goto loop;
                }

                yield return tail(x);                
            }
        }

        public static IEnumerable<TResult> MapAlternate<T, TResult>(this IEnumerable<T> xs, Func<T, TResult> odd, Func<T, TResult> even)
        {
            var on_odd = false;

            foreach (var x in xs)
                yield return ((on_odd = !on_odd) ? odd : even)(x);
        }

        //Alternative to LINQ's "select"
        public static IEnumerable<TResult> Map<T, TResult>(this IEnumerable<T> xs, Func<T, TResult> f)
        {
            return xs.Select(f);
        }

        //Alternative to LINQ's "select"
        public static IEnumerable<TResult> Map<T, TResult>(this IEnumerable<T> xs, int n, Func<T, int, TResult> fn)
        {
            return xs.Select(fn);
        }

        //public static IEnumerable<T> Bind<S, T>(this IEnumerable<S> xs, Func<S, IEnumerable<T>> f)
        //{
        //    return xs.SelectMany(f);
        //}

        //public static IEnumerable<T> Bind<S, T>(this IEnumerable<S> xs, Func<S, int, IEnumerable<T>> f)
        //{
        //    return xs.SelectMany(f);
        //}

        //public static IEnumerable<U> Bind<S, T, U>(this IEnumerable<S> xs, Func<S, IEnumerable<T>> f, Func<S, T, U> g)
        //{
        //    return xs.SelectMany(f, g);
        //}

        //public static IEnumerable<U> Bind<S, T, U>(this IEnumerable<S> xs, Func<S, int, IEnumerable<T>> f, Func<S, T, U> g)
        //{
        //    return xs.SelectMany(f, g);
        //}

        public static IEnumerable<T> Except<T>(this IEnumerable<T> xs, T x)
        {
            return xs.Except (new[] { x });
        }

        public static IEnumerable<T> Reject<T>(this IEnumerable<T> xs, Func<T, bool> predicate)
        {
            return xs.Where(x => !predicate(x));
        }

        public static IEnumerable<T> SkipAll<T>(this IEnumerable<T> xs)
        {
            #pragma warning disable
            
            while (false)
                yield return default(T);
            
            #pragma warning restore
        }

        public sealed class Partition<T>
        {
            public readonly IEnumerable<T> Part;
            public readonly IEnumerable<T> Rest;

            internal Partition(IEnumerable<T> Part, IEnumerable<T> Rest)
            {
                this.Part = Part;
                this.Rest = Rest;
            }
        }

        public sealed class TaggedEnumerable<K, T> : IEnumerable<T>
        {
            public K Tag { get; private set; }

            IEnumerable<T> list;

            internal TaggedEnumerable(K tag, IEnumerable<T> list)
            {
                Tag = tag;
                this.list = list;
            }
            
            public IEnumerator<T> GetEnumerator()
            {
                return list.GetEnumerator();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        public sealed class TaggedEnumerableCollection<K, T> : IEnumerable<TaggedEnumerable<K, T>>
        {
            readonly List<TaggedEnumerable<K, T>> xs;
            public readonly IEnumerable<T> Remainder;

            internal TaggedEnumerableCollection(IEnumerable<TaggedEnumerable<K, T>> xs, IEnumerable<T> Remainder)
            {
                this.xs = new List<TaggedEnumerable<K, T>>(xs);
                this.Remainder = Remainder;
            }

            public IEnumerator<TaggedEnumerable<K, T>> GetEnumerator()
            {
                return xs.GetEnumerator();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        public static Partition<T> PartitionBy<T>(this IEnumerable<T> xs, Func<T, bool> p)
        {
            var part = new List<T>();
            var rest = new List<T>();

            foreach (var x in xs)
                if (p(x))
                    part.Add(x);
                else
                    rest.Add(x);

            return new Partition<T>(part, rest);
        }

        public static TaggedEnumerableCollection<U, T> PartitionBy<T, U>(this IEnumerable<T> xs, IEnumerable<U> ys, Func<U, Func<T, bool>> predicateBuilder)
        {
            var ps = ys.Select(y => new { Tag = y, Predicate = predicateBuilder(y) });

            var partitions = new List<TaggedEnumerable<U,T>>();
            Partition<T> partition = null;

            foreach (var p in ps)
            {
                partition = xs.PartitionBy(p.Predicate);
                partitions.Add(new TaggedEnumerable<U,T>(p.Tag, partition.Part));
                xs = partition.Rest;
            }

            return new TaggedEnumerableCollection<U, T>(partitions, partition.Rest);
        }
    }
}
