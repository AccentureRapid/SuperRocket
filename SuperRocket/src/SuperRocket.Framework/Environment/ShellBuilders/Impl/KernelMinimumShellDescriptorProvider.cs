using SuperRocket.Framework.Environment.Descriptor.Models;
using System.Collections.Generic;

namespace SuperRocket.Framework.Environment.ShellBuilders.Impl
{
    internal sealed class KernelMinimumShellDescriptorProvider : IMinimumShellDescriptorProvider
    {
        #region Implementation of IMinimumShellDescriptorProvider

        /// <summary>
        /// 获取外壳描述符。
        /// </summary>
        /// <param name="features">外壳特性描述符。</param>
        public void GetFeatures(ICollection<ShellFeature> features)
        {
            var list = new[]
            {
                new ShellFeature {Name = "SuperRocket.Framework"},
                new ShellFeature {Name = "Settings"}
            };

            foreach (var feature in list)
                features.Add(feature);
        }

        #endregion Implementation of IMinimumShellDescriptorProvider
    }
}