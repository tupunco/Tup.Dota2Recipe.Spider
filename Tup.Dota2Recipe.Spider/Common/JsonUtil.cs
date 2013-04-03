using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Tup.Dota2Recipe.Spider.Common
{
    /// <summary>
    /// JSON 序列化/反序列化 工具类
    /// </summary>
    internal static class JsonUtil
    {
        #region 反序列化
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T">结果类型</typeparam>
        /// <param name="json">待解析的JSON字符串</param>
        /// <returns></returns>
        public static T Deserialize<T>(string json)
        {
            ThrowHelper.ThrowIfNull(json, "json");

            return JsonConvert.DeserializeObject<T>(json);
        }
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T">结果类型</typeparam>
        /// <param name="json">待解析的JSON字符串</param>
        /// <returns></returns>
        public static T Deserialize<T>(TextReader jsonReader)
        {
            ThrowHelper.ThrowIfNull(jsonReader, "jsonReader");

            return Deserialize<T>(jsonReader.ReadToEnd());
        }
        #endregion

        #region 序列化
        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string Serialize(object obj)
        {
            ThrowHelper.ThrowIfNull(obj, "obj");

            return JsonConvert.SerializeObject(obj);
        }
        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="outFileName"></param>
        /// <returns></returns>
        public static void SerializeToFile(object obj, string outFileName)
        {
            ThrowHelper.ThrowIfNull(obj, "obj");
            ThrowHelper.ThrowIfNull(outFileName, "outFileName");

            File.WriteAllText(outFileName, JsonConvert.SerializeObject(obj), Encoding.UTF8);
        }
        #endregion
    }
}
