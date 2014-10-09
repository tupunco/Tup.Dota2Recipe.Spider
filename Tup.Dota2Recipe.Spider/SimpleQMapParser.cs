using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Text;

namespace Tup.Dota2Recipe.Spider
{
    /// <summary>
    /// SimpleQMapParser
    /// </summary>
    /// <remarks>
    /// Comment: # // [$...]
    /// TODO: BRUSHES INFO
    /// REFER:
    ///     http://www.gamers.org/dEngine/quake/QDP/qmapspec.html
    ///     https://github.com/facebook-csharp-sdk/simple-json/blob/master/src/SimpleJson/SimpleJson.cs
    ///     https://github.com/id-Software/Quake-III-Arena/blob/master/common/scriplib.c
    ///     https://github.com/id-Software/Quake-III-Arena/blob/master/q3map/map.c
    /// </remarks>
    public static class SimpleQMapParser
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="qmapScript"></param>
        /// <returns></returns>
        public static QMapPair Parse(string qmapScript)
        {
            QMapPair obj;
            if (TryParse(qmapScript, out obj))
                return obj;
            throw new SerializationException("Invalid QMap string");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="qmapScript"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool TryParse(string qmapScript, out QMapPair obj)
        {
            bool success = true;
            if (qmapScript != null)
                obj = (new QMapParser(qmapScript)).ParsePair(ref success);
            else
                obj = null;

            return success;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="qmapValue"></param>
        /// <returns></returns>
        public static QMapObjectValue ToObjectValue(this QMapValue qmapValue)
        {
            if (qmapValue == null || !(qmapValue is QMapObjectValue))
                return null;

            return qmapValue as QMapObjectValue;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="qmapObjectValue"></param>
        /// <param name="key"></param>
        public static QMapValue Find(this QMapObjectValue qmapObjectValue, string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key");

            if (qmapObjectValue == null || qmapObjectValue.Value == null || qmapObjectValue.Value.Count <= 0)
                return null;

            QMapPair pair = null;
            if (qmapObjectValue.Value.TryGetValue(key, out pair) && pair != null)
                return pair.Value;
            else
                return null;
        }
    }
    /// <summary>
    /// QMapParser
    /// </summary>
    public class QMapParser
    {
        /// <summary>
        /// 
        /// </summary>
        private enum QMapToken
        {
            None = 0,
            /// <summary>
            /// { symbol
            /// </summary>  
            CurlyOpen,
            /// <summary>
            /// } symbol
            /// </summary>
            CurlyClose,
            /// <summary>
            /// " string
            /// </summary>
            String,
            /// <summary>
            /// [$ symbol(Comment)
            /// </summary>
            SquaredComment,
            /// <summary>
            /// / symbol(Comment)
            /// </summary>
            VirguleComment,
            /// <summary>
            /// # symbol(Comment)
            /// </summary>
            SharpComment
        }
        /// <summary>
        /// 
        /// </summary>
        private char[] scriptBuffer;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="qmapScript"></param>
        public QMapParser(string qmapScript)
        {
            if (qmapScript == null)
                throw new System.ArgumentNullException("qmapScript");

            scriptBuffer = qmapScript.ToCharArray();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="success"></param>
        /// <returns></returns>
        public QMapPair ParsePair(ref bool success)
        {
            int index = 0;
            return ParsePair(ref index, ref success);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="success"></param>
        /// <returns></returns>
        private QMapPair ParsePair(ref int index, ref bool success)
        {
            EatLineWhitespace(ref index);

            var t = QMapToken.None;
            while ((t = LookAhead(true, index)) != QMapToken.None)
            {
                if (t == QMapToken.String)
                {
                    EatLineWhitespace(ref index);

                    string name = ParseString(ref index, ref success);
                    if (!success)
                    {
                        success = false;
                        return null;
                    }

                    var token = LookAhead(index);

                    if (token == QMapToken.SharpComment
                            || token == QMapToken.VirguleComment)
                    {
                        var comment = ParseAllComment(ref index, ref success);

                        if (!success)
                        {
                            success = false;
                            return null;
                        }
                        Debug.WriteLine("T:{0} #{1}", index, comment);
                        //goto LABE_TOKEN_CURLY_OPEN;
                    }
                    else if (token == QMapToken.String)
                    {
                        var value = ParseString(ref index, ref success);
                        if (!success)
                        {
                            success = false;
                            return null;
                        }

                        return new QMapPair()
                        {
                            Key = name,
                            Value = new QMapValue() { Value = new List<string>(1) { value } }
                        };
                    }
                    else
                    {
                        if (token == QMapToken.None)
                            token = LookAhead(true, index);

                        if (token != QMapToken.CurlyOpen)
                            throw new SerializationException("Invalid Pair");
                    }

                    //LABE_TOKEN_CURLY_OPEN:
                    {
                        var value = ParseObject(ref index, ref success);
                        if (!success || value == null)
                        {
                            success = false;
                            return null;
                        }

                        return new QMapPair() { Key = name, Value = value };
                    }
                }
                else if (t == QMapToken.SquaredComment
                            || t == QMapToken.VirguleComment
                            || t == QMapToken.SharpComment)
                {
                    var comment = ParseAllComment(ref index, ref success);
                    if (!success)
                    {
                        success = false;
                        return null;
                    }
                    Debug.WriteLine("T:{0} #{1}", index, comment);
                }
                else
                    throw new SerializationException("Invalid Pair-!SHARP");
            }
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="success"></param>
        /// <returns></returns>
        private QMapObjectValue ParseObject(ref int index, ref bool success)
        {
            var token = QMapToken.None;
            var outValue = new Dictionary<string, QMapPair>(StringComparer.OrdinalIgnoreCase);
            EatLineWhitespace(ref index);
            // {
            NextToken(ref index);
            while (true)
            {
                token = LookAhead(true, index);
                if (token == QMapToken.None)
                {
                    success = false;
                    return null;
                }
                else if (token == QMapToken.CurlyClose) //}
                {
                    NextToken(true, ref index);
                    return outValue.Count > 0 ? new QMapObjectValue() { Value = outValue } : null;
                }
                else
                {
                    // pair
                    var pair = ParsePair(ref index, ref success);
                    if (!success || pair == null)
                    {
                        success = false;
                        return null;
                    }
                    TryAddToObjectValue(outValue, pair);
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectValue"></param>
        /// <param name="pair"></param>
        private static void TryAddToObjectValue(Dictionary<string, QMapPair> objectValue, QMapPair pair)
        {
            if (objectValue == null || pair == null)
                throw new System.ArgumentNullException("objectValue/pair");

            var key = pair.Key;
            QMapPair oPair = null;
            if (objectValue.TryGetValue(key, out oPair))
                oPair.Value.Value.AddRange(pair.Value.Value);
            else
                objectValue.Add(key, pair);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="success"></param>
        /// <returns></returns>
        private string ParseAllComment(ref int index, ref bool success)
        {
            EatLineWhitespace(ref index);

            var s = new StringBuilder();
            var comment = string.Empty;

            var token = LookAhead(true, index);
            while (token == QMapToken.SharpComment // #
                || token == QMapToken.VirguleComment // //
                || token == QMapToken.SquaredComment) //[$...]
            {
                comment = ParseComment(token, ref index, ref success);
                if (!success)
                {
                    success = false;
                    break;
                }

                if (s.Length > 0)
                    s.AppendLine();
                s.Append(comment);

                token = LookAhead(true, index);
            }
            return s.ToString();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="commentType"></param>
        /// <param name="index"></param>
        /// <param name="success"></param>
        /// <returns></returns>
        private string ParseComment(QMapToken commentType, ref int index, ref bool success)
        {
            StringBuilder s = new StringBuilder();
            char c;

            EatWhitespace(ref index);

            if (commentType == QMapToken.VirguleComment // //
                    || commentType == QMapToken.SquaredComment) //[$...]
                index += 2;
            else if (commentType == QMapToken.SharpComment)//#
                c = scriptBuffer[index++];
            else
                throw new System.NotSupportedException("CommentType-" + commentType);

            var complete = false;
            var endFlag = false;
            while (!complete)
            {
                if (index == scriptBuffer.Length)
                    break;

                c = scriptBuffer[index++];

                if (commentType == QMapToken.SquaredComment)
                    endFlag = c == ']';
                else
                {
                    endFlag = c == '\r' || c == '\n';
                    if (endFlag)
                        index--;
                }

                if (endFlag)
                {
                    complete = true;
                    break;
                }
                else
                    s.Append(c);
            }
            if (!complete)
            {
                success = false;
                return null;
            }

            if (commentType == QMapToken.SquaredComment) //[$...]
                EatWhitespace(ref index);
            else
                EatLineWhitespace(ref index);

            return s.ToString();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="success"></param>
        /// <returns></returns>
        private string ParseString(ref int index, ref bool success)
        {
            StringBuilder s = new StringBuilder();
            char c;

            EatWhitespace(ref index);

            // "
            c = scriptBuffer[index++];
            bool complete = false;
            while (!complete)
            {
                if (index == scriptBuffer.Length)
                    break;

                c = scriptBuffer[index++];
                if (c == '"')
                {
                    complete = true;
                    break;
                }
                else if (c == '\\')
                {
                    if (index == scriptBuffer.Length)
                        break;

                    c = scriptBuffer[index++];
                    if (c == '"')
                        s.Append('"');
                    else if (c == '\\')
                        s.Append('\\');
                    else if (c == '/')
                        s.Append('/');
                    else if (c == 'b')
                        s.Append('\b');
                    else if (c == 'f')
                        s.Append('\f');
                    else if (c == 'n')
                        s.Append('\n');
                    else if (c == 'r')
                        s.Append('\r');
                    else if (c == 't')
                        s.Append('\t');
                    else
                        throw new SerializationException("Invalid STRING-Escape");
                }
                else
                    s.Append(c);
            }
            if (!complete)
            {
                success = false;
                return null;
            }
            return s.ToString();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private QMapToken LookAhead(int index)
        {
            return LookAhead(false, index);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="crossline"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private QMapToken LookAhead(bool crossline, int index)
        {
            int saveIndex = index;
            return NextToken(crossline, ref saveIndex);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private QMapToken NextToken(ref int index)
        {
            return NextToken(false, ref index);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="crossline"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private QMapToken NextToken(bool crossline, ref int index)
        {
            if (crossline)
                EatLineWhitespace(ref index);
            else
                EatWhitespace(ref index);

            if (index == scriptBuffer.Length)
                return QMapToken.None;

            char c = scriptBuffer[index];
            index++;
            switch (c)
            {
                case '{':
                    return QMapToken.CurlyOpen;
                case '}':
                    return QMapToken.CurlyClose;
                case '"':
                    return QMapToken.String;
                case '#':
                    return QMapToken.SharpComment;
            }
            index--;

            int remainingLength = scriptBuffer.Length - index;
            // [$...] Comment
            if (remainingLength >= 3) //[$]
            {
                if (scriptBuffer[index] == '[' && scriptBuffer[index + 1] == '$')
                {
                    index += 2;
                    return QMapToken.SquaredComment;
                }
            }
            // // Comment
            if (remainingLength >= 2)
            {
                if (scriptBuffer[index] == '/' && scriptBuffer[index + 1] == '/')
                {
                    index += 2;
                    return QMapToken.VirguleComment;
                }
            }

            return QMapToken.None;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        private void EatWhitespace(ref int index)
        {
            if (scriptBuffer == null)
                return;

            for (; index < scriptBuffer.Length; index++)
                if (" \t\b\f".IndexOf(scriptBuffer[index]) == -1) break;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        private void EatLineWhitespace(ref int index)
        {
            if (scriptBuffer == null)
                return;

            for (; index < scriptBuffer.Length; index++)
                if (" \t\n\r\b\f".IndexOf(scriptBuffer[index]) == -1) break;
        }
    }
    /// <summary>
    /// QMap Pair Entity
    /// </summary>
    public class QMapPair
    {
        /// <summary>
        /// 键
        /// </summary>
        public string Key { get; internal set; }
        /// <summary>
        /// 值
        /// </summary>
        public QMapValue Value { get; internal set; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("\"{0}\"{1}", this.Key, this.Value);
        }
    }
    /// <summary>
    /// QMap String Value
    /// </summary>
    public class QMapValue
    {
        /// <summary>
        /// 
        /// </summary>
        public List<string> Value { get; internal set; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("\t\"{0}\"\n", string.Join(",", this.Value));
        }
    }
    /// <summary>
    /// QMap Object Value
    /// </summary>
    public class QMapObjectValue : QMapValue
    {
        /// <summary>
        /// 
        /// </summary>
        public new IDictionary<string, QMapPair> Value { get; internal set; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (this.Value != null)
                return string.Format("\n{{\n{0}\n}}\n", string.Join<QMapPair>("", this.Value.Values));
            else
                return string.Empty;
        }
    }
}
