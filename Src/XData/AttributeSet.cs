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
        private int IndexOf(string name) {
            var count = _attributeList.Count;
            for (var i = 0; i < count; ++i) {
                if (_attributeList[i].Name == name) {
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
                throw new ArgumentException("Attribute '{0}' already exists.".InvFormat(attribute.Name));
            }
            _attributeList.Add(SetParentTo(attribute));
        }
        public void AddOrSet(XAttribute attribute) {
            if (attribute == null) {
                throw new ArgumentNullException("attribute");
            }
            var idx = IndexOf(attribute.Name);
            if (idx == -1) {
                _attributeList.Add(SetParentTo(attribute));
            }
            else {
                _attributeList[idx] = SetParentTo(attribute);
            }
        }
        public bool Contains(string name) {
            return IndexOf(name) != -1;
        }
        public bool Contains(XAttribute attribute) {
            if (attribute == null) {
                throw new ArgumentNullException("attribute");
            }
            return Contains(attribute.Name);
        }
        public XAttribute TryGet(string name) {
            foreach (var attribute in _attributeList) {
                if (attribute.Name == name) {
                    return attribute;
                }
            }
            return null;
        }
        public bool TryGet(string name, out XAttribute attribute) {
            attribute = TryGet(name);
            return attribute != null;
        }
        public int Count {
            get {
                return _attributeList.Count;
            }
        }
        public bool Remove(string name) {
            var idx = IndexOf(name);
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
            return Remove(attribute.Name);
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
        protected T CreateAttribute<T>(string name, bool @try = false) where T : XAttribute {
            var attributeInfo = AttributeSetInfo.TryGetAttribute(name);
            if (attributeInfo == null) {
                if (@try) {
                    return null;
                }
                throw new InvalidOperationException("Attribute '{0}' not exists in the attribute set.".InvFormat(name));
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
