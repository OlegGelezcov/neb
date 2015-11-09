using ServerClientCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestClient {
    public class TestCryptoPassword {

        public void Test() {
            string password = "nebulaonline";

            string sourcePassword = StringChiper.Encrypt("87e898AA");
            Console.WriteLine("encrypted password: {0}", sourcePassword);

            string decriptedPassword = StringChiper.Decrypt(sourcePassword);
            Console.WriteLine("decripted password: {0}", decriptedPassword);
        }
    }
}
