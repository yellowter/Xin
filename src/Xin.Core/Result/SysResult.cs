namespace Xin.Core.Result
{
    /// <summary>
    ///     返回结果集
    /// </summary>
    public class SysResult
    {
        /// <summary>
        ///     错误码
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        ///     附加消息
        /// </summary>
        public string Msg { get; set; }


        /// <summary>
        ///     是否成功
        /// </summary>

        public bool Ok => Code == 0;
    }


    /// <summary>
    ///     返回结果集
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SysResult<T> : SysResult
    {
        /// <summary>
        ///     返回结果
        /// </summary>
        public T Data { get; set; }
    }
}
