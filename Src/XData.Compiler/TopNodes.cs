using System;
using System.Collections.Generic;
using XData.IO.Text;

namespace XData.Compiler {
    internal abstract class Node {
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
                throw new InvalidOperationException("Cannot get ancestor " + typeof(T).FullName);
            }
            return null;
        }
    }
    internal abstract class CompilationUnitBaseNode : Node {
        protected CompilationUnitBaseNode() : base(null) { }
        public List<UriAliasingNode> UriAliasingList;
    }
    internal sealed class CompilationUnitNode : CompilationUnitBaseNode {
        public List<NamespaceNode> NamespaceList;
    }
    internal sealed class IndicatorCompilationUnitNode : CompilationUnitBaseNode {
        public List<IndicatorNode> IndicatorList;
    }

    internal abstract class NamespaceBaseNode : Node {
        protected NamespaceBaseNode(Node parent) : base(parent) { }
        public UriNode UriNode;
        public string Uri {
            get {
                return UriNode.Value;
            }
        }
    }
    internal sealed class IndicatorNode : NamespaceBaseNode {
        public IndicatorNode(Node parent) : base(parent) { }
        public TextSpan TextSpan;
        public bool IsRef;
        public CSharpNamespaceNameNode CSharpNamespaceName;
    }
    internal sealed class LogicalNamespaceSet : Dictionary<string, LogicalNamespace> { }
    internal sealed class LogicalNamespace : List<NamespaceNode> {
        public string Uri {
            get {
                return this[0].Uri;
            }
        }
        public CSharpNamespaceNameNode CSharpNamespaceName;
        public bool IsCSharpNamespaceRef;
        public NamespaceSymbol NamespaceSymbol;
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
    internal sealed class NamespaceNode : NamespaceBaseNode {
        public NamespaceNode(Node parent) : base(parent) { }
        public List<ImportNode> ImportList;
        public List<NamespaceMemberNode> MemberList;
        //
        public LogicalNamespace LogicalNamespace;
        //
        public void ResolveImports(LogicalNamespaceSet nsDict) {
            if (ImportList != null) {
                for (var i = 0; i < ImportList.Count; ++i) {
                    var import = ImportList[i];
                    if (!nsDict.TryGetValue(import.Uri.Value, out import.LogicalNamespace)) {
                        DiagContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.InvalidImportUri, import.Uri.Value), import.Uri.TextSpan);
                    }
                }
            }
        }
        public void CheckDuplicateMembers(List<NamespaceMemberNode> otherList) {
            if (MemberList != null && otherList != null) {
                foreach (var thisMember in MemberList) {
                    foreach (var otherMember in otherList) {
                        if (thisMember.Name == otherMember.Name) {
                            DiagContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.DuplicateNamespaceMember, otherMember.Name.ToString()),
                                otherMember.Name.TextSpan);
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
        public void CreateSymbols() {
            if (MemberList != null) {
                foreach (var member in MemberList) {
                    member.CreateSymbol();
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
                        DiagContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.InvalidQualifiableNameAlias, alias.ToString()), alias.TextSpan);
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
                                    DiagContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.AmbiguousNameReference, name.ToString()), name.TextSpan);
                                }
                                result = member;
                            }
                        }
                    }
                }
            }
            if (result == null) {
                DiagContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.InvalidNameReference, name.ToString()), name.TextSpan);
            }
            return result;
        }
        public TypeNode ResolveAsType(QualifiableNameNode qName) {
            var result = Resolve(qName) as TypeNode;
            if (result == null) {
                DiagContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.InvalidTypeNameReference, qName.ToString()), qName.TextSpan);
            }
            return result;
        }
        public GlobalElementNode ResolveAsElement(QualifiableNameNode qName) {
            var result = Resolve(qName) as GlobalElementNode;
            if (result == null) {
                DiagContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.InvalidElementNameReference, qName.ToString()), qName.TextSpan);
            }
            return result;
        }
    }

    internal struct UriNode {
        public UriNode(NameNode alias, AtomValueNode stringValue, string value) {
            Alias = alias;
            StringValue = stringValue;
            Value = value;
        }
        public readonly NameNode Alias;
        public readonly AtomValueNode StringValue;
        public readonly string Value;
        public TextSpan TextSpan {
            get {
                if (Alias.IsValid) {
                    return Alias.TextSpan;
                }
                return StringValue.TextSpan;
            }
        }
    }
    internal struct UriAliasingNode {
        public UriAliasingNode(AtomValueNode uri, NameNode alias) {
            Uri = uri;
            Alias = alias;
        }
        public readonly AtomValueNode Uri;
        public readonly NameNode Alias;
    }
    internal struct ImportNode {
        public ImportNode(UriNode uri, NameNode alias) {
            Uri = uri;
            Alias = alias;
            LogicalNamespace = null;
        }
        public readonly UriNode Uri;
        public readonly NameNode Alias;//opt
        public LogicalNamespace LogicalNamespace;
    }

    internal abstract class ObjectNode :Node {
        protected ObjectNode(Node parent) : base(parent) { }
        private NamespaceNode _namespaceAncestor;
        public NamespaceNode NamespaceAncestor {
            get {
                return _namespaceAncestor ?? (_namespaceAncestor = GetAncestor<NamespaceNode>());
            }
        }
    }
    internal abstract class NamespaceMemberNode : ObjectNode {
        protected NamespaceMemberNode(Node parent) : base(parent) { }
        public NameNode Name;
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

        private string _csName;
        public string CSName {
            get {
                return _csName ?? (_csName = Name.Value.EscapeId());
            }
        }
        private FullName? _fullName;
        public FullName FullName {
            get {
                if (_fullName == null) {
                    _fullName = new FullName(NamespaceAncestor.Uri, Name.Value);
                }
                return _fullName.Value;
            }
        }
        public abstract void Resolve();
        //
        protected IGlobalObjectSymbol _objectSymbol;
        private bool _isProcessing;
        public IGlobalObjectSymbol CreateSymbol() {
            if (_objectSymbol == null) {
                if (_isProcessing) {
                    DiagContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.CircularReferenceDetected), Name.TextSpan);
                }
                _isProcessing = true;
                var parent = NamespaceAncestor.LogicalNamespace.NamespaceSymbol;
                var objectSymbol = CreateSymbolCore(parent, CSName, FullName, IsAbstract, IsSealed);
                parent.GlobalObjectList.Add(objectSymbol);
                _objectSymbol = objectSymbol;
                _isProcessing = false;
            }
            return _objectSymbol;
        }
        protected abstract IGlobalObjectSymbol CreateSymbolCore(NamespaceSymbol parent, string csName, FullName fullName, bool isAbstract, bool isSealed);
    }

}
