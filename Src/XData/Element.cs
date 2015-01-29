using System;
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
    }
    internal enum CreationResult : byte {
        Error,
        Skipped,
        OK
    }
    public abstract class XElementBase : XChild {
        public ElementInfo ElementInfo {
            get {
                return (ElementInfo)ObjectInfo;
            }
        }
        internal static CreationResult TrySkippableCreate(DiagContext context, ElementInfo elementInfo, ElementNode elementNode, out XChild result) {
            result = null;
            var effElementInfo = elementInfo.TryGetEffectiveElement(elementNode.FullName);
            if (effElementInfo == null) {
                return CreationResult.Skipped;
            }
            var elementNameTextSpan = elementNode.QName.TextSpan;
            if (effElementInfo.IsAbstract) {
                context.AddErrorDiag(new DiagMsg(DiagCode.ElementIsAbstract, effElementInfo.DisplayName), elementNameTextSpan);
                return CreationResult.Error;
            }
            XType type = null;
            var elementValueNode = elementNode.Value;
            var isNullable = effElementInfo.IsNullable;
            if (elementValueNode.IsValid) {
                var complexTypeInfo = effElementInfo.Type as ComplexTypeInfo;
                if (complexTypeInfo != null) {
                    var complexValueNode = elementValueNode.ComplexValue;
                    if (!complexValueNode.IsValid) {
                        context.AddErrorDiag(new DiagMsg(DiagCode.ElementRequiresComplexTypeValue, effElementInfo.DisplayName),
                            elementNameTextSpan);
                        return CreationResult.Error;
                    }
                    XComplexType complexType;
                    if (!XComplexType.TryCreate(context, effElementInfo.Program, complexTypeInfo, complexValueNode, out complexType)) {
                        return CreationResult.Error;
                    }
                    type = complexType;
                }
                else {
                    var simpleTypeInfo = effElementInfo.Type as SimpleTypeInfo;
                    var simpleValueNode = elementValueNode.SimpleValue;
                    if (!simpleValueNode.IsValid) {
                        context.AddErrorDiag(new DiagMsg(DiagCode.ElementRequiresSimpleTypeValue, effElementInfo.DisplayName),
                            elementNameTextSpan);
                        return CreationResult.Error;
                    }
                    XSimpleType simpleType;
                    if (!XSimpleType.TryCreate(context, effElementInfo.Program, simpleTypeInfo,
                        simpleValueNode, out simpleType)) {
                        return CreationResult.Error;
                    }
                    type = simpleType;
                }
            }
            else {
                if (!isNullable) {
                    context.AddErrorDiag(new DiagMsg(DiagCode.ElementIsNotNullable, effElementInfo.DisplayName), elementNameTextSpan);
                    return CreationResult.Error;
                }
            }
            //
            var effElement = effElementInfo.CreateInstance<XElement>();
            effElement.TextSpan = elementNameTextSpan;
            effElement.Type = type;
            if (elementInfo.IsReference) {
                var elementRef = elementInfo.CreateInstance<XElementReference>();
                elementRef.TextSpan = elementNameTextSpan;
                elementRef.ReferencedElement = effElement;
                result = elementRef;
            }
            else {
                result = effElement;
            }
            return CreationResult.OK;
        }
    }

    public abstract class XElement : XElementBase {
        protected XElement() {
            _fullName = ElementInfo.FullName;
        }
        private readonly FullName _fullName;
        public FullName FullName {
            get {
                return _fullName;
            }
        }
        private XType _type;
        public XType Type {
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
        public XType GenericType {
            get {
                return _type;
            }
            set {
                Type = value;
            }
        }
        public T EnsureType<T>(bool @try = false) where T : XType {
            var obj = _type as T;
            if (obj != null) return obj;
            if ((obj = ElementInfo.Type.CreateInstance<T>(@try)) != null) {
                Type = obj;
            }
            return obj;
        }
        public XType EnsureType(bool @try = false) {
            return EnsureType<XType>(@try);
        }
        public override XObject DeepClone() {
            var obj = (XElement)base.DeepClone();
            obj.Type = _type;
            return obj;
        }
        //
        public void SaveAsRoot(SavingContext context) {

        }
        internal override sealed void Save(SavingContext context) {
            context.Append(_fullName);
            if (_type != null) {
                context.StringBuilder.Append(" = ");
                _type.Save(context, ElementInfo.Type);
                context.AppendLine();
            }
        }
        internal override bool TryValidateCore(DiagContext context) {
            var elementInfo = ElementInfo;
            if (_type != null) {
                if (!_type.CheckEqualToOrDeriveFrom(context, elementInfo.Type)) {
                    return false;
                }
                if (!_type.TryValidate(context)) {
                    return false;
                }
            }
            else if (!elementInfo.IsNullable) {
                context.AddErrorDiag(new DiagMsg(DiagCode.ElementIsNotNullable, elementInfo.DisplayName), this);
                return false;
            }
            return true;
        }
        //
        internal static bool TryCreate<T>(DiagContext context, ElementInfo elementInfo, ElementNode elementNode, out T result) where T : XElement {
            if (!elementInfo.IsGlobal) throw new ArgumentException("!elementInfo.IsGlobal");
            result = null;
            XChild child;
            var res = TrySkippableCreate(context, elementInfo, elementNode, out child);
            if (res == CreationResult.Error) {
                return false;
            }
            if (res == CreationResult.Skipped) {
                context.AddErrorDiag(new DiagMsg(DiagCode.InvalidElement, elementNode.FullName.ToString(), elementInfo.DisplayName),
                    elementNode.QName.TextSpan);
                return false;
            }
            result = (T)child;
            return true;
        }
    }
    public abstract class XElementReference : XElementBase {
        private XElement _referencedElement;
        public XElement ReferencedElement {
            get {
                return _referencedElement;
            }
            set {
                _referencedElement = value;
            }
        }
        public XElement GenericReferentialElement {
            get {
                return _referencedElement;
            }
            set {
                _referencedElement = value;
            }
        }
        public T EnsureReferencedElement<T>(bool @try = false) where T : XElement {
            var obj = _referencedElement as T;
            if (obj != null) return obj;
            _referencedElement = obj = ElementInfo.ReferencedElement.CreateInstance<T>(@try);
            return obj;
        }
        public XElement EnsureReferencedElement(bool @try = false) {
            return EnsureReferencedElement<XElement>(@try);
        }
        public FullName FullName {
            get {
                return _referencedElement != null ? _referencedElement.FullName : default(FullName);
            }
        }
        public XType Type {
            get {
                return _referencedElement != null ? _referencedElement.Type : null;
            }
            set {
                EnsureReferencedElement<XElement>().Type = value;
            }
        }
        public XType GenericType {
            get {
                return Type;
            }
            set {
                Type = value;
            }
        }
        public T EnsureType<T>(bool @try = false) where T : XType {
            var referencedElement = EnsureReferencedElement<XElement>(@try);
            if (referencedElement == null) return null;
            return referencedElement.EnsureType<T>(@try);
        }
        public XType EnsureType(bool @try = false) {
            return EnsureType<XType>(@try);
        }
        internal override sealed void Save(SavingContext context) {
            throw new NotImplementedException();
        }
    }
}
