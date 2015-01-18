using System;

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
                if ((object)this == i) {
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














}
