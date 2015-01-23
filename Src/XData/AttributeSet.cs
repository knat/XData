using System;
using System.Collections.Generic;
using XData.IO.Text;

namespace XData {

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
            TextSpan equalsTokenTextSpan, NodeList<AttributeNode> attributeListNode, out XAttributeSet result) {
            result = null;

            return true;
        }
    }
}
