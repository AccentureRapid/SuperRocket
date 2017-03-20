using SuperRocket.Framework.Extensions.Models;
using System;

namespace SuperRocket.Framework.Environment.ShellBuilders.Models
{
    /// <summary>
    /// 蓝图项。
    /// </summary>
    public class BlueprintItem
    {
        /// <summary>
        /// 类型。
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// 特性条目。
        /// </summary>
        public Feature Feature { get; set; }
    }
}