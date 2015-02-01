using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace XData.Compiler {
    internal abstract class Symbol {
    }
    internal sealed class ProgramSymbol : Symbol {
        public ProgramSymbol(bool needGenCode) {
            NeedGenCode = needGenCode;
            NamespaceList = new List<NamespaceSymbol>();
            //NamespaceList.Add(NamespaceSymbol.System);
        }
        public readonly bool NeedGenCode;
        public readonly List<NamespaceSymbol> NamespaceList;
    }
    internal abstract class ObjectBaseSymbol : Symbol {
        protected ObjectBaseSymbol(NameSyntax csFullName, ExpressionSyntax csFullExp) {
            CSFullName = csFullName;
            CSFullExp = csFullExp;
        }
        public readonly NameSyntax CSFullName;//C# qualified name
        public readonly ExpressionSyntax CSFullExp;//C# member access expr
        private List<MemberDeclarationSyntax> _csMemberList;
        public List<MemberDeclarationSyntax> CSMemberList {
            get {
                return _csMemberList ?? (_csMemberList = new List<MemberDeclarationSyntax>());
            }
        }
    }
    internal sealed class NamespaceSymbol : ObjectBaseSymbol {
        public NamespaceSymbol(string uri, CSNamespaceNameNode csNamespaceName, bool isCSNamespaceRef)
            : base(csNamespaceName.CSFullName, csNamespaceName.CSFullExp) {
            Uri = uri;
            CSNamespaceName = csNamespaceName;
            IsCSNamespaceRef = isCSNamespaceRef;
            GlobalObjectList = new List<IGlobalObjectSymbol>();
        }
        public readonly string Uri;
        public readonly CSNamespaceNameNode CSNamespaceName;
        public readonly bool IsCSNamespaceRef;
        public readonly List<IGlobalObjectSymbol> GlobalObjectList;
        public IGlobalObjectSymbol TryGetGlobalObject(string name) {
            foreach (var obj in GlobalObjectList) {
                if (obj.FullName.Name == name) {
                    return obj;
                }
            }
            return null;
        }
        //public NamedObjectSymbol TryGetGlobalObject(FullName fullName) {
        //    foreach (var obj in GlobalObjectList) {
        //        if (obj.FullName == fullName) {
        //            return obj;
        //        }
        //    }
        //    return null;
        //}

        //
        public static readonly NamespaceSymbol System;
        public static readonly SimpleTypeSymbol SystemSimpleType;
        public static readonly AtomTypeSymbol SystemAtomType;
        public static readonly ListTypeSymbol SystemListType;
        public static readonly ComplexTypeSymbol SystemComplexType;
        static NamespaceSymbol() {
            System = new NamespaceSymbol(Extensions.SystemUri, new CSNamespaceNameNode { "XData" }, true);
            SystemSimpleType = new SimpleTypeSymbol(System, TypeKind.SimpleType.ToClassName(), true, false, null, null, TypeKind.SimpleType.ToFullName(), TypeKind.SimpleType, null, null);
            SystemAtomType = new AtomTypeSymbol(System, TypeKind.AtomType.ToClassName(), true, false, TypeKind.AtomType.ToFullName(), TypeKind.AtomType, SystemSimpleType, null, null);
            CreateAndAdd(SystemAtomType, TypeKind.String, CS.StringType);
            CreateAndAdd(SystemAtomType, TypeKind.IgnoreCaseString, CS.StringType);
            var Decimal = CreateAndAdd(SystemAtomType, TypeKind.Decimal, CS.DecimalType);
            var Int64 = CreateAndAdd(Decimal, TypeKind.Int64, CS.LongType);
            var Int32 = CreateAndAdd(Int64, TypeKind.Int32, CS.IntType);
            var Int16 = CreateAndAdd(Int32, TypeKind.Int16, CS.ShortType);
            CreateAndAdd(Int16, TypeKind.SByte, CS.SByteType);
            var UInt64 = CreateAndAdd(Decimal, TypeKind.UInt64, CS.ULongType);
            var UInt32 = CreateAndAdd(UInt64, TypeKind.UInt32, CS.UIntType);
            var UInt16 = CreateAndAdd(UInt32, TypeKind.UInt16, CS.UShortType);
            CreateAndAdd(UInt16, TypeKind.Byte, CS.ByteType);
            var Double = CreateAndAdd(SystemAtomType, TypeKind.Double, CS.DoubleType);
            CreateAndAdd(Double, TypeKind.Single, CS.FloatType);
            CreateAndAdd(SystemAtomType, TypeKind.Boolean, CS.BoolType);
            CreateAndAdd(SystemAtomType, TypeKind.Binary, CS.ByteArrayType);
            CreateAndAdd(SystemAtomType, TypeKind.Guid, CS.GuidName);
            CreateAndAdd(SystemAtomType, TypeKind.TimeSpan, CS.TimeSpanName);
            CreateAndAdd(SystemAtomType, TypeKind.DateTimeOffset, CS.DateTimeOffsetName);
            SystemListType = new ListTypeSymbol(System, TypeKind.ListType.ToClassName(), true, false, TypeKind.ListType.ToFullName(), SystemSimpleType, null, null, null);
            SystemComplexType = new ComplexTypeSymbol(System, TypeKind.ComplexType.ToClassName(), true, false, TypeKind.ComplexType.ToFullName(), null);
            var objList = System.GlobalObjectList;
            objList.Add(SystemSimpleType);
            objList.Add(SystemAtomType);
            objList.Add(SystemListType);
            objList.Add(SystemComplexType);
        }
        private static AtomTypeSymbol CreateAndAdd(AtomTypeSymbol baseType, TypeKind kind, TypeSyntax valueCSFullName) {
            var symbol = new AtomTypeSymbol(System, kind.ToClassName(), false, false, kind.ToFullName(), kind, baseType, null, valueCSFullName);
            System.GlobalObjectList.Add(symbol);
            return symbol;
        }
    }
    internal abstract class ObjectSymbol : ObjectBaseSymbol {
        protected ObjectSymbol(ObjectBaseSymbol parent, string csName, bool isAbstract, bool isSealed, bool isCSOverride,
            NameSyntax csBaseFullName, NameSyntax[] csItfNames, string displayName)
            : base(CS.QualifiedName(parent.CSFullName, csName), CS.MemberAccessExpr(parent.CSFullExp, csName)) {
            Parent = parent;
            CSName = csName;
            IsAbstract = isAbstract;
            IsSealed = isSealed;
            IsCSOverride = isCSOverride;
            CSBaseFullName = csBaseFullName;
            CSItfNames = csItfNames;
            DisplayName = displayName;
        }
        public readonly ObjectBaseSymbol Parent;
        public readonly string CSName;//C# classs escaped name
        public readonly bool IsAbstract;
        public readonly bool IsSealed;
        public readonly bool IsCSOverride;//same name inner class? (need 'new' modifier)
        public readonly NameSyntax CSBaseFullName;
        public readonly NameSyntax[] CSItfNames;//interface names, opt 
        public readonly string DisplayName;
        //
        private ExpressionSyntax _thisInfoExp;
        public ExpressionSyntax ThisInfoExp {
            get {
                return _thisInfoExp ?? (_thisInfoExp = CS.MemberAccessExpr(CSFullExp, "ThisInfo"));
            }
        }

        private bool _isGenerated;
        public void GenerateCSClass() {
            if (_isGenerated == false) {
                _isGenerated = true;
            }
        }

    }
    internal interface IGlobalObjectSymbol {
        FullName FullName { get; }
    }
    internal abstract class TypeSymbol : ObjectSymbol, IGlobalObjectSymbol {
        protected TypeSymbol(ObjectBaseSymbol parent, string csName, bool isAbstract, bool isSealed,
            NameSyntax csBaseFullName, NameSyntax[] csItfNames, FullName fullName, TypeKind kind, TypeSymbol baseType)
            : base(parent, csName, isAbstract, isSealed, false, csBaseFullName, csItfNames, fullName.ToString()) {
            FullName = fullName;
            Kind = kind;
            BaseType = baseType;
        }
        public FullName FullName { get; private set; }
        public readonly TypeKind Kind;
        public readonly TypeSymbol BaseType;
        public bool EqualToOrDeriveFrom(TypeSymbol other) {
            if (other == null) throw new ArgumentNullException("other");
            for (var symbol = this; symbol != null; symbol = symbol.BaseType) {
                if (symbol == other) {
                    return true;
                }
            }
            return false;
        }

    }
    internal sealed class FacetSetSymbol : FacetSetInfo {
        public FacetSetSymbol(
            ulong? minLength, ulong? maxLength,
            byte? precision, byte? scale,
            ValueBoundaryInfo? minValue, ValueBoundaryInfo? maxValue,
            EnumInfo? @enum, PatternInfo[] patterns,
            FacetSetSymbol baseFacetSet) :
            base(minLength, maxLength, precision, scale, minValue, maxValue, @enum, patterns) {
            BaseFacetSet = baseFacetSet;
        }
        public readonly FacetSetSymbol BaseFacetSet;
    }

    internal class SimpleTypeSymbol : TypeSymbol {
        public SimpleTypeSymbol(ObjectBaseSymbol parent, string csName, bool isAbstract, bool isSealed,
            NameSyntax csBaseFullName, NameSyntax[] csItfNames, FullName fullName, TypeKind kind, SimpleTypeSymbol baseType,
            FacetSetSymbol facets)
            : base(parent, csName, isAbstract, isSealed, csBaseFullName, csItfNames, fullName, kind, baseType) {
            Facets = facets;
        }
        public readonly FacetSetSymbol Facets;
    }
    internal sealed class AtomTypeSymbol : SimpleTypeSymbol {
        public AtomTypeSymbol(ObjectBaseSymbol parent, string csName, bool isAbstract, bool isSealed,
            FullName fullName, TypeKind kind, SimpleTypeSymbol baseType, FacetSetSymbol facets, TypeSyntax valueCSFullName)
            : base(parent, csName, isAbstract, isSealed, baseType.CSFullName, null, fullName, kind, baseType, facets) {
            ValueCSFullName = valueCSFullName;
        }
        public readonly TypeSyntax ValueCSFullName;
    }
    internal sealed class ListTypeSymbol : SimpleTypeSymbol {
        public ListTypeSymbol(ObjectBaseSymbol parent, string csName, bool isAbstract, bool isSealed,
            FullName fullName, SimpleTypeSymbol baseType, FacetSetSymbol facets, NameSyntax[] csItfNames, SimpleTypeSymbol itemType)
            : base(parent, csName, isAbstract, isSealed, baseType.CSFullName, csItfNames, fullName, TypeKind.ListType, baseType, facets) {
            ItemType = itemType;
        }
        public readonly SimpleTypeSymbol ItemType;
    }
    internal sealed class ComplexTypeSymbol : TypeSymbol {
        public ComplexTypeSymbol(ObjectBaseSymbol parent, string csName, bool isAbstract, bool isSealed,
            FullName fullName, ComplexTypeSymbol baseType)
            : base(parent, csName, isAbstract, isSealed, baseType != null ? baseType.CSFullName : CSEX.XComplexTypeName, null,
                  fullName, TypeKind.ComplexType, baseType) {
        }
        public AttributeSetSymbol Attributes;
        public ObjectSymbol Children;
        public SimpleTypeSymbol SimpleChild {
            get {
                return Children as SimpleTypeSymbol;
            }
        }
        public ChildSetSymbol ComplexChildren {
            get {
                return Children as ChildSetSymbol;
            }
        }
    }
    internal sealed class AttributeSetSymbol : ObjectSymbol {
        public AttributeSetSymbol(ComplexTypeSymbol parent, AttributeSetSymbol baseAttributeSet, string displayName) :
            base(parent, "CLS_Attributes", false, false, baseAttributeSet != null,
                baseAttributeSet != null ? baseAttributeSet.CSFullName : CSEX.XAttributeSetName, null, displayName) {
            BaseAttributeSet = baseAttributeSet;
            AttributeList = new List<AttributeSymbol>();
        }
        public readonly AttributeSetSymbol BaseAttributeSet;//opt
        public readonly List<AttributeSymbol> AttributeList;
    }
    internal sealed class AttributeSymbol : ObjectSymbol {
        public AttributeSymbol(AttributeSetSymbol parent, string csName,
            string name, string displayName, bool isOptional, bool isNullable, SimpleTypeSymbol type, AttributeSymbol restrictedAttribute) :
                base(parent, csName, false, false, restrictedAttribute != null,
                    restrictedAttribute != null ? restrictedAttribute.CSFullName : CSEX.XAttributeName, null, displayName) {
            Name = name;
            IsOptional = isOptional;
            IsNullable = isNullable;
            Type = type;
            RestrictedAttribute = restrictedAttribute;
        }
        public readonly string Name;
        public readonly bool IsOptional;
        public readonly bool IsNullable;
        public readonly SimpleTypeSymbol Type;
        public readonly AttributeSymbol RestrictedAttribute;
    }

    internal abstract class ChildSymbol : ObjectSymbol {
        protected ChildSymbol(ObjectBaseSymbol parent, string csName, bool isAbstract, bool isSealed, bool isCSOverride, NameSyntax csBaseFullName, NameSyntax[] csItfNames,
            ChildKind kind, string displayName, string memberName, ulong minOccurrence, ulong maxOccurrence, int order, ChildSymbol restrictedChild) :
                base(parent, csName, isAbstract, isSealed, isCSOverride, csBaseFullName, csItfNames, displayName) {
            Kind = kind;
            MemberName = memberName;
            MinOccurrence = minOccurrence;
            MaxOccurrence = maxOccurrence;
            Order = order;
            RestrictedChild = restrictedChild;
        }
        public readonly ChildKind Kind;
        public readonly string MemberName;
        public readonly ulong MinOccurrence;
        public readonly ulong MaxOccurrence;
        public virtual bool IsOptional {
            get {
                return MinOccurrence == 0;
            }
        }
        public readonly int Order;
        public readonly ChildSymbol RestrictedChild;
        public bool HasIntersection(FullName fullName) {
            return HasIntersection(fullName, false);
        }
        public abstract bool HasIntersection(FullName fullName, bool forChoice);
    }

    internal sealed class ElementSymbol : ChildSymbol, IGlobalObjectSymbol {
        public ElementSymbol(ObjectBaseSymbol parent, string csName, bool isAbstract, bool isSealed,
            ChildKind kind, string displayName, string memberName, ulong minOccurrence, ulong maxOccurrence, int order, ElementSymbol restrictedElement,
            FullName fullName, bool isNullable, TypeSymbol type, ElementSymbol referencedElement, ElementSymbol substitutedElement
             )
            : base(parent, csName, isAbstract, isSealed, restrictedElement != null,
                 restrictedElement != null ? restrictedElement.CSFullName : substitutedElement != null ? substitutedElement.CSFullName :
                    kind == ChildKind.LocalElement ? CSEX.XLocalElementName : kind == ChildKind.GlobalElementRef ? CSEX.XGlobalElementRefName : CSEX.XGlobalElementName,
                 null, kind, displayName, memberName, minOccurrence, maxOccurrence, order, restrictedElement) {
            FullName = fullName;
            IsNullable = isNullable;
            Type = type;
            ReferencedElement = referencedElement;
            SubstitutedElement = substitutedElement;
        }
        public FullName FullName { get; private set; }
        public readonly bool IsNullable;
        public readonly TypeSymbol Type;
        public ElementSymbol RestrictedElement {
            get {
                return (ElementSymbol)RestrictedChild;
            }
        }
        public readonly ElementSymbol ReferencedElement;
        public readonly ElementSymbol SubstitutedElement;
        public GlobalElementNode GlobalElementNode;//opt
        public bool IsLocal {
            get {
                return Kind == ChildKind.LocalElement;
            }
        }
        public bool IsGlobal {
            get {
                return Kind == ChildKind.GlobalElement;
            }
        }
        public bool IsGlobalRef {
            get {
                return Kind == ChildKind.GlobalElementRef;
            }
        }
        public bool EqualToOrSubstituteFor(ElementSymbol other) {
            if (other == null) throw new ArgumentNullException("other");
            for (var symbol = this; symbol != null; symbol = symbol.SubstitutedElement) {
                if (symbol == other) {
                    return true;
                }
            }
            return false;
        }
        public override bool HasIntersection(FullName fullName, bool forChoice) {
            if (MaxOccurrence > MinOccurrence || forChoice) {
                if (IsLocal) {
                    return FullName == fullName;
                }
                return ReferencedElement.GlobalElementNode.HasIntersection(fullName);
            }
            return false;
        }
    }
    internal abstract class ChildContainerSymbol : ChildSymbol {
        public ChildContainerSymbol(ObjectSymbol parent, string csName, bool isCSOverride, NameSyntax csBaseFullName, NameSyntax[] csItfNames,
            ChildKind kind, string displayName, string memberName, ulong minOccurrence, ulong maxOccurrence, int order, ChildContainerSymbol restrictedChildContainer)
            : base(parent, csName, false, false, isCSOverride, csBaseFullName, csItfNames, kind, displayName, memberName, minOccurrence, maxOccurrence, order, restrictedChildContainer) {
        }
    }

    internal sealed class ChildSetSymbol : ChildContainerSymbol {
        public ChildSetSymbol(ObjectSymbol parent, string csName,
            ChildKind kind, string displayName, string memberName, ulong minOccurrence, ulong maxOccurrence, int order, ChildSetSymbol restrictedChildSet,
            bool isRoot, ChildSetSymbol baseChildSet)
            : base(parent, csName, restrictedChildSet != null || baseChildSet != null,
                 restrictedChildSet != null ? restrictedChildSet.CSFullName : baseChildSet != null ? baseChildSet.CSFullName : CSEX.XChildSequenceName, null,
                 kind, displayName, memberName, minOccurrence, maxOccurrence, order, restrictedChildSet) {
            IsRoot = isRoot;
            BaseChildSet = baseChildSet;
            ChildList = new List<ChildSymbol>();
        }
        public readonly bool IsRoot;
        public int NextChildOrder;//for root
        public readonly ChildSetSymbol BaseChildSet;//for root
        public readonly List<ChildSymbol> ChildList;
        private bool? _isOptional;
        public override bool IsOptional {
            get {
                if (_isOptional == null) {
                    if (base.IsOptional || ChildList.Count == 0) {
                        _isOptional = true;
                    }
                    else {
                        if (IsSequence) {
                            _isOptional = true;
                            foreach (var child in ChildList) {
                                if (!child.IsOptional) {
                                    _isOptional = false;
                                    break;
                                }
                            }
                        }
                        else {//choice
                            _isOptional = false;
                            foreach (var child in ChildList) {
                                if (child.IsOptional) {
                                    _isOptional = true;
                                    break;
                                }
                            }
                        }
                    }
                }
                return _isOptional.Value;
            }
        }
        public ChildSetSymbol RestrictedChildSet {
            get {
                return (ChildSetSymbol)RestrictedChild;
            }
        }
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
        public override bool HasIntersection(FullName fullName, bool forChoice) {
            if (IsSequence) {
                if (forChoice) {
                    foreach (var child in ChildList) {
                        if (child.HasIntersection(fullName, true)) {
                            return true;
                        }
                        if (child.MinOccurrence == child.MaxOccurrence) {
                            break;
                        }
                    }
                    return false;
                }
                else {
                    for (var i = ChildList.Count - 1; i >= 0; --i) {
                        var child = ChildList[i];
                        if (child.HasIntersection(fullName, false)) {
                            return true;
                        }
                        if (child.MinOccurrence == child.MaxOccurrence) {
                            return false;
                        }
                    }
                }
            }
            else {//choice
                foreach (var child in ChildList) {
                    if (child.HasIntersection(fullName, true)) {
                        return true;
                    }
                }
            }
            if (!forChoice) {
                var parentChildSet = Parent as ChildSetSymbol;
                if (parentChildSet != null) {
                    return parentChildSet.HasIntersection(fullName, false);
                }
            }
            return false;
        }
    }
    internal sealed class ChildListSymbol : ChildContainerSymbol {
        public ChildListSymbol(ChildSetSymbol parent, string csName,
            string displayName, string memberName, ulong minOccurrence, ulong maxOccurrence, int order, ChildListSymbol restrictedChildList,
             ChildSymbol item)
            : base(parent, csName, restrictedChildList != null,
                restrictedChildList != null ? restrictedChildList.CSFullName : CSEX.XChildListOf(item.CSFullName),
                restrictedChildList != null ? CSEX.IListAndIReadOnlyListOf(item.CSFullName) : null, ChildKind.List,
                displayName, memberName, minOccurrence, maxOccurrence, order, restrictedChildList) {
            Item = item;
        }
        public readonly ChildSymbol Item;
        public override bool HasIntersection(FullName fullName, bool forChoice) {
            return Item.HasIntersection(fullName, forChoice);
        }
    }


}