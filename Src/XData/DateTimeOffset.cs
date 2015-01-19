using System;
using System.Globalization;

namespace XData {
    public class XDateTimeOffset : XAtomType {
        public XDateTimeOffset() { }
        public XDateTimeOffset(DateTimeOffset value) {
            _value = value;
        }
        public static implicit operator XDateTimeOffset(DateTimeOffset value) {
            return new XDateTimeOffset(value);
        }
        public static implicit operator DateTimeOffset(XDateTimeOffset obj) {
            if ((object)obj == null) return DateTimeOffset.MinValue;
            return obj._value;
        }
        private DateTimeOffset _value;
        public DateTimeOffset Value {
            get {
                return _value;
            }
            set {
                _value = value;
            }
        }
        public override bool Equals(XSimpleType other) {
            if ((object)this == other) return true;
            var otherType = other as XDateTimeOffset;
            if ((object)otherType == null) return false;
            return _value == otherType._value;
        }
        public override int GetHashCode() {
            return _value.GetHashCode();
        }
        public override bool ValueEquals(object other) {
            if (other is DateTimeOffset) {
                return _value == (DateTimeOffset)other;
            }
            return false;
        }
        public override bool TryCompareTo(XAtomType other, out int result) {
            result = 0;
            if ((object)this == other) return true;
            var otherType = other as XDateTimeOffset;
            if ((object)otherType == null) return false;
            result = _value.CompareTo(otherType._value);
            return true;
        }
        public override bool TryCompareValueTo(object other, out int result) {
            if (other is DateTimeOffset) {
                result = _value.CompareTo((DateTimeOffset)other);
                return true;
            }
            result = 0;
            return false;
        }
        public const string FormatString = "yyyy-MM-ddTHH:mm:ss.FFFFFFFzzz";
        public override bool TryParseAndSet(string literal) {
            DateTimeOffset r;
            if (DateTimeOffset.TryParseExact(literal, FormatString, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None, out r)) {
                _value = r;
                return true;
            }
            return false;
        }
        public override string ToString() {
            return _value.ToString(FormatString, DateTimeFormatInfo.InvariantInfo);
        }
        public override ObjectInfo ObjectInfo { get { return ThisInfo; } }
        new public static readonly AtomTypeInfo ThisInfo = AtomTypeKind.DateTimeOffset.ToAtomTypeInfo(typeof(XDateTimeOffset), XAtomType.ThisInfo);
    }
}
