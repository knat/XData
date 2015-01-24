﻿using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace XData.Compiler {
    public abstract class Symbol {
    }
    public sealed class ProgramSymbol : Symbol {
        public ProgramSymbol(bool needGenCode) {
            NeedGenCode = needGenCode;
            NamespaceList = new List<NamespaceSymbol>();
            //NamespaceList.Add(NamespaceSymbol.System);
        }
        public readonly bool NeedGenCode;
        public readonly List<NamespaceSymbol> NamespaceList;
    }
    public abstract class ObjectBaseSymbol : Symbol {
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
    public sealed class NamespaceSymbol : ObjectBaseSymbol {
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
            System = new NamespaceSymbol(InfoExtensions.SystemUri, new CSNamespaceNameNode { "XData" }, true);
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
    public abstract class ObjectSymbol : ObjectBaseSymbol {
        protected ObjectSymbol(ObjectBaseSymbol parent, string csName, bool isAbstract, bool isSealed, bool isCSOverride,
            NameSyntax csBaseFullName, NameSyntax[] csItfNames)
            : base(CS.QualifiedName(parent.CSFullName, csName), CS.MemberAccessExpr(parent.CSFullExp, csName)) {
            Parent = parent;
            CSName = csName;
            IsAbstract = isAbstract;
            IsSealed = isSealed;
            IsCSOverride = isCSOverride;
            CSBaseFullName = csBaseFullName;
            CSItfNames = csItfNames;
        }
        public readonly ObjectBaseSymbol Parent;
        public readonly string CSName;//C# classs escaped name
        public readonly bool IsAbstract;
        public readonly bool IsSealed;
        public readonly bool IsCSOverride;//same name inner class? (need 'new' modifier)
        public readonly NameSyntax CSBaseFullName;
        public readonly NameSyntax[] CSItfNames;//interface names, opt 
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
    //public abstract class NamedObjectSymbol : ObjectSymbol {
    //    protected NamedObjectSymbol(ObjectBaseSymbol parent, string csName, bool isAbstract, bool isSealed, bool isCSOverride,
    //        NameSyntax csBaseFullName, NameSyntax[] csItfNames, FullName fullName)
    //        : base(parent, csName, isAbstract, isSealed, isCSOverride, csBaseFullName, csItfNames) {
    //        FullName = fullName;
    //    }
    //    public readonly FullName FullName;
    //}
    public interface IGlobalObjectSymbol {
        FullName FullName { get; }
    }
    public abstract class TypeSymbol : ObjectSymbol, IGlobalObjectSymbol {
        protected TypeSymbol(ObjectBaseSymbol parent, string csName, bool isAbstract, bool isSealed,
            NameSyntax csBaseFullName, NameSyntax[] csItfNames, FullName fullName, TypeKind kind, TypeSymbol baseType)
            : base(parent, csName, isAbstract, isSealed, false, csBaseFullName, csItfNames) {
            FullName = fullName;
            Kind = kind;
            BaseType = baseType;
        }
        public FullName FullName { get; private set; }
        public readonly TypeKind Kind;
        public readonly TypeSymbol BaseType;
        public bool IsEqualToOrDeriveFrom(TypeSymbol other) {
            if (other == null) throw new ArgumentNullException("other");
            for (var info = this; info != null; info = info.BaseType) {
                if (info == other) {
                    return true;
                }
            }
            return false;
        }

    }
    public class SimpleTypeSymbol : TypeSymbol {
        public SimpleTypeSymbol(ObjectBaseSymbol parent, string csName, bool isAbstract, bool isSealed,
            NameSyntax csBaseFullName, NameSyntax[] csItfNames, FullName fullName, TypeKind kind, SimpleTypeSymbol baseType,
            ValueFacetSetInfo valueRestrictions)
            : base(parent, csName, isAbstract, isSealed, csBaseFullName, csItfNames, fullName, kind, baseType) {
            ValueRestrictions = valueRestrictions;
        }
        public readonly ValueFacetSetInfo ValueRestrictions;
    }
    public sealed class AtomTypeSymbol : SimpleTypeSymbol {
        public AtomTypeSymbol(ObjectBaseSymbol parent, string csName, bool isAbstract, bool isSealed,
            FullName fullName, TypeKind kind, SimpleTypeSymbol baseType, ValueFacetSetInfo valueRestrictions,
            TypeSyntax valueCSFullName)
            : base(parent, csName, isAbstract, isSealed, baseType.CSFullName, null, fullName, kind, baseType, valueRestrictions) {
            ValueCSFullName = valueCSFullName;
        }
        public readonly TypeSyntax ValueCSFullName;
    }
    public sealed class ListTypeSymbol : SimpleTypeSymbol {
        public ListTypeSymbol(ObjectBaseSymbol parent, string csName, bool isAbstract, bool isSealed,
            FullName fullName, SimpleTypeSymbol baseType, ValueFacetSetInfo valueRestrictions,
            NameSyntax[] csItfNames, SimpleTypeSymbol itemType)
            : base(parent, csName, isAbstract, isSealed, baseType.CSFullName, csItfNames, fullName, TypeKind.ListType, baseType, valueRestrictions) {
            ItemType = itemType;
        }
        public readonly SimpleTypeSymbol ItemType;
    }
    public sealed class ComplexTypeSymbol : TypeSymbol {
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
    public sealed class AttributeSetSymbol : ObjectSymbol {
        public AttributeSetSymbol(ComplexTypeSymbol parent, AttributeSetSymbol baseAttributeSet) :
            base(parent, "CLS_Attributes", false, false, baseAttributeSet != null,
                baseAttributeSet != null ? baseAttributeSet.CSFullName : CSEX.XAttributeSetName, null) {
            BaseAttributeSet = baseAttributeSet;
            AttributeList = new List<AttributeSymbol>();
        }
        public readonly AttributeSetSymbol BaseAttributeSet;//opt
        public readonly List<AttributeSymbol> AttributeList;

    }
    //public abstract class EntitySymbol : NamedObjectSymbol {
    //    protected EntitySymbol(ObjectBaseSymbol parent, string csName, bool isAbstract, bool isSealed, bool isCSOverride, NameSyntax csBaseFullName, FullName fullName,
    //        ElementKind declKind, EntitySymbol restrictedEntity, EntitySymbol substitutedEntity, EntitySymbol referencedEntity,
    //        string displayName, string memberName, bool isNullable, bool isOptional, TypeSymbol type) :
    //            base(parent, csName, isAbstract, isSealed, isCSOverride, csBaseFullName, null, fullName) {
    //        DeclKind = declKind;
    //        RestrictedEntity = restrictedEntity;
    //        ReferencedEntity = referencedEntity;
    //        DisplayName = displayName;
    //        MemberName = memberName;
    //        IsNullable = isNullable;
    //        IsOptional = isOptional;
    //        Type = type;
    //    }

    //    public readonly ElementKind DeclKind;
    //    public readonly EntitySymbol RestrictedEntity;
    //    public readonly EntitySymbol SubstitutedEntity;
    //    public readonly EntitySymbol ReferencedEntity;//for ref
    //    public string DisplayName { get; private set; }
    //    public string MemberName { get; private set; }

    //    public readonly bool IsNullable;
    //    public bool IsOptional { get; private set; }
    //    public readonly TypeSymbol Type;
    //    public bool IsLocal {
    //        get {
    //            return DeclKind == ElementKind.Local;
    //        }
    //    }
    //    public bool IsGlobal {
    //        get {
    //            return DeclKind == ElementKind.Global;
    //        }
    //    }
    //    public bool IsReference {
    //        get {
    //            return DeclKind == ElementKind.Reference;
    //        }
    //    }

    //}
    public sealed class AttributeSymbol : ObjectSymbol {
        public AttributeSymbol(AttributeSetSymbol parent, string csName,
            string name, string displayName, bool isOptional, bool isNullable, SimpleTypeSymbol type, AttributeSymbol restrictedAttribute) :
                base(parent, csName, false, false, restrictedAttribute != null,
                    restrictedAttribute != null ? restrictedAttribute.CSFullName : CSEX.XAttributeName, null) {
            Name = name;
            DisplayName = displayName;
            IsOptional = isOptional;
            IsNullable = isNullable;
            Type = type;
            RestrictedAttribute = restrictedAttribute;
        }
        public readonly string Name;
        public readonly string DisplayName;
        public readonly bool IsOptional;
        public readonly bool IsNullable;
        public readonly SimpleTypeSymbol Type;
        public readonly AttributeSymbol RestrictedAttribute;
    }
    //public interface IChildSymbol {
    //    string DisplayName { get; }
    //    string MemberName { get; }
    //    int Order { get; }
    //    bool IsOptional { get; }
    //}
    public abstract class ChildSymbol : ObjectSymbol {
        protected ChildSymbol(ObjectBaseSymbol parent, string csName, bool isAbstract, bool isSealed, bool isCSOverride, NameSyntax csBaseFullName, NameSyntax[] csItfNames,
            string displayName, string memberName, bool isOptional, int order, ChildSymbol restrictedChild) :
                base(parent, csName, isAbstract, isSealed, isCSOverride, csBaseFullName, csItfNames) {
            DisplayName = displayName;
            MemberName = memberName;
            _isOptional = isOptional;
            Order = order;
            RestrictedChild = restrictedChild;
        }
        public readonly string DisplayName;
        public readonly string MemberName;
        private readonly bool _isOptional;
        public virtual bool IsOptional { get { return _isOptional; } }
        public readonly int Order;
        public readonly ChildSymbol RestrictedChild;
    }

    public sealed class ElementSymbol : ChildSymbol, IGlobalObjectSymbol {
        public ElementSymbol(ObjectBaseSymbol parent, string csName, bool isAbstract, bool isSealed,
            string displayName, string memberName, bool isOptional, int order, ElementSymbol restrictedElement,
            FullName fullName, ElementKind kind, bool isNullable, TypeSymbol type, ElementSymbol referencedElement, ElementSymbol substitutedElement
             )
            : base(parent, csName, isAbstract, isSealed, restrictedElement != null,
                 restrictedElement != null ? restrictedElement.CSFullName : substitutedElement != null ? substitutedElement.CSFullName : CSEX.XElementName,
                 null, displayName, memberName, isOptional, order, restrictedElement) {
            FullName = fullName;
            Kind = kind;
            IsNullable = isNullable;
            Type = type;
            ReferencedElement = referencedElement;
            SubstitutedElement = substitutedElement;
        }
        public FullName FullName { get; private set; }
        public readonly ElementKind Kind;
        public readonly bool IsNullable;
        public readonly TypeSymbol Type;
        public ElementSymbol RestrictedElement {
            get {
                return (ElementSymbol)RestrictedChild;
            }
        }
        public readonly ElementSymbol ReferencedElement;
        public readonly ElementSymbol SubstitutedElement;

    }
    public abstract class ChildContainerSymbol : ChildSymbol {
        protected ChildContainerSymbol(ObjectSymbol parent, string csName, bool isCSOverride, NameSyntax csBaseFullName, NameSyntax[] csItfNames,
            string displayName, string memberName, bool isOptional, int order, ChildContainerSymbol restrictedChildContainer)
            : base(parent, csName, false, false, isCSOverride, csBaseFullName, csItfNames, displayName, memberName, isOptional, order, restrictedChildContainer) {
        }
    }

    public sealed class ChildSetSymbol : ChildContainerSymbol {
        public ChildSetSymbol(ObjectSymbol parent, string csName,
            string displayName, string memberName, bool isOptional, int order, ChildSetSymbol restrictedChildSet,
            ChildSetKind kind, bool isRoot, ChildSetSymbol baseChildSet)
            : base(parent, csName, restrictedChildSet != null || baseChildSet != null,
                 restrictedChildSet != null ? restrictedChildSet.CSFullName : baseChildSet != null ? baseChildSet.CSFullName : CSEX.XChildSetName, null,
                 displayName, memberName, isOptional, order, restrictedChildSet) {
            Kind = kind;
            IsRoot = isRoot;
            BaseChildSet = baseChildSet;
            MemberList = new List<ChildSymbol>();
        }
        public readonly ChildSetKind Kind;
        public readonly bool IsRoot;
        public readonly ChildSetSymbol BaseChildSet;//for root
        public readonly List<ChildSymbol> MemberList;
        private bool? _isOptional;
        public override bool IsOptional {
            get {
                if (_isOptional == null) {
                    if (base.IsOptional || MemberList.Count == 0) {
                        _isOptional = true;
                    }
                    else {
                        if (Kind == ChildSetKind.Sequence) {
                            _isOptional = true;
                            foreach (var member in MemberList) {
                                if (!member.IsOptional) {
                                    _isOptional = false;
                                    break;
                                }
                            }
                        }
                        else {//choice
                            _isOptional = false;
                            foreach (var member in MemberList) {
                                if (member.IsOptional) {
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

    }
    public sealed class ChildListSymbol : ChildContainerSymbol {
        public ChildListSymbol(ObjectSymbol parent, string csName,
            string displayName, string memberName, bool isOptional, int order, ChildListSymbol restrictedChildList,
            ulong minOccurs, ulong maxOccurs, ChildSymbol item)
            : base(parent, csName, restrictedChildList != null,
                restrictedChildList != null ? restrictedChildList.CSFullName : CSEX.XChildListOf(item.CSFullName),
                null, displayName, memberName, isOptional, order, restrictedChildList) {
            MinOccurs = minOccurs;
            MaxOccurs = maxOccurs;
            Item = item;
        }
        public readonly ulong MinOccurs;
        public readonly ulong MaxOccurs;
        public readonly ChildSymbol Item;
    }


}