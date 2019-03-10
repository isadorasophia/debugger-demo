using System;

#if NETFRAMEWORK
namespace PokeAPI
{
    public struct ValueTuple<T1, T2>
    {
        public T1 Item1;
        public T2 Item2;

        public ValueTuple(T1 i1, T2 i2)
        {
            Item1 = i1;
            Item2 = i2;
        }
    }

    public static class ValueTuple
    {
        public static ValueTuple<T1, T2> Create<T1, T2>(T1 i1, T2 i2)
        {
            return new ValueTuple<T1, T2>(i1, i2);
        }
    }
}
#endif

