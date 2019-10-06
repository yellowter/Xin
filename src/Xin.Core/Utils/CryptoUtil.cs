using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Xin.Core.Utils
{
    /// <summary>
    ///     加密
    /// </summary>
    public class CryptoUtil
    {
        /// <summary>
        ///     3DES加解密的默认密钥, 前8位作为向量
        /// </summary>
        private const string KeyComplement = "Z!E@R#O$Z%H^E&N*G(L)I_N+G{J}U|N?";

        #region 密码加密

        /// <summary>
        ///     返回使用MD5加密后字符串
        /// </summary>
        /// <param name="strpwd">待加密字符串</param>
        /// <returns>加密后字符串</returns>
        public static string RegUser_MD5_Pwd(string strpwd)
        {
            #region

            var appkey = KeyComplement; //，。加一特殊的字符后再加密，这样更安全些
            //strpwd += appkey;

            MD5 MD5 = new MD5CryptoServiceProvider();
            var a = Encoding.Default.GetBytes(appkey);
            var datSource = Encoding.Default.GetBytes(strpwd);
            var b = new byte[a.Length + 4 + datSource.Length];

            int i;
            for (i = 0; i < datSource.Length; i++) b[i] = datSource[i];

            b[i++] = 163;
            b[i++] = 172;
            b[i++] = 161;
            b[i++] = 163;

            foreach (var t in a)
            {
                b[i] = t;
                i++;
            }

            var newSource = MD5.ComputeHash(b);
            string byte2String = null;
            for (i = 0; i < newSource.Length; i++)
            {
                var thisByte = newSource[i].ToString("x");
                if (thisByte.Length == 1) thisByte = "0" + thisByte;

                byte2String += thisByte;
            }

            return byte2String;

            #endregion
        }

        #endregion


        #region SHA1加密

        /// <summary>
        ///     SHA1加密，等效于 PHP 的 SHA1() 代码
        /// </summary>
        /// <param name="source">被加密的字符串</param>
        /// <returns>加密后的字符串</returns>
        public static string SHA1_Encrypt(string source)
        {
            var temp1 = Encoding.UTF8.GetBytes(source);

            var sha = new SHA1CryptoServiceProvider();
            var temp2 = sha.ComputeHash(temp1);
            sha.Clear();

            //注意，不能用这个
            //string output = Convert.ToBase64String(temp2); 

            var output = BitConverter.ToString(temp2);
            output = output.Replace("-", "");
            output = output.ToLower();
            return output;
        }

        #endregion

        #region 通过HTTP传递的Base64编码

        /// <summary>
        ///     编码 通过HTTP传递的Base64编码
        /// </summary>
        /// <param name="source">编码前的</param>
        /// <returns>编码后的</returns>
        public static string HttpBase64Encode(string source)
        {
            //空串处理
            if (string.IsNullOrEmpty(source)) return "";

            //编码
            var encodeString = Convert.ToBase64String(Encoding.UTF8.GetBytes(source));

            //过滤
            encodeString = encodeString.Replace("+", "~");
            encodeString = encodeString.Replace("/", "@");
            encodeString = encodeString.Replace("=", "$");

            //返回
            return encodeString;
        }

        #endregion

        #region 通过HTTP传递的Base64解码

        /// <summary>
        ///     解码 通过HTTP传递的Base64解码
        /// </summary>
        /// <param name="source">解码前的</param>
        /// <returns>解码后的</returns>
        public static string HttpBase64Decode(string source)
        {
            //空串处理
            if (string.IsNullOrEmpty(source)) return "";

            //还原
            var deocdeString = source;
            deocdeString = deocdeString.Replace("~", "+");
            deocdeString = deocdeString.Replace("@", "/");
            deocdeString = deocdeString.Replace("$", "=");

            //Base64解码
            deocdeString = Encoding.UTF8.GetString(Convert.FromBase64String(deocdeString));

            //返回
            return deocdeString;
        }

        #endregion

        /// <summary>
        ///     计算文件的MD5值并返回
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetMD5HashFromFile(string fileName)
        {
            using (var file = new FileStream(fileName, FileMode.Open))
            {
                MD5 md5 = new MD5CryptoServiceProvider();
                var retVal = md5.ComputeHash(file);
                file.Close();
                return BitConverter.ToString(retVal).Replace("-", "");
            }
        }

        /// <summary>
        ///     AES加密（加密步骤）
        ///     1，加密字符串得到2进制数组；
        ///     2，将2禁止数组转为16进制；
        ///     3，进行base64编码
        /// </summary>
        /// <param name="toEncrypt">要加密的字符串</param>
        /// <param name="key">密钥</param>
        public static string AES_Encrypt(string toEncrypt, string key)
        {
            var _Key = Encoding.ASCII.GetBytes(BuildKey(key, 32));
            var _Source = Encoding.UTF8.GetBytes(toEncrypt);

            var aes = Aes.Create("AES");
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = _Key;
            var cTransform = aes.CreateEncryptor();
            var cryptData = cTransform.TransformFinalBlock(_Source, 0, _Source.Length);
            var HexCryptString = Hex_2To16(cryptData);
            var HexCryptData = Encoding.UTF8.GetBytes(HexCryptString);
            var CryptString = Convert.ToBase64String(HexCryptData);
            return CryptString;
        }

        /// <summary>
        ///     AES解密（解密步骤）
        ///     1，将BASE64字符串转为16进制数组
        ///     2，将16进制数组转为字符串
        ///     3，将字符串转为2进制数据
        ///     4，用AES解密数据
        /// </summary>
        /// <param name="encryptedSource">已加密的内容</param>
        /// <param name="key">密钥</param>
        public static string AES_Decrypt(string encryptedSource, string key)
        {
            var _Key = Encoding.ASCII.GetBytes(BuildKey(key, 32));
            var aes = Aes.Create("AES");
            aes.Mode = CipherMode.ECB;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = _Key;
            var cTransform = aes.CreateDecryptor();

            var encryptedData = Convert.FromBase64String(encryptedSource);
            var encryptedString = Encoding.UTF8.GetString(encryptedData);
            var _Source = Hex_16To2(encryptedString);
            var originalSrouceData = cTransform.TransformFinalBlock(_Source, 0, _Source.Length);
            var originalString = Encoding.UTF8.GetString(originalSrouceData);
            return originalString;
        }

        private static string BuildKey(string key, int length = 8)
        {
            return ((key ?? string.Empty) + KeyComplement).Substring(0, length);
        }

        private static string Hex_2To16(byte[] bytes)
        {
            var hexString = string.Empty;
            var iLength = 65535;
            if (bytes != null)
            {
                var strB = new StringBuilder();

                if (bytes.Length < iLength) iLength = bytes.Length;

                for (var i = 0; i < iLength; i++) strB.Append(bytes[i].ToString("X2"));
                hexString = strB.ToString();
            }

            return hexString;
        }

        private static byte[] Hex_16To2(string hexString)
        {
            if (hexString.Length % 2 != 0) hexString += " ";
            var returnBytes = new byte[hexString.Length / 2];
            for (var i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }


        #region 使用Get传输替换关键字符为全角和半角转换

        /// <summary>
        ///     使用Get传输替换关键字符为全角
        /// </summary>
        /// <param name="UrlParam"></param>
        /// <returns></returns>
        public static string UrlParamUrlEncodeRun(string UrlParam)
        {
            UrlParam = UrlParam.Replace("+", "＋");
            UrlParam = UrlParam.Replace("=", "＝");
            UrlParam = UrlParam.Replace("&", "＆");
            UrlParam = UrlParam.Replace("?", "？");
            return UrlParam;
        }

        /// <summary>
        ///     使用Get传输替换关键字符为半角
        /// </summary>
        /// <param name="UrlParam"></param>
        /// <returns></returns>
        public static string UrlParamUrlDecodeRun(string UrlParam)
        {
            UrlParam = UrlParam.Replace("＋", "+");
            UrlParam = UrlParam.Replace("＝", "=");
            UrlParam = UrlParam.Replace("＆", "&");
            UrlParam = UrlParam.Replace("？", "?");
            return UrlParam;
        }

        #endregion

        #region  MD5加密

        /// <summary>
        ///     标准MD5加密
        /// </summary>
        /// <param name="source">待加密字符串</param>
        /// <param name="addKey">附加字符串</param>
        /// <param name="encoding">编码方式</param>
        /// <returns></returns>
        public static string MD5_Encrypt(string source, string addKey, Encoding encoding)
        {
            if (addKey.Length > 0) source = source + addKey;
            return MD5_Encrypt(encoding.GetBytes(source));
        }


        /// <summary>
        ///     标准md5加密
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string MD5_Encrypt(byte[] source)
        {
            MD5 MD5 = new MD5CryptoServiceProvider();
            var newSource = MD5.ComputeHash(source);
            string byte2String = null;
            for (var i = 0; i < newSource.Length; i++)
            {
                var thisByte = newSource[i].ToString("x");
                if (thisByte.Length == 1) thisByte = "0" + thisByte;

                byte2String += thisByte;
            }

            return byte2String;
        }

        /// <summary>
        ///     标准MD5加密
        /// </summary>
        /// <param name="source">待加密字符串</param>
        /// <param name="encoding">编码方式</param>
        /// <returns></returns>
        public static string MD5_Encrypt(string source, Encoding encoding)
        {
            return MD5_Encrypt(source, string.Empty, encoding);
        }

        /// <summary>
        ///     标准MD5加密
        /// </summary>
        /// <param name="source">被加密的字符串</param>
        /// <returns></returns>
        public static string MD5_Encrypt(string source)
        {
            return MD5_Encrypt(source, string.Empty, Encoding.UTF8);
        }

        #endregion

        #region  DES 加解密

        /// <summary>
        ///     Desc加密 默认使用Mode=CBC,Padding=PKCS7,Encoding=UTF8
        /// </summary>
        /// <param name="source">待加密字符</param>
        /// <param name="key">密钥</param>
        /// <returns>string</returns>
        public static string DES_Encrypt(string source, string key)
        {
            key = BuildKey(key);
            var btKey = Encoding.UTF8.GetBytes(key);
            var btIV = Encoding.UTF8.GetBytes(key);
            using (var des = new DESCryptoServiceProvider())
            {
                des.Mode = CipherMode.CBC;
                des.Padding = PaddingMode.PKCS7;
                using (var ms = new MemoryStream())
                {
                    var inData = Encoding.UTF8.GetBytes(source);
                    using (var cs = new CryptoStream(ms, des.CreateEncryptor(btKey, btIV), CryptoStreamMode.Write))
                    {
                        cs.Write(inData, 0, inData.Length);
                        cs.FlushFinalBlock();
                    }

                    var ret = new StringBuilder();
                    foreach (var b in ms.ToArray()) ret.AppendFormat("{0:X2}", b);

                    return ret.ToString();
                }
            }
        }

        /// <summary>
        ///     使用默认key 做 DES加密 默认使用Mode=CBC,Padding=PKCS7,Encoding=UTF8
        /// </summary>
        /// <param name="source">明文</param>
        /// <returns>密文</returns>
        public static string DES_Encrypt(string source)
        {
            return DES_Encrypt(source, KeyComplement);
        }

        /// <summary>
        ///     使用默认key 做 DES解密 默认使用Mode=CBC,Padding=PKCS7,Encoding=UTF8
        /// </summary>
        /// <param name="source">密文</param>
        /// <returns>明文</returns>
        public static string DES_Decrypt(string source)
        {
            return DES_Decrypt(source, KeyComplement);
        }

        /// <summary>
        ///     DES解密 默认使用Mode=CBC,Padding=PKCS7,Encoding=UTF8
        /// </summary>
        /// <param name="source">密文</param>
        /// <param name="key">密钥</param>
        /// <returns>明文</returns>
        public static string DES_Decrypt(string source, string key)
        {
            //将字符串转为字节数组  
            var inputByteArray = new byte[source.Length / 2];
            for (var x = 0; x < source.Length / 2; x++)
            {
                var i = Convert.ToInt32(source.Substring(x * 2, 2), 16);
                inputByteArray[x] = (byte) i;
            }

            key = BuildKey(key);
            using (var des = new DESCryptoServiceProvider())
            {
                des.Mode = CipherMode.CBC;
                des.Padding = PaddingMode.PKCS7;
                des.Key = Encoding.UTF8.GetBytes(key);
                des.IV = Encoding.UTF8.GetBytes(key);
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(inputByteArray, 0, inputByteArray.Length);
                        cs.FlushFinalBlock();
                        cs.Close();
                    }

                    return Encoding.UTF8.GetString(ms.ToArray());
                }
            }
        }

        #endregion


        #region 3DES加解密

        /// <summary>
        ///     使用指定的key和iv，加密input数据
        /// </summary>
        /// <param name="input"></param>
        /// <param name="key">密钥，必须为24位长度</param>
        /// <param name="iv">微量，必须为8位长度</param>
        /// <returns></returns>
        public static string TripleDES_Encrypt(string input, string key = null, string iv = null)
        {
            key = BuildKey(key, 24);

            iv = BuildKey(iv);

            var arrKey = Encoding.UTF8.GetBytes(key);
            var arrIV = Encoding.UTF8.GetBytes(iv);

            // 获取加密后的字节数据
            var arrData = Encoding.UTF8.GetBytes(input);
            var result = TripleDesEncrypt(arrKey, arrIV, arrData);

            // 转换为16进制字符串
            var ret = new StringBuilder();
            foreach (var b in result) ret.AppendFormat("{0:X2}", b);
            return ret.ToString();
        }

        /// <summary>
        ///     使用指定的key和iv，解密input数据
        /// </summary>
        /// <param name="input"></param>
        /// <param name="key">密钥，必须为24位长度</param>
        /// <param name="iv">微量，必须为8位长度</param>
        /// <returns></returns>
        public static string TripleDES_Decrypt(string input, string key = null, string iv = null)
        {
            key = BuildKey(key, 24);

            iv = BuildKey(iv);

            var arrKey = Encoding.UTF8.GetBytes(key);
            var arrIV = Encoding.UTF8.GetBytes(iv);

            // 获取加密后的字节数据
            var len = input.Length / 2;
            var arrData = new byte[len];
            for (var x = 0; x < len; x++)
            {
                var i = Convert.ToInt32(input.Substring(x * 2, 2), 16);
                arrData[x] = (byte) i;
            }

            var result = TripleDesDecrypt(arrKey, arrIV, arrData);
            return Encoding.UTF8.GetString(result);
        }


        #region TripleDesEncrypt加密(3DES加密)

        /// <summary>
        ///     3Des加密，密钥长度必需是24字节
        /// </summary>
        /// <param name="key">密钥字节数组</param>
        /// <param name="iv">向量字节数组</param>
        /// <param name="source">源字节数组</param>
        /// <returns>加密后的字节数组</returns>
        private static byte[] TripleDesEncrypt(byte[] key, byte[] iv, byte[] source)
        {
            var dsp = new TripleDESCryptoServiceProvider
            {
                Mode = CipherMode.CBC, // 默认值
                Padding = PaddingMode.PKCS7 // 默认值
            };
            using (var mStream = new MemoryStream())
            using (var cStream = new CryptoStream(mStream, dsp.CreateEncryptor(key, iv), CryptoStreamMode.Write))
            {
                cStream.Write(source, 0, source.Length);
                cStream.FlushFinalBlock();
                var result = mStream.ToArray();
                cStream.Close();
                mStream.Close();
                return result;
            }
        }

        #endregion

        #region TripleDesDecrypt解密(3DES解密)

        /// <summary>
        ///     3Des解密，密钥长度必需是24字节
        /// </summary>
        /// <param name="key">密钥字节数组</param>
        /// <param name="iv">向量字节数组</param>
        /// <param name="source">加密后的字节数组</param>
        /// <param name="dataLen">解密后的数据长度</param>
        /// <returns>解密后的字节数组</returns>
        private static byte[] TripleDesDecrypt(byte[] key, byte[] iv, byte[] source, out int dataLen)
        {
            var dsp = new TripleDESCryptoServiceProvider
            {
                Mode = CipherMode.CBC, // 默认值
                Padding = PaddingMode.PKCS7 // 默认值
            };
            using (var mStream = new MemoryStream(source))
            using (var cStream = new CryptoStream(mStream, dsp.CreateDecryptor(key, iv), CryptoStreamMode.Read))
            {
                var result = new byte[source.Length];
                dataLen = cStream.Read(result, 0, result.Length);
                cStream.Close();
                mStream.Close();
                return result;
            }
        }

        /// <summary>
        ///     3Des解密，密钥长度必需是24字节
        /// </summary>
        /// <param name="key">密钥字节数组</param>
        /// <param name="iv">向量字节数组</param>
        /// <param name="source">加密后的字节数组</param>
        /// <returns>解密后的字节数组</returns>
        private static byte[] TripleDesDecrypt(byte[] key, byte[] iv, byte[] source)
        {
            var result = TripleDesDecrypt(key, iv, source, out var dataLen);

            if (result.Length != dataLen)
            {
                // 如果数组长度不是解密后的实际长度，需要截断多余的数据，用来解决Gzip的"Magic byte doesn't match"的问题
                var resultToReturn = new byte[dataLen];
                Array.Copy(result, resultToReturn, dataLen);
                return resultToReturn;
            }

            return result;
        }

        #endregion

        #endregion
    }
}
