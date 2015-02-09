using System;
using System.Collections.Generic;
using XData.IO.Text;

namespace XData {
    public abstract class XAttributeSet : XObject, ICollection<XAttribute>, IReadOnlyCollection<XAttribute> {
        protected XAttributeSet() {
            _list = new List<XAttribute>();
        }
        private List<XAttribute> _list;
        internal void InternalAdd(XAttribute attribute) {
            _list.Add(SetParentTo(attribute));
        }
        public override XObject DeepClone() {
            var obj = (XAttributeSet)base.DeepClone();
            obj._list = new List<XAttribute>();
            foreach (var attribute in _list) {
                obj._list.Add(obj.SetParentTo(attribute));
            }
            return obj;
        }
        private int IndexOf(string name) {
            var list = _list;
            var count = list.Count;
            for (var i = 0; i < count; ++i) {
                if (list[i].Name == name) {
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
            _list.Add(SetParentTo(attribute));
        }
        public void AddOrSetAttribute(XAttribute attribute) {
            if (attribute == null) {
                throw new ArgumentNullException("attribute");
            }
            var idx = IndexOf(attribute.Name);
            if (idx == -1) {
                _list.Add(SetParentTo(attribute));
            }
            else {
                _list[idx] = SetParentTo(attribute);
            }
        }
        public bool ContainsAttribute(string name) {
            return IndexOf(name) != -1;
        }
        public bool Contains(XAttribute attribute) {
            if (attribute == null) {
                throw new ArgumentNullException("attribute");
            }
            return ContainsAttribute(attribute.Name);
        }
        public XAttribute TryGetAttribute(string name) {
            var list = _list;
            var count = list.Count;
            for (var i = 0; i < count; ++i) {
                if (list[i].Name == name) {
                    return list[i];
                }
            }
            return null;
        }
        public int Count {
            get {
                return _list.Count;
            }
        }
        public bool RemoveAttribute(string name) {
            var idx = IndexOf(name);
            if (idx != -1) {
                _list.RemoveAt(idx);
                return true;
            }
            return false;
        }
        public bool Remove(XAttribute attribute) {
            if (attribute == null) {
                throw new ArgumentNullException("attribute");
            }
            return RemoveAttribute(attribute.Name);
        }
        public void Clear() {
            _list.Clear();
        }
        public List<XAttribute>.Enumerator GetEnumerator() {
            return _list.GetEnumerator();
        }
        IEnumerator<XAttribute> IEnumerable<XAttribute>.GetEnumerator() {
            return GetEnumerator();
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
        public void CopyTo(XAttribute[] array, int arrayIndex) {
            _list.CopyTo(array, arrayIndex);
        }
        bool ICollection<XAttribute>.IsReadOnly {
            get {
                return false;
            }
        }
        public T CreateAttribute<T>(string name, bool @try = false) where T : XAttribute {
            var attributeInfo = AttributeSetInfo.TryGetAttribute(name);
            if (attributeInfo == null) {
                if (@try) {
                    return null;
                }
                throw new InvalidOperationException("Cannot find attribute '{0}'.".InvFormat(name));
            }
            return attributeInfo.CreateInstance<T>(@try);
        }
        public AttributeSetInfo AttributeSetInfo {
            get {
                return (AttributeSetInfo)ObjectInfo;
            }
        }
        //
        public IEnumerable<T> SelfAttributes<T>(Func<T, bool> filter = null) where T : XAttribute {
            foreach (var att in _list) {
                var attribute = att as T;
                if (attribute != null) {
                    if (filter == null || filter(attribute)) {
                        yield return attribute;
                    }
                }
            }
        }
        public IEnumerable<T> SelfAttributeTypes<T>(Func<XAttribute, bool> attributeFilter = null, Func<T, bool> typeFilter = null) where T : XSimpleType {
            foreach (var att in _list) {
                if (attributeFilter == null || attributeFilter(att)) {
                    var type = att.Type as T;
                    if ((object)type != null) {
                        if (typeFilter == null || typeFilter(type)) {
                            yield return type;
                        }
                    }
                }
            }
        }

        //
        internal void Save(SavingContext context) {
            context.AppendLine('[');
            context.PushIndent();
            foreach (var attribute in _list) {
                attribute.Save(context);
            }
            context.PopIndent();
            context.Append(']');
        }
        internal override bool TryValidateCore(DiagContext context) {
            var attributeSetInfo = AttributeSetInfo;
            var dMarker = context.MarkDiags();
            var attributeList = new List<XAttribute>(_list);
            if (attributeSetInfo.Attributes != null) {
                foreach (var attributeInfo in attributeSetInfo.Attributes) {
                    var found = false;
                    for (var i = 0; i < attributeList.Count; ++i) {
                        var attribute = attributeList[i];
                        if (attribute.Name == attributeInfo.Name) {
                            if (attribute.CheckEqualTo(context, attributeInfo)) {
                                attribute.TryValidate(context);
                            }
                            attributeList.RemoveAt(i);
                            found = true;
                            break;
                        }
                    }
                    if (!found && !attributeInfo.IsOptional) {
                        context.AddErrorDiag(new DiagMsg(DiagCode.RequiredAttributeNotFound, attributeInfo.DisplayName), this);
                    }
                }
            }
            if (attributeList.Count > 0) {
                foreach (var attribute in attributeList) {
                    context.AddErrorDiag(new DiagMsg(DiagCode.RedundantAttribute, attribute.ObjectInfo.DisplayName), attribute);
                }
            }
            return !dMarker.HasErrors;
        }
        internal static bool TryCreate(DiagContext context, ProgramInfo programInfo, AttributeSetInfo attributeSetInfo,
            NodeList<AttributeNode> attributeNodeList, TextSpan closeAttributesTextSpan, out XAttributeSet result) {
            result = null;
            var dMarker = context.MarkDiags();
            List<XAttribute> attributeList = null;
            if (attributeSetInfo.Attributes != null) {
                foreach (var attributeInfo in attributeSetInfo.Attributes) {
                    var found = false;
                    if (attributeNodeList != null) {
                        for (var i = 0; i < attributeNodeList.Count; ++i) {
                            var attributeNode = attributeNodeList[i];
                            if (attributeNode.Name == attributeInfo.Name) {
                                XAttribute attribute;
                                if (XAttribute.TryCreate(context, programInfo, attributeInfo, attributeNode, out attribute)) {
                                    EX.CreateAndAdd(ref attributeList, attribute);
                                }
                                attributeNodeList.RemoveAt(i);
                                found = true;
                                break;
                            }
                        }
                    }
                    if (!found && !attributeInfo.IsOptional) {
                        context.AddErrorDiag(new DiagMsg(DiagCode.RequiredAttributeNotFound, attributeInfo.DisplayName), closeAttributesTextSpan);
                    }
                }
            }
            if (attributeNodeList.CountOrZero() > 0) {
                foreach (var attributeNode in attributeNodeList) {
                    context.AddErrorDiag(new DiagMsg(DiagCode.RedundantAttribute, attributeNode.NameNode.ToString()), attributeNode.NameNode.TextSpan);
                }
            }
            if (dMarker.HasErrors) {
                return false;
            }
            if (attributeNodeList != null) {
                var attributeSet = attributeSetInfo.CreateInstance<XAttributeSet>();
                attributeSet.TextSpan = attributeNodeList.OpenTokenTextSpan;
                if (attributeList != null) {
                    foreach (var attribute in attributeList) {
                        attributeSet.InternalAdd(attribute);
                    }
                }
                result = attributeSet;
            }
            return true;
        }
    }
}
