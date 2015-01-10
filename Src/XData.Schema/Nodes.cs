using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XData.TextIO;

namespace XData.Schema {
    public struct CompilationUnitNode {
        public CompilationUnitNode(List<UriAliasingNode> uriAliasingList, List<NamespaceNode> namespaceList) {
            UriAliasingList = uriAliasingList;
            NamespaceList = namespaceList;
        }
        public readonly List<UriAliasingNode> UriAliasingList;
        public readonly List<NamespaceNode> NamespaceList;
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
    public struct NamespaceNode {
        public NamespaceNode(List<ImportNode> importList, List<MemberNode> memberList) {
            ImportList = importList;
            MemberList = memberList;
        }
        public readonly List<ImportNode> ImportList;
        public readonly List<MemberNode> MemberList;
    }
    public abstract class MemberNode {
        protected MemberNode(NameNode name) {
            Name = name;
        }
        public readonly NameNode Name;
    }
    public sealed class TypeNode : MemberNode {
        public TypeNode(NameNode name) : base(name) {

        }
    }
    public sealed class AttributeNode : MemberNode {
        public AttributeNode(NameNode name) : base(name) {

        }

    }
    public sealed class ElementNode : MemberNode {
        public ElementNode(NameNode name) : base(name) {

        }

    }

}
