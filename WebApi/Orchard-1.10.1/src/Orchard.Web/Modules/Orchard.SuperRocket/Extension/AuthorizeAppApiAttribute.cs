using System;

namespace Orchard.SuperRocket.Extension
{
    public class AuthorizeAppApiAttribute : Attribute {
        public AuthorizeAppApiAttribute() {
            Enabled = true;
        }
        public AuthorizeAppApiAttribute(bool enabled) {
            Enabled = enabled;
        }

        public bool Enabled { get; set; }
    }
}
