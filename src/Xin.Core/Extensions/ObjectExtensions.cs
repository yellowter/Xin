using System;

namespace Xin.Core.Extensions
{
    /// <summary>
    ///     对象扩展方法
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        ///     将对象转成Int32类型，如果转换失败，则返回-1
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static int ToInt32(this object source)
        {
            return source.ToInt32(-1);
        }

        /// <summary>
        ///     将对象转成Int32类型
        /// </summary>
        /// <param name="source"></param>
        /// <param name="defaultValue">如果转换失败，返回的数值</param>
        /// <returns></returns>
        public static int ToInt32(this object source, int defaultValue)
        {
            if (source != null)
            {
                if (source is int)
                    return (int) source;

                int result;
                if (int.TryParse(source.ToString(), out result)) return result;
            }

            return defaultValue;
        }

        /// <summary>
        ///     将对象转成Int64类型，如果转换失败，则返回-1
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static long ToInt64(this object source)
        {
            return source.ToInt64(-1);
        }

        /// <summary>
        ///     将对象转成Int64类型
        /// </summary>
        /// <param name="source"></param>
        /// <param name="defaultValue">如果转换失败，返回的数值</param>
        /// <returns></returns>
        public static long ToInt64(this object source, long defaultValue)
        {
            if (null != source)
            {
                if (source is long)
                    return (long) source;


                long result;
                if (long.TryParse(source.ToString(), out result)) return result;
            }

            return defaultValue;
        }

        /// <summary>
        ///     将对象转成double类型，如果转换失败，则返回-1
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static double ToDouble(this object source)
        {
            return source.ToDouble(-1d);
        }

        /// <summary>
        ///     将对象转成float类型，如果转换失败，则返回-1
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static float ToFloat(this object source)
        {
            return source.ToFloat(-1f);
        }

        /// <summary>
        ///     将对象转成float类型
        /// </summary>
        /// <param name="source"></param>
        /// <param name="defaultValue">如果转换失败，返回的数值</param>
        /// <returns></returns>
        public static float ToFloat(this object source, float defaultValue)
        {
            if (null != source)
            {
                if (source is float f)
                    return f;

                if (float.TryParse(source.ToString(), out var result)) return result;
            }

            return defaultValue;
        }

        /// <summary>
        ///     将对象转成double类型
        /// </summary>
        /// <param name="source"></param>
        /// <param name="defaultValue">如果转换失败，返回的数值</param>
        /// <returns></returns>
        public static double ToDouble(this object source, double defaultValue)
        {
            if (null != source)
            {
                if (source is double)
                    return (double) source;

                if (double.TryParse(source.ToString(), out var result)) return result;
            }

            return defaultValue;
        }

        /// <summary>
        ///     将对象转成Byte类型
        /// </summary>
        /// <param name="source"></param>
        /// <param name="defaultValue">如果转换失败，返回的数值</param>
        /// <returns></returns>
        public static byte ToByte(this object source, byte defaultValue)
        {
            if (source != null)
            {
                if (source is byte)
                    return (byte) source;

                if (byte.TryParse(source.ToString(), out var result)) return result;
            }

            return defaultValue;
        }

        /// <summary>
        ///     将对象转换成DateTime类型
        /// </summary>
        /// <param name="source"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(this object source, DateTime defaultValue)
        {
            if (source != null)
            {
                if (source is DateTime)
                    return (DateTime) source;

                if (DateTime.TryParse(source.ToString(), out var result)) return result;
            }

            return defaultValue;
        }

        /// <summary>
        ///     将对象转换成DateTime类型
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(this object source)
        {
            return source.ToDateTime(DateTime.Now);
        }

        /// <summary>
        ///     将对象转成Byte类型
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static byte ToByte(this object source)
        {
            return source.ToByte(default);
        }
    }
}
