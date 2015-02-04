using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace XData.Compiler {
    internal static class CSEX {
        internal static string ToClassName(this TypeKind kind) {
            return "X" + kind.ToString();
        }
        //global::XData
        internal static AliasQualifiedNameSyntax XDataName {
            get { return CS.GlobalAliasQualifiedName("XData"); }
        }
        //global::XDataProgramInfo
        internal static AliasQualifiedNameSyntax XDataProgramInfoName {
            get { return CS.GlobalAliasQualifiedName("XDataProgramInfo"); }
        }
        //global::XDataProgramInfo.Instance
        internal static MemberAccessExpressionSyntax XDataProgramInfoInstanceExp {
            get { return CS.MemberAccessExpr(XDataProgramInfoName, "Instance"); }
        }
        internal static QualifiedNameSyntax ProgramInfoName {
            get { return CS.QualifiedName(XDataName, "ProgramInfo"); }
        }
        internal static QualifiedNameSyntax NamespaceInfoName {
            get { return CS.QualifiedName(XDataName, "NamespaceInfo"); }
        }
        internal static ArrayTypeSyntax NamespaceInfoArrayType {
            get { return CS.OneDimArrayType(NamespaceInfoName); }
        }
        internal static QualifiedNameSyntax ObjectInfoName {
            get { return CS.QualifiedName(XDataName, "ObjectInfo"); }
        }
        //global::XData.Extensions
        internal static MemberAccessExpressionSyntax ExtensionsExp {
            get { return CS.MemberAccessExpr(XDataName, "Extensions"); }
        }

        //>public override ObjectInfo ObjectInfo { 
        //  get { 
        //    global::XData.Extensions.PublicParameterlessConstructorRequired<CSFullName>();
        //    return ThisInfo;
        //  }
        //}
        internal static PropertyDeclarationSyntax ObjectInfoProperty(bool isAbstract, NameSyntax csFullName) {
            StatementSyntax[] stms;
            if (isAbstract) {
                stms = new[] { CS.ReturnStm(CS.IdName("ThisInfo")) };
            }
            else {
                stms = new StatementSyntax[] {
                    CS.ExprStm(CS.InvoExpr(CS.MemberAccessExpr(ExtensionsExp, CS.GenericName("PublicParameterlessConstructorRequired", csFullName)))),
                    CS.ReturnStm(CS.IdName("ThisInfo"))
                };
            }
            return CS.Property(CS.PublicOverrideTokenList, ObjectInfoName, "ObjectInfo", true,
                default(SyntaxTokenList), stms);
        }

        #region facets
        internal static QualifiedNameSyntax FacetSetInfoName {
            get { return CS.QualifiedName(XDataName, "FacetSetInfo"); }
        }
        internal static QualifiedNameSyntax ValueBoundaryInfoName {
            get { return CS.QualifiedName(XDataName, "ValueBoundaryInfo"); }
        }
        internal static QualifiedNameSyntax EnumInfoName {
            get { return CS.QualifiedName(XDataName, "EnumInfo"); }
        }
        internal static QualifiedNameSyntax PatternInfoName {
            get { return CS.QualifiedName(XDataName, "PatternInfo"); }
        }
        internal static ArrayTypeSyntax PatternInfoArrayType {
            get { return CS.OneDimArrayType(PatternInfoName); }
        }
        internal static ExpressionSyntax FacetSetInfo(FacetSetSymbol value) {
            return CS.NewObjExpr(FacetSetInfoName, CS.Literal(value.MinLength), CS.Literal(value.MaxLength),
                CS.Literal(value.Precision), CS.Literal(value.Scale),
                ValueBoundaryInfo(value.MinValue), ValueBoundaryInfo(value.MaxValue),
                EnumInfo(value.Enum), PatternInfoArray(value.PatternList)
                );
        }
        internal static ExpressionSyntax ValueBoundaryInfo(ValueBoundaryInfo? value) {
            if (value == null) return CS.NullLiteral;
            var v = value.Value;
            return CS.NewObjExpr(ValueBoundaryInfoName, AtomValue(v.Value), CS.Literal(v.Text),
                CS.Literal(v.IsInclusive));
        }
        internal static ExpressionSyntax EnumInfo(EnumInfoEx? value) {
            if (value == null) return CS.NullLiteral;
            var v = value.Value;
            return CS.NewObjExpr(EnumInfoName, CS.NewArrExpr(CS.ObjectArrayType,
                v.ItemList.Select(i => AtomValue(i.Value))), CS.Literal(v.Text));
        }
        internal static ExpressionSyntax PatternInfoArray(List<string> value) {
            if (value == null) return CS.NullLiteral;
            return CS.NewArrExpr(PatternInfoArrayType, value.Select(i => CS.NewObjExpr(PatternInfoName, CS.Literal(i))));
        }
        internal static ExpressionSyntax AtomValue(object value) {
            if (value == null) return CS.NullLiteral;
            switch (Type.GetTypeCode(value.GetType())) {
                case TypeCode.String: return CS.Literal((string)value);
                case TypeCode.Boolean: return CS.Literal((bool)value);
                case TypeCode.Decimal: return CS.Literal((decimal)value);
                case TypeCode.Int64: return CS.Literal((long)value);
                case TypeCode.Int32: return CS.Literal((int)value);
                case TypeCode.Int16: return CS.Literal((short)value);
                case TypeCode.SByte: return CS.Literal((sbyte)value);
                case TypeCode.UInt64: return CS.Literal((ulong)value);
                case TypeCode.UInt32: return CS.Literal((uint)value);
                case TypeCode.UInt16: return CS.Literal((ushort)value);
                case TypeCode.Byte: return CS.Literal((byte)value);
                case TypeCode.Double: return CS.Literal((double)value);
                case TypeCode.Single: return CS.Literal((float)value);
            }
            var bytes = value as byte[];
            if (bytes != null) {
                return CS.Literal(bytes);
            }
            if (value is Guid) {
                return CS.Literal((Guid)value);
            }
            if (value is DateTimeOffset) {
                return CS.Literal((DateTimeOffset)value);
            }
            if (value is TimeSpan) {
                return CS.Literal((TimeSpan)value);
            }
            throw new InvalidOperationException("Invalid value");
        }
        #endregion facets
        internal static QualifiedNameSyntax FullNameName {
            get { return CS.QualifiedName(XDataName, "FullName"); }
        }
        internal static ExpressionSyntax FullName(FullName value) {
            return CS.NewObjExpr(FullNameName, CS.Literal(value.Uri), CS.Literal(value.Name));
        }
        internal static QualifiedNameSyntax TypeKindName {
            get { return CS.QualifiedName(XDataName, "TypeKind"); }
        }
        internal static ExpressionSyntax TypeKind(TypeKind value) {
            return SyntaxFactory.CastExpression(TypeKindName, CS.Literal((int)value));
        }
        internal static QualifiedNameSyntax AtomTypeInfoName {
            get { return CS.QualifiedName(XDataName, "AtomTypeInfo"); }
        }
        internal static QualifiedNameSyntax ListTypeInfoName {
            get { return CS.QualifiedName(XDataName, "ListTypeInfo"); }
        }
        internal static QualifiedNameSyntax ComplexTypeInfoName {
            get { return CS.QualifiedName(XDataName, "ComplexTypeInfo"); }
        }
        internal static QualifiedNameSyntax AttributeSetInfoName {
            get { return CS.QualifiedName(XDataName, "AttributeSetInfo"); }
        }
        internal static QualifiedNameSyntax AttributeInfoName {
            get { return CS.QualifiedName(XDataName, "AttributeInfo"); }
        }
        internal static ArrayTypeSyntax AttributeInfoArrayType {
            get { return CS.OneDimArrayType(AttributeInfoName); }
        }


        internal static QualifiedNameSyntax ContextName {
            get { return CS.QualifiedName(XDataName, "Context"); }
        }
        internal static QualifiedNameSyntax XComplexTypeName {
            get { return CS.QualifiedName(XDataName, "XComplexType"); }
        }
        internal static QualifiedNameSyntax XAttributeName {
            get { return CS.QualifiedName(XDataName, "XAttribute"); }
        }
        internal static QualifiedNameSyntax XAttributeSetName {
            get { return CS.QualifiedName(XDataName, "XAttributeSet"); }
        }
        internal static QualifiedNameSyntax XLocalElementName {
            get { return CS.QualifiedName(XDataName, "XLocalElement"); }
        }
        internal static QualifiedNameSyntax XGlobalElementName {
            get { return CS.QualifiedName(XDataName, "XGlobalElement"); }
        }
        internal static QualifiedNameSyntax XGlobalElementRefName {
            get { return CS.QualifiedName(XDataName, "XGlobalElementRef"); }
        }
        internal static QualifiedNameSyntax XChildSequenceName {
            get { return CS.QualifiedName(XDataName, "XChildSequence"); }
        }
        internal static QualifiedNameSyntax XChildChoiceName {
            get { return CS.QualifiedName(XDataName, "XChildChoice"); }
        }
        internal static QualifiedNameSyntax XChildListOf(TypeSyntax item) {
            return SyntaxFactory.QualifiedName(XDataName, CS.GenericName("XChildList", item));
        }




        //>var obj = objExp; if(obj == null) return null; return obj.memberName;
        internal static StatementSyntax[] NullOrMemberStms(ExpressionSyntax objExp, string memberName) {
            return new StatementSyntax[] {
                CS.LocalDeclStm(CS.VarIdName, "obj", objExp),
                CS.IfStm(CS.EqualsExpr(CS.IdName("obj"), CS.NullLiteral), CS.ReturnStm(CS.NullLiteral)),
                CS.ReturnStm(CS.MemberAccessExpr(CS.IdName("obj"), memberName))
            };
        }
        internal static NameSyntax[] IListAndIReadOnlyListOf(TypeSyntax itemType) {
            return new NameSyntax[] { CS.IListOf(itemType), CS.IReadOnlyListOf(itemType) };
        }
        internal static void IListOverrideMembers(List<MemberDeclarationSyntax> list, TypeSyntax itemType) {
            //>public bool Contains(TYPE item) { return base.Contains(item); }
            list.Add(CS.Method(CS.PublicTokenList, CS.BoolType, "Contains",
                new[] { CS.Parameter(itemType, "item") },
                CS.ReturnStm(CS.InvoExpr(CS.BaseMemberAccessExpr("Contains"), CS.IdName("item")))));
            //>public int IndexOf(TYPE item) { return base.IndexOf(item); }
            list.Add(CS.Method(CS.PublicTokenList, CS.IntType, "IndexOf",
                new[] { CS.Parameter(itemType, "item") },
                CS.ReturnStm(CS.InvoExpr(CS.BaseMemberAccessExpr("IndexOf"), CS.IdName("item")))));
            //>public void Add(TYPE item) { base.Add(item); }
            list.Add(CS.Method(CS.PublicTokenList, CS.VoidType, "Add",
                new[] { CS.Parameter(itemType, "item") },
                CS.ExprStm(CS.InvoExpr(CS.BaseMemberAccessExpr("Add"), CS.IdName("item")))));
            //>public void Insert(int index, TYPE item) { base.Insert(index, item); }
            list.Add(CS.Method(CS.PublicTokenList, CS.VoidType, "Insert",
                new[] { CS.Parameter(CS.IntType, "index"), CS.Parameter(itemType, "item") },
                CS.ExprStm(CS.InvoExpr(CS.BaseMemberAccessExpr("Insert"), CS.IdName("index"), CS.IdName("item")))));
            //>new public TYPE this[int index] {
            //>    get { return base[index] as TYPE; }
            //>    set { base[index] = value; }
            //>}
            list.Add(CS.Indexer(CS.NewPublicTokenList, itemType, new[] { CS.Parameter(CS.IntType, "index") }, false,
                default(SyntaxTokenList), new[] { CS.ReturnStm(CS.AsExpr(CS.BaseElementAccessExpr(CS.IdName("index")), itemType)) },
                default(SyntaxTokenList), new[] { CS.ExprStm(CS.AssignExpr(CS.BaseElementAccessExpr(CS.IdName("index")), CS.IdName("value"))) }));
            //>public bool Remove(TYPE item) { return base.Remove(item); }
            list.Add(CS.Method(CS.PublicTokenList, CS.BoolType, "Remove",
                new[] { CS.Parameter(itemType, "item") },
                CS.ReturnStm(CS.InvoExpr(CS.BaseMemberAccessExpr("Remove"), CS.IdName("item")))));
            //>new public IEnumerator<TYPE> GetEnumerator(){ return GetEnumeratorCore<XInt32>(); }
            list.Add(CS.Method(CS.NewPublicTokenList, CS.IEnumeratorOf(itemType), "GetEnumerator", null, new[] {
                    CS.ReturnStm(CS.InvoExpr(CS.GenericName("GetEnumeratorCore", itemType)))
            }));
            //>public void CopyTo(TYPE[] array, int arrayIndex) { CopyToCore(array, arrayIndex); }
            list.Add(CS.Method(CS.PublicTokenList, CS.VoidType, "CopyTo",
                new[] { CS.Parameter(CS.OneDimArrayType(itemType), "array"), CS.Parameter(CS.IntType, "arrayIndex") },
                CS.ExprStm(CS.InvoExpr(CS.IdName("CopyToCore"), CS.IdName("array"), CS.IdName("arrayIndex")))));


        }
    }
}
