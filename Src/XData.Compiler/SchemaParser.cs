using System;
using System.Collections.Generic;
using XData.TextIO;

namespace XData.Compiler {

    public sealed class Parser : ParserBase {
        public const string AbstractKeyword = "abstract";
        public const string AliasKeyword = "alias";
        public const string AsKeyword = "as";
        public const string AttributeKeyword = "attribute";
        public const string DigitsKeyword = "digits";
        public const string ElementKeyword = "element";
        public const string EnumsKeyword = "enums";
        public const string ExtendsKeyword = "extends";
        public const string ImportKeyword = "import";
        public const string LengthsKeyword = "lengths";
        public const string ListsKeyword = "lists";
        public const string MemberNameKeyword = "membername";
        public const string NamespaceKeyword = "namespace";
        public const string NullableKeyword = "nullable";
        public const string PatternKeyword = "pattern";
        public const string QualifiedKeyword = "qualified";
        public const string RestrictsKeyword = "restricts";
        public const string SealedKeyword = "sealed";
        public const string SubstitutesKeyword = "substitutes";
        public const string TypeKeyword = "type";
        public const string ValuesKeyword = "values";
        //
        //
        private Parser() {
            _uriAliasingGetter = UriAliasing;
            _namespaceGetter = Namespace;
            _importGetter = Import;
            _namespaceMemberGetter = NamespaceMember;
            _memberAttributeGetter = MemberAttribute;
            _memberChildGetter = MemberChild;
            _abstractOrSealedGetter = AbstractOrSealed;
            _qualifiedGetter = Qualified;
            _nullableGetter = Nullable;
            _optionalGetter = Optional;
            _memberNameGetter = MemberName;
            _substitutionGetter = Substitution;
            _occurrenceGetter = Occurrence;
        }
        private delegate bool NodeItemGetter<T>(Node parent, out T node);
        private readonly NodeItemGetter<UriAliasingNode> _uriAliasingGetter;
        private readonly NodeItemGetter<NamespaceNode> _namespaceGetter;
        private readonly NodeItemGetter<ImportNode> _importGetter;
        private readonly NodeItemGetter<NamespaceMemberNode> _namespaceMemberGetter;
        private readonly NodeItemGetter<MemberAttributeNode> _memberAttributeGetter;
        private readonly NodeItemGetter<MemberChildNode> _memberChildGetter;

        private readonly ItemGetter<NameNode> _abstractOrSealedGetter;
        private readonly ItemGetter<TextSpan> _qualifiedGetter;
        private readonly ItemGetter<TextSpan> _nullableGetter;
        private readonly ItemGetter<TextSpan> _optionalGetter;
        private readonly ItemGetter<NameNode> _memberNameGetter;
        private readonly ItemGetter<QualifiableNameNode> _substitutionGetter;
        private readonly ItemGetter<OccurrenceNode> _occurrenceGetter;

        private void ErrorDiagnosticAndThrow(DiagnosticCodeEx code, string errMsg, TextSpan textSpan) {
            ErrorDiagnosticAndThrow((int)code, errMsg, textSpan);
        }

        private bool List<T>(Node parent, NodeItemGetter<T> itemGetter, out List<T> result) {
            result = null;
            T item;
            while (itemGetter(parent, out item)) {
                if (result == null) {
                    result = new List<T>();
                }
                result.Add(item);
            }
            return result != null;
        }

