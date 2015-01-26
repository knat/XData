using System;
using System.Collections.Generic;
using XData.IO.Text;

namespace XData.Compiler {
    public sealed class GlobalElementNode : NamespaceMemberNode {
        public GlobalElementNode(Node parent) : base(parent) { }
        public TextSpan Nullable;
        public QualifiableNameNode SubstitutedGlobalElementQName;
        public GlobalElementNode SubstitutedGlobalElement;
        private List<GlobalElementNode> _directSubstitutorList;
        public List<GlobalElementNode> DirectSubstitutorList {
            get {
                return _directSubstitutorList ?? (_directSubstitutorList = new List<GlobalElementNode>());
            }
        }
        public QualifiableNameNode TypeQName;
        public TypeNode Type;
        public bool IsNullable {
            get {
                return Nullable.IsValid;
            }
        }
        public bool HasIntersection(FullName fullName) {
            if (FullName == fullName && !IsAbstract) {
                return true;
            }
            if (_directSubstitutorList != null) {
                foreach (var i in _directSubstitutorList) {
                    if (i.HasIntersection(fullName)) {
                        return true;
                    }
                }
            }
            return false;
        }
        public override void Resolve() {
            if (SubstitutedGlobalElementQName.IsValid) {
                SubstitutedGlobalElement = NamespaceAncestor.ResolveAsElement(SubstitutedGlobalElementQName);
                for (var i = SubstitutedGlobalElement; i != null; i = i.SubstitutedGlobalElement) {
                    if (i == this) {
                        ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.CircularReferenceDetected), SubstitutedGlobalElementQName.TextSpan);
                    }
                }
                SubstitutedGlobalElement.DirectSubstitutorList.Add(this);
            }
            Type = NamespaceAncestor.ResolveAsType(TypeQName);
        }
        protected override IGlobalObjectSymbol CreateSymbolCore(NamespaceSymbol parent, string csName, FullName fullName, bool isAbstract, bool isSealed) {
            ElementSymbol substitutedElementSymbol = null;
            if (SubstitutedGlobalElement != null) {
                substitutedElementSymbol = (ElementSymbol)SubstitutedGlobalElement.CreateSymbol();
                if (substitutedElementSymbol.IsSealed) {
                    ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.SubstitutedElementIsSealed), SubstitutedGlobalElementQName.TextSpan);
                }
            }
            var typeSymbol = (TypeSymbol)Type.CreateSymbol();
            if (substitutedElementSymbol != null) {
                if (IsNullable && !substitutedElementSymbol.IsNullable) {
                    ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.ElementIsNullableButSubstitutedIsNotNullable),
                        Nullable);
                }
                if (!typeSymbol.IsEqualToOrDeriveFrom(substitutedElementSymbol.Type)) {
                    ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.TypeNotEqualToOrDeriveFromSubstituted,
                        typeSymbol.FullName.ToString(), substitutedElementSymbol.Type.FullName.ToString()),
                        TypeQName.TextSpan);
                }
            }
            return new ElementSymbol(parent, csName, isAbstract, isSealed, ChildKind.GlobalElement, fullName.ToString(), null, false, -1, null, fullName,
                IsNullable, typeSymbol, null, substitutedElementSymbol) { GlobalElementNode = this };
        }
    }
    public sealed class ComplexChildrenNode : Node {
        public ComplexChildrenNode(Node parent) : base(parent) { }
        public List<MemberChildNode> ChildList;
        public TextSpan OpenBraceToken, CloseBraceToken;
        public void Resolve() {
            if (ChildList != null) {
                foreach (var child in ChildList) {
                    child.Resolve();
                }
            }
        }
        public ChildSetSymbol CreateSymbol(ComplexTypeSymbol parent, ChildSetSymbol baseChildSetSymbol, bool isExtension) {
            var baseChildSymbolList = baseChildSetSymbol != null ? baseChildSetSymbol.ChildList : null;
            var nextChildOrder = baseChildSetSymbol != null ? baseChildSetSymbol.NextChildOrder : 0;
            var displayNameBase = parent.FullName.ToString() + ".{}";
            var childSetSymbol = new ChildSetSymbol(parent, "CLS_Children", ChildKind.Sequence, displayNameBase, null, false, -1, null,
                true, baseChildSetSymbol);
            if (baseChildSymbolList != null) {
                childSetSymbol.ChildList.AddRange(baseChildSymbolList);
            }
            if (isExtension) {
                if (ChildList != null) {
                    foreach (var child in ChildList) {
                        var memberName = child.MemberName;
                        if (baseChildSymbolList != null) {
                            foreach (var baseChildSymbol in baseChildSymbolList) {
                                if (baseChildSymbol.MemberName == memberName) {
                                    ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.DuplicateMemberName, memberName),
                                        child.MemberNameNode.TextSpan);
                                }
                            }
                        }
                        childSetSymbol.ChildList.Add(child.CreateSymbol(childSetSymbol, null, nextChildOrder++, displayNameBase));
                    }
                }
                childSetSymbol.NextChildOrder = nextChildOrder;
            }
            else {//restriction
                CreateRestrictionSymbols(childSetSymbol, ChildList, displayNameBase);
            }
            return childSetSymbol;
        }
        public static void CreateRestrictionSymbols(ChildSetSymbol childSetSymbol, List<MemberChildNode> childList, string displayNameBase) {
            if (childList != null) {
                var childSymbolList = childSetSymbol.ChildList;
                foreach (var child in childList) {
                    ChildSymbol restrictedChildSymbol = null;
                    int idx;
                    var memberName = child.MemberName;
                    for (idx = 0; idx < childSymbolList.Count; ++idx) {
                        if (childSymbolList[idx].MemberName == memberName) {
                            restrictedChildSymbol = childSymbolList[idx];
                            break;
                        }
                    }
                    if (restrictedChildSymbol == null) {
                        ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.CannotFindRestrictedChild, memberName),
                            child.MemberNameNode.TextSpan);
                    }
                    var isDelete = child.MaxOccurrence == 0;
                    if (isDelete) {
                        if (!restrictedChildSymbol.IsOptional) {
                            ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.CannotDeleteChildBecauseItIsNotOptional, memberName),
                                child.MemberNameNode.TextSpan);
                        }
                    }
                    childSymbolList.RemoveAt(idx);
                    if (!isDelete) {
                        childSymbolList.Insert(idx, child.CreateSymbol(childSetSymbol, restrictedChildSymbol, restrictedChildSymbol.Order, displayNameBase));
                    }
                }
            }
        }
    }
    public abstract class MemberChildNode : Node {
        protected MemberChildNode(Node parent) : base(parent) { }
        public ChildKind Kind;
        public NameNode MemberNameNode;
        public string MemberName {
            get {
                return MemberNameNode.Value;
            }
        }
        public OccurrenceNode Occurrence;
        public ulong MinOccurrence {
            get {
                return Occurrence.IsValid ? Occurrence.MinValue : 1;
            }
        }
        public ulong MaxOccurrence {
            get {
                return Occurrence.IsValid ? Occurrence.MaxValue : 1;
            }
        }
        public abstract void Resolve();
        public ChildSymbol CreateSymbol(ChildSetSymbol parent, ChildSymbol restrictedChildSymbol, int order, string displayNameBase) {
            var minOccurrence = MinOccurrence;
            var maxOccurrence = MaxOccurrence;
            var isList = maxOccurrence > 1;
            ChildSymbol restrictedItemSymbol = restrictedChildSymbol;
            if (restrictedChildSymbol == null) {
                if (maxOccurrence == 0) {
                    ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.MaxOccurrenceCannotBeZeroInExtension), Occurrence.Token);
                }
            }
            else {
                var restrictedKind = restrictedChildSymbol.Kind;
                if (restrictedKind != Kind) {
                    ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.ChildKindNotEqualToRestricted, Kind.ToString(),
                        restrictedKind.ToString()), MemberNameNode.TextSpan);
                }
                ulong restrictedMinOccurrence = restrictedChildSymbol.IsOptional ? (ulong)0 : 1;
                if (restrictedKind == ChildKind.List) {
                    isList = true;
                    var restrictedListSymbol = (ChildListSymbol)restrictedChildSymbol;
                    restrictedItemSymbol = restrictedListSymbol.Item;
                    restrictedKind = restrictedItemSymbol.Kind;
                    restrictedMinOccurrence = restrictedListSymbol.MinOccurrence;
                    if (maxOccurrence > restrictedListSymbol.MaxOccurrence) {
                        ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.MaxOccurrenceNotEqualToOrLessThanRestricted,
                            maxOccurrence.ToInvString(), restrictedListSymbol.MaxOccurrence.ToInvString()),
                            Occurrence.Token.IsValid ? Occurrence.Token : MemberNameNode.TextSpan);
                    }
                }
                if (minOccurrence < restrictedMinOccurrence) {
                    ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.MinOccurrenceNotEqualToOrGreaterThanRestricted,
                        minOccurrence.ToInvString(), restrictedMinOccurrence.ToInvString()),
                        Occurrence.Token.IsValid ? Occurrence.Token : MemberNameNode.TextSpan);
                }
            }
            var memberName = MemberName;
            var ipOptional = minOccurrence == 0;
            var displayName = displayNameBase + "." + memberName;
            var childSymbol = CreateSymbolCore(parent, restrictedItemSymbol, ipOptional, order,
                displayName, (isList ? "CLSITEM_" : "CLS_") + memberName);
            if (!isList) return childSymbol;
            return new ChildListSymbol(parent, "CLS_" + memberName, displayName, memberName, ipOptional,
                order, (ChildListSymbol)restrictedChildSymbol, minOccurrence, maxOccurrence, childSymbol);
        }
        protected abstract ChildSymbol CreateSymbolCore(ChildSetSymbol parent, ChildSymbol restrictedChildSymbol,
            bool isOptional, int order, string displayName, string csName);

    }
    public abstract class MemberElementNode : MemberChildNode {
        protected MemberElementNode(Node parent) : base(parent) { }
    }
    public sealed class LocalElementNode : MemberElementNode {
        public LocalElementNode(Node parent) : base(parent) {
            Kind = ChildKind.LocalElement;
        }
        public NameNode NameNode;
        public TextSpan Nullable;
        public QualifiableNameNode TypeQName;
        public TypeNode Type;
        public string Name {
            get {
                return NameNode.Value;
            }
        }
        public bool IsNullable {
            get {
                return Nullable.IsValid;
            }
        }
        public override void Resolve() {
            Type = NamespaceAncestor.ResolveAsType(TypeQName);
        }
        protected override ChildSymbol CreateSymbolCore(ChildSetSymbol parent, ChildSymbol restrictedChildSymbol,
            bool isOptional, int order, string displayName, string csName) {
            var typeSymbol = (TypeSymbol)Type.CreateSymbol();
            var restrictedElementSymbol = (ElementSymbol)restrictedChildSymbol;
            if (restrictedElementSymbol != null) {
                if (Name != restrictedElementSymbol.FullName.Name) {
                    ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.ElementNameNotEqualToRestricted,
                        Name, restrictedElementSymbol.FullName.Name), NameNode.TextSpan);
                }
                if (IsNullable && !restrictedElementSymbol.IsNullable) {
                    ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.ElementIsNullableButRestrictedIsNotNullable), Nullable);
                }
                if (!typeSymbol.IsEqualToOrDeriveFrom(restrictedElementSymbol.Type)) {
                    ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.TypeNotEqualToOrDeriveFromRestricted,
                        typeSymbol.FullName.ToString(), restrictedElementSymbol.Type.FullName.ToString()),
                        TypeQName.TextSpan);
                }
            }
            return new ElementSymbol(parent, csName, false, false, ChildKind.LocalElement, displayName, MemberName,
                isOptional, order, restrictedElementSymbol, new FullName(null, Name), IsNullable, typeSymbol, null, null);
        }
    }
    public sealed class GlobalElementRefNode : MemberElementNode {
        public GlobalElementRefNode(Node parent) : base(parent) {
            Kind = ChildKind.GlobalElementReference;
        }
        public QualifiableNameNode GlobalElementQName;
        public GlobalElementNode GlobalElement;
        public override void Resolve() {
            GlobalElement = NamespaceAncestor.ResolveAsElement(GlobalElementQName);
        }
        protected override ChildSymbol CreateSymbolCore(ChildSetSymbol parent, ChildSymbol restrictedChildSymbol,
            bool isOptional, int order, string displayName, string csName) {
            var globalElementSymbol = (ElementSymbol)GlobalElement.CreateSymbol();
            var restrictedElementSymbol = (ElementSymbol)restrictedChildSymbol;
            if (restrictedElementSymbol != null) {
                if (!globalElementSymbol.IsEqualToOrSubstitute(restrictedElementSymbol.ReferencedElement)) {
                    ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.ElementNotEqualToOrSubstituteRestricted,
                        globalElementSymbol.FullName.ToString(), restrictedElementSymbol.ReferencedElement.FullName.ToString()),
                        GlobalElementQName.TextSpan);
                }
            }
            return new ElementSymbol(parent, csName, false, false, ChildKind.GlobalElementReference, displayName, MemberName,
                isOptional, order, restrictedElementSymbol, GlobalElement.FullName, GlobalElement.IsNullable,
                globalElementSymbol.Type, globalElementSymbol, null);
        }
    }
    public sealed class MemberComplexChildrenNode : MemberChildNode {
        public MemberComplexChildrenNode(Node parent) : base(parent) { }
        public List<MemberChildNode> ChildList;
        public TextSpan OpenBraceToken, CloseBraceToken;
        public bool IsSequence {
            get {
                return Kind == ChildKind.Sequence;
            }
        }
        public bool IsChoice {
            get {
                return Kind == ChildKind.Choice;
            }
        }
        public override void Resolve() {
            if (ChildList != null) {
                foreach (var child in ChildList) {
                    child.Resolve();
                }
            }
        }
        protected override ChildSymbol CreateSymbolCore(ChildSetSymbol parent, ChildSymbol restrictedChildSymbol,
            bool isOptional, int order, string displayName, string csName) {
            var restrictedChildSetSymbol = (ChildSetSymbol)restrictedChildSymbol;
            var childSetSymbol = new ChildSetSymbol(parent, csName, Kind, displayName, MemberName, isOptional, order, restrictedChildSetSymbol,
                false, null);
            if (restrictedChildSetSymbol == null) {
                if (ChildList != null) {
                    for (var i = 0; i < ChildList.Count; ++i) {
                        childSetSymbol.ChildList.Add(ChildList[i].CreateSymbol(childSetSymbol, null, i, displayName));
                    }
                }
            }
            else {
                childSetSymbol.ChildList.AddRange(restrictedChildSetSymbol.ChildList);
                ComplexChildrenNode.CreateRestrictionSymbols(childSetSymbol, ChildList, displayName);
            }
            return childSetSymbol;
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
