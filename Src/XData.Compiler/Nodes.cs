using System;
using System.Collections.Generic;
using XData.IO.Text;

namespace XData.Compiler {
    public sealed class ProgramNode : Node {
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
                        ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.InvalidIndicatorNamespaceName, nsIndicator.Uri),
                            nsIndicator.UriNode.TextSpan);
                    }
                    if (logicalNS.CSNamespaceName == null) {
                        logicalNS.CSNamespaceName = nsIndicator.CSNamespaceName;
                        logicalNS.IsCSNamespaceRef = nsIndicator.IsRef;
                    }
                    else if (logicalNS.IsCSNamespaceRef != nsIndicator.IsRef || logicalNS.CSNamespaceName != nsIndicator.CSNamespaceName) {
                        ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.InconsistentCSharpNamespaceName,
                            logicalNS.IsCSNamespaceRef ? "&" : "=", logicalNS.CSNamespaceName.ToString(), nsIndicator.IsRef ? "&" : "=", nsIndicator.CSNamespaceName.ToString()),
                            nsIndicator.CSNamespaceName.TextSpan);
                    }

                }
                foreach (var logicalNS in nsDict.Values) {
                    if (logicalNS.CSNamespaceName == null) {
                        ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.CSNamespaceNameNotSpecifiedForNamespace, logicalNS.Uri),
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
    public class CompilationUnitNode : Node {
        public CompilationUnitNode(Node parent) : base(parent) { }
        public List<UriAliasingNode> UriAliasingList;
        public List<NamespaceNode> NamespaceList;
    }
    public sealed class CSNSIndicatorCompilationUnitNode : CompilationUnitNode {
        public CSNSIndicatorCompilationUnitNode(Node parent) : base(parent) { }
        new public List<CSNSIndicatorNode> NamespaceList;
    }
    public sealed class NamespaceNodeDict : Dictionary<string, LogicalNamespaceNode> {

    }
    public sealed class LogicalNamespaceNode : List<NamespaceNode> {
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
                        ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.InvalidImportUri, import.Uri.Value), import.Uri.TextSpan);
                    }
                }
            }
        }
        public void CheckDuplicateMembers(List<NamespaceMemberNode> otherList) {
            if (MemberList != null && otherList != null) {
                foreach (var thisMember in MemberList) {
                    foreach (var otherMember in otherList) {
                        if (thisMember.Name == otherMember.Name) {
                            ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.DuplicateNamespaceMember, otherMember.Name.ToString()),
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
                        ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.InvalidQualifiableNameAlias, alias.ToString()), alias.TextSpan);
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
                                    ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.AmbiguousNameReference, name.ToString()), name.TextSpan);
                                }
                                result = member;
                            }
                        }
                    }
                }
            }
            if (result == null) {
                ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.InvalidNameReference, name.ToString()), name.TextSpan);
            }
            return result;
        }
        public TypeNode ResolveAsType(QualifiableNameNode qName) {
            var result = Resolve(qName) as TypeNode;
            if (result == null) {
                ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.InvalidTypeNameReference, qName.ToString()), qName.TextSpan);
            }
            return result;
        }
        public GlobalAttributeNode ResolveAsAttribute(QualifiableNameNode qName) {
            var result = Resolve(qName) as GlobalAttributeNode;
            if (result == null) {
                ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.InvalidAttributeNameReference, qName.ToString()), qName.TextSpan);
            }
            return result;
        }
        public GlobalElementNode ResolveAsElement(QualifiableNameNode qName) {
            var result = Resolve(qName) as GlobalElementNode;
            if (result == null) {
                ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.InvalidElementNameReference, qName.ToString()), qName.TextSpan);
            }
            return result;
        }
    }
    public sealed class CSNSIndicatorNode : NamespaceNode {
        public CSNSIndicatorNode(Node parent) : base(parent) { }
        public TextSpan TextSpan;
        public bool IsRef;
        public CSNamespaceNameNode CSNamespaceName;
    }

    public struct UriNode {
        public UriNode(NameNode alias, AtomValueNode stringValue, string value) {
            Alias = alias;
            StringValue = stringValue;
            Value = value;
        }
        public readonly NameNode Alias;
        public readonly AtomValueNode StringValue;
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
        public UriAliasingNode(AtomValueNode uri, NameNode alias) {
            Uri = uri;
            Alias = alias;
        }
        public readonly AtomValueNode Uri;
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
    public abstract class ObjectNode : Node {
        protected ObjectNode(Node parent) : base(parent) { }
        //public abstract TextSpan TextSpan { get; }
        //protected abstract ObjectSymbol CreateSymbolCore(ObjectBaseSymbol parent);
    }
    public abstract class NamespaceMemberNode : ObjectNode {
        protected NamespaceMemberNode(Node parent) : base(parent) {
        }
        public NameNode Name;
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
        protected NamedObjectSymbol _objectSymbol;
        private bool _isProcessing;
        public NamedObjectSymbol CreateSymbol() {
            if (_objectSymbol == null) {
                if (_isProcessing) {
                    ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.CircularReferenceDetected), Name.TextSpan);
                }
                _isProcessing = true;
                _objectSymbol = CreateSymbolCore();
                if (_objectSymbol.FullName.Uri != InfoExtensions.SystemUri) {
                    NamespaceAncestor.LogicalNamespace.NamespaceSymbol.GlobalObjectList.Add(_objectSymbol);
                }
                _isProcessing = false;
            }
            return _objectSymbol;
        }
        protected abstract NamedObjectSymbol CreateSymbolCore();
    }
    public sealed class SystemTypeNode : TypeNode {
        private SystemTypeNode() : base(null) {
            //Name = new NameNode(kind.ToString(), default(TextSpan));
            //_fullName = kind.ToFullName();
            //Kind = kind;
            //_objectSymbol= .TryGetGlobalObject(_fullName.Value);
        }
        //public readonly TypeKind Kind;
        private static readonly Dictionary<string, SystemTypeNode> _dict;
        static SystemTypeNode() {
            _dict = new Dictionary<string, SystemTypeNode>();
            var sysNs = NamespaceSymbol.System;
            for (var kind = InfoExtensions.TypeStart; kind <= InfoExtensions.TypeEnd; ++kind) {
                var name = kind.ToString();
                _dict.Add(name, new SystemTypeNode() { _objectSymbol = sysNs.TryGetGlobalObject(name) });
            }
        }
        public static SystemTypeNode TryGet(string name) {
            SystemTypeNode result;
            _dict.TryGetValue(name, out result);
            return result;
        }
        protected override NamedObjectSymbol CreateSymbolCore() {
            throw new NotImplementedException();
        }
    }
    public class TypeNode : NamespaceMemberNode {
        public TypeNode(Node parent) : base(parent) { }
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
        protected override NamedObjectSymbol CreateSymbolCore() {
            return Body.CreateSymbolCore(NamespaceAncestor.LogicalNamespace.NamespaceSymbol, CSName, FullName, IsAbstract, IsSealed);
        }
    }
    public abstract class TypeBodyNode : Node {
        protected TypeBodyNode(Node parent) : base(parent) { }
        public abstract void Resolve();
        public abstract TypeSymbol CreateSymbolCore(NamespaceSymbol parent, string csName, FullName fullName, bool isAbstract, bool isSealed);
    }
    public sealed class TypeListNode : TypeBodyNode {
        public TypeListNode(Node parent) : base(parent) { }
        public QualifiableNameNode ItemQName;
        public TypeNode ItemType;
        public ValueRestrictionsNode ValueRestrictions;//opt
        public override void Resolve() {
            ItemType = NamespaceAncestor.ResolveAsType(ItemQName);
            if (ValueRestrictions != null && ValueRestrictions.ListItemTypeQName.IsValid) {
                ContextEx.ErrorDiag(new DiagMsgEx(DiagCodeEx.ValueRestrictionNotApplicable), ValueRestrictions.ListItemTypeQName.TextSpan);
            }
        }
        public override TypeSymbol CreateSymbolCore(NamespaceSymbol parent, string csName, FullName fullName, bool isAbstract, bool isSealed) {
            var itemSymbol = ItemType.CreateSymbol() as SimpleTypeSymbol;
            if (itemSymbol == null) {
                ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.SimpleTypeRequired), ItemQName.TextSpan);
            }
            if (ValueRestrictions != null) {
                if (ValueRestrictions.ListItemTypeQName.IsValid) {

                }
            }

            return null;// new ListTypeSymbol(parent, csName, isAbstract, isSealed, fullName, itemSymbol);
        }
    }
    public sealed class ChildrenNode : Node {
        public ChildrenNode(Node parent) : base(parent) { }
        public RootComplexChildrenNode ComplexChildren;
        public QualifiableNameNode SimpleChildQName;
        public TypeNode SimpleChild;
        public void Resolve() {
            if (ComplexChildren != null) {
                ComplexChildren.Resolve();
            }
            else {
                SimpleChild = NamespaceAncestor.ResolveAsType(SimpleChildQName);
            }
        }
    }
    public sealed class TypeDirectnessNode : TypeBodyNode {
        public TypeDirectnessNode(Node parent) : base(parent) { }
        public AttributesNode Attributes;
        public ChildrenNode Children;
        public override void Resolve() {
            if (Attributes != null) {
                Attributes.Resolve();
            }
            if (Children != null) {
                Children.Resolve();
            }
        }
        public override TypeSymbol CreateSymbolCore(NamespaceSymbol parent, string csName, FullName fullName, bool isAbstract, bool isSealed) {
            AttributeSetSymbol attributesSymbol = null;
            if (Attributes != null) {
                attributesSymbol = Attributes.CreateSymbol(null, false);
            }
            ObjectSymbol childrenSymbol = null;
            if (Children != null) {
                if (Children.ComplexChildren != null) {
                    childrenSymbol = Children.ComplexChildren.CreateSymbol(null, false);
                }
                else {
                    childrenSymbol = Children.SimpleChild.CreateSymbol();
                }
            }
            return new ComplexTypeSymbol(parent, csName, isAbstract, isSealed, fullName, null, attributesSymbol, childrenSymbol);
        }
    }
    public abstract class TypeDerivation : TypeBodyNode {
        public TypeDerivation(Node parent) : base(parent) { }
        public QualifiableNameNode BaseTypeQName;
        public AttributesNode Attributes;
        public TypeNode BaseType;
        public override void Resolve() {
            BaseType = NamespaceAncestor.ResolveAsType(BaseTypeQName);
            if (Attributes != null) {
                Attributes.Resolve();
            }
        }
    }
    public sealed class TypeExtension : TypeDerivation {
        public TypeExtension(Node parent) : base(parent) { }
        public ChildrenNode Children;
        public override void Resolve() {
            base.Resolve();
            if (Children != null) {
                Children.Resolve();
            }
        }
        public override TypeSymbol CreateSymbolCore(NamespaceSymbol parent, string csName, FullName fullName, bool isAbstract, bool isSealed) {
            var baseTypeSymbol = BaseType.CreateSymbol() as ComplexTypeSymbol;
            if (baseTypeSymbol == null) {
                ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.ComplexTypeRequired), BaseTypeQName.TextSpan);
            }
            AttributeSetSymbol attributesSymbol = null;
            if (Attributes != null) {
                attributesSymbol = Attributes.CreateSymbol(baseTypeSymbol.Attributes, true);
            }
            ObjectSymbol childrenSymbol = null;
            if (Children != null) {
                if (Children.ComplexChildren != null) {
                    if (baseTypeSymbol.SimpleChild != null) {
                        ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.ThisTypeContainsComplexChildrenButBaseTypeContainsSimpleChild), Children.ComplexChildren.OpenBraceToken);
                    }
                    childrenSymbol = Children.ComplexChildren.CreateSymbol(baseTypeSymbol.ComplexChildren, true);
                }
                else {
                    if (baseTypeSymbol.Children != null) {
                        ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.ExtendingWithSimpleChildRequiresBaseTypeHasNoChildren), Children.SimpleChildQName.TextSpan);
                    }
                    childrenSymbol = Children.SimpleChild.CreateSymbol();
                }
            }
            return new ComplexTypeSymbol(parent, csName, isAbstract, isSealed, fullName, baseTypeSymbol, attributesSymbol, childrenSymbol);
        }
    }
    public sealed class TypeRestriction : TypeDerivation {
        public TypeRestriction(Node parent) : base(parent) { }
        public RootComplexChildrenNode ComplexChildren;
        public ValueRestrictionsNode ValueRestrictions;
        public override void Resolve() {
            base.Resolve();
            if (ComplexChildren != null) {
                ComplexChildren.Resolve();
            }
            else if (ValueRestrictions != null) {
                ValueRestrictions.Resolve();
            }
        }
        private SimpleTypeSymbol CreateSimpleTypeSymbol(NamespaceSymbol parent, string csName, FullName fullName, bool isAbstract, bool isSealed,
            SimpleTypeSymbol baseTypeSymbol, bool inComplexType) {
            if (baseTypeSymbol == null) {

            }

            if (inComplexType) {
                parent.GlobalObjectList.Add(null);
            }
            return null;
        }
        public override TypeSymbol CreateSymbolCore(NamespaceSymbol parent, string csName, FullName fullName, bool isAbstract, bool isSealed) {
            var baseTypeSymbol = BaseType.CreateSymbol();
            var baseSimpleTypeSymbol = baseTypeSymbol as SimpleTypeSymbol;
            if (baseSimpleTypeSymbol != null) {
                if (Attributes != null) {
                    ContextEx.ErrorDiag(new DiagMsgEx(DiagCodeEx.AttributesNotAllowedInSimpleTypeRestriction), Attributes.OpenBracketToken);
                }
                if (ComplexChildren != null) {
                    ContextEx.ErrorDiag(new DiagMsgEx(DiagCodeEx.ComplexChildrenNotAllowedInSimpleTypeRestriction), ComplexChildren.OpenBraceToken);
                }
                ContextEx.ThrowIfHasErrors();
                return CreateSimpleTypeSymbol(parent, csName, fullName, isAbstract, isSealed, baseSimpleTypeSymbol, false);
            }
            else {
                var baseComplexTypeSymbol = (ComplexTypeSymbol)baseTypeSymbol;
                AttributeSetSymbol attributesSymbol = null;
                if (Attributes != null) {
                    attributesSymbol = Attributes.CreateSymbol(baseComplexTypeSymbol.Attributes, false);
                }
                ObjectSymbol childrenSymbol = null;
                if (ComplexChildren != null) {
                    if (baseComplexTypeSymbol.SimpleChild != null) {
                        ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.ThisTypeContainsComplexChildrenButBaseTypeContainsSimpleChild), ComplexChildren.OpenBraceToken);
                    }
                    childrenSymbol = ComplexChildren.CreateSymbol(baseComplexTypeSymbol.ComplexChildren, false);
                }
                else if (ValueRestrictions != null) {
                    if (baseComplexTypeSymbol.ComplexChildren != null) {
                        ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.ThisTypeContainsSimpleChildButBaseTypeContainsComplexChildren), ValueRestrictions.OpenBraceToken);
                    }
                    childrenSymbol = CreateSimpleTypeSymbol(parent, csName, fullName, isAbstract, isSealed, baseComplexTypeSymbol.SimpleChild, true);
                }
                return new ComplexTypeSymbol(parent, csName, isAbstract, isSealed, fullName, baseComplexTypeSymbol, attributesSymbol, childrenSymbol);
            }
        }

    }

    public sealed class ValueRestrictionsNode : Node {
        public ValueRestrictionsNode(Node parent) : base(parent) { }
        public IntegerRangeNode<ulong> Lengths;
        public IntegerRangeNode<byte> Digits;
        public ValueRangeNode Values;
        public List<SimpleValueNode> Enums;
        public AtomValueNode Pattern;
        public QualifiableNameNode ListItemTypeQName;
        public TypeNode ListItemType;
        public TextSpan OpenBraceToken, CloseBraceToken;
        public void Resolve() {
            if (ListItemTypeQName.IsValid) {
                ListItemType = NamespaceAncestor.ResolveAsType(ListItemTypeQName);
            }
        }
        public void CheckApplicabilities(TypeKind kind) {
            if (Lengths.IsValid && !Contains(_lengthsTypeKinds, kind)) {
                ContextEx.ErrorDiag(new DiagMsgEx(DiagCodeEx.ValueRestrictionNotApplicable), Lengths.DotDotToken);
            }
            if (Digits.IsValid) {
                if (Digits.MinValue != null && !Contains(_totalDigitsTypeKinds, kind)) {
                    ContextEx.ErrorDiag(new DiagMsgEx(DiagCodeEx.ValueRestrictionNotApplicable), Digits.MinValueNode.TextSpan);
                }
                if (Digits.MaxValue != null && kind != TypeKind.Decimal) {
                    ContextEx.ErrorDiag(new DiagMsgEx(DiagCodeEx.ValueRestrictionNotApplicable), Digits.MaxValueNode.TextSpan);
                }
            }
            if (Values.IsValid && !Contains(_valuesTypeKinds, kind)) {
                ContextEx.ErrorDiag(new DiagMsgEx(DiagCodeEx.ValueRestrictionNotApplicable), Values.DotDotToken);
            }
            if (Enums != null && (!kind.IsConcreteAtomType())) {
                ContextEx.ErrorDiag(new DiagMsgEx(DiagCodeEx.ValueRestrictionNotApplicable), Enums[0].TextSpan);
            }
            if (Pattern.IsValid && !kind.IsConcreteAtomType()) {
                ContextEx.ErrorDiag(new DiagMsgEx(DiagCodeEx.ValueRestrictionNotApplicable), Pattern.TextSpan);
            }
            if (ListItemTypeQName.IsValid && kind != TypeKind.ListType) {
                ContextEx.ErrorDiag(new DiagMsgEx(DiagCodeEx.ValueRestrictionNotApplicable), ListItemTypeQName.TextSpan);
            }
            ContextEx.ThrowIfHasErrors();
        }
        private static readonly TypeKind[] _lengthsTypeKinds = new TypeKind[] { TypeKind.ListType, TypeKind.String, TypeKind.IgnoreCaseString, TypeKind.Binary };
        private static readonly TypeKind[] _totalDigitsTypeKinds = new TypeKind[] { TypeKind.Decimal, TypeKind.Int64, TypeKind.Int32, TypeKind.Int16, TypeKind.SByte, TypeKind.UInt64, TypeKind.UInt32, TypeKind.UInt16, TypeKind.Byte };
        //private static readonly TypeKind[] _fractionDigitsTypeKinds = new TypeKind[] { TypeKind.Decimal };
        private static readonly TypeKind[] _valuesTypeKinds = new TypeKind[] { TypeKind.String, TypeKind.IgnoreCaseString, TypeKind.Decimal, TypeKind.Int64, TypeKind.Int32, TypeKind.Int16, TypeKind.SByte, TypeKind.UInt64, TypeKind.UInt32, TypeKind.UInt16, TypeKind.Byte, TypeKind.Double, TypeKind.Single, TypeKind.Guid, TypeKind.TimeSpan, TypeKind.DateTimeOffset };
        private static bool Contains(TypeKind[] array, TypeKind kind) {
            foreach (var item in array) {
                if (item == kind) {
                    return true;
                }
            }
            return false;
        }
    }

    public struct IntegerRangeNode<T> where T : struct {
        public IntegerRangeNode(AtomValueNode minValueNode, T? minValue, AtomValueNode maxValueNode, T? maxValue, TextSpan dotDotToken) {
            MinValueNode = minValueNode;
            MinValue = minValue;
            MaxValueNode = maxValueNode;
            MaxValue = maxValue;
            DotDotToken = dotDotToken;
        }
        public readonly AtomValueNode MinValueNode;
        public readonly T? MinValue;
        public readonly AtomValueNode MaxValueNode;
        public readonly T? MaxValue;
        public readonly TextSpan DotDotToken;
        public bool IsValid {
            get {
                return DotDotToken.IsValid;
            }
        }
    }
    public struct ValueRangeNode {
        public ValueRangeNode(ValueBoundaryNode? minValue, ValueBoundaryNode? maxValue, TextSpan dotDotToken) {
            MinValue = minValue;
            MaxValue = maxValue;
            DotDotToken = dotDotToken;
        }
        public readonly ValueBoundaryNode? MinValue;
        public readonly ValueBoundaryNode? MaxValue;
        public readonly TextSpan DotDotToken;
        public bool IsValid {
            get {
                return DotDotToken.IsValid;
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
        public AttributeSetSymbol CreateSymbol(AttributeSetSymbol baseAttributeSet, bool isExtension) {

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
        public ChildSetSymbol CreateSymbol(ChildSetSymbol baseChildSet, bool isExtension) {

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
