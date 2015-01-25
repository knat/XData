using System;
using System.Collections.Generic;
using XData.IO.Text;

namespace XData.Compiler {
    //public abstract class EntityNode : NamespaceMemberNode {
    //    protected EntityNode(Node parent) : base(parent) { }
    //    public TextSpan Nullable;
    //    public QualifiableNameNode SubstitutedEntityQName;
    //    public QualifiableNameNode TypeQName;
    //    public TypeNode Type;
    //    public bool IsNullable {
    //        get {
    //            return Nullable.IsValid;
    //        }
    //    }
    //    public override void Resolve() {
    //        Type = NamespaceAncestor.ResolveAsType(TypeQName);
    //    }
    //}

    //public sealed class GlobalAttributeNode : EntityNode {
    //    public GlobalAttributeNode(Node parent) : base(parent) { }
    //    public GlobalAttributeNode SubstitutedAttribute;
    //    public override void Resolve() {
    //        base.Resolve();
    //        if (SubstitutedEntityQName.IsValid) {
    //            SubstitutedAttribute = NamespaceAncestor.ResolveAsAttribute(SubstitutedEntityQName);
    //        }
    //    }
    //    protected override NamedObjectSymbol CreateSymbolCore(NamespaceSymbol parent, string csName, FullName fullName) {
    //        AttributeSymbol substitutedAttributeSymbol = null;
    //        if (SubstitutedAttribute != null) {
    //            substitutedAttributeSymbol = (AttributeSymbol)SubstitutedAttribute.CreateSymbol();
    //        }
    //        var simpleTypeSymbol = Type.CreateSymbol() as SimpleTypeSymbol;
    //        if (simpleTypeSymbol == null) {
    //            ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.SimpleTypeRequired), TypeQName.TextSpan);
    //        }
    //        if (substitutedAttributeSymbol != null) {
    //            if (IsNullable && !substitutedAttributeSymbol.IsNullable) {
    //                ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.AttributeIsNullableButSubstitutedIsNotNullable),
    //                    Name.TextSpan);
    //            }
    //            if (!simpleTypeSymbol.IsEqualToOrDeriveFrom(substitutedAttributeSymbol.Type)) {
    //                ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.AttributeTypeNotEqualToOrDeriveFromSubstituted,
    //                    simpleTypeSymbol.FullName.ToString(), substitutedAttributeSymbol.Type.FullName.ToString()),
    //                    TypeQName.TextSpan);
    //            }
    //        }
    //        return new AttributeSymbol(parent, csName, IsAbstract, IsSealed, fullName, ElementKind.Global, null, substitutedAttributeSymbol, null,
    //            fullName.ToString(), null, IsNullable, false, simpleTypeSymbol);
    //    }
    //}
    public sealed class AttributesNode : Node {
        public AttributesNode(Node parent) : base(parent) { }
        public List<AttributeNode> AttributeList;
        public TextSpan OpenBracketToken, CloseBracketToken;
        public void Resolve() {
            if (AttributeList != null) {
                foreach (var attribute in AttributeList) {
                    attribute.Resolve();
                }
            }
        }
        public AttributeSetSymbol CreateSymbol(ComplexTypeSymbol parent, AttributeSetSymbol baseAttributeSetSymbol, bool isExtension) {
            var baseAttributeSymbolList = baseAttributeSetSymbol != null ? baseAttributeSetSymbol.AttributeList : null;
            var attributeSetSymbol = new AttributeSetSymbol(parent, baseAttributeSetSymbol);
            var attributeSymbolList = attributeSetSymbol.AttributeList;
            var displayNameBase = parent.FullName.ToString() + ".[].";
            if (baseAttributeSymbolList != null) {
                attributeSymbolList.AddRange(baseAttributeSymbolList);
            }
            if (isExtension) {
                if (AttributeList != null) {
                    foreach (var attribute in AttributeList) {
                        if (baseAttributeSymbolList != null) {
                            foreach (var baseAttributeSymbol in baseAttributeSymbolList) {
                                if (baseAttributeSymbol.Name == attribute.Name) {
                                    ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.DuplicateAttributeName, baseAttributeSymbol.Name),
                                        attribute.NameNode.TextSpan);
                                }
                            }
                        }
                        if (attribute.IsDelete) {
                            ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.DeletionNotAllowed), attribute.OptionalOrDelete.TextSpan);
                        }
                        attributeSymbolList.Add(attribute.CreateSymbol(attributeSetSymbol, null, displayNameBase));
                    }
                }
            }
            else {//restriction
                if (AttributeList != null) {
                    foreach (var attribute in AttributeList) {
                        AttributeSymbol restrictedAttributeSymbol = null;
                        int idx;
                        var attName = attribute.Name;
                        for (idx = 0; idx < attributeSymbolList.Count; ++idx) {
                            if (attributeSymbolList[idx].Name == attName) {
                                restrictedAttributeSymbol = attributeSymbolList[idx];
                                break;
                            }
                        }
                        if (restrictedAttributeSymbol == null) {
                            ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.CannotFindRestrictedAttribute, attName),
                                attribute.NameNode.TextSpan);
                        }
                        var isDelete = attribute.IsDelete;
                        if (isDelete) {
                            if (!restrictedAttributeSymbol.IsOptional) {
                                ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.CannotDeleteAttributeBecauseItIsNotOptional, attName),
                                    attribute.NameNode.TextSpan);
                            }
                        }
                        attributeSymbolList.RemoveAt(idx);
                        if (!isDelete) {
                            if (attribute.IsOptional && !restrictedAttributeSymbol.IsOptional) {
                                ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.AttributeIsOptionalButRestrictedIsRequired),
                                    attribute.OptionalOrDelete.TextSpan);
                            }
                            if (attribute.IsNullable && !restrictedAttributeSymbol.IsNullable) {
                                ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.AttributeIsNullableButRestrictedIsNotNullable),
                                    attribute.Nullable);
                            }
                            var attributeSymbol = attribute.CreateSymbol(attributeSetSymbol, restrictedAttributeSymbol, displayNameBase);
                            if (!attributeSymbol.Type.IsEqualToOrDeriveFrom(restrictedAttributeSymbol.Type)) {
                                ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.TypeNotEqualToOrDeriveFromRestricted,
                                    attributeSymbol.Type.FullName.ToString(), restrictedAttributeSymbol.Type.FullName.ToString()),
                                    attribute.TypeQName.TextSpan);
                            }
                            attributeSymbolList.Insert(idx, attributeSymbol);
                        }
                    }
                }
            }

            return attributeSetSymbol;
        }

        //public AttributeSetSymbol CreateSymbol(ComplexTypeSymbol parent, AttributeSetSymbol baseAttributeSetSymbol, bool isExtension) {
        //    var baseAttributeSymbolList = baseAttributeSetSymbol != null ? baseAttributeSetSymbol.AttributeList : null;
        //    var attributeSetSymbol = new AttributeSetSymbol(parent, baseAttributeSetSymbol);
        //    var attributeSymbolList = attributeSetSymbol.AttributeList;
        //    var displayNameBase = parent.FullName.ToString() + ".[]";
        //    if (isExtension) {
        //        if (baseAttributeSymbolList != null) {
        //            attributeSymbolList.AddRange(baseAttributeSymbolList);
        //        }
        //        if (AttributeList != null) {
        //            foreach (var attribute in AttributeList) {
        //                if (baseAttributeSymbolList != null) {
        //                    foreach (var baseAttributeSymbol in baseAttributeSymbolList) {
        //                        if (baseAttributeSymbol.FullName == attribute.FullName) {
        //                            ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.DuplicateAttributeName, attribute.FullName.ToString()),
        //                                attribute.MemberName.TextSpan);
        //                        }
        //                        if (baseAttributeSymbol.MemberName == attribute.MemberName.Value) {
        //                            ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.DuplicateMemberName, baseAttributeSymbol.MemberName),
        //                                attribute.MemberName.TextSpan);
        //                        }
        //                    }
        //                }
        //                attributeSymbolList.Add(attribute.CreateSymbol(attributeSetSymbol, null, displayNameBase));
        //            }
        //        }
        //    }
        //    else {
        //        if (AttributeList != null) {
        //            foreach (var attribute in AttributeList) {
        //                AttributeSymbol restrictedAttributeSymbol = null;
        //                if (baseAttributeSymbolList != null) {
        //                    foreach (var baseAttributeSymbol in baseAttributeSymbolList) {
        //                        if (baseAttributeSymbol.MemberName == attribute.MemberName.Value) {
        //                            restrictedAttributeSymbol = baseAttributeSymbol;
        //                            break;
        //                        }
        //                    }
        //                }
        //                if (restrictedAttributeSymbol == null) {
        //                    ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.CannotFindRestrictedAttribute, attribute.MemberName.ToString()),
        //                        attribute.MemberName.TextSpan);
        //                }
        //                if (attribute.FullName != restrictedAttributeSymbol.FullName) {
        //                    ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.AttributeFullNameNotEqualToRestricted,
        //                        attribute.FullName.ToString(), restrictedAttributeSymbol.FullName.ToString()), attribute.MemberName.TextSpan);
        //                }
        //                if (attribute.IsOptional && !restrictedAttributeSymbol.IsOptional) {
        //                    ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.AttributeIsOptionalButRestrictedIsRequired),
        //                        attribute.Optional);
        //                }
        //                if (attribute.IsNullable && !restrictedAttributeSymbol.IsNullable) {
        //                    ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.AttributeIsNullableButRestrictedIsNotNullable),
        //                        attribute.MemberName.TextSpan);
        //                }
        //                var attributeSymbol = attribute.CreateSymbol(attributeSetSymbol, restrictedAttributeSymbol, displayNameBase);
        //                if (attributeSymbol.DeclKind != restrictedAttributeSymbol.DeclKind) {
        //                    ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.AttributeDeclarationNotEqualToRestricted,
        //                        attributeSymbol.DeclKind.ToString(), restrictedAttributeSymbol.DeclKind.ToString()),
        //                        attribute.MemberName.TextSpan);
        //                }
        //                if (!attributeSymbol.Type.IsEqualToOrDeriveFrom(restrictedAttributeSymbol.Type)) {
        //                    ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.AttributeTypeNotEqualToOrDeriveFromRestricted,
        //                        attributeSymbol.Type.FullName.ToString(), restrictedAttributeSymbol.Type.FullName.ToString()),
        //                        attribute.MemberName.TextSpan);
        //                }
        //                attributeSymbolList.Add(attributeSymbol);
        //            }
        //        }
        //        if (baseAttributeSymbolList != null) {
        //            foreach (var baseAttributeSymbol in baseAttributeSymbolList) {
        //                if (!baseAttributeSymbol.IsOptional) {
        //                    var found = false;
        //                    foreach (var attributeSymbol in attributeSymbolList) {
        //                        if (baseAttributeSymbol.MemberName == attributeSymbol.MemberName) {
        //                            found = true;
        //                            break;
        //                        }
        //                    }
        //                    if (!found) {
        //                        ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.RequiredAttributeNotRestricting, baseAttributeSymbol.MemberName), CloseBracketToken);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    return attributeSetSymbol;
        //}
    }
    //public abstract class MemberAttributeNode : Node {
    //    protected MemberAttributeNode(Node parent) : base(parent) { }
    //    public TextSpan Optional;
    //    public NameNode MemberName;
    //    public FullName FullName;
    //    public bool IsNullable;
    //    public bool IsOptional {
    //        get {
    //            return Optional.IsValid;
    //        }
    //    }
    //    public abstract void Resolve();
    //    public abstract AttributeSymbol CreateSymbol(AttributeSetSymbol parent, AttributeSymbol restrictedAttribute, string displayNameBase);
    //}

    public sealed class AttributeNode : Node {
        public AttributeNode(Node parent) : base(parent) { }
        public NameNode NameNode;
        public TextSpan Nullable;
        public OptionalOrDeleteNode OptionalOrDelete;
        public TextSpan Deletion;
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
        public AttributeSymbol CreateSymbol(AttributeSetSymbol parent, AttributeSymbol restrictedAttributeSymbol, string displayNameBase) {
            var typeSymbol = Type.CreateSymbol() as SimpleTypeSymbol;
            if (typeSymbol == null) {
                ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.SimpleTypeRequired), TypeQName.TextSpan);
            }
            var name = Name;
            return new AttributeSymbol(parent, "CLS_" + name, name, displayNameBase + name, IsOptional, IsNullable, typeSymbol, restrictedAttributeSymbol);
        }
    }
    public struct OptionalOrDeleteNode {
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

    //public sealed class GlobalAttributeRefNode : MemberAttributeNode {
    //    public GlobalAttributeRefNode(Node parent) : base(parent) { }
    //    public QualifiableNameNode GlobalAttributeQName;
    //    public GlobalAttributeNode GlobalAttribute;
    //    public override void Resolve() {
    //        GlobalAttribute = NamespaceAncestor.ResolveAsAttribute(GlobalAttributeQName);
    //        FullName = GlobalAttribute.FullName;
    //        IsNullable = GlobalAttribute.IsNullable;
    //    }
    //    public override AttributeSymbol CreateSymbol(AttributeSetSymbol parent, AttributeSymbol restrictedAttribute, string displayNameBase) {
    //        var globalAttributeSymbol = (AttributeSymbol)GlobalAttribute.CreateSymbol();
    //        return new AttributeSymbol(parent, "CLS_" + MemberName.Value, false, false, FullName, ElementKind.Reference, restrictedAttribute, null, globalAttributeSymbol,
    //            displayNameBase + "." + MemberName.Value, MemberName.Value, IsNullable, IsOptional, globalAttributeSymbol.Type);
    //    }
    //}

}
