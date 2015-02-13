using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XData.IO.Text;

namespace XData {
    public abstract class XElement : XChild {
        public abstract FullName FullName { get; }
        public abstract XType Type { get; set; }
        public XType GenericType {
            get {
                return Type;
            }
            set {
                Type = value;
            }
        }
        public bool HasType {
            get {
                return Type != null;
            }
        }
        //
        //
        #region LINQ
        private XAttributeSet Attributes {
            get {
                var complexType = Type as XComplexType;
                if (complexType != null) {
                    return complexType.Attributes;
                }
                return null;
            }
        }
        internal XObject Children {
            get {
                var complexType = Type as XComplexType;
                if (complexType != null) {
                    return complexType.Children;
                }
                return null;
            }
        }
        public IEnumerable<T> SelfAttributes<T>(Func<T, bool> filter = null) where T : XAttribute {
            var attributes = Attributes;
            if (attributes != null) {
                return attributes.SelfAttributes(filter);
            }
            return Enumerable.Empty<T>();
        }
        public IEnumerable<T> SelfAttributeTypes<T>(Func<XAttribute, bool> attributeFilter = null, Func<T, bool> typeFilter = null) where T : XSimpleType {
            var attributes = Attributes;
            if (attributes != null) {
                return attributes.SelfAttributeTypes(attributeFilter, typeFilter);
            }
            return Enumerable.Empty<T>();
        }
        public IEnumerable<T> SubElements<T>(Func<T, bool> filter = null) where T : XElement {
            var complexChildren = Children as XChildCollection;
            if (complexChildren != null) {
                return complexChildren.SubElements(filter);
            }
            return Enumerable.Empty<T>();
        }
        public IEnumerable<T> SubElementTypes<T>(Func<XElement, bool> elementFilter = null,
            Func<T, bool> typeFilter = null) where T : XType {
            var complexChildren = Children as XChildCollection;
            if (complexChildren != null) {
                return complexChildren.SubElementTypes(elementFilter, typeFilter);
            }
            return Enumerable.Empty<T>();
        }
        public IEnumerable<T> SubElementAttributes<T>(Func<XElement, bool> elementFilter = null,
            Func<T, bool> attributeFilter = null) where T : XAttribute {
            var complexChildren = Children as XChildCollection;
            if (complexChildren != null) {
                return complexChildren.SubElementAttributes(elementFilter, attributeFilter);
            }
            return Enumerable.Empty<T>();
        }
        public IEnumerable<T> SubElementAttributeTypes<T>(Func<XElement, bool> elementFilter = null,
            Func<XAttribute, bool> attributeFilter = null, Func<T, bool> typeFilter = null) where T : XSimpleType {
            var complexChildren = Children as XChildCollection;
            if (complexChildren != null) {
                return complexChildren.SubElementAttributeTypes(elementFilter, attributeFilter, typeFilter);
            }
            return Enumerable.Empty<T>();
        }
        public IEnumerable<T> SubElementChildren<T>(Func<XElement, bool> elementFilter = null,
            Func<T, bool> childrenFilter = null) where T : XObject {
            var complexChildren = Children as XChildCollection;
            if (complexChildren != null) {
                return complexChildren.SubElementChildren(elementFilter, childrenFilter);
            }
            return Enumerable.Empty<T>();
        }
        public IEnumerable<T> DescendantElements<T>(Func<T, bool> filter = null) where T : XElement {
            var complexChildren = Children as XChildCollection;
            if (complexChildren != null) {
                return complexChildren.DescendantElements(filter);
            }
            return Enumerable.Empty<T>();
        }
        public IEnumerable<T> DescendantElementTypes<T>(Func<XElement, bool> elementFilter = null,
            Func<T, bool> typeFilter = null) where T : XType {
            var complexChildren = Children as XChildCollection;
            if (complexChildren != null) {
                return complexChildren.DescendantElementTypes(elementFilter, typeFilter);
            }
            return Enumerable.Empty<T>();
        }
        public IEnumerable<T> DescendantElementAttributes<T>(Func<XElement, bool> elementFilter = null,
            Func<T, bool> attributeFilter = null) where T : XAttribute {
            var complexChildren = Children as XChildCollection;
            if (complexChildren != null) {
                return complexChildren.DescendantElementAttributes(elementFilter, attributeFilter);
            }
            return Enumerable.Empty<T>();
        }
        public IEnumerable<T> DescendantElementAttributeTypes<T>(Func<XElement, bool> elementFilter = null,
            Func<XAttribute, bool> attributeFilter = null, Func<T, bool> typeFilter = null) where T : XSimpleType {
            var complexChildren = Children as XChildCollection;
            if (complexChildren != null) {
                return complexChildren.DescendantElementAttributeTypes(elementFilter, attributeFilter, typeFilter);
            }
            return Enumerable.Empty<T>();
        }
        public IEnumerable<T> DescendantElementChildren<T>(Func<XElement, bool> elementFilter = null,
            Func<T, bool> childrenFilter = null) where T : XObject {
            var complexChildren = Children as XChildCollection;
            if (complexChildren != null) {
                return complexChildren.DescendantElementChildren(elementFilter, childrenFilter);
            }
            return Enumerable.Empty<T>();
        }



