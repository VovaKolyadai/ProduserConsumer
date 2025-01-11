using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdConsum
{
    public class SafeStack<T> : IProducerConsumerCollection<T>
    {
        private object _lock = new object();
        private Stack<T> _sequentiqlStack = null;
        public SafeStack()
        {
            _sequentiqlStack = new Stack<T>();
        }
        public SafeStack(IEnumerable<T> collection)
        {
            _sequentiqlStack = new Stack<T>(collection);
        }
        public void Push(T item)
        {
            lock (_lock) _sequentiqlStack.Push(item);
        }
        public bool TryPop(out T item)
        {
            bool rval = true;
            lock(_lock)
            {
                if(_sequentiqlStack.Count == 0)
                {
                    item = default(T);
                    rval = false;
                }
                else
                {
                    item = _sequentiqlStack.Pop();
                }
                return rval;
            }
        }
        public int Count
        {
            get { return _sequentiqlStack.Count; }
        }
        public bool IsSynchronized
        {
            get { return true; }
        }

        public object SyncRoot
        {
            get { return _lock; }
        }
        public void CopyTo(T[] array, int index)
        {
            lock (_lock) _sequentiqlStack.CopyTo(array, index);
        }

        public void CopyTo(Array array, int index)
        {
            lock (_lock) ((ICollection)_sequentiqlStack).CopyTo(array, index);
        }

        public IEnumerator<T> GetEnumerator()
        {
            Stack<T> stackCopy = null;
            lock (_lock) stackCopy = new Stack<T>(_sequentiqlStack);
            return stackCopy.GetEnumerator();
        }

        public T[] ToArray()
        {
            T[] rval = null;
            lock (_lock) rval = _sequentiqlStack.ToArray();
            return rval;
        }

        public bool TryAdd(T item)
        {
            Push(item); 
            return true;
        }

        public bool TryTake([MaybeNullWhen(false)] out T item)
        {
            return TryPop(out item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<T>)this).GetEnumerator();
        }
    }
}
