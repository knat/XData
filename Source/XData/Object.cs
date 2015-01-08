using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using XData.TextIO;

namespace XData {
    [Serializable]
    public abstract class XObject {
        protected XObject() { }
        private XObject _parent;
        public XObject Parent {
            get {
                return _parent;
            }
        }
        public bool HasParent {
            get {
                return _parent != null;
            }
        }
        private XObject SetParent(XObject parent) {
            if (parent == null) {
                throw new ArgumentNullException("parent");
            }
            for (var i = parent; i != null; i = i._parent) {
                if (object.ReferenceEquals(this, i)) {
                    throw new InvalidOperationException("Circular reference detected");
                }
            }
            XObject obj;
            if (_parent == null) {
                obj = this;
            }
            else {
                obj = DeepClone();
            }
            obj._parent = parent;
            return obj;
        }
        protected T SetParentTo<T>(T obj, bool allowNull = true) where T : XObject {
            if (obj == null) {
                if (!allowNull) {
                    throw new ArgumentNullException("obj");
                }
                return null;
            }
            return (T)obj.SetParent(this);
        }
        public T GetAncestor<T>(bool @try = true, bool testSelf = false) where T : class {
            for (var obj = testSelf ? this : _parent; obj != null; obj = obj._parent) {
                var res = obj as T;
                if (res != null) {
                    return res;
                }
            }
            if (!@try) {
                throw new InvalidOperationException("Cannot get ancestor '{0}'.".InvFormat(typeof(T).FullName));
            }
            return null;
        }
        //
        public virtual XObject DeepClone() {
            var obj = (XObject)MemberwiseClone();
            obj._parent = null;
            return obj;
        }
        public T DeepClone<T>() where T : XObject {
            return (T)DeepClone();
        }
        public abstract ObjectInfo ObjectInfo { get; }
        //public static readonly ObjectInfo ThisInfo = new ObjectInfo(typeof(XObject));
        //public bool IsSpecific {
        //    get {
        //        return ObjectInfo != null;
        //    }
        //}
        public bool TryValidate(Context context) {
            if (context == null) {
                throw new ArgumentNullException("context");
            }
            var success = TryValidating(context, true);
            if (success) {
                success = TryValidateCore(context);
            }
            return TryValidated(context, success);
        }
        protected virtual bool TryValidating(Context context, bool fromValidate) {
            return true;
        }
        protected virtual bool TryValidateCore(Context context) {
            return true;
        }
        protected virtual bool TryValidated(Context context, bool success) {
            return success;
        }
        internal bool InvokeTryValidatePair(Context context) {
            return TryValidated(context, TryValidating(context, false));
        }
    }

    [Serializable]
    public abstract class XType : XObject {
        protected XType() { }
        public TypeInfo TypeInfo {
            get {
                return (TypeInfo)ObjectInfo;
            }
        }
        public static readonly TypeInfo ThisInfo = new TypeInfo(typeof(XType), TypeKind.Type.ToFullName(), TypeKind.Type, null);
        //
        internal static TypeInfo GetTypeInfo(Context context, ProgramInfo programInfo, QualifiableNameNode typeQName,
            TypeInfo declTypeInfo, TextSpan declTypeTextSpan) {
            TypeInfo typeInfo = null;
            if (typeQName.IsValid) {
                var typeFullName = typeQName.FullName;
                typeInfo = programInfo.TryGetGlobalObject(typeFullName) as TypeInfo;
                if (typeInfo == null) {
                    context.AddErrorDiagnostic(DiagnosticCode.InvalidTypeName, "Invalid type name '{0}'.".InvFormat(typeFullName.ToString()),
                        typeQName.TextSpan);
                    return null;
                }
                if (!declTypeInfo.IsAssignableFrom(typeInfo)) {
                    context.AddErrorDiagnostic(DiagnosticCode.TypeDoesNotEqualToOrDeriveFrom,
                        "Type '{0}' does not equal to or derive from '{1}'.".InvFormat(typeFullName.ToString(), declTypeInfo.FullName.ToString()),
                        typeQName.TextSpan);
                    return null;
                }
            }
            var effTypeInfo = typeInfo ?? declTypeInfo;
            if (effTypeInfo.IsAbstract) {
                context.AddErrorDiagnostic(DiagnosticCode.TypeIsAbstract, "Type '{0}' is abstract.".InvFormat(effTypeInfo.FullName.ToString()),
                    typeInfo != null ? typeQName.TextSpan : declTypeTextSpan);
                return null;
            }
            return effTypeInfo;
        }

    }

    [Serializable]
    public abstract class XSimpleType : XType, IEquatable<XSimpleType> {
        protected XSimpleType() { }
        private object _value;
        public object Value {
            get { return _value; }
            set {
                var valueObj = value as XObject;
                if (valueObj != null) {
                    _value = SetParentTo(valueObj);
                }
                else {
                    _value = value;
                }
            }
        }
        public object GenericValue {
            get {
                return _value;
            }
            set {
                Value = value;
            }
        }
        public bool HasValue {
            get {
                return _value != null;
            }
        }
        public override XObject DeepClone() {
            var obj = (XSimpleType)base.DeepClone();
            obj.Value = _value;
            return obj;
        }
        [ThreadStatic]
        private static IEqualityComparer _valueEqualityComparer;
        public static IEqualityComparer ValueEqualityComparer {
            get {
                return _valueEqualityComparer ?? (_valueEqualityComparer = DefaultValueComparer.Instance);
            }
            set {
                _valueEqualityComparer = value;
            }
        }
        [ThreadStatic]
        private static IComparer _valueComparer;
        public static IComparer ValueComparer {
            get {
                return _valueComparer ?? (_valueComparer = DefaultValueComparer.Instance);
            }
            set {
                _valueComparer = value;
            }
        }
        public sealed class DefaultValueComparer : IEqualityComparer, IComparer {
            private DefaultValueComparer() { }
            public static readonly DefaultValueComparer Instance = new DefaultValueComparer();
            new public bool Equals(object x, object y) {
                return object.Equals(x, y);
            }
            public int GetHashCode(object obj) {
                if (obj == null) return 0;
                return obj.GetHashCode();
            }
            public int Compare(object x, object y) {
                return Comparer.Default.Compare(x, y);
            }
        }
        public virtual bool Equals(XSimpleType other) {
            if (object.ReferenceEquals(this, other)) return true;
            if (object.ReferenceEquals(other, null)) return false;
            return ValueEqualityComparer.Equals(_value, other._value);
        }
        public override sealed bool Equals(object obj) {
            return Equals(obj as XSimpleType);
        }
        public override int GetHashCode() {
            return ValueEqualityComparer.GetHashCode(_value);
        }
        public static bool operator ==(XSimpleType left, XSimpleType right) {
            if (object.ReferenceEquals(left, null)) {
                return object.ReferenceEquals(right, null);
            }
            return left.Equals(right);
        }
        public static bool operator !=(XSimpleType left, XSimpleType right) {
            return !(left == right);
        }


        public SimpleTypeInfo SimpleTypeInfo {
            get {
                return (SimpleTypeInfo)ObjectInfo;
            }
        }
        new public static readonly SimpleTypeInfo ThisInfo = new SimpleTypeInfo(typeof(XSimpleType), TypeKind.SimpleType.ToFullName(), TypeKind.SimpleType, XType.ThisInfo, typeof(object), null);
        //
        internal static bool TryCreate(Context context, SimpleTypeInfo simpleTypeInfo, ProgramInfo programInfo, bool isNullable, SimpleValueNode simpleValueNode, out XSimpleType result) {
            result = null;
            var effSimpleTypeInfo = (SimpleTypeInfo)GetTypeInfo(context, programInfo, simpleValueNode.TypeQName, simpleTypeInfo, simpleValueNode.TextSpan);
            if (effSimpleTypeInfo == null) {
                return false;
            }


            return true;
        }
    }
    [Serializable]
    public abstract class XObjectList<T> : XObject, IList<T>, IReadOnlyList<T> where T : XObject {
        protected XObjectList() {
            _itemList = new List<T>();
        }
        protected XObjectList(IEnumerable<T> items)
            : this() {
            AddRange(items);
        }
        protected List<T> _itemList;
        public override XObject DeepClone() {
            var obj = (XObjectList<T>)base.DeepClone();
            obj._itemList = new List<T>();
            foreach (var item in _itemList) {
                obj.Add(item);
            }
            return obj;
        }
        public int Count {
            get {
                return _itemList.Count;
            }
        }
        public T this[int index] {
            get {
                return _itemList[index];
            }
            set {
                _itemList[index] = SetParentTo(value);
            }
        }
        public void Add(T item) {
            _itemList.Add(SetParentTo(item));
        }
        public void AddRange(IEnumerable<T> items) {
            if (items != null) {
                foreach (var item in items) {
                    Add(item);
                }
            }
        }
        public void Insert(int index, T item) {
            _itemList.Insert(index, SetParentTo(item));
        }
        public bool Remove(T item) {
            return _itemList.Remove(item);
        }
        public void RemoveAt(int index) {
            _itemList.RemoveAt(index);
        }
        public void Clear() {
            _itemList.Clear();
        }
        public int IndexOf(T item) {
            return _itemList.IndexOf(item);
        }
        public bool Contains(T item) {
            return _itemList.Contains(item);
        }
        public void CopyTo(T[] array, int arrayIndex) {
            _itemList.CopyTo(array, arrayIndex);
        }
        public List<T>.Enumerator GetEnumerator() {
            return _itemList.GetEnumerator();
        }
        IEnumerator<T> IEnumerable<T>.GetEnumerator() {
            return GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
        bool ICollection<T>.IsReadOnly {
            get {
                return false;
            }
        }
    }
    #region List type
    [Serializable]
    public sealed class XListTypeValue<T> : XObjectList<T>, IEquatable<XListTypeValue<T>> where T : XSimpleType {
        public XListTypeValue() { }
        public XListTypeValue(IEnumerable<T> items)
            : base(items) {
        }
        public bool Equals(XListTypeValue<T> other) {
            if (object.ReferenceEquals(this, other)) return true;
            if (object.ReferenceEquals(other, null)) return false;
            var count = _itemList.Count;
            if (count != other._itemList.Count) {
                return false;
            }
            for (var i = 0; i < count; i++) {
                if (_itemList[i] != other._itemList[i]) {
                    return false;
                }
            }
            return true;
        }
        public override bool Equals(object obj) {
            return Equals(obj as XListTypeValue<T>);
        }
        public override int GetHashCode() {
            var hash = 17;
            var count = Math.Min(_itemList.Count, 7);
            for (var i = 0; i < count; i++) {
                hash = Extensions.AggregateHash(hash, _itemList[i].GetHashCode());
            }
            return hash;
        }
        public static bool operator ==(XListTypeValue<T> left, XListTypeValue<T> right) {
            if (object.ReferenceEquals(left, null)) {
                return object.ReferenceEquals(right, null);
            }
            return left.Equals(right);
        }
        public static bool operator !=(XListTypeValue<T> left, XListTypeValue<T> right) {
            return !(left == right);
        }
        public override ObjectInfo ObjectInfo {
            get { throw new NotSupportedException(); }
        }
    }

