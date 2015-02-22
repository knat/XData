using System.Linq;
using System.Collections.Generic;
using XData.IO.Text;

namespace XData.Compiler {
    internal sealed class GlobalElementNode : NamespaceMemberNode {
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
        public IEnumerable<FullName> DirectSubstitutorFullNames {
            get {
                if (_directSubstitutorList != null) {
                    return _directSubstitutorList.Select(i => i.FullName);
                }
                return Enumerable.Empty<FullName>();
            }
        }
        public QualifiableNameNode TypeQName;
        public TypeNode Type;
        public bool IsNullable {
            get {
                return Nullable.IsValid;
            }
        }
        //public bool HasIntersection(FullName fullName) {
        //    if (!IsAbstract && FullName == fullName) {
        //        return true;
        //    }
        //    if (_directSubstitutorList != null) {
        //        foreach (var i in _directSubstitutorList) {
        //            if (i.HasIntersection(fullName)) {
        //                return true;
        //            }
        //        }
        //    }
        //    return false;
        //}
        //public void GetNonAbstractFullNames(ref List<FullName> list) {
        //    if (!IsAbstract) {
        //        Extensions.CreateAndAdd(ref list, FullName);
        //    }
        //    if (_directSubstitutorList != null) {
        //        foreach (var i in _directSubstitutorList) {
        //            i.GetNonAbstractFullNames(ref list);
        //        }
        //    }
        //}
        public bool AddNonAbstractFullNames(List<FullName> fullNameList, out FullName dupFullName) {
            if (!IsAbstract) {
                if (fullNameList.Contains(FullName)) {
                    dupFullName = FullName;
                    return false;
                }
                fullNameList.Add(FullName);
            }
            if (_directSubstitutorList != null) {
                foreach (var i in _directSubstitutorList) {
                    if (!i.AddNonAbstractFullNames(fullNameList, out dupFullName)) {
                        return false;
                    }
                }
            }
            dupFullName = default(FullName);
            return true;
        }
        public override void Resolve() {
            if (SubstitutedGlobalElementQName.IsValid) {
                SubstitutedGlobalElement = NamespaceAncestor.ResolveAsElement(SubstitutedGlobalElementQName);
                for (var i = SubstitutedGlobalElement; i != null; i = i.SubstitutedGlobalElement) {
                    if (i == this) {
                        DiagContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.CircularReferenceNotAllowed),
                            SubstitutedGlobalElementQName.TextSpan);
                    }
                }
                SubstitutedGlobalElement.DirectSubstitutorList.Add(this);
            }
            Type = NamespaceAncestor.ResolveAsType(TypeQName);
        }
        protected override IGlobalObjectSymbol CreateSymbolCore(NamespaceSymbol parent, string csName, FullName fullName,
            bool isAbstract, bool isSealed) {
            ElementSymbol substitutedElementSymbol = null;
            if (SubstitutedGlobalElement != null) {
                substitutedElementSymbol = (ElementSymbol)SubstitutedGlobalElement.CreateSymbol();
                if (substitutedElementSymbol.IsSealed) {
                    DiagContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.SubstitutedElementSealed, substitutedElementSymbol.DisplayName),
                        SubstitutedGlobalElementQName.TextSpan);
                }
            }
            var typeSymbol = (TypeSymbol)Type.CreateSymbol();
            if (substitutedElementSymbol != null) {
                if (IsNullable && !substitutedElementSymbol.IsNullable) {
                    DiagContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.CannotChangeNonNullableToNullable),
                        Nullable);
                }
                if (!typeSymbol.EqualToOrDeriveFrom(substitutedElementSymbol.Type)) {
                    DiagContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.TypeNotEqualToOrDeriveFromSubstituted,
                        typeSymbol.DisplayName, substitutedElementSymbol.Type.DisplayName),
                        TypeQName.TextSpan);
                }
            }
            return new ElementSymbol(parent, csName, isAbstract, isSealed, ChildKind.GlobalElement, fullName.ToString(),
                null, 1, 1, false, -1, null, fullName, IsNullable, typeSymbol, null, substitutedElementSymbol) {
                    GlobalElementNode = this
                };
        }
    }
    internal sealed class ComplexChildrenNode : Node {
        public ComplexChildrenNode(Node parent) : base(parent) { }
        public ChildKind Kind;
        public List<MemberChildNode> ChildList;
        public TextSpan OpenBraceTextSpan, CloseBraceTextSpan;
        public bool IsSet {
            get {
                return Kind == ChildKind.Set;
            }
        }
        public bool IsSequence {
            get {
                return Kind == ChildKind.Sequence;
            }
        }
        public void Resolve() {
            if (ChildList != null) {
                foreach (var child in ChildList) {
                    child.Resolve();
                }
            }
        }
        public ChildStructSymbol CreateSymbol(ComplexTypeSymbol parent, ChildStructSymbol baseChildStructSymbol, bool isExtension) {
            if (baseChildStructSymbol != null && baseChildStructSymbol.Kind != Kind) {
                DiagContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.ComplexChildrenKindNotEqualToBase, Kind.ToString(),
                    baseChildStructSymbol.Kind.ToString()), OpenBraceTextSpan);
            }
            var isSet = IsSet;
            var displayName = parent.DisplayName + (isSet ? ".{}" : ".#{}");
            var childStructSymbol = new ChildStructSymbol(parent, "CLS_Children", Kind, displayName, null, 1, 1, false, -1, null,
                true, baseChildStructSymbol);
            if (isExtension) {
                if (ChildList != null) {
                    foreach (var child in ChildList) {
                        var memberName = child.MemberName;
                        if (childStructSymbol.MemberNameList.Contains(memberName)) {
                            DiagContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.DuplicateMemberName, memberName),
                                child.MemberNameNode.TextSpan);
                        }
                        childStructSymbol.MemberNameList.Add(memberName);
                        var childSymbol = child.CreateSymbol(childStructSymbol, null, childStructSymbol.NextChildOrder++, displayName);
                        if (isSet) {
                            FullName dupFullName;
                            if (!((ElementSymbol)childSymbol).AddFullNames(childStructSymbol.FullNameList, out dupFullName)) {
                                DiagContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.DuplicateElementFullName, dupFullName.ToString()),
                                    child.MemberNameNode.TextSpan);
                            }
                        }
                        childStructSymbol.ChildList.Add(childSymbol);
                    }
                }
            }
            else {//restriction
                CreateRestrictionSymbols(childStructSymbol, ChildList, displayName);
            }
            return childStructSymbol;
        }
        public static void CreateRestrictionSymbols(ChildStructSymbol childStructSymbol, List<MemberChildNode> childList, string parentDisplayName) {
            if (childList != null) {
                var childSymbolList = childStructSymbol.ChildList;
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
                        DiagContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.CannotFindRestrictedMember, memberName),
                            child.MemberNameNode.TextSpan);
                    }
                    var isDelete = child.IsDelete;
                    if (isDelete) {
                        if (!restrictedChildSymbol.IsOptional && !restrictedChildSymbol.IsChoiceMember) {
                            DiagContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.CannotDeleteRequiredMember, memberName),
                                child.Occurrence.TextSpan);
                        }
                    }
                    childSymbolList.RemoveAt(idx);
                    if (!isDelete) {
                        childSymbolList.Insert(idx, child.CreateSymbol(childStructSymbol, restrictedChildSymbol,
                            restrictedChildSymbol.Order, parentDisplayName));
                    }
                }
            }
        }
    }
    internal abstract class MemberChildNode : ObjectNode {
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
        public bool IsDelete {
            get {
                return Occurrence.IsValid && Occurrence.MaxValue == 0;
            }
        }
        public abstract void Resolve();
        public ChildSymbol CreateSymbol(ChildStructSymbol parent, ChildSymbol restrictedChildSymbol, int order, string parentDisplayName) {
            var minOccurrence = MinOccurrence;
            var maxOccurrence = MaxOccurrence;
            var isList = maxOccurrence > 1;
            ChildSymbol restrictedItemSymbol = restrictedChildSymbol;
            var restrictedListSymbol = restrictedChildSymbol as ChildListSymbol;
            if (restrictedChildSymbol == null) {
                if (IsDelete) {
                    DiagContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.DeletionNotAllowedInExtension),
                        Occurrence.TextSpan);
                }
            }
            else {
                if (restrictedListSymbol != null) {
                    isList = true;
                    restrictedItemSymbol = restrictedListSymbol.Item;
                }
                if (Kind != restrictedItemSymbol.Kind) {
                    DiagContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.MemberKindNotEqualToRestricted, Kind.ToString(),
                        restrictedItemSymbol.Kind.ToString()), MemberNameNode.TextSpan);
                }
                if (minOccurrence < restrictedChildSymbol.MinOccurrence) {
                    DiagContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.MinOccurrenceNotEqualToOrGreaterThanRestricted,
                        minOccurrence.ToInvString(), restrictedChildSymbol.MinOccurrence.ToInvString()),
                        Occurrence.IsValid ? Occurrence.TextSpan : MemberNameNode.TextSpan);
                }
                if (maxOccurrence > restrictedChildSymbol.MaxOccurrence) {
                    DiagContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.MaxOccurrenceNotEqualToOrLessThanRestricted,
                        maxOccurrence.ToInvString(), restrictedChildSymbol.MaxOccurrence.ToInvString()),
                        Occurrence.IsValid ? Occurrence.TextSpan : MemberNameNode.TextSpan);
                }
            }
            var memberName = MemberName;
            var displayName = parentDisplayName + "." + memberName;
            var childSymbol = CreateSymbolCore(parent, restrictedItemSymbol, isList, order, displayName,
                (isList ? "CLSITEM_" : "CLS_") + memberName);
            if (!isList) {
                return childSymbol;
            }
            return new ChildListSymbol(parent, "CLS_" + memberName, displayName, memberName, minOccurrence, maxOccurrence,
                order, restrictedListSymbol, childSymbol);
        }
        protected abstract ChildSymbol CreateSymbolCore(ChildStructSymbol parent, ChildSymbol restrictedChildSymbol, bool isListItem,
            int order, string displayName, string csName);

    }
    internal abstract class MemberElementNode : MemberChildNode {
        protected MemberElementNode(Node parent) : base(parent) { }
    }
    internal sealed class LocalElementNode : MemberElementNode {
        public LocalElementNode(Node parent)
            : base(parent) {
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
        protected override ChildSymbol CreateSymbolCore(ChildStructSymbol parent, ChildSymbol restrictedChildSymbol, bool isListItem,
            int order, string displayName, string csName) {
            var restrictedElementSymbol = (ElementSymbol)restrictedChildSymbol;
            var name = Name;
            var fullName = new FullName(null, name);
            if (restrictedElementSymbol == null) {
                //if (parent.HasIntersection(fullName)) {
                //    DiagContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.AmbiguousElementFullName, fullName.ToString()),
                //        NameNode.TextSpan);
                //}
            }
            else {
                if (name != restrictedElementSymbol.FullName.Name) {
                    DiagContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.ElementNameNotEqualToRestricted,
                        name, restrictedElementSymbol.FullName.Name), NameNode.TextSpan);
                }
                if (IsNullable && !restrictedElementSymbol.IsNullable) {
                    DiagContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.CannotChangeNonNullableToNullable),
                        Nullable);
                }
            }
            var typeSymbol = (TypeSymbol)Type.CreateSymbol();
            if (restrictedElementSymbol != null) {
                if (!typeSymbol.EqualToOrDeriveFrom(restrictedElementSymbol.Type)) {
                    DiagContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.TypeNotEqualToOrDeriveFromRestricted,
                        typeSymbol.DisplayName, restrictedElementSymbol.Type.DisplayName),
                        TypeQName.TextSpan);
                }
            }
            return new ElementSymbol(parent, csName, false, false, ChildKind.LocalElement, displayName, MemberName,
                MinOccurrence, MaxOccurrence, isListItem, order, restrictedElementSymbol, fullName, IsNullable, typeSymbol, null, null);
        }
    }
    internal sealed class GlobalElementRefNode : MemberElementNode {
        public GlobalElementRefNode(Node parent)
            : base(parent) {
            Kind = ChildKind.GlobalElementRef;
        }
        public QualifiableNameNode GlobalElementQName;
        public GlobalElementNode GlobalElement;
        public override void Resolve() {
            GlobalElement = NamespaceAncestor.ResolveAsElement(GlobalElementQName);
        }
        protected override ChildSymbol CreateSymbolCore(ChildStructSymbol parent, ChildSymbol restrictedChildSymbol, bool isListItem,
            int order, string displayName, string csName) {
            var restrictedElementSymbol = (ElementSymbol)restrictedChildSymbol;
            var globalElementSymbol = (ElementSymbol)GlobalElement.CreateSymbol();
            if (restrictedElementSymbol == null) {
                //List<FullName> fullNameList = null;
                //GlobalElement.GetNonAbstractFullNames(ref fullNameList);
                //if (fullNameList != null) {
                //    foreach (var fullName in fullNameList) {
                //        if (parent.HasIntersection(fullName)) {
                //            DiagContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.AmbiguousElementFullName,
                //                fullName.ToString()), GlobalElementQName.TextSpan);
                //        }
                //    }
                //}
            }
            else {
                if (!globalElementSymbol.EqualToOrSubstituteFor(restrictedElementSymbol.ReferencedElement)) {
                    DiagContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.ElementNotEqualToOrSubstituteForRestricted,
                        globalElementSymbol.DisplayName, restrictedElementSymbol.ReferencedElement.DisplayName),
                        GlobalElementQName.TextSpan);
                }
            }
            return new ElementSymbol(parent, csName, false, false, ChildKind.GlobalElementRef, displayName, MemberName,
                MinOccurrence, MaxOccurrence, isListItem, order, restrictedElementSymbol, GlobalElement.FullName,
                GlobalElement.IsNullable, globalElementSymbol.Type, globalElementSymbol, null);
        }
    }
    internal sealed class MemberComplexChildrenNode : MemberChildNode {
        public MemberComplexChildrenNode(Node parent) : base(parent) { }
        public List<MemberChildNode> ChildList;
        public TextSpan OpenBraceTextSpan, CloseBraceTextSpan;
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
        protected override ChildSymbol CreateSymbolCore(ChildStructSymbol parent, ChildSymbol restrictedChildSymbol, bool isListItem,
            int order, string displayName, string csName) {
            var restrictedChildSetSymbol = (ChildStructSymbol)restrictedChildSymbol;
            var childSetSymbol = new ChildStructSymbol(parent, csName, Kind, displayName, MemberName, MinOccurrence, MaxOccurrence,
                isListItem, order, restrictedChildSetSymbol, false, null);
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
    internal struct OccurrenceNode {
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
