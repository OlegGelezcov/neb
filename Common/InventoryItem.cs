using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Common
{
    public class InventoryItem<T>
    {
        protected int _count;
        protected T _object;

        public InventoryItem(T obj, int count) {
            Set(obj, count);
        }

        public InventoryItem()
        {
            Set(default(T), 0);
        }

        public void Set(T obj, int count) {
            _object = obj;
            _count = count;
        }

        public void Add(int count) {
            if (count > 0) {
                _count += count;
            }
        }

        public void Remove(int count) {
            if (count > 0) {
                _count -= count;
                if (_count < 0) {
                    _count = 0;
                }
            }
        }

        public int Count {
            get {
                return _count;
            }
        }

        public bool Has {
            get {
                return _count > 0;
            }
        }

        public T Object {
            get {
                return _object;
            }
        }

        public virtual Hashtable GetInfo() {
            return new Hashtable();
        }
    }
}