    [Serializable]
    public abstract class XListType<T> : XSimpleType, IList<T>, IReadOnlyList<T> where T : XSimpleType {
        protected XListType() { }
        protected XListType(XListTypeValue<T> value) {
            GenericValue = value;
        }
        protected XListType(IEnumerable<T> items) {
            AddRange(items);
        }
        public static implicit operator XListTypeValue<T>(XListType<T> obj) {
            if (obj == null) return null;
            return obj.Value;
        }
        new public XListTypeValue<T> Value {
            get {
                return GenericValue as XListTypeValue<T>;
            }
            set {
                GenericValue = value;
            }
        }
        public XListTypeValue<T> EnsureValue() {
            return Value ?? (Value = new XListTypeValue<T>());
        }
        public int Count {
            get {
                return EnsureValue().Count;
            }
        }
        public T this[int index] {
            get {
                return EnsureValue()[index];
            }
            set {
                EnsureValue()[index] = value;
            }
        }
        public void Add(T item) {
            EnsureValue().Add(item);
        }
        public void AddRange(IEnumerable<T> items) {
            EnsureValue().AddRange(items);
        }
        public void Insert(int index, T item) {
            EnsureValue().Insert(index, item);
        }
        public bool Remove(T item) {
            return EnsureValue().Remove(item);
        }
        public void RemoveAt(int index) {
            EnsureValue().RemoveAt(index);
        }
        public void Clear() {
            EnsureValue().Clear();
        }
        public int IndexOf(T item) {
            return EnsureValue().IndexOf(item);
        }
        public bool Contains(T item) {
            return EnsureValue().Contains(item);
        }
        public void CopyTo(T[] array, int arrayIndex) {
            EnsureValue().CopyTo(array, arrayIndex);
        }
        public List<T>.Enumerator GetEnumerator() {
            return EnsureValue().GetEnumerator();
        }
        IEnumerator<T> IEnumerable<T>.GetEnumerator() {
            return GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
        bool ICollection<T>.IsReadOnly {
            get { return false; }
        }
    }
    #endregion List type
    #region Atomic types
    [Serializable]
    public abstract class XAtomicType : XSimpleType {
        protected XAtomicType() { }

    }
    [Serializable]
    public class XString : XAtomicType {
        public XString() { }
        public XString(string value) { GenericValue = value; }
        public static implicit operator XString(string value) {
            if (value == null) return null;
            return new XString(value);
        }
        public static implicit operator string(XString obj) {
            if (obj == null) return null;
            return obj.Value;
        }
        new public string Value {
            get {
                return GenericValue as string;
            }
            set {
                GenericValue = value;
            }
        }
        public override ObjectInfo ObjectInfo { get { return ThisInfo; } }
        new public static readonly AtomicTypeInfo ThisInfo = new AtomicTypeInfo(typeof(XString), TypeKind.String.ToFullName(), TypeKind.String,
             XSimpleType.ThisInfo, typeof(string), null);
    }
    [Serializable]
    public class XDecimal : XAtomicType {
        public XDecimal() { }
        public XDecimal(decimal? value) { GenericValue = value; }
        public XDecimal(decimal value) { GenericValue = value; }
        public static implicit operator XDecimal(decimal? value) {
            if (value == null) return null;
            return new XDecimal(value);
        }
        public static implicit operator XDecimal(decimal value) {
            return new XDecimal(value);
        }
        public static implicit operator decimal?(XDecimal obj) {
            if (obj == null) return null;
            return obj.NullableValue;
        }
        public static explicit operator decimal(XDecimal obj) {
            return obj.Value;
        }
        public decimal? NullableValue {
            get {
                decimal? r;
                TryGetTypedValue(GenericValue, out r);
                return r;
            }
            set {
                GenericValue = value;
            }
        }
        new public decimal Value {
            get {
                return NullableValue.Value;
            }
            set {
                GenericValue = value;
            }
        }

