namespace Xin.AspNetCore.MiddleWares.CustomCodeHandle
{
    /// <summary>
    /// 处理方式
    /// </summary>
    public enum CustomHandleType
    {
        /// <summary>
        /// 
        /// </summary>
        HttpCodeHandle = 0,

        /// <summary>
        /// 
        /// </summary>
        PageHandle = 1, //跳转网页处理

        /// <summary>
        /// 
        /// </summary>
        Both = 2 //根据Url关键字自动处理
    }
}
