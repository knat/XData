using System;

namespace XData {
    public class XDecimal : XAtomType {
        public XDecimal() { }
        public XDecimal(decimal value) {
            Value = value;
        }
        public static implicit operator XDecimal(decimal value) {
            return new XDecimal(value);
        }
        public static implicit operator decimal (XDecimal obj) {
            if ((object)obj == null) return 0;
            return obj.Value;
        }
        private decimal _value;
        public virtual decimal GetDecimalValue() {
            return _value;
        }
        public virtual bool SetDecimalValue(decimal value, bool @try = false) {
            _value = value;
            return true;
        }
        public decimal Value {
            get {
                return GetDecimalValue();
            }
            set {
                SetDecimalValue(value);
            }
        }
        public override object GetValue() {
            return Value;
        }
        public override bool Equals(XSimpleType other) {
            if ((object)this == (object)other) return true;
            var otherType = other as XDecimal;
            if ((object)otherType == null) return false;
            return Value == otherType.Value;
        }
        public override int GetHashCode() {
            return Value.GetHashCode();
        }
        public override bool ValueEquals(object other) {
            if (other is decimal) {
                return Value == (decimal)other;
            }
            return false;
        }
        public override bool TryCompareTo(XAtomType other, out int result) {
            result = 0;
            if ((object)this == (object)other) return true;
            var otherType = other as XDecimal;
            if ((object)otherType == null) return false;
            result = Value.CompareTo(otherType.Value);
            return true;
        }
        public override bool TryCompareValueTo(object other, out int result) {
            if (other is decimal) {
                result = Value.CompareTo((decimal)other);
                return true;
            }
            result = 0;
            return false;
        }
        private static byte GetPrecisionAndScale(decimal value, out byte scale) {
            byte p = 0;
            byte s = 0;
            var inFraction = false;
            var inLeadingZero = true;
            var str = value.ToInvString();
            var length = str.Length;
            for (var i = 0; i < length; ++i) {
                var ch = str[i];
                if (inFraction) {
                    ++p;
                    ++s;
                }
                else if (ch == '.') {
                    inLeadingZero = false;
                    inFraction = true;
                }
                else if (ch >= '1' && ch <= '9') {
                    inLeadingZero = false;
                    ++p;
                }
                else if (ch == '0' && !inLeadingZero) {
                    ++p;
                }
            }
            if (inLeadingZero) {
                ++p;
            }
            scale = s;
            return p;
        }
        internal override bool TryGetValuePrecisionAndScale(out byte precision, out byte scale) {
            precision = GetPrecisionAndScale(Value, out scale);
            return true;
        }
        public override bool TryParseAndSet(string literal) {
            decimal r;
            if (!literal.TryToInvDecimal(out r)) {
                return false;
            }
            return SetDecimalValue(r, true);
        }
        public override string ToString() {
            return Value.ToInvString();
        }
        public override ObjectInfo ObjectInfo { get { return ThisInfo; } }
        new public static readonly AtomTypeInfo ThisInfo = TypeKind.Decimal.ToAtomTypeInfo(typeof(XDecimal), XAtomType.ThisInfo);
    }
    public class XInt64 : XDecimal {
        public XInt64() { }
        public XInt64(long value) {
            Value = value;
        }
        public static implicit operator XInt64(long value) {
            return new XInt64(value);
        }
        public static implicit operator long (XInt64 obj) {
            if ((object)obj == null) return 0;
            return obj.Value;
        }
        private long _value;
        public virtual long GetInt64Value() {
            return _value;
        }
        public virtual bool SetInt64Value(long value, bool @try = false) {
            _value = value;
            return true;
        }
        new public long Value {
            get {
                return GetInt64Value();
            }
            set {
                SetInt64Value(value);
            }
        }
        public override object GetValue() {
            return Value;
        }
        public override decimal GetDecimalValue() {
            return Value;
        }
        public override bool SetDecimalValue(decimal value, bool @try = false) {
            if (value >= long.MinValue && value <= long.MaxValue) {
                return SetInt64Value((long)value, @try);
            }
            else if (@try) {
                return false;
            }
            throw new ArgumentOutOfRangeException("value");
        }
        public override bool Equals(XSimpleType other) {
            if ((object)this == (object)other) return true;
            var otherType = other as XInt64;
            if ((object)otherType == null) return false;
            return Value == otherType.Value;
        }
        public override int GetHashCode() {
            return Value.GetHashCode();
        }
        public override bool ValueEquals(object other) {
            if (other is long) {
                return Value == (long)other;
            }
            return false;
        }
        public override bool TryCompareTo(XAtomType other, out int result) {
            result = 0;
            if ((object)this == (object)other) return true;
            var otherType = other as XInt64;
            if ((object)otherType == null) return false;
            result = Value.CompareTo(otherType.Value);
            return true;
        }
        public override bool TryCompareValueTo(object other, out int result) {
            if (other is long) {
                result = Value.CompareTo((long)other);
                return true;
            }
            result = 0;
            return false;
        }
        public override bool TryParseAndSet(string literal) {
            long r;
            if (!literal.TryToInvInt64(out r)) {
                return false;
            }
            return SetInt64Value(r, true);
        }
        public override string ToString() {
            return Value.ToInvString();
        }
        public override ObjectInfo ObjectInfo { get { return ThisInfo; } }
        new public static readonly AtomTypeInfo ThisInfo = TypeKind.Int64.ToAtomTypeInfo(typeof(XInt64), XDecimal.ThisInfo);
    }
    public class XInt32 : XInt64 {
        public XInt32() { }
        public XInt32(int value) {
            Value = value;
        }
        public static implicit operator XInt32(int value) {
            return new XInt32(value);
        }
        public static implicit operator int (XInt32 obj) {
            if ((object)obj == null) return 0;
            return obj.Value;
        }
        private int _value;
        public virtual int GetInt32Value() {
            return _value;
        }
        public virtual bool SetInt32Value(int value, bool @try = false) {
            _value = value;
            return true;
        }
        new public int Value {
            get {
                return GetInt32Value();
            }
            set {
                SetInt32Value(value);
            }
        }
        public override object GetValue() {
            return Value;
        }
        public override long GetInt64Value() {
            return Value;
        }
        public override bool SetInt64Value(long value, bool @try = false) {
            if (value >= int.MinValue && value <= int.MaxValue) {
                return SetInt32Value((int)value, @try);
            }
            else if (@try) {
                return false;
            }
            throw new ArgumentOutOfRangeException("value");
        }
        public override decimal GetDecimalValue() {
            return Value;
        }
        public override bool SetDecimalValue(decimal value, bool @try = false) {
            if (value >= int.MinValue && value <= int.MaxValue) {
                return SetInt32Value((int)value, @try);
            }
            else if (@try) {
                return false;
            }
            throw new ArgumentOutOfRangeException("value");
        }
        public override bool Equals(XSimpleType other) {
            if ((object)this == (object)other) return true;
            var otherType = other as XInt32;
            if ((object)otherType == null) return false;
            return Value == otherType.Value;
        }
        public override int GetHashCode() {
            return Value.GetHashCode();
        }
        public override bool ValueEquals(object other) {
            if (other is int) {
                return Value == (int)other;
            }
            return false;
        }
        public override bool TryCompareTo(XAtomType other, out int result) {
            result = 0;
            if ((object)this == (object)other) return true;
            var otherType = other as XInt32;
            if ((object)otherType == null) return false;
            result = Value.CompareTo(otherType.Value);
            return true;
        }
        public override bool TryCompareValueTo(object other, out int result) {
            if (other is int) {
                result = Value.CompareTo((int)other);
                return true;
            }
            result = 0;
            return false;
        }
        public override bool TryParseAndSet(string literal) {
            int r;
            if (!literal.TryToInvInt32(out r)) {
                return false;
            }
            return SetInt32Value(r, true);
        }
        public override string ToString() {
            return Value.ToInvString();
        }
        public override ObjectInfo ObjectInfo { get { return ThisInfo; } }
        new public static readonly AtomTypeInfo ThisInfo = TypeKind.Int32.ToAtomTypeInfo(typeof(XInt32), XInt64.ThisInfo);
    }
    public class XInt16 : XInt32 {
        public XInt16() { }
        public XInt16(short value) {
            Value = value;
        }
        public static implicit operator XInt16(short value) {
            return new XInt16(value);
        }
        public static implicit operator short (XInt16 obj) {
            if ((object)obj == null) return 0;
            return obj.Value;
        }
        private short _value;
        public virtual short GetInt16Value() {
            return _value;
        }
        public virtual bool SetInt16Value(short value, bool @try = false) {
            _value = value;
            return true;
        }
        new public short Value {
            get {
                return GetInt16Value();
            }
            set {
                SetInt16Value(value);
            }
        }
        public override object GetValue() {
            return Value;
        }
        public override int GetInt32Value() {
            return Value;
        }
        public override bool SetInt32Value(int value, bool @try = false) {
            if (value >= short.MinValue && value <= short.MaxValue) {
                return SetInt16Value((short)value, @try);
            }
            else if (@try) {
                return false;
            }
            throw new ArgumentOutOfRangeException("value");
        }
        public override long GetInt64Value() {
            return Value;
        }
        public override bool SetInt64Value(long value, bool @try = false) {
            if (value >= short.MinValue && value <= short.MaxValue) {
                return SetInt16Value((short)value, @try);
            }
            else if (@try) {
                return false;
            }
            throw new ArgumentOutOfRangeException("value");
        }
        public override decimal GetDecimalValue() {
            return Value;
        }
        public override bool SetDecimalValue(decimal value, bool @try = false) {
            if (value >= short.MinValue && value <= short.MaxValue) {
                return SetInt16Value((short)value, @try);
            }
            else if (@try) {
                return false;
            }
            throw new ArgumentOutOfRangeException("value");
        }
        public override bool Equals(XSimpleType other) {
            if ((object)this == (object)other) return true;
            var otherType = other as XInt16;
            if ((object)otherType == null) return false;
            return Value == otherType.Value;
        }
        public override int GetHashCode() {
            return Value.GetHashCode();
        }
        public override bool ValueEquals(object other) {
            if (other is short) {
                return Value == (short)other;
            }
            return false;
        }
        public override bool TryCompareTo(XAtomType other, out int result) {
            result = 0;
            if ((object)this == (object)other) return true;
            var otherType = other as XInt16;
            if ((object)otherType == null) return false;
            result = Value.CompareTo(otherType.Value);
            return true;
        }
        public override bool TryCompareValueTo(object other, out int result) {
            if (other is short) {
                result = Value.CompareTo((short)other);
                return true;
            }
            result = 0;
            return false;
        }
        public override bool TryParseAndSet(string literal) {
            short r;
            if (!literal.TryToInvInt16(out r)) {
                return false;
            }
            return SetInt16Value(r, true);
        }
        public override string ToString() {
            return Value.ToInvString();
        }
        public override ObjectInfo ObjectInfo { get { return ThisInfo; } }
        new public static readonly AtomTypeInfo ThisInfo = TypeKind.Int16.ToAtomTypeInfo(typeof(XInt16), XInt32.ThisInfo);
    }
    public class XSByte : XInt16 {
        public XSByte() { }
        public XSByte(sbyte value) {
            Value = value;
        }
        public static implicit operator XSByte(sbyte value) {
            return new XSByte(value);
        }
        public static implicit operator sbyte (XSByte obj) {
            if ((object)obj == null) return 0;
            return obj.Value;
        }
        private sbyte _value;
        public virtual sbyte GetSByteValue() {
            return _value;
        }
        public virtual bool SetSByteValue(sbyte value, bool @try = false) {
            _value = value;
            return true;
        }
        new public sbyte Value {
            get {
                return GetSByteValue();
            }
            set {
                SetSByteValue(value);
            }
        }
        public override object GetValue() {
            return Value;
        }
        public override short GetInt16Value() {
            return Value;
        }
        public override bool SetInt16Value(short value, bool @try = false) {
            if (value >= sbyte.MinValue && value <= sbyte.MaxValue) {
                return SetSByteValue((sbyte)value, @try);
            }
            else if (@try) {
                return false;
            }
            throw new ArgumentOutOfRangeException("value");
        }
        public override int GetInt32Value() {
            return Value;
        }
        public override bool SetInt32Value(int value, bool @try = false) {
            if (value >= sbyte.MinValue && value <= sbyte.MaxValue) {
                return SetSByteValue((sbyte)value, @try);
            }
            else if (@try) {
                return false;
            }
            throw new ArgumentOutOfRangeException("value");
        }
        public override long GetInt64Value() {
            return Value;
        }
        public override bool SetInt64Value(long value, bool @try = false) {
            if (value >= sbyte.MinValue && value <= sbyte.MaxValue) {
                return SetSByteValue((sbyte)value, @try);
            }
            else if (@try) {
                return false;
            }
            throw new ArgumentOutOfRangeException("value");
        }
        public override decimal GetDecimalValue() {
            return Value;
        }
        public override bool SetDecimalValue(decimal value, bool @try = false) {
            if (value >= sbyte.MinValue && value <= sbyte.MaxValue) {
                return SetSByteValue((sbyte)value, @try);
            }
            else if (@try) {
                return false;
            }
            throw new ArgumentOutOfRangeException("value");
        }
        public override bool Equals(XSimpleType other) {
            if ((object)this == (object)other) return true;
            var otherType = other as XSByte;
            if ((object)otherType == null) return false;
            return Value == otherType.Value;
        }
        public override int GetHashCode() {
            return Value.GetHashCode();
        }
        public override bool ValueEquals(object other) {
            if (other is sbyte) {
                return Value == (sbyte)other;
            }
            return false;
        }
        public override bool TryCompareTo(XAtomType other, out int result) {
            result = 0;
            if ((object)this == (object)other) return true;
            var otherType = other as XSByte;
            if ((object)otherType == null) return false;
            result = Value.CompareTo(otherType.Value);
            return true;
        }
        public override bool TryCompareValueTo(object other, out int result) {
            if (other is sbyte) {
                result = Value.CompareTo((sbyte)other);
                return true;
            }
            result = 0;
            return false;
        }
        public override bool TryParseAndSet(string literal) {
            sbyte r;
            if (!literal.TryToInvSByte(out r)) {
                return false;
            }
            return SetSByteValue(r, true);
        }
        public override string ToString() {
            return Value.ToInvString();
        }
        public override ObjectInfo ObjectInfo { get { return ThisInfo; } }
        new public static readonly AtomTypeInfo ThisInfo = TypeKind.SByte.ToAtomTypeInfo(typeof(XSByte), XInt16.ThisInfo);
    }

