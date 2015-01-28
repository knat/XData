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
        internal static bool TryCreate(DiagContext context, ProgramInfo programInfo, ChildSetInfo childSetInfo,
            TextSpan openChildrenTextSpan, TextSpan closeChildrenTextSpan, List<ElementNode> elementNodeList, out XChildSet result) {
            result = null;
            new CreationContext(context, programInfo, openChildrenTextSpan, closeChildrenTextSpan, elementNodeList);


            return true;
        }

        private struct CreationContext {
            internal CreationContext(DiagContext context, ProgramInfo programInfo,
                TextSpan openChildrenTextSpan, TextSpan closeChildrenTextSpan, List<ElementNode> elementNodeList) {
                _context = context;
                _programInfo = programInfo;
                _openChildrenTextSpan = openChildrenTextSpan;
                _closeChildrenTextSpan = closeChildrenTextSpan;
                _elementNodeList = elementNodeList;
                _count = _elementNodeList.Count;
                _index = 0;
            }
            private readonly DiagContext _context;
            private readonly ProgramInfo _programInfo;
            private readonly TextSpan _openChildrenTextSpan, _closeChildrenTextSpan;
            private readonly List<ElementNode> _elementNodeList;
            private readonly int _count;
            private int _index;
            private bool IsEOF {
                get {
                    return _index >= _count;
                }
            }
            private ElementNode GetElementNode() {
                if (_index < _count) {
                    return _elementNodeList[_index];
                }
                return default(ElementNode);
            }
            private void ConsumeElementNode() {
                ++_index;
            }
            private TextSpan GetTextSpan() {
                if (_index < _count) {
                    return _elementNodeList[_index].QName.TextSpan;
                }
                return _closeChildrenTextSpan;
            }
            private CreationResult Create(ChildInfo childInfo, out XChild result) {
                result = null;
                if (IsEOF) {
                    return CreationResult.Skipped;
                }
                var elementInfo = childInfo as ElementInfo;
                if (elementInfo != null) {
                    var res = XElementBase.TrySkippableCreate(_context, elementInfo, GetElementNode(), out result);
                    if (res == CreationResult.OK) {
                        ConsumeElementNode();
                    }
                    return res;
                }
                else {
                    var childSetInfo = childInfo as ChildSetInfo;
                    if (childSetInfo != null) {
                        if (childSetInfo.IsSequence) {
                            List<XChild> childList = null;
                            foreach (var memberChildInfo in childSetInfo.Children) {
                                XChild child;
                                var res = Create(memberChildInfo, out child);
                                if (res == CreationResult.OK) {
                                    Extensions.CreateAndAdd(ref childList, child);
                                }
                                else if (res == CreationResult.Skipped) {
                                    if (!memberChildInfo.IsOptional) {
                                        if (childList.CountOrZero() == 0) {
                                            return res;
                                        }
                                        _context.AddErrorDiag(new DiagMsg(DiagCode.RequiredChildNotFound, memberChildInfo.DisplayName), GetTextSpan());
                                        return CreationResult.Error;
                                    }
                                }
                                else {//error
                                    return res;
                                }
                            }
                            if (childList.CountOrZero() == 0) {
                                return CreationResult.Skipped;
                            }
                            var container = childInfo.CreateInstance<XChildContainer>();
                            container.TextSpan = childList[0].TextSpan;
                            foreach (var child in childList) {
                                container.InternalAdd(child);
                            }
                            result = container;
                            return CreationResult.OK;
                        }
                        else {//choice
                            XChild choice = null;
                            foreach (var memberChildInfo in childSetInfo.Children) {
                                XChild child;
                                var res = Create(memberChildInfo, out child);
                                if (res == CreationResult.OK) {
                                    choice = child;
                                    break;
                                }
                                else if (res == CreationResult.Error) {
                                    return res;
                                }
                            }
                            if (choice == null) {
                                return CreationResult.Skipped;
                            }
                            var container = childInfo.CreateInstance<XChildContainer>();
                            container.TextSpan = choice.TextSpan;
                            container.InternalAdd(choice);
                            result = container;
                            return CreationResult.OK;
                        }
                    }
                    else {
                        var childListInfo = childInfo as ChildListInfo;
                        var itemInfo = childListInfo.Item;
                        var itemCount = 0UL;
                        var maxOccurrence = childListInfo.MaxOccurrence;
                        List<XChild> itemList = null;
                        while (itemCount <= maxOccurrence) {
                            XChild item;
                            var res = Create(itemInfo, out item);
                            if (res == CreationResult.OK) {
                                Extensions.CreateAndAdd(ref itemList, item);
                                ++itemCount;
                            }
                            else if (res == CreationResult.Skipped) {
                                if (itemCount == 0) {
                                    return res;
                                }
                                if (itemCount < childListInfo.MinOccurrence) {
                                    _context.AddErrorDiag(new DiagMsg(DiagCode.ChildListCountNotGreaterThanOrEqualToMinOccurrence,
                                        childListInfo.DisplayName, itemCount.ToInvString(), childListInfo.MinOccurrence.ToInvString()), GetTextSpan());
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
                        if (itemList.CountOrZero() == 0) {
                            return CreationResult.Skipped;
                        }
                        var container = childInfo.CreateInstance<XChildContainer>();
                        container.TextSpan = itemList[0].TextSpan;
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
            _itemList = new List<T>();
        }
        protected XChildList(IEnumerable<T> items) : this() {
            AddRange(items);
        }
        private List<T> _itemList;
        internal override sealed void InternalAdd(XChild child) {
            Add((T)child);
        }
        public override XObject DeepClone() {
            var obj = (XChildList<T>)base.DeepClone();
            obj._itemList = new List<T>();
            foreach (var child in _itemList) {
                obj.Add(child);
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
        public bool IsReadOnly {
            get {
                return false;
            }
        }
        protected IEnumerator<U> GetEnumeratorCore<U>() where U : T {
            foreach (var item in _itemList) {
                yield return item as U;
            }
        }
        protected void CopyToCore<U>(U[] array, int arrayIndex) where U : T {
            if (array == null) {
                throw new ArgumentNullException("array");
            }
            if (arrayIndex < 0 || arrayIndex > array.Length) {
                throw new ArgumentOutOfRangeException("arrayIndex");
            }
            var count = _itemList.Count;
            if (array.Length - arrayIndex < count) {
                throw new ArgumentException("Insufficient array space.");
            }
            for (var i = 0; i < count; ++i) {
                array[arrayIndex++] = _itemList[i] as U;
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
