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
        public abstract T EnsureType<T>(bool @try = false) where T : XType;
        public XType EnsureType(bool @try = false) {
            return EnsureType<XType>(@try);
        }
        //
        public XAttributeSet Attributes {
            get {
                var complexType = Type as XComplexType;
                if (complexType != null) {
                    return complexType.Attributes;
                }
                return null;
            }
            set {
                EnsureType<XComplexType>().Attributes = value;
            }
        }
        public XAttributeSet GenericAttributes {
            get {
                return Attributes;
            }
            set {
                Attributes = value;
            }
        }
        public T EnsureAttributes<T>(bool @try = false) where T : XAttributeSet {
            var complexType = EnsureType<XComplexType>(@try);
            if (complexType != null) {
                return complexType.EnsureAttributes<T>(@try);
            }
            return null;
        }
        public XAttributeSet EnsureAttributes(bool @try = false) {
            return EnsureAttributes<XAttributeSet>(@try);
        }
        public XAttribute TryGetAttribute(string name) {
            return EnsureAttributes().TryGetAttribute(name);
        }
        public void AddOrSetAttribute(XAttribute attribute) {
            EnsureAttributes().AddOrSetAttribute(attribute);
        }
        public bool RemoveAttribute(string name) {
            return EnsureAttributes().RemoveAttribute(name);
        }
        public T CreateAttribute<T>(string name, bool @try = false) where T : XAttribute {
            return EnsureAttributes().CreateAttribute<T>(name, @try);
        }

        //
        public XObject Children {
            get {
                var complexType = Type as XComplexType;
                if (complexType != null) {
                    return complexType.Children;
                }
                return null;
            }
            set {
                EnsureType<XComplexType>().Children = value;
            }
        }
        public XObject GenericChildren {
            get {
                return Children;
            }
            set {
                Children = value;
            }
        }
        public T EnsureChildren<T>(bool @try = false) where T : XObject {
            var complexType = EnsureType<XComplexType>(@try);
            if (complexType != null) {
                return complexType.EnsureChildren<T>(@try);
            }
            return null;
        }
        public XObject EnsureChildren(bool @try = false) {
            return EnsureChildren<XObject>(@try);
        }
        public XChild TryGetChild(int order) {
            return EnsureChildren<XChildSequence>().TryGetChild(order);
        }
        public void AddOrSetChild(XChild child) {
            EnsureChildren<XChildSequence>().AddOrSetChild(child);
        }
        public bool RemoveChild(int order) {
            return EnsureChildren<XChildSequence>().RemoveChild(order);
        }
        public T CreateChild<T>(int order, bool @try = false) where T : XChild {
            return EnsureChildren<XChildSequence>().CreateChild<T>(order, @try);
        }

        //
        #region LINQ
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
            var complexChildren = Children as XChildSequence;
            if (complexChildren != null) {
                return complexChildren.SubElements(filter);
            }
            return Enumerable.Empty<T>();
        }
        public IEnumerable<T> SubElementTypes<T>(Func<XElement, bool> elementFilter = null,
            Func<T, bool> typeFilter = null) where T : XType {
            var complexChildren = Children as XChildSequence;
            if (complexChildren != null) {
                return complexChildren.SubElementTypes(elementFilter, typeFilter);
            }
            return Enumerable.Empty<T>();
        }
        public IEnumerable<T> SubElementAttributes<T>(Func<XElement, bool> elementFilter = null,
            Func<T, bool> attributeFilter = null) where T : XAttribute {
            var complexChildren = Children as XChildSequence;
            if (complexChildren != null) {
                return complexChildren.SubElementAttributes(elementFilter, attributeFilter);
            }
            return Enumerable.Empty<T>();
        }
        public IEnumerable<T> SubElementAttributeTypes<T>(Func<XElement, bool> elementFilter = null,
            Func<XAttribute, bool> attributeFilter = null, Func<T, bool> typeFilter = null) where T : XSimpleType {
            var complexChildren = Children as XChildSequence;
            if (complexChildren != null) {
                return complexChildren.SubElementAttributeTypes(elementFilter, attributeFilter, typeFilter);
            }
            return Enumerable.Empty<T>();
        }
        public IEnumerable<T> SubElementChildren<T>(Func<XElement, bool> elementFilter = null,
            Func<T, bool> childrenFilter = null) where T : XObject {
            var complexChildren = Children as XChildSequence;
            if (complexChildren != null) {
                return complexChildren.SubElementChildren(elementFilter, childrenFilter);
            }
            return Enumerable.Empty<T>();
        }
        public IEnumerable<T> DescendantElements<T>(Func<T, bool> filter = null) where T : XElement {
            var complexChildren = Children as XChildSequence;
            if (complexChildren != null) {
                return complexChildren.DescendantElements(filter);
            }
            return Enumerable.Empty<T>();
        }
        public IEnumerable<T> DescendantElementTypes<T>(Func<XElement, bool> elementFilter = null,
            Func<T, bool> typeFilter = null) where T : XType {
            var complexChildren = Children as XChildSequence;
            if (complexChildren != null) {
                return complexChildren.DescendantElementTypes(elementFilter, typeFilter);
            }
            return Enumerable.Empty<T>();
        }
        public IEnumerable<T> DescendantElementAttributes<T>(Func<XElement, bool> elementFilter = null,
            Func<T, bool> attributeFilter = null) where T : XAttribute {
            var complexChildren = Children as XChildSequence;
            if (complexChildren != null) {
                return complexChildren.DescendantElementAttributes(elementFilter, attributeFilter);
            }
            return Enumerable.Empty<T>();
        }
        public IEnumerable<T> DescendantElementAttributeTypes<T>(Func<XElement, bool> elementFilter = null,
            Func<XAttribute, bool> attributeFilter = null, Func<T, bool> typeFilter = null) where T : XSimpleType {
            var complexChildren = Children as XChildSequence;
            if (complexChildren != null) {
                return complexChildren.DescendantElementAttributeTypes(elementFilter, attributeFilter, typeFilter);
            }
            return Enumerable.Empty<T>();
        }
        public IEnumerable<T> DescendantElementChildren<T>(Func<XElement, bool> elementFilter = null,
            Func<T, bool> childrenFilter = null) where T : XObject {
            var complexChildren = Children as XChildSequence;
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
                        context.AddErrorDiag(new DiagMsg(DiagCode.ComplexTypeValueRequiredForElement, effElementInfo.DisplayName),
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
                    context.AddErrorDiag(new DiagMsg(DiagCode.ElementIsNotNullable, effElementInfo.DisplayName), elementNameTextSpan);
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
        public override sealed T EnsureType<T>(bool @try = false) {
            var obj = _type as T;
            if (obj != null) return obj;
            if ((obj = ElementInfo.Type.CreateInstance<T>(@try)) != null) {
                Type = obj;
            }
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
                context.AddErrorDiag(new DiagMsg(DiagCode.ElementIsNotNullable, elementInfo.DisplayName), this);
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
        internal static bool TryCreate<T>(DiagContext context, ElementInfo elementInfo, ElementNode elementNode, out T result) where T : XEntityElement {
            if (!elementInfo.IsGlobal) throw new ArgumentException("!elementInfo.IsGlobal");
            result = null;
            XChild child;
            var res = TrySkippableCreate(context, elementInfo, elementNode, out child);
            if (res == CreationResult.Error) {
                return false;
            }
            if (res == CreationResult.Skipped) {
                context.AddErrorDiag(new DiagMsg(DiagCode.InvalidElementNode, elementNode.FullName.ToString(), elementInfo.DisplayName),
                    elementNode.QName.TextSpan);
                return false;
            }
            result = (T)child;
            return true;
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
        public override sealed T EnsureType<T>(bool @try = false) {
            var globalElement = EnsureGlobalElement<XGlobalElement>(@try);
            if (globalElement != null) {
                return globalElement.EnsureType<T>(@try);
            }
            return null;
        }
        internal override sealed void Save(SavingContext context) {
            if (_globalElement != null) {
                _globalElement.Save(context);
            }
        }
        internal override sealed bool TryValidateCore(DiagContext context) {
            if (_globalElement == null) {
                context.AddErrorDiag(new DiagMsg(DiagCode.EntityElementIsNull), this);
                return false;
            }
            if (_globalElement.CheckEqualToOrSubstituteFor(context, ElementInfo.ReferencedElement)) {
                return _globalElement.TryValidate(context);
            }
            return false;
        }
    }
}
