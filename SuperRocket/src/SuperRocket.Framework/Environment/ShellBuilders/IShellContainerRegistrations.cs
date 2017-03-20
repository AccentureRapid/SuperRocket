using Autofac;
using SuperRocket.Framework.Environment.ShellBuilders.Models;

namespace SuperRocket.Framework.Environment.ShellBuilders
{
    /// <summary>
    /// 一个抽象的外壳容器注册提供程序。
    /// </summary>
    public interface IShellContainerRegistrations
    {
        /// <summary>
        /// 注册动作。
        /// </summary>
        /// <param name="builder">容器建造者。</param>
        /// <param name="blueprint">外壳蓝图。</param>
        void Registrations(ContainerBuilder builder, ShellBlueprint blueprint);
    }
}