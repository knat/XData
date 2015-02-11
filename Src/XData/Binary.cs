using System;
using XData.IO.Text;

namespace XData {
    public class XBinary : XAtomType {
        public XBinary() {
            _value = _emptyValue;
        }
        public XBinary(byte[] value) {
            Value = value;
        }
        public static implicit operator XBinary(byte[] value) {
            return new XBinary(value);
        }
        public static implicit operator byte[] (XBinary obj) {
            if ((object)obj == null) return _emptyValue;
            return obj._value;
        }
        private static readonly byte[] _emptyValue = new byte[0];
        private byte[] _value;
        public byte[] Value {
            get {
                return _value;
            }
            set {
                _value = value ?? _emptyValue;
            }
        }
        public override object GetValue() {
            return _value;
        }
        public override XObject DeepClone() {
            var obj = (XBinary)base.DeepClone();
            if (_value.Length > 0) {
                obj._value = (byte[])_value.Clone();
            }
            return obj;
        }
        private static bool ValueEquals(byte[] x, byte[] y) {
            if (x == y) {
                return true;
            }
            var xLength = x.Length;
            if (xLength != y.Length) {
                return false;
            }
            for (var i = 0; i < xLength; ++i) {
                if (x[i] != y[i]) {
                    return false;
                }
            }
            return true;
        }
        public override bool Equals(XSimpleType other) {
            if ((object)this == (object)other) return true;
            var otherType = other as XBinary;
            if ((object)otherType == null) return false;
            return ValueEquals(_value, otherType._value);
        }
        public override int GetHashCode() {
            var hash = 17;
            var count = Math.Min(_value.Length, 7);
            for (var i = 0; i < count; ++i) {
                hash = Extensions.AggregateHash(hash, _value[i]);
            }
            return hash;
        }
        public override bool ValueEquals(object other) {
            var otherValue = other as byte[];
            if (otherValue == null) {
                return false;
            }
            return ValueEquals(_value, otherValue);
        }

        public override bool TryParseAndSet(string literal) {
            try {
                _value = Convert.FromBase64String(literal);
                return true;
            }
            catch (FormatException) {
                return false;
            }
        }
        public override string ToString() {
            if (_value.Length == 0) return string.Empty;
            return Convert.ToBase64String(_value);
        }
        internal override bool TryGetValueLength(out ulong result) {
            result = (ulong)_value.Length;
            return true;
        }
        internal override sealed void SaveValue(SavingContext context) {
            var sb = context.StringBuilder;
            sb.Append('"');
            sb.Append(ToString());
            sb.Append('"');
        }
        public override ObjectInfo ObjectInfo { get { return ThisInfo; } }
        new public static readonly AtomTypeInfo ThisInfo = TypeKind.Binary.ToAtomTypeInfo(typeof(XBinary), XAtomType.ThisInfo);
    }
}