        private bool CompilationUnit(Node parent, out CompilationUnitNode result) {
            result = new CompilationUnitNode(parent);
            List(result, _uriAliasingGetter, out result.UriAliasingList);
            List(result, _namespaceGetter, out result.NamespaceList);
            return true;
        }
        private bool UriAliasing(Node parent, out UriAliasingNode result) {
            if (Keyword(AliasKeyword)) {
                var value = StringValueExpected();
                KeywordExpected(AsKeyword);
                result = new UriAliasingNode(value, NameExpected());
                return true;
            }
            result = default(UriAliasingNode);
            return false;
        }
        private bool Namespace(Node parent, out NamespaceNode result) {
            if (Keyword(NamespaceKeyword)) {
                result = new NamespaceNode(parent);
                result.Uri = UriExpected();
                TokenExpected('{');
                List(result, _importGetter, out result.ImportList);
                List(result, _namespaceMemberGetter, out result.MemberList);
                TokenExpected('}');
                return true;
            }
            result = default(NamespaceNode);
            return false;
        }
        private bool Import(Node parent, out ImportNode result) {
            if (Keyword(ImportKeyword)) {
                var uri = UriExpected();
                var alias = default(NameNode);
                if (Keyword(AsKeyword)) {
                    alias = NameExpected();
                }
                result = new ImportNode(uri, alias);
                return true;
            }
            result = default(ImportNode);
            return false;
        }
        private bool Uri(out UriNode result) {
            NameNode alias;
            var value = default(AtomicValueNode);
            var hasValue = false;
            var hasAlias = Name(out alias);
            if (!hasAlias) {
                hasValue = StringValue(out value);
            }
            if (hasAlias || hasValue) {
                result = new UriNode(alias, value);
                return true;
            }
            result = default(UriNode);
            return false;
        }
        private UriNode UriExpected() {
            UriNode uri;
            if (Uri(out uri)) {
                return uri;
            }
            ErrorDiagnosticAndThrow("Uri expected.");
            return uri;
        }
        private bool NamespaceMember(Node parent, out NamespaceMemberNode result) {
            if (!Type(parent, out result)) {
                if (!GlobalElement(parent, out result)) {
                    if (!GlobalAttribute(parent, out result)) {
                        return false;
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
                if (!TypeRestriction(type, out type.Body)) {
                    if (!TypeExtension(type, out type.Body)) {
                        if (!TypeList(type, out type.Body)) {
                            if (!TypeDirectness(type, out type.Body)) {
                                ErrorDiagnosticAndThrow("Type directness, type list, type extension or type restriction expected.");
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
        private bool TypeDirectness(Node parent, out TypeBodyNode result) {
            var directness = new TypeDirectnessNode(parent);
            var hasAttributes = Attributes(directness, out directness.Attributes);
            var hasChildren = RootStructuralChildren(directness, out directness.StructuralChildren);
            var hasSemicolon = false;
            if (!hasAttributes && !hasChildren) {
                hasSemicolon = Token(';');
            }
            if (hasAttributes || hasChildren || hasSemicolon) {
                result = directness;
                return true;
            }
            result = null;
            return false;
        }
        private bool TypeList(Node parent, out TypeBodyNode result) {
            if (Keyword(ListsKeyword)) {
                var list = new TypeListNode(parent);
                list.ItemQName = QualifiableNameExpected();
                result = list;
                return true;
            }
            result = null;
            return false;
        }
        private bool TypeExtension(Node parent, out TypeBodyNode result) {
            if (Keyword(ExtendsKeyword)) {
                var extension = new TypeExtension(parent);
                extension.BaseQName = QualifiableNameExpected();
                Attributes(extension, out extension.Attributes);
                RootStructuralChildren(extension, out extension.StructuralChildren);
                result = extension;
                return true;
            }
            result = null;
            return false;
        }
        private bool TypeRestriction(Node parent, out TypeBodyNode result) {
            if (Keyword(RestrictsKeyword)) {
                var restriction = new TypeRestriction(parent);
                restriction.BaseQName = QualifiableNameExpected();
                Attributes(restriction, out restriction.Attributes);
                if (!RootStructuralChildren(restriction, out restriction.StructuralChildren)) {
                    SimpleValueRestrictions(restriction, out restriction.SimpleValueRestrictions);
                }
                result = restriction;
                return true;
            }
            result = null;
            return false;
        }
        private bool SimpleValueRestrictions(Node parent, out SimpleValueRestrictionsNode result) {
            if (Token(TokenKind.DollarOpenBrace)) {
                result = new SimpleValueRestrictionsNode(parent);
                bool hasLengths = false, hasDigits = false, hasValues = false, hasEnums = false, hasPattern = false;
                while (true) {
                    var get = false;
                    if (!hasLengths) {
                        if (hasLengths = Lengths(out result.Lengths)) {
                            get = true;
                        }
                    }
                    if (!get && !hasDigits) {
                        if (hasDigits = Digits(out result.Digits)) {
                            get = true;
                        }
                    }
                    if (!get && !hasValues) {
                        if (hasValues = Values(out result.Values)) {
                            get = true;
                        }
                    }
                    if (!get && !hasEnums) {
                        if (hasEnums = Enumerations(out result.Enumerations)) {
                            get = true;
                        }
                    }
                    if (!get && !hasPattern) {
                        if (hasPattern = Pattern(out result.Pattern)) {
                            get = true;
                        }
                    }
                    if (Token('}')) {
                        return true;
                    }
                    if (!get) {
                        ErrorDiagnosticAndThrow("lengths, digits, values, enums, pattern or } expected.");
                    }
                }
            }
            result = null;
            return false;
        }
        private bool Lengths(out IntegerRangeNode<ulong> result) {
            if (Keyword(LengthsKeyword)) {
                result = UInt64Range();
                return true;
            }
            result = default(IntegerRangeNode<ulong>);
            return false;
        }
        private bool Digits(out IntegerRangeNode<byte> result) {
            if (Keyword(DigitsKeyword)) {
                result = ByteRange();
                return true;
            }
            result = default(IntegerRangeNode<byte>);
            return false;
        }
        private bool Values(out ValueRangeNode result) {
            if (Keyword(ValuesKeyword)) {
                bool? minIsInclusive = null, maxIsInclusive = null;
                SimpleValueNode minValue = default(SimpleValueNode), maxValue;
                TextSpan textSpan;
                if (Token('(')) {
                    minIsInclusive = false;
                }
                else if (Token('[')) {
                    minIsInclusive = true;
                }
                if (minIsInclusive != null) {
                    minValue = SimpleValueExpected();
                }
                TokenExpected((int)TokenKind.DotDot, ".. expected.", out textSpan);
                if (SimpleValue(out maxValue)) {
                    if (Token(')')) {
                        maxIsInclusive = false;
                    }
                    else {
                        TokenExpected(']');
                        maxIsInclusive = true;
                    }
                }
                else if (minIsInclusive == null) {
                    ErrorDiagnosticAndThrow("Max value expected.");
                }
                result = new ValueRangeNode(minIsInclusive == null ? default(ValueBoundaryNode?) : new ValueBoundaryNode(minValue, minIsInclusive.Value),
                    maxIsInclusive == null ? default(ValueBoundaryNode?) : new ValueBoundaryNode(maxValue, maxIsInclusive.Value), textSpan);
                return true;
            }
            result = default(ValueRangeNode);
            return false;
        }
        private bool Enumerations(out EnumerationsNode result) {
            TextSpan textSpan;
            if (Keyword(EnumsKeyword, out textSpan)) {
                var list = new List<SimpleValueNode>();
                while (true) {
                    SimpleValueNode item;
                    if (SimpleValue(out item)) {
                        list.Add(item);
                    }
                    else {
                        if (list.Count > 0) {
                            break;
                        }
                        ErrorDiagnosticAndThrow("Simple value expected.");
                    }
                }
                result = new EnumerationsNode(list, textSpan);
                return true;
            }
            result = default(EnumerationsNode);
            return false;
        }
        private bool Pattern(out AtomicValueNode result) {
            if (Keyword(PatternKeyword)) {
                result = StringValueExpected();
                return true;
            }
            result = default(AtomicValueNode);
            return false;
        }
        private IntegerRangeNode<ulong> UInt64Range() {
            ulong? minValue = null, maxValue = null;
            TextSpan textSpan;
            AtomicValueNode minValueNode, maxValueNode;
            if (IntegerValue(out minValueNode)) {
                minValue = ToUInt64(minValueNode);
            }
            TokenExpected((int)TokenKind.DotDot, ".. expected.", out textSpan);
            if (IntegerValue(out maxValueNode)) {
                maxValue = ToUInt64(maxValueNode);
                if (maxValue < minValue) {
                    ErrorDiagnosticAndThrow(DiagnosticCodeEx.MaxValueMustEqualToOrBeGreaterThanMinValue,
                        "Max value '{0}' must equal to or be greater than min value '{1}'.".InvFormat(maxValue.Value.ToInvString(), minValue.Value.ToInvString()),
                        maxValueNode.TextSpan);
                }
            }
            else if (minValue == null) {
                ErrorDiagnosticAndThrow("Max value expected.");
            }
            return new IntegerRangeNode<ulong>(minValue, maxValue, textSpan);
        }
        private IntegerRangeNode<byte> ByteRange() {
            byte? minValue = null, maxValue = null;
            TextSpan textSpan;
            AtomicValueNode minValueNode, maxValueNode;
            if (IntegerValue(out minValueNode)) {
                minValue = ToByte(minValueNode);
            }
            TokenExpected((int)TokenKind.DotDot, ".. expected.", out textSpan);
            if (IntegerValue(out maxValueNode)) {
                maxValue = ToByte(maxValueNode);
                if (maxValue < minValue) {
                    ErrorDiagnosticAndThrow(DiagnosticCodeEx.MaxValueMustEqualToOrBeGreaterThanMinValue,
                        "Max value '{0}' must equal to or be greater than min value '{1}'.".InvFormat(maxValue.Value.ToInvString(), minValue.Value.ToInvString()),
                        maxValueNode.TextSpan);
                }
            }
            else if (minValue == null) {
                ErrorDiagnosticAndThrow("Max value expected.");
            }
            return new IntegerRangeNode<byte>(minValue, maxValue, textSpan);
        }
        private ulong ToUInt64(AtomicValueNode node) {
            ulong value;
            if (!node.Value.TryToInvUInt64(out value)) {
                ErrorDiagnosticAndThrow(DiagnosticCodeEx.UInt64ValueRequired, "UInt64 value required.", node.TextSpan);
            }
            return value;
        }
        private byte ToByte(AtomicValueNode node) {
            byte value;
            if (!node.Value.TryToInvByte(out value)) {
                ErrorDiagnosticAndThrow(DiagnosticCodeEx.ByteValueRequired, "Byte value required.", node.TextSpan);
            }
            return value;
        }


        private bool Attributes(Node parent, out AttributesNode result) {
            if (Token('[')) {
                result = new AttributesNode(parent);
                List(result, _memberAttributeGetter, out result.AttributeList);
                TokenExpected(']', out result.CloseTokenTextSpan);
                return true;
            }
            result = null;
            return false;
        }
        private bool RootStructuralChildren(Node parent, out RootStructuralChildrenNode result) {
            if (Token('{')) {
                result = new RootStructuralChildrenNode(parent);
                List(result, _memberChildGetter, out result.ChildList);
                TokenExpected('}', out result.CloseTokenTextSpan);
                return true;
            }
            result = null;
            return false;
        }

        private bool GlobalAttribute(Node parent, out NamespaceMemberNode result) {
            if (Keyword(AttributeKeyword)) {
                var attribute = new GlobalAttributeNode(parent);
                attribute.Name = NameExpected();
                Unordered(_nullableGetter, out attribute.Nullable, "nullable or > expected.");
                KeywordExpected(AsKeyword);
                attribute.TypeQName = QualifiableNameExpected();
                result = attribute;
                return true;
            }
            result = null;
            return false;
        }
        private bool MemberAttribute(Node parent, out MemberAttributeNode result) {
            if (LocalAttribute(parent, out result)) {
                return true;
            }
            return GlobalAttributeRef(parent, out result);
        }
        private bool LocalAttribute(Node parent, out MemberAttributeNode result) {
            NameNode name;
            if (Name(out name)) {
                var attribute = new LocalAttributeNode(parent) { Name = name };
                Unordered(_nullableGetter, _optionalGetter, _memberNameGetter, _qualifiedGetter,
                    out attribute.Nullable, out attribute.Optional, out attribute.MemberName, out attribute.Qualified,
                    "nullable, ?, membername, qualified or > expected.");
                KeywordExpected(AsKeyword);
                attribute.TypeQName = QualifiableNameExpected();
                result = attribute;
                return true;
            }
            result = null;
            return false;
        }
        private bool GlobalAttributeRef(Node parent, out MemberAttributeNode result) {
            if (Token('&')) {
                var attribute = new GlobalAttributeRefNode(parent);
                attribute.QName = QualifiableNameExpected();
                Unordered(_optionalGetter, _memberNameGetter,
                    out attribute.Optional, out attribute.MemberName, "?, membername or > expected.");
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
                Unordered(_abstractOrSealedGetter, _nullableGetter, _substitutionGetter,
                    out element.AbstractOrSealed, out element.Nullable, out element.Substitution,
                    "abstract, sealed, nullable, substitutes or > expected.");
                KeywordExpected(AsKeyword);
                element.TypeQName = QualifiableNameExpected();
                result = element;
                return true;
            }
            result = null;
            return false;
        }
        private bool MemberChild(Node parent, out MemberChildNode result) {
            if (!LocalElement(parent, out result)) {
                if (!GlobalElementRef(parent, out result)) {
                    return StructuralChildren(parent, out result);
                }
            }
            return true;
        }
        private bool LocalElement(Node parent, out MemberChildNode result) {
            NameNode name;
            if (Name(out name)) {
                var element = new LocalElementNode(parent) { Name = name };
                Unordered(_nullableGetter, _occurrenceGetter, _memberNameGetter,
                    out element.Nullable, out element.Occurrence, out element.MemberName,
                    "nullable, occurrence, membername or > expected.");
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
                element.QName = QualifiableNameExpected();
                Unordered(_occurrenceGetter, _memberNameGetter,
                    out element.Occurrence, out element.MemberName, "occurrence, membername or > expected.");
                result = element;
                return true;
            }
            result = null;
            return false;
        }
        private bool StructuralChildren(Node parent, out MemberChildNode result) {
            bool? isSequence = null;
            if (Token('{')) {
                isSequence = true;
            }
            else if (Token(TokenKind.QuestionOpenBrace)) {
                isSequence = false;
            }
            if (isSequence != null) {
                var children = new StructuralChildrenNode(parent) { IsSequence = isSequence.Value };
                List(children, _memberChildGetter, out children.ChildList);
                TokenExpected('}', out children.CloseTokenTextSpan);
                Unordered(_occurrenceGetter, _memberNameGetter,
                    out children.Occurrence, out children.MemberName, "occurrence, membername or > expected.");
                result = children;
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
        private bool Qualified(out TextSpan result) {
            return Keyword(QualifiedKeyword, out result);
        }
        private bool Nullable(out TextSpan result) {
            return Keyword(NullableKeyword, out result);
        }
        private bool Optional(out TextSpan result) {
            return Token('?', out result);
        }
        private bool MemberName(out NameNode result) {
            if (Keyword(MemberNameKeyword)) {
                //TokenExpected(':');
                result = NameExpected();
                return true;
            }
            result = default(NameNode);
            return false;
        }
        private bool Substitution(out QualifiableNameNode result) {
            if (Keyword(SubstitutesKeyword)) {
                //TokenExpected(':');
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
                AtomicValueNode minValueNode;
                if (IntegerValue(out minValueNode)) {
                    minValue = ToUInt64(minValueNode);
                    TokenExpected((int)TokenKind.DotDot, ".. expected.", out textSpan);
                    AtomicValueNode maxValueNode;
                    if (IntegerValue(out maxValueNode)) {
                        maxValue = ToUInt64(maxValueNode);
                        if (maxValue < minValue) {
                            ErrorDiagnosticAndThrow(DiagnosticCodeEx.MaxValueMustEqualToOrBeGreaterThanMinValue,
                                "Max value '{0}' must equal to or be greater than min value '{1}'.".InvFormat(maxValue.ToInvString(), minValue.ToInvString()),
                                maxValueNode.TextSpan);
                        }
                        else if (maxValue == 0) {
                            ErrorDiagnosticAndThrow(DiagnosticCodeEx.MaxValueMustBeGreaterThanZero,
                                "Max value must be greater than zero.", maxValueNode.TextSpan);
                        }
                    }
                    else {
                        maxValue = ulong.MaxValue;
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

        private bool Unordered<T>(ItemGetter<T> getter, out T value, string errMsg, int startToken = '<', int endToken = '>') {
            value = default(T);
            if (Token(startToken)) {
                getter(out value);
                if (Token(endToken)) {
                    return true;
                }
                ErrorDiagnosticAndThrow(errMsg);
            }
            return false;
        }
        private bool Unordered<T1, T2>(ItemGetter<T1> getter1, ItemGetter<T2> getter2,
            out T1 value1, out T2 value2, string errMsg, int startToken = '<', int endToken = '>') {
            value1 = default(T1);
            value2 = default(T2);
            if (Token(startToken)) {
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
                    if (Token(endToken)) {
                        return true;
                    }
                    if (!get) {
                        ErrorDiagnosticAndThrow(errMsg);
                    }
                }
            }
            return false;
        }
        private bool Unordered<T1, T2, T3>(ItemGetter<T1> getter1, ItemGetter<T2> getter2, ItemGetter<T3> getter3,
            out T1 value1, out T2 value2, out T3 value3, string errMsg, int startToken = '<', int endToken = '>') {
            value1 = default(T1);
            value2 = default(T2);
            value3 = default(T3);
            if (Token(startToken)) {
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
                    if (Token(endToken)) {
                        return true;
                    }
                    if (!get) {
                        ErrorDiagnosticAndThrow(errMsg);
                    }
                }
            }
            return false;
        }
        private bool Unordered<T1, T2, T3, T4>(ItemGetter<T1> getter1, ItemGetter<T2> getter2, ItemGetter<T3> getter3, ItemGetter<T4> getter4,
            out T1 value1, out T2 value2, out T3 value3, out T4 value4, string errMsg, int startToken = '<', int endToken = '>') {
            value1 = default(T1);
            value2 = default(T2);
            value3 = default(T3);
            value4 = default(T4);
            if (Token(startToken)) {
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
                    if (Token(endToken)) {
                        return true;
                    }
                    if (!get) {
                        ErrorDiagnosticAndThrow(errMsg);
                    }
                }
            }
            return false;
        }

        private bool Unordered<T1, T2, T3, T4, T5>(ItemGetter<T1> getter1, ItemGetter<T2> getter2, ItemGetter<T3> getter3, ItemGetter<T4> getter4, ItemGetter<T5> getter5,
            out T1 value1, out T2 value2, out T3 value3, out T4 value4, out T5 value5, string errMsg, int startToken = '<', int endToken = '>') {
            value1 = default(T1);
            value2 = default(T2);
            value3 = default(T3);
            value4 = default(T4);
            value5 = default(T5);
            if (Token(startToken)) {
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
                    if (Token(endToken)) {
                        return true;
                    }
                    if (!get) {
                        ErrorDiagnosticAndThrow(errMsg);
                    }
                }
            }
            return false;
        }


    }
}
