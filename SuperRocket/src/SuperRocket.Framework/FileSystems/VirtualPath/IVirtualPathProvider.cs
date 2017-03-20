using SuperRocket.Framework.Caching;

namespace SuperRocket.Framework.FileSystems.VirtualPath
{
    /// <summary>
    /// 一个抽象的虚拟路径提供者。
    /// </summary>
    public interface IVirtualPathProvider : IVirtualPathProviderBase, IVolatileProvider
    {
    }
}