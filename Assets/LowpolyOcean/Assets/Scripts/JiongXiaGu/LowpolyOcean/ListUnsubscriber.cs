using System;
using System.Collections.Generic;

namespace JiongXiaGu.LowpolyOcean
{
    public class ListUnsubscriber<T> : IDisposable
    {
        private List<T> list;
        private T target;

        public ListUnsubscriber(List<T> list, T target)
        {
            this.list = list;
            this.target = target;
        }

        public void Dispose()
        {
            if (list != null)
            {
                list.Remove(target);
                list = default(List<T>);
                target = default(T);
            }
        }
    }
}
