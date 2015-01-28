namespace XData {
    public class XBoolean : XAtomType {
        public XBoolean() { }
        public XBoolean(bool value) {
            _value = value;
        }
        public static implicit operator XBoolean(bool value) {
            return new XBoolean(value);
        }
        public static implicit operator bool (XBoolean obj) {
            if ((object)obj == null) return false;
            return obj._value;
        }
        private bool _value;
        public bool Value {
            get {
                return _value;
            }
            set {
                _value = value;
            }
        }
        public override bool Equals(XSimpleType other) {
            if ((object)this == other) return true;
            var otherType = other as XBoolean;
            if ((object)otherType == null) return false;
            return _value == otherType._value;
        }
        public override int GetHashCode() {
            return _value.GetHashCode();
        }
        public override bool ValueEquals(object other) {
            if (other is bool) {
                return _value == (bool)other;
            }
            return false;
        }
        public override bool TryCompareTo(XAtomType other, out int result) {
            result = 0;
            if ((object)this == other) return true;
            var otherType = other as XBoolean;
            if ((object)otherType == null) return false;
            result = _value.CompareTo(otherType._value);
            return true;
        }
        public override bool TryCompareValueTo(object other, out int result) {
            if (other is bool) {
                result = _value.CompareTo((bool)other);
                return true;
            }
            result = 0;
            return false;
        }
        public override bool TryParseAndSet(string literal) {
            bool r;
            if (literal == "true") {
                r = true;
            }
            else if (literal == "false") {
                r = false;
            }
            else {
                return false;
            }
            _value = r;
            return true;
        }
        public override string ToString() {
            return _value ? "true" : "false";
        }
        public override ObjectInfo ObjectInfo { get { return ThisInfo; } }
        new public static readonly AtomTypeInfo ThisInfo = TypeKind.Boolean.ToAtomTypeInfo(typeof(XBoolean), XAtomType.ThisInfo);
    }
}
