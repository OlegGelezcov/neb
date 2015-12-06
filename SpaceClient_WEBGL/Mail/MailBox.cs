﻿using Common;
using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;

namespace Nebula.Client.Mail {
    public class MailBox : IInfoParser {

        public Dictionary<string, MailMessage> messages { get; private set; }

        public void ParseInfo(Hashtable info) {
            messages = new Dictionary<string, MailMessage>();

            foreach (System.Collections.DictionaryEntry messageEntry in info) {
                Hashtable messageInfo = messageEntry.Value as Hashtable;
                if (messageInfo == null) {
                    throw new FormatException("message info must be Hashtable");
                }
                messages.Add(messageEntry.Key.ToString(), new MailMessage(messageInfo));
            }
        }
    }
}
