
namespace Nebula.Client.Res
{
    public class ResHelpElement
    {
        private string icon;
        private string text;

        public ResHelpElement(string text, string icon)
        {
            this.icon = icon;
            this.text = text;
        }

        public string Text()
        {
            return this.text;
        }

        public string Icon()
        {
            return this.icon;
        }

        public bool HasIcon()
        {
            return (!string.IsNullOrEmpty(this.icon));
        }

    }
}