        #endregion LINQ
        //
        public ElementInfo ElementInfo {
            get {
                return (ElementInfo)ObjectInfo;
            }
        }
        internal static CreationResult TrySkippableCreate(DiagContext context, ElementInfo elementInfo, ElementNode elementNode, out XElement result) {
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
                        context.AddErrorDiag(new DiagMsg(DiagCode.ComplexValueRequiredForElement, effElementInfo.DisplayName),
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
                        context.AddErrorDiag(new DiagMsg(DiagCode.SimpleValueRequiredForElement, effElementInfo.DisplayName),
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
                    context.AddErrorDiag(new DiagMsg(DiagCode.ElementValueNotSet, effElementInfo.DisplayName),
                        elementNameTextSpan);
                    return CreationResult.Error;
                }
            }
            //
            var effElement = effElementInfo.CreateInstance<XEntityElement>();
            effElement.TextSpan = elementNameTextSpan;
            effElement.Type = type;
            if (elementInfo.IsGlobalRef) {
                var elementRef = elementInfo.CreateInstance<XGlobalElementRef>();
                elementRef.TextSpan = elementNameTextSpan;
                elementRef.GlobalElement = (XGlobalElement)effElement;
                result = elementRef;
            }
            else {
                result = effElement;
            }
            return CreationResult.OK;
        }
    }

