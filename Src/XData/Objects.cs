using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using XData.IO.Text;

namespace XData {
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
        //public T DeepClone<T>() where T : XObject {
        //    return (T)DeepClone();
        //}
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
            return TryValidateCore(context);
            //var success = TryValidating(context, true);
            //if (success) {
            //    success = TryValidateCore(context);
            //}
            //return TryValidated(context, success);
        }
        protected abstract bool TryValidateCore(Context context);
        //protected virtual bool TryValidating(Context context, bool fromValidate) {
        //    return true;
        //}
        //protected virtual bool TryValidated(Context context, bool success) {
        //    return success;
        //}
        //internal bool InvokeTryValidatePair(Context context) {
        //    return TryValidated(context, TryValidating(context, false));
        //}
    }

    public abstract class XType : XObject {
        protected XType() { }
        public TypeInfo TypeInfo {
            get {
                return (TypeInfo)ObjectInfo;
            }
        }
        //public static readonly TypeInfo ThisInfo = new TypeInfo(typeof(XType), AtomicTypeKind.Type.ToFullName(), AtomicTypeKind.Type, null);
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
                if (!typeInfo.IsEqualToOrDeriveFrom(declTypeInfo)) {
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

    public abstract class XSimpleType : XType, IEquatable<XSimpleType> {
        protected XSimpleType() { }
        //private object _value;
        //public object Value {
        //    get { return _value; }
        //    set {
        //        var valueObj = value as XObject;
        //        if (valueObj != null) {
        //            _value = SetParentTo(valueObj);
        //        }
        //        else {
        //            _value = value;
        //        }
        //    }
        //}
        //public object GenericValue {
        //    get {
        //        return _value;
        //    }
        //    set {
        //        Value = value;
        //    }
        //}
        //public bool HasValue {
        //    get {
        //        return _value != null;
        //    }
        //}
        //public override XObject DeepClone() {
        //    var obj = (XSimpleType)base.DeepClone();
        //    obj.Value = _value;
        //    return obj;
        //}
        //[ThreadStatic]
        //private static IEqualityComparer _valueEqualityComparer;
        //public static IEqualityComparer ValueEqualityComparer {
        //    get {
        //        return _valueEqualityComparer ?? (_valueEqualityComparer = DefaultValueComparer.Instance);
        //    }
        //    set {
        //        _valueEqualityComparer = value;
        //    }
        //}
        //[ThreadStatic]
        //private static IComparer _valueComparer;
        //public static IComparer ValueComparer {
        //    get {
        //        return _valueComparer ?? (_valueComparer = DefaultValueComparer.Instance);
        //    }
        //    set {
        //        _valueComparer = value;
        //    }
        //}
        //public sealed class DefaultValueComparer : IEqualityComparer, IComparer {
        //    private DefaultValueComparer() { }
        //    public static readonly DefaultValueComparer Instance = new DefaultValueComparer();
        //    new public bool Equals(object x, object y) {
        //        return object.Equals(x, y);
        //    }
        //    public int GetHashCode(object obj) {
        //        if (obj == null) return 0;
        //        return obj.GetHashCode();
        //    }
        //    public int Compare(object x, object y) {
        //        return Comparer<object>.Default.Compare(x, y);
        //    }
        //}
        //public virtual bool Equals(XSimpleType other) {
        //    if (object.ReferenceEquals(this, other)) return true;
        //    if (object.ReferenceEquals(other, null)) return false;
        //    return ValueEqualityComparer.Equals(_value, other._value);
        //}
        //public override sealed bool Equals(object obj) {
        //    return Equals(obj as XSimpleType);
        //}
        //public override int GetHashCode() {
        //    return ValueEqualityComparer.GetHashCode(_value);
        //}
        public abstract bool Equals(XSimpleType other);
        public abstract bool ValueEquals(object other);
        public override sealed bool Equals(object obj) {
            return Equals(obj as XSimpleType);
        }
        public override int GetHashCode() {
            throw new NotImplementedException();
        }
        public static bool operator ==(XSimpleType left, XSimpleType right) {
            if ((object)left == null) {
                return (object)right == null;
            }
            return left.Equals(right);
        }
        public static bool operator !=(XSimpleType left, XSimpleType right) {
            return !(left == right);
        }
        public virtual bool TryGetValueLength(out ulong result) {
            result = 0;
            return false;
        }
        protected override bool TryValidateCore(Context context) {
            var simpleTypeInfo = (SimpleTypeInfo)ObjectInfo;
            var restrictionSet = simpleTypeInfo.RestrictionSet;
            if (restrictionSet != null) {
                var minLength = restrictionSet.MinLength;
                var maxLength = restrictionSet.MaxLength;
                if (minLength != null || maxLength != null) {
                    ulong length;
                    if (!TryGetValueLength(out length)) {
                        throw new InvalidOperationException("!TryGetValueLength()");
                    }

                }
                var n_enumerations = restrictionSet.Enumerations;
                if (n_enumerations != null) {
                    var enumerations = n_enumerations.Value;
                    var found = false;
                    foreach (var item in enumerations.Items) {
                        if (ValueEquals(item.Value)) {
                            found = true;
                            break;
                        }
                    }
                    if (!found) {

                    }
                }
            }
            return true;
        }

        //public SimpleTypeInfo SimpleTypeInfo {
        //    get {
        //        return (SimpleTypeInfo)ObjectInfo;
        //    }
        //}
        public static readonly SimpleTypeInfo ThisInfo = new SimpleTypeInfo(typeof(XSimpleType), true, Extensions.SimpleTypeFullName, null, null);
        //
        internal static bool TryCreate(Context context, ProgramInfo programInfo, SimpleTypeInfo simpleTypeInfo,
            SimpleValueNode simpleValueNode, out XSimpleType result) {
            result = null;
            var effSimpleTypeInfo = (SimpleTypeInfo)GetTypeInfo(context, programInfo, simpleValueNode.TypeQName, simpleTypeInfo, simpleValueNode.TextSpan);
            if (effSimpleTypeInfo == null) {
                return false;
            }
            var atomicTypeInfo = effSimpleTypeInfo as AtomicTypeInfo;
            if (atomicTypeInfo != null) {
                var atomicValueNode = simpleValueNode.Atom;
                if (!atomicValueNode.IsValid) {
                    //context.AddErrorDiagnostic()
                    return false;
                }
                XAtomicType atomicType;
                if (!XAtomicType.TryCreate(context, atomicTypeInfo, atomicValueNode, out atomicType)) {
                    return false;
                }
                result = atomicType;
            }
            else {
                var listNode = simpleValueNode.List;
                if (listNode == null) {
                    //context.AddErrorDiagnostic()
                    return false;
                }
                XListType listType;
                if (!XListType.TryCreate(context, programInfo, (ListTypeInfo)effSimpleTypeInfo, listNode, out listType)) {
                    return false;
                }
                result = listType;
            }
            return true;
        }
        //public static bool TryParseValue(TypeKind typeKind, string literal, out object result) {
        //    switch (typeKind) {
        //        case TypeKind.SimpleType:
        //        case TypeKind.AtomicType:
        //        case TypeKind.String:
        //            result = literal;
        //            return true;
        //        case TypeKind.Decimal:
        //            {
        //                decimal r;
        //                if (XDecimal.TryParseValue(literal, out r)) {
        //                    result = r;
        //                    return true;
        //                }
        //            }
        //            break;
        //        case TypeKind.Int64:
        //            {
        //                long r;
        //                if (XInt64.TryParseValue(literal, out r)) {
        //                    result = r;
        //                    return true;
        //                }
        //            }
        //            break;
        //        case TypeKind.Int32:
        //            {
        //                int r;
        //                if (XInt32.TryParseValue(literal, out r)) {
        //                    result = r;
        //                    return true;
        //                }
        //            }
        //            break;
        //        case TypeKind.Int16:
        //            {
        //                short r;
        //                if (XInt16.TryParseValue(literal, out r)) {
        //                    result = r;
        //                    return true;
        //                }
        //            }
        //            break;
        //        case TypeKind.SByte:
        //            {
        //                sbyte r;
        //                if (XSByte.TryParseValue(literal, out r)) {
        //                    result = r;
        //                    return true;
        //                }
        //            }
        //            break;
        //        case TypeKind.UInt64:
        //            {
        //                ulong r;
        //                if (XUInt64.TryParseValue(literal, out r)) {
        //                    result = r;
        //                    return true;
        //                }
        //            }
        //            break;
        //        case TypeKind.UInt32:
        //            {
        //                uint r;
        //                if (XUInt32.TryParseValue(literal, out r)) {
        //                    result = r;
        //                    return true;
        //                }
        //            }
        //            break;
        //        case TypeKind.UInt16:
        //            {
        //                ushort r;
        //                if (XUInt16.TryParseValue(literal, out r)) {
        //                    result = r;
        //                    return true;
        //                }
        //            }
        //            break;
        //        case TypeKind.Byte:
        //            {
        //                byte r;
        //                if (XByte.TryParseValue(literal, out r)) {
        //                    result = r;
        //                    return true;
        //                }
        //            }
        //            break;
        //        case TypeKind.Double:
        //            {
        //                double r;
        //                if (XDouble.TryParseValue(literal, out r)) {
        //                    result = r;
        //                    return true;
        //                }
        //            }
        //            break;
        //        case TypeKind.Single:
        //            {
        //                float r;
        //                if (XSingle.TryParseValue(literal, out r)) {
        //                    result = r;
        //                    return true;
        //                }
        //            }
        //            break;
        //        case TypeKind.Boolean:
        //            {
        //                bool r;
        //                if (XBoolean.TryParseValue(literal, out r)) {
        //                    result = r;
        //                    return true;
        //                }
        //            }
        //            break;
        //        case TypeKind.Binary:
        //            {
        //                XBinaryValue r;
        //                if (XBinary.TryParseValue(literal, out r)) {
        //                    result = r;
        //                    return true;
        //                }
        //            }
        //            break;


        //        default:
        //            throw new ArgumentException("Invalid type kind: " + typeKind.ToString());
        //    }
        //    result = null;
        //    return false;
        //}


    }
    //public abstract class XObjectList<T> : XObject, IList<T>, IReadOnlyList<T> where T : XObject {
    //    protected XObjectList() {
    //        _itemList = new List<T>();
    //    }
    //    protected XObjectList(IEnumerable<T> items)
    //        : this() {
    //        AddRange(items);
    //    }
    //    protected List<T> _itemList;
    //    public override XObject DeepClone() {
    //        var obj = (XObjectList<T>)base.DeepClone();
    //        obj._itemList = new List<T>();
    //        foreach (var item in _itemList) {
    //            obj.Add(item);
    //        }
    //        return obj;
    //    }
    //    public int Count {
    //        get {
    //            return _itemList.Count;
    //        }
    //    }
    //    public T this[int index] {
    //        get {
    //            return _itemList[index];
    //        }
    //        set {
    //            _itemList[index] = SetParentTo(value);
    //        }
    //    }
    //    public void Add(T item) {
    //        _itemList.Add(SetParentTo(item));
    //    }
    //    public void AddRange(IEnumerable<T> items) {
    //        if (items != null) {
    //            foreach (var item in items) {
    //                Add(item);
    //            }
    //        }
    //    }
    //    public void Insert(int index, T item) {
    //        _itemList.Insert(index, SetParentTo(item));
    //    }
    //    public bool Remove(T item) {
    //        return _itemList.Remove(item);
    //    }
    //    public void RemoveAt(int index) {
    //        _itemList.RemoveAt(index);
    //    }
    //    public void Clear() {
    //        _itemList.Clear();
    //    }
    //    public int IndexOf(T item) {
    //        return _itemList.IndexOf(item);
    //    }
    //    public bool Contains(T item) {
    //        return _itemList.Contains(item);
    //    }
    //    public void CopyTo(T[] array, int arrayIndex) {
    //        _itemList.CopyTo(array, arrayIndex);
    //    }
    //    public List<T>.Enumerator GetEnumerator() {
    //        return _itemList.GetEnumerator();
    //    }
    //    IEnumerator<T> IEnumerable<T>.GetEnumerator() {
    //        return GetEnumerator();
    //    }
    //    IEnumerator IEnumerable.GetEnumerator() {
    //        return GetEnumerator();
    //    }
    //    bool ICollection<T>.IsReadOnly {
    //        get {
    //            return false;
    //        }
    //    }
    //}
    #region List type
    //public sealed class XListTypeValue : XObject, IList<object>, IReadOnlyList<object>, IEquatable<XListTypeValue> {
    //    public XListTypeValue() {
    //        _itemList = new List<object>();
    //    }
    //    public XListTypeValue(IEnumerable<object> items)
    //        : this() {
    //        AddRange(items);
    //    }
    //    private List<object> _itemList;
    //    public override XObject DeepClone() {
    //        var obj = (XListTypeValue)base.DeepClone();
    //        obj._itemList = new List<object>();
    //        foreach (var item in _itemList) {
    //            obj.Add(item);
    //        }
    //        return obj;
    //    }
    //    public int Count {
    //        get {
    //            return _itemList.Count;
    //        }
    //    }
    //    private object GetValue(object value) {
    //        var valueObj = value as XObject;
    //        if (valueObj != null) {
    //            return SetParentTo(valueObj);
    //        }
    //        return value;
    //    }
    //    public object this[int index] {
    //        get {
    //            return _itemList[index];
    //        }
    //        set {
    //            _itemList[index] = GetValue(value);
    //        }
    //    }
    //    public void Add(object item) {
    //        _itemList.Add(GetValue(item));
    //    }
    //    public void AddRange(IEnumerable<object> items) {
    //        if (items != null) {
    //            foreach (var item in items) {
    //                Add(item);
    //            }
    //        }
    //    }
    //    public void Insert(int index, object item) {
    //        _itemList.Insert(index, GetValue(item));
    //    }
    //    public bool Remove(object item) {
    //        return _itemList.Remove(item);
    //    }
    //    public void RemoveAt(int index) {
    //        _itemList.RemoveAt(index);
    //    }
    //    public void Clear() {
    //        _itemList.Clear();
    //    }
    //    public int IndexOf(object item) {
    //        return _itemList.IndexOf(item);
    //    }
    //    public bool Contains(object item) {
    //        return _itemList.Contains(item);
    //    }
    //    public void CopyTo(object[] array, int arrayIndex) {
    //        _itemList.CopyTo(array, arrayIndex);
    //    }
    //    public List<object>.Enumerator GetEnumerator() {
    //        return _itemList.GetEnumerator();
    //    }
    //    IEnumerator<object> IEnumerable<object>.GetEnumerator() {
    //        return GetEnumerator();
    //    }
    //    IEnumerator IEnumerable.GetEnumerator() {
    //        return GetEnumerator();
    //    }
    //    bool ICollection<object>.IsReadOnly {
    //        get {
    //            return false;
    //        }
    //    }
    //    //
    //    public bool Equals(XListTypeValue other) {
    //        if (object.ReferenceEquals(this, other)) return true;
    //        if (object.ReferenceEquals(other, null)) return false;
    //        var count = _itemList.Count;
    //        if (count != other._itemList.Count) {
    //            return false;
    //        }
    //        var valueEqualityComparer = XSimpleType.ValueEqualityComparer;
    //        for (var i = 0; i < count; i++) {
    //            if (!valueEqualityComparer.Equals(_itemList[i], other._itemList[i])) {
    //                return false;
    //            }
    //        }
    //        return true;
    //    }
    //    public override bool Equals(object obj) {
    //        return Equals(obj as XListTypeValue);
    //    }
    //    public override int GetHashCode() {
    //        var valueEqualityComparer = XSimpleType.ValueEqualityComparer;
    //        var hash = 17;
    //        var count = Math.Min(_itemList.Count, 5);
    //        for (var i = 0; i < count; i++) {
    //            hash = Extensions.AggregateHash(hash, valueEqualityComparer.GetHashCode(_itemList[i]));
    //        }
    //        return hash;
    //    }
    //    public static bool operator ==(XListTypeValue left, XListTypeValue right) {
    //        if (object.ReferenceEquals(left, null)) {
    //            return object.ReferenceEquals(right, null);
    //        }
    //        return left.Equals(right);
    //    }
    //    public static bool operator !=(XListTypeValue left, XListTypeValue right) {
    //        return !(left == right);
    //    }
    //    public override ObjectInfo ObjectInfo {
    //        get { throw new NotSupportedException(); }
    //    }
    //}

    public abstract class XListType : XSimpleType {
        internal static bool TryCreate(Context context, ProgramInfo programInfo, ListTypeInfo listTypeInfo,
            DelimitedList<SimpleValueNode> listNode, out XListType result) {
            result = null;
            var itemInfo = listTypeInfo.ItemType;
            var listType = listTypeInfo.CreateInstance<XListType>();
            foreach (var itemNode in listNode) {
                XSimpleType item;
                if (!TryCreate(context, programInfo, itemInfo, itemNode, out item)) {
                    return false;
                }
                listType.InternalAdd(item);
            }
            if (!listType.TryValidate(context)) {
                return false;
            }
            result = listType;
            return true;
        }
        internal abstract void InternalAdd(XSimpleType item);

        new public static readonly ListTypeInfo ThisInfo = new ListTypeInfo(typeof(XListType), true, Extensions.ListTypeFullName,
            XSimpleType.ThisInfo, null, null);
    }
    public abstract class XListType<T> : XListType, IList<T>, IReadOnlyList<T> where T : XSimpleType {
        protected XListType() {
            _itemList = new List<T>();
        }
        protected XListType(IEnumerable<T> items) : this() {
            AddRange(items);
        }
        private List<T> _itemList;
        internal override sealed void InternalAdd(XSimpleType item) {
            Add((T)item);
        }
        public override sealed bool TryGetValueLength(out ulong result) {
            result = (ulong)_itemList.Count;
            return true;
        }
        public override XObject DeepClone() {
            var obj = (XListType<T>)base.DeepClone();
            obj._itemList = new List<T>();
            foreach (var item in _itemList) {
                obj.Add(item);
            }
            return obj;
        }
        public override bool Equals(XSimpleType other) {
            if ((object)this == other) return true;
            var otherType = other as XListType<T>;
            if ((object)otherType == null) return false;
            var count = _itemList.Count;
            if (count != otherType._itemList.Count) {
                return false;
            }
            for (var i = 0; i < count; ++i) {
                if (_itemList[i] != otherType._itemList[i]) {
                    return false;
                }
            }
            return true;
        }
        public override bool ValueEquals(object other) {
            var otherArr = other as object[];
            if (otherArr == null) {
                return false;
            }
            var count = _itemList.Count;
            if (count != otherArr.Length) {
                return false;
            }
            for (var i = 0; i < count; ++i) {
                if (!_itemList[i].ValueEquals(otherArr[i])) {
                    return false;
                }
            }
            return true;
        }
        public override int GetHashCode() {
            var hash = 17;
            var count = Math.Min(_itemList.Count, 5);
            for (var i = 0; i < count; ++i) {
                hash = Extensions.AggregateHash(hash, _itemList[i].GetHashCode());
            }
            return hash;
        }
        public override string ToString() {
            var count = _itemList.Count;
            if (count == 0) {
                return "#[]";
            }
            if (count == 1) {
                return "[#" + _itemList[0].ToString() + "]";
            }
            var sb = Extensions.AcquireStringBuilder();
            sb.Append("#[");
            for (var i = 0; i < count; ++i) {
                if (i > 0) {
                    sb.Append(' ');
                }
                sb.Append(_itemList[i].ToString());
            }
            sb.Append(']');
            return sb.ToStringAndRelease();
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
    #endregion List type
    #region Atomic types
    public interface ITryComparable<in T> {
        bool TryCompareTo(T other, out int result);
    }

    public abstract class XAtomicType : XSimpleType, ITryComparable<XAtomicType> {
        protected XAtomicType() { }
        public abstract bool TryParseAndSet(string literal);
        public virtual bool TryCompareTo(XAtomicType other, out int result) {
            result = 0;
            return false;
        }
        public virtual bool TryCompareValueTo(object other, out int result) {
            result = 0;
            return false;
        }
        protected override sealed bool TryValidateCore(Context context) {
            if (!base.TryValidateCore(context)) {
                return false;
            }
            var atomicTypeInfo = (AtomicTypeInfo)ObjectInfo;
            var restrictionSet = atomicTypeInfo.RestrictionSet;
            if (restrictionSet != null) {

            }
            return true;
        }
        internal static bool TryCreate(Context context, AtomicTypeInfo atomicTypeInfo,
            AtomicValueNode atomicValueNode, out XAtomicType result) {
            result = null;
            var atomicType = atomicTypeInfo.CreateInstance<XAtomicType>();
            if (!atomicType.TryParseAndSet(atomicValueNode.Value)) {
                //context.AddErrorDiagnostic()
                return false;
            }
            if (!atomicType.TryValidate(context)) {
                return false;
            }
            result = atomicType;
            return true;
        }

        new public static readonly AtomicTypeInfo ThisInfo = new AtomicTypeInfo(typeof(XAtomicType), true, Extensions.AtomicTypeFullName,
            XSimpleType.ThisInfo, null, AtomicTypeKind.None);

    }
    public abstract class XStringBase : XAtomicType {
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
        public override bool TryCompareTo(XAtomicType other, out int result) {
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
        new public static readonly AtomicTypeInfo ThisInfo = AtomicTypeKind.StringBase.ToAtomicTypeInfo(typeof(XStringBase), XAtomicType.ThisInfo, true);
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
        new public static readonly AtomicTypeInfo ThisInfo = AtomicTypeKind.String.ToAtomicTypeInfo(typeof(XString), XStringBase.ThisInfo);
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
        new public static readonly AtomicTypeInfo ThisInfo = AtomicTypeKind.IgnoreCaseString.ToAtomicTypeInfo(typeof(XIgnoreCaseString), XStringBase.ThisInfo);
    }

    public class XDecimal : XAtomicType {
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
        protected virtual decimal GetDecimalValue() {
            return _value;
        }
        protected virtual bool SetDecimalValue(decimal value, bool @try = false) {
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
        public override bool Equals(XSimpleType other) {
            if ((object)this == other) return true;
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
        public override bool TryCompareTo(XAtomicType other, out int result) {
            result = 0;
            if ((object)this == other) return true;
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
        public override bool TryParseAndSet(string literal) {
            decimal r;
            if (!decimal.TryParse(literal, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, NumberFormatInfo.InvariantInfo, out r)) {
                return false;
            }
            return SetDecimalValue(r, true);
        }
        public override string ToString() {
            return Value.ToString(NumberFormatInfo.InvariantInfo);
        }
        public override ObjectInfo ObjectInfo { get { return ThisInfo; } }
        new public static readonly AtomicTypeInfo ThisInfo = AtomicTypeKind.Decimal.ToAtomicTypeInfo(typeof(XDecimal), XAtomicType.ThisInfo);
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
        protected virtual long GetInt64Value() {
            return _value;
        }
        protected virtual bool SetInt64Value(long value, bool @try = false) {
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
        protected override decimal GetDecimalValue() {
            return Value;
        }
        protected override bool SetDecimalValue(decimal value, bool @try = false) {
            if (value >= long.MinValue && value <= long.MaxValue) {
                return SetInt64Value((long)value, @try);
            }
            else if (@try) {
                return false;
            }
            throw new ArgumentOutOfRangeException("value");
        }
        public override bool Equals(XSimpleType other) {
            if ((object)this == other) return true;
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
        public override bool TryCompareTo(XAtomicType other, out int result) {
            result = 0;
            if ((object)this == other) return true;
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
            if (!long.TryParse(literal, NumberStyles.AllowLeadingSign, NumberFormatInfo.InvariantInfo, out r)) {
                return false;
            }
            return SetInt64Value(r, true);
        }
        public override string ToString() {
            return Value.ToString(NumberFormatInfo.InvariantInfo);
        }
        public override ObjectInfo ObjectInfo { get { return ThisInfo; } }
        new public static readonly AtomicTypeInfo ThisInfo = AtomicTypeKind.Int64.ToAtomicTypeInfo(typeof(XInt64), XDecimal.ThisInfo);
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
        protected virtual int GetInt32Value() {
            return _value;
        }
        protected virtual bool SetInt32Value(int value, bool @try = false) {
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
        protected override long GetInt64Value() {
            return Value;
        }
        protected override bool SetInt64Value(long value, bool @try = false) {
            if (value >= int.MinValue && value <= int.MaxValue) {
                return SetInt32Value((int)value, @try);
            }
            else if (@try) {
                return false;
            }
            throw new ArgumentOutOfRangeException("value");
        }
        protected override decimal GetDecimalValue() {
            return Value;
        }
        protected override bool SetDecimalValue(decimal value, bool @try = false) {
            if (value >= int.MinValue && value <= int.MaxValue) {
                return SetInt32Value((int)value, @try);
            }
            else if (@try) {
                return false;
            }
            throw new ArgumentOutOfRangeException("value");
        }
        public override bool Equals(XSimpleType other) {
            if ((object)this == other) return true;
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
        public override bool TryCompareTo(XAtomicType other, out int result) {
            result = 0;
            if ((object)this == other) return true;
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
            if (!int.TryParse(literal, NumberStyles.AllowLeadingSign, NumberFormatInfo.InvariantInfo, out r)) {
                return false;
            }
            return SetInt32Value(r, true);
        }
        public override string ToString() {
            return Value.ToString(NumberFormatInfo.InvariantInfo);
        }
        public override ObjectInfo ObjectInfo { get { return ThisInfo; } }
        new public static readonly AtomicTypeInfo ThisInfo = AtomicTypeKind.Int32.ToAtomicTypeInfo(typeof(XInt32), XInt64.ThisInfo);
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
        protected virtual short GetInt16Value() {
            return _value;
        }
        protected virtual bool SetInt16Value(short value, bool @try = false) {
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
        protected override int GetInt32Value() {
            return Value;
        }
        protected override bool SetInt32Value(int value, bool @try = false) {
            if (value >= short.MinValue && value <= short.MaxValue) {
                return SetInt16Value((short)value, @try);
            }
            else if (@try) {
                return false;
            }
            throw new ArgumentOutOfRangeException("value");
        }
        protected override long GetInt64Value() {
            return Value;
        }
        protected override bool SetInt64Value(long value, bool @try = false) {
            if (value >= short.MinValue && value <= short.MaxValue) {
                return SetInt16Value((short)value, @try);
            }
            else if (@try) {
                return false;
            }
            throw new ArgumentOutOfRangeException("value");
        }
        protected override decimal GetDecimalValue() {
            return Value;
        }
        protected override bool SetDecimalValue(decimal value, bool @try = false) {
            if (value >= short.MinValue && value <= short.MaxValue) {
                return SetInt16Value((short)value, @try);
            }
            else if (@try) {
                return false;
            }
            throw new ArgumentOutOfRangeException("value");
        }
        public override bool Equals(XSimpleType other) {
            if ((object)this == other) return true;
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
        public override bool TryCompareTo(XAtomicType other, out int result) {
            result = 0;
            if ((object)this == other) return true;
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
            if (!short.TryParse(literal, NumberStyles.AllowLeadingSign, NumberFormatInfo.InvariantInfo, out r)) {
                return false;
            }
            return SetInt16Value(r, true);
        }
        public override string ToString() {
            return Value.ToString(NumberFormatInfo.InvariantInfo);
        }
        public override ObjectInfo ObjectInfo { get { return ThisInfo; } }
        new public static readonly AtomicTypeInfo ThisInfo = AtomicTypeKind.Int16.ToAtomicTypeInfo(typeof(XInt16), XInt32.ThisInfo);
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
        protected virtual sbyte GetSByteValue() {
            return _value;
        }
        protected virtual bool SetSByteValue(sbyte value, bool @try = false) {
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
        protected override short GetInt16Value() {
            return Value;
        }
        protected override bool SetInt16Value(short value, bool @try = false) {
            if (value >= sbyte.MinValue && value <= sbyte.MaxValue) {
                return SetSByteValue((sbyte)value, @try);
            }
            else if (@try) {
                return false;
            }
            throw new ArgumentOutOfRangeException("value");
        }
        protected override int GetInt32Value() {
            return Value;
        }
        protected override bool SetInt32Value(int value, bool @try = false) {
            if (value >= sbyte.MinValue && value <= sbyte.MaxValue) {
                return SetSByteValue((sbyte)value, @try);
            }
            else if (@try) {
                return false;
            }
            throw new ArgumentOutOfRangeException("value");
        }
        protected override long GetInt64Value() {
            return Value;
        }
        protected override bool SetInt64Value(long value, bool @try = false) {
            if (value >= sbyte.MinValue && value <= sbyte.MaxValue) {
                return SetSByteValue((sbyte)value, @try);
            }
            else if (@try) {
                return false;
            }
            throw new ArgumentOutOfRangeException("value");
        }
        protected override decimal GetDecimalValue() {
            return Value;
        }
        protected override bool SetDecimalValue(decimal value, bool @try = false) {
            if (value >= sbyte.MinValue && value <= sbyte.MaxValue) {
                return SetSByteValue((sbyte)value, @try);
            }
            else if (@try) {
                return false;
            }
            throw new ArgumentOutOfRangeException("value");
        }
        public override bool Equals(XSimpleType other) {
            if ((object)this == other) return true;
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
        public override bool TryCompareTo(XAtomicType other, out int result) {
            result = 0;
            if ((object)this == other) return true;
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
            if (!sbyte.TryParse(literal, NumberStyles.AllowLeadingSign, NumberFormatInfo.InvariantInfo, out r)) {
                return false;
            }
            return SetSByteValue(r, true);
        }
        public override string ToString() {
            return Value.ToString(NumberFormatInfo.InvariantInfo);
        }
        public override ObjectInfo ObjectInfo { get { return ThisInfo; } }
        new public static readonly AtomicTypeInfo ThisInfo = AtomicTypeKind.SByte.ToAtomicTypeInfo(typeof(XSByte), XInt16.ThisInfo);
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
        protected virtual ulong GetUInt64Value() {
            return _value;
        }
        protected virtual bool SetUInt64Value(ulong value, bool @try = false) {
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
        protected override decimal GetDecimalValue() {
            return Value;
        }
        protected override bool SetDecimalValue(decimal value, bool @try = false) {
            if (value >= ulong.MinValue && value <= ulong.MaxValue) {
                return SetUInt64Value((ulong)value, @try);
            }
            else if (@try) {
                return false;
            }
            throw new ArgumentOutOfRangeException("value");
        }
        public override bool Equals(XSimpleType other) {
            if ((object)this == other) return true;
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
        public override bool TryCompareTo(XAtomicType other, out int result) {
            result = 0;
            if ((object)this == other) return true;
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
            if (!ulong.TryParse(literal, NumberStyles.AllowLeadingSign, NumberFormatInfo.InvariantInfo, out r)) {
                return false;
            }
            return SetUInt64Value(r, true);
        }
        public override string ToString() {
            return Value.ToString(NumberFormatInfo.InvariantInfo);
        }
        public override ObjectInfo ObjectInfo { get { return ThisInfo; } }
        new public static readonly AtomicTypeInfo ThisInfo = AtomicTypeKind.UInt64.ToAtomicTypeInfo(typeof(XUInt64), XDecimal.ThisInfo);
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
        protected virtual uint GetUInt32Value() {
            return _value;
        }
        protected virtual bool SetUInt32Value(uint value, bool @try = false) {
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
        protected override ulong GetUInt64Value() {
            return Value;
        }
        protected override bool SetUInt64Value(ulong value, bool @try = false) {
            if (value >= uint.MinValue && value <= uint.MaxValue) {
                return SetUInt32Value((uint)value, @try);
            }
            else if (@try) {
                return false;
            }
            throw new ArgumentOutOfRangeException("value");
        }
        protected override decimal GetDecimalValue() {
            return Value;
        }
        protected override bool SetDecimalValue(decimal value, bool @try = false) {
            if (value >= uint.MinValue && value <= uint.MaxValue) {
                return SetUInt32Value((uint)value, @try);
            }
            else if (@try) {
                return false;
            }
            throw new ArgumentOutOfRangeException("value");
        }
        public override bool Equals(XSimpleType other) {
            if ((object)this == other) return true;
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
        public override bool TryCompareTo(XAtomicType other, out int result) {
            result = 0;
            if ((object)this == other) return true;
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
            if (!uint.TryParse(literal, NumberStyles.AllowLeadingSign, NumberFormatInfo.InvariantInfo, out r)) {
                return false;
            }
            return SetUInt32Value(r, true);
        }
        public override string ToString() {
            return Value.ToString(NumberFormatInfo.InvariantInfo);
        }
        public override ObjectInfo ObjectInfo { get { return ThisInfo; } }
        new public static readonly AtomicTypeInfo ThisInfo = AtomicTypeKind.UInt32.ToAtomicTypeInfo(typeof(XUInt32), XUInt64.ThisInfo);
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
        protected virtual ushort GetUInt16Value() {
            return _value;
        }
        protected virtual bool SetUInt16Value(ushort value, bool @try = false) {
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
        protected override uint GetUInt32Value() {
            return Value;
        }
        protected override bool SetUInt32Value(uint value, bool @try = false) {
            if (value >= ushort.MinValue && value <= ushort.MaxValue) {
                return SetUInt16Value((ushort)value, @try);
            }
            else if (@try) {
                return false;
            }
            throw new ArgumentOutOfRangeException("value");
        }
        protected override ulong GetUInt64Value() {
            return Value;
        }
        protected override bool SetUInt64Value(ulong value, bool @try = false) {
            if (value >= ushort.MinValue && value <= ushort.MaxValue) {
                return SetUInt16Value((ushort)value, @try);
            }
            else if (@try) {
                return false;
            }
            throw new ArgumentOutOfRangeException("value");
        }
        protected override decimal GetDecimalValue() {
            return Value;
        }
        protected override bool SetDecimalValue(decimal value, bool @try = false) {
            if (value >= ushort.MinValue && value <= ushort.MaxValue) {
                return SetUInt16Value((ushort)value, @try);
            }
            else if (@try) {
                return false;
            }
            throw new ArgumentOutOfRangeException("value");
        }
        public override bool Equals(XSimpleType other) {
            if ((object)this == other) return true;
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
        public override bool TryCompareTo(XAtomicType other, out int result) {
            result = 0;
            if ((object)this == other) return true;
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
            if (!ushort.TryParse(literal, NumberStyles.AllowLeadingSign, NumberFormatInfo.InvariantInfo, out r)) {
                return false;
            }
            return SetUInt16Value(r, true);
        }
        public override string ToString() {
            return Value.ToString(NumberFormatInfo.InvariantInfo);
        }
        public override ObjectInfo ObjectInfo { get { return ThisInfo; } }
        new public static readonly AtomicTypeInfo ThisInfo = AtomicTypeKind.UInt16.ToAtomicTypeInfo(typeof(XUInt16), XUInt32.ThisInfo);
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
        protected virtual byte GetByteValue() {
            return _value;
        }
        protected virtual bool SetByteValue(byte value, bool @try = false) {
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
        protected override ushort GetUInt16Value() {
            return Value;
        }
        protected override bool SetUInt16Value(ushort value, bool @try = false) {
            if (value >= byte.MinValue && value <= byte.MaxValue) {
                return SetByteValue((byte)value, @try);
            }
            else if (@try) {
                return false;
            }
            throw new ArgumentOutOfRangeException("value");
        }
        protected override uint GetUInt32Value() {
            return Value;
        }
        protected override bool SetUInt32Value(uint value, bool @try = false) {
            if (value >= byte.MinValue && value <= byte.MaxValue) {
                return SetByteValue((byte)value, @try);
            }
            else if (@try) {
                return false;
            }
            throw new ArgumentOutOfRangeException("value");
        }
        protected override ulong GetUInt64Value() {
            return Value;
        }
        protected override bool SetUInt64Value(ulong value, bool @try = false) {
            if (value >= byte.MinValue && value <= byte.MaxValue) {
                return SetByteValue((byte)value, @try);
            }
            else if (@try) {
                return false;
            }
            throw new ArgumentOutOfRangeException("value");
        }
        protected override decimal GetDecimalValue() {
            return Value;
        }
        protected override bool SetDecimalValue(decimal value, bool @try = false) {
            if (value >= byte.MinValue && value <= byte.MaxValue) {
                return SetByteValue((byte)value, @try);
            }
            else if (@try) {
                return false;
            }
            throw new ArgumentOutOfRangeException("value");
        }
        public override bool Equals(XSimpleType other) {
            if ((object)this == other) return true;
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
        public override bool TryCompareTo(XAtomicType other, out int result) {
            result = 0;
            if ((object)this == other) return true;
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
            if (!byte.TryParse(literal, NumberStyles.AllowLeadingSign, NumberFormatInfo.InvariantInfo, out r)) {
                return false;
            }
            return SetByteValue(r, true);
        }
        public override string ToString() {
            return Value.ToString(NumberFormatInfo.InvariantInfo);
        }
        public override ObjectInfo ObjectInfo { get { return ThisInfo; } }
        new public static readonly AtomicTypeInfo ThisInfo = AtomicTypeKind.Byte.ToAtomicTypeInfo(typeof(XByte), XUInt16.ThisInfo);
    }


    public class XDouble : XAtomicType {
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
        public override bool TryCompareTo(XAtomicType other, out int result) {
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
        new public static readonly AtomicTypeInfo ThisInfo = AtomicTypeKind.Double.ToAtomicTypeInfo(typeof(XDouble), XAtomicType.ThisInfo);
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
        public override bool TryCompareTo(XAtomicType other, out int result) {
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
        new public static readonly AtomicTypeInfo ThisInfo = AtomicTypeKind.Single.ToAtomicTypeInfo(typeof(XSingle), XDouble.ThisInfo);
    }

    public class XBoolean : XAtomicType {
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
        public override bool TryCompareTo(XAtomicType other, out int result) {
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
        new public static readonly AtomicTypeInfo ThisInfo = AtomicTypeKind.Boolean.ToAtomicTypeInfo(typeof(XBoolean), XAtomicType.ThisInfo);
    }


    public class XBinary : XAtomicType {
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
            if ((object)this == other) return true;
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
            if (_value.Length == 0) return "";
            return Convert.ToBase64String(_value);
        }
        public override bool TryGetValueLength(out ulong result) {
            result = (ulong)_value.Length;
            return true;
        }
        public override ObjectInfo ObjectInfo { get { return ThisInfo; } }
        new public static readonly AtomicTypeInfo ThisInfo = AtomicTypeKind.Binary.ToAtomicTypeInfo(typeof(XBinary), XAtomicType.ThisInfo);
    }

    public class XGuid : XAtomicType {
        public XGuid() { }
        public XGuid(Guid value) {
            _value = value;
        }
        public static implicit operator XGuid(Guid value) {
            return new XGuid(value);
        }
        public static implicit operator Guid(XGuid obj) {
            if ((object)obj == null) return Guid.Empty;
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
        public override bool TryCompareTo(XAtomicType other, out int result) {
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
        public override ObjectInfo ObjectInfo { get { return ThisInfo; } }
        new public static readonly AtomicTypeInfo ThisInfo = AtomicTypeKind.Guid.ToAtomicTypeInfo(typeof(XGuid), XAtomicType.ThisInfo);
    }

    public class XDuration : XAtomicType {
        public XDuration() { }
        public XDuration(TimeSpan value) {
            _value = value;
        }
        public static implicit operator XDuration(TimeSpan value) {
            return new XDuration(value);
        }
        public static implicit operator TimeSpan(XDuration obj) {
            if ((object)obj == null) return TimeSpan.Zero;
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
            var otherType = other as XDuration;
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
        public override bool TryCompareTo(XAtomicType other, out int result) {
            result = 0;
            if ((object)this == other) return true;
            var otherType = other as XDuration;
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
        public override ObjectInfo ObjectInfo { get { return ThisInfo; } }
        new public static readonly AtomicTypeInfo ThisInfo = AtomicTypeKind.Duration.ToAtomicTypeInfo(typeof(XDuration), XAtomicType.ThisInfo);
    }

    public class XDateTime : XAtomicType {
        public XDateTime() { }
        public XDateTime(DateTimeOffset value) {
            _value = value;
        }
        public static implicit operator XDateTime(DateTimeOffset value) {
            return new XDateTime(value);
        }
        public static implicit operator DateTimeOffset(XDateTime obj) {
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
            var otherType = other as XDateTime;
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
        public override bool TryCompareTo(XAtomicType other, out int result) {
            result = 0;
            if ((object)this == other) return true;
            var otherType = other as XDateTime;
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
        new public static readonly AtomicTypeInfo ThisInfo = AtomicTypeKind.DateTime.ToAtomicTypeInfo(typeof(XDateTime), XAtomicType.ThisInfo);
    }


    //public class XDate : XDateTime {
    //    public XDate() { }
    //    public XDate(DateTime? value) { ObjectValue = value; }
    //    public XDate(DateTime value) { ObjectValue = value; }
    //    public static implicit operator XDate(DateTime? value) {
    //        if (value == null) return null;
    //        return new XDate(value);
    //    }
    //    public static implicit operator XDate(DateTime value) {
    //        return new XDate(value);
    //    }
    //    public static implicit operator DateTime? (XDate obj) {
    //        if (obj == null) return null;
    //        return obj.NullableValue;
    //    }
    //    public static explicit operator DateTime(XDate obj) {
    //        return obj.Value;
    //    }
    //    new public DateTime? NullableValue {
    //        get {
    //            DateTime? r;
    //            TryGetTypedValue(ObjectValue, out r);
    //            return r;

    //        }
    //        set {
    //            ObjectValue = value;
    //        }
    //    }
    //    new public DateTime Value {
    //        get {
    //            return NullableValue.Value;
    //        }
    //        set {
    //            ObjectValue = value;
    //        }
    //    }
    //    public override ObjectInfo ObjectInfo { get { return ThisInfo; } }
    //    new public static readonly AtomicTypeInfo ThisInfo = new AtomicTypeInfo(typeof(XDate), AtomicTypeKind.Date.ToFullName(), AtomicTypeKind.Date,
    //        XDateTime.ThisInfo, typeof(DateTime), null);

    //}

    //public class XTime : XDateTime {
    //    public XTime() { }
    //    public XTime(TimeSpan? value) { ObjectValue = value; }
    //    public XTime(TimeSpan value) { ObjectValue = value; }
    //    public static implicit operator XTime(TimeSpan? value) {
    //        if (value == null) return null;
    //        return new XTime(value);
    //    }
    //    public static implicit operator XTime(TimeSpan value) {
    //        return new XTime(value);
    //    }
    //    public static implicit operator TimeSpan? (XTime obj) {
    //        if (obj == null) return null;
    //        return obj.NullableValue;
    //    }
    //    public static explicit operator TimeSpan(XTime obj) {
    //        return obj.Value;
    //    }
    //    new public TimeSpan? NullableValue {
    //        get {
    //            TimeSpan? r;
    //            TryGetTypedValue(ObjectValue, out r);
    //            return r;

    //        }
    //        set {
    //            ObjectValue = value;
    //        }
    //    }
    //    new public TimeSpan Value {
    //        get {
    //            return NullableValue.Value;
    //        }
    //        set {
    //            ObjectValue = value;
    //        }
    //    }
    //    public override ObjectInfo ObjectInfo { get { return ThisInfo; } }
    //    new public static readonly AtomicTypeInfo ThisInfo = new AtomicTypeInfo(typeof(XTime), AtomicTypeKind.Time.ToFullName(), AtomicTypeKind.Time,
    //        XDateTime.ThisInfo, typeof(TimeSpan), null);

    //}
    #endregion Atomic types

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
            if (complexTypeInfo.Attributes == null) {
                if (@try) {
                    return null;
                }
                throw new InvalidOperationException("Attribute set not allowed.");
            }
            obj = complexTypeInfo.Attributes.CreateInstance<T>();
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
            if (complexTypeInfo.Children == null) {
                if (@try) {
                    return null;
                }
                throw new InvalidOperationException("Child not allowed.");
            }
            obj = complexTypeInfo.Children.CreateInstance<T>();
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
        internal static bool TryCreate(Context context, ProgramInfo programInfo, ComplexTypeInfo complexTypeInfo, bool isNullable,
            ComplexValueNode complexValueNode, out XComplexType result) {
            result = null;
            var equalsTokenTextSpan = complexValueNode.EqualsTokenTextSpan;
            var effComplexTypeInfo = (ComplexTypeInfo)GetTypeInfo(context, programInfo, complexValueNode.TypeQName, complexTypeInfo, equalsTokenTextSpan);
            if (effComplexTypeInfo == null) {
                return false;
            }
            //
            XAttributeSet attributeSet = null;
            var attributeListNode = complexValueNode.AttributeList;
            var attributeSetInfo = effComplexTypeInfo.Attributes;
            if (attributeSetInfo != null) {
                if (!XAttributeSet.TryCreate(context, programInfo, attributeSetInfo,
                    equalsTokenTextSpan, attributeListNode, out attributeSet)) {
                    return false;
                }
            }
            else {
                if (attributeListNode != null && attributeListNode.Count > 0) {
                    context.AddErrorDiagnostic(DiagnosticCode.TypeProhibitsAttributes,
                        "Type '{0}' prohibits attributes.".InvFormat(effComplexTypeInfo.FullName.ToString()),
                        attributeListNode.OpenTokenTextSpan);
                    return false;
                }
            }
            //
            XObject children = null;
            var simpleValueNode = complexValueNode.SimpleValue;
            var simpleTypeInfo = effComplexTypeInfo.Children as SimpleTypeInfo;
            if (simpleTypeInfo != null) {
                if (!simpleValueNode.IsValid) {
                    context.AddErrorDiagnostic(DiagnosticCode.TypeRequiresSimpleTypeChild,
                        "Type '{0}' requires simple type child.".InvFormat(effComplexTypeInfo.FullName.ToString()),
                        complexValueNode.OpenElementTextSpan);
                    return false;
                }
                XSimpleType simpleType;
                if (!XSimpleType.TryCreate(context, programInfo, simpleTypeInfo,
                    simpleValueNode, out simpleType)) {
                    return false;
                }
                children = simpleType;
            }
            else {
                var elementListNode = complexValueNode.ElementList;
                var childSetInfo = effComplexTypeInfo.Children as ChildSetInfo;
                if (childSetInfo != null) {
                    if (simpleValueNode.IsValid) {
                        context.AddErrorDiagnostic(DiagnosticCode.TypeRequiresElementChildren,
                            "Type '{0}' requires elemment children.".InvFormat(effComplexTypeInfo.FullName.ToString()),
                            simpleValueNode.TextSpan);
                        return false;
                    }
                    XChildSet childSet;
                    if (!XChildSet.TryCreate(context, programInfo, childSetInfo,
                        complexValueNode.CloseElementTextSpan, elementListNode, out childSet)) {
                        return false;
                    }
                    children = childSet;
                }
                else {
                    if (simpleValueNode.IsValid || (elementListNode != null && elementListNode.Count > 0)) {
                        context.AddErrorDiagnostic(DiagnosticCode.TypeProhibitsChildren,
                            "Type '{0}' prohibits children.".InvFormat(effComplexTypeInfo.FullName.ToString()),
                            complexValueNode.OpenElementTextSpan);
                    }
                }
            }
            result = effComplexTypeInfo.CreateInstance<XComplexType>();
            result.Attributes = attributeSet;
            result.Children = children;
            return true;
        }
    }
    public interface IEntityObject {//Attribute and Element impl this interface
        EntityInfo EntityInfo { get; }
        FullName FullName { get; }
        XType Type { get; }
    }

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
            obj = AttributeInfo.Type.CreateInstance<T>(@try);
            SetType(obj);
            return obj;
        }
        public XSimpleType EnsureType(bool @try = false) {
            return EnsureType<XSimpleType>(@try);
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
            return attributeInfo.CreateInstance<T>(@try);
        }
        //
        internal static bool TryCreate(Context context, ProgramInfo programInfo, AttributeSetInfo attributeSetInfo,
            TextSpan equalsTokenTextSpan, DelimitedList<AttributeNode> attributeListNode, out XAttributeSet result) {
            result = null;

            return true;
        }
    }

    public abstract class XChild : XObject {
        public abstract int Order { get; }
    }

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
            obj = ElementInfo.Type.CreateInstance<T>(@try);
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
        internal static CreationResult TrySkippableCreate(Context context, ElementInfo elementInfo, ElementNode elementNode, out XChild result) {
            result = null;
            ElementInfo globalElementInfo;
            if (!elementInfo.IsMatch(elementNode.FullName, out globalElementInfo)) {
                return CreationResult.Skipped;
            }
            var elementNameTextSpan = elementNode.QName.TextSpan;
            var effElementInfo = globalElementInfo ?? elementInfo;
            if (effElementInfo.IsAbstract) {
                context.AddErrorDiagnostic(DiagnosticCode.ElementIsAbstract,
                    "Element '{0}' is abstract.".InvFormat(effElementInfo.DisplayName),
                    elementNameTextSpan);
                return CreationResult.Error;
            }
            XType type = null;
            var elementValueNode = elementNode.Value;
            var isNullable = elementInfo.IsNullable;
            if (elementValueNode.IsValid) {
                var complexTypeInfo = effElementInfo.Type as ComplexTypeInfo;
                if (complexTypeInfo != null) {
                    var complexValueNode = elementValueNode.ComplexValue;
                    if (!complexValueNode.IsValid) {
                        context.AddErrorDiagnostic(DiagnosticCode.ElementRequiresComplexTypeValue,
                            "Element '{0}' requires complex type value.".InvFormat(effElementInfo.DisplayName),
                            elementNameTextSpan);
                        return CreationResult.Error;
                    }
                    XComplexType complexType;
                    if (!XComplexType.TryCreate(context, elementInfo.Program, complexTypeInfo, isNullable,
                        complexValueNode, out complexType)) {
                        return CreationResult.Error;
                    }
                    type = complexType;
                }
                else {
                    var simpleTypeInfo = effElementInfo.Type as SimpleTypeInfo;
                    var simpleValueNode = elementValueNode.SimpleValue;
                    if (!simpleValueNode.IsValid) {
                        context.AddErrorDiagnostic(DiagnosticCode.ElementRequiresSimpleTypeValue,
                            "Element '{0}' requires simple type value.".InvFormat(effElementInfo.DisplayName),
                            elementNameTextSpan);
                        return CreationResult.Error;
                    }
                    XSimpleType simpleType;
                    if (!XSimpleType.TryCreate(context, elementInfo.Program, simpleTypeInfo,
                        simpleValueNode, out simpleType)) {
                        return CreationResult.Error;
                    }
                    type = simpleType;
                }
            }
            else {
                if (!isNullable) {
                    context.AddErrorDiagnostic(DiagnosticCode.ElementIsNotNullable,
                        "Element '{0}' is not nullable.".InvFormat(effElementInfo.DisplayName),
                        elementNameTextSpan);
                    return CreationResult.Error;
                }
            }
            //
            var effElement = effElementInfo.CreateInstance<XElement>();
            effElement.SetType(type);
            if (elementInfo.IsReference) {
                var refElement = elementInfo.CreateInstance<XElement>();
                refElement.ReferentialElement = effElement;
                result = refElement;
            }
            else {
                result = effElement;
            }
            return CreationResult.OK;
        }

        private static bool TryLoadAndValidate<T>(Context context, ElementInfo elementInfo, ElementNode elementNode, out T result) where T : XElement {



            result = null;
            return false;
        }
    }
    internal enum CreationResult : byte {
        Error,
        Skipped,
        OK
    }

    public abstract class XChildContainer : XChild {
        internal abstract void InternalAdd(XChild child);
    }
    public abstract class XChildSet : XChildContainer, ICollection<XChild>, IReadOnlyCollection<XChild> {
        protected XChildSet() {
            _childList = new List<XChild>();
        }
        private List<XChild> _childList;
        internal override sealed void InternalAdd(XChild child) {
            _childList.Add(SetParentTo(child));
        }
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
            var childList = _childList;
            var count = childList.Count;
            for (i = 0; i < count; i++) {
                var childOrder = childList[i].Order;
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
        public void AddRange(IEnumerable<XChild> children) {
            if (children != null) {
                foreach (var child in children) {
                    Add(child);
                }
            }
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
        //
        internal static bool TryCreate(Context context, ProgramInfo programInfo, ChildSetInfo childSetInfo,
            TextSpan closeElementTextSpan, DelimitedList<ElementNode> elementListNode, out XChildSet result) {
            result = null;
            new CreationContext(context, programInfo, closeElementTextSpan, elementListNode);


            return true;
        }

        private struct CreationContext {
            internal CreationContext(Context context, ProgramInfo programInfo, TextSpan closeElementTextSpan, List<ElementNode> list) {
                _context = context;
                _programInfo = programInfo;
                _closeElementTextSpan = closeElementTextSpan;
                _list = list ?? emptyList;
                _count = _list.Count;
                _index = 0;
            }
            private static readonly List<ElementNode> emptyList = new List<ElementNode>();
            private readonly Context _context;
            private readonly ProgramInfo _programInfo;
            private readonly TextSpan _closeElementTextSpan;
            private readonly List<ElementNode> _list;
            private readonly int _count;
            private int _index;
            private bool IsEOF {
                get {
                    return _index >= _count;
                }
            }
            private ElementNode GetElementNode() {
                if (_index < _count) {
                    return _list[_index];
                }
                return default(ElementNode);
            }
            private void ConsumeElementNode() {
                ++_index;
            }
            private TextSpan GetTextSpan() {
                if (_index < _count) {
                    return _list[_index].QName.TextSpan;
                }
                return _closeElementTextSpan;
            }
            private CreationResult Create(IChildInfo childInfo, out XChild result) {
                result = null;
                if (IsEOF) {
                    return CreationResult.Skipped;
                }
                var elementInfo = childInfo as ElementInfo;
                if (elementInfo != null) {
                    var res = XElement.TrySkippableCreate(_context, elementInfo, GetElementNode(), out result);
                    if (res == CreationResult.OK) {
                        ConsumeElementNode();
                    }
                    return res;
                }
                else {
                    var childSetInfo = childInfo as ChildSetInfo;
                    if (childSetInfo != null) {
                        if (childSetInfo.Kind == ChildSetKind.Sequence) {
                            var memberList = new List<XChild>();
                            foreach (var memberInfo in childSetInfo.Members) {
                                XChild member;
                                var res = Create(memberInfo, out member);
                                if (res == CreationResult.OK) {
                                    memberList.Add(member);
                                }
                                else if (res == CreationResult.Skipped) {
                                    if (!memberInfo.IsEffectiveOptional) {
                                        if (memberList.Count == 0) {
                                            return res;
                                        }
                                        _context.AddErrorDiagnostic(DiagnosticCode.RequiredChildMemberIsNotMatched,
                                            "Required child member '{0}' is not matched.".InvFormat(memberInfo.DisplayName), GetTextSpan());
                                        return CreationResult.Error;
                                    }
                                }
                                else {//error
                                    return res;
                                }
                            }
                            if (memberList.Count == 0) {
                                return CreationResult.Skipped;
                            }
                            var container = ((ObjectInfo)childInfo).CreateInstance<XChildContainer>();
                            foreach (var member in memberList) {
                                container.InternalAdd(member);
                            }
                            result = container;
                            return CreationResult.OK;
                        }
                        else {//choice
                            XChild choice = null;
                            foreach (var memberInfo in childSetInfo.Members) {
                                XChild member;
                                var res = Create(memberInfo, out member);
                                if (res == CreationResult.OK) {
                                    choice = member;
                                    break;
                                }
                                else if (res == CreationResult.Error) {
                                    return res;
                                }
                            }
                            if (choice == null) {
                                return CreationResult.Skipped;
                            }
                            var container = ((ObjectInfo)childInfo).CreateInstance<XChildContainer>();
                            container.InternalAdd(choice);
                            result = container;
                            return CreationResult.OK;
                        }
                    }
                    else {
                        var childListInfo = childInfo as ChildListInfo;
                        var itemInfo = childListInfo.Item;
                        var itemCount = 0UL;
                        var maxOccurs = childListInfo.MaxOccurs;
                        var itemList = new List<XChild>();
                        while (itemCount <= maxOccurs) {
                            XChild item;
                            var res = Create(itemInfo, out item);
                            if (res == CreationResult.OK) {
                                itemList.Add(item);
                                ++itemCount;
                            }
                            else if (res == CreationResult.Skipped) {
                                if (itemCount == 0) {
                                    return res;
                                }
                                if (itemCount < childListInfo.MinOccurs) {
                                    _context.AddErrorDiagnostic(DiagnosticCode.ChildListCountIsNotGreaterThanOrEqualToMinOccurs,
                                        "Child list '{0}' count '{1}' is not greater than or equal to min occurs '{2}'.".InvFormat(
                                        childListInfo.DisplayName, itemCount.ToInvString(), childListInfo.MinOccurs.ToInvString()), GetTextSpan());
                                    return CreationResult.Error;
                                }
                                else {
                                    break;
                                }
                            }
                            else {//error
                                return res;
                            }
                        }
                        if (itemList.Count == 0) {
                            return CreationResult.Skipped;
                        }
                        var container = ((ObjectInfo)childInfo).CreateInstance<XChildContainer>();
                        foreach (var item in itemList) {
                            container.InternalAdd(item);
                        }
                        result = container;
                        return CreationResult.OK;
                    }
                }
            }

        }

    }

    public abstract class XChildList<T> : XChildContainer, IList<T>, IReadOnlyList<T> where T : XChild {
        protected XChildList() {
            _childList = new List<T>();
        }
        protected XChildList(IEnumerable<T> items)
            : this() {
            AddRange(items);
        }
        private List<T> _childList;
        internal override sealed void InternalAdd(XChild child) {
            _childList.Add(SetParentTo((T)child));
        }
        public override XObject DeepClone() {
            var obj = (XChildList<T>)base.DeepClone();
            obj._childList = new List<T>();
            foreach (var child in _childList) {
                obj._childList.Add(obj.SetParentTo(child));
            }
            return obj;
        }
        public int Count {
            get {
                return _childList.Count;
            }
        }
        public T this[int index] {
            get {
                return _childList[index];
            }
            set {
                _childList[index] = SetParentTo(value);
            }
        }
        public void Add(T item) {
            _childList.Add(SetParentTo(item));
        }
        public void AddRange(IEnumerable<T> items) {
            if (items != null) {
                foreach (var item in items) {
                    Add(item);
                }
            }
        }
        public void Insert(int index, T item) {
            _childList.Insert(index, SetParentTo(item));
        }
        public bool Remove(T item) {
            return _childList.Remove(item);
        }
        public void RemoveAt(int index) {
            _childList.RemoveAt(index);
        }
        public void Clear() {
            _childList.Clear();
        }
        public int IndexOf(T item) {
            return _childList.IndexOf(item);
        }
        public bool Contains(T item) {
            return _childList.Contains(item);
        }
        public void CopyTo(T[] array, int arrayIndex) {
            _childList.CopyTo(array, arrayIndex);
        }
        public List<T>.Enumerator GetEnumerator() {
            return _childList.GetEnumerator();
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
