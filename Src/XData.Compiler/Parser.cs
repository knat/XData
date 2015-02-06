using System;
using System.Collections.Generic;
using System.IO;
using XData.IO.Text;

namespace XData.Compiler {
    internal sealed class Parser : ParserBase {
        public const string AbstractKeyword = "abstract";
        public const string AliasKeyword = "alias";
        public const string AsKeyword = "as";
        public const string ElementKeyword = "element";
        public const string EnumKeyword = "enum";
        public const string ExtendsKeyword = "extends";
        public const string ImportKeyword = "import";
        public const string LengthRangeKeyword = "lengthrange";
        public const string ListsKeyword = "lists";
        public const string MemberNameKeyword = "membername";
        public const string NamespaceKeyword = "namespace";
        public const string NullableKeyword = "nullable";
        public const string PatternKeyword = "pattern";
        public const string PrecisionKeyword = "precision";
        public const string RestrictsKeyword = "restricts";
        public const string ScaleKeyword = "scale";
        public const string SealedKeyword = "sealed";
        public const string SubstitutesKeyword = "substitutes";
        public const string TypeKeyword = "type";
        public const string ValueRangeKeyword = "valuerange";
        //
        //
        [ThreadStatic]
        private static Parser _instance;
        public static bool Parse(string filePath, TextReader reader, DiagContext context,
            out CompilationUnitNode result) {
            return (_instance ?? (_instance = new Parser())).CompilationUnit(filePath, reader, context, out result);
        }
        public static bool Parse(string filePath, TextReader reader, DiagContext context,
            out IndicatorCompilationUnitNode result) {
            return (_instance ?? (_instance = new Parser())).IndicatorCompilationUnit(filePath, reader, context, out result);
        }
        private Parser() {
            _uriAliasingGetter = UriAliasing;
            _namespaceGetter = Namespace;
            _indicatorGetter = Indicator;
            _importGetter = Import;
            _namespaceMemberGetter = NamespaceMember;
            _attributeGetter = Attribute;
            _memberChildGetter = MemberChild;
            _abstractOrSealedGetter = AbstractOrSealed;
            _nullableGetter = Nullable;
            _optionalOrDeleteGetter = OptionalOrDelete;
            _memberNameGetter = MemberName;
            _substituteGetter = Substitute;
            _occurrenceGetter = Occurrence;
        }
        private delegate bool NodeGetterWithParent<T>(Node parent, out T node);
        private delegate bool NodeGetterWithParentList<T>(Node parent, List<T> list, out T node);
        private readonly NodeGetterWithList<UriAliasingNode> _uriAliasingGetter;
        private readonly NodeGetterWithParent<NamespaceNode> _namespaceGetter;
        private readonly NodeGetterWithParent<IndicatorNode> _indicatorGetter;
        private readonly NodeGetterWithParentList<ImportNode> _importGetter;
        private readonly NodeGetterWithParentList<NamespaceMemberNode> _namespaceMemberGetter;
        private readonly NodeGetterWithParentList<AttributeNode> _attributeGetter;
        private readonly NodeGetterWithParentList<MemberChildNode> _memberChildGetter;
        private readonly NodeGetter<NameNode> _abstractOrSealedGetter;
        private readonly NodeGetter<TextSpan> _nullableGetter;
        private readonly NodeGetter<OptionalOrDeleteNode> _optionalOrDeleteGetter;
        private readonly NodeGetter<NameNode> _memberNameGetter;
        private readonly NodeGetter<QualifiableNameNode> _substituteGetter;
        private readonly NodeGetter<OccurrenceNode> _occurrenceGetter;
        private bool CompilationUnit(string filePath, TextReader reader, DiagContext context,
            out CompilationUnitNode result) {
            Set(filePath, reader, context);
            try {
                result = new CompilationUnitNode();
                List(_uriAliasingGetter, out result.UriAliasingList);
                List(result, _namespaceGetter, out result.NamespaceList);
                EndOfFileExpected();
                return true;
            }
            catch (ParsingException) { }
            finally {
                Clear();
            }
            result = null;
            return false;
        }
        private bool IndicatorCompilationUnit(string filePath, TextReader reader, DiagContext context,
            out IndicatorCompilationUnitNode result) {
            Set(filePath, reader, context);
            try {
                result = new IndicatorCompilationUnitNode();
                List(_uriAliasingGetter, out result.UriAliasingList);
                List(result, _indicatorGetter, out result.IndicatorList);
                EndOfFileExpected();
                return true;
            }
            catch (ParsingException) { }
            finally {
                Clear();
            }
            result = null;
            return false;
        }
        private bool Namespace(Node parent, out NamespaceNode result) {
            if (Keyword(NamespaceKeyword)) {
                result = new NamespaceNode(parent);
                result.UriNode = UriExpected(result);
                TokenExpected('{');
                List(result, _importGetter, out result.ImportList);
                List(result, _namespaceMemberGetter, out result.MemberList);
                TokenExpected('}');
                return true;
            }
            result = default(NamespaceNode);
            return false;
        }
        private bool Indicator(Node parent, out IndicatorNode result) {
            TextSpan textSpan;
            if (Keyword(NamespaceKeyword, out textSpan)) {
                result = new IndicatorNode(parent) { TextSpan = textSpan };
                result.UriNode = UriExpected(result);
                if (Token('=')) {
                    result.IsRef = false;
                }
                else {
                    TokenExpected('&', "= or & expected.");
                    result.IsRef = true;
                }
                if (!CSharpNamespaceName(out result.CSharpNamespaceName)) {
                    ErrorDiagAndThrow("C# namespace name expected.");
                }
                return true;
            }
            result = default(IndicatorNode);
            return false;
        }
        private bool CSharpNamespaceName(out CSharpNamespaceNameNode result) {
            result = null;
            NameNode name;
            if (Name(out name)) {
                result = new CSharpNamespaceNameNode { TextSpan = name.TextSpan };
                result.Add(name.Value);
                while (true) {
                    if (Token('.')) {
                        result.Add(NameExpected().Value);
                    }
                    else {
                        break;
                    }
                }
            }
            return result != null;
        }
        private void CheckAlias(NameNode alias) {
            if (alias.Value == "sys") {
                ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.AliasSysIsReserved), alias.TextSpan);
            }
        }
        private void CheckUri(AtomValueNode uri) {
            if (uri.Value == Extensions.SystemUri) {
                ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.UriSystemIsReserved), uri.TextSpan);
            }
        }
        private bool UriAliasing(List<UriAliasingNode> list, out UriAliasingNode result) {
            if (Keyword(AliasKeyword)) {
                var uri = StringValueExpected();
                CheckUri(uri);
                KeywordExpected(AsKeyword);
                var alias = NameExpected();
                CheckAlias(alias);
                if (list.CountOrZero() > 0) {
                    foreach (var item in list) {
                        if (item.Alias == alias) {
                            ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.DuplicateUriAlias, alias.ToString()), alias.TextSpan);
                        }
                    }
                }
                result = new UriAliasingNode(uri, alias);
                return true;
            }
            result = default(UriAliasingNode);
            return false;
        }
        private bool Uri(Node parent, out UriNode result) {
            string value = null;
            NameNode alias;
            var stringValue = default(AtomValueNode);
            if (Name(out alias)) {
                var uaList = parent.GetAncestor<CompilationUnitBaseNode>().UriAliasingList;
                if (uaList != null) {
                    foreach (var ua in uaList) {
                        if (ua.Alias == alias) {
                            value = ua.Uri.Value;
                            break;
                        }
                    }
                }
                if (value == null) {
                    ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.InvalidUriAlias, alias.ToString()), alias.TextSpan);
                }
            }
            else if (StringValue(out stringValue)) {
                CheckUri(stringValue);
                value = stringValue.Value;
            }
            if (value != null) {
                result = new UriNode(alias, stringValue, value);
                return true;
            }
            result = default(UriNode);
            return false;
        }
        private UriNode UriExpected(Node parent) {
            UriNode uri;
            if (Uri(parent, out uri)) {
                return uri;
            }
            ErrorDiagAndThrow("Uri expected.");
            return uri;
        }
        private bool Import(Node parent, List<ImportNode> list, out ImportNode result) {
            if (Keyword(ImportKeyword)) {
                var uri = UriExpected(parent);
                var alias = default(NameNode);
                if (Keyword(AsKeyword)) {
                    alias = NameExpected();
                    CheckAlias(alias);
                    if (list.CountOrZero() > 0) {
                        foreach (var item in list) {
                            if (item.Alias == alias) {
                                ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.DuplicateImportAlias, alias.ToString()), alias.TextSpan);
                            }
                        }
                    }
                }
                result = new ImportNode(uri, alias);
                return true;
            }
            result = default(ImportNode);
            return false;
        }
        private bool NamespaceMember(Node parent, List<NamespaceMemberNode> list, out NamespaceMemberNode result) {
            if (!Type(parent, out result)) {
                if (!GlobalElement(parent, out result)) {
                    return false;
                }
            }
            if (list.CountOrZero() > 0) {
                var name = result.Name;
                foreach (var item in list) {
                    if (item.Name == name) {
                        ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.DuplicateNamespaceMember, name.ToString()), name.TextSpan);
                    }
                }
            }
            return true;
        }
        private bool Type(Node parent, out NamespaceMemberNode result) {
            if (Keyword(TypeKeyword)) {
                var type = new TypeNode(parent);
                type.Name = NameExpected();
                Unordered(_abstractOrSealedGetter, out type.AbstractOrSealed, "abstract, sealed or > expected.");
                if (!TypeDirectness(type, out type.Body)) {
                    if (!TypeList(type, out type.Body)) {
                        if (!TypeExtension(type, out type.Body)) {
                            if (!TypeRestriction(type, out type.Body)) {
                                ErrorDiagAndThrow("Type directness, type list, type extension or type restriction expected.");
                            }
                        }
                    }
                }
                result = type;
                return true;
            }
            result = null;
            return false;
        }
        private bool TypeList(Node parent, out TypeBodyNode result) {
            if (Keyword(ListsKeyword)) {
                var list = new TypeListNode(parent);
                list.ItemTypeQName = QualifiableNameExpected();
                Facets(list, out list.Facets);

                result = list;
                return true;
            }
            result = null;
            return false;
        }
        private bool TypeDirectness(Node parent, out TypeBodyNode result) {
            if (PeekToken('[', '{', '$', ';')) {
                var directness = new TypeDirectnessNode(parent);
                if (!AttributesChildren(directness, out directness.AttributesChildren)) {
                    Token(';');
                }
                result = directness;
                return true;
            }
            result = null;
            return false;
        }
        private bool TypeExtension(Node parent, out TypeBodyNode result) {
            if (Keyword(ExtendsKeyword)) {
                var extension = new TypeExtension(parent);
                extension.BaseTypeQName = QualifiableNameExpected();
                AttributesChildren(extension, out extension.AttributesChildren);
                result = extension;
                return true;
            }
            result = null;
            return false;
        }
        private bool TypeRestriction(Node parent, out TypeBodyNode result) {
            if (Keyword(RestrictsKeyword)) {
                var restriction = new TypeRestriction(parent);
                restriction.BaseTypeQName = QualifiableNameExpected();
                if (!AttributesChildren(restriction, out restriction.AttributesChildren)) {
                    Facets(restriction, out restriction.Facets);
                }
                result = restriction;
                return true;
            }
            result = null;
            return false;
        }
        private bool AttributesChildren(Node parent, out AttributesChildrenNode result) {
            if (PeekToken('[', '{', '$')) {
                var attributesChildren = new AttributesChildrenNode(parent);
                Attributes(attributesChildren, out attributesChildren.Attributes);
                if (!ComplexChildren(attributesChildren, out attributesChildren.ComplexChildren)) {
                    if (Token('$')) {
                        attributesChildren.SimpleChildQName = QualifiableNameExpected();
                    }
                }
                result = attributesChildren;
                return true;
            }
            result = null;
            return false;
        }
        #region facets
        private bool Facets(Node parent, out FacetsNode result) {
            TextSpan openBraceTextSpan;
            if (Token((int)TokenKind.DollarOpenBrace, out openBraceTextSpan)) {
                result = new FacetsNode(parent) { OpenBraceTextSpan = openBraceTextSpan };
                bool hasLengthRange = false, hasPrecision = false, hasScale = false, hasValueRange = false,
                    hasEnum = false, hasPattern = false, hasListItemType = false;
                while (true) {
                    var get = false;
                    if (!hasLengthRange) {
                        if (hasLengthRange = LengthRange(out result.LengthRange)) {
                            get = true;
                        }
                    }
                    if (!get && !hasPrecision) {
                        if (hasPrecision = Precision(out result.Precision)) {
                            get = true;
                        }
                    }
                    if (!get && !hasScale) {
                        if (hasScale = Scale(out result.Scale)) {
                            get = true;
                        }
                    }
                    if (!get && !hasValueRange) {
                        if (hasValueRange = ValueRange(out result.ValueRange)) {
                            get = true;
                        }
                    }
                    if (!get && !hasEnum) {
                        if (hasEnum = Enum(out result.Enum)) {
                            get = true;
                        }
                    }
                    if (!get && !hasPattern) {
                        if (hasPattern = Pattern(out result.Pattern)) {
                            get = true;
                        }
                    }
                    if (!get && !hasListItemType) {
                        if (hasListItemType = ListItemType(out result.ListItemTypeQName)) {
                            get = true;
                        }
                    }
                    if (Token('}', out result.CloseBraceTextSpan)) {
                        if (hasPrecision && hasScale) {
                            var p = result.Precision.Value;
                            var s = result.Scale.Value;
                            if (p < s) {
                                ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.ScaleNotLessThanOrEqualToPrecision,
                                     s.ToInvString(), p.ToInvString()), result.Scale.TextSpan);
                            }
                        }
                        return true;
                    }
                    if (!get) {
                        ErrorDiagAndThrow("Lengthrange, valuerange, precision, scale, enum, pattern, list item type or } expected.");
                    }
                }
            }
            result = null;
            return false;
        }
        private bool LengthRange(out IntegerRangeNode<ulong> result) {
            if (Keyword(LengthRangeKeyword)) {
                result = UInt64RangeExpected(false, DiagCodeEx.MaxLengthNotGreaterThanOrEqualToMinLength);
                return true;
            }
            result = default(IntegerRangeNode<ulong>);
            return false;
        }
        private bool Precision(out IntegerNode<byte> result) {
            if (Keyword(PrecisionKeyword)) {
                result = ByteValueExpected();
                return true;
            }
            result = default(IntegerNode<byte>);
            return false;
        }
        private bool Scale(out IntegerNode<byte> result) {
            if (Keyword(ScaleKeyword)) {
                result = ByteValueExpected();
                return true;
            }
            result = default(IntegerNode<byte>);
            return false;
        }
        private bool ValueRange(out ValueRangeNode result) {
            if (Keyword(ValueRangeKeyword)) {
                bool? minIsInclusive = null, maxIsInclusive = null;
                AtomValueNode minValue = default(AtomValueNode), maxValue;
                TextSpan dotDotTextSpan;
                if (Token('(')) {
                    minIsInclusive = false;
                }
                else if (Token('[')) {
                    minIsInclusive = true;
                }
                if (minIsInclusive != null) {
                    minValue = AtomValueExpected();
                }
                TokenExpected((int)TokenKind.DotDot, ".. expected.", out dotDotTextSpan);
                if (AtomValue(out maxValue)) {
                    if (Token(')')) {
                        maxIsInclusive = false;
                    }
                    else {
                        TokenExpected(']', ") or ] expected.");
                        maxIsInclusive = true;
                    }
                }
                else if (minIsInclusive == null) {
                    ErrorDiagAndThrow("Max value expected.");
                }
                result = new ValueRangeNode(minIsInclusive == null ? default(ValueBoundaryNode) : new ValueBoundaryNode(minValue, minIsInclusive.Value),
                    maxIsInclusive == null ? default(ValueBoundaryNode) : new ValueBoundaryNode(maxValue, maxIsInclusive.Value), dotDotTextSpan);
                return true;
            }
            result = default(ValueRangeNode);
            return false;
        }
        private bool Enum(out List<EnumItemNode> result) {
            if (Keyword(EnumKeyword)) {
                var list = new List<EnumItemNode>();
                while (true) {
                    EnumItemNode item;
                    if (EnumItem(list, out item)) {
                        list.Add(item);
                    }
                    else {
                        if (list.Count > 0) {
                            break;
                        }
                        ErrorDiagAndThrow("Enum item expected.");
                    }
                }
                result = list;
                return true;
            }
            result = null;
            return false;
        }
        //protected override bool SimpleValue(out SimpleValueNode result) {
        //    return SimpleValue(default(QualifiableNameNode), out result);
        //}
        private bool EnumItem(List<EnumItemNode> list, out EnumItemNode result) {
            AtomValueNode value;
            if (AtomValue(out value)) {
                NameNode name = default(NameNode);
                if (Keyword(AsKeyword)) {
                    name = NameExpected();
                    if (list.CountOrZero() > 0) {
                        foreach (var item in list) {
                            if (item.Name.IsValid && item.Name == name) {
                                ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.DuplicateEnumItemName, name.ToString()), name.TextSpan);
                            }
                        }
                    }
                }
                result = new EnumItemNode(value, name);
                return true;
            }
            result = default(EnumItemNode);
            return false;
        }
        private bool Pattern(out AtomValueNode result) {
            if (Keyword(PatternKeyword)) {
                result = StringValueExpected();
                return true;
            }
            result = default(AtomValueNode);
            return false;
        }
        private bool ListItemType(out QualifiableNameNode result) {
            if (Keyword(ListsKeyword)) {
                result = QualifiableNameExpected();
                return true;
            }
            result = default(QualifiableNameNode);
            return false;
        }
        private bool UInt64Range(bool minValueRequired, DiagCodeEx diagCode, out IntegerRangeNode<ulong> result) {
            IntegerNode<ulong> minValue, maxValue;
            TextSpan dotdotTextSpan = default(TextSpan);
            if (!UInt64Value(out minValue)) {
                if (minValueRequired || !Token((int)TokenKind.DotDot, out dotdotTextSpan)) {
                    result = default(IntegerRangeNode<ulong>);
                    return false;
                }
            }
            if (minValue.IsValid && !dotdotTextSpan.IsValid) {
                ErrorDiagAndThrow(".. expected.");
            }
            if (UInt64Value(out maxValue)) {
                if (minValue.IsValid) {
                    if (maxValue.Value < minValue.Value) {
                        ErrorDiagAndThrow(new DiagMsgEx(diagCode,
                            maxValue.Value.ToInvString(), minValue.Value.ToInvString()), maxValue.TextSpan);
                    }
                }
            }
            else {
                if (!minValue.IsValid) {
                    ErrorDiagAndThrow("UInt64 value expected.");
                }
            }
            result = new IntegerRangeNode<ulong>(minValue, maxValue, dotdotTextSpan);
            return true;
        }
        private IntegerRangeNode<ulong> UInt64RangeExpected(bool minValueRequired, DiagCodeEx diagCode) {
            IntegerRangeNode<ulong> result;
            if (!UInt64Range(minValueRequired, diagCode, out result)) {
                ErrorDiagAndThrow("UInt64 range expected.");
            }
            return result;
        }
        private bool ByteValue(out IntegerNode<byte> result) {
            AtomValueNode node;
            if (IntegerValue(out node)) {
                byte value;
                if (!node.Value.TryToInvByte(out value)) {
                    ErrorDiagAndThrow("Byte value expected.", node.TextSpan);
                }
                result = new IntegerNode<byte>(node, value);
                return true;
            }
            result = default(IntegerNode<byte>);
            return false;
        }
        private IntegerNode<byte> ByteValueExpected() {
            IntegerNode<byte> result;
            if (!ByteValue(out result)) {
                ErrorDiagAndThrow("Byte value expected.");
            }
            return result;
        }
        private bool UInt64Value(out IntegerNode<ulong> result) {
            AtomValueNode node;
            if (IntegerValue(out node)) {
                ulong value;
                if (!node.Value.TryToInvUInt64(out value)) {
                    ErrorDiagAndThrow("UInt64 value expected.", node.TextSpan);
                }
                result = new IntegerNode<ulong>(node, value);
                return true;
            }
            result = default(IntegerNode<ulong>);
            return false;
        }
        private IntegerNode<ulong> UInt64ValueExpected() {
            IntegerNode<ulong> result;
            if (!UInt64Value(out result)) {
                ErrorDiagAndThrow("UInt64 value expected.");
            }
            return result;
        }
        #endregion facets
        private bool Attributes(Node parent, out AttributesNode result) {
            TextSpan openBracketToken;
            if (Token('[', out openBracketToken)) {
                result = new AttributesNode(parent) { OpenBracketTextSpan = openBracketToken };
                List(result, _attributeGetter, out result.AttributeList);
                TokenExpected(']', out result.CloseBracketTextSpan);
                return true;
            }
            result = null;
            return false;
        }
        private bool Attribute(Node parent, List<AttributeNode> list, out AttributeNode result) {
            NameNode name;
            if (Name(out name)) {
                var attribute = new AttributeNode(parent) { NameNode = name };
                Unordered(_nullableGetter, _optionalOrDeleteGetter,
                    out attribute.Nullable, out attribute.OptionalOrDelete,
                    "Nullable, ?, x, or > expected.");
                if (list.CountOrZero() > 0) {
                    foreach (var item in list) {
                        if (item.NameNode == name) {
                            ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.DuplicateAttributeName, name.ToString()), name.TextSpan);
                        }
                    }
                }
                KeywordExpected(AsKeyword);
                attribute.TypeQName = QualifiableNameExpected();
                result = attribute;
                return true;
            }
            result = null;
            return false;
        }


        private bool GlobalElement(Node parent, out NamespaceMemberNode result) {
            if (Keyword(ElementKeyword)) {
                var element = new GlobalElementNode(parent);
                element.Name = NameExpected();
                Unordered(_abstractOrSealedGetter, _nullableGetter, _substituteGetter,
                    out element.AbstractOrSealed, out element.Nullable, out element.SubstitutedGlobalElementQName,
                    "Abstract, sealed, nullable, substitutes or > expected.");
                KeywordExpected(AsKeyword);
                element.TypeQName = QualifiableNameExpected();
                result = element;
                return true;
            }
            result = null;
            return false;
        }
        private bool ComplexChildren(Node parent, out ComplexChildrenNode result) {
            TextSpan openBraceToken;
            if (Token('{', out openBraceToken)) {
                result = new ComplexChildrenNode(parent) { OpenBraceTextSpan = openBraceToken };
                List(result, _memberChildGetter, out result.ChildList);
                TokenExpected('}', out result.CloseBraceTextSpan);
                return true;
            }
            result = null;
            return false;
        }
        private bool MemberChild(Node parent, List<MemberChildNode> list, out MemberChildNode result) {
            if (!LocalElement(parent, out result)) {
                if (!GlobalElementRef(parent, out result)) {
                    if (!MemberComplexChildren(parent, out result)) {
                        return false;
                    }
                }
            }
            if (list.CountOrZero() > 0) {
                var memberName = result.MemberNameNode;
                foreach (var item in list) {
                    if (item.MemberNameNode == memberName) {
                        ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.DuplicateMemberName, memberName.ToString()), memberName.TextSpan);
                    }
                }
            }
            return true;
        }
        private bool LocalElement(Node parent, out MemberChildNode result) {
            NameNode name;
            if (Name(out name)) {
                var element = new LocalElementNode(parent) { NameNode = name };
                Unordered(_nullableGetter, _occurrenceGetter, _memberNameGetter,
                    out element.Nullable, out element.Occurrence, out element.MemberNameNode,
                    "Nullable, occurrence, membername or > expected.");
                if (!element.MemberNameNode.IsValid) {
                    element.MemberNameNode = name;
                }
                KeywordExpected(AsKeyword);
                element.TypeQName = QualifiableNameExpected();
                result = element;
                return true;
            }
            result = null;
            return false;
        }
        private bool GlobalElementRef(Node parent, out MemberChildNode result) {
            if (Token('&')) {
                var element = new GlobalElementRefNode(parent);
                element.GlobalElementQName = QualifiableNameExpected();
                Unordered(_occurrenceGetter, _memberNameGetter,
                    out element.Occurrence, out element.MemberNameNode, "Occurrence, membername or > expected.");
                if (!element.MemberNameNode.IsValid) {
                    element.MemberNameNode = element.GlobalElementQName.Name;
                }
                result = element;
                return true;
            }
            result = null;
            return false;
        }
        private bool MemberComplexChildren(Node parent, out MemberChildNode result) {
            ChildKind kind = ChildKind.None;
            TextSpan openBraceToken;
            if (Token('{', out openBraceToken)) {
                kind = ChildKind.Sequence;
            }
            else if (Token((int)TokenKind.QuestionOpenBrace, out openBraceToken)) {
                kind = ChildKind.Choice;
            }
            if (kind != ChildKind.None) {
                var complexChildren = new MemberComplexChildrenNode(parent) { Kind = kind, OpenBraceToken = openBraceToken };
                List(complexChildren, _memberChildGetter, out complexChildren.ChildList);
                TokenExpected('}', out complexChildren.CloseBraceToken);
                Unordered(_occurrenceGetter, _memberNameGetter,
                    out complexChildren.Occurrence, out complexChildren.MemberNameNode, "Occurrence, membername or > expected.");
                if (!complexChildren.MemberNameNode.IsValid) {
                    complexChildren.MemberNameNode = new NameNode(kind == ChildKind.Sequence ? "Seq" : "Choice", complexChildren.CloseBraceToken);
                }
                result = complexChildren;
                return true;
            }
            result = null;
            return false;
        }

        private bool AbstractOrSealed(out NameNode result) {
            if (Keyword(AbstractKeyword, out result)) {
                return true;
            }
            return Keyword(SealedKeyword, out result);
        }
        private bool Nullable(out TextSpan result) {
            return Keyword(NullableKeyword, out result);
        }
        private bool OptionalOrDelete(out OptionalOrDeleteNode result) {
            TextSpan optional;
            var delete = default(NameNode);
            if (!Token('?', out optional)) {
                if (!Keyword("x", out delete)) {
                    result = default(OptionalOrDeleteNode);
                    return false;
                }
            }
            result = new OptionalOrDeleteNode(optional, delete);
            return true;
        }
        private bool MemberName(out NameNode result) {
            if (Keyword(MemberNameKeyword)) {
                result = NameExpected();
                return true;
            }
            result = default(NameNode);
            return false;
        }
        private bool Substitute(out QualifiableNameNode result) {
            if (Keyword(SubstitutesKeyword)) {
                result = QualifiableNameExpected();
                return true;
            }
            result = default(QualifiableNameNode);
            return false;
        }
        private bool Occurrence(out OccurrenceNode result) {
            ulong minValue = 0, maxValue = 0;
            TextSpan textSpan;
            if (Token('?', out textSpan)) {
                minValue = 0;
                maxValue = 1;
            }
            else if (Token('*', out textSpan)) {
                minValue = 0;
                maxValue = ulong.MaxValue;
            }
            else if (Token('+', out textSpan)) {
                minValue = 1;
                maxValue = ulong.MaxValue;
            }
            else {
                IntegerRangeNode<ulong> range;
                if (UInt64Range(true, DiagCodeEx.MaxOccurrenceNotEqualToOrGreaterThanMinOccurrence, out range)) {
                    minValue = range.MinValue.Value;
                    if (range.MaxValue.IsValid) {
                        maxValue = range.MaxValue.Value;
                    }
                    else {
                        maxValue = ulong.MaxValue;
                    }
                    textSpan = range.DotDotTextSpan;
                }
            }
            if (textSpan.IsValid) {
                result = new OccurrenceNode(minValue, maxValue, textSpan);
                return true;
            }
            result = default(OccurrenceNode);
            return false;
        }

        private bool Unordered<T>(NodeGetter<T> getter, out T value, string errMsg, int openTokenKind = '<', int closeTokenKind = '>') {
            value = default(T);
            if (Token(openTokenKind)) {
                getter(out value);
                if (Token(closeTokenKind)) {
                    return true;
                }
                ErrorDiagAndThrow(errMsg);
            }
            return false;
        }
        private bool Unordered<T1, T2>(NodeGetter<T1> getter1, NodeGetter<T2> getter2,
            out T1 value1, out T2 value2, string errMsg, int openTokenKind = '<', int closeTokenKind = '>') {
            value1 = default(T1);
            value2 = default(T2);
            if (Token(openTokenKind)) {
                bool hasv1 = false, hasv2 = false;
                while (true) {
                    var get = false;
                    if (!hasv1) {
                        if (hasv1 = getter1(out value1)) {
                            get = true;
                        }
                    }
                    if (!get && !hasv2) {
                        if (hasv2 = getter2(out value2)) {
                            get = true;
                        }
                    }
                    if (Token(closeTokenKind)) {
                        return true;
                    }
                    if (!get) {
                        ErrorDiagAndThrow(errMsg);
                    }
                }
            }
            return false;
        }
        private bool Unordered<T1, T2, T3>(NodeGetter<T1> getter1, NodeGetter<T2> getter2, NodeGetter<T3> getter3,
            out T1 value1, out T2 value2, out T3 value3, string errMsg, int openTokenKind = '<', int closeTokenKind = '>') {
            value1 = default(T1);
            value2 = default(T2);
            value3 = default(T3);
            if (Token(openTokenKind)) {
                bool hasv1 = false, hasv2 = false, hasv3 = false;
                while (true) {
                    var get = false;
                    if (!hasv1) {
                        if (hasv1 = getter1(out value1)) {
                            get = true;
                        }
                    }
                    if (!get && !hasv2) {
                        if (hasv2 = getter2(out value2)) {
                            get = true;
                        }
                    }
                    if (!get && !hasv3) {
                        if (hasv3 = getter3(out value3)) {
                            get = true;
                        }
                    }
                    if (Token(closeTokenKind)) {
                        return true;
                    }
                    if (!get) {
                        ErrorDiagAndThrow(errMsg);
                    }
                }
            }
            return false;
        }
        private bool Unordered<T1, T2, T3, T4>(NodeGetter<T1> getter1, NodeGetter<T2> getter2, NodeGetter<T3> getter3, NodeGetter<T4> getter4,
            out T1 value1, out T2 value2, out T3 value3, out T4 value4, string errMsg, int openTokenKind = '<', int closeTokenKind = '>') {
            value1 = default(T1);
            value2 = default(T2);
            value3 = default(T3);
            value4 = default(T4);
            if (Token(openTokenKind)) {
                bool hasv1 = false, hasv2 = false, hasv3 = false, hasv4 = false;
                while (true) {
                    var get = false;
                    if (!hasv1) {
                        if (hasv1 = getter1(out value1)) {
                            get = true;
                        }
                    }
                    if (!get && !hasv2) {
                        if (hasv2 = getter2(out value2)) {
                            get = true;
                        }
                    }
                    if (!get && !hasv3) {
                        if (hasv3 = getter3(out value3)) {
                            get = true;
                        }
                    }
                    if (!get && !hasv4) {
                        if (hasv4 = getter4(out value4)) {
                            get = true;
                        }
                    }
                    if (Token(closeTokenKind)) {
                        return true;
                    }
                    if (!get) {
                        ErrorDiagAndThrow(errMsg);
                    }
                }
            }
            return false;
        }
        private bool Unordered<T1, T2, T3, T4, T5>(NodeGetter<T1> getter1, NodeGetter<T2> getter2, NodeGetter<T3> getter3, NodeGetter<T4> getter4, NodeGetter<T5> getter5,
            out T1 value1, out T2 value2, out T3 value3, out T4 value4, out T5 value5, string errMsg, int openTokenKind = '<', int closeTokenKind = '>') {
            value1 = default(T1);
            value2 = default(T2);
            value3 = default(T3);
            value4 = default(T4);
            value5 = default(T5);
            if (Token(openTokenKind)) {
                bool hasv1 = false, hasv2 = false, hasv3 = false, hasv4 = false, hasv5 = false;
                while (true) {
                    var get = false;
                    if (!hasv1) {
                        if (hasv1 = getter1(out value1)) {
                            get = true;
                        }
                    }
                    if (!get && !hasv2) {
                        if (hasv2 = getter2(out value2)) {
                            get = true;
                        }
                    }
                    if (!get && !hasv3) {
                        if (hasv3 = getter3(out value3)) {
                            get = true;
                        }
                    }
                    if (!get && !hasv4) {
                        if (hasv4 = getter4(out value4)) {
                            get = true;
                        }
                    }
                    if (!get && !hasv5) {
                        if (hasv5 = getter5(out value5)) {
                            get = true;
                        }
                    }
                    if (Token(closeTokenKind)) {
                        return true;
                    }
                    if (!get) {
                        ErrorDiagAndThrow(errMsg);
                    }
                }
            }
            return false;
        }
        private bool List<T>(Node parent, NodeGetterWithParent<T> nodeGetterWithParent, out List<T> result) {
            result = null;
            T item;
            while (nodeGetterWithParent(parent, out item)) {
                if (result == null) {
                    result = new List<T>();
                }
                result.Add(item);
            }
            return result != null;
        }
        private bool List<T>(Node parent, NodeGetterWithParentList<T> nodeGetterWithParentList, out List<T> result) {
            result = null;
            T item;
            while (nodeGetterWithParentList(parent, result, out item)) {
                if (result == null) {
                    result = new List<T>();
                }
                result.Add(item);
            }
            return result != null;
        }
        private void ErrorDiagAndThrow(DiagMsgEx diagMsg, TextSpan textSpan) {
            _context.AddDiag(DiagSeverity.Error, (int)diagMsg.Code, diagMsg.GetMessage(), textSpan, null);
            throw _parsingException;
        }

    }
}
