using SuperRocket.Framework.Caching;
using System;

namespace SuperRocket.Framework.Extensions
{
    /// <summary>
    /// 一个抽象的扩展监控协调者。
    /// </summary>
    public interface IExtensionMonitoringCoordinator
    {
        /// <summary>
        /// 监控扩展。
        /// </summary>
        /// <param name="monitor">监控动作。</param>
        void MonitorExtensions(Action<IVolatileToken> monitor);
    }
}