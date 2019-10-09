using System;

namespace Xin.Core.Extensions
{
    /// <summary>
    ///     long扩展
    /// </summary>
    public static class Int64Extensions
    {
        /// <summary>
        ///     将指定的长整值转换为对应的字节大小
        /// </summary>
        /// <param name="fileSize"></param>
        /// <returns></returns>
        public static string ToFileSize(this long fileSize)
        {
            if (fileSize < 0x400L) return $"{fileSize}Byte";
            if (fileSize >= 0x400L && fileSize <= 0x100000L)
                return $"{fileSize * 1.0 / 0x400L:F2}KB".TrimEnd('0').TrimEnd('.');
            if (fileSize >= 0x100000L && fileSize <= 0x40000000L)
                return $"{fileSize * 1.0 / 0x100000L:F2}MB".TrimEnd('0').TrimEnd('.');
            if (fileSize >= 0x40000000L)
                return $"{fileSize * 1.0 / 0x40000000L:F2}GB".TrimEnd('0').TrimEnd('.');
            return "";
        }

        /// <summary>
        ///     将字符串转成指定的枚举类型(字符串可以是枚举的名称也可以是枚举值)
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <param name="source">源字符串</param>
        /// <param name="defaultValue">如果转换失败，返回默认的枚举项</param>
        /// <returns></returns>
        public static T ToEnum<T>(this long source, T defaultValue)
        {
            try
            {
                var value = (T) Enum.Parse(typeof(T), source.ToString(), true);
                if (Enum.IsDefined(typeof(T), value)) return value;
            }
            catch
            {
                // ignored
            }

            return defaultValue;
        }

        /// <summary>
        ///     将字符串转成指定的枚举类型(字符串可以是枚举的名称也可以是枚举值)
        ///     <remarks>支持枚举值的并集</remarks>
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <param name="source">源字符串</param>
        /// <param name="defaultValue">如果转换失败，返回默认的枚举项</param>
        /// <returns></returns>
        public static T ToEnumExt<T>(this long source, T defaultValue)
        {
            try
            {
                return (T) Enum.Parse(typeof(T), source.ToString(), true);
            }
            catch
            {
                // ignored
            }

            return defaultValue;
        }
    }
}