        //public static string ToString(decimal value) {
        //    var s = value.ToString("G29", NumberFormatInfo.InvariantInfo);
        //    //if (s.IndexOf('.') == -1) s += ".0";
        //    return s;
        //}
        public static bool TryParseValue(string literal, out decimal result) {
            return decimal.TryParse(literal, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, NumberFormatInfo.InvariantInfo, out result);
        }
        public override ObjectInfo ObjectInfo { get { return ThisInfo; } }
        new public static readonly AtomicTypeInfo ThisInfo = new AtomicTypeInfo(typeof(XDecimal), TypeKind.Decimal.ToFullName(), TypeKind.Decimal,
            XSimpleType.ThisInfo, typeof(decimal), null);
        //
        //
        #region
        public static bool TryGetTypedValue(object value, out decimal? result) {
            result = value as decimal?;
            if (result == null && value != null) {
                switch (Type.GetTypeCode(value.GetType())) {
                    case TypeCode.Int64: result = (long)value; break;
                    case TypeCode.Int32: result = (int)value; break;
                    case TypeCode.Int16: result = (short)value; break;
                    case TypeCode.SByte: result = (sbyte)value; break;
                    case TypeCode.UInt64: result = (ulong)value; break;
                    case TypeCode.UInt32: result = (uint)value; break;
                    case TypeCode.UInt16: result = (ushort)value; break;
                    case TypeCode.Byte: result = (byte)value; break;
                }
            }
            return result != null;
        }
        public static bool TryGetTypedValue(object value, out long? result) {
            result = value as long?;
            if (result == null && value != null) {
                switch (Type.GetTypeCode(value.GetType())) {
                    case TypeCode.Decimal: {
                            var decimalValue = (decimal)value;
                            if (decimalValue >= long.MinValue && decimalValue <= long.MaxValue) result = (long)decimalValue;
                        }
                        break;
                    case TypeCode.Int32: result = (int)value; break;
                    case TypeCode.Int16: result = (short)value; break;
                    case TypeCode.SByte: result = (sbyte)value; break;
                    case TypeCode.UInt64: {
                            var ulongValue = (ulong)value;
                            if (ulongValue <= long.MaxValue) result = (long)ulongValue;
                        }
                        break;
                    case TypeCode.UInt32: result = (uint)value; break;
                    case TypeCode.UInt16: result = (ushort)value; break;
                    case TypeCode.Byte: result = (byte)value; break;
                }
            }
            return result != null;
        }
        public static bool TryGetTypedValue(object value, out ulong? result) {
            result = value as ulong?;
            if (result == null && value != null) {
                switch (Type.GetTypeCode(value.GetType())) {
                    case TypeCode.Decimal: {
                            var decimalValue = (decimal)value;
                            if (decimalValue >= 0 && decimalValue <= ulong.MaxValue) result = (ulong)decimalValue;
                        }
                        break;
                    case TypeCode.Int64: {
                            var longValue = (long)value;
                            if (longValue >= 0) result = (ulong)longValue;
                        }
                        break;
                    case TypeCode.Int32: {
                            var intValue = (int)value;
                            if (intValue >= 0) result = (ulong)intValue;
                        }
                        break;
                    case TypeCode.Int16: {
                            var shortValue = (short)value;
                            if (shortValue >= 0) result = (ulong)shortValue;
                        }
                        break;
                    case TypeCode.SByte: {
                            var sbyteValue = (sbyte)value;
                            if (sbyteValue >= 0) result = (ulong)sbyteValue;
                        }
                        break;
                    case TypeCode.UInt32: result = (uint)value; break;
                    case TypeCode.UInt16: result = (ushort)value; break;
                    case TypeCode.Byte: result = (byte)value; break;
                }
            }
            return result != null;
        }
        public static bool TryGetTypedValue(object value, out int? result) {
            result = value as int?;
            if (result == null && value != null) {
                switch (Type.GetTypeCode(value.GetType())) {
                    case TypeCode.Decimal: {
                            var decimalValue = (decimal)value;
                            if (decimalValue >= int.MinValue && decimalValue <= int.MaxValue) result = (int)decimalValue;
                        }
                        break;
                    case TypeCode.Int64: {
                            var longValue = (long)value;
                            if (longValue >= int.MinValue && longValue <= int.MaxValue) result = (int)longValue;
                        }
                        break;
                    case TypeCode.Int16: result = (short)value; break;
                    case TypeCode.SByte: result = (sbyte)value; break;
                    case TypeCode.UInt64: {
                            var ulongValue = (ulong)value;
                            if (ulongValue <= int.MaxValue) result = (int)ulongValue;
                        }
                        break;
                    case TypeCode.UInt32: {
                            var uintValue = (uint)value;
                            if (uintValue <= int.MaxValue) result = (int)uintValue;
                        }
                        break;
                    case TypeCode.UInt16: result = (ushort)value; break;
                    case TypeCode.Byte: result = (byte)value; break;
                }
            }
            return result != null;
        }
        public static bool TryGetTypedValue(object value, out uint? result) {
            result = value as uint?;
            if (result == null && value != null) {
                switch (Type.GetTypeCode(value.GetType())) {
                    case TypeCode.Decimal: {
                            var decimalValue = (decimal)value;
                            if (decimalValue >= 0 && decimalValue <= uint.MaxValue) result = (uint)decimalValue;
                        }
                        break;
                    case TypeCode.Int64: {
                            var longValue = (long)value;
                            if (longValue >= 0 && longValue <= uint.MaxValue) result = (uint)longValue;
                        }
                        break;
                    case TypeCode.Int32: {
                            var intValue = (int)value;
                            if (intValue >= 0) result = (uint)intValue;
                        }
                        break;
                    case TypeCode.Int16: {
                            var shortValue = (short)value;
                            if (shortValue >= 0) result = (uint)shortValue;
                        }
                        break;
                    case TypeCode.SByte: {
                            var sbyteValue = (sbyte)value;
                            if (sbyteValue >= 0) result = (uint)sbyteValue;
                        }
                        break;
                    case TypeCode.UInt64: {
                            var ulongValue = (ulong)value;
                            if (ulongValue <= uint.MaxValue) result = (uint)ulongValue;
                        }
                        break;
                    case TypeCode.UInt16: result = (ushort)value; break;
                    case TypeCode.Byte: result = (byte)value; break;
                }
            }
            return result != null;
        }
        public static bool TryGetTypedValue(object value, out short? result) {
            result = value as short?;
            if (result == null && value != null) {
                switch (Type.GetTypeCode(value.GetType())) {
                    case TypeCode.Decimal: {
                            var decimalValue = (decimal)value;
                            if (decimalValue >= short.MinValue && decimalValue <= short.MaxValue) result = (short)decimalValue;
                        }
                        break;
                    case TypeCode.Int64: {
                            var longValue = (long)value;
                            if (longValue >= short.MinValue && longValue <= short.MaxValue) result = (short)longValue;
                        }
                        break;
                    case TypeCode.Int32: {
                            var intValue = (int)value;
                            if (intValue >= short.MinValue && intValue <= short.MaxValue) result = (short)intValue;
                        }
                        break;
                    case TypeCode.SByte: result = (sbyte)value; break;
                    case TypeCode.UInt64: {
                            var ulongValue = (ulong)value;
                            if (ulongValue <= (ulong)short.MaxValue) result = (short)ulongValue;
                        }
                        break;
                    case TypeCode.UInt32: {
                            var uintValue = (uint)value;
                            if (uintValue <= short.MaxValue) result = (short)uintValue;
                        }
                        break;
                    case TypeCode.UInt16: {
                            var ushortValue = (ushort)value;
                            if (ushortValue <= short.MaxValue) result = (short)ushortValue;
                        }
                        break;
                    case TypeCode.Byte: result = (byte)value; break;
                }
            }
            return result != null;
        }
        public static bool TryGetTypedValue(object value, out ushort? result) {
            result = value as ushort?;
            if (result == null && value != null) {
                switch (Type.GetTypeCode(value.GetType())) {
                    case TypeCode.Decimal: {
                            var decimalValue = (decimal)value;
                            if (decimalValue >= 0 && decimalValue <= ushort.MaxValue) result = (ushort)decimalValue;
                        }
                        break;
                    case TypeCode.Int64: {
                            var longValue = (long)value;
                            if (longValue >= 0 && longValue <= ushort.MaxValue) result = (ushort)longValue;
                        }
                        break;
                    case TypeCode.Int32: {
                            var intValue = (int)value;
                            if (intValue >= 0 && intValue <= ushort.MaxValue) result = (ushort)intValue;
                        }
                        break;
                    case TypeCode.Int16: {
                            var shortValue = (short)value;
                            if (shortValue >= 0) result = (ushort)shortValue;
                        }
                        break;
                    case TypeCode.SByte: {
                            var sbyteValue = (sbyte)value;
                            if (sbyteValue >= 0) result = (ushort)sbyteValue;
                        }
                        break;
                    case TypeCode.UInt64: {
                            var ulongValue = (ulong)value;
                            if (ulongValue <= ushort.MaxValue) result = (ushort)ulongValue;
                        }
                        break;
                    case TypeCode.UInt32: {
                            var uintValue = (uint)value;
                            if (uintValue <= ushort.MaxValue) result = (ushort)uintValue;
                        }
                        break;
                    case TypeCode.Byte: result = (byte)value; break;
                }
            }
            return result != null;
        }
        public static bool TryGetTypedValue(object value, out sbyte? result) {
            result = value as sbyte?;
            if (result == null && value != null) {
                switch (Type.GetTypeCode(value.GetType())) {
                    case TypeCode.Decimal: {
                            var decimalValue = (decimal)value;
                            if (decimalValue >= sbyte.MinValue && decimalValue <= sbyte.MaxValue) result = (sbyte)decimalValue;
                        }
                        break;
                    case TypeCode.Int64: {
                            var longValue = (long)value;
                            if (longValue >= sbyte.MinValue && longValue <= sbyte.MaxValue) result = (sbyte)longValue;
                        }
                        break;
                    case TypeCode.Int32: {
                            var intValue = (int)value;
                            if (intValue >= sbyte.MinValue && intValue <= sbyte.MaxValue) result = (sbyte)intValue;
                        }
                        break;
                    case TypeCode.Int16: {
                            var shortValue = (short)value;
                            if (shortValue >= sbyte.MinValue && shortValue <= sbyte.MaxValue) result = (sbyte)shortValue;
                        }
                        break;
                    case TypeCode.UInt64: {
                            var ulongValue = (ulong)value;
                            if (ulongValue <= (ulong)sbyte.MaxValue) result = (sbyte)ulongValue;
                        }
                        break;
                    case TypeCode.UInt32: {
                            var uintValue = (uint)value;
                            if (uintValue <= sbyte.MaxValue) result = (sbyte)uintValue;
                        }
                        break;
                    case TypeCode.UInt16: {
                            var ushortValue = (ushort)value;
                            if (ushortValue <= sbyte.MaxValue) result = (sbyte)ushortValue;
                        }
                        break;
                    case TypeCode.Byte: {
                            var byteValue = (byte)value;
                            if (byteValue <= sbyte.MaxValue) result = (sbyte)byteValue;
                        }
                        break;
                }
            }
            return result != null;
        }
        public static bool TryGetTypedValue(object value, out byte? result) {
            result = value as byte?;
            if (result == null && value != null) {
                switch (Type.GetTypeCode(value.GetType())) {
                    case TypeCode.Decimal: {
                            var decimalValue = (decimal)value;
                            if (decimalValue >= 0 && decimalValue <= byte.MaxValue) result = (byte)decimalValue;
                        }
                        break;
                    case TypeCode.Int64: {
                            var longValue = (long)value;
                            if (longValue >= 0 && longValue <= byte.MaxValue) result = (byte)longValue;
                        }
                        break;
                    case TypeCode.Int32: {
                            var intValue = (int)value;
                            if (intValue >= 0 && intValue <= byte.MaxValue) result = (byte)intValue;
                        }
                        break;
                    case TypeCode.Int16: {
                            var shortValue = (short)value;
                            if (shortValue >= 0 && shortValue <= byte.MaxValue) result = (byte)shortValue;
                        }
                        break;
                    case TypeCode.SByte: {
                            var sbyteValue = (sbyte)value;
                            if (sbyteValue >= 0) result = (byte)sbyteValue;
                        }
                        break;
                    case TypeCode.UInt64: {
                            var ulongValue = (ulong)value;
                            if (ulongValue <= byte.MaxValue) result = (byte)ulongValue;
                        }
                        break;
                    case TypeCode.UInt32: {
                            var uintValue = (uint)value;
                            if (uintValue <= byte.MaxValue) result = (byte)uintValue;
                        }
                        break;
                    case TypeCode.UInt16: {
                            var ushortValue = (ushort)value;
                            if (ushortValue <= byte.MaxValue) result = (byte)ushortValue;
                        }
                        break;
                }
            }
            return result != null;
        }
        #endregion
    }
    [Serializable]
    public class XInt64 : XDecimal {
        public XInt64() { }
        public XInt64(long? value) { GenericValue = value; }
        public XInt64(long value) { GenericValue = value; }
        public static implicit operator XInt64(long? value) {
            if (value == null) return null;
            return new XInt64(value);
        }
        public static implicit operator XInt64(long value) {
            return new XInt64(value);
        }
        public static implicit operator long?(XInt64 obj) {
            if (obj == null) return null;
            return obj.NullableValue;
        }
        public static explicit operator long(XInt64 obj) {
            return obj.Value;
        }
        new public long? NullableValue {
            get {
                long? r;
                TryGetTypedValue(GenericValue, out r);
                return r;
            }
            set { GenericValue = value; }
        }
        new public long Value {
            get {
                return NullableValue.Value;
            }
            set {
                GenericValue = value;
            }
        }
        public static bool TryParseValue(string literal, out long result) {
            return long.TryParse(literal, NumberStyles.AllowLeadingSign, NumberFormatInfo.InvariantInfo, out result);
        }
        public override ObjectInfo ObjectInfo { get { return ThisInfo; } }
        new public static readonly AtomicTypeInfo ThisInfo = new AtomicTypeInfo(typeof(XInt64), TypeKind.Int64.ToFullName(), TypeKind.Int64,
            XDecimal.ThisInfo, typeof(long), null);
    }
    [Serializable]
    public class XInt32 : XInt64 {
        public XInt32() { }
        public XInt32(int? value) { GenericValue = value; }
        public XInt32(int value) { GenericValue = value; }
        public static implicit operator XInt32(int? value) {
            if (value == null) return null;
            return new XInt32(value);
        }
        public static implicit operator XInt32(int value) {
            return new XInt32(value);
        }
        public static implicit operator int?(XInt32 obj) {
            if (obj == null) return null;
            return obj.NullableValue;
        }
        public static explicit operator int(XInt32 obj) {
            return obj.Value;
        }
        new public int? NullableValue {
            get {
                int? r;
                TryGetTypedValue(GenericValue, out r);
                return r;
            }
            set { GenericValue = value; }
        }
        new public int Value {
            get {
                return NullableValue.Value;
            }
            set {
                GenericValue = value;
            }
        }
        public static bool TryParseValue(string literal, out int result) {
            return int.TryParse(literal, NumberStyles.AllowLeadingSign, NumberFormatInfo.InvariantInfo, out result);
        }
        public override ObjectInfo ObjectInfo { get { return ThisInfo; } }
        new public static readonly AtomicTypeInfo ThisInfo = new AtomicTypeInfo(typeof(XInt32), TypeKind.Int32.ToFullName(), TypeKind.Int32,
            XInt64.ThisInfo, typeof(int), null);
    }

