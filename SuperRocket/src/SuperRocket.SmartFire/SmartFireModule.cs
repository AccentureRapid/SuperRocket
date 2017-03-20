using Prism.Modularity;
using Prism.Regions;
using System;

namespace SuperRocket.SmartFire
{
    public class SmartFireModule : IModule
    {
        IRegionManager _regionManager;

        public SmartFireModule(RegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void Initialize()
        {
            //throw new NotImplementedException();
        }
    }
}