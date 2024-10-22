﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace LGBTCN.Bot.Util
{
    // SOURCE   https://github.com/Guila767/GoogleTranslateApi
    // AUTHOR   GitHub@Guila767
    // MODIFIER GitHub@KevinZonda
    // LICENCE  MIT LICENSE
    public class Translate
    {
        private const string Url = "https://translate.googleapis.com/translate_a/single?client=gtx&sl=";
        private string Request { get; set; } = String.Empty;

        private struct Block
        {
            public object[] Data { get; }
            public int Elements { get => Data.Length; }
            public int Blocks
            {
                get
                {
                    int c = 0;
                    foreach (object _data in Data)
                        if (_data is Block)
                            c++;
                    return c;
                }
            }
            public Block this[int x]
            {
                get
                {
                    if (x > Blocks - 1)
                        throw new IndexOutOfRangeException("Index out of range");
                    List<Block> _blocks = new List<Block>();
                    foreach (object _data in Data)
                    {
                        if (_data is Block)
                        {
                            //Block block = _data as Block? ?? throw new Exception("Cannot Parse the block at the given index");
                            _blocks.Add((Block)_data);
                        }
                    }
                    return _blocks[x];
                }
            }

            /// <summary>
            /// Creates a Block class 
            /// </summary>
            /// <param name="data">The data string to be parsed</param>
            /// <exception cref="ArgumentException"></exception>
            public Block(string data)
            {
                if (!IsValidData(data))
                    throw new ArgumentException("Invalid data string", "data");
                List<object> ldata = new List<object>() { null };
                Queue<char> vs = new Queue<char>(data.ToCharArray());

                while (vs.Count > 0)
                {
                    char @char = vs.Dequeue();
                    switch (@char)
                    {
                        case '[':
                            string nblock = new string(vs.ToArray());
                            int end = 0;
                            /* FIXED - find the correct end*/
                            for (int n = 1; n != 0; end++)
                            {
                                if (nblock[end] == '[')
                                    n++;
                                else if (nblock[end] == ']')
                                    n--;
                            }
                            vs = new Queue<char>(nblock.Substring(end));
                            ldata[ldata.Count - 1] = new Block(nblock.Substring(0, end - 1));
                            break;
                        case ',':
                            ldata.Add(null);
                            continue;
                        case ']':
                            Data = ldata.ToArray();
                            return;
                        case '"':
                            do
                            {
                                @char = vs.Peek();
                                switch (@char)
                                {
                                    case '\\':
                                        @char = vs.Dequeue();
                                        break;
                                    case '"':
                                        continue;
                                    default:
                                        @char = vs.Dequeue();
                                        break;
                                }
                                ldata[ldata.Count - 1] = String.Concat(ldata[ldata.Count - 1], @char == '\\' ? vs.Dequeue() : @char);
                            }
                            while (vs.Peek() != '"');
                            vs.Dequeue();
                            break;
                        default:
                            ldata[ldata.Count - 1] = String.Concat(ldata[ldata.Count - 1], @char);
                            break;
                    }

                }

                Data = ldata.ToArray();
            }

            private static bool IsValidData(string data)
            {
                if ((data.Count(c => c == '[') + data.Count(c => c == ']')) % 2 != 0)
                    return false;
                if (data.Count(c => c == '"') % 2 != 0)
                    return false;
                return true;
            }
        }

        public class Language
        {
            private Language(string x) { Value = x; }

            public string Value { get; set; }

            public static Language Auto { get { return new Language("auto"); } }
            public static Language Portuguese { get { return new Language("pt"); } }
            public static Language English { get { return new Language("en"); } }
            public static Language Spanish { get { return new Language("es"); } }
            public static Language Russian { get { return new Language("ru"); } }
            public static Language French { get { return new Language("fr"); } }
            public static Language German { get { return new Language("de"); } }
            public static Language Swedish { get { return new Language("sv"); } }
            public static Language Italian { get { return new Language("it"); } }
            public static Language Japanese { get { return new Language("ja"); } }
            public static Language Chinese { get { return new Language("zh"); } }
        }


        public Translate(Language source, Language target)
        {
            //if (target.Value == Language.Auto.Value)
            //    throw new Exception("The target language can't be Language.Auto");
            this.Request = Url + $"{source.Value}&tl={target.Value}&dt=t&q=";
        }

        private string Download(string text)
        {
            WebClient web = new WebClient
            {
                Encoding = Encoding.UTF8
            };
            //web.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/5.0");
            //web.Headers.Add(HttpRequestHeader.AcceptCharset, "UTF-8");
            Uri uri = new Uri(this.Request + Uri.EscapeUriString(text));
            return web.DownloadString(uri);
        }

        /// <summary>
        /// Returns the translated text
        /// </summary>
        /// <param name="text">The text to be translated</param>
        /// <returns>A string that contains the translated text</returns>
        public string Text(string text)
        {
            string Dest = string.Empty;
            /* FIXED - Remove '\n' (Line feed/new line char) */
            text = (Download(text)).Replace("\n", "");
            /* FIXED - Gets the multiples blocks that can be received */
            Block Datablock = new Block(text);
            for (int n = 0; n < Datablock[0][0].Blocks; n++)
            {
                Block splitData = Datablock[0][0][n];
                Dest = String.Concat(Dest, splitData.Data[0]);
            }
            return Dest;
        }
    }
}