using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Xin.Core.Extensions
{
    /// <summary>
    ///     字符串扩展方法
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        ///     去除文件名中不可用于文件名的11个字符
        /// </summary>
        /// <param name="filenameNoDir"></param>
        /// <param name="replaceWith">用什么字符串替换</param>
        /// <returns></returns>
        public static string ReplaceNonValidChars(this string filenameNoDir, string replaceWith)
        {
            return string.IsNullOrEmpty(filenameNoDir) ? string.Empty : Regex.Replace(filenameNoDir, @"[\<\>\/\\\|\:""\*\?\r\n]", replaceWith, RegexOptions.Compiled);
            //替换这9个字符<>/\|:"*? 以及 回车换行
        }

        /// <summary>
        ///     去除非打印字符
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string RemoveNonPrintChars(this string source)
        {
            if (string.IsNullOrEmpty(source))
                return string.Empty;
            var reg = new Regex("[\x00-\x08\x0B\x0C\x0E-\x1F]");
            return reg.Replace(source, "");
        }

        /// <summary>
        ///     获取汉字字符串的首字母
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string GetPinYin(this string source)
        {
            return GetChineseSpell(source);
        }

        /// <summary>
        ///     取得汉字字符串的拼音的首字母
        /// </summary>
        /// <param name="strText"></param>
        /// <returns></returns>
        private static string GetChineseSpell(string strText)
        {
            if (string.IsNullOrEmpty(strText))
                return string.Empty;
            var len = strText.Length;
            var myStr = "";
            for (var i = 0; i < len; i++) myStr += getSpell(strText.Substring(i, 1));
            return myStr;
        }

        /// <summary>
        ///     取得汉字字符的拼音的首字母
        /// </summary>
        /// <param name="cnChar"></param>
        /// <returns></returns>
        private static string getSpell(string cnChar)
        {
            if (string.IsNullOrEmpty(cnChar))
                return string.Empty;
            var arrCn = Encoding.Default.GetBytes(cnChar);
            if (arrCn.Length <= 1) return cnChar;
            int area = arrCn[0];
            int pos = arrCn[1];
            var code = (area << 8) + pos;
            int[] areacode =
            {
                45217, 45253, 45761, 46318, 46826, 47010, 47297, 47614, 48119, 48119, 49062, 49324, 49896, 50371,
                50614, 50622, 50906, 51387, 51446, 52218, 52698, 52698, 52698, 52980, 53689, 54481
            };
            for (var i = 0; i < 26; i++)
            {
                var max = 55290;
                if (i != 25) max = areacode[i + 1];
                if (areacode[i] <= code && code < max) return Encoding.Default.GetString(new[] {(byte) (65 + i)});
            }

            return "*";

        }

        #region 字符串操作

        /// <summary>
        ///     获取字符串的实际长度(按单字节)
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static int GetRealLength(this string source)
        {
            return string.IsNullOrEmpty(source) ? 0 : Encoding.Default.GetByteCount(source);
        }

        /// <summary>
        ///     取得固定长度的字符串(按单字节截取)。
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <param name="resultLength">截取长度</param>
        /// <returns></returns>
        public static string SubString(this string source, int resultLength)
        {
            if (string.IsNullOrEmpty(source))
                return string.Empty;

            //判断字符串长度是否大于截断长度
            if (Encoding.Default.GetByteCount(source) <= resultLength) return source;
            //初始化
            int i = 0, j = 0;

            //为汉字或全脚符号长度加2否则加1
            foreach (var newChar in source)
            {
                if (newChar > 127)
                    i += 2;
                else
                    i++;
                if (i > resultLength)
                {
                    source = source.Substring(0, j);
                    break;
                }

                j++;
            }

            return source;
        }

        /// <summary>
        ///     取得固定长度字符的字符串，后面加上…(按单字节截取)
        /// </summary>
        /// <param name="source"></param>
        /// <param name="resultLength"></param>
        /// <returns></returns>
        public static string SubStr(this string source, int resultLength)
        {
            if (string.IsNullOrEmpty(source))
                return string.Empty;

            if (source.GetRealLength() <= resultLength)
                return source;
            return source.SubString(resultLength) + "...";
        }

        #endregion

        #region 字符串格式验证

        /// <summary>
        ///     判断字符串是否为null或为空.判断为空操作前先进行了Trim操作。
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string source)
        {
            if (source != null) return source.Trim().Length < 1;
            return true;
        }

        /// <summary>
        ///     判断字符串是否为整型
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsInteger(this string source)
        {
            if (string.IsNullOrEmpty(source)) return false;
            int i;
            return int.TryParse(source, out i);
        }

        /// <summary>
        ///     Email 格式是否合法
        /// </summary>
        /// <param name="source"></param>
        public static bool IsEmail(this string source)
        {
            return Regex.IsMatch(source, @"^\w+((-\w+)|(\.\w+))*\@[A-Za-z0-9]+((\.|-)[A-Za-z0-9]+)*\.[A-Za-z0-9]+$");
        }

        /// <summary>
        ///     判断是否公网IP
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsPublicIp(this string source)
        {
            return Regex.IsMatch(source,
                @"^(((25[0-5]|2[0-4][0-9]|19[0-1]|19[3-9]|18[0-9]|17[0-1]|17[3-9]|1[0-6][0-9]|1[1-9]|[2-9][0-9]|[0-9])\.(25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9][0-9]|[0-9]))|(192\.(25[0-5]|2[0-4][0-9]|16[0-7]|169|1[0-5][0-9]|1[7-9][0-9]|[1-9][0-9]|[0-9]))|(172\.(25[0-5]|2[0-4][0-9]|1[0-9][0-9]|1[0-5]|3[2-9]|[4-9][0-9]|[0-9])))\.(25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9][0-9]|[0-9])\.(25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9][0-9]|[0-9])$");
        }

        /// <summary>
        ///     验证IP
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsIp(this string source)
        {
            return Regex.IsMatch(source,
                @"^(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])$");
        }

        /// <summary>
        ///     检查字符串是否为A-Z、0-9及下划线以内的字符
        /// </summary>
        /// <param name="source">被检查的字符串</param>
        /// <returns>是否有特殊字符</returns>
        public static bool IsLetterOrNumber(this string source)
        {
            var b = Regex.IsMatch(source, @"\w");
            return b;
        }

        /// <summary>
        ///     验输入字符串是否含有“/\:.?*|$]”特殊字符
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsSpecialChar(this string source)
        {
            var r = new Regex(@"[/\<>:.?*|$]");
            return r.IsMatch(source);
        }

        /// <summary>
        ///     是否全为中文/日文/韩文字符
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <returns></returns>
        public static bool IsChineseChar(this string source)
        {
            //中文/日文/韩文: [\u4E00-\u9FA5]
            //英文:[a-zA-Z]
            return Regex.IsMatch(source, @"^[\u4E00-\u9FA5]+$");
        }

        /// <summary>
        ///     是否包含双字节字符(允许有单字节字符)
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsDoubleChar(this string source)
        {
            return Regex.IsMatch(source, @"[^\x00-\xff]");
        }

        /// <summary>
        ///     是否为日期型字符串
        /// </summary>
        /// <param name="source">日期字符串(2005-6-30)</param>
        /// <returns></returns>
        public static bool IsDate(this string source)
        {
            return Regex.IsMatch(source,
                @"^((((1[6-9]|[2-9]\d)\d{2})-(0?[13578]|1[02])-(0?[1-9]|[12]\d|3[01]))|(((1[6-9]|[2-9]\d)\d{2})-(0?[13456789]|1[012])-(0?[1-9]|[12]\d|30))|(((1[6-9]|[2-9]\d)\d{2})-0?2-(0?[1-9]|1\d|2[0-8]))|(((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))-0?2-29-))$");
        }


        /// <summary>
        ///     是否为时间型字符串
        /// </summary>
        /// <param name="source">时间字符串(15:00:00)</param>
        /// <returns></returns>
        public static bool IsTime(this string source)
        {
            return Regex.IsMatch(source, @"^((20|21|22|23|[0-1]?\d):[0-5]?\d:[0-5]?\d)$");
        }

        /// <summary>
        ///     是否为日期+时间型字符串
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsDateTime(this string source)
        {
            return Regex.IsMatch(source,
                @"^(((((1[6-9]|[2-9]\d)\d{2})-(0?[13578]|1[02])-(0?[1-9]|[12]\d|3[01]))|(((1[6-9]|[2-9]\d)\d{2})-(0?[13456789]|1[012])-(0?[1-9]|[12]\d|30))|(((1[6-9]|[2-9]\d)\d{2})-0?2-(0?[1-9]|1\d|2[0-8]))|(((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))-0?2-29-)) (20|21|22|23|[0-1]?\d):[0-5]?\d:[0-5]?\d)$");
        }

        /// <summary>
        ///     是否为文件物理路径
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsPhysicalPath(this string source)
        {
            return Regex.IsMatch(source,
                @"^[a-zA-Z]:[\\/]+(?:[^\<\>\/\\\|\:""\*\?\r\n]+[\\/]+)*[^\<\>\/\\\|\:""\*\?\r\n]*$");
        }

        #endregion

        #region 字符串编码

        /// <summary>
        ///     将字符串使用base64算法加密
        /// </summary>
        /// <param name="source">待加密的字符串</param>
        /// <returns>加码后的文本字符串</returns>
        public static string ToBase64(this string source)
        {
            if (string.IsNullOrEmpty(source))
                return string.Empty;
            return Convert.ToBase64String(Encoding.Default.GetBytes(source));
        }

        /// <summary>
        ///     从Base64编码的字符串中还原字符串，支持中文
        /// </summary>
        /// <param name="source">Base64加密后的字符串</param>
        /// <returns>还原后的文本字符串</returns>
        public static string FromBase64(this string source)
        {
            if (string.IsNullOrEmpty(source))
                return string.Empty;
            return Encoding.Default.GetString(Convert.FromBase64String(source));
        }

        /// <summary>
        ///     将 GB2312 值转换为 UTF8 字符串(如：测试 -> 娴嬭瘯 )
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string FromGbToUtf8(this string source)
        {
            return string.IsNullOrEmpty(source) ? string.Empty : Encoding.GetEncoding("GB2312").GetString(Encoding.UTF8.GetBytes(source));
        }

        /// <summary>
        ///     将 UTF8 值转换为 GB2312 字符串 (如：娴嬭瘯 -> 测试)
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string FromUtf8ToGb(this string source)
        {
            return string.IsNullOrEmpty(source) ? string.Empty : Encoding.UTF8.GetString(Encoding.GetEncoding("GB2312").GetBytes(source));
        }


        /// <summary>
        ///     由16进制转为汉字字符串（如：B2E2 -> 测 ）
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string FromHex(this string source)
        {
            if (string.IsNullOrEmpty(source))
                return string.Empty;
            var oribyte = new byte[source.Length / 2];
            for (var i = 0; i < source.Length; i += 2)
            {
                var str = Convert.ToInt32(source.Substring(i, 2), 16).ToString();
                oribyte[i / 2] = Convert.ToByte(source.Substring(i, 2), 16);
            }

            return Encoding.Default.GetString(oribyte);
        }

        /// <summary>
        ///     字符串转为16进制字符串（如：测 -> B2E2 ）
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ToHex(this string source)
        {
            if (string.IsNullOrEmpty(source))
                return string.Empty;
            var i = source.Length;
            string temp;
            var end = "";
            var array = new byte[2];
            int i1, i2;
            for (var j = 0; j < i; j++)
            {
                temp = source.Substring(j, 1);
                array = Encoding.Default.GetBytes(temp);
                if (array.Length.ToString() == "1")
                {
                    i1 = Convert.ToInt32(array[0]);
                    end += Convert.ToString(i1, 16);
                }
                else
                {
                    i1 = Convert.ToInt32(array[0]);
                    i2 = Convert.ToInt32(array[1]);
                    end += Convert.ToString(i1, 16);
                    end += Convert.ToString(i2, 16);
                }
            }

            return end.ToUpper();
        }

        /// <summary>
        ///     字符串转为unicode字符串（如：测试 -> &#27979;&#35797;）
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ToUnicode(this string source)
        {
            if (string.IsNullOrEmpty(source))
                return string.Empty;
            var sa = new StringBuilder(); //Unicode
            string s1;
            string s2;
            for (var i = 0; i < source.Length; i++)
            {
                var bt = Encoding.Unicode.GetBytes(source.Substring(i, 1));
                if (bt.Length > 1) //判断是否汉字
                {
                    s1 = Convert.ToString((short) (bt[1] - '\0'), 16); //转化为16进制字符串
                    s2 = Convert.ToString((short) (bt[0] - '\0'), 16); //转化为16进制字符串
                    s1 = (s1.Length == 1 ? "0" : "") + s1; //不足位补0
                    s2 = (s2.Length == 1 ? "0" : "") + s2; //不足位补0
                    sa.Append("&#" + Convert.ToInt32(s1 + s2, 16) + ";");
                }
            }

            return sa.ToString();
        }


        /// <summary>
        ///     字符串转为UTF8字符串（如：测试 -> \u6d4b\u8bd5）
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ToUtf8(this string source)
        {
            if (string.IsNullOrEmpty(source))
                return string.Empty;
            var sb = new StringBuilder(); //UTF8
            string s1;
            string s2;
            for (var i = 0; i < source.Length; i++)
            {
                var bt = Encoding.Unicode.GetBytes(source.Substring(i, 1));
                if (bt.Length > 1) //判断是否汉字
                {
                    s1 = Convert.ToString((short) (bt[1] - '\0'), 16); //转化为16进制字符串
                    s2 = Convert.ToString((short) (bt[0] - '\0'), 16); //转化为16进制字符串
                    s1 = (s1.Length == 1 ? "0" : "") + s1; //不足位补0
                    s2 = (s2.Length == 1 ? "0" : "") + s2; //不足位补0
                    sb.Append("\\u" + s1 + s2);
                }
            }

            return sb.ToString();
        }

        /// <summary>
        ///     将字符串转为安全的Sql字符串，不建议使用。尽可能使用参数化查询来避免
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ToSafeSql(this string source)
        {
            if (string.IsNullOrEmpty(source))
                return string.Empty;
            return source.Replace("'", "''");
        }

        /// <summary>
        ///     将字符串转换化安全的js字符串值（对字符串中的' "进行转义)
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ToSafeJsString(this string source)
        {
            if (string.IsNullOrEmpty(source))
                return string.Empty;
            source = source.Replace("'", "\\'");
            source = source.Replace("\"", "\\\"");
            source = source.Replace("\r\n", "").Replace("\r", "").Replace("\n", "");
            return source;
        }

        /// <summary>
        ///     注释like操作字符串中出现的特殊符号
        /// </summary>
        /// <remarks>注意：如果like查询中本身有使用到特殊字符，请不要使用此方法</remarks>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ToEscapeRegChars(this string source)
        {
            if (string.IsNullOrEmpty(source))
                return string.Empty;
            //[符号要第一个替换
            source = source.Replace("[", "[[]");

            source = source.Replace("%", "[%]");
            source = source.Replace("_", "[_]");
            source = source.Replace("^", "[^]");
            return source;
        }

        /// <summary>
        ///     将字符串包装成 &lt;![CDATA[字符串]]&gt; 形式
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string WrapWithCData(this string source)
        {
            return string.IsNullOrEmpty(source) ? string.Empty : $"<![CDATA[{source}]]>";
        }

        /// <summary>
        ///     将字符串转换化安全的XML字符串值
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ToSafeXmlString(this string source)
        {
            if (string.IsNullOrEmpty(source))
                return string.Empty;
            return source.Replace(">", "&gt;").Replace("<", "&lt;").Replace("&", "&amp;").Replace("\"", "&quot;")
                .Replace("'", "&apos;");
        }

        /// <summary>
        ///     将字母，数字由全角转化为半角
        /// </summary>
        /// <returns></returns>
        public static string NarrowToSmall(this string inputString)
        {
            var c = inputString.ToCharArray();
            for (var i = 0; i < c.Length; i++)
            {
                var b = Encoding.Unicode.GetBytes(c, i, 1);
                if (b.Length != 2) continue;
                if (b[1] != 255) continue;
                b[0] = (byte) (b[0] + 32);
                b[1] = 0;
                c[i] = Encoding.Unicode.GetChars(b)[0];
            }

            var returnString = new string(c);
            return returnString; // 返回半角字符   
        }

        /// <summary>
        ///     将字母，数字由半角转化为全角
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        public static string NarrowToBig(this string inputString)
        {
            var c = inputString.ToCharArray();
            for (var i = 0; i < c.Length; i++)
            {
                var b = Encoding.Unicode.GetBytes(c, i, 1);
                if (b.Length != 2) continue;
                if (b[1] != 0) continue;
                b[0] = (byte) (b[0] - 32);
                b[1] = 255;
                c[i] = Encoding.Unicode.GetChars(b)[0];
            }

            var returnString = new string(c);
            return returnString; // 返回全角字符   
        }

        #endregion

        #region 类型转换

        /// <summary>
        ///     将字符串转成Int32类型，如果转换失败，则返回-1
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <returns></returns>
        public static int ToInt32(this string source)
        {
            return source.ToInt32(-1);
        }

        /// <summary>
        ///     将字符串转成Int32类型
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <param name="defaultValue">如果转换失败，返回的数值</param>
        /// <returns></returns>
        public static int ToInt32(this string source, int defaultValue)
        {
            if (string.IsNullOrEmpty(source)) return defaultValue;
            return int.TryParse(source, out var result) ? result : defaultValue;
        }

        /// <summary>
        ///     将字符串转成Int64类型，如果转换失败，则返回-1
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <returns></returns>
        public static long ToInt64(this string source)
        {
            return source.ToInt64(-1);
        }

        /// <summary>
        ///     将字符串转成Int64类型
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <param name="defaultValue">如果转换失败，返回的数值</param>
        /// <returns></returns>
        public static long ToInt64(this string source, long defaultValue)
        {
            if (string.IsNullOrEmpty(source)) return defaultValue;
            return long.TryParse(source, out var result) ? result : defaultValue;
        }

        /// <summary>
        ///     将字符串转成double类型，如果转换失败，则返回-1
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <returns></returns>
        public static double ToDouble(this string source)
        {
            return source.ToDouble(-1.0);
        }

        /// <summary>
        ///     将字符串转成double类型
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <param name="defaultValue">如果转换失败，返回的数值</param>
        /// <returns></returns>
        public static double ToDouble(this string source, double defaultValue)
        {
            if (string.IsNullOrEmpty(source)) return defaultValue;
            return double.TryParse(source, out var result) ? result : defaultValue;
        }

        /// <summary>
        ///     将字符串转成DateTime类型，如果转换失败，则返回当前时间
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <returns></returns>
        public static DateTime ToDateTime(this string source)
        {
            return source.ToDateTime(DateTime.Now);
        }

        /// <summary>
        ///     将字符串转成DateTime类型
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <param name="defaultValue">如果转换失败，返回的默认时间</param>
        /// <returns></returns>
        public static DateTime ToDateTime(this string source, DateTime defaultValue)
        {
            if (string.IsNullOrEmpty(source)) return defaultValue;
            return DateTime.TryParse(source, out var result) ? result : defaultValue;
        }

        /// <summary>
        ///     将字符串转成Boolean类型，如果转换失败，则返回false
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <returns></returns>
        public static bool ToBoolean(this string source)
        {
            if (string.IsNullOrEmpty(source)) return false;
            return bool.TryParse(source, out var result) && result;
        }

        /// <summary>
        ///     将字符串转成指定的枚举类型(字符串可以是枚举的名称也可以是枚举值)
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <param name="source">源字符串</param>
        /// <param name="defaultValue">如果转换失败，返回默认的枚举项</param>
        /// <returns></returns>
        public static T ToEnum<T>(this string source, T defaultValue)
        {
            if (string.IsNullOrEmpty(source)) return defaultValue;
            try
            {
                var value = (T) Enum.Parse(typeof(T), source, true);
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
        public static T ToEnumExt<T>(this string source, T defaultValue)
        {
            if (string.IsNullOrEmpty(source)) return defaultValue;
            try
            {
                return (T) Enum.Parse(typeof(T), source, true);
            }
            catch
            {
                // ignored
            }

            return defaultValue;
        }

        #endregion
    }
}
