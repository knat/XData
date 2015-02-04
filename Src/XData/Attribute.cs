using XData.IO.Text;

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
        internal void Save(SavingContext context) {
            context.Append(_name);
            if (_type != null) {
                context.StringBuilder.Append(" = ");
                _type.Save(context, AttributeInfo.Type);
                context.AppendLine();
            }
        }
        internal override bool TryValidateCore(DiagContext context) {
            var attributeInfo = AttributeInfo;
            if (_type != null) {
                if (!_type.CheckEqualToOrDeriveFrom(context, attributeInfo.Type)) {
                    return false;
                }
                if (!_type.TryValidate(context)) {
                    return false;
                }
            }
            else if (!attributeInfo.IsNullable) {
                context.AddErrorDiag(new DiagMsg(DiagCode.AttributeIsNotNullable, attributeInfo.DisplayName), this);
                return false;
            }
            return true;
        }
        internal static bool TryCreate(DiagContext context, ProgramInfo programInfo, AttributeInfo attributeInfo,
            AttributeNode attributeNode, out XAttribute result) {
            result = null;
            XSimpleType type = null;
            if (attributeNode.Value.IsValid) {
                if (!XSimpleType.TryCreate(context, programInfo, attributeInfo.Type, attributeNode.Value, out type)) {
                    return false;
                }
            }
            else if (!attributeInfo.IsNullable) {
                context.AddErrorDiag(new DiagMsg(DiagCode.AttributeIsNotNullable, attributeInfo.DisplayName), attributeNode.NameNode.TextSpan);
                return false;
            }
            var attribute = attributeInfo.CreateInstance<XAttribute>();
            attribute.TextSpan = attributeNode.NameNode.TextSpan;
            attribute.Type = type;
            result = attribute;
            return true;
        }
    }

}
