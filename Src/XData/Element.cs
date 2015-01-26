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
        internal static CreationResult TrySkippableCreate(Context context, ElementInfo elementInfo, ElementNode elementNode, out XChild result) {
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
                    if (!XComplexType.TryCreate(context, effElementInfo.Program, complexTypeInfo, isNullable,
                        complexValueNode, out complexType)) {
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
            effElement.Type = type;
            if (elementInfo.IsReference) {
                var elementRef = elementInfo.CreateInstance<XElementReference>();
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
        private static bool TryLoadAndValidate<T>(Context context, ElementInfo elementInfo, ElementNode elementNode, out T result) where T : XElement {



            result = null;
            return false;
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

    }
}
