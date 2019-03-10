using System;
using System.Runtime.CompilerServices;

namespace PokeAPI
{
    public enum Unit : byte { Value }

    public static class Maybe
    {
        private const MethodImplOptions AggressiveInlining = (MethodImplOptions)0x100; // AggressiveInlining, will only happen when .NET 4.5+ is installed.

        [MethodImpl(AggressiveInlining)]
        public static Maybe<T> Just<T>(T value) => new Maybe<T>(value);
    }
    public

        struct Maybe<T> // Nullable is only for structs, need support for any T
    {
        public static readonly Maybe<T> Nothing = new Maybe<T>();

        readonly T v;

        public bool HasValue
        {
            get;
        }
        public T Value => !HasValue ? throw new InvalidOperationException() : v;

        public Maybe(T value)
        {
            HasValue = true;
            v = value;
        }
    }
}
