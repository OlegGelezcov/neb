// SafeEnumerator.cs
// Nebula
// 
// Created by Oleg Zhelestcov on Tuesday, November 4, 2014 5:13:12 PM
// Copyright (c) 2014 KomarGames. All rights reserved.
//

namespace Common {

    using System.Collections.Generic;
    using System.Threading;

    public class SafeEnumerator<T> : IEnumerator<T> {
        private readonly IEnumerator<T> m_Inner;
        private readonly object m_Lock;

        public SafeEnumerator(IEnumerator<T> inner, object @lock) {
            m_Inner = inner;
            m_Lock = @lock;
            Monitor.Enter(m_Lock);
        }

        public void Dispose() {
            Monitor.Exit(m_Lock);
        }

        public bool MoveNext() {
            return m_Inner.MoveNext();
        }

        public void Reset() {
            m_Inner.Reset();
        }

        public T Current {
            get {
                return m_Inner.Current;
            }
        }

        object System.Collections.IEnumerator.Current {
            get {
                return Current;
            }
        }
    }
}
