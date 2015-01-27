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
        internal static bool TryCreate(DiagContext context, ProgramInfo programInfo, ComplexTypeInfo complexTypeInfo, bool isNullable,
            ComplexValueNode complexValueNode, out XComplexType result) {
            result = null;
            var equalsTokenTextSpan = complexValueNode.EqualsToken;
            var effComplexTypeInfo = (ComplexTypeInfo)GetEffectiveTypeInfo(context, programInfo, complexValueNode.TypeQName, complexTypeInfo, equalsTokenTextSpan);
            if (effComplexTypeInfo == null) {
                return false;
            }
            //
            XAttributeSet attributes = null;
            var attributeListNode = complexValueNode.Attributes;
            var attributeSetInfo = effComplexTypeInfo.Attributes;
            if (attributeSetInfo != null) {
                if (!XAttributeSet.TryCreate(context, programInfo, attributeSetInfo,
                    equalsTokenTextSpan, attributeListNode, out attributes)) {
                    return false;
                }
            }
            else {
                if (attributeListNode != null && attributeListNode.Count > 0) {
                    context.AddErrorDiag(new DiagMsg(DiagCode.TypeProhibitsAttributes, effComplexTypeInfo.FullName.ToString()),
                        attributeListNode.OpenToken);
                    return false;
                }
            }
            //
            XObject children = null;
            var simpleValueNode = complexValueNode.SimpleChild;
            var simpleTypeInfo = effComplexTypeInfo.SimpleChild;
            if (simpleTypeInfo != null) {
                if (!simpleValueNode.IsValid) {
                    context.AddErrorDiag(new DiagMsg(DiagCode.TypeRequiresSimpleChild, effComplexTypeInfo.FullName.ToString()),
                        complexValueNode.ChildrenOpenToken);
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
                var elementListNode = complexValueNode.ComplexChildren;
                var childSetInfo = effComplexTypeInfo.ComplexChildren;
                if (childSetInfo != null) {
                    if (simpleValueNode.IsValid) {
                        context.AddErrorDiag(new DiagMsg(DiagCode.TypeRequiresComplexChildren, effComplexTypeInfo.FullName.ToString()),
                            simpleValueNode.TextSpan);
                        return false;
                    }
                    XChildSet childSet;
                    if (!XChildSet.TryCreate(context, programInfo, childSetInfo,
                        complexValueNode.ChildrenCloseToken, elementListNode, out childSet)) {
                        return false;
                    }
                    children = childSet;
                }
                else {
                    if (simpleValueNode.IsValid || (elementListNode != null && elementListNode.Count > 0)) {
                        context.AddErrorDiag(new DiagMsg(DiagCode.TypeProhibitsChildren, effComplexTypeInfo.FullName.ToString()),
                            complexValueNode.ChildrenOpenToken);
                    }
                }
            }
            result = effComplexTypeInfo.CreateInstance<XComplexType>();
            result.Attributes = attributes;
            result.Children = children;
            return true;
        }
    }

}
