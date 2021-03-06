﻿using System;
using System.Collections.Generic;
using System.Linq;
using XData.IO.Text;

namespace XData {
    public abstract class XChild : XObject {
        protected XChild() {
            _order = ChildInfo.Order;
        }
        private readonly int _order;
        public int Order {
            get {
                return _order;
            }
        }
        public ChildInfo ChildInfo {
            get {
                return (ChildInfo)ObjectInfo;
            }
        }
        internal abstract void Save(SavingContext context);
        internal enum CreationResult : byte {
            Error,
            Skipped,
            OK
        }
    }

    public abstract class XChildContainer : XChild {
        #region LINQ
        public IEnumerable<T> ChildElements<T>(Func<T, bool> filter = null) where T : XElement {
            foreach (var child in InternalChildren) {
                var element = child as T;
                if (element != null) {
                    if (filter == null || filter(element)) {
                        yield return element;
                    }
                }
                else {
                    foreach (var i in ((XChildContainer)child).ChildElements(filter)) {
                        yield return i;
                    }
                }
            }
        }
        public IEnumerable<T> ChildElementTypes<T>(Func<XElement, bool> elementFilter = null,
            Func<T, bool> typeFilter = null) where T : XType {
            foreach (var child in InternalChildren) {
                var element = child as XElement;
                if (element != null) {
                    if (elementFilter == null || elementFilter(element)) {
                        var type = element.Type as T;
                        if (type != null) {
                            if (typeFilter == null || typeFilter(type)) {
                                yield return type;
                            }
                        }
                    }
                }
                else {
                    foreach (var i in ((XChildContainer)child).ChildElementTypes(elementFilter, typeFilter)) {
                        yield return i;
                    }
                }
            }
        }
        public IEnumerable<T> ChildElementAttributes<T>(Func<XElement, bool> elementFilter = null,
            Func<T, bool> attributeFilter = null) where T : XAttribute {
            foreach (var child in InternalChildren) {
                var element = child as XElement;
                if (element != null) {
                    if (elementFilter == null || elementFilter(element)) {
                        foreach (var i in element.SelfAttributes(attributeFilter)) {
                            yield return i;
                        }
                    }
                }
                else {
                    foreach (var i in ((XChildContainer)child).ChildElementAttributes(elementFilter, attributeFilter)) {
                        yield return i;
                    }
                }
            }
        }
        public IEnumerable<T> ChildElementAttributeTypes<T>(Func<XElement, bool> elementFilter = null,
            Func<XAttribute, bool> attributeFilter = null, Func<T, bool> typeFilter = null) where T : XSimpleType {
            foreach (var child in InternalChildren) {
                var element = child as XElement;
                if (element != null) {
                    if (elementFilter == null || elementFilter(element)) {
                        foreach (var i in element.SelfAttributeTypes(attributeFilter, typeFilter)) {
                            yield return i;
                        }
                    }
                }
                else {
                    foreach (var i in ((XChildContainer)child).ChildElementAttributeTypes(elementFilter, attributeFilter, typeFilter)) {
                        yield return i;
                    }
                }
            }
        }
        public IEnumerable<T> ChildElementChildren<T>(Func<XElement, bool> elementFilter = null,
            Func<T, bool> childrenFilter = null) where T : XObject {
            foreach (var child in InternalChildren) {
                var element = child as XElement;
                if (element != null) {
                    if (elementFilter == null || elementFilter(element)) {
                        var children = element.Children as T;
                        if (children != null) {
                            if (childrenFilter == null || childrenFilter(children)) {
                                yield return children;
                            }
                        }
                    }
                }
                else {
                    foreach (var i in ((XChildContainer)child).ChildElementChildren(elementFilter, childrenFilter)) {
                        yield return i;
                    }
                }
            }
        }
        public IEnumerable<T> DescendantElements<T>(Func<T, bool> filter = null) where T : XElement {
            foreach (var child in InternalChildren) {
                var element = child as T;
                if (element != null) {
                    if (filter == null || filter(element)) {
                        yield return element;
                    }
                    foreach (var i in element.DescendantElements(filter)) {
                        yield return i;
                    }
                }
                else {
                    foreach (var i in ((XChildContainer)child).DescendantElements(filter)) {
                        yield return i;
                    }
                }
            }
        }
        public IEnumerable<T> DescendantElementTypes<T>(Func<XElement, bool> elementFilter = null,
            Func<T, bool> typeFilter = null) where T : XType {
            foreach (var child in InternalChildren) {
                var element = child as XElement;
                if (element != null) {
                    if (elementFilter == null || elementFilter(element)) {
                        var type = element.Type as T;
                        if (type != null) {
                            if (typeFilter == null || typeFilter(type)) {
                                yield return type;
                            }
                        }
                    }
                    foreach (var i in element.DescendantElementTypes(elementFilter, typeFilter)) {
                        yield return i;
                    }
                }
                else {
                    foreach (var i in ((XChildContainer)child).DescendantElementTypes(elementFilter, typeFilter)) {
                        yield return i;
                    }
                }
            }
        }
        public IEnumerable<T> DescendantElementAttributes<T>(Func<XElement, bool> elementFilter = null,
            Func<T, bool> attributeFilter = null) where T : XAttribute {
            foreach (var child in InternalChildren) {
                var element = child as XElement;
                if (element != null) {
                    if (elementFilter == null || elementFilter(element)) {
                        foreach (var i in element.SelfAttributes(attributeFilter)) {
                            yield return i;
                        }
                    }
                    foreach (var i in element.DescendantElementAttributes(elementFilter, attributeFilter)) {
                        yield return i;
                    }
                }
                else {
                    foreach (var i in ((XChildContainer)child).DescendantElementAttributes(elementFilter, attributeFilter)) {
                        yield return i;
                    }
                }
            }
        }
        public IEnumerable<T> DescendantElementAttributeTypes<T>(Func<XElement, bool> elementFilter = null,
            Func<XAttribute, bool> attributeFilter = null, Func<T, bool> typeFilter = null) where T : XSimpleType {
            foreach (var child in InternalChildren) {
                var element = child as XElement;
                if (element != null) {
                    if (elementFilter == null || elementFilter(element)) {
                        foreach (var i in element.SelfAttributeTypes(attributeFilter, typeFilter)) {
                            yield return i;
                        }
                    }
                    foreach (var i in element.DescendantElementAttributeTypes(elementFilter, attributeFilter, typeFilter)) {
                        yield return i;
                    }
                }
                else {
                    foreach (var i in ((XChildContainer)child).DescendantElementAttributeTypes(elementFilter, attributeFilter, typeFilter)) {
                        yield return i;
                    }
                }
            }
        }
        public IEnumerable<T> DescendantElementChildren<T>(Func<XElement, bool> elementFilter = null,
            Func<T, bool> childrenFilter = null) where T : XObject {
            foreach (var child in InternalChildren) {
                var element = child as XElement;
                if (element != null) {
                    if (elementFilter == null || elementFilter(element)) {
                        var children = element.Children as T;
                        if (children != null) {
                            if (childrenFilter == null || childrenFilter(children)) {
                                yield return children;
                            }
                        }
                    }
                    foreach (var i in element.DescendantElementChildren(elementFilter, childrenFilter)) {
                        yield return i;
                    }
                }
                else {
                    foreach (var i in ((XChildContainer)child).DescendantElementChildren(elementFilter, childrenFilter)) {
                        yield return i;
                    }
                }
            }
        }
        #endregion LINQ
        internal abstract IEnumerable<XChild> InternalChildren { get; }
    }
    public abstract class XChildCollection : XChildContainer, ICollection<XChild>, IReadOnlyCollection<XChild> {
        protected XChildCollection() {
            _list = new List<XChild>();
        }
        internal List<XChild> _list;
        public override XObject DeepClone() {
            var obj = (XChildCollection)base.DeepClone();
            obj._list = new List<XChild>();
            foreach (var child in _list) {
                obj._list.Add(obj.SetParentTo(child));
            }
            return obj;
        }
        internal override sealed IEnumerable<XChild> InternalChildren {
            get {
                return _list;
            }
        }
        internal void InternalAdd(XChild child) {
            _list.Add(SetParentTo(child));
        }
        public abstract void Add(XChild child);
        public abstract void AddOrSetChild(XChild child);
        public void AddRange(IEnumerable<XChild> children) {
            if (children != null) {
                foreach (var child in children) {
                    Add(child);
                }
            }
        }
        internal int IndexOf(int order) {
            var list = _list;
            var count = list.Count;
            for (var i = 0; i < count; ++i) {
                if (list[i].Order == order) {
                    return i;
                }
            }
            return -1;
        }
        public XChild TryGetChild(int order) {
            var idx = IndexOf(order);
            if (idx != -1) {
                return _list[idx];
            }
            return null;
        }
        public bool ContainsChild(int order) {
            return IndexOf(order) != -1;
        }
        public bool Contains(XChild child) {
            if (child == null) throw new ArgumentNullException("child");
            return ContainsChild(child.Order);
        }
        public bool RemoveChild(int order) {
            var idx = IndexOf(order);
            if (idx != -1) {
                _list.RemoveAt(idx);
                return true;
            }
            return false;
        }
        public bool Remove(XChild child) {
            if (child == null) throw new ArgumentNullException("child");
            return RemoveChild(child.Order);
        }
        public int Count {
            get {
                return _list.Count;
            }
        }
        public void Clear() {
            _list.Clear();
        }
        public List<XChild>.Enumerator GetEnumerator() {
            return _list.GetEnumerator();
        }
        IEnumerator<XChild> IEnumerable<XChild>.GetEnumerator() {
            return GetEnumerator();
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
        public void CopyTo(XChild[] array, int arrayIndex) {
            _list.CopyTo(array, arrayIndex);
        }
        bool ICollection<XChild>.IsReadOnly {
            get {
                return false;
            }
        }
        public T CreateChild<T>(int order, bool @try = false) where T : XChild {
            var childInfo = ChildStructInfo.TryGetChild(order);
            if (childInfo == null) {
                if (@try) {
                    return null;
                }
                throw new InvalidOperationException("Cannot find child '{0}'.".InvFormat(order.ToInvString()));
            }
            return childInfo.CreateInstance<T>(@try);
        }
        internal override sealed void Save(SavingContext context) {
            foreach (var child in _list) {
                child.Save(context);
            }
        }
        internal void SaveAsRoot(SavingContext context) {
            context.AppendLine('{');
            context.PushIndent();
            foreach (var child in _list) {
                child.Save(context);
            }
            context.PopIndent();
            context.Append('}');
        }
        public ChildStructInfo ChildStructInfo {
            get {
                return (ChildStructInfo)ObjectInfo;
            }
        }
        internal override sealed bool TryValidateCore(DiagContext context) {
            var childStructInfo = ChildStructInfo;
            var childList = new List<XChild>(_list);
            var dMarker = context.MarkDiags();
            if (childStructInfo.Children != null) {
                var isSeq = childStructInfo.IsSequence;
                foreach (var childInfo in childStructInfo.Children) {
                    var found = false;
                    for (var i = 0; i < childList.Count; ++i) {
                        var child = childList[i];
                        if (child.Order == childInfo.Order) {
                            if (child.CheckEqualTo(context, childInfo)) {
                                child.TryValidate(context);
                            }
                            childList.RemoveAt(i);
                            found = true;
                            break;
                        }
                        else if (isSeq && child.Order > childInfo.Order) {
                            break;
                        }
                    }
                    if (!found && !childInfo.IsOptional) {
                        context.AddErrorDiag(new DiagMsg(DiagCode.RequiredChildNotSet, childInfo.DisplayName), this);
                    }
                }
            }
            if (childList.Count > 0) {
                foreach (var child in childList) {
                    context.AddErrorDiag(new DiagMsg(DiagCode.RedundantChild, child.ObjectInfo.DisplayName), child);
                }
            }
            return !dMarker.HasErrors;
        }

        internal static bool TryCreate(DiagContext context, ChildStructInfo childStructInfo,
            NodeList<ElementNode> elementNodeList, TextSpan closeChildrenTextSpan, out XChildCollection result) {
            if (childStructInfo.IsSet) {
                result = null;
                var dMarker = context.MarkDiags();
                List<XElement> elementList = null;
                if (childStructInfo.Children != null) {
                    foreach (ElementInfo elementInfo in childStructInfo.Children) {
                        var found = false;
                        if (elementNodeList != null) {
                            for (var i = 0; i < elementNodeList.Count; ++i) {
                                var elementNode = elementNodeList[i];
                                XElement element;
                                var res = XElement.TrySkippableCreate(context, elementInfo, elementNode, out element);
                                if (res == CreationResult.OK) {
                                    Extensions.CreateAndAdd(ref elementList, element);
                                    elementNodeList.RemoveAt(i);
                                    found = true;
                                    break;
                                }
                            }
                        }
                        if (!found && !elementInfo.IsOptional) {
                            context.AddErrorDiag(new DiagMsg(DiagCode.RequiredChildNotMatched, elementInfo.DisplayName),
                                closeChildrenTextSpan);
                        }
                    }
                }
                if (elementNodeList.CountOrZero() > 0) {
                    foreach (var elementNode in elementNodeList) {
                        context.AddErrorDiag(new DiagMsg(DiagCode.RedundantChild, elementNode.FullName.ToString()),
                            elementNode.QName.TextSpan);
                    }
                }
                if (dMarker.HasErrors) {
                    return false;
                }
                if (elementNodeList != null) {
                    var childSet = childStructInfo.CreateInstance<XChildSet>();
                    childSet.TextSpan = elementNodeList.OpenTokenTextSpan;
                    if (elementList != null) {
                        foreach (var element in elementList) {
                            childSet.InternalAdd(element);
                        }
                    }
                    result = childSet;
                }
                return true;
            }
            return new CreationStruct().TryCreate(context, childStructInfo, elementNodeList, closeChildrenTextSpan, out result);
        }

        private struct CreationStruct {
            internal bool TryCreate(DiagContext context, ChildStructInfo childStructInfo,
                NodeList<ElementNode> elementNodeList, TextSpan closeChildrenTextSpan, out XChildCollection result) {
                _context = context;
                _elementNodeList = elementNodeList;
                _closeChildrenTextSpan = closeChildrenTextSpan;
                _count = elementNodeList.CountOrZero();
                _index = 0;
                //
                result = null;
                XChild child;
                var res = Create(childStructInfo, out child);
                if (res == CreationResult.Error) {
                    return false;
                }
                var childColl = (XChildCollection)child;
                if (childColl == null) {
                    if (!childStructInfo.IsOptional) {
                        context.AddErrorDiag(new DiagMsg(DiagCode.RequiredChildNotMatched, childStructInfo.DisplayName),
                            closeChildrenTextSpan);
                        return false;
                    }
                }
                if (!IsEOF) {
                    while (!IsEOF) {
                        var elementNode = GetElementNode();
                        context.AddErrorDiag(new DiagMsg(DiagCode.RedundantChild, elementNode.FullName.ToString()),
                            elementNode.QName.TextSpan);
                        ConsumeElementNode();
                    }
                    return false;
                }
                if (childColl == null) {
                    if (elementNodeList != null) {
                        childColl = childStructInfo.CreateInstance<XChildCollection>();
                        childColl.TextSpan = elementNodeList.OpenTokenTextSpan;
                    }
                }
                result = childColl;
                return true;
            }
            private DiagContext _context;
            private NodeList<ElementNode> _elementNodeList;
            private TextSpan _closeChildrenTextSpan;
            private int _count, _index;
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
                    XElement element;
                    var res = XElement.TrySkippableCreate(_context, elementInfo, GetElementNode(), out element);
                    if (res == CreationResult.OK) {
                        result = element;
                        ConsumeElementNode();
                    }
                    return res;
                }
                else {
                    var childStructInfo = childInfo as ChildStructInfo;
                    if (childStructInfo != null) {
                        if (childStructInfo.IsSequence) {
                            List<XChild> childList = null;
                            if (childStructInfo.Children != null) {
                                foreach (var memberChildInfo in childStructInfo.Children) {
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
                                            _context.AddErrorDiag(new DiagMsg(DiagCode.RequiredChildNotMatched, memberChildInfo.DisplayName),
                                                GetTextSpan());
                                            return CreationResult.Error;
                                        }
                                    }
                                    else {//error
                                        return res;
                                    }
                                }
                            }
                            if (childList.CountOrZero() == 0) {
                                return CreationResult.Skipped;
                            }
                            var childSeq = childInfo.CreateInstance<XChildSequence>();
                            childSeq.TextSpan = childList[0].TextSpan;
                            foreach (var child in childList) {
                                childSeq.InternalAdd(child);
                            }
                            result = childSeq;
                            return CreationResult.OK;
                        }
                        else {//choice
                            XChild choice = null;
                            if (childStructInfo.Children != null) {
                                foreach (var memberChildInfo in childStructInfo.Children) {
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
                            }
                            if (choice == null) {
                                return CreationResult.Skipped;
                            }
                            var childChoice = childInfo.CreateInstance<XChildChoice>();
                            childChoice.TextSpan = choice.TextSpan;
                            childChoice.Choice = choice;
                            result = childChoice;
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
                        var childList = childInfo.CreateInstance<XChildList>();
                        childList.TextSpan = itemList[0].TextSpan;
                        foreach (var item in itemList) {
                            childList.InternalAdd(item);
                        }
                        result = childList;
                        return CreationResult.OK;
                    }
                }
            }

        }
    }
    public abstract class XChildSet : XChildCollection {
        public override sealed void Add(XChild child) {
            if (Contains(child)) {
                throw new ArgumentException("Child '{0}' already exists.".InvFormat(child.Order.ToInvString()));
            }
            _list.Add(SetParentTo(child));
        }
        public override sealed void AddOrSetChild(XChild child) {
            if (child == null) throw new ArgumentNullException("child");
            var idx = IndexOf(child.Order);
            if (idx == -1) {
                _list.Add(SetParentTo(child));
            }
            else {
                _list[idx] = SetParentTo(child);
            }
        }
    }
    public abstract class XChildSequence : XChildCollection {
        private bool TryGetIndexOf(int order, out int index) {
            int i;
            var found = false;
            var list = _list;
            var count = list.Count;
            for (i = 0; i < count; ++i) {
                var itemOrder = list[i].Order;
                if (itemOrder == order) {
                    found = true;
                    break;
                }
                if (itemOrder > order) {
                    break;
                }
            }
            index = i;
            return found;
        }
        public override sealed void Add(XChild child) {
            if (child == null) throw new ArgumentNullException("child");
            var order = child.Order;
            int index;
            if (TryGetIndexOf(order, out index)) {
                throw new ArgumentException("Child '{0}' already exists.".InvFormat(order.ToInvString()));
            }
            _list.Insert(index, SetParentTo(child));
        }
        public override sealed void AddOrSetChild(XChild child) {
            if (child == null) throw new ArgumentNullException("child");
            int index;
            if (TryGetIndexOf(child.Order, out index)) {
                _list[index] = SetParentTo(child);
            }
            else {
                _list.Insert(index, SetParentTo(child));
            }
        }

    }
    public abstract class XChildChoice : XChildContainer {
        private XChild _choice;
        public XChild Choice {
            get {
                return _choice;
            }
            set {
                _choice = SetParentTo(value);
            }
        }
        public XChild TryGetChild(int order) {
            return _choice;
        }
        public void AddOrSetChild(XChild child) {
            Choice = child;
        }
        public bool RemoveChild(int order) {
            _choice = null;
            return true;
        }
        public T CreateChild<T>(int order, bool @try = false) where T : XChild {
            var childInfo = ChildStructInfo.TryGetChild(order);
            if (childInfo == null) {
                if (@try) {
                    return null;
                }
                throw new InvalidOperationException("Cannot find child '{0}'.".InvFormat(order.ToInvString()));
            }
            return childInfo.CreateInstance<T>(@try);
        }
        internal override sealed void Save(SavingContext context) {
            if (_choice != null) {
                _choice.Save(context);
            }
        }
        internal override sealed IEnumerable<XChild> InternalChildren {
            get {
                if (_choice != null) {
                    return Enumerable.Repeat(_choice, 1);
                }
                return Enumerable.Empty<XChild>();
            }
        }
        public ChildStructInfo ChildStructInfo {
            get {
                return (ChildStructInfo)ObjectInfo;
            }
        }
        internal override bool TryValidateCore(DiagContext context) {
            var childStructInfo = ChildStructInfo;
            var choice = _choice;
            var dMarker = context.MarkDiags();
            if (choice != null) {
                var found = false;
                if (childStructInfo.Children != null) {
                    foreach (var childInfo in childStructInfo.Children) {
                        if (choice.Order == childInfo.Order) {
                            if (choice.CheckEqualTo(context, childInfo)) {
                                choice.TryValidate(context);
                            }
                            found = true;
                            break;
                        }
                    }
                }
                if (!found) {
                    context.AddErrorDiag(new DiagMsg(DiagCode.RedundantChild, choice.ObjectInfo.DisplayName), choice);
                }
            }
            else {
                context.AddErrorDiag(new DiagMsg(DiagCode.ChoiceNotSet, childStructInfo.DisplayName), this);
            }
            return !dMarker.HasErrors;
        }

    }
    public abstract class XChildList : XChildContainer {
        public ChildListInfo ChildListInfo {
            get {
                return (ChildListInfo)ObjectInfo;
            }
        }
        internal abstract void InternalAdd(XChild child);
    }
    public abstract class XChildList<T> : XChildList, IList<T>, IReadOnlyList<T> where T : XChild {
        protected XChildList() {
            _list = new List<T>();
        }
        protected XChildList(IEnumerable<T> items)
            : this() {
            AddRange(items);
        }
        private List<T> _list;
        public override XObject DeepClone() {
            var obj = (XChildList<T>)base.DeepClone();
            obj._list = new List<T>();
            foreach (var child in _list) {
                obj.Add(child);
            }
            return obj;
        }
        public int Count {
            get {
                return _list.Count;
            }
        }
        public T this[int index] {
            get {
                return _list[index];
            }
            set {
                _list[index] = SetParentTo(value, false);
            }
        }
        public void AddRange(IEnumerable<T> items) {
            if (items != null) {
                foreach (var item in items) {
                    Add(item);
                }
            }
        }
        public void Add(T item) {
            _list.Add(SetParentTo(item, false));
        }
        public void Insert(int index, T item) {
            _list.Insert(index, SetParentTo(item, false));
        }
        public bool Remove(T item) {
            return _list.Remove(item);
        }
        public void RemoveAt(int index) {
            _list.RemoveAt(index);
        }
        public void Clear() {
            _list.Clear();
        }
        public int IndexOf(T item) {
            return _list.IndexOf(item);
        }
        public bool Contains(T item) {
            return _list.Contains(item);
        }
        public void CopyTo(T[] array, int arrayIndex) {
            _list.CopyTo(array, arrayIndex);
        }
        public List<T>.Enumerator GetEnumerator() {
            return _list.GetEnumerator();
        }
        IEnumerator<T> IEnumerable<T>.GetEnumerator() {
            return GetEnumerator();
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
        public bool IsReadOnly {
            get {
                return false;
            }
        }
        protected IEnumerator<U> GetEnumeratorCore<U>() where U : T {
            foreach (var item in _list) {
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
            var count = _list.Count;
            if (array.Length - arrayIndex < count) {
                throw new ArgumentException("Insufficient array space.");
            }
            for (var i = 0; i < count; ++i) {
                array[arrayIndex++] = _list[i] as U;
            }
        }
        protected U CreateItem<U>() where U : T {
            return ChildListInfo.Item.CreateInstance<U>();
        }
        protected U CreateAndAddItem<U>() where U : T {
            var item = CreateItem<U>();
            Add(item);
            return item;
        }
        protected void Add<U>(Action<U> itemSetter) where U : T {
            if (itemSetter == null) throw new ArgumentNullException("itemSetter");
            var item = CreateItem<U>();
            itemSetter(item);
            Add(item);
        }
        protected void AddRange<U, TItemValue>(IEnumerable<TItemValue> itemValues, Action<U, TItemValue> itemSetter) where U : T {
            if (itemValues != null) {
                if (itemSetter == null) throw new ArgumentNullException("itemSetter");
                foreach (var itemValue in itemValues) {
                    var item = CreateItem<U>();
                    itemSetter(item, itemValue);
                    Add(item);
                }
            }
        }
        internal override sealed IEnumerable<XChild> InternalChildren {
            get {
                return _list;
            }
        }
        internal override sealed void InternalAdd(XChild child) {
            Add((T)child);
        }
        internal override sealed void Save(SavingContext context) {
            foreach (var child in _list) {
                child.Save(context);
            }
        }
        internal override bool TryValidateCore(DiagContext context) {
            var childListInfo = ChildListInfo;
            var itemInfo = childListInfo.Item;
            ulong itemCount = 0;
            var maxOccurrence = childListInfo.MaxOccurrence;
            var dMarker = context.MarkDiags();
            foreach (var item in _list) {
                ++itemCount;
                if (itemCount > maxOccurrence) {
                    context.AddErrorDiag(new DiagMsg(DiagCode.RedundantChild, item.ObjectInfo.DisplayName), item);
                }
                else if (item.CheckEqualTo(context, itemInfo)) {
                    item.TryValidate(context);
                }
            }
            if (itemCount < childListInfo.MinOccurrence) {
                context.AddErrorDiag(new DiagMsg(DiagCode.ChildListCountNotGreaterThanOrEqualToMinOccurrence,
                    childListInfo.DisplayName, itemCount.ToInvString(), childListInfo.MinOccurrence.ToInvString()), this);
            }
            return !dMarker.HasErrors;
        }
    }
}
