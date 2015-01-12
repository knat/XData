using System;
using System.Collections.Generic;
using XData.TextIO;

namespace XData.Compiler {
    public enum DiagnosticCodeEx {
        None = 0,
        UInt64ValueRequired = -2000,
        ByteValueRequired,
        MaxValueMustEqualToOrBeGreaterThanMinValue,
        MaxValueMustBeGreaterThanZero,

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
        public List<NamespaceMemberNode> MemberList;
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
