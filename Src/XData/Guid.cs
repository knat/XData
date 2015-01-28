using System;
using XData.IO.Text;

namespace XData {
    public class XGuid : XAtomType {
        public XGuid() { }
        public XGuid(Guid value) {
            _value = value;
        }
        public static implicit operator XGuid(Guid value) {
            return new XGuid(value);
        }
        public static implicit operator Guid(XGuid obj) {
            if ((object)obj == null) return default(Guid);
            return obj._value;
        }
        private Guid _value;
        public Guid Value {
            get {
                return _value;
            }
            set {
                _value = value;
            }
        }
        public override bool Equals(XSimpleType other) {
            if ((object)this == other) return true;
            var otherType = other as XGuid;
            if ((object)otherType == null) return false;
            return _value == otherType._value;
        }
        public override int GetHashCode() {
            return _value.GetHashCode();
        }
        public override bool ValueEquals(object other) {
            if (other is Guid) {
                return _value == (Guid)other;
            }
            return false;
        }
        public override bool TryCompareTo(XAtomType other, out int result) {
            result = 0;
            if ((object)this == other) return true;
            var otherType = other as XGuid;
            if ((object)otherType == null) return false;
            result = _value.CompareTo(otherType._value);
            return true;
        }
        public override bool TryCompareValueTo(object other, out int result) {
            if (other is Guid) {
                result = _value.CompareTo((Guid)other);
                return true;
            }
            result = 0;
            return false;
        }
        public override bool TryParseAndSet(string literal) {
            Guid r;
            if (Guid.TryParseExact(literal, "D", out r)) {
                _value = r;
                return true;
            }
            return false;
        }
        public override string ToString() {
            return _value.ToString("D");
        }
        public override sealed void SaveValue(IndentedStringBuilder sb) {
            sb.Append('"');
            sb.StringBuilder.Append(ToString());
            sb.StringBuilder.Append('"');
        }
        public override ObjectInfo ObjectInfo { get { return ThisInfo; } }
        new public static readonly AtomTypeInfo ThisInfo = TypeKind.Guid.ToAtomTypeInfo(typeof(XGuid), XAtomType.ThisInfo);
    }
}
