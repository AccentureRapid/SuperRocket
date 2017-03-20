using SuperRocket.Framework.Environment;
using SuperRocket.Framework.FileSystems.VirtualPath;

namespace SuperRocket.Framework.FileSystems.Application.Impl
{
    internal sealed class DefaultApplicationFolder : FolderBase, IApplicationFolder
    {
        #region Constructor

        public DefaultApplicationFolder(IHostEnvironment hostEnvironment, IVirtualPathMonitor virtualPathMonitor)
            : base(hostEnvironment, virtualPathMonitor)
        {
        }

        #endregion Constructor

        #region Overrides of VirtualPathProviderBase

        /// <summary>
        /// 根文件夹虚拟路径 ~/ or ~/Abc
        /// </summary>
        public override string RootPath
        {
            get { return "~/"; }
        }

        #endregion Overrides of VirtualPathProviderBase
    }
}