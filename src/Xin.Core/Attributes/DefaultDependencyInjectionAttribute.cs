using System;

namespace Xin.Core.Attributes
{
    public enum DefaultDependencyInjectionType
    {
        Transient,
        Scoped,
        Singleton
    }

    /// <summary>
    /// 默认DI
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class DefaultDependencyInjectionAttribute : Attribute
    {
        /// <summary>
        /// 注入类型
        /// </summary>
        public DefaultDependencyInjectionType DependencyInjectionType { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        public DefaultDependencyInjectionAttribute(
            DefaultDependencyInjectionType type = DefaultDependencyInjectionType.Scoped)
        {
            DependencyInjectionType = type;
        }
    }
}
