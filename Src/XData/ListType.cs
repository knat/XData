﻿using System;
using System.Collections;
using System.Collections.Generic;
using XData.IO.Text;

namespace XData {
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

        new public static readonly ListTypeInfo ThisInfo = new ListTypeInfo(typeof(XListType), true, TypeKind.ListType.ToFullName(),
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
}