    [Serializable]
    public class XInt16 : XInt32 {
        public XInt16() { }
        public XInt16(short? value) { GenericValue = value; }
        public XInt16(short value) { GenericValue = value; }
        public static implicit operator XInt16(short? value) {
            if (value == null) return null;
            return new XInt16(value);
        }
        public static implicit operator XInt16(short value) {
            return new XInt16(value);
        }
        public static implicit operator short?(XInt16 obj) {
            if (obj == null) return null;
            return obj.NullableValue;
        }
        public static explicit operator short(XInt16 obj) {
            return obj.Value;
        }
        new public short? NullableValue {
            get {
                short? r;
                TryGetTypedValue(GenericValue, out r);
                return r;
            }
            set { GenericValue = value; }
        }
        new public short Value {
            get {
                return NullableValue.Value;
            }
            set {
                GenericValue = value;
            }
        }
        public static bool TryParseValue(string literal, out short result) {
            return short.TryParse(literal, NumberStyles.AllowLeadingSign, NumberFormatInfo.InvariantInfo, out result);
        }
        public override ObjectInfo ObjectInfo { get { return ThisInfo; } }
        new public static readonly AtomicTypeInfo ThisInfo = new AtomicTypeInfo(typeof(XInt16), TypeKind.Int16.ToFullName(), TypeKind.Int16,
            XInt32.ThisInfo, typeof(short), null);
    }
    [Serializable]
    public class XSByte : XInt16 {
        public XSByte() { }
        public XSByte(sbyte? value) { GenericValue = value; }
        public XSByte(sbyte value) { GenericValue = value; }
        public static implicit operator XSByte(sbyte? value) {
            if (value == null) return null;
            return new XSByte(value);
        }
        public static implicit operator XSByte(sbyte value) {
            return new XSByte(value);
        }
        public static implicit operator sbyte?(XSByte obj) {
            if (obj == null) return null;
            return obj.NullableValue;
        }
        public static explicit operator sbyte(XSByte obj) {
            return obj.Value;
        }
        new public sbyte? NullableValue {
            get {
                sbyte? r;
                TryGetTypedValue(GenericValue, out r);
                return r;
            }
            set { GenericValue = value; }
        }
        new public sbyte Value {
            get {
                return NullableValue.Value;
            }
            set {
                GenericValue = value;
            }
        }
        public static bool TryParseValue(string literal, out sbyte result) {
            return sbyte.TryParse(literal, NumberStyles.AllowLeadingSign, NumberFormatInfo.InvariantInfo, out result);
        }
        public override ObjectInfo ObjectInfo { get { return ThisInfo; } }
        new public static readonly AtomicTypeInfo ThisInfo = new AtomicTypeInfo(typeof(XSByte), TypeKind.SByte.ToFullName(), TypeKind.SByte,
            XInt16.ThisInfo, typeof(sbyte), null);
    }

    [Serializable]
    public class XUInt64 : XDecimal {
        public XUInt64() { }
        public XUInt64(ulong? value) { GenericValue = value; }
        public XUInt64(ulong value) { GenericValue = value; }
        public static implicit operator XUInt64(ulong? value) {
            if (value == null) return null;
            return new XUInt64(value);
        }
        public static implicit operator XUInt64(ulong value) {
            return new XUInt64(value);
        }
        public static implicit operator ulong?(XUInt64 obj) {
            if (obj == null) return null;
            return obj.NullableValue;
        }
        public static explicit operator ulong(XUInt64 obj) {
            return obj.Value;
        }
        new public ulong? NullableValue {
            get {
                ulong? r;
                TryGetTypedValue(GenericValue, out r);
                return r;
            }
            set { GenericValue = value; }
        }
        new public ulong Value {
            get {
                return NullableValue.Value;
            }
            set {
                GenericValue = value;
            }
        }
        public static bool TryParseValue(string literal, out ulong result) {
            return ulong.TryParse(literal, NumberStyles.AllowLeadingSign, NumberFormatInfo.InvariantInfo, out result);
        }
        public override ObjectInfo ObjectInfo { get { return ThisInfo; } }
        new public static readonly AtomicTypeInfo ThisInfo = new AtomicTypeInfo(typeof(XUInt64), TypeKind.UInt64.ToFullName(), TypeKind.UInt64,
            XDecimal.ThisInfo, typeof(long), null);
    }
    [Serializable]
    public class XUInt32 : XUInt64 {
        public XUInt32() { }
        public XUInt32(uint? value) { GenericValue = value; }
        public XUInt32(uint value) { GenericValue = value; }
        public static implicit operator XUInt32(uint? value) {
            if (value == null) return null;
            return new XUInt32(value);
        }
        public static implicit operator XUInt32(uint value) {
            return new XUInt32(value);
        }
        public static implicit operator uint?(XUInt32 obj) {
            if (obj == null) return null;
            return obj.NullableValue;
        }
        public static explicit operator uint(XUInt32 obj) {
            return obj.Value;
        }
        new public uint? NullableValue {
            get {
                uint? r;
                TryGetTypedValue(GenericValue, out r);
                return r;
            }
            set { GenericValue = value; }
        }
        new public uint Value {
            get {
                return NullableValue.Value;
            }
            set {
                GenericValue = value;
            }
        }
        public static bool TryParseValue(string literal, out uint result) {
            return uint.TryParse(literal, NumberStyles.AllowLeadingSign, NumberFormatInfo.InvariantInfo, out result);
        }
        public override ObjectInfo ObjectInfo { get { return ThisInfo; } }
        new public static readonly AtomicTypeInfo ThisInfo = new AtomicTypeInfo(typeof(XUInt32), TypeKind.UInt32.ToFullName(), TypeKind.UInt32,
            XUInt64.ThisInfo, typeof(uint), null);
    }

    [Serializable]
    public class XUInt16 : XUInt32 {
        public XUInt16() { }
        public XUInt16(ushort? value) { GenericValue = value; }
        public XUInt16(ushort value) { GenericValue = value; }
        public static implicit operator XUInt16(ushort? value) {
            if (value == null) return null;
            return new XUInt16(value);
        }
        public static implicit operator XUInt16(ushort value) {
            return new XUInt16(value);
        }
        public static implicit operator ushort?(XUInt16 obj) {
            if (obj == null) return null;
            return obj.NullableValue;
        }
        public static explicit operator ushort(XUInt16 obj) {
            return obj.Value;
        }
        new public ushort? NullableValue {
            get {
                ushort? r;
                TryGetTypedValue(GenericValue, out r);
                return r;
            }
            set { GenericValue = value; }
        }
        new public ushort Value {
            get {
                return NullableValue.Value;
            }
            set {
                GenericValue = value;
            }
        }
        public static bool TryParseValue(string literal, out ushort result) {
            return ushort.TryParse(literal, NumberStyles.AllowLeadingSign, NumberFormatInfo.InvariantInfo, out result);
        }
        public override ObjectInfo ObjectInfo { get { return ThisInfo; } }
        new public static readonly AtomicTypeInfo ThisInfo = new AtomicTypeInfo(typeof(XUInt16), TypeKind.UInt16.ToFullName(), TypeKind.UInt16,
            XUInt32.ThisInfo, typeof(short), null);
    }
    [Serializable]
    public class XByte : XUInt16 {
        public XByte() { }
        public XByte(byte? value) { GenericValue = value; }
        public XByte(byte value) { GenericValue = value; }
        public static implicit operator XByte(byte? value) {
            if (value == null) return null;
            return new XByte(value);
        }
        public static implicit operator XByte(byte value) {
            return new XByte(value);
        }
        public static implicit operator byte?(XByte obj) {
            if (obj == null) return null;
            return obj.NullableValue;
        }
        public static explicit operator byte(XByte obj) {
            return obj.Value;
        }
        new public byte? NullableValue {
            get {
                byte? r;
                TryGetTypedValue(GenericValue, out r);
                return r;
            }
            set { GenericValue = value; }
        }
        new public byte Value {
            get {
                return NullableValue.Value;
            }
            set {
                GenericValue = value;
            }
        }
        public static bool TryParseValue(string literal, out byte result) {
            return byte.TryParse(literal, NumberStyles.AllowLeadingSign, NumberFormatInfo.InvariantInfo, out result);
        }
        public override ObjectInfo ObjectInfo { get { return ThisInfo; } }
        new public static readonly AtomicTypeInfo ThisInfo = new AtomicTypeInfo(typeof(XByte), TypeKind.Byte.ToFullName(), TypeKind.Byte,
            XUInt16.ThisInfo, typeof(byte), null);
    }

