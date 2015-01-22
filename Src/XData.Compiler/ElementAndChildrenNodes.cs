using System;
using System.Collections.Generic;
using XData.IO.Text;

namespace XData.Compiler {
    public sealed class GlobalElementNode : NamespaceMemberNode {
        public GlobalElementNode(Node parent) : base(parent) { }
        public NameNode AbstractOrSealed;
        public TextSpan Nullable;
        public QualifiableNameNode SubstitutedGlobalElementQName;
        public QualifiableNameNode TypeQName;
        public GlobalElementNode SubstitutedGlobalElement;
        public TypeNode Type;
        public bool IsAbstract {
            get {
                return AbstractOrSealed.Value == Parser.AbstractKeyword;
            }
        }
        public bool IsSealed {
            get {
                return AbstractOrSealed.Value == Parser.SealedKeyword;
            }
        }
        public bool IsNullable {
            get {
                return Nullable.IsValid;
            }
        }
        public override void Resolve() {
            if (SubstitutedGlobalElementQName.IsValid) {
                SubstitutedGlobalElement = NamespaceAncestor.ResolveAsElement(SubstitutedGlobalElementQName);
            }
            Type = NamespaceAncestor.ResolveAsType(TypeQName);
        }
        protected override NamedObjectSymbol CreateSymbolCore() {
            throw new NotImplementedException();
        }
    }
    public sealed class RootComplexChildrenNode : Node {
        public RootComplexChildrenNode(Node parent) : base(parent) { }
        public List<MemberChildNode> ChildList;
        public TextSpan OpenBraceToken, CloseBraceToken;
        public void Resolve() {
            if (ChildList != null) {
                foreach (var item in ChildList) {
                    item.Resolve();
                }
            }
        }
        public ChildSetSymbol CreateSymbol(ComplexTypeSymbol parent, ChildSetSymbol baseChildSet, bool isExtension) {

            return null;
        }
    }
    public abstract class MemberChildNode : Node {
        protected MemberChildNode(Node parent) : base(parent) { }
        public OccurrenceNode Occurrence;
        public NameNode MemberName;
        public abstract void Resolve();
    }
    public abstract class MemberElementNode : MemberChildNode {
        protected MemberElementNode(Node parent) : base(parent) { }
        public FullName FullName;
    }
    public sealed class LocalElementNode : MemberElementNode {
        public LocalElementNode(Node parent) : base(parent) { }
        public NameNode Name;
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
            FullName = new FullName(NamespaceAncestor.Uri, Name.Value);
        }
    }
    public sealed class GlobalElementRefNode : MemberElementNode {
        public GlobalElementRefNode(Node parent) : base(parent) { }
        public QualifiableNameNode GlobalElementQName;
        public GlobalElementNode GlobalElement;
        public override void Resolve() {
            GlobalElement = NamespaceAncestor.ResolveAsElement(GlobalElementQName);
            FullName = GlobalElement.FullName;
        }
    }
    public sealed class ComplexChildrenNode : MemberChildNode {
        public ComplexChildrenNode(Node parent) : base(parent) { }
        public bool IsSequence;
        public List<MemberChildNode> ChildList;
        public TextSpan OpenBraceToken, CloseBraceToken;
        public override void Resolve() {
            if (ChildList != null) {
                foreach (var item in ChildList) {
                    item.Resolve();
                }
            }
        }
    }
    public struct OccurrenceNode {
        public OccurrenceNode(ulong minValue, ulong maxValue, TextSpan token) {
            MinValue = minValue;
            MaxValue = maxValue;
            Token = token;
        }
        public readonly ulong MinValue;
        public readonly ulong MaxValue;
        public readonly TextSpan Token;
        public bool IsValid {
            get {
                return Token.IsValid;
            }
        }
    }
}
