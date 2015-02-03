using System;
using System.Collections.Generic;
using System.Linq;
using XData.IO.Text;

namespace XData {
    public abstract class XComplexType : XType {
        private XAttributeSet _attributes;
        public XAttributeSet Attributes {
            get {
                return _attributes;
            }
            set {
                _attributes = SetParentTo(value);
            }
        }
        public XAttributeSet GenericAttributes {
            get {
                return _attributes;
            }
            set {
                Attributes = value;
            }
        }
        public T EnsureAttributes<T>(bool @try = false) where T : XAttributeSet {
            var obj = _attributes as T;
            if (obj != null) {
                return obj;
            }
            var complexTypeInfo = ComplexTypeInfo;
            if (complexTypeInfo.Attributes == null) {
                if (@try) {
                    return null;
                }
                throw new InvalidOperationException("Attribute set not allowed.");
            }
            obj = complexTypeInfo.Attributes.CreateInstance<T>();
            Attributes = obj;
            return obj;
        }
        public XAttributeSet EnsureAttributes(bool @try = false) {
            return EnsureAttributes<XAttributeSet>(@try);
        }
        //
        private XObject _children;
        public XObject Children {
            get {
                return _children;
            }
            set {
                _children = SetParentTo(value);
            }
        }
        public XObject GenericChildren {
            get {
                return _children;
            }
            set {
                Children = value;
            }
        }
        public T EnsureChildren<T>(bool @try = false) where T : XObject {
            var obj = _children as T;
            if (obj != null) {
                return obj;
            }
            var complexTypeInfo = ComplexTypeInfo;
            if (complexTypeInfo.Children == null) {
                if (@try) {
                    return null;
                }
                throw new InvalidOperationException("Children not allowed.");
            }
            if ((obj = complexTypeInfo.Children.CreateInstance<T>(@try)) != null) {
                Children = obj;
            }
            return obj;
        }
        public XObject EnsureChildren(bool @try = false) {
            return EnsureChildren<XObject>(@try);
        }
        //
        #region LINQ
        public IEnumerable<T> SelfAttributes<T>(Func<T, bool> filter = null) where T : XAttribute {
            if (_attributes != null) {
                return _attributes.SelfAttributes(filter);
            }
            return Enumerable.Empty<T>();
        }
        public IEnumerable<T> SelfAttributeTypes<T>(Func<XAttribute, bool> attributeFilter = null, 
            Func<T, bool> typeFilter = null) where T : XSimpleType {
            if (_attributes != null) {
                return _attributes.SelfAttributeTypes(attributeFilter, typeFilter);
            }
            return Enumerable.Empty<T>();
        }
        public IEnumerable<T> SubElements<T>(Func<T, bool> filter = null) where T : XElement {
            var complexChildren = _children as XChildSequence;
            if (complexChildren != null) {
                return complexChildren.SubElements(filter);
            }
            return Enumerable.Empty<T>();
        }
        public IEnumerable<T> SubElementTypes<T>(Func<XElement, bool> elementFilter = null,
            Func<T, bool> typeFilter = null) where T : XType {
            var complexChildren = _children as XChildSequence;
            if (complexChildren != null) {
                return complexChildren.SubElementTypes(elementFilter, typeFilter);
            }
            return Enumerable.Empty<T>();
        }
        public IEnumerable<T> SubElementAttributes<T>(Func<XElement, bool> elementFilter = null, 
            Func<T, bool> attributeFilter = null) where T : XAttribute {
            var complexChildren = _children as XChildSequence;
            if (complexChildren != null) {
                return complexChildren.SubElementAttributes(elementFilter, attributeFilter);
            }
            return Enumerable.Empty<T>();
        }
        public IEnumerable<T> SubElementAttributeTypes<T>(Func<XElement, bool> elementFilter = null,
            Func<XAttribute, bool> attributeFilter = null, Func<T, bool> typeFilter = null) where T : XSimpleType {
            var complexChildren = _children as XChildSequence;
            if (complexChildren != null) {
                return complexChildren.SubElementAttributeTypes(elementFilter, attributeFilter, typeFilter);
            }
            return Enumerable.Empty<T>();
        }
        public IEnumerable<T> SubElementChildren<T>(Func<XElement, bool> elementFilter = null,
            Func<T, bool> childrenFilter = null) where T : XObject {
            var complexChildren = _children as XChildSequence;
            if (complexChildren != null) {
                return complexChildren.SubElementChildren(elementFilter, childrenFilter);
            }
            return Enumerable.Empty<T>();
        }
        public IEnumerable<T> DescendantElements<T>(Func<T, bool> filter = null) where T : XElement {
            var complexChildren = _children as XChildSequence;
            if (complexChildren != null) {
                return complexChildren.DescendantElements(filter);
            }
            return Enumerable.Empty<T>();
        }
        public IEnumerable<T> DescendantElementTypes<T>(Func<XElement, bool> elementFilter = null,
            Func<T, bool> typeFilter = null) where T : XType {
            var complexChildren = _children as XChildSequence;
            if (complexChildren != null) {
                return complexChildren.DescendantElementTypes(elementFilter, typeFilter);
            }
            return Enumerable.Empty<T>();
        }
        public IEnumerable<T> DescendantElementAttributes<T>(Func<XElement, bool> elementFilter = null,
            Func<T, bool> attributeFilter = null) where T : XAttribute {
            var complexChildren = _children as XChildSequence;
            if (complexChildren != null) {
                return complexChildren.DescendantElementAttributes(elementFilter, attributeFilter);
            }
            return Enumerable.Empty<T>();
        }
        public IEnumerable<T> DescendantElementAttributeTypes<T>(Func<XElement, bool> elementFilter = null,
            Func<XAttribute, bool> attributeFilter = null, Func<T, bool> typeFilter = null) where T : XSimpleType {
            var complexChildren = _children as XChildSequence;
            if (complexChildren != null) {
                return complexChildren.DescendantElementAttributeTypes(elementFilter, attributeFilter, typeFilter);
            }
            return Enumerable.Empty<T>();
        }
        public IEnumerable<T> DescendantElementChildren<T>(Func<XElement, bool> elementFilter = null,
            Func<T, bool> childrenFilter = null) where T : XObject {
            var complexChildren = _children as XChildSequence;
            if (complexChildren != null) {
                return complexChildren.DescendantElementChildren(elementFilter, childrenFilter);
            }
            return Enumerable.Empty<T>();
        }
        #endregion LINQ
        //
        public ComplexTypeInfo ComplexTypeInfo {
            get {
                return (ComplexTypeInfo)ObjectInfo;
            }
        }
        internal override sealed void SaveValue(SavingContext context) {
            if (_attributes != null) {
                context.AppendLine();
                context.PushIndent();
                _attributes.Save(context);
                context.PopIndent();
            }
            var simpleChildInfo = ComplexTypeInfo.SimpleChild;
            if (simpleChildInfo != null) {
                var simpleChild = _children as XSimpleType;
                if (simpleChild != null) {
                    if (_attributes != null) {
                        context.AppendLine();
                        context.PushIndent();
                    }
                    context.Append("$ ");
                    simpleChild.Save(context, simpleChildInfo);
                    if (_attributes != null) {
                        context.PopIndent();
                    }
                }
            }
            else {
                var complexChildren = _children as XChildSequence;
                if (complexChildren != null) {
                    context.AppendLine();
                    context.PushIndent();
                    complexChildren.SaveAsRoot(context);
                    context.PopIndent();
                }
            }
        }
        internal override sealed bool TryValidateCore(DiagContext context) {
            var dMarker = context.MarkDiags();
            var complexTypeInfo = ComplexTypeInfo;
            var attributeSetInfo = complexTypeInfo.Attributes;
            if (attributeSetInfo != null) {
                if (_attributes != null) {
                    if (_attributes.CheckEqualTo(context, attributeSetInfo)) {
                        _attributes.TryValidate(context);
                    }
                }
                else if (attributeSetInfo.Attributes != null) {
                    foreach (var attributeInfo in attributeSetInfo.Attributes) {
                        if (!attributeInfo.IsOptional) {
                            context.AddErrorDiag(new DiagMsg(DiagCode.RequiredAttributeNotFound, attributeInfo.DisplayName), this);
                        }
                    }
                }
            }
            else if (_attributes != null) {
                context.AddErrorDiag(new DiagMsg(DiagCode.AttributesNotAllowedForType, complexTypeInfo.DisplayName), this);
            }
            //
            var simpleTypeInfo = complexTypeInfo.SimpleChild;
            if (simpleTypeInfo != null) {
                var simpleChild = _children as XSimpleType;
                if ((object)simpleChild != null) {
                    if (simpleChild.CheckEqualTo(context, simpleTypeInfo)) {
                        simpleChild.TryValidate(context);
                    }
                }
                else {
                    context.AddErrorDiag(new DiagMsg(DiagCode.SimpleChildRequiredForType, complexTypeInfo.DisplayName), this);
                }
            }
            else {
                var complexChildrenInfo = complexTypeInfo.ComplexChildren;
                if (complexChildrenInfo != null) {
                    var complexChildren = _children as XChildSequence;
                    if (complexChildren != null) {
                        if (complexChildren.CheckEqualTo(context, complexChildrenInfo)) {
                            complexChildren.TryValidate(context);
                        }
                    }
                    else if (complexChildrenInfo.Children != null) {
                        foreach (var childInfo in complexChildrenInfo.Children) {
                            if (!childInfo.IsOptional) {
                                context.AddErrorDiag(new DiagMsg(DiagCode.RequiredChildNotFound, childInfo.DisplayName), this);
                            }
                        }
                    }
                }
                else {
                    if (_children != null) {
                        context.AddErrorDiag(new DiagMsg(DiagCode.ChildrenNotAllowedForType, complexTypeInfo.DisplayName),
                            this);
                    }
                }
            }
            return !dMarker.HasErrors;
        }
        internal static bool TryCreate(DiagContext context, ProgramInfo programInfo, ComplexTypeInfo complexTypeInfo,
            ComplexValueNode complexValueNode, out XComplexType result) {
            result = null;
            var equalsTextSpan = complexValueNode.EqualsTextSpan;
            var effComplexTypeInfo = (ComplexTypeInfo)GetEffectiveTypeInfo(context, programInfo, complexValueNode.TypeQName,
                complexTypeInfo, equalsTextSpan);
            if (effComplexTypeInfo == null) {
                return false;
            }
            //
            XAttributeSet attributes = null;
            var attributeListNode = complexValueNode.AttributeList;
            var attributeSetInfo = effComplexTypeInfo.Attributes;
            if (attributeSetInfo != null) {
                if (!XAttributeSet.TryCreate(context, programInfo, attributeSetInfo,
                    attributeListNode, complexValueNode.CloseAttributesTextSpan, out attributes)) {
                    return false;
                }
            }
            else {
                if (attributeListNode != null) {
                    context.AddErrorDiag(new DiagMsg(DiagCode.AttributesNotAllowedForType, effComplexTypeInfo.DisplayName),
                        attributeListNode.OpenTokenTextSpan);
                    return false;
                }
            }
            //
            XObject children = null;
            var simpleValueNode = complexValueNode.SimpleChild;
            var simpleTypeInfo = effComplexTypeInfo.SimpleChild;
            if (simpleTypeInfo != null) {
                if (!simpleValueNode.IsValid) {
                    context.AddErrorDiag(new DiagMsg(DiagCode.SimpleChildRequiredForType, effComplexTypeInfo.DisplayName),
                        complexValueNode.OpenChildrenTextSpan);
                    return false;
                }
                XSimpleType simpleType;
                if (!XSimpleType.TryCreate(context, programInfo, simpleTypeInfo, simpleValueNode, out simpleType)) {
                    return false;
                }
                children = simpleType;
            }
            else {
                var elementListNode = complexValueNode.ElementList;
                var complexChildrenInfo = effComplexTypeInfo.ComplexChildren;
                if (complexChildrenInfo != null) {
                    if (simpleValueNode.IsValid) {
                        context.AddErrorDiag(new DiagMsg(DiagCode.ComplexChildrenRequiredForType, effComplexTypeInfo.DisplayName),
                            simpleValueNode.TextSpan);
                        return false;
                    }
                    XChildSequence childSeq;
                    if (!XChildSequence.TryCreate(context, complexChildrenInfo,
                         elementListNode, complexValueNode.CloseChildrenTextSpan, out childSeq)) {
                        return false;
                    }
                    children = childSeq;
                }
                else {
                    if (simpleValueNode.IsValid || elementListNode != null) {
                        context.AddErrorDiag(new DiagMsg(DiagCode.ChildrenNotAllowedForType, effComplexTypeInfo.DisplayName),
                            complexValueNode.OpenChildrenTextSpan);
                        return false;
                    }
                }
            }
            result = effComplexTypeInfo.CreateInstance<XComplexType>();
            result.TextSpan = equalsTextSpan;
            result.Attributes = attributes;
            result.Children = children;
            return true;
        }
    }

}
