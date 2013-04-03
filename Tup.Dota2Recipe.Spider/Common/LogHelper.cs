using System;
using System.Diagnostics;

namespace Tup.Dota2Recipe.Spider.Common
{
    /// <summary>
    /// 简单控制台调试日志
    /// </summary>
    internal static class LogHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        [Conditional("DEBUG")]
        public static void LogDebug(string msg)
        {
            Debug.WriteLine("[BTC]\t{0}\t{1}", DateTime.Now, msg);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="para"></param>
        [Conditional("DEBUG")]
        public static void LogDebug(string format, params object[] para)
        {
            LogDebug(string.Format(format, para));
        }

        #region LogError
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        public static void LogError(string msg)
        {
            Debug.WriteLine("[BTC]\t{0}\t{1}", DateTime.Now, msg);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="para"></param>
        public static void LogError(string format, params object[] para)
        {
            LogDebug(string.Format(format, para));
        } 
        #endregion
    }
}
