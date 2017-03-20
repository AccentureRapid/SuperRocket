using Autofac;
using SuperRocket.Framework.Environment.Configuration;
using SuperRocket.Framework.Environment.Descriptor.Models;
using SuperRocket.Framework.Environment.ShellBuilders.Models;

namespace SuperRocket.Framework.Environment.ShellBuilders
{
    /// <summary>
    /// 外壳上下文。
    /// </summary>
    public sealed class ShellContext
    {
        /// <summary>
        /// 外壳设置。
        /// </summary>
        public ShellSettings Settings { get; set; }

        /// <summary>
        /// 外壳描述符。
        /// </summary>
        public ShellDescriptor Descriptor { get; set; }

        /// <summary>
        /// 外壳蓝图。
        /// </summary>
        public ShellBlueprint Blueprint { get; set; }

        /// <summary>
        /// 外壳容器。
        /// </summary>
        public ILifetimeScope Container { get; set; }

        /// <summary>
        /// 外壳。
        /// </summary>
        public IShell Shell { get; set; }
    }
}