    public abstract class XEntityElement : XElement {
        protected XEntityElement() {
            _fullName = ElementInfo.FullName;
        }
        private readonly FullName _fullName;
        public override sealed FullName FullName {
            get {
                return _fullName;
            }
        }
        private XType _type;
        public override sealed XType Type {
            get {
                return _type;
            }
            set {
                _type = SetParentTo(value);
            }
        }
        public override XObject DeepClone() {
            var obj = (XEntityElement)base.DeepClone();
            obj.Type = _type;
            return obj;
        }
        //
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
                context.AddErrorDiag(new DiagMsg(DiagCode.ElementValueNotSet, elementInfo.DisplayName), this);
                return false;
            }
            return true;
        }
    }
    public abstract class XLocalElement : XEntityElement {
    }
    public abstract class XGlobalElement : XEntityElement {
        public void Save(StringBuilder stringBuilder,
            string indentString = Extensions.DefaultIndentString, string newLineString = Extensions.DefaultNewLineString) {
            if (stringBuilder == null) throw new ArgumentNullException("stringBuilder");
            SaveAsRoot(new SavingContext(stringBuilder, indentString, newLineString));
        }
        public void Save(System.IO.TextWriter writer,
            string indentString = Extensions.DefaultIndentString, string newLineString = Extensions.DefaultNewLineString) {
            if (writer == null) throw new ArgumentNullException("writer");
            var sb = new StringBuilder(1024 * 2);
            Save(sb, indentString, newLineString);
            writer.Write(sb.ToString());
        }
        private void SaveAsRoot(SavingContext context) {
            var fullName = FullName;
            var alias = context.AddUri(fullName.Uri);
            var type = Type;
            if (type != null) {
                context.StringBuilder.Append(" = ");
                type.Save(context, ElementInfo.Type);
                context.AppendLine();
            }
            context.InsertRootElement(alias, fullName.Name);
        }
        internal bool CheckEqualToOrSubstituteFor(DiagContext context, ElementInfo otherElementInfo) {
            var elementInfo = ElementInfo;
            if (!elementInfo.EqualToOrSubstituteFor(otherElementInfo)) {
                context.AddErrorDiag(new DiagMsg(DiagCode.ElementNotEqualToOrSubstituteFor,
                    elementInfo.DisplayName, otherElementInfo.DisplayName), this);
                return false;
            }
            return true;
        }
        internal override sealed bool TryValidateCore(DiagContext context) {
            var result = context.GetValidationResult(this);
            if (result != null) {
                return result.Value;
            }
            var success = base.TryValidateCore(context);
            context.SetValidationResult(this, success);
            return success;
        }
        private static bool TryCreate<T>(DiagContext context, ElementInfo elementInfo, ElementNode elementNode, out T result) where T : XGlobalElement {
            if (!elementInfo.IsGlobal) throw new ArgumentException("!elementInfo.IsGlobal");
            result = null;
            XElement element;
            var res = TrySkippableCreate(context, elementInfo, elementNode, out element);
            if (res == CreationResult.Error) {
                return false;
            }
            if (res == CreationResult.Skipped) {
                context.AddErrorDiag(new DiagMsg(DiagCode.InvalidElementNode, elementNode.FullName.ToString(), elementInfo.DisplayName),
                    elementNode.QName.TextSpan);
                return false;
            }
            result = (T)element;
            return true;
        }
        protected static bool TryLoadAndValidate<T>(string filePath, System.IO.TextReader reader, DiagContext context, ElementInfo elementInfo, out T result) where T : XGlobalElement {
            result = null;
            ElementNode elementNode;
            if (!Parser.Parse(filePath, reader, context, out elementNode)) {
                return false;
            }
            return TryCreate<T>(context, elementInfo, elementNode, out result);
        }

    }
    public abstract class XGlobalElementRef : XElement {
        private XGlobalElement _globalElement;
        public XGlobalElement GlobalElement {
            get {
                return _globalElement;
            }
            set {
                _globalElement = value;
            }
        }
        public XGlobalElement GenericGlobalElement {
            get {
                return _globalElement;
            }
            set {
                _globalElement = value;
            }
        }
        public T EnsureGlobalElement<T>(bool @try = false) where T : XGlobalElement {
            var obj = _globalElement as T;
            if (obj != null) return obj;
            if ((obj = ElementInfo.ReferencedElement.CreateInstance<T>(@try)) != null) {
                _globalElement = obj;
            }
            return obj;
        }
        public XGlobalElement EnsureGlobalElement(bool @try = false) {
            return EnsureGlobalElement<XGlobalElement>(@try);
        }
        public override sealed FullName FullName {
            get {
                return _globalElement != null ? _globalElement.FullName : default(FullName);
            }
        }
        public override sealed XType Type {
            get {
                return _globalElement != null ? _globalElement.Type : null;
            }
            set {
                EnsureGlobalElement<XGlobalElement>().Type = value;
            }
        }
        internal override sealed void Save(SavingContext context) {
            if (_globalElement != null) {
                _globalElement.Save(context);
            }
        }
        internal override sealed bool TryValidateCore(DiagContext context) {
            if (_globalElement == null) {
                context.AddErrorDiag(new DiagMsg(DiagCode.GlobalElementNotSet), this);
                return false;
            }
            if (_globalElement.CheckEqualToOrSubstituteFor(context, ElementInfo.ReferencedElement)) {
                return _globalElement.TryValidate(context);
            }
            return false;
        }
    }
}
