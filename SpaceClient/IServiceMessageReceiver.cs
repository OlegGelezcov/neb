using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Client
{
    /// <summary>
    /// Interface which realize all Service message receivers
    /// </summary>
    public interface IServiceMessageReceiver
    {
        void AddMessage(Hashtable messageTable);
        IServiceMessage[] RecentMessages(int count);
        IServiceMessage[] Messages { get; }

        IServiceMessage[] MessagesAtIndex(int index);
        void Clear();
        int Count { get; }
        int MaxCount { get; }
    }
}