    public class XUInt64 : XDecimal {
        public XUInt64() { }
        public XUInt64(ulong value) {
            Value = value;
        }
        public static implicit operator XUInt64(ulong value) {
            return new XUInt64(value);
        }
        public static implicit operator ulong (XUInt64 obj) {
            if ((object)obj == null) return 0;
            return obj.Value;
        }
        private ulong _value;
        public virtual ulong GetUInt64Value() {
            return _value;
        }
        public virtual bool SetUInt64Value(ulong value, bool @try = false) {
            _value = value;
            return true;
        }
        new public ulong Value {
            get {
                return GetUInt64Value();
            }
            set {
                SetUInt64Value(value);
            }
        }
        public override object GetValue() {
            return Value;
        }
        public override decimal GetDecimalValue() {
            return Value;
        }
        public override bool SetDecimalValue(decimal value, bool @try = false) {
            if (value >= ulong.MinValue && value <= ulong.MaxValue) {
                return SetUInt64Value((ulong)value, @try);
            }
            else if (@try) {
                return false;
            }
            throw new ArgumentOutOfRangeException("value");
        }
        public override bool Equals(XSimpleType other) {
            if ((object)this == (object)other) return true;
            var otherType = other as XUInt64;
            if ((object)otherType == null) return false;
            return Value == otherType.Value;
        }
        public override int GetHashCode() {
            return Value.GetHashCode();
        }
        public override bool ValueEquals(object other) {
            if (other is ulong) {
                return Value == (ulong)other;
            }
            return false;
        }
        public override bool TryCompareTo(XAtomType other, out int result) {
            result = 0;
            if ((object)this == (object)other) return true;
            var otherType = other as XUInt64;
            if ((object)otherType == null) return false;
            result = Value.CompareTo(otherType.Value);
            return true;
        }
        public override bool TryCompareValueTo(object other, out int result) {
            if (other is ulong) {
                result = Value.CompareTo((ulong)other);
                return true;
            }
            result = 0;
            return false;
        }
        public override bool TryParseAndSet(string literal) {
            ulong r;
            if (!literal.TryToInvUInt64(out r)) {
                return false;
            }
            return SetUInt64Value(r, true);
        }
        public override string ToString() {
            return Value.ToInvString();
        }
        public override ObjectInfo ObjectInfo { get { return ThisInfo; } }
        new public static readonly AtomTypeInfo ThisInfo = TypeKind.UInt64.ToAtomTypeInfo(typeof(XUInt64), XDecimal.ThisInfo);
    }
    public class XUInt32 : XUInt64 {
        public XUInt32() { }
        public XUInt32(uint value) {
            Value = value;
        }
        public static implicit operator XUInt32(uint value) {
            return new XUInt32(value);
        }
        public static implicit operator uint (XUInt32 obj) {
            if ((object)obj == null) return 0;
            return obj.Value;
        }
        private uint _value;
        public virtual uint GetUInt32Value() {
            return _value;
        }
        public virtual bool SetUInt32Value(uint value, bool @try = false) {
            _value = value;
            return true;
        }
        new public uint Value {
            get {
                return GetUInt32Value();
            }
            set {
                SetUInt32Value(value);
            }
        }
        public override object GetValue() {
            return Value;
        }
        public override ulong GetUInt64Value() {
            return Value;
        }
        public override bool SetUInt64Value(ulong value, bool @try = false) {
            if (value >= uint.MinValue && value <= uint.MaxValue) {
                return SetUInt32Value((uint)value, @try);
            }
            else if (@try) {
                return false;
            }
            throw new ArgumentOutOfRangeException("value");
        }
        public override decimal GetDecimalValue() {
            return Value;
        }
        public override bool SetDecimalValue(decimal value, bool @try = false) {
            if (value >= uint.MinValue && value <= uint.MaxValue) {
                return SetUInt32Value((uint)value, @try);
            }
            else if (@try) {
                return false;
            }
            throw new ArgumentOutOfRangeException("value");
        }
        public override bool Equals(XSimpleType other) {
            if ((object)this == (object)other) return true;
            var otherType = other as XInt32;
            if ((object)otherType == null) return false;
            return Value == otherType.Value;
        }
        public override int GetHashCode() {
            return Value.GetHashCode();
        }
        public override bool ValueEquals(object other) {
            if (other is int) {
                return Value == (int)other;
            }
            return false;
        }
        public override bool TryCompareTo(XAtomType other, out int result) {
            result = 0;
            if ((object)this == (object)other) return true;
            var otherType = other as XUInt32;
            if ((object)otherType == null) return false;
            result = Value.CompareTo(otherType.Value);
            return true;
        }
        public override bool TryCompareValueTo(object other, out int result) {
            if (other is uint) {
                result = Value.CompareTo((uint)other);
                return true;
            }
            result = 0;
            return false;
        }
        public override bool TryParseAndSet(string literal) {
            uint r;
            if (!literal.TryToInvUInt32(out r)) {
                return false;
            }
            return SetUInt32Value(r, true);
        }
        public override string ToString() {
            return Value.ToInvString();
        }
        public override ObjectInfo ObjectInfo { get { return ThisInfo; } }
        new public static readonly AtomTypeInfo ThisInfo = TypeKind.UInt32.ToAtomTypeInfo(typeof(XUInt32), XUInt64.ThisInfo);
    }
    public class XUInt16 : XUInt32 {
        public XUInt16() { }
        public XUInt16(ushort value) {
            Value = value;
        }
        public static implicit operator XUInt16(ushort value) {
            return new XUInt16(value);
        }
        public static implicit operator ushort (XUInt16 obj) {
            if ((object)obj == null) return 0;
            return obj.Value;
        }
        private ushort _value;
        public virtual ushort GetUInt16Value() {
            return _value;
        }
        public virtual bool SetUInt16Value(ushort value, bool @try = false) {
            _value = value;
            return true;
        }
        new public ushort Value {
            get {
                return GetUInt16Value();
            }
            set {
                SetUInt16Value(value);
            }
        }
        public override object GetValue() {
            return Value;
        }
        public override uint GetUInt32Value() {
            return Value;
        }
        public override bool SetUInt32Value(uint value, bool @try = false) {
            if (value >= ushort.MinValue && value <= ushort.MaxValue) {
                return SetUInt16Value((ushort)value, @try);
            }
            else if (@try) {
                return false;
            }
            throw new ArgumentOutOfRangeException("value");
        }
        public override ulong GetUInt64Value() {
            return Value;
        }
        public override bool SetUInt64Value(ulong value, bool @try = false) {
            if (value >= ushort.MinValue && value <= ushort.MaxValue) {
                return SetUInt16Value((ushort)value, @try);
            }
            else if (@try) {
                return false;
            }
            throw new ArgumentOutOfRangeException("value");
        }
        public override decimal GetDecimalValue() {
            return Value;
        }
        public override bool SetDecimalValue(decimal value, bool @try = false) {
            if (value >= ushort.MinValue && value <= ushort.MaxValue) {
                return SetUInt16Value((ushort)value, @try);
            }
            else if (@try) {
                return false;
            }
            throw new ArgumentOutOfRangeException("value");
        }
        public override bool Equals(XSimpleType other) {
            if ((object)this == (object)other) return true;
            var otherType = other as XUInt16;
            if ((object)otherType == null) return false;
            return Value == otherType.Value;
        }
        public override int GetHashCode() {
            return Value.GetHashCode();
        }
        public override bool ValueEquals(object other) {
            if (other is ushort) {
                return Value == (ushort)other;
            }
            return false;
        }
        public override bool TryCompareTo(XAtomType other, out int result) {
            result = 0;
            if ((object)this == (object)other) return true;
            var otherType = other as XUInt16;
            if ((object)otherType == null) return false;
            result = Value.CompareTo(otherType.Value);
            return true;
        }
        public override bool TryCompareValueTo(object other, out int result) {
            if (other is ushort) {
                result = Value.CompareTo((ushort)other);
                return true;
            }
            result = 0;
            return false;
        }
        public override bool TryParseAndSet(string literal) {
            ushort r;
            if (!literal.TryToInvUInt16(out r)) {
                return false;
            }
            return SetUInt16Value(r, true);
        }
        public override string ToString() {
            return Value.ToInvString();
        }
        public override ObjectInfo ObjectInfo { get { return ThisInfo; } }
        new public static readonly AtomTypeInfo ThisInfo = TypeKind.UInt16.ToAtomTypeInfo(typeof(XUInt16), XUInt32.ThisInfo);
    }
    public class XByte : XUInt16 {
        public XByte() { }
        public XByte(byte value) {
            Value = value;
        }
        public static implicit operator XByte(byte value) {
            return new XByte(value);
        }
        public static implicit operator byte (XByte obj) {
            if ((object)obj == null) return 0;
            return obj.Value;
        }
        private byte _value;
        public virtual byte GetByteValue() {
            return _value;
        }
        public virtual bool SetByteValue(byte value, bool @try = false) {
            _value = value;
            return true;
        }
        new public byte Value {
            get {
                return GetByteValue();
            }
            set {
                SetByteValue(value);
            }
        }
        public override object GetValue() {
            return Value;
        }
        public override ushort GetUInt16Value() {
            return Value;
        }
        public override bool SetUInt16Value(ushort value, bool @try = false) {
            if (value >= byte.MinValue && value <= byte.MaxValue) {
                return SetByteValue((byte)value, @try);
            }
            else if (@try) {
                return false;
            }
            throw new ArgumentOutOfRangeException("value");
        }
        public override uint GetUInt32Value() {
            return Value;
        }
        public override bool SetUInt32Value(uint value, bool @try = false) {
            if (value >= byte.MinValue && value <= byte.MaxValue) {
                return SetByteValue((byte)value, @try);
            }
            else if (@try) {
                return false;
            }
            throw new ArgumentOutOfRangeException("value");
        }
        public override ulong GetUInt64Value() {
            return Value;
        }
        public override bool SetUInt64Value(ulong value, bool @try = false) {
            if (value >= byte.MinValue && value <= byte.MaxValue) {
                return SetByteValue((byte)value, @try);
            }
            else if (@try) {
                return false;
            }
            throw new ArgumentOutOfRangeException("value");
        }
        public override decimal GetDecimalValue() {
            return Value;
        }
        public override bool SetDecimalValue(decimal value, bool @try = false) {
            if (value >= byte.MinValue && value <= byte.MaxValue) {
                return SetByteValue((byte)value, @try);
            }
            else if (@try) {
                return false;
            }
            throw new ArgumentOutOfRangeException("value");
        }
        public override bool Equals(XSimpleType other) {
            if ((object)this == (object)other) return true;
            var otherType = other as XByte;
            if ((object)otherType == null) return false;
            return Value == otherType.Value;
        }
        public override int GetHashCode() {
            return Value.GetHashCode();
        }
        public override bool ValueEquals(object other) {
            if (other is byte) {
                return Value == (byte)other;
            }
            return false;
        }
        public override bool TryCompareTo(XAtomType other, out int result) {
            result = 0;
            if ((object)this == (object)other) return true;
            var otherType = other as XByte;
            if ((object)otherType == null) return false;
            result = Value.CompareTo(otherType.Value);
            return true;
        }
        public override bool TryCompareValueTo(object other, out int result) {
            if (other is byte) {
                result = Value.CompareTo((byte)other);
                return true;
            }
            result = 0;
            return false;
        }
        public override bool TryParseAndSet(string literal) {
            byte r;
            if (!literal.TryToInvByte(out r)) {
                return false;
            }
            return SetByteValue(r, true);
        }
        public override string ToString() {
            return Value.ToInvString();
        }
        public override ObjectInfo ObjectInfo { get { return ThisInfo; } }
        new public static readonly AtomTypeInfo ThisInfo = TypeKind.Byte.ToAtomTypeInfo(typeof(XByte), XUInt16.ThisInfo);
    }

}