    [Serializable]
    public class XDouble : XAtomicType {
        public XDouble() { }
        public XDouble(double? value) { GenericValue = value; }
        public XDouble(double value) { GenericValue = value; }
        public static implicit operator XDouble(double? value) {
            if (value == null) return null;
            return new XDouble(value);
        }
        public static implicit operator XDouble(double value) {
            return new XDouble(value);
        }
        public static implicit operator double?(XDouble obj) {
            if (obj == null) return null;
            return obj.NullableValue;
        }
        public static explicit operator double(XDouble obj) {
            return obj.Value;
        }
        public double? NullableValue {
            get {
                double? r;
                TryGetTypedValue(GenericValue, out r);
                return r;
            }
            set {
                GenericValue = value;
            }
        }
        new public double Value {
            get {
                return NullableValue.Value;
            }
            set {
                GenericValue = value;
            }
        }
        public static bool TryGetTypedValue(object value, out double? result) {
            result = value as double?;
            if (result != null) {
                return true;
            }
            if (value is float) {
                result = (float)value;
                return true;
            }
            return false;
        }
        public static bool TryGetTypedValue(object value, out float? result) {
            result = value as float?;
            if (result != null) {
                return true;
            }
            if (value is double) {
                var doubleValue = (double)value;
                if (double.IsNegativeInfinity(doubleValue)) {
                    result = float.NegativeInfinity;
                    return true;
                }
                else if (double.IsPositiveInfinity(doubleValue)) {
                    result = float.PositiveInfinity;
                    return true;
                }
                else if (double.IsNaN(doubleValue)) {
                    result = float.NaN;
                    return true;
                }
                else if (doubleValue >= float.MinValue && doubleValue <= float.MaxValue) {
                    result = (float)doubleValue;
                    return true;
                }
            }
            return false;
        }

        public static string ToString(double value) {
            if (double.IsNegativeInfinity(value)) return NegativeInfinityLexicalValue;
            else if (double.IsPositiveInfinity(value)) return PositiveInfinityLexicalValue;
            else if (double.IsNaN(value)) return NaNLexicalValue;
            return value.ToString("0.0###############E0", NumberFormatInfo.InvariantInfo);
        }
        public static bool TryParseValue(string literal, out double result) {
            if (TryParseLexicalValue(literal, out result)) return true;
            return double.TryParse(literal, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent, NumberFormatInfo.InvariantInfo, out result);
        }
        private static bool TryParseLexicalValue(string literal, out double result) {
            if (literal == NegativeInfinityLexicalValue) result = double.NegativeInfinity;
            else if (literal == PositiveInfinityLexicalValue) result = double.PositiveInfinity;
            else if (literal == NaNLexicalValue) result = double.NaN;
            else { result = default(double); return false; }
            return true;
        }
        public const string NegativeInfinityLexicalValue = "-INF";
        public const string PositiveInfinityLexicalValue = "INF";
        public const string NaNLexicalValue = "NaN";
        public override ObjectInfo ObjectInfo { get { return ThisInfo; } }
        new public static readonly AtomicTypeInfo ThisInfo = new AtomicTypeInfo(typeof(XDouble), TypeKind.Double.ToFullName(), TypeKind.Double,
            XSimpleType.ThisInfo, typeof(double), null);
    }
    [Serializable]
    public class XSingle : XDouble {
        public XSingle() { }
        public XSingle(float? value) { GenericValue = value; }
        public XSingle(float value) { GenericValue = value; }
        public static implicit operator XSingle(float? value) {
            if (value == null) return null;
            return new XSingle(value);
        }
        public static implicit operator XSingle(float value) {
            return new XSingle(value);
        }
        public static implicit operator float?(XSingle obj) {
            if (obj == null) return null;
            return obj.NullableValue;
        }
        public static explicit operator float(XSingle obj) {
            return obj.Value;
        }
        new public float? NullableValue {
            get {
                float? r;
                TryGetTypedValue(GenericValue, out r);
                return r;
            }
            set {
                GenericValue = value;
            }
        }
        new public float Value {
            get {
                return NullableValue.Value;
            }
            set {
                GenericValue = value;
            }
        }
        public override ObjectInfo ObjectInfo { get { return ThisInfo; } }
        new public static readonly AtomicTypeInfo ThisInfo = new AtomicTypeInfo(typeof(XSingle), TypeKind.Single.ToFullName(), TypeKind.Single,
            XDouble.ThisInfo, typeof(float), null);

    }
    [Serializable]
    public class XBoolean : XAtomicType {
        public XBoolean() { }
        public XBoolean(bool? value) { GenericValue = value; }
        public XBoolean(bool value) { GenericValue = value; }
        public static implicit operator XBoolean(bool? value) {
            if (value == null) return null;
            return new XBoolean(value);
        }
        public static implicit operator XBoolean(bool value) {
            return new XBoolean(value);
        }
        public static implicit operator bool?(XBoolean obj) {
            if (obj == null) return null;
            return obj.NullableValue;
        }
        public static explicit operator bool(XBoolean obj) {
            return obj.Value;
        }
        public bool? NullableValue {
            get {
                return GenericValue as bool?;
            }
            set {
                GenericValue = value;
            }
        }
        new public bool Value {
            get {
                return NullableValue.Value;
            }
            set {
                GenericValue = value;
            }
        }
        public static string ToString(bool value) { return value ? "true" : "false"; }
        public static bool TryParseValue(string literal, out bool result) {
            if (literal == "true") {
                result = true;
            }
            else if (literal == "false") {
                result = false;
            }
            else {
                result = default(bool);
                return false;
            }
            return true;
        }
        public override ObjectInfo ObjectInfo { get { return ThisInfo; } }
        new public static readonly AtomicTypeInfo ThisInfo = new AtomicTypeInfo(typeof(XBoolean), TypeKind.Boolean.ToFullName(), TypeKind.Boolean,
            XSimpleType.ThisInfo, typeof(bool), null);
    }


    [Serializable]
    public sealed class XBinaryValue : XObject, IEquatable<XBinaryValue> {
        public XBinaryValue() { }
        public XBinaryValue(byte[] array) {
            _array = array;
        }
        private byte[] _array;
        public byte[] Array {
            get { return _array; }
            set { _array = value; }
        }
        public override XObject DeepClone() {
            var obj = (XBinaryValue)base.DeepClone();
            if (_array != null) {
                obj._array = (byte[])_array.Clone();
            }
            return obj;
        }
        public bool Equals(XBinaryValue other) {
            if (object.ReferenceEquals(this, other)) return true;
            if (object.ReferenceEquals(other, null)) return false;
            if (_array == null) {
                return other._array == null;
            }
            if (other._array == null) {
                return false;
            }
            var length = _array.Length;
            if (length != other._array.Length) {
                return false;
            }
            for (var i = 0; i < length; i++) {
                if (_array[i] != other._array[i]) {
                    return false;
                }
            }
            return true;
        }
        public override bool Equals(object obj) {
            return Equals(obj as XBinaryValue);
        }
        public override int GetHashCode() {
            if (_array == null) {
                return 0;
            }
            var hash = 17;
            var count = Math.Min(_array.Length, 13);
            for (var i = 0; i < count; i++) {
                hash = Extensions.AggregateHash(hash, _array[i]);
            }
            return hash;
        }
        public static bool operator ==(XBinaryValue left, XBinaryValue right) {
            if (object.ReferenceEquals(left, null)) {
                return object.ReferenceEquals(right, null);
            }
            return left.Equals(right);
        }
        public static bool operator !=(XBinaryValue left, XBinaryValue right) {
            return !(left == right);
        }
        public override ObjectInfo ObjectInfo {
            get { throw new NotSupportedException(); }
        }
    }

    [Serializable]
    public class XBinary : XAtomicType {
        public XBinary() { }
        public XBinary(XBinaryValue value) { GenericValue = value; }
        public static implicit operator XBinary(XBinaryValue value) {
            if (value == null) return null;
            return new XBinary(value);
        }
        public static implicit operator XBinaryValue(XBinary obj) {
            if (obj == null) return null;
            return obj.Value;
        }
        new public XBinaryValue Value {
            get {
                return GenericValue as XBinaryValue;
            }
            set {
                GenericValue = value;
            }
        }
        public static string ToString(byte[] value) {
            if (value == null) return null;
            return Convert.ToBase64String(value);
        }
        public static bool TryParseValue(string literal, out byte[] result) {
            try {
                result = Convert.FromBase64String(literal);
                return true;
            }
            catch (Exception) {
                result = null;
                return false;
            }
        }
        public override ObjectInfo ObjectInfo { get { return ThisInfo; } }
        new public static readonly AtomicTypeInfo ThisInfo = new AtomicTypeInfo(typeof(XBinary), TypeKind.Binary.ToFullName(), TypeKind.Binary,
             XSimpleType.ThisInfo, typeof(XBinaryValue), null);

    }

