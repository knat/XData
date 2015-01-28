namespace XData {

    public abstract class XAttribute : XObject {
        protected XAttribute() {
            _name = AttributeInfo.Name;
        }
        private readonly string _name;
        public string Name {
            get {
                return _name;
            }
        }

        private XSimpleType _type;
        public XSimpleType Type {
            get {
                return _type;
            }
            set {
                _type = SetParentTo(value);
            }
        }
        public bool HasType {
            get {
                return _type != null;
            }
        }
        public XSimpleType GenericType {
            get {
                return _type;
            }
            set {
                Type = value;
            }
        }
        public T EnsureType<T>(bool @try = false) where T : XSimpleType {
            var obj = _type as T;
            if (obj != null) return obj;
            if ((obj = AttributeInfo.Type.CreateInstance<T>(@try)) != null) {
                Type = obj;
            }
            return obj;
        }
        public XSimpleType EnsureType(bool @try = false) {
            return EnsureType<XSimpleType>(@try);
        }
        public override XObject DeepClone() {
            var obj = (XAttribute)base.DeepClone();
            obj.Type = _type;
            return obj;
        }
        public AttributeInfo AttributeInfo {
            get {
                return (AttributeInfo)ObjectInfo;
            }
        }

    }


}
