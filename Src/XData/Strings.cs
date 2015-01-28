using System;

namespace XData {
    public abstract class XStringBase : XAtomType {
        protected XStringBase() {
            _value = "";
        }
        protected XStringBase(string value) {
            Value = value;
        }
        public static implicit operator string (XStringBase obj) {
            if ((object)obj == null) return "";
            return obj._value;
        }
        private string _value;
        public string Value {
            get {
                return _value;
            }
            set {
                _value = value ?? "";
            }
        }
        protected abstract bool ValueEquals(string a, string b);
        protected abstract int GetValueHashCode(string s);
        protected abstract int CompareValue(string a, string b);
        public override bool Equals(XSimpleType other) {
            if ((object)this == other) return true;
            var otherType = other as XStringBase;
            if ((object)otherType == null) return false;
            return ValueEquals(_value, otherType._value);
        }
        public override int GetHashCode() {
            return GetValueHashCode(_value);
        }
        public override bool ValueEquals(object other) {
            return ValueEquals(_value, other as string);
        }
        public override bool TryCompareTo(XAtomType other, out int result) {
            result = 0;
            if ((object)this == other) return true;
            var otherType = other as XStringBase;
            if ((object)otherType == null) return false;
            result = CompareValue(_value, otherType._value);
            return true;
        }
        public override bool TryCompareValueTo(object other, out int result) {
            result = 0;
            var otherValue = other as string;
            if (otherValue == null) {
                return false;
            }
            result = CompareValue(_value, otherValue);
            return true;
        }
        public override bool TryParseAndSet(string literal) {
            Value = literal;
            return true;
        }
        public override string ToString() {
            return _value;
        }
        public override bool TryGetValueLength(out ulong result) {
            result = (ulong)_value.Length;
            return true;
        }
        public override void SaveValue(IndentedStringBuilder isb) {
            var value = _value;
            var length = value.Length;
            if (length == 0) {
                isb.Append("\"\"");
            }
            else {
                isb.Append("@\"");
                var sb = isb.StringBuilder;
                for (var i = 0; i < length; ++i) {
                    var ch = value[i];
                    if (ch == '"') {
                        sb.Append("\"\"");
                    }
                    else {
                        sb.Append(ch);
                    }
                }
                sb.Append('"');
            }
        }
    }
    public class XString : XStringBase {
        public XString() { }
        public XString(string value) : base(value) { }
        public static implicit operator XString(string value) {
            return new XString(value);
        }
        protected override sealed bool ValueEquals(string a, string b) {
            return a == b;
        }
        protected override sealed int GetValueHashCode(string s) {
            return s.GetHashCode();
        }
        protected override sealed int CompareValue(string a, string b) {
            return string.CompareOrdinal(a, b);
        }
        public override ObjectInfo ObjectInfo { get { return ThisInfo; } }
        new public static readonly AtomTypeInfo ThisInfo = TypeKind.String.ToAtomTypeInfo(typeof(XString), XAtomType.ThisInfo);
    }
    public class XIgnoreCaseString : XStringBase {
        public XIgnoreCaseString() { }
        public XIgnoreCaseString(string value) : base(value) { }
        public static implicit operator XIgnoreCaseString(string value) {
            return new XIgnoreCaseString(value);
        }
        protected override sealed bool ValueEquals(string a, string b) {
            return string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
        }
        protected override sealed int GetValueHashCode(string s) {
            return StringComparer.OrdinalIgnoreCase.GetHashCode(s);
        }
        protected override sealed int CompareValue(string a, string b) {
            return string.Compare(a, b, StringComparison.OrdinalIgnoreCase);
        }
        public override ObjectInfo ObjectInfo { get { return ThisInfo; } }
        new public static readonly AtomTypeInfo ThisInfo = TypeKind.IgnoreCaseString.ToAtomTypeInfo(typeof(XIgnoreCaseString), XAtomType.ThisInfo);
    }
}
