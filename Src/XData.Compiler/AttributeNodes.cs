using System.Collections.Generic;
using XData.IO.Text;

namespace XData.Compiler {
    internal sealed class AttributesNode : Node {
        public AttributesNode(Node parent) : base(parent) { }
        public List<AttributeNode> AttributeList;
        public TextSpan OpenBracketTextSpan, CloseBracketTextSpan;
        public void Resolve() {
            if (AttributeList != null) {
                foreach (var attribute in AttributeList) {
                    attribute.Resolve();
                }
            }
        }
        public AttributeSetSymbol CreateSymbol(ComplexTypeSymbol parent, AttributeSetSymbol baseAttributeSetSymbol, bool isExtension) {
            var displayName = parent.DisplayName + ".[]";
            var attributeSetSymbol = new AttributeSetSymbol(parent, baseAttributeSetSymbol, displayName);
            if (isExtension) {
                if (AttributeList != null) {
                    foreach (var attribute in AttributeList) {
                        var attributeName = attribute.Name;
                        if (attributeSetSymbol.AttributeNameList.Contains(attributeName)) {
                            DiagContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.DuplicateAttributeName, attributeName),
                                attribute.NameNode.TextSpan);
                        }
                        attributeSetSymbol.AttributeNameList.Add(attributeName);
                        attributeSetSymbol.AttributeList.Add(attribute.CreateSymbol(attributeSetSymbol, null, displayName));
                    }
                }
            }
            else {//restriction
                if (AttributeList != null) {
                    var attributeSymbolList = attributeSetSymbol.AttributeList;
                    foreach (var attribute in AttributeList) {
                        AttributeSymbol restrictedAttributeSymbol = null;
                        int idx;
                        var attributeName = attribute.Name;
                        for (idx = 0; idx < attributeSymbolList.Count; ++idx) {
                            if (attributeSymbolList[idx].Name == attributeName) {
                                restrictedAttributeSymbol = attributeSymbolList[idx];
                                break;
                            }
                        }
                        if (restrictedAttributeSymbol == null) {
                            DiagContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.CannotFindRestrictedAttribute, attributeName),
                                attribute.NameNode.TextSpan);
                        }
                        var isDelete = attribute.IsDelete;
                        if (isDelete) {
                            if (!restrictedAttributeSymbol.IsOptional) {
                                DiagContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.CannotDeleteRequiredAttribute, attributeName),
                                    attribute.OptionalOrDelete.TextSpan);
                            }
                        }
                        attributeSymbolList.RemoveAt(idx);
                        if (!isDelete) {
                            attributeSymbolList.Insert(idx, attribute.CreateSymbol(attributeSetSymbol, restrictedAttributeSymbol, displayName));
                        }
                    }
                }
            }
            return attributeSetSymbol;
        }

    }

    internal sealed class AttributeNode : ObjectNode {
        public AttributeNode(Node parent) : base(parent) { }
        public NameNode NameNode;
        public TextSpan Nullable;
        public OptionalOrDeleteNode OptionalOrDelete;
        public QualifiableNameNode TypeQName;
        public TypeNode Type;
        public string Name {
            get {
                return NameNode.Value;
            }
        }
        public bool IsNullable {
            get {
                return Nullable.IsValid;
            }
        }
        public bool IsOptional {
            get {
                return OptionalOrDelete.IsOptional;
            }
        }
        public bool IsDelete {
            get {
                return OptionalOrDelete.IsDelete;
            }
        }
        public void Resolve() {
            Type = NamespaceAncestor.ResolveAsType(TypeQName);
        }
        public AttributeSymbol CreateSymbol(AttributeSetSymbol parent, AttributeSymbol restrictedAttributeSymbol, string parentDisplayName) {
            if (restrictedAttributeSymbol == null) {
                if (IsDelete) {
                    DiagContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.DeletionNotAllowedInExtension), OptionalOrDelete.TextSpan);
                }
            }
            else {
                if (IsOptional && !restrictedAttributeSymbol.IsOptional) {
                    DiagContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.CannotChangeRequiredToOptional),
                        OptionalOrDelete.TextSpan);
                }
                if (IsNullable && !restrictedAttributeSymbol.IsNullable) {
                    DiagContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.CannotChangeNonNullableToNullable),
                        Nullable);
                }
            }
            var typeSymbol = Type.CreateSymbol() as SimpleTypeSymbol;
            if (typeSymbol == null) {
                DiagContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.SimpleTypeRequired), TypeQName.TextSpan);
            }
            if (restrictedAttributeSymbol != null) {
                if (!typeSymbol.EqualToOrDeriveFrom(restrictedAttributeSymbol.Type)) {
                    DiagContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.TypeNotEqualToOrDeriveFromRestricted,
                        typeSymbol.DisplayName, restrictedAttributeSymbol.Type.DisplayName),
                        TypeQName.TextSpan);
                }
            }
            var name = Name;
            return new AttributeSymbol(parent, "CLS_" + name, name, parentDisplayName + "." + name, IsOptional, IsNullable,
                typeSymbol, restrictedAttributeSymbol);
        }
    }
    internal struct OptionalOrDeleteNode {
        public OptionalOrDeleteNode(TextSpan optional, NameNode delete) {
            Optional = optional;
            Delete = delete;
        }
        public readonly TextSpan Optional;
        public readonly NameNode Delete;
        public bool IsOptional {
            get {
                return Optional.IsValid;
            }
        }
        public bool IsDelete {
            get {
                return Delete.IsValid;
            }
        }
        public TextSpan TextSpan {
            get {
                if (Delete.IsValid) {
                    return Delete.TextSpan;
                }
                return Optional;
            }
        }
    }


}
