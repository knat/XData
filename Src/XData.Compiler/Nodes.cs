using System;
using System.Collections.Generic;
using XData.TextIO;

namespace XData.Compiler {
    public abstract class Node {
        protected Node(Node parent) {
            Parent = parent;
        }
        public readonly Node Parent;
        public T GetAncestor<T>(bool @try = false, bool testSelf = false) where T : class {
            for (var obj = testSelf ? this : Parent; obj != null; obj = obj.Parent) {
                var res = obj as T;
                if (res != null) {
                    return res;
                }
            }
            if (!@try) {
                throw new InvalidOperationException("Cannot get ancestor: " + typeof(T).FullName);
            }
            return null;
        }

    }
    public sealed class CompilerNode : Node {
        public CompilerNode() : base(null) { }
        public List<CompilationUnitNode> CompilationUnitList;
    }
    public sealed class CompilationUnitNode : Node {
        public CompilationUnitNode(Node parent)
            : base(parent) {
        }
        public List<UriAliasingNode> UriAliasingList;
        public List<NamespaceNode> NamespaceList;
    }
    public struct UriNode {
        public UriNode(NameNode alias, AtomicValueNode value) {
            Alias = alias;
            Value = value;
        }
        public readonly NameNode Alias;
        public readonly AtomicValueNode Value;
        public bool IsValid {
            get {
                return Alias.IsValid || Value.IsValid;
            }
        }
    }
    public struct UriAliasingNode {
        public UriAliasingNode(AtomicValueNode value, NameNode alias) {
            Value = value;
            Alias = alias;
        }
        public readonly AtomicValueNode Value;
        public readonly NameNode Alias;
    }
    public struct ImportNode {
        public ImportNode(UriNode uri, NameNode alias) {
            Uri = uri;
            Alias = alias;
        }
        public readonly UriNode Uri;
        public readonly NameNode Alias;//opt
    }
    public sealed class NamespaceNode : Node {
        public NamespaceNode(Node parent)
            : base(parent) {
        }
        public UriNode Uri;
        public List<ImportNode> ImportList;
        public List<MemberNode> MemberList;
    }
    public abstract class MemberNode : Node {
        protected MemberNode(Node parent)
            : base(parent) {
        }
        public NameNode Name;
    }
    public sealed class TypeNode : MemberNode {
        public TypeNode(Node parent)
            : base(parent) {

        }
        public NameNode AbstractOrSealed;
        public TypeBodyNode Body;
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

    }
    public abstract class TypeBodyNode : Node {
        protected TypeBodyNode(Node parent) : base(parent) { }
    }
    public sealed class TypeListNode : TypeBodyNode {
        public TypeListNode(Node parent) : base(parent) { }
        public QualifiableNameNode ItemQName;
    }
    public sealed class TypeDirectnessNode : TypeBodyNode {
        public TypeDirectnessNode(Node parent) : base(parent) { }
        public AttributesNode Attributes;
        public StructuralChildrenNode StructuralChildren;
    }
    public abstract class TypeDerivation : TypeBodyNode {
        public TypeDerivation(Node parent) : base(parent) { }
        public QualifiableNameNode BaseQName;
        public AttributesNode Attributes;
    }
    public sealed class TypeExtension : TypeDerivation {
        public TypeExtension(Node parent) : base(parent) { }
        public StructuralChildrenNode StructuralChildren;
    }
    public sealed class TypeRestriction : TypeDerivation {
        public TypeRestriction(Node parent) : base(parent) { }
        public ChildrenNode Children;
    }

    public sealed class AttributesNode : Node {
        public AttributesNode(Node parent) : base(parent) { }
    }
    public abstract class ChildrenNode : Node {
        public ChildrenNode(Node parent) : base(parent) { }
    }
    public sealed class StructuralChildrenNode : ChildrenNode {
        public StructuralChildrenNode(Node parent) : base(parent) { }
    }
    public sealed class RestrictedSimpleChildNode : ChildrenNode {
        public RestrictedSimpleChildNode(Node parent) : base(parent) { }
    }

    public sealed class GlobalAttributeNode : MemberNode {
        public GlobalAttributeNode(Node parent) : base(parent) { }
        public QualifiableNameNode TypeQName;
    }
    public abstract class AttributeNode :Node {
        protected AttributeNode(Node parent) : base(parent) { }
        public SimpleValueNode DefaultValue;
    }
    public sealed class LocalAttributeNode : AttributeNode {
        public LocalAttributeNode(Node parent) : base(parent) { }
        public NameNode Name;
        public NameNode Qualified;
        public QualifiableNameNode TypeQName;
    }
    public sealed class AttributeRefNode : AttributeNode {
        public AttributeRefNode(Node parent) : base(parent) { }
        public QualifiableNameNode QName;
    }



    public sealed class GlobalElementNode : MemberNode {
        public GlobalElementNode(Node parent) : base(parent) { }
        public NameNode AbstractOrSealed;
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

        public QualifiableNameNode TypeQName;

    }

}
