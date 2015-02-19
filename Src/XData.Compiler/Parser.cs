using System;
using System.Collections.Generic;
using System.IO;
using XData.IO.Text;

namespace XData.Compiler {
    public static class ParserConstants {
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
        public static readonly HashSet<string> KeywordSet = new HashSet<string>
        {
            AbstractKeyword,
            AliasKeyword,
            AsKeyword,
            ElementKeyword,
            EnumKeyword,
            ExtendsKeyword,
            ImportKeyword,
            LengthRangeKeyword,
            ListsKeyword,
            MemberNameKeyword,
            NamespaceKeyword,
            NullableKeyword,
            PatternKeyword,
            PrecisionKeyword,
            RestrictsKeyword,
            ScaleKeyword,
            SealedKeyword,
            SubstitutesKeyword,
            TypeKeyword,
            ValueRangeKeyword,
            "true",
            "false"
        };
    }
    internal sealed class Parser : ParserBase {
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
            if (Keyword(ParserConstants.NamespaceKeyword)) {
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
            if (Keyword(ParserConstants.NamespaceKeyword, out textSpan)) {
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
                ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.AliasSysReserved), alias.TextSpan);
            }
        }
        private void CheckUri(AtomValueNode uri) {
            if (uri.Value == Extensions.SystemUri) {
                ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.UriSystemReserved), uri.TextSpan);
            }
        }
        private bool UriAliasing(List<UriAliasingNode> list, out UriAliasingNode result) {
            if (Keyword(ParserConstants.AliasKeyword)) {
                var uri = StringValueExpected();
                CheckUri(uri);
                KeywordExpected(ParserConstants.AsKeyword);
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
            NameNode aliasNode;
            var stringNode = default(AtomValueNode);
            if (Name(out aliasNode)) {
                var uaList = parent.GetAncestor<CompilationUnitBaseNode>().UriAliasingList;
                if (uaList != null) {
                    foreach (var ua in uaList) {
                        if (ua.Alias == aliasNode) {
                            value = ua.Uri.Value;
                            break;
                        }
                    }
                }
                if (value == null) {
                    ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.InvalidUriReference, aliasNode.ToString()), aliasNode.TextSpan);
                }
            }
            else if (StringValue(out stringNode)) {
                CheckUri(stringNode);
                value = stringNode.Value;
            }
            if (value != null) {
                result = new UriNode(aliasNode, stringNode, value);
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
            if (Keyword(ParserConstants.ImportKeyword)) {
                var uri = UriExpected(parent);
                var alias = default(NameNode);
                if (Keyword(ParserConstants.AsKeyword)) {
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
                var name = result.NameNode;
                foreach (var item in list) {
                    if (item.NameNode == name) {
                        ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.DuplicateNamespaceMember, name.ToString()), name.TextSpan);
                    }
                }
            }
            return true;
        }
        private bool Type(Node parent, out NamespaceMemberNode result) {
            if (Keyword(ParserConstants.TypeKeyword)) {
                var obj = new TypeNode(parent);
                obj.NameNode = NameExpected();
                Unordered(_abstractOrSealedGetter, out obj.AbstractOrSealed, "abstract, sealed or > expected.");
                if (!TypeDirectness(obj, out obj.Body)) {
                    if (!TypeList(obj, out obj.Body)) {
                        if (!TypeExtension(obj, out obj.Body)) {
                            if (!TypeRestriction(obj, out obj.Body)) {
                                ErrorDiagAndThrow("Type directness, type list, type extension or type restriction expected.");
                            }
                        }
                    }
                }
                result = obj;
                return true;
            }
            result = null;
            return false;
        }
        private bool TypeList(Node parent, out TypeBodyNode result) {
            if (Keyword(ParserConstants.ListsKeyword)) {
                var obj = new TypeListNode(parent);
                obj.ItemTypeQName = QualifiableNameExpected();
                Facets(obj, out obj.Facets);
                result = obj;
                return true;
            }
            result = null;
            return false;
        }
        private bool TypeDirectness(Node parent, out TypeBodyNode result) {
            if (PeekToken('[', '{', (int)TokenKind.HashOpenBrace, '$', ';')) {
                var obj = new TypeDirectnessNode(parent);
                if (!AttributesChildren(obj, out obj.AttributesChildren)) {
                    Token(';');
                }
                result = obj;
                return true;
            }
            result = null;
            return false;
        }
        private bool TypeExtension(Node parent, out TypeBodyNode result) {
            if (Keyword(ParserConstants.ExtendsKeyword)) {
                var obj = new TypeExtension(parent);
                obj.BaseTypeQName = QualifiableNameExpected();
                AttributesChildren(obj, out obj.AttributesChildren);
                result = obj;
                return true;
            }
            result = null;
            return false;
        }
        private bool TypeRestriction(Node parent, out TypeBodyNode result) {
            if (Keyword(ParserConstants.RestrictsKeyword)) {
                var obj = new TypeRestriction(parent);
                obj.BaseTypeQName = QualifiableNameExpected();
                if (!AttributesChildren(obj, out obj.AttributesChildren)) {
                    Facets(obj, out obj.Facets);
                }
                result = obj;
                return true;
            }
            result = null;
            return false;
        }
        private bool AttributesChildren(Node parent, out AttributesChildrenNode result) {
            if (PeekToken('[', '{', (int)TokenKind.HashOpenBrace, '$')) {
                var obj = new AttributesChildrenNode(parent);
                Attributes(obj, out obj.Attributes);
                if (!ComplexChildren(obj, out obj.ComplexChildren)) {
                    if (Token('$')) {
                        obj.SimpleChildQName = QualifiableNameExpected();
                    }
                }
                result = obj;
                return true;
            }
            result = null;
            return false;
        }
        #region facets
        private bool Facets(Node parent, out FacetsNode result) {
            TextSpan openBraceTextSpan;
            if (Token((int)TokenKind.DollarOpenBrace, out openBraceTextSpan)) {
                var obj = new FacetsNode(parent) { OpenBraceTextSpan = openBraceTextSpan };
                bool hasLengthRange = false, hasPrecision = false, hasScale = false, hasValueRange = false,
                    hasEnum = false, hasPattern = false, hasListItemType = false;
                while (true) {
                    var get = false;
                    if (!hasLengthRange) {
                        if (hasLengthRange = LengthRange(out obj.LengthRange)) {
                            get = true;
                        }
                    }
                    if (!get && !hasPrecision) {
                        if (hasPrecision = Precision(out obj.Precision)) {
                            get = true;
                        }
                    }
                    if (!get && !hasScale) {
                        if (hasScale = Scale(out obj.Scale)) {
                            get = true;
                        }
                    }
                    if (!get && !hasValueRange) {
                        if (hasValueRange = ValueRange(out obj.ValueRange)) {
                            get = true;
                        }
                    }
                    if (!get && !hasEnum) {
                        if (hasEnum = Enum(out obj.Enum)) {
                            get = true;
                        }
                    }
                    if (!get && !hasPattern) {
                        if (hasPattern = Pattern(out obj.Pattern)) {
                            get = true;
                        }
                    }
                    if (!get && !hasListItemType) {
                        if (hasListItemType = ListItemType(out obj.ListItemTypeQName)) {
                            get = true;
                        }
                    }
                    if (Token('}', out obj.CloseBraceTextSpan)) {
                        if (hasPrecision && hasScale) {
                            var p = obj.Precision.Value;
                            var s = obj.Scale.Value;
                            if (p < s) {
                                ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.ScaleNotLessThanOrEqualToPrecision,
                                     s.ToInvString(), p.ToInvString()), obj.Scale.TextSpan);
                            }
                        }
                        result = obj;
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
            if (Keyword(ParserConstants.LengthRangeKeyword)) {
                IntegerNode<ulong> minValue, maxValue;
                UInt64Value(out minValue);
                TextSpan dotdotTextSpan;
                TokenExpected((int)TokenKind.DotDot, ".. expected.", out dotdotTextSpan);
                if (UInt64Value(out maxValue)) {
                    if (minValue.IsValid) {
                        if (maxValue.Value < minValue.Value) {
                            ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.MaxLengthNotGreaterThanOrEqualToMinLength,
                                maxValue.Value.ToInvString(), minValue.Value.ToInvString()), maxValue.TextSpan);
                        }
                    }
                }
                else if (!minValue.IsValid) {
                    ErrorDiagAndThrow("Max length expected.");
                }
                result = new IntegerRangeNode<ulong>(minValue, maxValue, dotdotTextSpan);
                return true;
            }
            result = default(IntegerRangeNode<ulong>);
            return false;
        }
        private bool Precision(out IntegerNode<byte> result) {
            if (Keyword(ParserConstants.PrecisionKeyword)) {
                result = ByteValueExpected();
                if (result.Value == 0) {
                    ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.PrecisionCannotBeZero), result.TextSpan);
                }
                return true;
            }
            result = default(IntegerNode<byte>);
            return false;
        }
        private bool Scale(out IntegerNode<byte> result) {
            if (Keyword(ParserConstants.ScaleKeyword)) {
                result = ByteValueExpected();
                return true;
            }
            result = default(IntegerNode<byte>);
            return false;
        }
        private bool ValueRange(out ValueRangeNode result) {
            if (Keyword(ParserConstants.ValueRangeKeyword)) {
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
            if (Keyword(ParserConstants.EnumKeyword)) {
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
                if (Keyword(ParserConstants.AsKeyword)) {
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
            if (Keyword(ParserConstants.PatternKeyword)) {
                result = StringValueExpected();
                return true;
            }
            result = default(AtomValueNode);
            return false;
        }
        private bool ListItemType(out QualifiableNameNode result) {
            if (Keyword(ParserConstants.ListsKeyword)) {
                result = QualifiableNameExpected();
                return true;
            }
            result = default(QualifiableNameNode);
            return false;
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
        //private IntegerNode<ulong> UInt64ValueExpected() {
        //    IntegerNode<ulong> result;
        //    if (!UInt64Value(out result)) {
        //        ErrorDiagAndThrow("UInt64 value expected.");
        //    }
        //    return result;
        //}
        #endregion facets
        private bool Attributes(Node parent, out AttributesNode result) {
            TextSpan openBracketToken;
            if (Token('[', out openBracketToken)) {
                var obj = new AttributesNode(parent) { OpenBracketTextSpan = openBracketToken };
                List(obj, _attributeGetter, out obj.AttributeList);
                TokenExpected(']', out obj.CloseBracketTextSpan);
                result = obj;
                return true;
            }
            result = null;
            return false;
        }
        private bool Attribute(Node parent, List<AttributeNode> list, out AttributeNode result) {
            NameNode name;
            if (Name(out name)) {
                var obj = new AttributeNode(parent) { NameNode = name };
                Unordered(_nullableGetter, _optionalOrDeleteGetter,
                    out obj.Nullable, out obj.OptionalOrDelete,
                    "Nullable, ?, x, or > expected.");
                if (list.CountOrZero() > 0) {
                    foreach (var item in list) {
                        if (item.NameNode == name) {
                            ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.DuplicateAttributeName, name.ToString()), name.TextSpan);
                        }
                    }
                }
                KeywordExpected(ParserConstants.AsKeyword);
                obj.TypeQName = QualifiableNameExpected();
                result = obj;
                return true;
            }
            result = null;
            return false;
        }

        private bool GlobalElement(Node parent, out NamespaceMemberNode result) {
            if (Keyword(ParserConstants.ElementKeyword)) {
                var obj = new GlobalElementNode(parent);
                obj.NameNode = NameExpected();
                Unordered(_abstractOrSealedGetter, _nullableGetter, _substituteGetter,
                    out obj.AbstractOrSealed, out obj.Nullable, out obj.SubstitutedGlobalElementQName,
                    "Abstract, sealed, nullable, substitutes or > expected.");
                KeywordExpected(ParserConstants.AsKeyword);
                obj.TypeQName = QualifiableNameExpected();
                result = obj;
                return true;
            }
            result = null;
            return false;
        }
        private bool ComplexChildren(Node parent, out ComplexChildrenNode result) {
            var kind = ChildKind.None;
            TextSpan openBraceTextSpan;
            if (Token('{', out openBraceTextSpan)) {
                kind = ChildKind.Set;
            }
            else if (Token((int)TokenKind.HashOpenBrace, out openBraceTextSpan)) {
                kind = ChildKind.Sequence;
            }
            if (kind != ChildKind.None) {
                var obj = new ComplexChildrenNode(parent) { Kind = kind, OpenBraceTextSpan = openBraceTextSpan };
                List(obj, _memberChildGetter, out obj.ChildList);
                TokenExpected('}', out obj.CloseBraceTextSpan);
                result = obj;
                return true;
            }
            result = null;
            return false;
        }
        private bool MemberChild(Node parent, List<MemberChildNode> list, out MemberChildNode result) {
            var complexChildrenNode = parent as ComplexChildrenNode;
            var isInSet = complexChildrenNode != null && complexChildrenNode.IsSet;
            if (!LocalElement(parent, out result)) {
                if (!GlobalElementRef(parent, out result)) {
                    if (isInSet) {
                        return false;
                    }
                    if (!MemberComplexChildren(parent, out result)) {
                        return false;
                    }
                }
            }
            if (isInSet) {
                if (result.MaxOccurrence > 1) {
                    ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.MaxOccurrenceCannotGreaterThanOneInChildSet),
                        result.Occurrence.TextSpan);
                }
            }
            if (list.CountOrZero() > 0) {
                var memberName = result.MemberNameNode;
                foreach (var item in list) {
                    if (item.MemberNameNode == memberName) {
                        ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.DuplicateMemberName, memberName.ToString()),
                            memberName.TextSpan);
                    }
                }
            }
            return true;
        }
        private bool LocalElement(Node parent, out MemberChildNode result) {
            NameNode name;
            if (Name(out name)) {
                var obj = new LocalElementNode(parent) { NameNode = name };
                Unordered(_nullableGetter, _occurrenceGetter, _memberNameGetter,
                    out obj.Nullable, out obj.Occurrence, out obj.MemberNameNode,
                    "Nullable, occurrence, x, membername or > expected.");
                if (!obj.MemberNameNode.IsValid) {
                    obj.MemberNameNode = name;
                }
                KeywordExpected(ParserConstants.AsKeyword);
                obj.TypeQName = QualifiableNameExpected();
                result = obj;
                return true;
            }
            result = null;
            return false;
        }
        private bool GlobalElementRef(Node parent, out MemberChildNode result) {
            if (Token('&')) {
                var obj = new GlobalElementRefNode(parent);
                obj.GlobalElementQName = QualifiableNameExpected();
                Unordered(_occurrenceGetter, _memberNameGetter,
                    out obj.Occurrence, out obj.MemberNameNode, "Occurrence, x, membername or > expected.");
                if (!obj.MemberNameNode.IsValid) {
                    obj.MemberNameNode = obj.GlobalElementQName.Name;
                }
                result = obj;
                return true;
            }
            result = null;
            return false;
        }
        private bool MemberComplexChildren(Node parent, out MemberChildNode result) {
            var kind = ChildKind.None;
            TextSpan openBraceTextSpan;
            if (Token((int)TokenKind.HashOpenBrace, out openBraceTextSpan)) {
                kind = ChildKind.Sequence;
            }
            else if (Token((int)TokenKind.QuestionOpenBrace, out openBraceTextSpan)) {
                kind = ChildKind.Choice;
            }
            if (kind != ChildKind.None) {
                var obj = new MemberComplexChildrenNode(parent) { Kind = kind, OpenBraceTextSpan = openBraceTextSpan };
                List(obj, _memberChildGetter, out obj.ChildList);
                TokenExpected('}', out obj.CloseBraceTextSpan);
                Unordered(_occurrenceGetter, _memberNameGetter,
                    out obj.Occurrence, out obj.MemberNameNode, "Occurrence, x, membername or > expected.");
                if (!obj.MemberNameNode.IsValid) {
                    obj.MemberNameNode = new NameNode(kind == ChildKind.Sequence ? "Seq" : "Choice", obj.CloseBraceTextSpan);
                }
                result = obj;
                return true;
            }
            result = null;
            return false;
        }

        private bool AbstractOrSealed(out NameNode result) {
            if (Keyword(ParserConstants.AbstractKeyword, out result)) {
                return true;
            }
            return Keyword(ParserConstants.SealedKeyword, out result);
        }
        private bool Nullable(out TextSpan result) {
            return Keyword(ParserConstants.NullableKeyword, out result);
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
            if (Keyword(ParserConstants.MemberNameKeyword)) {
                result = NameExpected();
                return true;
            }
            result = default(NameNode);
            return false;
        }
        private bool Substitute(out QualifiableNameNode result) {
            if (Keyword(ParserConstants.SubstitutesKeyword)) {
                result = QualifiableNameExpected();
                return true;
            }
            result = default(QualifiableNameNode);
            return false;
        }
        private bool Occurrence(out OccurrenceNode result) {
            ulong minValue = 0, maxValue = ulong.MaxValue;
            TextSpan textSpan;
            if (Token('?', out textSpan)) {
                maxValue = 1;
            }
            else if (Token('*', out textSpan)) {
            }
            else if (Token('+', out textSpan)) {
                minValue = 1;
            }
            else if (Keyword("x", out textSpan)) {
                maxValue = 0;
            }
            else {
                IntegerNode<ulong> minValueNode, maxValueNode;
                if (UInt64Value(out minValueNode)) {
                    minValue = minValueNode.Value;
                    TokenExpected((int)TokenKind.DotDot, ".. expected.", out textSpan);
                    if (UInt64Value(out maxValueNode)) {
                        maxValue = maxValueNode.Value;
                        if (maxValue == 0) {
                            ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.MaxOccurrenceCannotBeZero), maxValueNode.TextSpan);
                        }
                        if (maxValue < minValue) {
                            ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.MaxOccurrenceNotEqualToOrGreaterThanMinOccurrence,
                                maxValue.ToInvString(), minValue.ToInvString()), maxValueNode.TextSpan);
                        }
                    }
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