    [Serializable]
    public class XGuid : XAtomicType {
        public XGuid() { }
        public XGuid(Guid? value) { GenericValue = value; }
        public XGuid(Guid value) { GenericValue = value; }
        public static implicit operator XGuid(Guid? value) {
            if (value == null) return null;
            return new XGuid(value);
        }
        public static implicit operator XGuid(Guid value) {
            return new XGuid(value);
        }
        public static implicit operator Guid?(XGuid obj) {
            if (obj == null) return null;
            return obj.NullableValue;
        }
        public static explicit operator Guid(XGuid obj) {
            return obj.Value;
        }
        public Guid? NullableValue {
            get {
                return GenericValue as Guid?;
            }
            set {
                GenericValue = value;
            }
        }
        new public Guid Value {
            get {
                return NullableValue.Value;
            }
            set {
                GenericValue = value;
            }
        }
        public static string ToString(bool value) {
            return value ? "true" : "false";
        }
        public static bool TryParseValue(string literal, out bool result) {
            if (literal == "true") {
                result = true;
            }
            else if (literal == "false") {
                result = false;
            }
            else {
                result = default(bool);
                return false;
            }
            return true;
        }
        public override ObjectInfo ObjectInfo { get { return ThisInfo; } }
        new public static readonly AtomicTypeInfo ThisInfo = new AtomicTypeInfo(typeof(XGuid), TypeKind.Guid.ToFullName(), TypeKind.Guid,
            XSimpleType.ThisInfo, typeof(Guid), null);
    }

    [Serializable]
    public class XDuration : XAtomicType {
        public XDuration() { }
        public XDuration(TimeSpan? value) {
            GenericValue = value;
        }
        public XDuration(TimeSpan value) {
            GenericValue = value;
        }
        public static implicit operator XDuration(TimeSpan? value) {
            if (value == null) return null;
            return new XDuration(value);
        }
        public static implicit operator XDuration(TimeSpan value) {
            return new XDuration(value);
        }
        public static implicit operator TimeSpan?(XDuration obj) {
            if (obj == null) return null;
            return obj.NullableValue;
        }
        public static explicit operator TimeSpan(XDuration obj) {
            return obj.Value;
        }
        public TimeSpan? NullableValue {
            get {
                return GenericValue as TimeSpan?;
            }
            set {
                GenericValue = value;
            }
        }
        new public TimeSpan Value {
            get {
                return NullableValue.Value;
            }
            set {
                GenericValue = value;
            }
        }
        public static string ToString(bool value) {
            return value ? "true" : "false";
        }
        public static bool TryParseValue(string literal, out bool result) {
            if (literal == "true") {
                result = true;
            }
            else if (literal == "false") {
                result = false;
            }
            else {
                result = default(bool);
                return false;
            }
            return true;
        }
        public override ObjectInfo ObjectInfo { get { return ThisInfo; } }
        new public static readonly AtomicTypeInfo ThisInfo = new AtomicTypeInfo(typeof(XDuration), TypeKind.Duration.ToFullName(), TypeKind.Duration,
            XSimpleType.ThisInfo, typeof(TimeSpan), null);
    }


    [Serializable]
    public class XDateTime : XAtomicType {
        public XDateTime() { }
        public XDateTime(DateTimeOffset? value) { GenericValue = value; }
        public XDateTime(DateTimeOffset value) { GenericValue = value; }
        public static implicit operator XDateTime(DateTimeOffset? value) {
            if (value == null) return null;
            return new XDateTime(value);
        }
        public static implicit operator XDateTime(DateTimeOffset value) {
            return new XDateTime(value);
        }
        public static implicit operator DateTimeOffset?(XDateTime obj) {
            if (obj == null) return null;
            return obj.NullableValue;
        }
        public static explicit operator DateTimeOffset(XDateTime obj) {
            return obj.Value;
        }
        public DateTimeOffset? NullableValue {
            get {
                DateTimeOffset? r;
                TryGetTypedValue(GenericValue, out r);
                return r;
            }
            set {
                GenericValue = value;
            }
        }
        new public DateTimeOffset Value {
            get {
                return NullableValue.Value;
            }
            set {
                GenericValue = value;
            }
        }
        public static bool TryGetTypedValue(object value, out DateTimeOffset? result) {
            result = value as DateTimeOffset?;
            if (result != null) {
                return true;
            }
            if (value is DateTime) {
                result = new DateTimeOffset((DateTime)value);
                return true;
            }
            else if (value is TimeSpan) {
                result = new DateTimeOffset(DateTime.MinValue, (TimeSpan)value);
                return true;

            }
            return false;
        }
        public static bool TryGetTypedValue(object value, out DateTime? result) {
            result = value as DateTime?;
            if (result != null) {
                return true;
            }
            if (value is DateTimeOffset) {
                result = ((DateTimeOffset)value).Date;
                return true;
            }
            return false;
        }
        public static bool TryGetTypedValue(object value, out TimeSpan? result) {
            result = value as TimeSpan?;
            if (result != null) {
                return true;
            }
            if (value is DateTimeOffset) {
                result = ((DateTimeOffset)value).TimeOfDay;
                return true;
            }
            return false;
        }

        public static string ToString(bool value) {
            return value ? "true" : "false";
        }
        public static bool TryParseValue(string literal, out bool result) {
            if (literal == "true") {
                result = true;
            }
            else if (literal == "false") {
                result = false;
            }
            else {
                result = default(bool);
                return false;
            }
            return true;
        }
        public override ObjectInfo ObjectInfo { get { return ThisInfo; } }
        new public static readonly AtomicTypeInfo ThisInfo = new AtomicTypeInfo(typeof(XDateTime), TypeKind.DateTime.ToFullName(), TypeKind.DateTime,
            XSimpleType.ThisInfo, typeof(DateTimeOffset), null);
    }

