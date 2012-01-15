using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Prelude
{
    abstract class MetricSpace<T>
    {
        readonly double delta;

        public MetricSpace(double delta)
        {
            this.delta = delta;
        }

        public abstract double Distance(T x, T y);

        public IEnumerable<T> Neighborhood(IEnumerable<T> xs, T p)
        {
            foreach (var x in xs)
                if (Distance(x, p) <= delta)
                    yield return x;
        }

        public IEnumerable<T> DeletedNeighborhood(IEnumerable<T> xs, T p)
        {
            foreach (var x in xs)
            {
                var distance = Distance(x, p);
                if (distance <= delta && distance > 0)
                    yield return x;
            }
        }
    }

    public interface Metric<T> : IEnumerable<T>
    {
        T Origin { get; } //The “zero” of the set T.
        
        double Distance(T x, T y);        
        double Delta { get; }
    }

    class _Metric<T> : Metric<T>
    {
        private IEnumerable<T> xs;
        private Func<T, T, double> distance;
        private Func<T> origin;
        private Func<double> delta;

        public _Metric(IEnumerable<T> xs, Func<T, T, double> distance, Func<T> origin, Func<double> delta)
        {
            this.xs = xs;
            this.distance = distance;
            this.origin = origin;
            this.delta = delta;
        }

        public _Metric(IEnumerable<T> xs, Func<T, T, double> distance, Func<T> origin) : this(xs, distance, origin, () => 0)
        {
            
        }

        public T Origin
        {
            get { return origin(); }
        }

        public double Distance(T x, T y)
        {
            return distance(x, y);
        }

        public double Delta
        {
            get { return delta(); }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return xs.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public static class Metric
    {
        public static Metric<T> ToMetric<T>(this IEnumerable<T> xs, Func<T, T, double> distance, Func<T> origin, Func<double> delta)
        {
            return new _Metric<T>(xs, distance, origin, delta);
        }

        public static Metric<T> ToMetric<T>(this IEnumerable<T> xs, Func<T, T, double> distance, Func<T> origin)
        {
            return ToMetric<T>(xs, distance, origin, null);
        }

        public static IEnumerable<T> Neighborhoood<T>(this Metric<T> metric, T point)
        {
            foreach (var x in metric)
                if (metric.Distance(x, point) <= metric.Delta)
                    yield return x;
        }

        public static IEnumerable<T> DeletedNeighborhood<T>(this Metric<T> metric, T point)
        {
            foreach (var x in metric)
            {
                var distance = metric.Distance(x, point);
                if (distance <= metric.Delta && distance > 0)
                    yield return x;
            }
        }

        public static IEnumerable<T> EpsilonNeighborhood<T>(this Metric<T> metric, T point, double epsilon)
        {
            foreach (var x in metric)
                if (metric.Distance(point, x) <= epsilon)
                    yield return x;
        }

        public static IEnumerable<T> DeletedEpsilonNeighborhood<T>(this Metric<T> metric, T point, double epsilon)
        {
            foreach (var x in metric)
            {
                var distance = metric.Distance(point, x);                
                if (distance <= epsilon && distance > 0)
                    yield return x;
            }
        }

        public static IEnumerable<T> Max<T>(this Metric<T> metric)
        {            
            var origin = metric.Origin;
            var dist = 0d;

            foreach (var x in metric)
                dist = Math.Max(dist, metric.Distance(x, origin));

            return new List<T>(metric.Where(x => metric.Distance(x, origin) >= dist));                
        }

        public static IEnumerable<T> Min<T>(this Metric<T> metric)
        {
            var origin = metric.Origin;
            var dist = double.MaxValue;

            foreach (var x in metric)
                dist = Math.Min(dist, metric.Distance(x, origin));

            return new List<T>(metric.Where(x => metric.Distance(x, origin) <= dist));
        }
    }
}
