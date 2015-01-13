using System;
using System.Collections.Generic;
using XData.IO.Text;

namespace XData.Compiler {
    public sealed class ProgramNode : Node {
        public ProgramNode() : base(null) {
            CompilationUnitList = new List<CompilationUnitNode>();
        }
        public readonly List<CompilationUnitNode> CompilationUnitList;
        public List<CodeCompilationUnitNode> CodeCompilationUnitList;
        //
        public List<NamespaceNode> NamespaceList;
        public Dictionary<string, List<NamespaceNode>> NamespaceDict;
        public void Analyze() {
            var nsList = new List<NamespaceNode>();
            foreach (var cu in CompilationUnitList) {
                if (cu.NamespaceList != null) {
                    nsList.AddRange(cu.NamespaceList);
                }
            }
            NamespaceList = nsList;
            //
            var nsDict = new Dictionary<string, List<NamespaceNode>>();
            foreach (var ns in nsList) {
                var uri = ns.Uri;
                List<NamespaceNode> list;
                if (!nsDict.TryGetValue(uri, out list)) {
                    list = new List<NamespaceNode>();
                    nsDict.Add(uri, list);
                }
                list.Add(ns);
            }
            NamespaceDict = nsDict;
            //
            foreach (var snsList in nsDict.Values) {
                for (var i = 0; i < snsList.Count - 1; ++i) {
                    for (var j = i + 1; j < snsList.Count; ++j) {
                        snsList[i].CheckDuplicateMembers(snsList[j].MemberList);
                    }
                }
            }
            //
            foreach (var ns in nsList) {

            }


            //var programSymbol = new ProgramSymbol();

            //return programSymbol;
        }
    }
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
        private CompilationUnitNode _compilationUnit;
        public CompilationUnitNode CompilationUnit {
            get {
                return _compilationUnit ?? (_compilationUnit = GetAncestor<CompilationUnitNode>());
            }
        }
        private NamespaceNode _namespace;
        public NamespaceNode NamespaceNode {
            get {
                return _namespace ?? (_namespace = GetAncestor<NamespaceNode>());
            }
        }
    }
    public class CompilationUnitNode : Node {
        public CompilationUnitNode(Node parent) : base(parent) { }
        public List<UriAliasingNode> UriAliasingList;
        public List<NamespaceNode> NamespaceList;
    }
    public sealed class CodeCompilationUnitNode : CompilationUnitNode {
        public CodeCompilationUnitNode(Node parent) : base(parent) { }
        new public List<CodeNamespaceNode> NamespaceList;
    }
    public class NamespaceNode : Node {
        public NamespaceNode(Node parent) : base(parent) { }
        public UriNode UriNode;
        public List<ImportNode> ImportList;
        public List<NamespaceMemberNode> MemberList;
        //
        public string Uri {
            get {
                return UriNode.Value;
            }
        }
        public void CheckDuplicateMembers(List<NamespaceMemberNode> otherList) {
            if (MemberList != null && otherList != null) {
                foreach (var thisItem in MemberList) {
                    foreach (var otherItem in otherList) {
                        if (thisItem.Name == otherItem.Name) {

                        }
                    }
                }
            }
        }
    }
    public sealed class CodeNamespaceNode : NamespaceNode {
        public CodeNamespaceNode(Node parent) : base(parent) { }
        public CSharpNamespaceNameNode Name;
    }
    public sealed class CSharpNamespaceNameNode : List<string>, IEquatable<CSharpNamespaceNameNode> {
        public TextSpan TextSpan;
        public bool Equals(CSharpNamespaceNameNode other) {
            if (object.ReferenceEquals(this, other)) return true;
            if (object.ReferenceEquals(other, null)) return false;
            var count = Count;
            if (count != other.Count) {
                return false;
            }
            for (var i = 0; i < count; i++) {
                if (this[i] != other[i]) {
                    return false;
                }
            }
            return true;
        }
        public override bool Equals(object obj) {
            return Equals(obj as CSharpNamespaceNameNode);
        }
        public override int GetHashCode() {
            var hash = 17;
            var count = Math.Min(Count, 5);
            for (var i = 0; i < count; i++) {
                hash = Extensions.AggregateHash(hash, this[i].GetHashCode());
            }
            return hash;
        }
        public static bool operator ==(CSharpNamespaceNameNode left, CSharpNamespaceNameNode right) {
            if (object.ReferenceEquals(left, null)) {
                return object.ReferenceEquals(right, null);
            }
            return left.Equals(right);
        }
        public static bool operator !=(CSharpNamespaceNameNode left, CSharpNamespaceNameNode right) {
            return !(left == right);
        }
    }

    public struct UriNode {
        public UriNode(NameNode alias, AtomicValueNode stringValue, string value) {
            Alias = alias;
            StringValue = stringValue;
            Value = value;
        }
        public readonly NameNode Alias;
        public readonly AtomicValueNode StringValue;
        public readonly string Value;
        public bool IsValid {
            get {
                return Alias.IsValid || StringValue.IsValid;
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
    public abstract class NamespaceMemberNode : Node {
        protected NamespaceMemberNode(Node parent)
            : base(parent) {
        }
        public NameNode Name;
    }
    public sealed class TypeNode : NamespaceMemberNode {
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
        public RootStructuralChildrenNode StructuralChildren;
    }
    public abstract class TypeDerivation : TypeBodyNode {
        public TypeDerivation(Node parent) : base(parent) { }
        public QualifiableNameNode BaseQName;
        public AttributesNode Attributes;
        public RootStructuralChildrenNode StructuralChildren;
    }
    public sealed class TypeExtension : TypeDerivation {
        public TypeExtension(Node parent) : base(parent) { }
    }
    public sealed class TypeRestriction : TypeDerivation {
        public TypeRestriction(Node parent) : base(parent) { }
        public SimpleValueRestrictionsNode SimpleValueRestrictions;
    }

    public sealed class AttributesNode : Node {
        public AttributesNode(Node parent) : base(parent) { }
        public List<MemberAttributeNode> AttributeList;
        public TextSpan CloseTokenTextSpan;
    }
    public sealed class RootStructuralChildrenNode : Node {
        public RootStructuralChildrenNode(Node parent) : base(parent) { }
        public List<MemberChildNode> ChildList;
        public TextSpan CloseTokenTextSpan;
    }
    public sealed class SimpleValueRestrictionsNode : Node {
        public SimpleValueRestrictionsNode(Node parent) : base(parent) { }
        public IntegerRangeNode<ulong> Lengths;
        public IntegerRangeNode<byte> Digits;
        public ValueRangeNode Values;
        public EnumerationsNode Enumerations;
        public AtomicValueNode Pattern;
    }

    public struct IntegerRangeNode<T> where T : struct {
        public IntegerRangeNode(T? minValue, T? maxValue, TextSpan textSpan) {
            MinValue = minValue;
            MaxValue = maxValue;
            TextSpan = textSpan;
        }
        public readonly T? MinValue;
        public readonly T? MaxValue;
        public readonly TextSpan TextSpan;
        public bool IsValid {
            get {
                return TextSpan.IsValid;
            }
        }
    }
    public struct ValueRangeNode {
        public ValueRangeNode(ValueBoundaryNode? minValue, ValueBoundaryNode? maxValue, TextSpan textSpan) {
            MinValue = minValue;
            MaxValue = maxValue;
            TextSpan = textSpan;
        }
        public readonly ValueBoundaryNode? MinValue;
        public readonly ValueBoundaryNode? MaxValue;
        public readonly TextSpan TextSpan;
        public bool IsValid {
            get {
                return TextSpan.IsValid;
            }
        }
    }
    public struct ValueBoundaryNode {
        public ValueBoundaryNode(SimpleValueNode value, bool isInclusive) {
            Value = value;
            IsInclusive = isInclusive;
        }
        public readonly SimpleValueNode Value;
        public readonly bool IsInclusive;
        public bool IsValid {
            get {
                return Value.IsValid;
            }
        }
    }
    public struct EnumerationsNode {
        public EnumerationsNode(List<SimpleValueNode> itemList, TextSpan textSpan) {
            ItemList = itemList;
            TextSpan = textSpan;
        }
        public readonly List<SimpleValueNode> ItemList;
        public readonly TextSpan TextSpan;
        public bool IsValid {
            get {
                return TextSpan.IsValid;
            }
        }
    }



    public sealed class GlobalAttributeNode : NamespaceMemberNode {
        public GlobalAttributeNode(Node parent) : base(parent) { }
        public TextSpan Nullable;
        public QualifiableNameNode TypeQName;
        public bool IsNullable {
            get {
                return Nullable.IsValid;
            }
        }
    }
    public abstract class MemberAttributeNode : Node {
        protected MemberAttributeNode(Node parent) : base(parent) { }
        public TextSpan Optional;
        public NameNode MemberName;
        public bool IsOptional {
            get {
                return Optional.IsValid;
            }
        }
    }
    public sealed class LocalAttributeNode : MemberAttributeNode {
        public LocalAttributeNode(Node parent) : base(parent) { }
        public NameNode Name;
        public TextSpan Nullable;
        public TextSpan Qualified;
        public QualifiableNameNode TypeQName;
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
    }
    public sealed class GlobalAttributeRefNode : MemberAttributeNode {
        public GlobalAttributeRefNode(Node parent) : base(parent) { }
        public QualifiableNameNode QName;
    }



    public sealed class GlobalElementNode : NamespaceMemberNode {
        public GlobalElementNode(Node parent) : base(parent) { }
        public NameNode AbstractOrSealed;
        public TextSpan Nullable;
        public QualifiableNameNode Substitution;
        public QualifiableNameNode TypeQName;
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
    }
    public abstract class MemberChildNode : Node {
        protected MemberChildNode(Node parent) : base(parent) { }
        public OccurrenceNode Occurrence;
        public NameNode MemberName;
    }
    public sealed class LocalElementNode : MemberChildNode {
        public LocalElementNode(Node parent) : base(parent) { }
        public NameNode Name;
        public TextSpan Nullable;
        public QualifiableNameNode TypeQName;
        public bool IsNullable {
            get {
                return Nullable.IsValid;
            }
        }
    }
    public sealed class GlobalElementRefNode : MemberChildNode {
        public GlobalElementRefNode(Node parent) : base(parent) { }
        public QualifiableNameNode QName;
    }
    public sealed class StructuralChildrenNode : MemberChildNode {
        public StructuralChildrenNode(Node parent) : base(parent) { }
        public bool IsSequence;
        public List<MemberChildNode> ChildList;
        public TextSpan CloseTokenTextSpan;
    }


    public struct OccurrenceNode {
        public OccurrenceNode(ulong minValue, ulong maxValue, TextSpan textSpan) {
            MinValue = minValue;
            MaxValue = maxValue;
            TextSpan = textSpan;
        }
        public readonly ulong MinValue;
        public readonly ulong MaxValue;
        public readonly TextSpan TextSpan;
        public bool IsValid {
            get {
                return TextSpan.IsValid;
            }
        }
    }


}
