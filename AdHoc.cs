using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Prelude
{
    public static class _Utils
    {
        public static ISet<T> CreateSet<T>(this IEqualityComparer<T> eq)
        {
            return new HashSet<T>(eq);
        }

        public static ISet<T> CreateSet<T>(this IEqualityComparer<T> eq, IEnumerable<T> xs)
        {
            return new HashSet<T>(xs, eq);
        }

        public static ISet<T> CreateSet<T>(this Func<T, T, bool> eq)
        {
            return new HashSet<T>(new _EqualityComparer<T>(eq, x => x.GetHashCode()));
        }

        public static ISet<T> AddRange<T>(this ISet<T> s, IEnumerable<T> xs)
        {
            if (xs != null)
                foreach (var x in xs)
                    s.Add(x);

            return s;
        }
    }

    public class _Disposable : IDisposable
    {
        Action dispose;

        public _Disposable(Action dispose)
        {
            this.dispose = dispose;
        }

        public void Dispose()
        {
            dispose();
        }
    }

    public class _EqualityComparer<T> : IEqualityComparer<T>
    {
        Func<T, T, bool> equals;
        Func<T, int> _getHashCode;

        public _EqualityComparer(Func<T, T, bool> eq, Func<T, int> hashCode)
        {
            equals = eq;
            _getHashCode = hashCode;
        }

        public bool Equals(T x, T y)
        {
            return equals(x, y);
        }

        public int GetHashCode(T obj)
        {
            return _getHashCode(obj);
        }
    }
    
    public class _Equatable<T> : IEquatable<T>
    {
        readonly Func<T, bool> equals;

        public _Equatable(Func<T, bool> eq)
        {
            equals = eq;
        }
        
        public bool Equals(T other)
        {
            return equals(other);
        }
    }
    
    public class _Comparer<T> : IComparer<T>
    {
        readonly Func<T, T, int> comparer;

        public _Comparer(Func<T, T, int> comparer)
        {
            this.comparer = comparer;
        }        
        
        public int Compare(T x, T y)
        {
            return comparer(x, y);
        }
    }

    public class _Comparable<T> : IComparable<T>
    {
        readonly Func<T, int> compare;

        public _Comparable(Func<T, int> compare)
        {
            this.compare = compare;
        }
        
        public int CompareTo(T other)
        {
            return compare(other);
        }
    }

    public class _Enumerator<T> : IEnumerator<T>
    {
        readonly Func<T> current;
        readonly Action dispose, reset;
        readonly Func<bool> moveNext;

        public _Enumerator(Func<T> current, Func<bool> moveNext, Action dispose, Action reset)
        {
            this.current = current;
            this.moveNext = moveNext;
            this.dispose = dispose;
            this.reset = reset;
        }

        public _Enumerator(Func<T> current, Func<bool> moveNext) : this(current, moveNext, null, null) { }        
        
        public T Current
        {
            get { return current(); }
        }

        public void Dispose()
        {
            if (dispose != null)
                dispose();
        }

        object System.Collections.IEnumerator.Current
        {
            get { return this.Current; }
        }

        public bool MoveNext()
        {
            return this.moveNext();
        }

        public void Reset()
        {
            if (reset != null)
                reset();
        }

        public IEnumerable<T> ToEnumerable()
        {
            return new _Enumerable<T>(() => this);
        }
    }    

    public class _Enumerable<T> : IEnumerable<T>
    {
        Func<IEnumerator<T>> _getEnumerator;

        public _Enumerable(Func<IEnumerator<T>> getEnumerator)
        {
            _getEnumerator = getEnumerator;
        }
        
        public IEnumerator<T> GetEnumerator()
        {
            return _getEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class _Observer<T> : IObserver<T>
    {
        Action onCompleted;
        Action<Exception> error;
        Action<T> onNext;

        public _Observer(Action completed, Action<Exception> error, Action<T> next)
        {
            this.onCompleted = completed;
            this.error = error;
            this.onNext = next;
        }
        
        public void OnCompleted()
        {
            onCompleted();
        }

        public void OnError(Exception error)
        {
            this.error(error);
        }

        public void OnNext(T value)
        {
            onNext(value);
        }
    }

    public class _Observable<T> : IObservable<T>
    {
        Func<IObserver<T>, IDisposable> subscribe;

        public _Observable(Func<IObserver<T>, IDisposable> subscribe)
        {
            this.subscribe = subscribe;
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return subscribe(observer);
        }
    }
}