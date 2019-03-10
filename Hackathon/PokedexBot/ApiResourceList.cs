using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LitJson;

namespace PokeAPI
{
    struct ResourceListFragment<T, TInner>
        where TInner : ApiObject
        where T : ApiResource<TInner>
    {
        public int Count
        {
            get;
            internal set;
        }

        public Uri Next
        {
            get;
            internal set;
        }
        public Uri Previous
        {
            get;
            internal set;
        }

        public T[] Results
        {
            get;
            internal set;
        }
    }

    struct ResourceListEnumerator<T, TInner> : IEnumerator<T>
        where TInner : ApiObject
        where T : ApiResource<TInner>
    {
        ResourceListFragment<T, TInner> current, start;
        int index, limit;

        public T Current
        {
            get
            {
                if (index == -1)
                    throw new InvalidOperationException();

                return current.Results[index];
            }
        }

        object IEnumerator.Current => Current;

        internal ResourceListEnumerator(int limit = 20, ResourceListFragment<T, TInner>? start = null)
        {
            index = 0;
            this.limit = limit;

            this.start = current = default(ResourceListFragment<T, TInner>);

            if (start.HasValue)
                this.start = current = start.Value;
            else
            {
                var t = DataFetcher.GetListJsonOf<TInner>(0, limit);

                var j = t.Result;
                start = current = JsonMapper.ToObject<ResourceListFragment<T, TInner>>(j);
            }

            this.limit = current.Results.Length;
        }

        public void Dispose()
        {
            current = new ResourceListFragment<T, TInner>();
            index = -1;
        }

        public bool MoveNext()
        {
            if (index == -1)
                return false;

            if (++index >= limit)
            {
                if (current.Next == null)
                {
                    index = -1;
                    return false;
                }

                index = 0;

                var t = DataFetcher.GetJsonOf<TInner>(current.Next);
                Task.WaitAll(t);
                if (t.IsFaulted)
                    throw t.Exception;

                var j = t.Result;
                current = JsonMapper.ToObject<ResourceListFragment<T, TInner>>(j);
                limit = current.Results.Length;
            }

            return true;
        }

        public void Reset()
        {
            index = 0;
            current = start;
        }
    }

    public class ResourceList<T, TInner> : IEnumerable<T>
        where TInner : ApiObject
        where T : ApiResource<TInner>
    {
        ResourceListFragment<T, TInner> start;

        public int Count
        {
            get;
        }

        public int Limit
        {
            get;
            set;
        }

        internal ResourceList(int count, int limit, ResourceListFragment<T, TInner> st)
        {
            Count = count;
            Limit = limit;

            start = st;
        }

        public IEnumerator<T> GetEnumerator() => new ResourceListEnumerator<T, TInner>(Limit, start);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
