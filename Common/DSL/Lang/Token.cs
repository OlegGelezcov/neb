using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DSL.Lang {
    public class Token {
        public enum TokenType {
            BEGIN,
            END,
            PLUS, //4
            MINUS, //4
            MULT, //k-2
            DIV,  //k-2
            EQUAL, //3
            NOT_EQUAL, //k-4
            GT,   //3
            LT,   //3
            GE,   //3
            LE,   //3
            IF,   
            CALL, //k-1
            ELSE,
            ENDIF,
            ELSEIF,
            WHILE,
            ENDWHILE,
            ID,  //kk
            INT,  //k
            FLOAT, //k
            BOOL,  //k
            NAME, //k
            LBRACK, //k+1
            RBRACK, //k+1
            ASSIGN,  //1
            NOT, //k-2
            COMMA, //2
            INVALID,
            STAT_END,
            STAT_LIST
        }

        public TokenType type { get; private set; }
        public string text { get; private set; }
        public int index { get; private set; }

        public void SetIndex(int inIndex) {
            index = inIndex;
        }

        public Token(TokenType inType): this(inType, string.Empty){     
                 
        }



        public Token(TokenType inType, string inText) {
            type = inType;
            text = inText;
        }

        public override string ToString() {
            return string.Format("{0}: {1}", type, text);
        }
    }
}
