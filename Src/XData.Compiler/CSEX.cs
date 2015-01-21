using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;

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
        internal static QualifiedNameSyntax ContextName {
            get { return CS.QualifiedName(XDataName, "Context"); }
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
        internal static QualifiedNameSyntax XChildSetName {
            get { return CS.QualifiedName(XDataName, "XChildSet"); }
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

    }
}
