using System;
using XData.IO.Text;

namespace XData {
    public abstract class XChild : XObject {
        public abstract int Order { get; }
    }
    internal enum CreationResult : byte {
        Error,
        Skipped,
        OK
    }
    public abstract class XElement : XChild {
        protected XElement() {
            _fullName = GetFullName();
        }
        private readonly FullName _fullName;
        public FullName FullName {
            get {
                return EffectiveElement._fullName;
            }
        }
        protected abstract FullName GetFullName();
        private XElement _referencedElement;
        public XElement ReferencedElement {
            get {
                return _referencedElement;
            }
            set {
                if (value != null) {
                    if (!ElementInfo.IsReference) {
                        throw new InvalidOperationException("Cannot set referenced element if the element is not a reference.");
                    }
                    for (var i = value; i != null; i = i._referencedElement) {
                        if ((object)this == i) {
                            throw new InvalidOperationException("Circular reference detected.");
                        }
                    }
                }
                _referencedElement = value;
            }
        }
        public XElement GenericReferentialElement {
            get {
                return _referencedElement;
            }
            set {
                ReferencedElement = value;
            }
        }
        public bool IsReference {
            get {
                return _referencedElement != null;
            }
        }
        public XElement EffectiveElement {
            get {
                return _referencedElement == null ? this : _referencedElement.EffectiveElement;
            }
        }
        private XType _type;
        private void SetType(XType type) {
            _type = SetParentTo(type);
        }
        public XType Type {
            get {
                return EffectiveElement._type;
            }
            set {
                if (_referencedElement != null) {
                    _referencedElement.Type = value;
                }
                else {
                    SetType(value);
                }
            }
        }
        public bool HasType {
            get {
                return EffectiveElement._type != null;
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
            if (_referencedElement != null) {
                return _referencedElement.EnsureType<T>(@try);
            }
            var obj = _type as T;
            if (obj != null) return obj;
            obj = ElementInfo.Type.CreateInstance<T>(@try);
            SetType(obj);
            return obj;
        }
        public XType EnsureType(bool @try = false) {
            return EnsureType<XType>(@try);
        }
        public override XObject DeepClone() {
            var obj = (XElement)base.DeepClone();
            obj.SetType(_type);
            return obj;
        }
        public ElementInfo ElementInfo {
            get {
                return (ElementInfo)ObjectInfo;
            }
        }

        //
        internal static CreationResult TrySkippableCreate(Context context, ElementInfo elementInfo, ElementNode elementNode, out XChild result) {
            result = null;
            ElementInfo globalElementInfo;
            if (!elementInfo.IsMatch(elementNode.FullName, out globalElementInfo)) {
                return CreationResult.Skipped;
            }
            var elementNameTextSpan = elementNode.QName.TextSpan;
            var effElementInfo = globalElementInfo ?? elementInfo;
            if (effElementInfo.IsAbstract) {
                context.AddErrorDiag(new DiagMsg(DiagCode.ElementIsAbstract, effElementInfo.DisplayName), elementNameTextSpan);
                return CreationResult.Error;
            }
            XType type = null;
            var elementValueNode = elementNode.Value;
            var isNullable = elementInfo.IsNullable;
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
                    if (!XComplexType.TryCreate(context, elementInfo.Program, complexTypeInfo, isNullable,
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
                    if (!XSimpleType.TryCreate(context, elementInfo.Program, simpleTypeInfo,
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
            effElement.SetType(type);
            if (elementInfo.IsReference) {
                var refElement = elementInfo.CreateInstance<XElement>();
                refElement.ReferencedElement = effElement;
                result = refElement;
            }
            else {
                result = effElement;
            }
            return CreationResult.OK;
        }

        private static bool TryLoadAndValidate<T>(Context context, ElementInfo elementInfo, ElementNode elementNode, out T result) where T : XElement {



            result = null;
            return false;
        }
    }

}
