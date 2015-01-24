using System;
using System.Collections.Generic;

namespace XData.IO.Text {
    public sealed class NodeList<T> : List<T> {
        public NodeList(TextSpan openToken) {
            OpenToken = openToken;
        }
        public readonly TextSpan OpenToken;
        public TextSpan CloseToken;
    }

    public struct NameNode : IEquatable<NameNode> {
        public NameNode(string value, TextSpan textSpan) {
            Value = value;
            TextSpan = textSpan;
        }
        public readonly string Value;
        public readonly TextSpan TextSpan;
        public bool IsValid {
            get {
                return Value != null;
            }
        }
        public override string ToString() {
            return Value;
        }
        public bool Equals(NameNode other) {
            return Value == other.Value;
        }
        public override bool Equals(object obj) {
            return obj is NameNode && Equals((NameNode)obj);
        }
        public override int GetHashCode() {
            return Value != null ? Value.GetHashCode() : 0;
        }
        public static bool operator ==(NameNode left, NameNode right) {
            return left.Equals(right);
        }
        public static bool operator !=(NameNode left, NameNode right) {
            return !left.Equals(right);
        }
    }

    public struct QualifiableNameNode {
        public QualifiableNameNode(NameNode alias, NameNode name) {
            Alias = alias;
            Name = name;
            FullName = default(FullName);
        }
        public readonly NameNode Alias;
        public readonly NameNode Name;
        public FullName FullName;
        public bool IsQualified {
            get {
                return Alias.IsValid;
            }
        }
        public bool IsValid {
            get {
                return Name.IsValid;
            }
        }
        public override string ToString() {
            if (IsQualified) {
                return Alias.ToString() + ":" + Name.ToString();
            }
            return Name.ToString(); ;
        }
        public TextSpan TextSpan {
            get {
                return Name.TextSpan;
            }
        }
    }
    public enum AtomValueKind : byte {
        None = 0,
        String,
        //Char,
        Boolean,
        Integer,
        Decimal,
        Real,
    }
    public struct AtomValueNode {
        public AtomValueNode(AtomValueKind kind, string value, TextSpan textSpan) {
            Kind = kind;
            Value = value;
            TextSpan = textSpan;
        }
        public readonly AtomValueKind Kind;
        public readonly string Value;
        public readonly TextSpan TextSpan;
        public bool IsValid {
            get {
                return Kind != AtomValueKind.None;
            }
        }
    }
    public struct SimpleValueNode {
        public SimpleValueNode(QualifiableNameNode typeQName, AtomValueNode atom, NodeList<SimpleValueNode> list) {
            TypeQName = typeQName;
            Atom = atom;
            List = list;
        }
        public readonly QualifiableNameNode TypeQName;
        public readonly AtomValueNode Atom;
        public readonly NodeList<SimpleValueNode> List;
        public bool IsValid {
            get {
                return Atom.IsValid || List != null;
            }
        }
        public TextSpan TextSpan {
            get {
                if (Atom.IsValid) {
                    return Atom.TextSpan;
                }
                return List.OpenToken;
            }
        }
    }
    public struct UriAliasingNode {
        public UriAliasingNode(NameNode alias, AtomValueNode uri) {
            Alias = alias;
            Uri = uri;
        }
        public readonly NameNode Alias;
        public readonly AtomValueNode Uri;
    }
    public struct AttributeNode {
        public AttributeNode(NameNode name, SimpleValueNode value) {
            Name = name;
            Value = value;
        }
        public readonly NameNode Name;
        public readonly SimpleValueNode Value;
        public bool IsValid {
            get {
                return Name.IsValid;
            }
        }
    }
    public struct ElementValueNode {
        public ElementValueNode(ComplexValueNode complexValue, SimpleValueNode simpleValue) {
            ComplexValue = complexValue;
            SimpleValue = simpleValue;
        }
        public readonly ComplexValueNode ComplexValue;
        public readonly SimpleValueNode SimpleValue;
        public bool IsValid {
            get {
                return ComplexValue.IsValid || SimpleValue.IsValid;
            }
        }
    }
    public struct ComplexValueNode {
        public ComplexValueNode(TextSpan equalsToken, QualifiableNameNode typeQName,
            NodeList<AttributeNode> attributes, NodeList<ElementNode> complexChildren,
            SimpleValueNode simpleChild, TextSpan semicolonToken) {
            EqualsToken = equalsToken;
            TypeQName = typeQName;
            Attributes = attributes;
            ComplexChildren = complexChildren;
            SimpleChild = simpleChild;
            SemicolonToken = semicolonToken;
        }
        public readonly TextSpan EqualsToken;
        public readonly QualifiableNameNode TypeQName;
        public readonly NodeList<AttributeNode> Attributes;
        public readonly NodeList<ElementNode> ComplexChildren;
        public readonly SimpleValueNode SimpleChild;
        public readonly TextSpan SemicolonToken;
        public bool IsValid {
            get {
                return Attributes != null || ComplexChildren != null || SimpleChild.IsValid || SemicolonToken.IsValid;
            }
        }
        public TextSpan ChildrenOpenToken {
            get {
                if (ComplexChildren != null) {
                    return ComplexChildren.OpenToken;
                }
                if (SimpleChild.IsValid) {
                    return SimpleChild.TextSpan;
                }
                if (Attributes != null) {
                    return Attributes.CloseToken;
                }
                return SemicolonToken;
            }
        }
        public TextSpan ChildrenCloseToken {
            get {
                if (ComplexChildren != null) {
                    return ComplexChildren.CloseToken;
                }
                if (SimpleChild.IsValid) {
                    return SimpleChild.TextSpan;
                }
                if (Attributes != null) {
                    return Attributes.CloseToken;
                }
                return SemicolonToken;
            }
        }
    }
    public struct ElementNode {
        public ElementNode(QualifiableNameNode qName, ElementValueNode value) {
            QName = qName;
            Value = value;
        }
        public readonly QualifiableNameNode QName;
        public readonly ElementValueNode Value;
        //public bool HasValue {
        //    get {
        //        return Value.IsValid;
        //    }
        //}
        public bool IsValid {
            get {
                return QName.IsValid;
            }
        }
        public FullName FullName {
            get {
                return QName.FullName;
            }
        }
    }

}
