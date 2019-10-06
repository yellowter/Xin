using System;
using System.Text;
using Xin.Core.Extensions;

namespace Xin.Core.Utils
{
    /// <summary>
    ///     网络授权(只支持所有属性为字符串类型的类)
    /// </summary>
    public class TokenUtil
    {
        /// <summary>
        ///     序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="encryKey"></param>
        /// <returns></returns>
        public static string Serialize<T>(T model, string encryKey = null) where T : new()
        {
            var type = typeof(T);

            var sb = new StringBuilder();

            var properties = type.GetProperties();

            for (var i = 0; i < properties.Length;)
            {
                var p = properties[i];
                sb.Append(model.GetType().GetProperty(p.Name)?.GetValue(model, null));
                if (++i < properties.Length) sb.Append("&");
            }

            if (string.IsNullOrEmpty(encryKey))
                return CryptoUtil.DES_Encrypt(sb.ToString());
            return CryptoUtil.DES_Encrypt(sb.ToString(), encryKey);
        }

        /// <summary>
        ///     反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="token"></param>
        /// <param name="encryKey"></param>
        /// <returns></returns>
        public static T Deserialize<T>(string token, string encryKey = null) where T : new()
        {
            if (string.IsNullOrEmpty(token))
                return default;

            var sessionText = string.IsNullOrEmpty(encryKey)
                ? CryptoUtil.DES_Decrypt(token)
                : CryptoUtil.DES_Decrypt(token, encryKey);

            var tokens = sessionText.Split('&');

            var type = typeof(T);

            var ps = type.GetProperties();

            if (tokens.Length != ps.Length)
                return default;

            var model = new T();

            for (var i = 0; i < ps.Length; i++)
            {
                var t = ps[i].PropertyType;
                var value = tokens[i];
                ps[i].SetValue(model, SwitchPropertyValue(t, value), null);
            }

            return model;
        }

        private static object SwitchPropertyValue(Type type, string value)
        {
            if (type.Name.Equals("Int16", StringComparison.OrdinalIgnoreCase)) return Convert.ToInt16(value);

            if (type.Name.Equals("Int32", StringComparison.OrdinalIgnoreCase))
                return ObjectExtensions.ToInt32(value, 0);

            if (type.Name.Equals("Int64", StringComparison.OrdinalIgnoreCase))
                return ObjectExtensions.ToInt64(value, 0);

            if (type.Name.Equals("Boolean", StringComparison.OrdinalIgnoreCase)) return value.ToBoolean();

            if (type.Name.Equals("DateTime", StringComparison.OrdinalIgnoreCase))
                return ObjectExtensions.ToDateTime(value, DateTime.MinValue);

            if (type.Name.Equals("Guid", StringComparison.OrdinalIgnoreCase)) return Guid.Parse(value);

            return value;
        }
    }
}
