using System;

namespace Xin.Core.Attributes
{
    /// <summary>
    /// 注册类型
    /// </summary>
    public enum DefaultDependencyInjectionType
    {
        /// <summary>
        /// 
        /// </summary>
        Transient,

        /// <summary>
        /// 
        /// </summary>
        Scoped,

        /// <summary>
        /// 
        /// </summary>
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