    [Serializable]
    public class XDate : XDateTime {
        public XDate() { }
        public XDate(DateTime? value) { GenericValue = value; }
        public XDate(DateTime value) { GenericValue = value; }
        public static implicit operator XDate(DateTime? value) {
            if (value == null) return null;
            return new XDate(value);
        }
        public static implicit operator XDate(DateTime value) {
            return new XDate(value);
        }
        public static implicit operator DateTime?(XDate obj) {
            if (obj == null) return null;
            return obj.NullableValue;
        }
        public static explicit operator DateTime(XDate obj) {
            return obj.Value;
        }
        new public DateTime? NullableValue {
            get {
                DateTime? r;
                TryGetTypedValue(GenericValue, out r);
                return r;

            }
            set {
                GenericValue = value;
            }
        }
        new public DateTime Value {
            get {
                return NullableValue.Value;
            }
            set {
                GenericValue = value;
            }
        }
        public override ObjectInfo ObjectInfo { get { return ThisInfo; } }
        new public static readonly AtomicTypeInfo ThisInfo = new AtomicTypeInfo(typeof(XDate), TypeKind.Date.ToFullName(), TypeKind.Date,
            XDateTime.ThisInfo, typeof(DateTime), null);

    }
    [Serializable]
    public class XTime : XDateTime {
        public XTime() { }
        public XTime(TimeSpan? value) { GenericValue = value; }
        public XTime(TimeSpan value) { GenericValue = value; }
        public static implicit operator XTime(TimeSpan? value) {
            if (value == null) return null;
            return new XTime(value);
        }
        public static implicit operator XTime(TimeSpan value) {
            return new XTime(value);
        }
        public static implicit operator TimeSpan?(XTime obj) {
            if (obj == null) return null;
            return obj.NullableValue;
        }
        public static explicit operator TimeSpan(XTime obj) {
            return obj.Value;
        }
        new public TimeSpan? NullableValue {
            get {
                TimeSpan? r;
                TryGetTypedValue(GenericValue, out r);
                return r;

            }
            set {
                GenericValue = value;
            }
        }
        new public TimeSpan Value {
            get {
                return NullableValue.Value;
            }
            set {
                GenericValue = value;
            }
        }
        public override ObjectInfo ObjectInfo { get { return ThisInfo; } }
        new public static readonly AtomicTypeInfo ThisInfo = new AtomicTypeInfo(typeof(XTime), TypeKind.Time.ToFullName(), TypeKind.Time,
            XDateTime.ThisInfo, typeof(TimeSpan), null);

    }
    #endregion Atomic types
    [Serializable]
    public abstract class XComplexType : XType {
        protected XComplexType() { }
        //
        private XAttributeSet _attributes;
        public XAttributeSet Attributes {
            get {
                return _attributes;
            }
            set {
                _attributes = SetParentTo(value);
            }
        }
        public XAttributeSet GenericAttributes {
            get {
                return _attributes;
            }
            set {
                Attributes = value;
            }
        }
        //public bool HasAttributes {
        //    get {
        //        return _attributes != null && _attributes.Count > 0;
        //    }
        //}
        public T EnsureAttributes<T>(bool @try = false) where T : XAttributeSet {
            var obj = _attributes as T;
            if (obj != null) {
                return obj;
            }
            var complexTypeInfo = ComplexTypeInfo;
            if (complexTypeInfo.AttributeSet == null) {
                if (@try) {
                    return null;
                }
                throw new InvalidOperationException("Attribute set not allowed.");
            }
            obj = (T)complexTypeInfo.AttributeSet.CreateInstance();
            Attributes = obj;
            return obj;
        }
        public XAttributeSet EnsureAttributes(bool @try = false) {
            return EnsureAttributes<XAttributeSet>(@try);
        }
        //
        private XObject _children;
        public XObject Children {
            get {
                return _children;
            }
            set {
                _children = SetParentTo(value);
            }
        }
        public XObject GenericChildren {
            get {
                return _children;
            }
            set {
                Children = value;
            }
        }
        public T EnsureChildren<T>(bool @try = false) where T : XObject {
            var obj = _children as T;
            if (obj != null) {
                return obj;
            }
            var complexTypeInfo = ComplexTypeInfo;
            if (complexTypeInfo.Child == null) {
                if (@try) {
                    return null;
                }
                throw new InvalidOperationException("Child not allowed.");
            }
            obj = (T)complexTypeInfo.Child.CreateInstance();
            Children = obj;
            return obj;
        }
        //
        public ComplexTypeInfo ComplexTypeInfo {
            get {
                return (ComplexTypeInfo)ObjectInfo;
            }
        }
        //
        internal static bool TryCreate(Context context, ComplexTypeInfo complexTypeInfo, ProgramInfo programInfo, bool isNullable,
            ComplexValueNode complexValueNode, out XComplexType result) {
            result = null;
            var effComplexTypeInfo = (ComplexTypeInfo)GetTypeInfo(context, programInfo, complexValueNode.TypeQName, complexTypeInfo, complexValueNode.TextSpan);
            if (effComplexTypeInfo == null) {
                return false;
            }
            XAttributeSet attributeSet = null;
            var attributeListNode = complexValueNode.AttributeList;
            var attributeSetInfo = effComplexTypeInfo.AttributeSet;
            if (attributeSetInfo != null) {
                if (!XAttributeSet.TryCreate(context, attributeSetInfo, programInfo, isNullable, attributeListNode, out attributeSet)) {
                    return false;
                }
            }
            else {
                if (attributeListNode.HasItem) {
                    context.AddErrorDiagnostic(DiagnosticCode.TypeDoesNotAllowAttributes, "Type '{0}' does not allow attributes.".InvFormat(effComplexTypeInfo.FullName.ToString()),
                        attributeListNode.TextSpan);
                    return false;
                }
            }


            return true;
        }
    }
    public interface IEntityObject {//Attribute and Element impl this interface
        EntityInfo EntityInfo { get; }
        FullName FullName { get; }
        XType Type { get; }
    }
    [Serializable]
    public abstract class XAttribute : XObject, IEntityObject {
        protected XAttribute() {
            _fullName = GetFullName();
        }
        private readonly FullName _fullName;
        public FullName FullName {
            get {
                return _fullName;
            }
        }
        protected abstract FullName GetFullName();
        private XAttribute _referentialAttribute;
        public XAttribute ReferentialAttribute {
            get {
                return _referentialAttribute;
            }
            set {
                if (value != null) {
                    if (value._fullName != _fullName) {
                        throw new InvalidOperationException("Referential attribute full name '{0}' not equal to '{1}'.".InvFormat(
                            value._fullName.ToString(), _fullName.ToString()));
                    }
                    for (var i = value; i != null; i = i._referentialAttribute) {
                        if (object.ReferenceEquals(this, i)) {
                            throw new InvalidOperationException("Circular reference detected.");
                        }
                    }
                }
                _referentialAttribute = value;
            }
        }
        public XAttribute GenericReferentialAttribute {
            get {
                return _referentialAttribute;
            }
            set {
                ReferentialAttribute = value;
            }
        }
        public bool IsReference {
            get {
                return _referentialAttribute != null;
            }
        }
        public XAttribute EffectiveAttribute {
            get {
                return _referentialAttribute == null ? this : _referentialAttribute.EffectiveAttribute;
            }
        }
        private XSimpleType _type;
        private void SetType(XSimpleType type) {
            _type = SetParentTo(type);
        }
        public XSimpleType Type {
            get {
                return EffectiveAttribute._type;
            }
            set {
                if (_referentialAttribute != null) {
                    _referentialAttribute.Type = value;
                }
                else {
                    SetType(value);
                }
            }
        }
        public bool HasType {
            get {
                return EffectiveAttribute._type != null;
            }
        }
        public XSimpleType GenericType {
            get {
                return Type;
            }
            set {
                Type = value;
            }
        }
        XType IEntityObject.Type {
            get {
                return Type;
            }
        }
        public T EnsureType<T>(bool @try = false) where T : XSimpleType {
            if (_referentialAttribute != null) {
                return _referentialAttribute.EnsureType<T>(@try);
            }
            var obj = _type as T;
            if (obj != null) return obj;
            obj = (T)AttributeInfo.Type.CreateInstance(@try);
            SetType(obj);
            return obj;
        }
        public XSimpleType EnsureType(bool @try = false) {
            return EnsureType<XSimpleType>(@try);
        }
        public object Value {
            get {
                var type = Type;
                return type == null ? null : type.Value;
            }
            set { EnsureType().Value = value; }
        }
        public object GenericValue {
            get {
                return Value;
            }
            set {
                Value = value;
            }
        }
        public override XObject DeepClone() {
            var obj = (XAttribute)base.DeepClone();
            obj.SetType(_type);
            return obj;
        }
        public AttributeInfo AttributeInfo {
            get {
                return (AttributeInfo)ObjectInfo;
            }
        }
        EntityInfo IEntityObject.EntityInfo {
            get {
                return AttributeInfo;
            }
        }

    }
    [Serializable]
    public abstract class XAttributeSet : XObject, ICollection<XAttribute>, IReadOnlyCollection<XAttribute> {
        protected XAttributeSet() {
            _attributeList = new List<XAttribute>();
        }
        private List<XAttribute> _attributeList;
        public override XObject DeepClone() {
            var obj = (XAttributeSet)base.DeepClone();
            obj._attributeList = new List<XAttribute>();
            foreach (var attribute in _attributeList) {
                obj._attributeList.Add(obj.SetParentTo(attribute));
            }
            return obj;
        }
        private int IndexOf(FullName fullName) {
            var count = _attributeList.Count;
            for (var i = 0; i < count; ++i) {
                if (_attributeList[i].FullName == fullName) {
                    return i;
                }
            }
            return -1;
        }
        public void AddRange(IEnumerable<XAttribute> attributes) {
            if (attributes != null) {
                foreach (var attribute in attributes) {
                    Add(attribute);
                }
            }
        }
        public void Add(XAttribute attribute) {
            if (Contains(attribute)) {
                throw new ArgumentException("Attribute '{0}' already exists.".InvFormat(attribute.FullName.ToString()));
            }
            _attributeList.Add(SetParentTo(attribute));
        }
        public void AddOrSet(XAttribute attribute) {
            if (attribute == null) {
                throw new ArgumentNullException("attribute");
            }
            var idx = IndexOf(attribute.FullName);
            if (idx == -1) {
                _attributeList.Add(SetParentTo(attribute));
            }
            else {
                _attributeList[idx] = SetParentTo(attribute);
            }
        }
        public bool Contains(FullName fullName) {
            return IndexOf(fullName) != -1;
        }
        public bool Contains(XAttribute attribute) {
            if (attribute == null) {
                throw new ArgumentNullException("attribute");
            }
            return Contains(attribute.FullName);
        }
        public XAttribute TryGet(FullName fullName) {
            foreach (var attribute in _attributeList) {
                if (attribute.FullName == fullName) {
                    return attribute;
                }
            }
            return null;
        }
        public bool TryGet(FullName fullName, out XAttribute attribute) {
            attribute = TryGet(fullName);
            return attribute != null;
        }
        public int Count {
            get {
                return _attributeList.Count;
            }
        }
        public bool Remove(FullName fullName) {
            var idx = IndexOf(fullName);
            if (idx != -1) {
                _attributeList.RemoveAt(idx);
                return true;
            }
            return false;
        }
        public bool Remove(XAttribute attribute) {
            if (attribute == null) {
                throw new ArgumentNullException("attribute");
            }
            return Remove(attribute.FullName);
        }
        public void Clear() {
            _attributeList.Clear();
        }
        public List<XAttribute>.Enumerator GetEnumerator() {
            return _attributeList.GetEnumerator();
        }
        IEnumerator<XAttribute> IEnumerable<XAttribute>.GetEnumerator() {
            return GetEnumerator();
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
        public void CopyTo(XAttribute[] array, int arrayIndex) {
            _attributeList.CopyTo(array, arrayIndex);
        }
        bool ICollection<XAttribute>.IsReadOnly {
            get {
                return false;
            }
        }
        public AttributeSetInfo AttributeSetInfo {
            get {
                return (AttributeSetInfo)ObjectInfo;
            }
        }
        protected T CreateAttribute<T>(FullName fullName, bool @try = false) where T : XAttribute {
            var attributeInfo = AttributeSetInfo.TryGetAttribute(fullName);
            if (attributeInfo == null) {
                if (@try) {
                    return null;
                }
                throw new InvalidOperationException("Attribute '{0}' not exists in the attribute set.".InvFormat(fullName.ToString()));
            }
            return (T)attributeInfo.CreateInstance(@try);
        }
        //
        internal static bool TryCreate(Context context, AttributeSetInfo attributeSetInfo, ProgramInfo programInfo, bool isNullable,
            ListOrSingleNode<AttributeNode> attributeListNode, out XAttributeSet result) {
            result = null;

            return true;
        }
    }
    [Serializable]
    public abstract class XChild : XObject {
        public abstract int Order { get; }
    }
    [Serializable]
    public abstract class XElement : XChild, IEntityObject {
        protected XElement() {
            _fullName = GetFullName();
        }
        private readonly FullName _fullName;
        public FullName FullName {
            get {
                return EffectiveElement._fullName;
            }
        }
        protected abstract FullName GetFullName();
        private XElement _referentialElement;
        public XElement ReferentialElement {
            get {
                return _referentialElement;
            }
            set {
                if (value != null) {
                    for (var i = value; i != null; i = i._referentialElement) {
                        if (object.ReferenceEquals(this, i)) {
                            throw new InvalidOperationException("Circular reference detected.");
                        }
                    }
                }
                _referentialElement = value;
            }
        }
        public XElement GenericReferentialElement {
            get {
                return _referentialElement;
            }
            set {
                ReferentialElement = value;
            }
        }
        public bool IsReference {
            get {
                return _referentialElement != null;
            }
        }
        public XElement EffectiveElement {
            get {
                return _referentialElement == null ? this : _referentialElement.EffectiveElement;
            }
        }
        private XType _type;
        private void SetType(XType type) {
            _type = SetParentTo(type);
        }
        public XType Type {
            get {
                return EffectiveElement._type;
            }
            set {
                if (_referentialElement != null) {
                    _referentialElement.Type = value;
                }
                else {
                    SetType(value);
                }
            }
        }
        public bool HasType {
            get {
                return EffectiveElement._type != null;
            }
        }
        public XType GenericType {
            get {
                return Type;
            }
            set {
                Type = value;
            }
        }
        public T EnsureType<T>(bool @try = false) where T : XType {
            if (_referentialElement != null) {
                return _referentialElement.EnsureType<T>(@try);
            }
            var obj = _type as T;
            if (obj != null) return obj;
            obj = (T)ElementInfo.Type.CreateInstance(@try);
            SetType(obj);
            return obj;
        }
        public XType EnsureType(bool @try = false) {
            return EnsureType<XType>(@try);
        }
        //public object Value {
        //    get {
        //        var type = Type;
        //        return type == null ? null : type.Value;
        //    }
        //    set { EnsureType().Value = value; }
        //}
        //public object GenericValue {
        //    get {
        //        return Value;
        //    }
        //    set {
        //        Value = value;
        //    }
        //}
        public override XObject DeepClone() {
            var obj = (XElement)base.DeepClone();
            obj.SetType(_type);
            return obj;
        }
        public ElementInfo ElementInfo {
            get {
                return (ElementInfo)ObjectInfo;
            }
        }
        EntityInfo IEntityObject.EntityInfo {
            get {
                return ElementInfo;
            }
        }

