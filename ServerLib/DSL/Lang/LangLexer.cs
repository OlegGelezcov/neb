using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DSL.Lang {
    public class LangLexer {

        private const char EOF = char.MaxValue;

        private string mSource;
        private int mIndex = -1;
        private char mCurrent;
        private List<Token> mTokens = new List<Token>();
        private List<Token> mResult = new List<Token>();


        public LangLexer(string source) {
            mTokens = new List<Token>();
            mResult = new List<Token>();

            string[] arr = source.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            foreach(string s in arr) {
                Console.WriteLine("parse line: " + s);
                ParseLine(s);
            }
        }

        private void ParseLine(string line) {

            mSource = line;
            Parse();
            mResult.AddRange(mTokens);
            mResult.Add(new Token(Token.TokenType.STAT_END, "<N>"));

        }

        public TokenStream GetStream() {
            return new TokenStream(mResult);
        }

        private void ToNext() {
            mIndex++;
            if(mIndex < mSource.Length) {
                mCurrent = mSource[mIndex];
            } else {
                mCurrent = EOF;
            }

            //Console.WriteLine("handle: " + mCurrent);
        }

        private char Forward(int k) {
            int i = mIndex + k;
            if(i < mSource.Length) {
                return mSource[i];
            }
            return EOF;
        }

        private void ClearTokens() {
            mIndex = -1;
            mTokens.Clear();
        }

        private void AddToken(Token token) {
            mTokens.Add(token);
        }

        private void WS() {
            while(char.IsWhiteSpace(mCurrent)) {
                ToNext();
            }
        }

        private void IDENTIFIER() {
            StringBuilder sb = new StringBuilder();
            sb.Append(mCurrent);
            ToNext();
            if(char.IsLetter(mCurrent) || mCurrent == '_' ) {
                while(char.IsLetterOrDigit(mCurrent) || mCurrent == '_') {
                    sb.Append(mCurrent);
                    ToNext();
                }
                AddToken(new Token(Token.TokenType.ID, sb.ToString()));
            } else {
                throw new Exception("First symbol of identifier must be letter");
            }
        }

        private void NUMBER() {
            bool wasPoint = false;
            StringBuilder sb = new StringBuilder();

            while(char.IsDigit(mCurrent) || mCurrent == '.') {

                if(char.IsDigit(mCurrent)) {
                    sb.Append(mCurrent);
                } else if(mCurrent == '.') {
                    if(!wasPoint) {
                        wasPoint = true;
                        sb.Append(".");
                    } else {
                        throw new Exception("already was point in number");
                    }
                }

                ToNext();
            }

            if(wasPoint) {
                AddToken(new Token(Token.TokenType.FLOAT, sb.ToString()));
            } else {
                AddToken(new Token(Token.TokenType.INT, sb.ToString()));
            }
        }

        private void NAME() {
            StringBuilder sb = new StringBuilder();
            while(char.IsLetterOrDigit(mCurrent)) {
                sb.Append(mCurrent);
                ToNext();
            }
            string str = sb.ToString();
            switch (str.ToLower()) {
                case "if":
                    AddToken(new Token(Token.TokenType.IF, "if"));
                    break;
                case "elseif":
                    AddToken(new Token(Token.TokenType.ELSEIF, "elseif"));
                    break;
                case "else":
                    AddToken(new Token(Token.TokenType.ELSE, "else"));
                    break;
                case "endif":
                    AddToken(new Token(Token.TokenType.ENDIF, "endif"));
                    break;
                case "while":
                    AddToken(new Token(Token.TokenType.WHILE, "while"));
                    break;
                case "endwhile":
                    AddToken(new Token(Token.TokenType.ENDWHILE, "endwhile"));
                    break;
                case "begin":
                    AddToken(new Token(Token.TokenType.BEGIN, "begin"));
                    break;
                case "end":
                    AddToken(new Token(Token.TokenType.END, "end"));
                    break;
                case "true":
                    AddToken(new Token(Token.TokenType.BOOL, "true"));
                    break;
                case "false":
                    AddToken(new Token(Token.TokenType.BOOL, "false"));
                    break;
                default:
                    AddToken(new Token(Token.TokenType.NAME, str));
                    break;
            }
        }

        public void Parse() {
            ClearTokens();
            ToNext();
            while(mCurrent != EOF) {
                switch (mCurrent) {
                    case ' ':
                    case '\t':
                    case '\n':
                    case '\r':
                        WS();
                        break;
                    case '+':
                        AddToken(new Token(Token.TokenType.PLUS));
                        ToNext();
                        break;
                    case '-':
                        ToNext();
                        if (mCurrent == '>') {
                            AddToken(new Token(Token.TokenType.CALL, "->"));
                            ToNext();
                        } else {
                            AddToken(new Token(Token.TokenType.MINUS, "-"));
                        }
                        break;
                    case '*':
                        AddToken(new Token(Token.TokenType.MULT, "*"));
                        ToNext();
                        break;
                    case '/':
                        AddToken(new Token(Token.TokenType.DIV, "/"));
                        ToNext();
                        break;
                    case '=':
                        ToNext();
                        if(mCurrent == '=') {
                            AddToken(new Token(Token.TokenType.EQUAL, "=="));
                            ToNext();
                        } else {
                            AddToken(new Token(Token.TokenType.ASSIGN, "="));
                        }
                        break;
                    case '!':
                        ToNext();
                        if(mCurrent == '=') {
                            AddToken(new Token(Token.TokenType.NOT_EQUAL, "!="));
                            ToNext();
                        } else {
                            AddToken(new Token(Token.TokenType.NOT, "!"));
                        }
                        break;
                    case '>':
                        ToNext();
                        if(mCurrent == '=') {
                            AddToken(new Token(Token.TokenType.GE, ">="));
                            ToNext();
                        } else {
                            AddToken(new Token(Token.TokenType.GT, ">"));
                        }
                        break;
                    case '<':
                        ToNext();
                        if(mCurrent == '=') {
                            AddToken(new Token(Token.TokenType.LE, "<="));
                            ToNext();
                        } else {
                            AddToken(new Token(Token.TokenType.LT, "<"));
                        }
                        break;
                    case '$':
                        IDENTIFIER();
                        break;
                    case '[':
                        AddToken(new Token(Token.TokenType.LBRACK, "["));
                        ToNext();
                        break;
                    case ']':
                        AddToken(new Token(Token.TokenType.RBRACK, "]"));
                        break;
                    case ',':
                        AddToken(new Token(Token.TokenType.COMMA, ","));
                        break;
                    default:
                        if(char.IsDigit(mCurrent)) {
                            NUMBER();
                        } else if(char.IsLetter(mCurrent)) {
                            NAME();
                        }
                        break;

                }
            }
        }


    }
}
