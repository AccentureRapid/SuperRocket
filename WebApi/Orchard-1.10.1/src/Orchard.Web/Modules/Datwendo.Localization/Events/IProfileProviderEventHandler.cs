using Orchard.Events;
using Orchard.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datwendo.Localization.Events
{
    public class PreferedCulture
    {
        public IUser User{ get; set; }
        public string Culture { get; set; }
       public IEnumerable<Tuple<string, int>> PreferredList { get; set; }
    }

    public interface IProfileProviderEventHandler : IEventHandler
    {
        /// <summary>
        /// Called when the user's preferred culture is requested
        /// </summary>
        void ProvidePreferredCultureRequest(PreferedCulture pref);
    }

    // A default implementation to use as an example
    public class DefaultProfileProviderEventHandler : IProfileProviderEventHandler
    {
        public void ProvidePreferredCultureRequest(PreferedCulture pref)
        {
            // Fill the pref
        }
    }
}
