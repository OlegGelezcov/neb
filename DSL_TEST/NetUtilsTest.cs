using Nebula;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSL_TEST {
    public static class NetUtilsTest {

        public static void TestSendToSlack() {
            Task.Factory.StartNew(() => {
                NetUtils.SendToSlack("Server test", "some message");
            });
        }
    }
}