        //
        internal static ChildCreationResult TrySkippableCreate(Context context, ElementInfo elementInfo, ElementNode elementNode, out XElement result) {
            result = null;
            var fullName = elementNode.FullName;
            ElementInfo globalElementInfo;
            if (!elementInfo.IsMatch(fullName, out globalElementInfo)) {
                return ChildCreationResult.Skipped;
            }
            var elementNameTextSpan = elementNode.QName.TextSpan;
            var effectiveElementInfo = globalElementInfo ?? elementInfo;
            if (effectiveElementInfo.IsAbstract) {
                context.AddErrorDiagnostic(DiagnosticCode.ElementIsAbstract, "Element '{0}' is abstract.".InvFormat(fullName.ToString()),
                    elementNameTextSpan);
                return ChildCreationResult.Fault;
            }
            XType type = null;
            var elementValueNode = elementNode.Value;
            var isNullable = elementInfo.IsNullable;
            if (elementValueNode.IsValid) {
                var complexTypeInfo = effectiveElementInfo.Type as ComplexTypeInfo;
                if (complexTypeInfo != null) {
                    var complexValueNode = elementValueNode.ComplexValue;
                    if (!complexValueNode.IsValid) {
                        context.AddErrorDiagnostic(DiagnosticCode.ElementRequiresComplexValue, "Element '{0}' requires complex value.".InvFormat(fullName.ToString()),
                            elementNameTextSpan);
                        return ChildCreationResult.Fault;
                    }
                    XComplexType complexType;
                    if (!XComplexType.TryCreate(context, complexTypeInfo, elementInfo.Program, isNullable, complexValueNode, out complexType)) {
                        return ChildCreationResult.Fault;
                    }
                    type = complexType;
                }
                else {
                    var simpleTypeInfo = effectiveElementInfo.Type as SimpleTypeInfo;
                    var simpleValueNode = elementValueNode.SimpleValue;
                    if (!simpleValueNode.IsValid) {
                        context.AddErrorDiagnostic(DiagnosticCode.ElementRequiresSimpleValue, "Element '{0}' requires simple value.".InvFormat(fullName.ToString()),
                            elementNameTextSpan);
                        return ChildCreationResult.Fault;
                    }
                    XSimpleType simpleType;
                    if (!XSimpleType.TryCreate(context, simpleTypeInfo, elementInfo.Program, isNullable, simpleValueNode, out simpleType)) {
                        return ChildCreationResult.Fault;
                    }
                    type = simpleType;
                }
            }
            else {
                if (!isNullable) {
                    context.AddErrorDiagnostic(DiagnosticCode.ElementIsNotNullable, "Element '{0}' is not nullable.".InvFormat(fullName.ToString()),
                        elementNameTextSpan);
                    return ChildCreationResult.Fault;
                }
            }
            //
            var effElement = (XElement)effectiveElementInfo.CreateInstance();
            effElement.SetType(type);
            if (elementInfo.IsReference) {
                result = (XElement)elementInfo.CreateInstance();
                result.ReferentialElement = effElement;
            }
            else {
                result = effElement;
            }
            return ChildCreationResult.Success;
        }

        private static bool TryLoadAndValidate<T>(Context context, ElementInfo elementInfo, ElementNode elementNode, out T result) where T : XElement {



            result = null;
            return false;
        }
    }
    internal enum ChildCreationResult : byte {
        Fault,
        Skipped,
        Success
    }

    [Serializable]
    public abstract class XChildSet : XChild, ICollection<XChild>, IReadOnlyCollection<XChild> {
        public XChildSet() {
            _childList = new List<XChild>();
        }
        private List<XChild> _childList;
        public override XObject DeepClone() {
            var obj = (XChildSet)base.DeepClone();
            obj._childList = new List<XChild>();
            foreach (var child in _childList) {
                obj._childList.Add(obj.SetParentTo(child));
            }
            return obj;
        }
        private bool TryGetIndexOf(int order, out int index) {
            int i;
            var found = false;
            var count = _childList.Count;
            for (i = 0; i < count; i++) {
                var childOrder = _childList[i].Order;
                if (childOrder == order) {
                    found = true;
                    break;
                }
                if (childOrder > order) {
                    break;
                }
            }
            index = i;
            return found;
        }
        private int IndexOf(int order) {
            int index;
            if (TryGetIndexOf(order, out index)) {
                return index;
            }
            return -1;
        }
        public void AddRange(IEnumerable<XChild> children) {
            if (children != null) {
                foreach (var child in children) {
                    Add(child);
                }
            }
        }
        public void Add(XChild child) {
            if (child == null) {
                throw new ArgumentNullException("child");
            }
            var order = child.Order;
            int index;
            if (TryGetIndexOf(order, out index)) {
                throw new ArgumentException("Child '{0}' already exists.".InvFormat(order.ToInvString()));
            }
            _childList.Insert(index, SetParentTo(child));
        }
        public void AddOrSet(XChild child) {
            if (child == null) {
                throw new ArgumentNullException("child");
            }
            var order = child.Order;
            int index;
            if (TryGetIndexOf(order, out index)) {
                _childList[index] = SetParentTo(child);
            }
            else {
                _childList.Insert(index, SetParentTo(child));
            }
        }
        public bool Contains(int order) {
            return IndexOf(order) != -1;
        }
        public bool Contains(XChild child) {
            if (child == null) {
                throw new ArgumentNullException("child");
            }
            return Contains(child.Order);
        }
        public XChild TryGet(int order) {
            foreach (var child in _childList) {
                if (child.Order == order) {
                    return child;
                }
            }
            return null;
        }
        public bool TryGet(int order, out XChild child) {
            child = TryGet(order);
            return child != null;
        }
        public int Count {
            get {
                return _childList.Count;
            }
        }
        public bool Remove(int order) {
            var idx = IndexOf(order);
            if (idx != -1) {
                _childList.RemoveAt(idx);
                return true;
            }
            return false;
        }
        public bool Remove(XChild child) {
            if (child == null) {
                throw new ArgumentNullException("child");
            }
            return Remove(child.Order);
        }
        public void Clear() {
            _childList.Clear();
        }
        public List<XChild>.Enumerator GetEnumerator() {
            return _childList.GetEnumerator();
        }
        IEnumerator<XChild> IEnumerable<XChild>.GetEnumerator() {
            return GetEnumerator();
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
        public void CopyTo(XChild[] array, int arrayIndex) {
            _childList.CopyTo(array, arrayIndex);
        }
        bool ICollection<XChild>.IsReadOnly {
            get {
                return false;
            }
        }
        public ChildSetInfo ChildSetInfo {
            get {
                return (ChildSetInfo)ObjectInfo;
            }
        }

    }
    [Serializable]
    public abstract class XChildList<T> : XChild, IList<T>, IReadOnlyList<T> where T : XChild {
        protected XChildList() {
            _itemList = new List<T>();
        }
        protected XChildList(IEnumerable<T> items)
            : this() {
            AddRange(items);
        }
        private List<T> _itemList;
        public override XObject DeepClone() {
            var obj = (XChildList<T>)base.DeepClone();
            obj._itemList = new List<T>();
            foreach (var item in _itemList) {
                obj.Add(item);
            }
            return obj;
        }
        public int Count {
            get {
                return _itemList.Count;
            }
        }
        public T this[int index] {
            get {
                return _itemList[index];
            }
            set {
                _itemList[index] = SetParentTo(value);
            }
        }
        public void Add(T item) {
            _itemList.Add(SetParentTo(item));
        }
        public void AddRange(IEnumerable<T> items) {
            if (items != null) {
                foreach (var item in items) {
                    Add(item);
                }
            }
        }
        public void Insert(int index, T item) {
            _itemList.Insert(index, SetParentTo(item));
        }
        public bool Remove(T item) {
            return _itemList.Remove(item);
        }
        public void RemoveAt(int index) {
            _itemList.RemoveAt(index);
        }
        public void Clear() {
            _itemList.Clear();
        }
        public int IndexOf(T item) {
            return _itemList.IndexOf(item);
        }
        public bool Contains(T item) {
            return _itemList.Contains(item);
        }
        public void CopyTo(T[] array, int arrayIndex) {
            _itemList.CopyTo(array, arrayIndex);
        }
        public List<T>.Enumerator GetEnumerator() {
            return _itemList.GetEnumerator();
        }
        IEnumerator<T> IEnumerable<T>.GetEnumerator() {
            return GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
        bool ICollection<T>.IsReadOnly {
            get {
                return false;
            }
        }
        //
        public ChildListInfo ChildListInfo {
            get {
                return (ChildListInfo)ObjectInfo;
            }
        }

    }

}
