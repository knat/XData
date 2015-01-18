using System;
using System.Collections.Generic;

namespace XData.IO.Text {
    public sealed class DelimitedList<T> : List<T> {
        public DelimitedList(TextSpan openTokenTextSpan) {
            OpenTokenTextSpan = openTokenTextSpan;
        }
        public readonly TextSpan OpenTokenTextSpan;
        public TextSpan CloseTokenTextSpan;
    }

    //public struct ListOrSingleNode<T> {
    //    public ListOrSingleNode(List<T> list, T single, bool hasSingle,
    //        TextSpan openTokenTextSpan, TextSpan closeTokenTextSpan) {
    //        List = list;
    //        Single = single;
    //        HasSingle = hasSingle;
    //        OpenTokenTextSpan = openTokenTextSpan;
    //        CloseTokenTextSpan = closeTokenTextSpan;
    //        IsValid = true;
    //    }
    //    public readonly List<T> List;//list take precedence
    //    public readonly T Single;
    //    public readonly bool HasSingle;
    //    public readonly TextSpan OpenTokenTextSpan;
    //    public readonly TextSpan CloseTokenTextSpan;
    //    public readonly bool IsValid;
    //    public bool HasList {
    //        get {
    //            return List != null;
    //        }
    //    }
    //    public bool HasItem {
    //        get {
    //            return List != null || HasSingle;
    //        }
    //    }
    //    public Enumerator GetEnumerator() {
    //        return new Enumerator(List, Single, HasSingle);
    //    }
    //    public struct Enumerator {
    //        internal Enumerator(List<T> list, T single, bool hasSingle) {
    //            _list = list;
    //            _single = single;
    //            _index = 0;
    //            if (list != null) {
    //                _count = list.Count;
    //            }
    //            else if (hasSingle) {
    //                _count = 1;
    //            }
    //            else {
    //                _count = 0;
    //            }
    //            _current = default(T);
    //        }
    //        private readonly List<T> _list;
    //        private readonly T _single;
    //        private readonly int _count;
    //        private int _index;
    //        private T _current;
    //        public bool MoveNext() {
    //            if (_index < _count) {
    //                if (_list != null) {
    //                    _current = _list[_index];
    //                }
    //                else {
    //                    _current = _single;
    //                }
    //                ++_index;
    //                return true;
    //            }
    //            return false;
    //        }
    //        public T Current {
    //            get {
    //                return _current;
    //            }
    //        }
    //    }
    //}

    //public struct ListNode<T> {
    //    public ListNode(List<T> list, TextSpan openTokenTextSpan, TextSpan closeTokenTextSpan) {
    //        List = list;
    //        OpenTokenTextSpan = openTokenTextSpan;
    //        CloseTokenTextSpan = closeTokenTextSpan;
    //    }
    //    public readonly List<T> List;
    //    public readonly TextSpan OpenTokenTextSpan;
    //    public readonly TextSpan CloseTokenTextSpan;
    //    private static readonly List<T> _emptyList = new List<T>();
    //    public List<T> EffectiveList {
    //        get {
    //            return List ?? _emptyList;
    //        }
    //    }
    //    public bool IsValid {
    //        get {
    //            return List != null;
    //        }
    //    }
    //}
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
    public enum AtomicValueKind : byte {
        None = 0,
        String,
        //Char,
        Boolean,
        Integer,
        Decimal,
        Real,
    }
    public struct AtomicValueNode {
        public AtomicValueNode(AtomicValueKind kind, string value, TextSpan textSpan) {
            Kind = kind;
            Value = value;
            TextSpan = textSpan;
        }
        public readonly AtomicValueKind Kind;
        public readonly string Value;
        public readonly TextSpan TextSpan;
        public bool IsValid {
            get {
                return Kind != AtomicValueKind.None;
            }
        }
    }
    public struct SimpleValueNode {
        public SimpleValueNode(QualifiableNameNode typeQName, AtomicValueNode atom, DelimitedList<SimpleValueNode> list) {
            TypeQName = typeQName;
            Atom = atom;
            List = list;
        }
        public readonly QualifiableNameNode TypeQName;
        public readonly AtomicValueNode Atom;
        public readonly DelimitedList<SimpleValueNode> List;
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
        public UriAliasingNode(NameNode alias, AtomicValueNode uri, bool isDefault) {
            Alias = alias;
            Uri = uri;
            IsDefault = isDefault;
        }
        public readonly NameNode Alias;
        public readonly AtomicValueNode Uri;
        public readonly bool IsDefault;
        //public bool HasAlias {
        //    get {
        //        return Alias.IsValid;
        //    }
        //}
        //public bool IsValid {
        //    get {
        //        return Uri.IsValid;
        //    }
        //}
    }
    public struct AttributeNode {
        public AttributeNode(QualifiableNameNode qName, SimpleValueNode value) {
            QName = qName;
            Value = value;
        }
        public readonly QualifiableNameNode QName;
        public readonly SimpleValueNode Value;
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
        public ComplexValueNode(TextSpan equalsTokenTextSpan, QualifiableNameNode typeQName,
            DelimitedList<AttributeNode> attributeList, DelimitedList<ElementNode> elementList,
            SimpleValueNode simpleValue, TextSpan semicolonTokenTextSpan) {
            EqualsTokenTextSpan = equalsTokenTextSpan;
            TypeQName = typeQName;
            AttributeList = attributeList;
            ElementList = elementList;
            SimpleValue = simpleValue;
            SemicolonTokenTextSpan = semicolonTokenTextSpan;
        }
        public readonly TextSpan EqualsTokenTextSpan;
        public readonly QualifiableNameNode TypeQName;
        public readonly DelimitedList<AttributeNode> AttributeList;
        public readonly DelimitedList<ElementNode> ElementList;
        public readonly SimpleValueNode SimpleValue;
        public readonly TextSpan SemicolonTokenTextSpan;
        public bool IsValid {
            get {
                return AttributeList != null || ElementList != null || SimpleValue.IsValid || SemicolonTokenTextSpan.IsValid;
            }
        }
        public TextSpan OpenElementTextSpan {
            get {
                if (ElementList != null) {
                    return ElementList.OpenTokenTextSpan;
                }
                if (SimpleValue.IsValid) {
                    return SimpleValue.TextSpan;
                }
                if (AttributeList != null) {
                    return AttributeList.CloseTokenTextSpan;
                }
                return SemicolonTokenTextSpan;
            }
        }
        public TextSpan CloseElementTextSpan {
            get {
                if (ElementList != null) {
                    return ElementList.CloseTokenTextSpan;
                }
                if (SimpleValue.IsValid) {
                    return SimpleValue.TextSpan;
                }
                if (AttributeList != null) {
                    return AttributeList.CloseTokenTextSpan;
                }
                return SemicolonTokenTextSpan;
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
