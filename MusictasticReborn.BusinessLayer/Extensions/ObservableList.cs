using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Collections.Generic
{
    public class ObservableList<T> : List<T>, INotifyCollectionChanged
    {
        public ObservableList()
            : base()
        {
        }

        public ObservableList(IEnumerable<T> et)
            : base(et)
        {
        }

        #region ObservableList<T> Members

        public new void Insert(int index, T item)
        {
            base.Insert(index, item);
            NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public new void Add(T item)
        {
            base.Add(item);
            NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, base.Count - 1));
        }

        public new void InsertRange(int index, IEnumerable<T> et)
        {
            base.InsertRange(index, et);
            NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public new void AddRange(IEnumerable<T> et)
        {
            base.AddRange(et);
            NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public new void RemoveAt(int index)
        {
            object obj = base[index];
            base.RemoveAt(index);
            NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, obj, index));
        }

        public new bool Remove(T item)
        {
            int ind = base.IndexOf(item);
            if (base.Remove(item))
            {
                NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, ind));
                return true;
            }
            return false;
        }

        public new void RemoveRange(int index, int count)
        {
            base.RemoveRange(index, count);
            NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public new void Clear()
        {
            base.Clear();
            NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void OrderBy()
        {
            this.OrderBy();
            NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public new void Sort()
        {
            base.Sort();
            NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public new IEnumerator<T> GetEnumerator()
        {
            return base.GetEnumerator();
        }
        #endregion


        #region events
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public void NotifyCollectionChanged(NotifyCollectionChangedEventArgs NCCEA)
        {
            if (CollectionChanged != null)
            {
                CollectionChanged(this, NCCEA);
            }
        }
        #endregion

    }
}
