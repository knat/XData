using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace XData.Compiler {
    internal abstract class Symbol {
    }
    internal abstract class ObjectBaseSymbol : Symbol {
        protected ObjectBaseSymbol(ObjectBaseSymbol parent, NameSyntax csFullName, ExpressionSyntax csFullExpr) {
            Parent = parent;
            CSFullName = csFullName;
            CSFullExpr = csFullExpr;
        }
        public readonly ObjectBaseSymbol Parent;
        public readonly NameSyntax CSFullName;//C# qualified name
        public readonly ExpressionSyntax CSFullExpr;//C# member access expr
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
        private List<MemberDeclarationSyntax> _csMemberList;
        public List<MemberDeclarationSyntax> CSMemberList {
            get {
                return _csMemberList ?? (_csMemberList = new List<MemberDeclarationSyntax>());
            }
        }

    }
    internal sealed class NamespaceSymbol : ObjectBaseSymbol {
        public NamespaceSymbol(string uri, CSharpNamespaceNameNode csNamespaceName, bool isCSNamespaceRef)
            : base(null, csNamespaceName.CSFullName, csNamespaceName.CSFullExpr) {
            Uri = uri;
            CSNamespaceName = csNamespaceName;
            IsCSNamespaceRef = isCSNamespaceRef;
            GlobalObjectList = new List<IGlobalObjectSymbol>();
        }
        public readonly string Uri;
        public readonly CSharpNamespaceNameNode CSNamespaceName;
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
        public void Generate(List<MemberDeclarationSyntax> list) {
            if (!IsCSNamespaceRef) {
                foreach (var obj in GlobalObjectList) {
                    obj.Generate();
                }
                list.Add(SyntaxFactory.NamespaceDeclaration(CSNamespaceName.CSNonGlobalFullName,
                    default(SyntaxList<ExternAliasDirectiveSyntax>), default(SyntaxList<UsingDirectiveSyntax>),
                    SyntaxFactory.List(CSMemberList)));
            }
        }
        public ExpressionSyntax InfoExpr {
            get {
                //>new NamespaceInfo("uri", new IGlobalObjectInfo[]{ ... }),
                return CS.NewObjExpr(CSEX.NamespaceInfoName, CS.Literal(Uri), CS.NewArrOrNullExpr(CSEX.IGlobalObjectInfoNameArrayType,
                    GlobalObjectList.Select(i => i.ThisInfoExpr)));
            }
        }
        //
        public static readonly NamespaceSymbol System;
        public static readonly SimpleTypeSymbol SystemSimpleType;
        public static readonly AtomTypeSymbol SystemAtomType;
        public static readonly ListTypeSymbol SystemListType;
        public static readonly ComplexTypeSymbol SystemComplexType;
        static NamespaceSymbol() {
            System = new NamespaceSymbol(Extensions.SystemUri, new CSharpNamespaceNameNode { "XData" }, true);
            SystemSimpleType = new SimpleTypeSymbol(System, TypeKind.SimpleType.ToClassName(), true, false, null, null,
                TypeKind.SimpleType.ToFullName(), TypeKind.SimpleType, null);
            SystemAtomType = new AtomTypeSymbol(System, TypeKind.AtomType.ToClassName(), true, false, TypeKind.AtomType.ToFullName(),
                TypeKind.AtomType, SystemSimpleType, null, true);
            CreateAndAdd(SystemAtomType, TypeKind.String, CS.StringType, true);
            CreateAndAdd(SystemAtomType, TypeKind.IgnoreCaseString, CS.StringType, true);
            var Decimal = CreateAndAdd(SystemAtomType, TypeKind.Decimal, CS.DecimalType, true);
            var Int64 = CreateAndAdd(Decimal, TypeKind.Int64, CS.LongType, true);
            var Int32 = CreateAndAdd(Int64, TypeKind.Int32, CS.IntType, true);
            var Int16 = CreateAndAdd(Int32, TypeKind.Int16, CS.ShortType, true);
            CreateAndAdd(Int16, TypeKind.SByte, CS.SByteType, true);
            var UInt64 = CreateAndAdd(Decimal, TypeKind.UInt64, CS.ULongType, true);
            var UInt32 = CreateAndAdd(UInt64, TypeKind.UInt32, CS.UIntType, true);
            var UInt16 = CreateAndAdd(UInt32, TypeKind.UInt16, CS.UShortType, true);
            CreateAndAdd(UInt16, TypeKind.Byte, CS.ByteType, true);
            var Double = CreateAndAdd(SystemAtomType, TypeKind.Double, CS.DoubleType, true);
            CreateAndAdd(Double, TypeKind.Single, CS.FloatType, true);
            CreateAndAdd(SystemAtomType, TypeKind.Boolean, CS.BoolType, true);
            CreateAndAdd(SystemAtomType, TypeKind.Binary, CS.ByteArrayType, false);
            CreateAndAdd(SystemAtomType, TypeKind.Guid, CS.GuidName, false);
            CreateAndAdd(SystemAtomType, TypeKind.TimeSpan, CS.TimeSpanName, false);
            CreateAndAdd(SystemAtomType, TypeKind.DateTimeOffset, CS.DateTimeOffsetName, false);
            SystemListType = new ListTypeSymbol(System, TypeKind.ListType.ToClassName(), true, false, TypeKind.ListType.ToFullName(),
                SystemSimpleType, null, null, null);
            SystemComplexType = new ComplexTypeSymbol(System, TypeKind.ComplexType.ToClassName(), true, false,
                TypeKind.ComplexType.ToFullName(), null);
            var objList = System.GlobalObjectList;
            objList.Add(SystemSimpleType);
            objList.Add(SystemAtomType);
            objList.Add(SystemListType);
            objList.Add(SystemComplexType);
        }
        private static AtomTypeSymbol CreateAndAdd(AtomTypeSymbol baseType, TypeKind kind, TypeSyntax valueCSFullName, bool canBeConst) {
            var symbol = new AtomTypeSymbol(System, kind.ToClassName(), false, false, kind.ToFullName(), kind, baseType, valueCSFullName, canBeConst);
            System.GlobalObjectList.Add(symbol);
            return symbol;
        }
    }
    internal abstract class ObjectSymbol : ObjectBaseSymbol {
        protected ObjectSymbol(ObjectBaseSymbol parent, string csName, bool isAbstract, bool isSealed, bool isCSOverride,
            NameSyntax csBaseFullName, NameSyntax[] csItfNames, string displayName)
            : base(parent, CS.QualifiedName(parent.CSFullName, csName), CS.MemberAccessExpr(parent.CSFullExpr, csName)) {
            CSName = csName;
            IsAbstract = isAbstract;
            IsSealed = isSealed;
            IsCSOverride = isCSOverride;
            CSBaseFullName = csBaseFullName;
            CSItfNames = csItfNames;
            DisplayName = displayName;
        }
        public readonly string CSName;//C# classs escaped name
        public readonly bool IsAbstract;
        public readonly bool IsSealed;
        public readonly bool IsCSOverride;//same name inner class? (need 'new' modifier)
        public readonly NameSyntax CSBaseFullName;
        public readonly NameSyntax[] CSItfNames;//interface names, opt 
        public readonly string DisplayName;
        //
        private ExpressionSyntax _thisInfoExpr;
        public ExpressionSyntax ThisInfoExpr {
            get {
                return _thisInfoExpr ?? (_thisInfoExpr = CS.MemberAccessExpr(CSFullExpr, "ThisInfo"));
            }
        }
        private NamespaceSymbol _namespaceAncestor;
        public NamespaceSymbol NamespaceAncestor {
            get {
                return _namespaceAncestor ?? (_namespaceAncestor = GetAncestor<NamespaceSymbol>());
            }
        }

        private bool _isGenerated;
        public void Generate() {
            if (!_isGenerated) {
                _isGenerated = true;
                if (!NamespaceAncestor.IsCSNamespaceRef) {
                    GenerateMembers(CSMemberList);
                    var csCls = SyntaxFactory.ClassDeclaration(
                        attributeLists: default(SyntaxList<AttributeListSyntax>),
                        modifiers: ModifiersTokenList,
                        identifier: CS.Id(CSName),
                        typeParameterList: null,
                        baseList: CSItfNames == null ? CS.BaseList(CSBaseFullName) : CS.BaseList(GetBaseNames()),
                        constraintClauses: default(SyntaxList<TypeParameterConstraintClauseSyntax>),
                        members: SyntaxFactory.List(CSMemberList)
                        );
                    Parent.CSMemberList.Add(csCls);
                }
            }
        }
        protected abstract void GenerateMembers(List<MemberDeclarationSyntax> list);
        private SyntaxTokenList ModifiersTokenList {
            get {
                if (IsCSOverride && IsAbstract) {
                    return CS.NewPublicAbstractPartialTokenList;
                }
                if (IsCSOverride) {
                    return CS.NewPublicPartialTokenList;
                }
                if (IsAbstract) {
                    return CS.PublicAbstractPartialTokenList;
                }
                return CS.PublicPartialTokenList;
            }
        }
        private IEnumerable<NameSyntax> GetBaseNames() {
            yield return CSBaseFullName;
            foreach (var itfName in CSItfNames) {
                yield return itfName;
            }
        }
    }
    internal interface IGlobalObjectSymbol {
        FullName FullName { get; }
        ExpressionSyntax ThisInfoExpr { get; }
        void Generate();
    }
    internal abstract class TypeSymbol : ObjectSymbol, IGlobalObjectSymbol {
        protected TypeSymbol(NamespaceSymbol parent, string csName, bool isAbstract, bool isSealed,
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
        public void GenerateConcreteType(List<MemberDeclarationSyntax> list) {
            //>new public XXX Type {
            //    get { return base.GenericType as XXX; }
            //    set { base.GenericType = value; }
            //}
            list.Add(CS.Property(CS.NewPublicTokenList, CSFullName, "Type", false,
                 default(SyntaxTokenList), new[] { CS.ReturnStm(CS.AsExpr(CS.BaseMemberAccessExpr("GenericType"), CSFullName)) },
                 default(SyntaxTokenList), new[] { CS.AssignStm(CS.BaseMemberAccessExpr("GenericType"), CS.IdName("value")) }));
            ////>new public XXX EnsureType(bool @try = false){
            ////  return base.EnsureType<XXX>(@try);
            ////}
            //list.Add(CS.Method(CS.NewPublicTokenList, CSFullName, "EnsureType",
            //    new[] { CS.Parameter(CS.BoolType, "@try", CS.FalseLiteral) },
            //    CS.ReturnStm(CS.InvoExpr(CS.BaseMemberAccessExpr(CS.GenericName("EnsureType", CSFullName)), CS.IdName("@try")))));
        }

    }
    #region facets
    internal sealed class FacetSetSymbol : Symbol {
        public FacetSetSymbol(SimpleTypeSymbol parent, FacetSetSymbol baseFacetSet,
            ulong? minLength, ulong? maxLength,
            byte? precision, byte? scale,
            ValueBoundaryInfo? minValue, ValueBoundaryInfo? maxValue,
            EnumInfoEx? @enum, List<string> patternList) {
            Parent = parent;
            BaseFacetSet = baseFacetSet;
            MinLength = minLength;
            MaxLength = maxLength;
            Precision = precision;
            Scale = scale;
            MinValue = minValue;
            MaxValue = maxValue;
            Enum = @enum;
            PatternList = patternList;
        }
        public readonly ulong? MinLength;
        public readonly ulong? MaxLength;
        public readonly byte? Precision;
        public readonly byte? Scale;
        public readonly ValueBoundaryInfo? MinValue;
        public readonly ValueBoundaryInfo? MaxValue;
        public readonly EnumInfoEx? Enum;
        public readonly List<string> PatternList;
        //
        public readonly SimpleTypeSymbol Parent;
        public readonly FacetSetSymbol BaseFacetSet;
        private ExpressionSyntax _thisFacetSetInfoExpr;
        public ExpressionSyntax ThisFacetSetInfoExpr {
            get {
                return _thisFacetSetInfoExpr ?? (_thisFacetSetInfoExpr = CS.MemberAccessExpr(Parent.CSFullExpr, "ThisFacetSetInfo"));
            }
        }
        private bool _isGenerated;
        public void Generate() {
            if (!_isGenerated) {
                _isGenerated = true;
                if (!Parent.NamespaceAncestor.IsCSNamespaceRef) {
                    //>public static readonly FacetSetInfo ThisFacetSetInfo = new ...;
                    Parent.CSMemberList.Add(CS.Field(BaseFacetSet == null ? CS.PublicStaticReadOnlyTokenList : CS.NewPublicStaticReadOnlyTokenList,
                    CSEX.FacetSetInfoName, "ThisFacetSetInfo", CSEX.FacetSetInfo(this)));
                }
            }
        }
    }
    public struct EnumInfoEx {
        public EnumInfoEx(List<EnumItemInfo> itemList, string text) {
            ItemList = itemList;
            Text = text;
        }
        public readonly List<EnumItemInfo> ItemList;
        public readonly string Text;
    }
    public struct EnumItemInfo {
        public EnumItemInfo(object value, string nameOrText) {
            Value = value;
            Name = nameOrText;
        }
        public readonly object Value;
        public readonly string Name;//opt
    }
    #endregion facets
    internal class SimpleTypeSymbol : TypeSymbol {
        public SimpleTypeSymbol(NamespaceSymbol parent, string csName, bool isAbstract, bool isSealed,
            NameSyntax csBaseFullName, NameSyntax[] csItfNames, FullName fullName, TypeKind kind, SimpleTypeSymbol baseType)
            : base(parent, csName, isAbstract, isSealed, csBaseFullName, csItfNames, fullName, kind, baseType) {
        }
        public FacetSetSymbol Facets;
        protected override void GenerateMembers(List<MemberDeclarationSyntax> list) {
            throw new NotImplementedException();
        }
    }
    internal sealed class AtomTypeSymbol : SimpleTypeSymbol {
        public AtomTypeSymbol(NamespaceSymbol parent, string csName, bool isAbstract, bool isSealed,
            FullName fullName, TypeKind kind, SimpleTypeSymbol baseType, TypeSyntax valueCSFullName, bool canBeConst)
            : base(parent, csName, isAbstract, isSealed, baseType.CSFullName, null, fullName, kind, baseType) {
            ValueCSFullName = valueCSFullName;
            CanBeConst = canBeConst;
        }
        public readonly TypeSyntax ValueCSFullName;
        public readonly bool CanBeConst;
        protected override void GenerateMembers(List<MemberDeclarationSyntax> list) {
            if (!IsAbstract) {
                //>public static implicit operator CLASS(VALUE value){
                //  return new CLASS{Value=value};
                //}
                list.Add(CS.ConversionOperator(true, CSFullName, new[] { CS.Parameter(ValueCSFullName, "value") },
                    CS.ReturnStm(CS.NewObjExpr(CSFullName, null, new[] { CS.AssignExpr(CS.IdName("Value"), CS.IdName("value")) }))));
            }
            if (Facets != null) {
                if (Facets.Enum != null) {
                    foreach (var item in Facets.Enum.Value.ItemList) {
                        if (item.Name != null) {
                            //>public static readonly ValueCSFullName E_Name = ...;
                            //>public const ValueCSFullName E_Name = ...;
                            list.Add(CS.Field(CanBeConst ? CS.PublicConstTokenList : CS.PublicStaticReadOnlyTokenList,
                                ValueCSFullName, "E_" + item.Name, CSEX.AtomValue(item.Value)));
                        }
                    }
                }
                Facets.Generate();
            }
            //>>new public static readonly AtomTypeInfo ThisInfo = ...;
            list.Add(CS.Field(CS.NewPublicStaticReadOnlyTokenList, CSEX.AtomTypeInfoName, "ThisInfo",
                CS.NewObjExpr(CSEX.AtomTypeInfoName, CS.TypeOfExpr(CSFullName), CS.Literal(IsAbstract), CSEX.FullName(FullName),
                BaseType.ThisInfoExpr, Facets == null ? CS.NullLiteral : Facets.ThisFacetSetInfoExpr, CSEX.TypeKind(Kind))));
            list.Add(CSEX.ObjectInfoProperty(IsAbstract, CSFullName));
        }
    }
    internal sealed class ListTypeSymbol : SimpleTypeSymbol {
        public ListTypeSymbol(NamespaceSymbol parent, string csName, bool isAbstract, bool isSealed,
            FullName fullName, SimpleTypeSymbol baseType, NameSyntax csBaseFullName, NameSyntax[] csItfNames, SimpleTypeSymbol itemType)
            : base(parent, csName, isAbstract, isSealed, csBaseFullName, csItfNames, fullName, TypeKind.ListType, baseType) {
            ItemType = itemType;
        }
        public readonly SimpleTypeSymbol ItemType;
        protected override void GenerateMembers(List<MemberDeclarationSyntax> list) {
            //>public CSFullName AddEx(ITEM item){
            //> base.Add(item);
            //> return this;
            //}
            list.Add(CS.Method(CSItfNames == null ? CS.NewPublicTokenList : CS.PublicTokenList, CSFullName, "AddEx",
                new[] { CS.Parameter(ItemType.CSFullName, "item") },
                CS.ExprStm(CS.InvoExpr(CS.BaseMemberAccessExpr("Add"), CS.IdName("item"))),
                CS.ReturnStm(SyntaxFactory.ThisExpression())
                ));
            //>public CSFullName AddRange(IEnumerable<ITEM> items){
            //> base.AddRange(items);
            //> return this;
            //}
            list.Add(CS.Method(CSItfNames == null ? CS.NewPublicTokenList : CS.PublicTokenList, CSFullName, "AddRange",
                new[] { CS.Parameter(CS.IEnumerableOf(ItemType.CSFullName), "items") },
                CS.ExprStm(CS.InvoExpr(CS.BaseMemberAccessExpr("AddRange"), CS.IdName("items"))),
                CS.ReturnStm(SyntaxFactory.ThisExpression())
                ));
            if (CSItfNames != null) {
                CSEX.IListOverrideMembers(list, ItemType.CSFullName);
            }

            if (Facets != null) {
                Facets.Generate();
            }
            //>>new public static readonly ListTypeInfo ThisInfo = ...;
            list.Add(CS.Field(CS.NewPublicStaticReadOnlyTokenList, CSEX.ListTypeInfoName, "ThisInfo",
                CS.NewObjExpr(CSEX.ListTypeInfoName, CS.TypeOfExpr(CSFullName), CS.Literal(IsAbstract), CSEX.FullName(FullName),
                BaseType.ThisInfoExpr, Facets == null ? CS.NullLiteral : Facets.ThisFacetSetInfoExpr, ItemType.ThisInfoExpr)));
            list.Add(CSEX.ObjectInfoProperty(IsAbstract, CSFullName));
        }
    }
    internal sealed class ComplexTypeSymbol : TypeSymbol {
        public ComplexTypeSymbol(NamespaceSymbol parent, string csName, bool isAbstract, bool isSealed,
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
        public ChildStructSymbol ComplexChildren {
            get {
                return Children as ChildStructSymbol;
            }
        }
        protected override void GenerateMembers(List<MemberDeclarationSyntax> list) {
            if (Attributes != null) {
                Attributes.Generate();
            }
            if (Children != null) {
                var simpleType = Children as SimpleTypeSymbol;
                if (simpleType != null) {
                    //>new public XXX Children {
                    //    get { return base.GenericChildren as XXX; }
                    //    set { base.GenericChildren = value; }
                    //}
                    list.Add(CS.Property(CS.NewPublicTokenList, simpleType.CSFullName, "Children", false,
                        default(SyntaxTokenList), new[] { CS.ReturnStm(CS.AsExpr(CS.BaseMemberAccessExpr("GenericChildren"), simpleType.CSFullName)) },
                        default(SyntaxTokenList), new[] { CS.AssignStm(CS.BaseMemberAccessExpr("GenericChildren"), CS.IdName("value")) }));
                }
                else {
                    Children.Generate();
                }
            }
            //>>new public static readonly ComplexTypeInfo ThisInfo = ...;
            list.Add(CS.Field(CS.NewPublicStaticReadOnlyTokenList, CSEX.ComplexTypeInfoName, "ThisInfo",
                CS.NewObjExpr(CSEX.ComplexTypeInfoName, CS.TypeOfExpr(CSFullName), CS.Literal(IsAbstract), CSEX.FullName(FullName),
                BaseType.ThisInfoExpr,
                Attributes == null ? CS.NullLiteral : Attributes.ThisInfoExpr,
                Children == null ? CS.NullLiteral : Children.ThisInfoExpr)));
            list.Add(CSEX.ObjectInfoProperty(IsAbstract, CSFullName));
        }
    }
    internal sealed class AttributeSetSymbol : ObjectSymbol {
        public AttributeSetSymbol(ComplexTypeSymbol parent, AttributeSetSymbol baseAttributeSet, string displayName) :
            base(parent, "CLS_Attributes", false, false, baseAttributeSet != null,
                baseAttributeSet != null ? baseAttributeSet.CSFullName : CSEX.XAttributeSetName, null, displayName) {
            BaseAttributeSet = baseAttributeSet;
            AttributeList = new List<AttributeSymbol>();
            AttributeNameList = new List<string>();
            if (baseAttributeSet != null) {
                AttributeList.AddRange(baseAttributeSet.AttributeList);
                AttributeNameList.AddRange(baseAttributeSet.AttributeNameList);
            }
        }
        public readonly AttributeSetSymbol BaseAttributeSet;//opt
        public readonly List<AttributeSymbol> AttributeList;
        public readonly List<string> AttributeNameList;
        protected override void GenerateMembers(List<MemberDeclarationSyntax> list) {
            if (AttributeList != null) {
                foreach (var attribute in AttributeList) {
                    attribute.Generate();
                }
            }
            //>>new public static readonly AttributeSetInfo ThisInfo = ...;
            list.Add(CS.Field(BaseAttributeSet == null ? CS.PublicStaticReadOnlyTokenList : CS.NewPublicStaticReadOnlyTokenList,
                CSEX.AttributeSetInfoName, "ThisInfo", CS.NewObjExpr(CSEX.AttributeSetInfoName, CS.TypeOfExpr(CSFullName), CS.Literal(DisplayName),
                AttributeList == null ? CS.NullLiteral : CS.NewArrOrNullExpr(CSEX.AttributeInfoArrayType, AttributeList.Select(i => i.ThisInfoExpr)))));
            list.Add(CSEX.ObjectInfoProperty(IsAbstract, CSFullName));
            //
            //parent: complex type
            var plist = Parent.CSMemberList;
            //>new public XXX Attributes {
            //    get { return base.GenericAttributes as XXX; }
            //    set { base.GenericAttributes = value; }
            //}
            plist.Add(CS.Property(CS.NewPublicTokenList, CSFullName, "Attributes", false,
                default(SyntaxTokenList), new[] { CS.ReturnStm(CS.AsExpr(CS.BaseMemberAccessExpr("GenericAttributes"), CSFullName)) },
                default(SyntaxTokenList), new[] { CS.AssignStm(CS.BaseMemberAccessExpr("GenericAttributes"), CS.IdName("value")) }));
            //>new public XXX EnsureAttributes(bool @try = false){
            //  return base.EnsureAttributes<XXX>(@try);
            //}
            plist.Add(CS.Method(CS.NewPublicTokenList, CSFullName, "EnsureAttributes",
                new[] { CS.Parameter(CS.BoolType, "@try", CS.FalseLiteral) },
                CS.ReturnStm(CS.InvoExpr(CS.BaseMemberAccessExpr(CS.GenericName("EnsureAttributes", CSFullName)), CS.IdName("@try")))));
        }
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
        protected override void GenerateMembers(List<MemberDeclarationSyntax> list) {
            Type.GenerateConcreteType(list);
            //>new public static readonly AttributeInfo ThisInfo = ...;
            list.Add(CS.Field(RestrictedAttribute == null ? CS.PublicStaticReadOnlyTokenList : CS.NewPublicStaticReadOnlyTokenList,
                CSEX.AttributeInfoName, "ThisInfo", CS.NewObjExpr(CSEX.AttributeInfoName, CS.TypeOfExpr(CSFullName), CS.Literal(DisplayName),
                CS.Literal(Name), CS.Literal(IsOptional), CS.Literal(IsNullable), Type.ThisInfoExpr)));
            list.Add(CSEX.ObjectInfoProperty(IsAbstract, CSFullName));
            //
            //parent: attribute set
            GenerateConcreteAttribute(Parent.CSMemberList);
            //
            //parent parent: complex type
            GenerateConcreteAttribute(Parent.Parent.CSMemberList);
        }
        private void GenerateConcreteAttribute(List<MemberDeclarationSyntax> list) {
            var modifiers = RestrictedAttribute == null ? CS.PublicTokenList : CS.NewPublicTokenList;
            var a_name = "A_" + Name;
            var ensurea_name = "EnsureA_" + Name;
            //public CLS A_NAME{
            //  get{ return base.TryGetAttribute(NAME) as CLS; }
            //  set{ if (value == null) base.RemoveAttribute(NAME); else base.AddOrSetAttribute(value);}
            //}
            list.Add(CS.Property(modifiers, CSFullName, a_name, false,
                default(SyntaxTokenList), new[] { CS.ReturnStm(CS.AsExpr(CS.InvoExpr(CS.BaseMemberAccessExpr("TryGetAttribute"), CS.Literal(Name)), CSFullName)) },
                default(SyntaxTokenList), new[] { CS.IfStm(CS.EqualsExpr(CS.IdName("value"), CS.NullLiteral),
                    CS.ExprStm(CS.InvoExpr(CS.BaseMemberAccessExpr("RemoveAttribute"),  CS.Literal(Name))),
                    CS.ExprStm(CS.InvoExpr(CS.BaseMemberAccessExpr("AddOrSetAttribute"), CS.IdName("value")))) }
                ));
            //public CLS EnsureA_NAME(bool @try = false){
            //  return A_NAME ?? (A_NAME = base.CreateAttribute<CLS>(NAME, @try));
            //}
            list.Add(CS.Method(modifiers, CSFullName, ensurea_name, new[] { CS.Parameter(CS.BoolType, "@try", CS.FalseLiteral) },
                CS.ReturnStm(CS.CoalesceExpr(CS.IdName(a_name),
                    CS.ParedExpr(CS.AssignExpr(CS.IdName(a_name),
                        CS.InvoExpr(CS.BaseMemberAccessExpr(CS.GenericName("CreateAttribute", CSFullName)), CS.Literal(Name), CS.IdName("@try"))))))));
            //public TYPE AT_NAME {
            //  get { var obj = A_NAME; if(obj == null) return null; return obj.Type; }
            //  set { EnsureA_NAME().Type = value; }
            //}
            list.Add(CS.Property(modifiers, Type.CSFullName, "AT_" + Name, false,
                default(SyntaxTokenList), CSEX.NullOrMemberStms(CS.IdName(a_name), "Type"),
                default(SyntaxTokenList), new[] { CS.AssignStm(CS.MemberAccessExpr(CS.InvoExpr(CS.IdName(ensurea_name)), "Type"), CS.IdName("value")) }
                ));

        }
    }

    internal abstract class ChildSymbol : ObjectSymbol {
        protected ChildSymbol(ObjectBaseSymbol parent, string csName, bool isAbstract, bool isSealed, bool isCSOverride,
            NameSyntax csBaseFullName, NameSyntax[] csItfNames, ChildKind kind, string displayName, string memberName,
            ulong minOccurrence, ulong maxOccurrence, bool isListItem, int order, ChildSymbol restrictedChild) :
            base(parent, csName, isAbstract, isSealed, isCSOverride, csBaseFullName, csItfNames, displayName) {
            Kind = kind;
            MemberName = memberName;
            MinOccurrence = minOccurrence;
            MaxOccurrence = maxOccurrence;
            IsListItem = isListItem;
            Order = order;
            RestrictedChild = restrictedChild;
        }
        public readonly ChildKind Kind;
        public readonly string MemberName;
        public readonly ulong MinOccurrence;
        public readonly ulong MaxOccurrence;
        public readonly bool IsListItem;
        public readonly int Order;
        public readonly ChildSymbol RestrictedChild;
        private bool? _atTopLevel;
        public bool AtTopLevel {
            get {
                if (_atTopLevel == null) {
                    var cs = Parent as ChildStructSymbol;
                    _atTopLevel = cs != null && cs.IsRoot;
                }
                return _atTopLevel.Value;
            }
        }
        public bool IsChoiceMember {
            get {
                var cs = Parent as ChildStructSymbol;
                return cs != null && cs.IsChoice;
            }
        }
        public virtual bool IsOptional {
            get {
                return MinOccurrence == 0;
            }
        }
        //public bool HasIntersection(FullName fullName) {
        //    return HasIntersection(fullName, false);
        //}
        //public abstract bool HasIntersection(FullName fullName, bool forChoice);
        protected void GenerateConcreteChild(List<MemberDeclarationSyntax> list) {
            var modifiers = RestrictedChild == null ? CS.PublicTokenList : CS.NewPublicTokenList;
            var c_name = "C_" + MemberName;
            var ensurec_name = "EnsureC_" + MemberName;
            //public CLS C_NAME{
            //  get{ return base.TryGetChild(NAME) as CLS; }
            //  set{ if (value == null) base.RemoveChild(NAME); else base.AddOrSetChild(value);}
            //}
            list.Add(CS.Property(modifiers, CSFullName, c_name, false,
                default(SyntaxTokenList), new[] { CS.ReturnStm(CS.AsExpr(CS.InvoExpr(CS.BaseMemberAccessExpr("TryGetChild"), CS.Literal(Order)), CSFullName)) },
                default(SyntaxTokenList), new[] { CS.IfStm(CS.EqualsExpr(CS.IdName("value"), CS.NullLiteral),
                    CS.ExprStm(CS.InvoExpr(CS.BaseMemberAccessExpr("RemoveChild"),  CS.Literal(Order))),
                    CS.ExprStm(CS.InvoExpr(CS.BaseMemberAccessExpr("AddOrSetChild"), CS.IdName("value")))) }
                ));
            //public CLS EnsureC_NAME(bool @try = false){
            //  return C_NAME ?? (C_NAME = base.CreateChild<CLS>(NAME, @try));
            //}
            list.Add(CS.Method(modifiers, CSFullName, ensurec_name, new[] { CS.Parameter(CS.BoolType, "@try", CS.FalseLiteral) },
                CS.ReturnStm(CS.CoalesceExpr(CS.IdName(c_name),
                    CS.ParedExpr(CS.AssignExpr(CS.IdName(c_name),
                        CS.InvoExpr(CS.BaseMemberAccessExpr(CS.GenericName("CreateChild", CSFullName)), CS.Literal(Order), CS.IdName("@try"))))))));
            GenerateConcreteChildEx(list, modifiers, c_name, ensurec_name);
        }
        protected virtual void GenerateConcreteChildEx(List<MemberDeclarationSyntax> list, SyntaxTokenList modifiers, string c_name, string ensurec_name) { }
    }

    internal sealed class ElementSymbol : ChildSymbol, IGlobalObjectSymbol {
        public ElementSymbol(ObjectBaseSymbol parent, string csName, bool isAbstract, bool isSealed,
            ChildKind kind, string displayName, string memberName, ulong minOccurrence, ulong maxOccurrence, bool isListItem,
            int order, ElementSymbol restrictedElement, FullName fullName, bool isNullable, TypeSymbol type,
            ElementSymbol referencedElement, ElementSymbol substitutedElement
             )
            : base(parent, csName, isAbstract, isSealed, restrictedElement != null,
                 restrictedElement != null ? restrictedElement.CSFullName : substitutedElement != null ? substitutedElement.CSFullName :
                    kind == ChildKind.LocalElement ? CSEX.XLocalElementName : kind == ChildKind.GlobalElementRef ? CSEX.XGlobalElementRefName : CSEX.XGlobalElementName,
                 null, kind, displayName, memberName, minOccurrence, maxOccurrence, isListItem, order, restrictedElement) {
            FullName = fullName;
            IsNullable = isNullable;
            Type = type;
            ReferencedElement = referencedElement;
            SubstitutedElement = substitutedElement;
        }
        public FullName FullName { get; private set; }
        public readonly bool IsNullable;
        public readonly TypeSymbol Type;
        //public ElementSymbol RestrictedElement {
        //    get {
        //        return (ElementSymbol)RestrictedChild;
        //    }
        //}
        public readonly ElementSymbol ReferencedElement;
        public readonly ElementSymbol SubstitutedElement;
        public GlobalElementNode GlobalElementNode;//opt,for global element
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
        public bool AddFullNames(List<FullName> fullNameList, out FullName dupFullName) {
            if (IsLocal) {
                if (fullNameList.Contains(FullName)) {
                    dupFullName = FullName;
                    return false;
                }
                fullNameList.Add(FullName);
            }
            else {//global element ref
                if (!ReferencedElement.GlobalElementNode.AddNonAbstractFullNames(fullNameList, out dupFullName)) {
                    return false;
                }
            }
            dupFullName = default(FullName);
            return true;
        }
        //public override bool HasIntersection(FullName fullName, bool forChoice) {
        //    if (MaxOccurrence > MinOccurrence || forChoice) {
        //        if (IsLocal) {
        //            return FullName == fullName;
        //        }
        //        return ReferencedElement.GlobalElementNode.HasIntersection(fullName);
        //    }
        //    return false;
        //}
        protected override void GenerateMembers(List<MemberDeclarationSyntax> list) {
            Type.GenerateConcreteType(list);
            if (IsGlobal) {
                //>public static bool TryLoadAndValidate(string filePath, System.IO.TextReader reader, DiagContext context, out XXX result) {
                //  return TryLoadAndValidate<XXX>(filePath, reader, context, ThisInfo, out result);
                //}
                list.Add(CS.Method(CS.PublicStaticTokenList, CS.BoolType, "TryLoadAndValidate", new[] {
                    CS.Parameter(CS.StringType, "filePath"), CS.Parameter(CS.TextReaderName, "reader"), CS.Parameter(CSEX.DiagContextName, "context"),
                        CS.OutParameter(CSFullName, "result") },
                    CS.ReturnStm(CS.InvoExpr(CS.GenericName("TryLoadAndValidate", CSFullName),
                        SyntaxFactory.Argument(CS.IdName("filePath")), SyntaxFactory.Argument(CS.IdName("reader")), SyntaxFactory.Argument(CS.IdName("context")),
                        SyntaxFactory.Argument(CS.IdName("ThisInfo")), CS.OutArgument("result")))));
            }
            else if (IsGlobalRef) {
                //>new public XXX GlobalElement {
                //    get { return base.GenericGlobalElement as XXX; }
                //    set { base.GenericGlobalElement = value; }
                //}
                list.Add(CS.Property(CS.NewPublicTokenList, ReferencedElement.CSFullName, "GlobalElement", false,
                     default(SyntaxTokenList), new[] { CS.ReturnStm(CS.AsExpr(CS.BaseMemberAccessExpr("GenericGlobalElement"), ReferencedElement.CSFullName)) },
                     default(SyntaxTokenList), new[] { CS.AssignStm(CS.BaseMemberAccessExpr("GenericGlobalElement"), CS.IdName("value")) }));
            }

            //>new public static readonly ElementInfo ThisInfo = ...;
            list.Add(CS.Field(RestrictedChild == null && SubstitutedElement == null ? CS.PublicStaticReadOnlyTokenList : CS.NewPublicStaticReadOnlyTokenList,
                CSEX.ElementInfoName, "ThisInfo", CS.NewObjExpr(CSEX.ElementInfoName, CS.TypeOfExpr(CSFullName), CS.Literal(IsAbstract),
                CS.Literal(DisplayName), CSEX.ChildKind(Kind), CS.Literal(IsOptional), CS.Literal(Order), CSEX.FullName(FullName),
                CS.Literal(IsNullable), Type.ThisInfoExpr,
                ReferencedElement == null ? CS.NullLiteral : ReferencedElement.ThisInfoExpr,
                SubstitutedElement == null ? CS.NullLiteral : SubstitutedElement.ThisInfoExpr,
                GlobalElementNode == null ? CS.NullLiteral : CS.NewArrOrNullExpr(CSEX.FullNameArrayType,
                    GlobalElementNode.DirectSubstitutorFullNames.Select(i => CSEX.FullName(i))),
                CSEX.XDataProgramInfoInstanceExpr
                )));
            list.Add(CSEX.ObjectInfoProperty(IsAbstract, CSFullName));
            if (!IsGlobal && !IsListItem) {
                //parent
                GenerateConcreteChild(Parent.CSMemberList);
                if (AtTopLevel) {
                    //parent parent
                    GenerateConcreteChild(Parent.Parent.CSMemberList);
                }
            }
        }
        protected override void GenerateConcreteChildEx(List<MemberDeclarationSyntax> list, SyntaxTokenList modifiers,
            string c_name, string ensurec_name) {
            //public TYPE CT_NAME {
            //  get { var obj = C_NAME; if(obj == null) return null; return obj.Type; }
            //  set { EnsureC_NAME().Type = value; }
            //}
            list.Add(CS.Property(modifiers, Type.CSFullName, "CT_" + MemberName, false,
                default(SyntaxTokenList), CSEX.NullOrMemberStms(CS.IdName(c_name), "Type"),
                default(SyntaxTokenList), new[] { CS.AssignStm(CS.MemberAccessExpr(CS.InvoExpr(CS.IdName(ensurec_name)), "Type"), CS.IdName("value")) }
                ));
        }
    }
    internal abstract class ChildContainerSymbol : ChildSymbol {
        protected ChildContainerSymbol(ObjectSymbol parent, string csName, bool isCSOverride, NameSyntax csBaseFullName, NameSyntax[] csItfNames,
            ChildKind kind, string displayName, string memberName, ulong minOccurrence, ulong maxOccurrence, bool isListItem, int order,
            ChildContainerSymbol restrictedChildContainer)
            : base(parent, csName, false, false, isCSOverride, csBaseFullName, csItfNames, kind, displayName, memberName,
                minOccurrence, maxOccurrence, isListItem, order, restrictedChildContainer) {
        }
    }

    internal sealed class ChildStructSymbol : ChildContainerSymbol {
        public ChildStructSymbol(ObjectSymbol parent, string csName, ChildKind kind, string displayName, string memberName,
            ulong minOccurrence, ulong maxOccurrence, bool isListItem, int order,
            ChildStructSymbol restrictedChildStruct, bool isRoot, ChildStructSymbol baseChildStruct)
            : base(parent, csName, restrictedChildStruct != null || baseChildStruct != null,
                 restrictedChildStruct != null ? restrictedChildStruct.CSFullName : baseChildStruct != null ? baseChildStruct.CSFullName :
                    kind == ChildKind.Set ? CSEX.XChildSetName : kind == ChildKind.Sequence ? CSEX.XChildSequenceName : CSEX.XChildChoiceName, null,
                 kind, displayName, memberName, minOccurrence, maxOccurrence, isListItem, order, restrictedChildStruct) {
            IsRoot = isRoot;
            BaseChildStruct = baseChildStruct;
            ChildList = new List<ChildSymbol>();
            if (isRoot) {
                MemberNameList = new List<string>();
                if (IsSet) {
                    FullNameList = new List<FullName>();
                }
                if (baseChildStruct != null) {
                    ChildList.AddRange(baseChildStruct.ChildList);
                    MemberNameList.AddRange(baseChildStruct.MemberNameList);
                    NextChildOrder = baseChildStruct.NextChildOrder;
                    if (IsSet) {
                        FullNameList.AddRange(baseChildStruct.FullNameList);
                    }
                }
            }
        }
        public readonly bool IsRoot;
        public readonly ChildStructSymbol BaseChildStruct;//for root
        public readonly List<string> MemberNameList;//for root
        public readonly List<FullName> FullNameList;//for set
        public int NextChildOrder;//for root
        public readonly List<ChildSymbol> ChildList;
        private bool? _isOptional;
        public override bool IsOptional {
            get {
                if (_isOptional == null) {
                    if (base.IsOptional || ChildList.Count == 0) {
                        _isOptional = true;
                    }
                    else {
                        if (IsSet || IsSequence) {
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
        public bool IsChoice {
            get {
                return Kind == ChildKind.Choice;
            }
        }
        //public override bool HasIntersection(FullName fullName, bool forChoice) {
        //    if (IsSequence) {
        //        if (forChoice) {
        //            foreach (var child in ChildList) {
        //                if (child.HasIntersection(fullName, true)) {
        //                    return true;
        //                }
        //                if (child.MinOccurrence == child.MaxOccurrence) {
        //                    break;
        //                }
        //            }
        //            return false;
        //        }
        //        else {
        //            for (var i = ChildList.Count - 1; i >= 0; --i) {
        //                var child = ChildList[i];
        //                if (child.HasIntersection(fullName, false)) {
        //                    return true;
        //                }
        //                if (child.MinOccurrence == child.MaxOccurrence) {
        //                    return false;
        //                }
        //            }
        //        }
        //    }
        //    else {//choice
        //        foreach (var child in ChildList) {
        //            if (child.HasIntersection(fullName, true)) {
        //                return true;
        //            }
        //        }
        //    }
        //    if (!forChoice) {
        //        var parentChildSet = Parent as ChildSetSymbol;
        //        if (parentChildSet != null) {
        //            return parentChildSet.HasIntersection(fullName, false);
        //        }
        //    }
        //    return false;
        //}
        protected override void GenerateMembers(List<MemberDeclarationSyntax> list) {
            if (ChildList != null) {
                foreach (var child in ChildList) {
                    child.Generate();
                }
            }
            //
            //>>new public static readonly ChildSetInfo ThisInfo = ...;
            list.Add(CS.Field(BaseChildStruct == null && RestrictedChild == null ? CS.PublicStaticReadOnlyTokenList : CS.NewPublicStaticReadOnlyTokenList,
                CSEX.ChildStructInfoName, "ThisInfo", CS.NewObjExpr(CSEX.ChildStructInfoName, CS.TypeOfExpr(CSFullName), CS.Literal(DisplayName),
                CSEX.ChildKind(Kind), CS.Literal(IsOptional), CS.Literal(Order),
                ChildList == null ? CS.NullLiteral : CS.NewArrOrNullExpr(CSEX.ChildInfoArrayType, ChildList.Select(i => i.ThisInfoExpr)))));
            list.Add(CSEX.ObjectInfoProperty(IsAbstract, CSFullName));
            if (IsRoot) {
                //
                //parent: complex type
                var plist = Parent.CSMemberList;
                //>new public XXX Children {
                //    get { return base.GenericChildren as XXX; }
                //    set { base.GenericChildren = value; }
                //}
                plist.Add(CS.Property(CS.NewPublicTokenList, CSFullName, "Children", false,
                    default(SyntaxTokenList), new[] { CS.ReturnStm(CS.AsExpr(CS.BaseMemberAccessExpr("GenericChildren"), CSFullName)) },
                    default(SyntaxTokenList), new[] { CS.AssignStm(CS.BaseMemberAccessExpr("GenericChildren"), CS.IdName("value")) }));
                //>new public XXX EnsureChildren(bool @try = false){
                //  return base.EnsureChildren<XXX>(@try);
                //}
                plist.Add(CS.Method(CS.NewPublicTokenList, CSFullName, "EnsureChildren",
                    new[] { CS.Parameter(CS.BoolType, "@try", CS.FalseLiteral) },
                    CS.ReturnStm(CS.InvoExpr(CS.BaseMemberAccessExpr(CS.GenericName("EnsureChildren", CSFullName)), CS.IdName("@try")))));
            }
            else if (!IsListItem) {
                //parent
                GenerateConcreteChild(Parent.CSMemberList);
                if (AtTopLevel) {
                    //parent parent
                    GenerateConcreteChild(Parent.Parent.CSMemberList);
                }
            }
        }
    }
    internal sealed class ChildListSymbol : ChildContainerSymbol {
        public ChildListSymbol(ChildStructSymbol parent, string csName,
            string displayName, string memberName, ulong minOccurrence, ulong maxOccurrence, int order, ChildListSymbol restrictedChildList,
             ChildSymbol item)
            : base(parent, csName, restrictedChildList != null,
                restrictedChildList != null ? restrictedChildList.CSFullName : CSEX.XChildListOf(item.CSFullName),
                restrictedChildList != null ? CSEX.IListAndIReadOnlyListOf(item.CSFullName) : null, ChildKind.List,
                displayName, memberName, minOccurrence, maxOccurrence, false, order, restrictedChildList) {
            Item = item;
        }
        public readonly ChildSymbol Item;
        //public override bool HasIntersection(FullName fullName, bool forChoice) {
        //    return Item.HasIntersection(fullName, forChoice);
        //}
        protected override void GenerateMembers(List<MemberDeclarationSyntax> list) {
            Item.Generate();
            //>public ITEM CreateItem(){
            //  return base.CreateItem<ITEM>();
            //}
            list.Add(CS.Method(RestrictedChild == null ? CS.PublicTokenList : CS.NewPublicTokenList, Item.CSFullName, "CreateItem", null,
                CS.ReturnStm(CS.InvoExpr(CS.BaseMemberAccessExpr(CS.GenericName("CreateItem", Item.CSFullName))))));
            //>public ITEM CreateAndAddItem(){
            //  return base.CreateAndAddItem<ITEM>();
            //}
            list.Add(CS.Method(RestrictedChild == null ? CS.PublicTokenList : CS.NewPublicTokenList, Item.CSFullName, "CreateAndAddItem", null,
                CS.ReturnStm(CS.InvoExpr(CS.BaseMemberAccessExpr(CS.GenericName("CreateAndAddItem", Item.CSFullName))))));
            //>public CSFullName Add(Action<ITEM> itemSetter){
            //  base.Add<ITEM>(itemSetter); 
            //  return this;
            //}
            list.Add(CS.Method(CS.PublicTokenList, CSFullName, "Add",
                new[] { CS.Parameter(CS.ActionOf(Item.CSFullName), "itemSetter") },
                CS.ExprStm(CS.InvoExpr(CS.BaseMemberAccessExpr(CS.GenericName("Add", Item.CSFullName)), CS.IdName("itemSetter"))),
                CS.ReturnStm(SyntaxFactory.ThisExpression())
                ));
            //>public CSFullName AddRange<TItemValue>(IEnumerable<TItemValue> itemValues, Action<ITEM, TItemValue> itemSetter){
            //  base.AddRange<ITEM, TItemValue>(itemValues, itemSetter);
            //  return this;
            //}
            list.Add(CS.Method(null, CS.PublicTokenList, CSFullName, CS.Id("AddRange"),
                new[] { SyntaxFactory.TypeParameter("TItemValue") },
                new[] { CS.Parameter(CS.IEnumerableOf(CS.IdName("TItemValue")), "itemValues"),
                    CS.Parameter(CS.ActionOf(Item.CSFullName, CS.IdName("TItemValue")), "itemSetter")  },
                null,
                new StatementSyntax[] {
                    CS.ExprStm(CS.InvoExpr(CS.BaseMemberAccessExpr(CS.GenericName("AddRange", Item.CSFullName, CS.IdName("TItemValue"))),
                        CS.IdName("itemValues"), CS.IdName("itemSetter"))),
                    CS.ReturnStm(SyntaxFactory.ThisExpression())
                }));

            if (CSItfNames != null) {
                CSEX.IListOverrideMembers(list, Item.CSFullName);
            }

            //>>new public static readonly ChildListInfo ThisInfo = ...;
            list.Add(CS.Field(RestrictedChild == null ? CS.PublicStaticReadOnlyTokenList : CS.NewPublicStaticReadOnlyTokenList,
                CSEX.ChildListInfoName, "ThisInfo", CS.NewObjExpr(CSEX.ChildListInfoName, CS.TypeOfExpr(CSFullName), CS.Literal(DisplayName),
                CS.Literal(IsOptional), CS.Literal(Order), CS.Literal(MinOccurrence), CS.Literal(MaxOccurrence), Item.ThisInfoExpr)));
            list.Add(CSEX.ObjectInfoProperty(IsAbstract, CSFullName));
            //parent
            GenerateConcreteChild(Parent.CSMemberList);
            if (AtTopLevel) {
                //parent parent
                GenerateConcreteChild(Parent.Parent.CSMemberList);
            }

        }
    }


}