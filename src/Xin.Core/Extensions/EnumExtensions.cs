using System;
using System.ComponentModel;

namespace Xin.Core.Extensions
{
    /// <summary>
    ///     枚举扩展方法
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        ///     将枚举转换为数字
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static int ToInt32(this Enum source)
        {
            return Convert.ToInt32(source);
        }

        /// <summary>
        ///     获取枚举值的描述信息
        /// </summary>
        /// <param name="source">枚举值</param>
        /// <returns></returns>
        public static string GetDescription(this Enum source)
        {
            var typeDescription = typeof(DescriptionAttribute);
            var fields = source.GetType().GetFields();
            var strText = string.Empty;
            var strValue = string.Empty;
            foreach (var field in fields)
                if (field.FieldType.IsEnum && field.Name.Equals(source.ToString()))
                {
                    var arr = field.GetCustomAttributes(typeDescription, true);
                    if (arr.Length > 0)
                    {
                        var aa = (DescriptionAttribute) arr[0];
                        strText = aa.Description;
                    }
                    else
                    {
                        strText = field.Name; 
                    }

                    break;
                }

            return strText;
        }

        /// <summary>
        ///     获取枚举值的描述信息
        /// </summary>
        /// <param name="enumType">Type,该参数的格式为typeof(需要读的枚举类型)</param>
        /// <param name="source">枚举值</param>
        /// <returns></returns>
        public static string GetDescription(this Enum source, Type enumType)
        {
            var typeDescription = typeof(DescriptionAttribute);
            var fields = enumType.GetFields();
            var strText = string.Empty;
            var strValue = string.Empty;
            foreach (var field in fields)
                if (field.FieldType.IsEnum && field.Name.Equals(source.ToString()))
                {
                    var arr = field.GetCustomAttributes(typeDescription, true);
                    if (arr.Length > 0)
                    {
                        var aa = (DescriptionAttribute) arr[0];
                        strText = aa.Description;
                    }
                    else
                    {
                        strText = field.Name;
                    }

                    break;
                }

            return strText;
        }
    }
}
