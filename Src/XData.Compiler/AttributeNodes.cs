using System;
using System.Collections.Generic;
using XData.IO.Text;

namespace XData.Compiler {
    public abstract class EntityNode : NamespaceMemberNode {
        protected EntityNode(Node parent) : base(parent) { }
        public TextSpan Nullable;
        public QualifiableNameNode SubstitutedEntityQName;
        public QualifiableNameNode TypeQName;
        public TypeNode Type;
        public bool IsNullable {
            get {
                return Nullable.IsValid;
            }
        }
        public override void Resolve() {
            Type = NamespaceAncestor.ResolveAsType(TypeQName);
        }
    }

    public sealed class GlobalAttributeNode : EntityNode {
        public GlobalAttributeNode(Node parent) : base(parent) { }
        public GlobalAttributeNode SubstitutedAttribute;
        public override void Resolve() {
            base.Resolve();
            if (SubstitutedEntityQName.IsValid) {
                SubstitutedAttribute = NamespaceAncestor.ResolveAsAttribute(SubstitutedEntityQName);
            }
        }
        protected override NamedObjectSymbol CreateSymbolCore(NamespaceSymbol parent, string csName, FullName fullName) {
            AttributeSymbol substitutedAttributeSymbol = null;
            if (SubstitutedAttribute != null) {
                substitutedAttributeSymbol = (AttributeSymbol)SubstitutedAttribute.CreateSymbol();
            }
            var simpleTypeSymbol = Type.CreateSymbol() as SimpleTypeSymbol;
            if (simpleTypeSymbol == null) {
                ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.SimpleTypeRequired), TypeQName.TextSpan);
            }
            if (substitutedAttributeSymbol != null) {
                if (IsNullable && !substitutedAttributeSymbol.IsNullable) {
                    ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.AttributeIsNullableButSubstitutedIsNotNullable),
                        Name.TextSpan);
                }
                if (!simpleTypeSymbol.IsEqualToOrDeriveFrom(substitutedAttributeSymbol.Type)) {
                    ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.AttributeTypeNotEqualToOrDeriveFromSubstituted,
                        simpleTypeSymbol.FullName.ToString(), substitutedAttributeSymbol.Type.FullName.ToString()),
                        TypeQName.TextSpan);
                }
            }
            return new AttributeSymbol(parent, csName, IsAbstract, IsSealed, fullName, EntityDeclKind.Global, null, substitutedAttributeSymbol, null,
                fullName.ToString(), null, IsNullable, false, simpleTypeSymbol);
        }
    }
    public sealed class AttributesNode : Node {
        public AttributesNode(Node parent) : base(parent) { }
        public List<MemberAttributeNode> AttributeList;
        public TextSpan OpenBracketToken, CloseBracketToken;
        public void Resolve() {
            if (AttributeList != null) {
                var count = AttributeList.Count;
                for (var i = 0; i < count; ++i) {
                    var attribute = AttributeList[i];
                    attribute.Resolve();
                    for (var j = 0; j < i - 1; ++j) {
                        if (AttributeList[j].FullName == attribute.FullName) {
                            ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.DuplicateAttributeFullName, attribute.FullName.ToString()),
                                attribute.MemberName.TextSpan);
                        }
                    }
                }
            }
        }
        public AttributeSetSymbol CreateSymbol(ComplexTypeSymbol parent, AttributeSetSymbol baseAttributeSetSymbol, bool isExtension) {
            var baseAttributeSymbolList = baseAttributeSetSymbol != null ? baseAttributeSetSymbol.AttributeList : null;
            var attributeSetSymbol = new AttributeSetSymbol(parent, baseAttributeSetSymbol);
            var attributeSymbolList = attributeSetSymbol.AttributeList;
            var displayNameBase = parent.FullName.ToString() + ".[]";
            if (isExtension) {
                if (baseAttributeSymbolList != null) {
                    attributeSymbolList.AddRange(baseAttributeSymbolList);
                }
                if (AttributeList != null) {
                    foreach (var attribute in AttributeList) {
                        if (baseAttributeSymbolList != null) {
                            foreach (var baseAttributeSymbol in baseAttributeSymbolList) {
                                if (baseAttributeSymbol.FullName == attribute.FullName) {
                                    ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.DuplicateAttributeFullName, attribute.FullName.ToString()),
                                        attribute.MemberName.TextSpan);
                                }
                                if (baseAttributeSymbol.MemberName == attribute.MemberName.Value) {
                                    ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.DuplicateMemberName, baseAttributeSymbol.MemberName),
                                        attribute.MemberName.TextSpan);
                                }
                            }
                        }
                        attributeSymbolList.Add(attribute.CreateSymbol(attributeSetSymbol, null, displayNameBase));
                    }
                }
            }
            else {
                if (AttributeList != null) {
                    foreach (var attribute in AttributeList) {
                        AttributeSymbol restrictedAttributeSymbol = null;
                        if (baseAttributeSymbolList != null) {
                            foreach (var baseAttributeSymbol in baseAttributeSymbolList) {
                                if (baseAttributeSymbol.MemberName == attribute.MemberName.Value) {
                                    restrictedAttributeSymbol = baseAttributeSymbol;
                                    break;
                                }
                            }
                        }
                        if (restrictedAttributeSymbol == null) {
                            ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.CannotFindRestrictedAttribute, attribute.MemberName.ToString()),
                                attribute.MemberName.TextSpan);
                        }
                        if (attribute.FullName != restrictedAttributeSymbol.FullName) {
                            ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.AttributeFullNameNotEqualToRestricted,
                                attribute.FullName.ToString(), restrictedAttributeSymbol.FullName.ToString()), attribute.MemberName.TextSpan);
                        }
                        if (attribute.IsOptional && !restrictedAttributeSymbol.IsOptional) {
                            ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.AttributeIsOptionalButRestrictedIsRequired),
                                attribute.Optional);
                        }
                        if (attribute.IsNullable && !restrictedAttributeSymbol.IsNullable) {
                            ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.AttributeIsNullableButRestrictedIsNotNullable),
                                attribute.MemberName.TextSpan);
                        }
                        var attributeSymbol = attribute.CreateSymbol(attributeSetSymbol, restrictedAttributeSymbol, displayNameBase);
                        if (attributeSymbol.DeclKind != restrictedAttributeSymbol.DeclKind) {
                            ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.AttributeDeclarationNotEqualToRestricted,
                                attributeSymbol.DeclKind.ToString(), restrictedAttributeSymbol.DeclKind.ToString()),
                                attribute.MemberName.TextSpan);
                        }
                        if (!attributeSymbol.Type.IsEqualToOrDeriveFrom(restrictedAttributeSymbol.Type)) {
                            ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.AttributeTypeNotEqualToOrDeriveFromRestricted,
                                attributeSymbol.Type.FullName.ToString(), restrictedAttributeSymbol.Type.FullName.ToString()),
                                attribute.MemberName.TextSpan);
                        }
                        attributeSymbolList.Add(attributeSymbol);
                    }
                }
                if (baseAttributeSymbolList != null) {
                    foreach (var baseAttributeSymbol in baseAttributeSymbolList) {
                        if (!baseAttributeSymbol.IsOptional) {
                            var found = false;
                            foreach (var attributeSymbol in attributeSymbolList) {
                                if (baseAttributeSymbol.MemberName == attributeSymbol.MemberName) {
                                    found = true;
                                    break;
                                }
                            }
                            if (!found) {
                                ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.RequiredAttributeNotRestricting, baseAttributeSymbol.MemberName), CloseBracketToken);
                            }
                        }
                    }
                }
            }
            return attributeSetSymbol;
        }
    }
    public abstract class MemberAttributeNode : Node {
        protected MemberAttributeNode(Node parent) : base(parent) { }
        public TextSpan Optional;
        public NameNode MemberName;
        public FullName FullName;
        public bool IsNullable;
        public bool IsOptional {
            get {
                return Optional.IsValid;
            }
        }
        public abstract void Resolve();
        public abstract AttributeSymbol CreateSymbol(AttributeSetSymbol parent, AttributeSymbol restrictedAttribute, string displayNameBase);
    }
    public sealed class LocalAttributeNode : MemberAttributeNode {
        public LocalAttributeNode(Node parent) : base(parent) { }
        public NameNode Name;
        public TextSpan Nullable;
        public TextSpan Qualified;
        public QualifiableNameNode TypeQName;
        public TypeNode Type;
        public bool IsQualified {
            get {
                return Qualified.IsValid;
            }
        }
        public override void Resolve() {
            Type = NamespaceAncestor.ResolveAsType(TypeQName);
            FullName = new FullName(IsQualified ? NamespaceAncestor.Uri : null, Name.Value);
            IsNullable = Nullable.IsValid;
        }
        public override AttributeSymbol CreateSymbol(AttributeSetSymbol parent, AttributeSymbol restrictedAttribute, string displayNameBase) {
            var typeSymbol = Type.CreateSymbol() as SimpleTypeSymbol;
            if (typeSymbol == null) {
                ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.SimpleTypeRequired), TypeQName.TextSpan);
            }
            return new AttributeSymbol(parent, "CLS_" + MemberName.Value, false, false, FullName, EntityDeclKind.Local, restrictedAttribute, null, null,
                displayNameBase + "." + MemberName.Value, MemberName.Value, IsNullable, IsOptional, typeSymbol);
        }
    }
    public sealed class GlobalAttributeRefNode : MemberAttributeNode {
        public GlobalAttributeRefNode(Node parent) : base(parent) { }
        public QualifiableNameNode GlobalAttributeQName;
        public GlobalAttributeNode GlobalAttribute;
        public override void Resolve() {
            GlobalAttribute = NamespaceAncestor.ResolveAsAttribute(GlobalAttributeQName);
            FullName = GlobalAttribute.FullName;
            IsNullable = GlobalAttribute.IsNullable;
        }
        public override AttributeSymbol CreateSymbol(AttributeSetSymbol parent, AttributeSymbol restrictedAttribute, string displayNameBase) {
            var globalAttributeSymbol = (AttributeSymbol)GlobalAttribute.CreateSymbol();
            return new AttributeSymbol(parent, "CLS_" + MemberName.Value, false, false, FullName, EntityDeclKind.Reference, restrictedAttribute, null, globalAttributeSymbol,
                displayNameBase + "." + MemberName.Value, MemberName.Value, IsNullable, IsOptional, globalAttributeSymbol.Type);
        }


    }

}
