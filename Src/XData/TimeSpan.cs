using System;
using System.Globalization;
using XData.IO.Text;

namespace XData {
    public class XTimeSpan : XAtomType {
        public XTimeSpan() { }
        public XTimeSpan(TimeSpan value) {
            _value = value;
        }
        public static implicit operator XTimeSpan(TimeSpan value) {
            return new XTimeSpan(value);
        }
        public static implicit operator TimeSpan(XTimeSpan obj) {
            if ((object)obj == null) return default(TimeSpan);
            return obj._value;
        }
        private TimeSpan _value;
        public TimeSpan Value {
            get {
                return _value;
            }
            set {
                _value = value;
            }
        }
        public override bool Equals(XSimpleType other) {
            if ((object)this == other) return true;
            var otherType = other as XTimeSpan;
            if ((object)otherType == null) return false;
            return _value == otherType._value;
        }
        public override int GetHashCode() {
            return _value.GetHashCode();
        }
        public override bool ValueEquals(object other) {
            if (other is TimeSpan) {
                return _value == (TimeSpan)other;
            }
            return false;
        }
        public override bool TryCompareTo(XAtomType other, out int result) {
            result = 0;
            if ((object)this == other) return true;
            var otherType = other as XTimeSpan;
            if ((object)otherType == null) return false;
            result = _value.CompareTo(otherType._value);
            return true;
        }
        public override bool TryCompareValueTo(object other, out int result) {
            if (other is TimeSpan) {
                result = _value.CompareTo((TimeSpan)other);
                return true;
            }
            result = 0;
            return false;
        }
        public override bool TryParseAndSet(string literal) {
            TimeSpan r;
            if (TimeSpan.TryParseExact(literal, "c", DateTimeFormatInfo.InvariantInfo, out r)) {
                _value = r;
                return true;
            }
            return false;
        }
        public override string ToString() {
            return _value.ToString("c");
        }
        public override void SaveValue(IndentedStringBuilder isb) {
            isb.Append('"');
            isb.StringBuilder.Append(ToString());
            isb.StringBuilder.Append('"');
        }
        public override ObjectInfo ObjectInfo { get { return ThisInfo; } }
        new public static readonly AtomTypeInfo ThisInfo = TypeKind.TimeSpan.ToAtomTypeInfo(typeof(XTimeSpan), XAtomType.ThisInfo);
    }
}
