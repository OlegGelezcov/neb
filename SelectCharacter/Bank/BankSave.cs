using MongoDB.Bson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectCharacter.Bank {
    public class BankSave {
        public ObjectId Id { get; set; }
        public string login { get; set; }
        public Hashtable bankInfo { get; set; }
    }
}
