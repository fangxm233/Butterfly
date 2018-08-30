using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Compiler.LexicalAnalysis
{
    internal class Lexer
    {
        public static Token NextToken;
        public static int Line = 1;
        public static int NowFile;
        private static char _peek = ' ';
        private static readonly Dictionary<string, TokenType> s_keywordsSet = new Dictionary<string, TokenType>();
        private static readonly Dictionary<char, char> s_ECSet = new Dictionary<char, char>();
        private static readonly Dictionary<string, TokenType> s_signSet = new Dictionary<string, TokenType>();
        private static StreamReader _streamReader;
        private static string[] _files;

        //文件 行号 第几个词
        private static readonly List<List<List<Token>>> s_tokens = new List<List<List<Token>>>();
        private static int _index;

        public static Token Next()
        {
            while (_index > s_tokens[NowFile][Line - 1].Count - 1)
            {
                Line++;
                _index = 0;
                if (NowFile < _files.Length && Line > s_tokens[NowFile].Count + 1)
                {
                    NowFile++;
                    Line = 1;
                }
            }
            NextToken = s_tokens[NowFile][Line - 1][_index++];
            return NextToken;
        }

        public static void Scan(string[] files)
        {
            Init();
            _files = files;
            for (int i = 0; i < _files.Length; i++)
            {
                s_tokens.Add(new List<List<Token>>());
                s_tokens.Last().Add(new List<Token>());
            }
            ScanFiles();
            NowFile = 0;
            Line = 1;
            _index = 0;
        }

        private static void Init()
        {
            s_keywordsSet.Add("if", TokenType.If);
            s_keywordsSet.Add("while", TokenType.While);
            s_keywordsSet.Add("for", TokenType.For);
            s_keywordsSet.Add("break", TokenType.Break);
            s_keywordsSet.Add("continue", TokenType.Continue);
            s_keywordsSet.Add("class", TokenType.Class);
            s_keywordsSet.Add("struct", TokenType.Struct);
            s_keywordsSet.Add("interface", TokenType.Interface);
            s_keywordsSet.Add("namespace", TokenType.NameSpace);
            s_keywordsSet.Add("using", TokenType.Using);
            s_keywordsSet.Add("func", TokenType.Func);
            s_keywordsSet.Add("var", TokenType.Var);
            s_keywordsSet.Add("new", TokenType.New);
            s_keywordsSet.Add("override", TokenType.Override);
            s_keywordsSet.Add("static", TokenType.Static);
            s_keywordsSet.Add("int", TokenType.Int);
            s_keywordsSet.Add("float", TokenType.Float);
            s_keywordsSet.Add("string", TokenType.String);
            s_keywordsSet.Add("char", TokenType.Char);
            s_keywordsSet.Add("byte", TokenType.Byte);
            s_keywordsSet.Add("bool", TokenType.Bool);
            s_keywordsSet.Add("true", TokenType.BasicType);
            s_keywordsSet.Add("false", TokenType.BasicType);

            s_ECSet.Add('\"', '\"');
            s_ECSet.Add('\'', '\'');
            s_ECSet.Add('\\', '\\');
            s_ECSet.Add('b', '\b'); //退格
            s_ECSet.Add('f', '\f'); //换页
            s_ECSet.Add('t', '\t'); //水平制表符
            s_ECSet.Add('r', '\r'); //回车
            s_ECSet.Add('n', '\n'); //换行

            s_signSet.Add("[", TokenType.SquareBracket);
            s_signSet.Add("]", TokenType.SquareBracket);
            s_signSet.Add("(", TokenType.Parenthesis);
            s_signSet.Add(")", TokenType.Parenthesis);
            s_signSet.Add(".", TokenType.Sign);
            s_signSet.Add("-", TokenType.Sign);
            s_signSet.Add("+", TokenType.Sign);
            s_signSet.Add("++", TokenType.Sign);
            s_signSet.Add("--", TokenType.Sign);
            s_signSet.Add("!", TokenType.Sign);
            s_signSet.Add("/", TokenType.Sign);
            s_signSet.Add("*", TokenType.Sign);
            s_signSet.Add("%", TokenType.Sign);
            s_signSet.Add(">", TokenType.Sign);
            s_signSet.Add(">=", TokenType.Sign);
            s_signSet.Add("<", TokenType.Sign);
            s_signSet.Add("<=", TokenType.Sign);
            s_signSet.Add("==", TokenType.Sign);
            s_signSet.Add("!=", TokenType.Sign);
            s_signSet.Add("&&", TokenType.Sign);
            s_signSet.Add("||", TokenType.Sign);
            s_signSet.Add("=", TokenType.Assign);
            s_signSet.Add("+=", TokenType.Assign);
            s_signSet.Add("-=", TokenType.Assign);
            s_signSet.Add("*=", TokenType.Assign);
            s_signSet.Add("/=", TokenType.Assign);
            s_signSet.Add(";", TokenType.Semicolon);
            s_signSet.Add("{", TokenType.Braces);
            s_signSet.Add("}", TokenType.Braces);
        }

        private static void ScanFiles()
        {
            for (int i = 0; i < _files.Length; i++, NowFile = i)
            {
                _streamReader = new StreamReader(new FileStream(_files[i], FileMode.Open));
                Readch();
                for (; !_streamReader.EndOfStream; JumpBlank())
                {
                    if (char.IsLetter(_peek) || _peek == '_')
                    {
                        ScanIdentifer();
                        continue;
                    }
                    if (_peek == '"')
                    {
                        ScanString();
                        continue;
                    }
                    if (_peek == '\'')
                    {
                        ScanChar();
                        continue;
                    }
                    if (_peek == '/')
                    {
                        JumpAnnotation();
                        continue;
                    }
                    if (char.IsSymbol(_peek) || s_signSet.ContainsKey(_peek.ToString()))
                    {
                        ScanSign();
                        continue;
                    }
                    if (char.IsNumber(_peek))
                    {
                        ScanNumber();
                        continue;
                    }
                    return; //TODO:报错 意外的符号
                }
                _streamReader.Close();
            }
        }

        private static void ScanSign()
        {
            char c = _peek;
            Readch();
            if (char.IsSymbol(_peek))
            {
                string s = c.ToString() + _peek;
                if (s_signSet.ContainsKey(s))
                {
                    AddToken(s_signSet[s], s);
                    Readch();
                    return;
                }
                if (s_signSet.ContainsKey(_peek.ToString()))
                {
                    if (s_signSet.ContainsKey(c.ToString())) AddToken(s_signSet[c.ToString()], c.ToString());
                    else return; //TODO:报错 意外的符号
                    AddToken(s_signSet[_peek.ToString()], _peek.ToString());
                    Readch();
                    return;
                }
                else return; //TODO:报错 意外的符号
            }
            if (s_signSet.ContainsKey(c.ToString()))
                AddToken(s_signSet[c.ToString()], c.ToString());
            else return; //TODO:报错 意外的符号
        }

        private static void ScanIdentifer()
        {
            StringBuilder sb = new StringBuilder();
            do
            {
                sb.Append(_peek);
                Readch();
            } while (char.IsLetterOrDigit(_peek) || _peek == '_');

            string s = sb.ToString();
            if (s_keywordsSet.ContainsKey(s))
                AddToken(s_keywordsSet[s], s);
            else
                AddToken(TokenType.Identifer, s);
        }

        private static void ScanNumber()
        {
            StringBuilder sb = new StringBuilder();
            while (char.IsDigit(_peek))
            {
                sb.Append(_peek);
                Readch();
            }

            if(!IsBlank(_peek)) { return;} //TODO:报错 标识符不能以数字开头
            if (_peek != '.')
            {
                if (int.TryParse(sb.ToString(), out int n))
                    AddToken(TokenType.Number, sb.ToString());
                else
                    return; //TODO:报错 整数常量太大
            }

            sb.Append(_peek);
            for (; ; Readch())
            {
                if (!char.IsDigit(_peek)) break;
                sb.Append(_peek);
            }

            if (float.TryParse(sb.ToString(), out float f))
                AddToken(TokenType.FloatC, sb.ToString());
            else
                return; //TODO:报错 浮点数常量超出范围
        }

        private static void JumpBlank()
        {
            for (; ; Readch())
            {
                switch (_peek)
                {
                    case ' ':
                    case '\t':
                    case '\r':
                        continue;
                    case '\n':
                        AddToken(TokenType.EOL);
                        continue;
                }
                if (_streamReader.EndOfStream)
                {
                    AddToken(TokenType.EOF);
                    break;
                }
                break;
            }
        }

        private static void JumpAnnotation()
        {
            Readch();
            if (_peek == '/')
                for (;; Readch())
                {
                    if (_peek == '\n')
                    {
                        AddToken(TokenType.EOL);
                        Readch();
                        return;
                    }

                    if (_streamReader.EndOfStream)
                        return;
                }

            if (_peek == '*')
            {
                Readch();
                for (;; Readch())
                {
                    if (_peek == '\n')
                    {
                        AddToken(TokenType.EOL);
                        Readch();
                    }

                    if (_peek == '*')
                    {
                        Readch();
                        if (_peek == '/')
                        {
                            Readch();
                            return;
                        }
                    }

                    if (_streamReader.EndOfStream)
                    {
                        AddToken(TokenType.EOF);
                        return;
                    }
                }
            }

            AddToken(TokenType.Sign, "/");
        }

        private static void ScanString()
        {
            Readch();
            StringBuilder sb = new StringBuilder();
            for (; _peek != '"'; Readch())
            {
                if(_peek == '\n') { return;} //TODO:报错 应输入 "
                if (_peek == '\\')
                {
                    sb.Append(ScanEC());
                    continue;
                }
                sb.Append(_peek);
            }
            AddToken(TokenType.StringC, sb.ToString());
            Readch();
        }

        private static void ScanChar()
        {
            Readch();
            if (_peek == '\\')
            {
                char ec = ScanEC();
                Readch();
                if (_peek != '\'') { return;} //TODO:报错 字符文本中的字符太多
                AddToken(TokenType.CharC, ec.ToString());
                Readch();
                return;
            }
            char c = _peek;
            Readch();
            if(_peek != '\'') { return;} //TODO:报错 字符文本中的字符太多
            AddToken(TokenType.CharC, c.ToString());
            Readch();
        }

        private static char ScanEC()
        {
            Readch();
            if (!s_ECSet.ContainsKey(_peek)) { return '\0';} //TODO:报错 无效的转义字符
            return s_ECSet[_peek];
        }

        private static bool IsBlank(char c) => c == '\t' || c == '\r' || c == '\n' || c == ' ';

        private static void Readch()
        {
            _peek = (char)_streamReader.Read();
        }

        private static void AddToken(TokenType type)
        {
            if (type == TokenType.EOL)
            {
                s_tokens[NowFile].Add(new List<Token>());
                Line++;
                return;
            }
            s_tokens[NowFile][Line - 1].Add(new Token(type));
            if (type == TokenType.EOF)
                NowFile++;
        }

        private static void AddToken(string content) =>
            s_tokens[NowFile][Line - 1].Add(new Token(content));

        private static void AddToken(TokenType type, string content)
        {
            Token token = new Token(type);
            token.Content = content;
            s_tokens[NowFile][Line - 1].Add(token);
        }
    }
}
// declaration_specifiers 