using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using XData.IO.Text;

namespace XData.Compiler {
    internal sealed class CSharpNamespaceNameNode : List<string>, IEquatable<CSharpNamespaceNameNode> {
        public TextSpan TextSpan;
        public bool Equals(CSharpNamespaceNameNode other) {
            if ((object)this == (object)other) return true;
            if ((object)other == null) return false;
            var count = Count;
            if (count != other.Count) {
                return false;
            }
            for (var i = 0; i < count; i++) {
                if (this[i] != other[i]) {
                    return false;
                }
            }
            return true;
        }
        public override bool Equals(object obj) {
            return Equals(obj as CSharpNamespaceNameNode);
        }
        public override int GetHashCode() {
            var hash = 17;
            var count = Math.Min(Count, 5);
            for (var i = 0; i < count; i++) {
                hash = EX.AggregateHash(hash, this[i].GetHashCode());
            }
            return hash;
        }
        public static bool operator ==(CSharpNamespaceNameNode left, CSharpNamespaceNameNode right) {
            if ((object)left == null) {
                return (object)right == null;
            }
            return left.Equals(right);
        }
        public static bool operator !=(CSharpNamespaceNameNode left, CSharpNamespaceNameNode right) {
            return !(left == right);
        }
        private string _string;
        public override string ToString() {
            if (_string == null) {
                var sb = EX.AcquireStringBuilder();
                for (var i = 0; i < Count; ++i) {
                    if (i > 0) {
                        sb.Append('.');
                    }
                    sb.Append(this[i]);
                }
                _string = sb.ToStringAndRelease();
            }
            return _string;
        }
        //
        private NameSyntax _csNonGlobalFullName;//@NS1.NS2
        internal NameSyntax CSNonGlobalFullName {
            get {
                if (_csNonGlobalFullName == null) {
                    foreach (var item in this) {
                        if (_csNonGlobalFullName == null) {
                            _csNonGlobalFullName = CS.IdName(item.EscapeId());
                        }
                        else {
                            _csNonGlobalFullName = CS.QualifiedName(_csNonGlobalFullName, item.EscapeId());
                        }
                    }
                }
                return _csNonGlobalFullName;
            }
        }
        private NameSyntax _csFullName;//global::@NS1.NS2
        internal NameSyntax CSFullName {
            get {
                if (_csFullName == null) {
                    foreach (var item in this) {
                        if (_csFullName == null) {
                            _csFullName = CS.GlobalAliasQualifiedName(item.EscapeId());
                        }
                        else {
                            _csFullName = CS.QualifiedName(_csFullName, item.EscapeId());
                        }
                    }
                }
                return _csFullName;
            }
        }
        private ExpressionSyntax _csFullExpr;//global::@NS1.NS2
        internal ExpressionSyntax CSFullExpr {
            get {
                if (_csFullExpr == null) {
                    foreach (var item in this) {
                        if (_csFullExpr == null) {
                            _csFullExpr = CS.GlobalAliasQualifiedName(item.EscapeId());
                        }
                        else {
                            _csFullExpr = CS.MemberAccessExpr(_csFullExpr, item.EscapeId());
                        }
                    }
                }
                return _csFullExpr;
            }
        }
    }
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
        internal static MemberAccessExpressionSyntax XDataProgramInfoInstanceExpr {
            get { return CS.MemberAccessExpr(XDataProgramInfoName, "Instance"); }
        }
        internal static QualifiedNameSyntax ProgramInfoName {
            get { return CS.QualifiedName(XDataName, "ProgramInfo"); }
        }
        internal static QualifiedNameSyntax NamespaceInfoName {
            get { return CS.QualifiedName(XDataName, "NamespaceInfo"); }
        }
        //internal static ArrayTypeSyntax NamespaceInfoArrayType {
        //    get { return CS.OneDimArrayType(NamespaceInfoName); }
        //}
        internal static QualifiedNameSyntax IGlobalObjectInfoName {
            get { return CS.QualifiedName(XDataName, "IGlobalObjectInfo"); }
        }
        internal static ArrayTypeSyntax IGlobalObjectInfoNameArrayType {
            get { return CS.OneDimArrayType(IGlobalObjectInfoName); }
        }


        internal static QualifiedNameSyntax ObjectInfoName {
            get { return CS.QualifiedName(XDataName, "ObjectInfo"); }
        }
        //global::XData.Extensions
        internal static MemberAccessExpressionSyntax ExtensionsExpr {
            get { return CS.MemberAccessExpr(XDataName, "Extensions"); }
        }

        internal static PropertyDeclarationSyntax ObjectInfoProperty(bool isAbstract, NameSyntax csFullName) {
            //>public override ObjectInfo ObjectInfo { 
            //  get { 
            //    global::XData.Extensions.PublicParameterlessConstructorRequired<CSFullName>();
            //    return ThisInfo;
            //  }
            //}
            StatementSyntax[] stms;
            if (isAbstract) {
                stms = new[] { CS.ReturnStm(CS.IdName("ThisInfo")) };
            }
            else {
                stms = new StatementSyntax[] {
                    CS.ExprStm(CS.InvoExpr(CS.MemberAccessExpr(ExtensionsExpr, CS.GenericName("PublicParameterlessConstructorRequired", csFullName)))),
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
        internal static ArrayTypeSyntax FullNameArrayType {
            get { return CS.OneDimArrayType(FullNameName); }
        }
        internal static ExpressionSyntax FullName(FullName value) {
            var uri = value.Uri;
            return CS.NewObjExpr(FullNameName, string.IsNullOrEmpty(uri) ? CS.NullLiteral : CS.Literal(uri), CS.Literal(value.Name));
        }
        internal static QualifiedNameSyntax TypeKindName {
            get { return CS.QualifiedName(XDataName, "TypeKind"); }
        }
        internal static ExpressionSyntax TypeKind(TypeKind value) {
            return CS.CastExpr(TypeKindName, CS.Literal((int)value));
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
        internal static QualifiedNameSyntax ChildKindName {
            get { return CS.QualifiedName(XDataName, "ChildKind"); }
        }
        internal static ExpressionSyntax ChildKind(ChildKind value) {
            return CS.CastExpr(ChildKindName, CS.Literal((int)value));
        }
        internal static QualifiedNameSyntax ElementInfoName {
            get { return CS.QualifiedName(XDataName, "ElementInfo"); }
        }
        internal static QualifiedNameSyntax ChildSetInfoName {
            get { return CS.QualifiedName(XDataName, "ChildSetInfo"); }
        }
        internal static QualifiedNameSyntax ChildInfoName {
            get { return CS.QualifiedName(XDataName, "ChildInfo"); }
        }
        internal static ArrayTypeSyntax ChildInfoType {
            get { return CS.OneDimArrayType(ChildInfoName); }
        }
        internal static QualifiedNameSyntax ChildListInfoName {
            get { return CS.QualifiedName(XDataName, "ChildListInfo"); }
        }

        //
        //
        internal static QualifiedNameSyntax DiagContextName {
            get { return CS.QualifiedName(XDataName, "DiagContext"); }
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
