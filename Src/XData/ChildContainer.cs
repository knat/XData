using System;
using System.Collections;
using System.Collections.Generic;
using XData.IO.Text;

namespace XData {
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
                                    if (!memberInfo.IsOptional) {
                                        if (memberList.Count == 0) {
                                            return res;
                                        }
                                        _context.AddErrorDiag(new DiagMsg(DiagCode.RequiredChildMemberIsNotMatched, memberInfo.DisplayName), GetTextSpan());
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
                                    _context.AddErrorDiag(new DiagMsg( DiagCode.ChildListCountIsNotGreaterThanOrEqualToMinOccurs,
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
