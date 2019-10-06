namespace Xin.Core.Utils
{
    /// <summary>
    /// </summary>
    public class KeyValueUtil
    {
        /// <summary>
        ///     组合字符串格式（例：1|共享,2|免费,3|试用）
        /// </summary>
        /// <param name="valueKeys"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetValue(string valueKeys, string key)
        {
            if (string.IsNullOrEmpty(valueKeys) || string.IsNullOrEmpty(key)) return valueKeys;

            var arrItems = valueKeys.Split(',');
            foreach (var item in arrItems)
            {
                var arrKeyVals = item.Split('|');
                if (arrKeyVals.Length == 2 && arrKeyVals[1].ToLower().Equals(key.ToLower())) return arrKeyVals[0];
            }

            return "";
        }

        /// <summary>
        ///     组合字符串格式（例：1|共享,2|免费,3|试用）
        /// </summary>
        /// <param name="valueKeys"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public string GetKey(string valueKeys, string value)
        {
            if (string.IsNullOrEmpty(valueKeys) || string.IsNullOrEmpty(value)) return valueKeys;

            var arrItems = valueKeys.Split(',');
            foreach (var item in arrItems)
            {
                var arrKeyVals = item.Split('|');
                if (arrKeyVals.Length == 2 && arrKeyVals[0].ToLower().Equals(value.ToLower())) return arrKeyVals[1];
            }

            return "";
        }
    }
}
