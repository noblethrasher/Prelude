using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Prelude
{
    public class Toggler
    {
        public static Toggler<A> Create<A>(Action<A> act1, Action<A> act2)
        {
            return new Toggler<A>(act1, act2);
        }

        public static Toggler<A, B> Create<A, B>(Action<A, B> act1, Action<A, B> act2)
        {
            return new Toggler<A, B>(act1, act2);
        }
        
        Action act1, act2;
        public bool State { get; private set; }

        public Toggler(Action act1, Action act2)
        {
            this.act1 = act1;
            this.act2 = act2;
        }

        public void Toggle()
        {
            if (State)
                act1();
            else
                act2();

            State = !State;
        }
    }

    public class Toggler<A>
    {
        Action<A> act1, act2;
        public bool State { get; private set; }

        public Toggler(Action<A> act1, Action<A> act2)
        {
            this.act1 = act1;
            this.act2 = act2;
        }

        public void Toggle(A obj)
        {
            if (State)
                act1(obj);
            else
                act2(obj);

            State = !State;
        }
    }

    public class Toggler<A, B>
    {
        Action<A, B> act1, act2;
        public bool State { get; private set; }

        public Toggler(Action<A, B> act1, Action<A, B> act2)
        {
            this.act1 = act1;
            this.act2 = act2;
        }

        public void Toggle(A obj1, B obj2)
        {
            if (State)
                act1(obj1, obj2);
            else
                act2(obj1, obj2);

            State = !State;
        }
    }
}
