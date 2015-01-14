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
        public NamespaceNodeDict NamespaceDict;
        public void Analyze() {
            var nsList = new List<NamespaceNode>();
            foreach (var cu in CompilationUnitList) {
                if (cu.NamespaceList != null) {
                    nsList.AddRange(cu.NamespaceList);
                }
            }
            NamespaceList = nsList;
            //
            var nsDict = new NamespaceNodeDict();
            foreach (var ns in nsList) {
                var uri = ns.Uri;
                LogicalNamespaceNode logicalNS;
                if (!nsDict.TryGetValue(uri, out logicalNS)) {
                    logicalNS = new LogicalNamespaceNode();
                    nsDict.Add(uri, logicalNS);
                }
                logicalNS.Add(ns);
                ns.LogicalNamespace = logicalNS;
            }
            NamespaceDict = nsDict;
            //
            foreach (var ns in nsList) {
                ns.ResolveImports(nsDict);
            }
            foreach (var logicalNS in nsDict.Values) {
                logicalNS.CheckDuplicateMembers();
            }
            foreach (var ns in nsList) {
                ns.Resolve();
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
        private ProgramNode _program;
        public ProgramNode Program {
            get {
                return _program ?? (_program = GetAncestor<ProgramNode>());
            }
        }
        private CompilationUnitNode _compilationUnit;
        public CompilationUnitNode CompilationUnit {
            get {
                return _compilationUnit ?? (_compilationUnit = GetAncestor<CompilationUnitNode>());
            }
        }
        private NamespaceNode _namespace;
        public NamespaceNode Namespace {
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
    public sealed class NamespaceNodeDict : Dictionary<string, LogicalNamespaceNode> {

    }
    public sealed class LogicalNamespaceNode : List<NamespaceNode> {

        public void CheckDuplicateMembers() {
            var count = Count;
            for (var i = 0; i < count - 1; ++i) {
                for (var j = i + 1; j < count; ++j) {
                    this[i].CheckDuplicateMembers(this[j].MemberList);
                }
            }
        }
        public NamespaceMemberNode TryGetMember(NameNode name) {
            var count = Count;
            for (var i = 0; i < count; ++i) {
                var memberList = this[i].MemberList;
                if (memberList != null) {
                    foreach (var member in memberList) {
                        if (member.Name == name) {
                            return member;
                        }
                    }
                }
            }
            return null;
        }
    }
    public class NamespaceNode : Node {
        public NamespaceNode(Node parent) : base(parent) { }
        public UriNode UriNode;
        public List<ImportNode> ImportList;
        public List<NamespaceMemberNode> MemberList;
        //
        public LogicalNamespaceNode LogicalNamespace;
        //
        public string Uri {
            get {
                return UriNode.Value;
            }
        }
        public void ResolveImports(NamespaceNodeDict nsDict) {
            if (ImportList != null) {
                for (var i = 0; i < ImportList.Count; ++i) {
                    var import = ImportList[i];
                    if (!nsDict.TryGetValue(import.Uri.Value, out import.LogicalNamespace)) {
                        ContextEx.ErrorDiagnosticAndThrow(DiagnosticCodeEx.InvalidImportUri,
                            "Invalid import uri '{0}'.".InvFormat(import.Uri.Value), import.Uri.TextSpan);
                    }
                }
            }
        }
        public void CheckDuplicateMembers(List<NamespaceMemberNode> otherList) {
            if (MemberList != null && otherList != null) {
                foreach (var thisMember in MemberList) {
                    foreach (var otherMember in otherList) {
                        if (thisMember.Name == otherMember.Name) {
                            ContextEx.ErrorDiagnosticAndThrow(DiagnosticCodeEx.DuplicateNamespaceMember,
                                "Duplicate namespace member '{0}'.".InvFormat(otherMember.Name.ToString()), otherMember.Name.TextSpan);
                        }
                    }
                }
            }
        }
        public void Resolve() {
            if (MemberList != null) {
                foreach (var member in MemberList) {
                    member.Resolve();
                }
            }
        }
        public NamespaceMemberNode Resolve(QualifiableNameNode qName) {
            NamespaceMemberNode result = null;
            var name = qName.Name;
            if (qName.IsQualified) {
                var alias = qName.Alias;
                if (alias.Value == "sys") {
                    result = SystemTypeNode.TryGet(name.Value);
                }
                else {
                    ImportNode? import = null;
                    if (ImportList != null) {
                        foreach (var item in ImportList) {
                            if (item.Alias == alias) {
                                import = item;
                                break;
                            }
                        }
                    }
                    if (import == null) {
                        ContextEx.ErrorDiagnosticAndThrow(DiagnosticCodeEx.InvalidQualifiableNameAlias,
                            "Invalid qualifiable name alias '{0}'.".InvFormat(alias.ToString()), alias.TextSpan);
                    }
                    result = import.Value.LogicalNamespace.TryGetMember(name);
                }
            }
            else {
                result = LogicalNamespace.TryGetMember(name);
                if (result == null) {
                    result = SystemTypeNode.TryGet(name.Value);
                    if (ImportList != null) {
                        foreach (var item in ImportList) {
                            var member = item.LogicalNamespace.TryGetMember(name);
                            if (member != null) {
                                if (result != null) {
                                    ContextEx.ErrorDiagnosticAndThrow(DiagnosticCodeEx.AmbiguousNameReference,
                                        "Ambiguous name reference '{0}'.".InvFormat(name.ToString()), name.TextSpan);
                                }
                                result = member;
                            }
                        }
                    }
                }
            }
            if (result == null) {
                ContextEx.ErrorDiagnosticAndThrow(DiagnosticCodeEx.InvalidNameReference,
                    "Invalid name reference '{0}'.".InvFormat(name.ToString()), name.TextSpan);
            }
            return result;
        }
        public TypeNode ResolveAsType(QualifiableNameNode qName) {
            var result = Resolve(qName) as TypeNode;
            if (result == null) {
                ContextEx.ErrorDiagnosticAndThrow(DiagnosticCodeEx.InvalidTypeNameReference,
                    "Invalid type name reference '{0}'.".InvFormat(qName.ToString()), qName.TextSpan);
            }
            return result;
        }
        public GlobalAttributeNode ResolveAsAttribute(QualifiableNameNode qName) {
            var result = Resolve(qName) as GlobalAttributeNode;
            if (result == null) {
                ContextEx.ErrorDiagnosticAndThrow(DiagnosticCodeEx.InvalidAttributeNameReference,
                    "Invalid attribute name reference '{0}'.".InvFormat(qName.ToString()), qName.TextSpan);
            }
            return result;
        }
        public GlobalElementNode ResolveAsElement(QualifiableNameNode qName) {
            var result = Resolve(qName) as GlobalElementNode;
            if (result == null) {
                ContextEx.ErrorDiagnosticAndThrow(DiagnosticCodeEx.InvalidElementNameReference,
                    "Invalid element name reference '{0}'.".InvFormat(qName.ToString()), qName.TextSpan);
            }
            return result;
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
        //public bool IsValid {
        //    get {
        //        return Alias.IsValid || StringValue.IsValid;
        //    }
        //}
        public TextSpan TextSpan {
            get {
                if (Alias.IsValid) {
                    return Alias.TextSpan;
                }
                return StringValue.TextSpan;
            }
        }
    }
    public struct UriAliasingNode {
        public UriAliasingNode(AtomicValueNode uri, NameNode alias) {
            Uri = uri;
            Alias = alias;
        }
        public readonly AtomicValueNode Uri;
        public readonly NameNode Alias;
    }
    public struct ImportNode {
        public ImportNode(UriNode uri, NameNode alias) {
            Uri = uri;
            Alias = alias;
            LogicalNamespace = null;
        }
        public readonly UriNode Uri;
        public readonly NameNode Alias;//opt
        public LogicalNamespaceNode LogicalNamespace;
    }
    public abstract class NamespaceMemberNode : Node {
        protected NamespaceMemberNode(Node parent) : base(parent) {
        }
        public NameNode Name;
        private FullName? _fullName;
        public FullName FullName {
            get {
                if (_fullName == null) {
                    _fullName = new FullName(Namespace.Uri, Name.Value);
                }
                return _fullName.Value;
            }
        }
        public abstract void Resolve();
    }
    public sealed class SystemTypeNode : TypeNode {
        private SystemTypeNode() : base(null) {
        }
        public TypeKind Kind;
        private static readonly Dictionary<string, SystemTypeNode> Dict;
        static SystemTypeNode() {
            Dict = new Dictionary<string, SystemTypeNode>();
            for (var kind = TypeKind.SimpleType; kind <= TypeKind.Time; ++kind) {
                var name = kind.ToString();
                Dict.Add(name, new SystemTypeNode() { Name = new NameNode(name, default(TextSpan)), Kind = kind });
            }
        }
        public static SystemTypeNode TryGet(string name) {
            SystemTypeNode result;
            Dict.TryGetValue(name, out result);
            return result;
        }
    }
    public class TypeNode : NamespaceMemberNode {
        public TypeNode(Node parent) : base(parent) {
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
        public override void Resolve() {
            Body.Resolve();
        }
    }
    public abstract class TypeBodyNode : Node {
        protected TypeBodyNode(Node parent) : base(parent) { }
        public abstract void Resolve();
    }
    public sealed class TypeListNode : TypeBodyNode {
        public TypeListNode(Node parent) : base(parent) { }
        public QualifiableNameNode ItemQName;
        public TypeNode ItemType;
        public override void Resolve() {
            ItemType = Namespace.ResolveAsType(ItemQName);
        }
    }
    public sealed class TypeDirectnessNode : TypeBodyNode {
        public TypeDirectnessNode(Node parent) : base(parent) { }
        public AttributesNode Attributes;
        public RootStructuralChildrenNode StructuralChildren;
        public override void Resolve() {
            if (Attributes != null) {
                Attributes.Resolve();
            }
            if (StructuralChildren != null) {
                StructuralChildren.Resolve();
            }
        }
    }
    public abstract class TypeDerivation : TypeBodyNode {
        public TypeDerivation(Node parent) : base(parent) { }
        public QualifiableNameNode BaseQName;
        public AttributesNode Attributes;
        public RootStructuralChildrenNode StructuralChildren;
        public TypeNode BaseType;
        public override void Resolve() {
            BaseType = Namespace.ResolveAsType(BaseQName);
            if (Attributes != null) {
                Attributes.Resolve();
            }
            if (StructuralChildren != null) {
                StructuralChildren.Resolve();
            }
        }
    }
    public sealed class TypeExtension : TypeDerivation {
        public TypeExtension(Node parent) : base(parent) { }
    }
    public sealed class TypeRestriction : TypeDerivation {
        public TypeRestriction(Node parent) : base(parent) { }
        public SimpleValueRestrictionsNode SimpleValueRestrictions;
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
        public TypeNode Type;
        public bool IsNullable {
            get {
                return Nullable.IsValid;
            }
        }
        public override void Resolve() {
            Type = Namespace.ResolveAsType(TypeQName);
        }
    }
    public sealed class AttributesNode : Node {
        public AttributesNode(Node parent) : base(parent) { }
        public List<MemberAttributeNode> AttributeList;
        public TextSpan CloseTokenTextSpan;
        public void Resolve() {
            if (AttributeList != null) {
                var count = AttributeList.Count;
                for (var i = 0; i < count; ++i) {
                    var attribute = AttributeList[i];
                    attribute.Resolve();
                    for (var j = 0; j < i - 1; ++j) {
                        if (AttributeList[j].FullName == attribute.FullName) {
                            ContextEx.ErrorDiagnosticAndThrow(DiagnosticCodeEx.DuplicateAttributeFullName,
                                "Duplicate attribute full name '{0}'.".InvFormat(attribute.FullName.ToString()), attribute.MemberName.TextSpan);
                        }
                    }
                }
            }
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
            Type = Namespace.ResolveAsType(TypeQName);
            FullName = new FullName(IsQualified ? Namespace.Uri : null, Name.Value);
        }
    }
    public sealed class GlobalAttributeRefNode : MemberAttributeNode {
        public GlobalAttributeRefNode(Node parent) : base(parent) { }
        public QualifiableNameNode GlobalAttributeQName;
        public GlobalAttributeNode GlobalAttribute;
        public override void Resolve() {
            GlobalAttribute = Namespace.ResolveAsAttribute(GlobalAttributeQName);
            FullName = GlobalAttribute.FullName;
        }
    }

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
                SubstitutedGlobalElement = Namespace.ResolveAsElement(SubstitutedGlobalElementQName);
            }
            Type = Namespace.ResolveAsType(TypeQName);
        }
    }
    public sealed class RootStructuralChildrenNode : Node {
        public RootStructuralChildrenNode(Node parent) : base(parent) { }
        public List<MemberChildNode> ChildList;
        public TextSpan CloseTokenTextSpan;
        public void Resolve() {
            if (ChildList != null) {
                foreach (var item in ChildList) {
                    item.Resolve();
                }
            }
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
            Type = Namespace.ResolveAsType(TypeQName);
            FullName = new FullName(Namespace.Uri, Name.Value);
        }
    }
    public sealed class GlobalElementRefNode : MemberElementNode {
        public GlobalElementRefNode(Node parent) : base(parent) { }
        public QualifiableNameNode GlobalElementQName;
        public GlobalElementNode GlobalElement;
        public override void Resolve() {
            GlobalElement = Namespace.ResolveAsElement(GlobalElementQName);
            FullName = GlobalElement.FullName;
        }
    }
    public sealed class StructuralChildrenNode : MemberChildNode {
        public StructuralChildrenNode(Node parent) : base(parent) { }
        public bool IsSequence;
        public List<MemberChildNode> ChildList;
        public TextSpan CloseTokenTextSpan;
        public override void Resolve() {
            if (ChildList != null) {
                foreach (var item in ChildList) {
                    item.Resolve();
                }
            }
        }
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
