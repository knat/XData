using System;
using System.Collections.Generic;
using XData.IO.Text;

namespace XData.Compiler {
    public sealed class GlobalAttributeNode : NamespaceMemberNode {
        public GlobalAttributeNode(Node parent) : base(parent) { }
        public TextSpan Nullable;
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
        protected override NamedObjectSymbol CreateSymbolCore() {
            var typeSymbol = Type.CreateSymbol();
            var simpleTypeSymbol = typeSymbol as SimpleTypeSymbol;
            if (simpleTypeSymbol == null) {
                ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.SimpleTypeRequired, typeSymbol.FullName.ToString()), TypeQName.TextSpan);
            }


            throw new NotImplementedException();
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
        public AttributeSetSymbol CreateSymbol(ComplexTypeSymbol parent, AttributeSetSymbol baseAttributeSet, bool isExtension) {

            return null;
        }
    }
    public abstract class MemberAttributeNode : Node {
        protected MemberAttributeNode(Node parent) : base(parent) { }
        public TextSpan Optional;
        public NameNode MemberName;
        public FullName FullName;
        public bool IsOptional {
            get {
                return Optional.IsValid;
            }
        }
        public abstract void Resolve();
    }
    public sealed class LocalAttributeNode : MemberAttributeNode {
        public LocalAttributeNode(Node parent) : base(parent) { }
        public NameNode Name;
        public TextSpan Nullable;
        public TextSpan Qualified;
        public QualifiableNameNode TypeQName;
        public TypeNode Type;
        public bool IsNullable {
            get {
                return Nullable.IsValid;
            }
        }
        public bool IsQualified {
            get {
                return Qualified.IsValid;
            }
        }
        public override void Resolve() {
            Type = NamespaceAncestor.ResolveAsType(TypeQName);
            FullName = new FullName(IsQualified ? NamespaceAncestor.Uri : null, Name.Value);
        }
    }
    public sealed class GlobalAttributeRefNode : MemberAttributeNode {
        public GlobalAttributeRefNode(Node parent) : base(parent) { }
        public QualifiableNameNode GlobalAttributeQName;
        public GlobalAttributeNode GlobalAttribute;
        public override void Resolve() {
            GlobalAttribute = NamespaceAncestor.ResolveAsAttribute(GlobalAttributeQName);
            FullName = GlobalAttribute.FullName;
        }
    }

}
