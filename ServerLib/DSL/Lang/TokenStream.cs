using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DSL.Lang {
    public class TokenStream {
        private List<Token> mTokens { get; set; } = new List<Token>();
        public int cursor { get; private set; }


        public TokenStream(List<Token> tokens) {
            mTokens = tokens;
            cursor = 0;
            for(int i = 0; i < mTokens.Count; i++) {
                mTokens[i].SetIndex(i);
            }
        }

        public Token current {
            get {
                if(cursor < mTokens.Count) {
                    return mTokens[cursor];
                }
                return new Token(Token.TokenType.INVALID);
            }
        }

        public void ToNext() {
            cursor++;
        }

        public Token Forward(int offset) {
            int i = cursor + offset;
            if(i < mTokens.Count) {
                return mTokens[i];
            }
            return new Token(Token.TokenType.INVALID);
        }

        public Token Backward(int offset) {
            int i = cursor - offset;
            if(i >= 0 && i < mTokens.Count) {
                return mTokens[i];
            }
            return new Token(Token.TokenType.INVALID);
        }

        public void Reset() {
            cursor = 0;
        }

        public void Print() {
            Reset();
            while(current.type != Token.TokenType.INVALID) {
                Console.WriteLine(current.ToString());
                ToNext();
            }
        }

        public Token LeftFromCurrent(Token.TokenType targetType) {
            for(int i = cursor - 1; i >= 0; i--) {
                if(mTokens[i].type == targetType) {
                    return mTokens[i];
                }
            }
            return new Token(Token.TokenType.INVALID);
        }

        public Token RightFromCurrent(Token.TokenType targetType) {
            for(int i = cursor + 1; i < mTokens.Count; i++) {
                if(mTokens[i].type == targetType) {
                    return mTokens[i];
                }
            }
            return new Token(Token.TokenType.INVALID);
        }
    }
}
