﻿using System;
using System.Collections.Generic;
using XData.IO.Text;

namespace XData {
    public abstract class XObject {
        private XObject _parent;
        private TextSpan _textSpan;
        public TextSpan TextSpan {
            get {
                return _textSpan;
            }
            set {
                _textSpan = value;
            }
        }
        public bool HasTextSpan {
            get {
                return _textSpan.IsValid;
            }
        }
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
                if ((object)this == i) {
                    throw new InvalidOperationException("Circular reference detected.");
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
                var ancestor = obj as T;
                if (ancestor != null) {
                    return ancestor;
                }
            }
            if (!@try) {
                throw new InvalidOperationException("Cannot get ancestor '{0}'.".InvFormat(typeof(T).FullName));
            }
            return null;
        }
        public IEnumerable<T> Ancestors<T>(Func<T, bool> filter = null) where T : class {
            for (var obj = _parent; obj != null; obj = obj._parent) {
                var ancestor = obj as T;
                if (ancestor != null) {
                    if (filter == null || filter(ancestor)) {
                        yield return ancestor;
                    }
                }
            }
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
        public bool TryValidate(DiagContext context) {
            if (context == null) throw new ArgumentNullException("context");
            return TryValidateCore(context);
        }
        internal abstract bool TryValidateCore(DiagContext context);
        internal bool CheckEqualTo(DiagContext context, ObjectInfo otherObjectInfo) {
            var objectInfo = ObjectInfo;
            if (objectInfo != otherObjectInfo) {
                context.AddErrorDiag(new DiagMsg(DiagCode.InvalidObject, objectInfo.DisplayName, otherObjectInfo.DisplayName), this);
                return false;
            }
            return true;
        }

    }














}
