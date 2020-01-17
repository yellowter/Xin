using System;

namespace Xin.Core.Extensions
{
    /// <summary>
    ///     
    /// </summary>
    public static class DecimalExtensions
    {
        public static string Format(this decimal source, int decimals = 2)
        {
            return source.ToString($"F{decimals}");
        }
    }
}
