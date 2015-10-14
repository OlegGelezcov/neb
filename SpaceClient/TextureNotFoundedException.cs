using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nebula.Client
{
    public class TextureNotFoundedException : NebulaException
    {
        public string TexturePath { get; private set; }

        public TextureNotFoundedException(string texturePath)
            : base()
        {
            this.TexturePath = texturePath;
        }

        public TextureNotFoundedException(string message, string texturePath)
            : base(message)
        {
            this.TexturePath = texturePath;
        }

        public TextureNotFoundedException(string message, string texturePath, Exception innerException)
            : base(message, innerException)
        {
            this.TexturePath = texturePath;
        }
    }
}
