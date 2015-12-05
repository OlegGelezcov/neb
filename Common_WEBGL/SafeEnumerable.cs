// SafeEnumerable.cs
// Nebula
// 
// Created by Oleg Zhelestcov on Tuesday, November 4, 2014 5:31:06 PM
// Copyright (c) 2014 KomarGames. All rights reserved.
//


namespace Common {
    using System.Collections.Generic;


    public class SafeEnumerable<T> : IEnumerable<T> {
        private readonly IEnumerable<T> m_Inner;
        private readonly object m_Lock;

        public SafeEnumerable(IEnumerable<T> inner, object @lock) {
            m_Lock = @lock;
            m_Inner = inner;
        }

        public IEnumerator<T> GetEnumerator() {
            return new SafeEnumerator<T>(m_Inner.GetEnumerator(), m_Lock);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}

