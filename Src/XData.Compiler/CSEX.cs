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
        internal static QualifiedNameSyntax ValueFacetSetInfoName {
            get { return CS.QualifiedName(XDataName, "ValueFacetSetInfo"); }
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
        internal static QualifiedNameSyntax XElementName {
            get { return CS.QualifiedName(XDataName, "XElement"); }
        }
        internal static QualifiedNameSyntax XElementReferenceName {
            get { return CS.QualifiedName(XDataName, "XElementReference"); }
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




        internal static QualifiedNameSyntax FullNameName {
            get { return CS.QualifiedName(XDataName, "FullName"); }
        }
        internal static ExpressionSyntax Literal(FullName value) {
            return CS.NewObjExpr(FullNameName, CS.Literal(value.Uri), CS.Literal(value.Name));
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
