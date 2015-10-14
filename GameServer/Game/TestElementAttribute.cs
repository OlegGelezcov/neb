using System;

namespace Space.Game
{
    [AttributeUsage(AttributeTargets.Method)]
    public class TestElementAttribute : System.Attribute
    {
        public readonly string Url;

        public TestElementAttribute(string url) {
            this.Url = url;
        }
    }
}
