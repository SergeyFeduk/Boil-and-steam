using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
class Deserializer {

    private readonly BinaryFormatter binaryFormatter;
    private readonly Lexer lexer;
    private readonly Parser parser;
    private readonly Generator generator;
    public Deserializer(BinaryFormatter binaryFormatter) {
        this.binaryFormatter = binaryFormatter;
        lexer = new Lexer();
        parser = new Parser();
        generator = new Generator(binaryFormatter);
    }

    public object Deserialize(string inputData) {
        Type objectType;
        string data;
        (objectType, data) = SeparateHeader(inputData);
        List<Lexer.Token> tokens = lexer.Tokenize(data);
        Parser.Node node = parser.Parse(tokens);
        object instance = generator.GenerateInstance(node, objectType);
        return instance;
    }
    private (Type, string) SeparateHeader(string inputData) {
        string[] lines = inputData.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        if (lines.Length < 3) throw new Exception("String does not contain any data");
        string name = lines[0];
        Type headType = Type.GetType(name);
        string[] bodyLines = new ArraySegment<string>(lines, 1, lines.Length - 1).ToArray();
        string body = string.Join("\n", bodyLines);
        return (headType, body);
    }

    /*private enum Tag {
        LBRACKET,
        RBRACKET,
        SEPARATOR,
        EQUALITY,

        VAL,
        REF,
        LIST,

        COMPDICT,
        COMP,

        IDENTIFIER,
        VALUE,

        EOF
    }
    private class Token {
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

    private class Sentence {

    }

    private class SentenceBlock : Sentence {
        public string identifier;
        public Tag type;
        public List<Sentence> sentences = new List<Sentence>();
        public SentenceBlock() { }
        public SentenceBlock(string identifier, Tag type, List<Sentence> sentences) {
            this.identifier = identifier;
            this.type = type;
            this.sentences = sentences;
        }
    }

    private class SentenceValue : Sentence {
        public string identifier;
        public Tag type;
        public string value;
        public SentenceValue() { }
        public SentenceValue(string identifier, Tag type, string value = null) {
            this.identifier = identifier;
            this.type = type;
            this.value = value;
        }
    }

    public object Deserialize(string serialized) {
        string[] lines = serialized.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        if (lines.Length < 3) throw new Exception("String does not contain any data");
        //Handle header
        string name = lines[0];
        Type headType = Type.GetType(name);
        //Handle body
        string[] bodyLines = new ArraySegment<string>(lines, 1, lines.Length - 1).ToArray();
        string body = string.Join("\n", bodyLines);
        List<Token> tokens = Tokenize(body);

        Sentence sentenceTree = Parse(tokens);
        return null;
        //return GenerateInstance(sentenceTree, headType);
    }

    /*private object GenerateInstance(List<Sentence> sentences, Type type) {
        object typedObj = Activator.CreateInstance(type);
        for (int i = 0; i < sentences.Count; i++) {
            PropertyInfo propertyInfo = type.GetProperty(sentences[i].identifier);
            HandleValue(propertyInfo, sentences[i].type, sentences[i].value, typedObj);
            HandleReference(propertyInfo, sentences[i].type, sentences[i].value, typedObj);
            HandleList(propertyInfo, sentences[i].type, sentences[i].value, typedObj);
        }
        return typedObj;
    }

    private void HandleValue(PropertyInfo propertyInfo, Tag tag, string value, object typedObj) {
        if (tag == Tag.VAL) {
            byte[] bytes = Convert.FromBase64String(value);
            using (MemoryStream stream = new MemoryStream(bytes)) {
                object obj = binaryFormatter.Deserialize(stream);
                propertyInfo.SetValue(typedObj, obj, null);
            }
        }
    }

    private void HandleReference(PropertyInfo propertyInfo, Tag tag, string value, object typedObj) {
        if (tag == Tag.REF) {
            int key = int.Parse(value);
            object obj = DAssetDatabase.GetAssetByKey(key);
            propertyInfo.SetValue(typedObj, obj, null);
        }
    }

    private void HandleList(PropertyInfo propertyInfo, Tag tag, string value, object typedObj) {
        if (tag == Tag.LIST) {
            Type Ttype = propertyInfo.PropertyType.GetGenericArguments()[0];
            var listType = typeof(List<>).MakeGenericType(Ttype);
            IList objList = (IList)Activator.CreateInstance(listType);
            string currentObj = "";

            for (int k = 1; k < value.Length; k++) {
                char currentChar = value[k];
                if (currentChar == '|' || k >= value.Length) {
                    byte[] bytes = Convert.FromBase64String(currentObj);
                    using (MemoryStream stream = new MemoryStream(bytes)) {
                        object obj = binaryFormatter.Deserialize(stream);
                        objList.Add(obj);
                    }
                    currentObj = "";
                    continue;
                }
                currentObj += currentChar;
            }
            propertyInfo.SetValue(typedObj, objList, null);
        }
    }

    private Sentence Parse(List<Token> tokens) {
        //Make this better please
        SentenceBlock sentenceTree = new SentenceBlock();
        int index = 0;
        return TryMakeSentence(ref index, ref tokens, sentenceTree);
    }

    private Sentence TryMakeSentence(ref int index, ref List<Token> tokens, SentenceBlock root) {
        while (index < tokens.Count) {
            Token currentToken = tokens[index];
            int savedIndex = index; Token savedToken = currentToken;
            Sentence sentence = TryMakeValueSentence(ref index, ref currentToken, ref tokens);
            if (sentence != null) {
                Debug.Log(sentence);
                root.sentences.Add(sentence);
            } else {
                index = savedIndex;
                currentToken = savedToken;

                sentence = TryMakeSentenceBlock(ref index, ref currentToken, ref tokens);
                if (sentence != null) {
                    Debug.Log(((SentenceBlock)sentence).identifier);
                    root.sentences.Add(sentence);
                } else {
                    return null;
                }

            }
        }
        return root;
    }

    private Sentence TryMakeSentenceBlock(ref int index, ref Token currentToken, ref List<Token> tokens) {
        SentenceBlock sentence = new SentenceBlock();
        Tag typeTag;
        parserAdvance(ref index, ref currentToken, tokens);
        if (currentToken.tag != Tag.IDENTIFIER) return null;
        sentence.identifier = currentToken.value as string;
        parserAdvance(ref index, ref currentToken, tokens);
        if (currentToken.tag != Tag.SEPARATOR) return null;
        parserAdvance(ref index, ref currentToken, tokens);
        typeTag = currentToken.tag;
        if (currentToken.tag != Tag.COMP && currentToken.tag != Tag.COMPDICT) return null;
        sentence.type = currentToken.tag;
        parserAdvance(ref index, ref currentToken, tokens);
        if (currentToken.tag != Tag.EQUALITY) return null;
        parserAdvance(ref index, ref currentToken, tokens);
        //From this all tokens are inside
        List<Token> tokensInside = new List<Token>();
        List<Sentence> sentencesInside = new List<Sentence>();
        //There is no nested components so it's okay
        Debug.Log(sentence.identifier);
        while (currentToken.tag != typeTag) {
            parserAdvance(ref index, ref currentToken, tokens);
            tokensInside.Add(currentToken);
            //Sentence insideSentence = TryMakeSentence(ref index, ref tokensInside, sentence);
            Sentence insideSentence = TryMakeValueSentence(ref index, ref currentToken, ref tokens);
            if (insideSentence == null) continue;
            else {
                Debug.Log(((SentenceValue)insideSentence).identifier);
                sentencesInside.Add(insideSentence);
                tokensInside.Clear();
            }
        }
        sentence.sentences.AddRange(sentencesInside);
        if (currentToken.tag != typeTag) return null;
        //parserAdvance(ref index, ref currentToken, tokens);
        //parserAdvance(ref index, ref currentToken, tokens);
        //parserAdvance(ref index, ref currentToken, tokens);

        return sentence;
    }

    private Sentence TryMakeValueSentence(ref int index, ref Token currentToken, ref List<Token> tokens) {
        SentenceValue sentence = new SentenceValue();
        parserAdvance(ref index, ref currentToken, tokens);
        if (currentToken.tag != Tag.IDENTIFIER) return null;
        sentence.identifier = currentToken.value as string;
        parserAdvance(ref index, ref currentToken, tokens);
        if (currentToken.tag != Tag.SEPARATOR) return null;
        parserAdvance(ref index, ref currentToken, tokens);
        if (currentToken.tag != Tag.VAL && currentToken.tag != Tag.REF && currentToken.tag != Tag.LIST) return null;
        sentence.type = currentToken.tag;
        parserAdvance(ref index, ref currentToken, tokens);
        if (currentToken.tag != Tag.EQUALITY) return null;
        parserAdvance(ref index, ref currentToken, tokens);
        if (currentToken.tag != Tag.VALUE) return null;
        sentence.value = currentToken.value as string;
        parserAdvance(ref index, ref currentToken, tokens);
        parserAdvance(ref index, ref currentToken, tokens);
        return sentence;
    }

    private void parserAdvance(ref int index, ref Token currentToken, List<Token> tokens) {
        index++;
        if (index > tokens.Count - 1) {
            currentToken = null;
        } else {
            currentToken = tokens[index];
        }
    }

    private List<Token> Tokenize(string serialized) {
        if (serialized.Length == 0) return null;
        List<Token> tokens = new List<Token>();
        char? currentChar;
        int index = 0;
        currentChar = serialized[index];
        while (index < serialized.Length) {
            int initialIndex = index;
            Token token = GenerateToken(ref index, ref currentChar, serialized, tokens);
            //Debug.Log(token);
            tokens.Add(token);
            if (initialIndex == index) throw new Exception("Failed tokenizing");
        }
        return tokens;
    }

    private void AddListOfTokens(List<Token> tokens, List<Token> totokens) {
        totokens.AddRange(tokens);
    }

    private Token GenerateToken(ref int index, ref char? currentChar, string line, List<Token> tokens) {
        while (currentChar != null) {
            Token generatedToken = GenerateCurrentToken(ref index, ref currentChar, line, tokens);
            if (generatedToken == null) continue;
            return generatedToken;
        }
        return new Token(Tag.EOF);
    }

    private string GetStringValue(ref int index, ref char? currentChar, string line) {
        string value = "";
        while (currentChar != null && currentChar != ']') {
            if (currentChar == ' ') {
                advance(ref index, ref currentChar, line);
                continue;
            }
            value += currentChar;
            advance(ref index, ref currentChar, line);
        }
        return value;
    }

    private Token GenerateCurrentToken(ref int index, ref char? currentChar, string line, List<Token> tokens) {
        int initialIndex = index;
        if (tokens.Count > 2 && tokens[^2] != null && tokens[^2].tag == Tag.COMPDICT) {
            //Handle components dictionary
            string value = GetStringValue(ref index, ref currentChar, line);
            List<Token> tokensToAdd = Tokenize(value);
            AddListOfTokens(tokensToAdd, tokens);
            return null;
        }
        if (tokens.Count > 2 && tokens[^2] != null && tokens[^2].tag == Tag.COMP) {
            //Handle nested component 
            string value = GetStringValue(ref index, ref currentChar, line);
            List<Token> tokensToAdd = Tokenize(value);
            AddListOfTokens(tokensToAdd, tokens);
            return null;
        }

        if (tokens.Count > 1 && tokens[^1] != null && tokens[^1].tag == Tag.EQUALITY) {
            string value = GetStringValue(ref index, ref currentChar, line);
            return new Token(Tag.VALUE, value);
        }

        if (currentChar == ' ' || currentChar == '\t') {
            skipSpace(ref index, ref currentChar, line);
        }
        if (currentChar == '\n') {
            advance(ref index, ref currentChar, line);
        }
        if (currentChar == '[') {
            advance(ref index, ref currentChar, line);
            return new Token(Tag.LBRACKET, null);
        }
        if (currentChar == ']') {
            advance(ref index, ref currentChar, line);
            return new Token(Tag.RBRACKET, null);
        }
        if (currentChar == ':') {
            advance(ref index, ref currentChar, line);
            return new Token(Tag.SEPARATOR, null);
        }
        if (currentChar == '=') {
            advance(ref index, ref currentChar, line);
            return new Token(Tag.EQUALITY, null);
        }
        if (letters.Contains(currentChar) || topLetters.Contains(currentChar)) {
            return ParseDatatype(ref index, ref currentChar, line);
        }
        advance(ref index, ref currentChar, line);
        if (initialIndex == index) throw new Exception("Failed tokenizing");
        return GenerateCurrentToken(ref index, ref currentChar, line, tokens);
    }

    private Token ParseDatatype(ref int index, ref char? currentChar, string line) {
        string name = "";
        while (currentChar != null && (letters.Contains(currentChar) || topLetters.Contains(currentChar))) {
            name += currentChar;
            advance(ref index, ref currentChar, line);
        }
        Tag tokenTag = keywords.Any(i => i.Key == name) ? keywords[name] : Tag.IDENTIFIER;
        return new Token(tokenTag, name);
    }

    private void advance(ref int index, ref char? currentChar, string line) {
        index++;
        if (index > line.Length - 1) {
            currentChar = null;
        } else {
            currentChar = line[index];
        }
    }
    private void skipSpace(ref int index, ref char? currentChar, string line) {
        while (currentChar != null && (currentChar == ' ' || currentChar == '\t')) advance(ref index, ref currentChar, line);
    }*/
}
