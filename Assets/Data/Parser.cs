using System.Collections.Generic;
using UnityEngine;
class Parser {

    #region Nodes
    public class Node {
        public string identifier;
        public Node(string identifier = "") {
            this.identifier = identifier;
        }
        public Node(ParentNode node) {
            identifier = node.identifier;
        }
        public Node(ValueNode node) {
            identifier = node.identifier;
        }
        public virtual string Output() {
            return identifier + "\n";
        }
    }

    public class ValueNode : Node {
        public string value;
        public Lexer.Tag tag;
        public ValueNode(Lexer.Tag tag, string identifier = "", string value = "") {
            this.identifier = identifier;
            this.value = value;
            this.tag = tag;
        }
        public ValueNode(Node node) {
            identifier = node.identifier;
        }
        public override string Output() {
            return identifier + " = " + value + "\n";
        }
    }

    public class ParentNode : Node {
        public List<Node> nodes = new List<Node>();
        public ParentNode() {
        }
        public ParentNode(Node node) {
            identifier = node.identifier;
        }
        public override string Output() {
            string output = identifier + "\n [ \n";
            for (int i = 0; i < nodes.Count; i++) {
                if (nodes[i] == null) continue;
                output += nodes[i].Output();
            }
            return output + "]\n";
        }
    }
    #endregion

    private List<Lexer.Token> tokens;
    private int index = -1;
    private Lexer.Token currentToken;

    public Node Parse(List<Lexer.Token> inputTokens) {
        tokens = inputTokens;
        index = -1;
        Advance();
        ParentNode root = new ParentNode();
        while (index < tokens.Count) {
            Node node = Expression();
            if (node != null) {
                root.nodes.Add(node);
            }
        }
        return root;
    }

    #region Parsers
    private Node Expression() {
        Node node = new Node();
        if (IsNotTag(Lexer.Tag.LBRACKET,true)) return null;
        if (IsNotTag(Lexer.Tag.IDENTIFIER,false)) return null;
        node.identifier = currentToken.value as string;
        Advance();
        if (IsNotTag(Lexer.Tag.SEPARATOR,true)) return null;
        if (currentToken.tag == Lexer.Tag.VAL || currentToken.tag == Lexer.Tag.REF || currentToken.tag == Lexer.Tag.LIST) {
            Lexer.Tag tag = currentToken.tag;
            Advance();
            return Serializable(node, tag);
        } else if (currentToken.tag == Lexer.Tag.COMPDICT || currentToken.tag == Lexer.Tag.COMP) {
            Advance();
            return Nested(node);
        }
        Advance();
        return null;
    }

    private Node Serializable(Node node, Lexer.Tag tag) {
        if (IsNotTag(Lexer.Tag.EQUALITY,true)) return null;
        if (IsNotTag(Lexer.Tag.LPARENT,true)) return null;
        ValueNode valueNode = new ValueNode(node);
        if (currentToken.tag != Lexer.Tag.IDENTIFIER) {
            if (tag == Lexer.Tag.LIST) {
                valueNode.value = "null";
                valueNode.tag = tag;
            } else {
                return null;
            }
        } else {
            valueNode.value = currentToken.value as string;
            valueNode.tag = tag;
            Advance();
        }
        if (IsNotTag(Lexer.Tag.RPARENT,true)) return null;
        if (IsNotTag(Lexer.Tag.RBRACKET,true)) return null;
        return valueNode;
    }

    private Node Nested(Node node) {
        if (IsNotTag(Lexer.Tag.EQUALITY,true)) return null;
        if (IsNotTag(Lexer.Tag.LPARENT,true)) return null;
        ParentNode parentNode = new ParentNode(node);
        while (currentToken.tag != Lexer.Tag.RPARENT) {
            parentNode.nodes.Add(Expression());
        }
        if (IsNotTag(Lexer.Tag.RPARENT, true)) return null;
        if (IsNotTag(Lexer.Tag.RBRACKET, true)) return null;
        return parentNode;
    }
    #endregion

    private bool IsNotTag(Lexer.Tag tag, bool immediateAdvance) {
        Lexer.Tag savedTag = currentToken.tag;
        if (immediateAdvance) Advance();
        if (savedTag != tag) {
            if (!immediateAdvance) Advance();
            return true;
        }
        return false;
    }

    private void Advance() {
        index++;
        if (index < tokens.Count) {
            currentToken = tokens[index];
        }
    }
}
