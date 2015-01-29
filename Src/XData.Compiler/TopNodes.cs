using System;
using System.Collections.Generic;
using XData.IO.Text;

namespace XData.Compiler {
    internal sealed class ProgramNode : Node {
        public ProgramNode() : base(null) {
        }
        public List<CompilationUnitNode> CompilationUnitList;
        public List<CSNSIndicatorCompilationUnitNode> CSNSIndicatorCompilationUnitList;
        //
        //public List<NamespaceNode> NamespaceList;
        //public NamespaceNodeDict NamespaceDict;
        public void Analyze() {
            var nsList = new List<NamespaceNode>();
            if (CompilationUnitList != null) {
                foreach (var cu in CompilationUnitList) {
                    if (cu.NamespaceList != null) {
                        nsList.AddRange(cu.NamespaceList);
                    }
                }
            }
            //NamespaceList = nsList;
            var nsIndicatorList = new List<CSNSIndicatorNode>();
            if (CSNSIndicatorCompilationUnitList != null) {
                foreach (var cu in CSNSIndicatorCompilationUnitList) {
                    if (cu.NamespaceList != null) {
                        nsIndicatorList.AddRange(cu.NamespaceList);
                    }
                }
            }
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
            //NamespaceDict = nsDict;
            if (nsIndicatorList.Count > 0) {
                foreach (var nsIndicator in nsIndicatorList) {
                    LogicalNamespaceNode logicalNS;
                    if (!nsDict.TryGetValue(nsIndicator.Uri, out logicalNS)) {
                        DiagContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.InvalidIndicatorNamespaceName, nsIndicator.Uri),
                            nsIndicator.UriNode.TextSpan);
                    }
                    if (logicalNS.CSNamespaceName == null) {
                        logicalNS.CSNamespaceName = nsIndicator.CSNamespaceName;
                        logicalNS.IsCSNamespaceRef = nsIndicator.IsRef;
                    }
                    else if (logicalNS.IsCSNamespaceRef != nsIndicator.IsRef || logicalNS.CSNamespaceName != nsIndicator.CSNamespaceName) {
                        DiagContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.InconsistentCSharpNamespaceName,
                            logicalNS.IsCSNamespaceRef ? "&" : "=", logicalNS.CSNamespaceName.ToString(), nsIndicator.IsRef ? "&" : "=", nsIndicator.CSNamespaceName.ToString()),
                            nsIndicator.CSNamespaceName.TextSpan);
                    }

                }
                foreach (var logicalNS in nsDict.Values) {
                    if (logicalNS.CSNamespaceName == null) {
                        DiagContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.CSNamespaceNameNotSpecifiedForNamespace, logicalNS.Uri),
                            nsIndicatorList[0].TextSpan);
                    }
                }
            }
            else {
                var idx = 0;
                foreach (var logicalNS in nsDict.Values) {
                    logicalNS.CSNamespaceName = new CSNamespaceNameNode() { "__fake_ns__" + (idx++).ToInvString() };
                    logicalNS.IsCSNamespaceRef = true;
                }
            }
            if (nsList.Count == 0) {
                return;
            }
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
            //
            var programSymbol = new ProgramSymbol(nsIndicatorList.Count > 0);
            foreach (var logicalNS in nsDict.Values) {
                logicalNS.NamespaceSymbol = new NamespaceSymbol(logicalNS.Uri, logicalNS.CSNamespaceName, logicalNS.IsCSNamespaceRef);
                programSymbol.NamespaceList.Add(logicalNS.NamespaceSymbol);
            }
            foreach (var ns in nsList) {
                ns.CreateSymbols();
            }

            //return programSymbol;

        }
    }
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
                throw new InvalidOperationException("Cannot get ancestor: " + typeof(T).FullName);
            }
            return null;
        }
        private ProgramNode _programAncestor;
        public ProgramNode ProgramAncestor {
            get {
                return _programAncestor ?? (_programAncestor = GetAncestor<ProgramNode>());
            }
        }
        private CompilationUnitNode _compilationUnitAncestor;
        public CompilationUnitNode CompilationUnitAncestor {
            get {
                return _compilationUnitAncestor ?? (_compilationUnitAncestor = GetAncestor<CompilationUnitNode>());
            }
        }
        private NamespaceNode _namespaceAncestor;
        public NamespaceNode NamespaceAncestor {
            get {
                return _namespaceAncestor ?? (_namespaceAncestor = GetAncestor<NamespaceNode>());
            }
        }
    }
    internal class CompilationUnitNode : Node {
        public CompilationUnitNode(Node parent) : base(parent) { }
        public List<UriAliasingNode> UriAliasingList;
        public List<NamespaceNode> NamespaceList;
    }
    internal sealed class CSNSIndicatorCompilationUnitNode : CompilationUnitNode {
        public CSNSIndicatorCompilationUnitNode(Node parent) : base(parent) { }
        new public List<CSNSIndicatorNode> NamespaceList;
    }
    internal sealed class NamespaceNodeDict : Dictionary<string, LogicalNamespaceNode> {
    }
    internal sealed class LogicalNamespaceNode : List<NamespaceNode> {
        public string Uri {
            get {
                return this[0].Uri;
            }
        }
        public CSNamespaceNameNode CSNamespaceName;
        public bool IsCSNamespaceRef;
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
    internal class NamespaceNode : Node {
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
    internal sealed class CSNSIndicatorNode : NamespaceNode {
        public CSNSIndicatorNode(Node parent) : base(parent) { }
        public TextSpan TextSpan;
        public bool IsRef;
        public CSNamespaceNameNode CSNamespaceName;
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
        public LogicalNamespaceNode LogicalNamespace;
    }
    internal abstract class NamespaceMemberNode : Node {
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
                var objectSymbol = CreateSymbolCore(parent, CSName, FullName,IsAbstract, IsSealed);
                parent.GlobalObjectList.Add(objectSymbol);
                _objectSymbol = objectSymbol;
                _isProcessing = false;
            }
            return _objectSymbol;
        }
        protected abstract IGlobalObjectSymbol CreateSymbolCore(NamespaceSymbol parent, string csName, FullName fullName, bool isAbstract, bool isSealed);
    }

}
