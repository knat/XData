﻿using System;
using System.Collections.Generic;

namespace XData.IO.Text {
    public sealed class NodeList<T> : List<T> {
        public NodeList(TextSpan openTokenTextSpan) {
            OpenTokenTextSpan = openTokenTextSpan;
        }
        public readonly TextSpan OpenTokenTextSpan;
        public TextSpan CloseTokenTextSpan;

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
                return List.OpenTokenTextSpan;
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
        public ComplexValueNode(TextSpan equalsTextSpan, QualifiableNameNode typeQName,
            NodeList<AttributeNode> attributeList, NodeList<ElementNode> elementList,
            SimpleValueNode simpleChild, TextSpan semicolonTextSpan) {
            EqualsTextSpan = equalsTextSpan;
            TypeQName = typeQName;
            AttributeList = attributeList;
            ElementList = elementList;
            SimpleChild = simpleChild;
            SemicolonTextSpan = semicolonTextSpan;
        }
        public readonly TextSpan EqualsTextSpan;
        public readonly QualifiableNameNode TypeQName;
        public readonly NodeList<AttributeNode> AttributeList;
        public readonly NodeList<ElementNode> ElementList;
        public readonly SimpleValueNode SimpleChild;
        public readonly TextSpan SemicolonTextSpan;
        private static readonly List<AttributeNode> _emptyAttributeList = new List<AttributeNode>();
        private static readonly List<ElementNode> _emptyElementList = new List<ElementNode>();
        public List<AttributeNode> EffectiveAttributeList {
            get {
                return AttributeList ?? _emptyAttributeList;
            }
        }
        public List<ElementNode> EffectiveElementList {
            get {
                return ElementList ?? _emptyElementList;
            }
        }
        public bool IsValid {
            get {
                return AttributeList != null || ElementList != null || SimpleChild.IsValid || SemicolonTextSpan.IsValid;
            }
        }

        public TextSpan OpenAttributesTextSpan {
            get {
                if (AttributeList != null) {
                    return AttributeList.OpenTokenTextSpan;
                }
                return EqualsTextSpan;
            }
        }
        public TextSpan CloseAttributesTextSpan {
            get {
                if (AttributeList != null) {
                    return AttributeList.CloseTokenTextSpan;
                }
                return EqualsTextSpan;
            }
        }
        public TextSpan OpenChildrenTextSpan {
            get {
                if (ElementList != null) {
                    return ElementList.OpenTokenTextSpan;
                }
                if (SimpleChild.IsValid) {
                    return SimpleChild.TextSpan;
                }
                if (AttributeList != null) {
                    return AttributeList.CloseTokenTextSpan;
                }
                return SemicolonTextSpan;
            }
        }
        public TextSpan CloseChildrenTextSpan {
            get {
                if (ElementList != null) {
                    return ElementList.CloseTokenTextSpan;
                }
                if (SimpleChild.IsValid) {
                    return SimpleChild.TextSpan;
                }
                if (AttributeList != null) {
                    return AttributeList.CloseTokenTextSpan;
                }
                return SemicolonTextSpan;
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
