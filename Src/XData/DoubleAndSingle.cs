using System;
using System.Globalization;

namespace XData {
    public class XDouble : XAtomType {
        public XDouble() { }
        public XDouble(double value) {
            Value = value;
        }
        public static implicit operator XDouble(double value) {
            return new XDouble(value);
        }
        public static implicit operator double (XDouble obj) {
            if ((object)obj == null) return 0;
            return obj.Value;
        }
        private double _value;
        public virtual double GetDoubleValue() {
            return _value;
        }
        public virtual bool SetDoubleValue(double value, bool @try = false) {
            _value = value;
            return true;
        }
        public double Value {
            get {
                return GetDoubleValue();
            }
            set {
                SetDoubleValue(value);
            }
        }
        public override bool Equals(XSimpleType other) {
            if ((object)this == other) return true;
            var otherType = other as XDouble;
            if ((object)otherType == null) return false;
            return Value == otherType.Value;
        }
        public override int GetHashCode() {
            return Value.GetHashCode();
        }
        public override bool ValueEquals(object other) {
            if (other is double) {
                return Value == (double)other;
            }
            return false;
        }
        public override bool TryCompareTo(XAtomType other, out int result) {
            result = 0;
            if ((object)this == other) return true;
            var otherType = other as XDouble;
            if ((object)otherType == null) return false;
            result = Value.CompareTo(otherType.Value);
            return true;
        }
        public override bool TryCompareValueTo(object other, out int result) {
            if (other is double) {
                result = Value.CompareTo((double)other);
                return true;
            }
            result = 0;
            return false;
        }
        public override bool TryParseAndSet(string literal) {
            double r;
            if (literal == "INF") {
                r = double.PositiveInfinity;
            }
            else if (literal == "-INF") {
                r = double.NegativeInfinity;
            }
            else if (!double.TryParse(literal, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent, NumberFormatInfo.InvariantInfo, out r)) {
                return false;
            }
            return SetDoubleValue(r);
        }
        public override string ToString() {
            var value = Value;
            if (double.IsPositiveInfinity(value)) {
                return "INF";
            }
            if (double.IsNegativeInfinity(value)) {
                return "-INF";
            }
            return value.ToString(NumberFormatInfo.InvariantInfo);
        }
        public override ObjectInfo ObjectInfo { get { return ThisInfo; } }
        new public static readonly AtomTypeInfo ThisInfo = AtomTypeKind.Double.ToAtomTypeInfo(typeof(XDouble), XAtomType.ThisInfo);
    }
    public class XSingle : XDouble {
        public XSingle() { }
        public XSingle(float value) {
            Value = value;
        }
        public static implicit operator XSingle(float value) {
            return new XSingle(value);
        }
        public static implicit operator float (XSingle obj) {
            if ((object)obj == null) return 0;
            return obj.Value;
        }
        private float _value;
        public virtual float GetSingleValue() {
            return _value;
        }
        public virtual bool SetSingleValue(float value, bool @try = false) {
            _value = value;
            return true;
        }
        new public float Value {
            get {
                return GetSingleValue();
            }
            set {
                SetSingleValue(value);
            }
        }
        public override double GetDoubleValue() {
            return Value;
        }
        public override bool SetDoubleValue(double value, bool @try = false) {
            float f;
            if (double.IsPositiveInfinity(value)) {
                f = float.PositiveInfinity;
            }
            else if (double.IsNegativeInfinity(value)) {
                f = float.NegativeInfinity;
            }
            else {
                f = (float)value;
                if (float.IsPositiveInfinity(f) || float.IsNegativeInfinity(f)) {
                    if (@try) {
                        return false;
                    }
                    throw new ArgumentOutOfRangeException("value");
                }
            }
            return SetSingleValue(f);
        }
        public override bool Equals(XSimpleType other) {
            if ((object)this == other) return true;
            var otherType = other as XSingle;
            if ((object)otherType == null) return false;
            return Value == otherType.Value;
        }
        public override int GetHashCode() {
            return Value.GetHashCode();
        }
        public override bool ValueEquals(object other) {
            if (other is float) {
                return Value == (float)other;
            }
            return false;
        }
        public override bool TryCompareTo(XAtomType other, out int result) {
            result = 0;
            if ((object)this == other) return true;
            var otherType = other as XSingle;
            if ((object)otherType == null) return false;
            result = Value.CompareTo(otherType.Value);
            return true;
        }
        public override bool TryCompareValueTo(object other, out int result) {
            if (other is float) {
                result = Value.CompareTo((float)other);
                return true;
            }
            result = 0;
            return false;
        }
        public override bool TryParseAndSet(string literal) {
            float r;
            if (literal == "INF") {
                r = float.PositiveInfinity;
            }
            else if (literal == "-INF") {
                r = float.NegativeInfinity;
            }
            else if (!float.TryParse(literal, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent, NumberFormatInfo.InvariantInfo, out r)) {
                return false;
            }
            return SetSingleValue(r);
        }
        public override string ToString() {
            var value = Value;
            if (float.IsPositiveInfinity(value)) {
                return "INF";
            }
            if (float.IsNegativeInfinity(value)) {
                return "-INF";
            }
            return value.ToString(NumberFormatInfo.InvariantInfo);
        }
        public override ObjectInfo ObjectInfo { get { return ThisInfo; } }
        new public static readonly AtomTypeInfo ThisInfo = AtomTypeKind.Single.ToAtomTypeInfo(typeof(XSingle), XDouble.ThisInfo);
    }
}
