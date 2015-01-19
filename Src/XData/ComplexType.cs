using System;
using XData.IO.Text;

namespace XData {
    public abstract class XComplexType : XType {
        protected XComplexType() { }
        //
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
        //public bool HasAttributes {
        //    get {
        //        return _attributes != null && _attributes.Count > 0;
        //    }
        //}
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
            obj = complexTypeInfo.Children.CreateInstance<T>();
            Children = obj;
            return obj;
        }
        //
        public ComplexTypeInfo ComplexTypeInfo {
            get {
                return (ComplexTypeInfo)ObjectInfo;
            }
        }
        //
        internal static bool TryCreate(Context context, ProgramInfo programInfo, ComplexTypeInfo complexTypeInfo, bool isNullable,
            ComplexValueNode complexValueNode, out XComplexType result) {
            result = null;
            var equalsTokenTextSpan = complexValueNode.EqualsTokenTextSpan;
            var effComplexTypeInfo = (ComplexTypeInfo)GetTypeInfo(context, programInfo, complexValueNode.TypeQName, complexTypeInfo, equalsTokenTextSpan);
            if (effComplexTypeInfo == null) {
                return false;
            }
            //
            XAttributeSet attributeSet = null;
            var attributeListNode = complexValueNode.AttributeList;
            var attributeSetInfo = effComplexTypeInfo.Attributes;
            if (attributeSetInfo != null) {
                if (!XAttributeSet.TryCreate(context, programInfo, attributeSetInfo,
                    equalsTokenTextSpan, attributeListNode, out attributeSet)) {
                    return false;
                }
            }
            else {
                if (attributeListNode != null && attributeListNode.Count > 0) {
                    context.AddErrorDiagnostic(DiagnosticCode.TypeProhibitsAttributes,
                        "Type '{0}' prohibits attributes.".InvFormat(effComplexTypeInfo.FullName.ToString()),
                        attributeListNode.OpenTokenTextSpan);
                    return false;
                }
            }
            //
            XObject children = null;
            var simpleValueNode = complexValueNode.SimpleValue;
            var simpleTypeInfo = effComplexTypeInfo.Children as SimpleTypeInfo;
            if (simpleTypeInfo != null) {
                if (!simpleValueNode.IsValid) {
                    context.AddErrorDiagnostic(DiagnosticCode.TypeRequiresSimpleTypeChild,
                        "Type '{0}' requires simple type child.".InvFormat(effComplexTypeInfo.FullName.ToString()),
                        complexValueNode.OpenElementTextSpan);
                    return false;
                }
                XSimpleType simpleType;
                if (!XSimpleType.TryCreate(context, programInfo, simpleTypeInfo,
                    simpleValueNode, out simpleType)) {
                    return false;
                }
                children = simpleType;
            }
            else {
                var elementListNode = complexValueNode.ElementList;
                var childSetInfo = effComplexTypeInfo.Children as ChildSetInfo;
                if (childSetInfo != null) {
                    if (simpleValueNode.IsValid) {
                        context.AddErrorDiagnostic(DiagnosticCode.TypeRequiresElementChildren,
                            "Type '{0}' requires elemment children.".InvFormat(effComplexTypeInfo.FullName.ToString()),
                            simpleValueNode.TextSpan);
                        return false;
                    }
                    XChildSet childSet;
                    if (!XChildSet.TryCreate(context, programInfo, childSetInfo,
                        complexValueNode.CloseElementTextSpan, elementListNode, out childSet)) {
                        return false;
                    }
                    children = childSet;
                }
                else {
                    if (simpleValueNode.IsValid || (elementListNode != null && elementListNode.Count > 0)) {
                        context.AddErrorDiagnostic(DiagnosticCode.TypeProhibitsChildren,
                            "Type '{0}' prohibits children.".InvFormat(effComplexTypeInfo.FullName.ToString()),
                            complexValueNode.OpenElementTextSpan);
                    }
                }
            }
            result = effComplexTypeInfo.CreateInstance<XComplexType>();
            result.Attributes = attributeSet;
            result.Children = children;
            return true;
        }
    }
    //public abstract class XSimpleChildComplexType : XComplexType {
    //    protected XSimpleChildComplexType() { }
    //    public XSimpleType Child {
    //        get {
    //            return Children as XSimpleType;
    //        }
    //        set {
    //            Children = value;
    //        }
    //    }
    //    public XSimpleType GenericChild {
    //        get {
    //            return Child;
    //        }
    //        set {
    //            Children = value;
    //        }
    //    }

    //}
    //public abstract class XComplexChildrenComplexType : XComplexType {
    //    protected XComplexChildrenComplexType() { }
    //    new public XChildSet Children {
    //        get {
    //            return base.Children as XChildSet;
    //        }
    //        set {
    //            base.Children = value;
    //        }
    //    }
    //    public XChildSet GenericChildren {
    //        get {
    //            return Children;
    //        }
    //        set {
    //            base.Children = value;
    //        }
    //    }
    //}

}
