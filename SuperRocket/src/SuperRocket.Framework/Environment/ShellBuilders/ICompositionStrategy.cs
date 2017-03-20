using SuperRocket.Framework.Environment.Configuration;
using SuperRocket.Framework.Environment.Descriptor.Models;
using SuperRocket.Framework.Environment.ShellBuilders.Models;

namespace SuperRocket.Framework.Environment.ShellBuilders
{
    /// <summary>
    /// 一个抽象的组合策略。
    /// </summary>
    public interface ICompositionStrategy
    {
        /// <summary>
        /// 组合外壳蓝图。
        /// </summary>
        /// <param name="settings">外壳设置。</param>
        /// <param name="descriptor">外壳描述符。</param>
        /// <returns>外壳蓝图。</returns>
        ShellBlueprint Compose(ShellSettings settings, ShellDescriptor descriptor);
    }
}