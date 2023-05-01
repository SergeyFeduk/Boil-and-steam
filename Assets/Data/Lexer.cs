using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
class Lexer {
    private readonly char?[] letters = Enumerable.Range('a', 'z' - 'a' + 1).Select(i => (char?)i).ToArray();
    private readonly char?[] topLetters = Enumerable.Range('A', 'Z' - 'A' + 1).Select(i => (char?)i).ToArray();
    private readonly char?[] digits = Enumerable.Range('0', '9' - '0' + 1).Select(i => (char?)i).ToArray();
    private readonly char?[] base64;
    private readonly Dictionary<string, Tag> keywords = new Dictionary<string, Tag>{
        { "val",        Tag.VAL     },
        { "ref",        Tag.REF     },
        { "list",       Tag.LIST    },
        { "compDict",   Tag.COMPDICT},
        { "comp",       Tag.COMP    },

    };
    public enum Tag {
        LBRACKET,
        RBRACKET,

        LPARENT,
        RPARENT,

        SEPARATOR,
        EQUALITY,

        VAL,
        REF,
        LIST,

        COMPDICT,
        COMP,

        IDENTIFIER,

        EOF
    }

    public class Token {
        public Tag tag;
        public object value;
        public Token(Tag tag, object value = null) {
            this.tag = tag;
            this.value = value;
        }
        public override string ToString() {
            return tag.ToString() + " : " + value ?? "Null";
        }
    }

    public Lexer() {
        int offset = 4;
        base64 = new char?[letters.Length + topLetters.Length + digits.Length + offset];
        base64[0] = '+';
        base64[1] = '/';
        base64[2] = '=';
        base64[3] = '|';
        for (int i = offset; i < letters.Length + offset; i++) {
            base64[i] = letters[i - offset];
        }
        for (int i = 0; i < topLetters.Length; i++) {
            base64[i + letters.Length + offset] = topLetters[i];
        }
        for (int i = 0; i < digits.Length; i++) {
            base64[i + letters.Length + topLetters.Length + offset] = digits[i];
        }
    }

    private int index = -1;
    private string data;
    private char? currentChar;

    public List<Token> Tokenize(string inputData) {
        data = inputData;
        index = -1;
        Advance();
        return MakeTokens();
    }

    private List<Token> MakeTokens() {
        List<Token> tokens = new List<Token>();
        while (currentChar != null) {
            if (currentChar == ' ' || currentChar == '\t') {
                SkipSpace();
            }
            if (currentChar == '\n') {
                Advance();
            }
            if (currentChar == '=') {
                Advance();
                tokens.Add(new Token(Tag.EQUALITY, null));
                continue;
            }
            if (base64.Contains((char)currentChar)) {
                tokens.Add(TokenizeDatatype());
                continue;
            }
            if (currentChar == '[') {
                Advance();
                tokens.Add(new Token(Tag.LBRACKET, null));
                continue;
            }
            if (currentChar == ']') {
                Advance();
                tokens.Add(new Token(Tag.RBRACKET, null));
                continue;
            }
            if (currentChar == '{') {
                Advance();
                tokens.Add(new Token(Tag.LPARENT, null));
                continue;
            }
            if (currentChar == '}') {
                Advance();
                tokens.Add(new Token(Tag.RPARENT, null));
                continue;
            }
            if (currentChar == ':') {
                Advance();
                tokens.Add(new Token(Tag.SEPARATOR, null));
                continue;
            }
            Advance();
            continue;
            throw new Exception("Tokenization error");
        }
        return tokens;
    }

    private Token TokenizeDatatype() {
        string line = "";
        while (currentChar != null && base64.Contains((char)currentChar)) {
            line += currentChar;
            Advance();
        }
        Tag tag = keywords.Any(i => i.Key == line) ? keywords[line] : Tag.IDENTIFIER;
        return new Token(tag, line);
    }

    private void Advance() {
        index++;
        if (index > data.Length - 1) {
            currentChar = null;
        } else {
            currentChar = data[index];
        }
    }

    private void SkipSpace() {
        while (currentChar != null && (currentChar == ' ' || currentChar == '\t')) Advance();
    }
}