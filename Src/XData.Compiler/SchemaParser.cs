using System;
using System.Collections.Generic;
using XData.TextIO;

namespace XData.Compiler {

    public sealed  class Parser : ParserBase {
        public const string AbstractKeyword = "abstract";
        public const string AliasKeyword = "alias";
        public const string AsKeyword = "as";
        public const string AttributeKeyword = "attribute";
        public const string DefaultKeyword = "default";
        public const string ElementKeyword = "element";
        public const string ExtendsKeyword = "extends";

        public const string ImportKeyword = "import";
        public const string ListsKeyword = "lists";

        public const string NamespaceKeyword = "namespace";
        public const string QualifiedKeyword = "qualified";

        public const string RestrictsKeyword = "restricts";
        public const string SealedKeyword = "sealed";
        public const string SubstitutesKeyword = "substitutes";

        public const string TypeKeyword = "type";
        //
        //
        private Parser() {
            _uriAliasingGetter = UriAliasing;
            _namespaceGetter = Namespace;
            _importGetter = Import;
            _namespaceMmemberGetter = NamespaceMember;
            _abstractOrSealedGetter = AbstractOrSealed;
            _qualifiedGetter = Qualified;
            _defaultGetter = Default;
            _substituteGetter = Substitute;
        }
        private delegate bool ItemNodeGetter<T>(Node parent, out T node);
        private readonly ItemNodeGetter<UriAliasingNode> _uriAliasingGetter;
        private readonly ItemNodeGetter<NamespaceNode> _namespaceGetter;
        private readonly ItemNodeGetter<ImportNode> _importGetter;
        private readonly ItemNodeGetter<MemberNode> _namespaceMmemberGetter;
        private readonly ItemGetter<NameNode> _abstractOrSealedGetter;
        private readonly ItemGetter<NameNode> _qualifiedGetter;
        private readonly ItemGetter<SimpleValueNode> _defaultGetter;
        private readonly ItemGetter<QualifiableNameNode> _substituteGetter;

        private bool List<T>(Node parent, ItemNodeGetter<T> itemGetter, out List<T> result) {
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
                List(result, _namespaceMmemberGetter, out result.MemberList);
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
        private bool NamespaceMember(Node parent, out MemberNode result) {
            if (!Type(parent, out result)) {
                if (!GlobalElement(parent, out result)) {
                    if (!GlobalAttribute(parent, out result)) {
                        return false;
                    }
                }
            }
            return true;
        }
        private bool Type(Node parent, out MemberNode result) {
            if (Keyword(TypeKeyword)) {
                var type = new TypeNode(parent);
                type.Name = NameExpected();
                Annotations(_abstractOrSealedGetter, out type.AbstractOrSealed, "abstract, sealed or > expected.");
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
            var hasChildren = StructuralChildren(directness, out directness.StructuralChildren);
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
                StructuralChildren(extension, out extension.StructuralChildren);
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
                if (!RestrictedSimpleChild(restriction, out restriction.Children)) {
                    StructuralChildrenNode sc;
                    if (StructuralChildren(restriction, out sc)) {
                        restriction.Children = sc;
                    }
                }
                result = restriction;
                return true;
            }
            result = null;
            return false;
        }

        private bool Attributes(Node parent, out AttributesNode result) {
            if (Token('[')) {
                result = new AttributesNode(parent);

                return true;
            }
            result = null;
            return false;
        }
        private bool StructuralChildren(Node parent, out StructuralChildrenNode result) {
            if (Token('{')) {
                result = new StructuralChildrenNode(parent);

                return true;
            }
            result = null;
            return false;
        }
        private bool RestrictedSimpleChild(Node parent, out ChildrenNode result) {
            if (Token(TokenKind.DollarOpenBrace)) {
                var rs = new RestrictedSimpleChildNode(parent);

                result = rs;
                return true;
            }
            result = null;
            return false;
        }

        private bool GlobalAttribute(Node parent, out MemberNode result) {
            if (Keyword(AttributeKeyword)) {
                var attribute = new GlobalAttributeNode(parent);
                attribute.Name = NameExpected();
                KeywordExpected(AsKeyword);
                attribute.TypeQName = QualifiableNameExpected();
                result = attribute;
                return true;
            }
            result = null;
            return false;
        }
        private bool LocalAttribute(Node parent, out AttributeNode result) {
            NameNode name;
            if (Name(out name)) {
                var attribute = new LocalAttributeNode(parent) { Name = name };
                Annotations(_qualifiedGetter, _defaultGetter, out attribute.Qualified, out attribute.DefaultValue, "qualified, default value or > expected.");
                KeywordExpected(AsKeyword);
                attribute.TypeQName = QualifiableNameExpected();
                result = attribute;
                return true;
            }
            result = null;
            return false;
        }
        private bool AttributeRef(Node parent, out AttributeNode result) {
            if (Token('&')) {
                var attRef = new AttributeRefNode(parent);
                attRef.QName = QualifiableNameExpected();
                Annotations(_defaultGetter, out attRef.DefaultValue, "Default value or > expected.");
                result = attRef;
                return true;
            }
            result = null;
            return false;
        }
        private bool GlobalElement(Node parent, out MemberNode result) {
            if (Keyword(ElementKeyword)) {
                var element = new GlobalElementNode(parent);
                element.Name = NameExpected();

                KeywordExpected(AsKeyword);
                element.TypeQName = QualifiableNameExpected();
                result = element;
                return true;
            }
            result = null;
            return false;
        }
        private bool AbstractOrSealed(out NameNode result) {
            if (!Keyword(AbstractKeyword, out result)) {
                if (!Keyword(SealedKeyword, out result)) {
                    return false;
                }
            }
            return true;
        }
        private bool Qualified(out NameNode result) {
            return Keyword(QualifiedKeyword, out result);
        }
        private bool Default(out SimpleValueNode result) {
            if (Keyword(DefaultKeyword)) {
                TokenExpected(':');
                result = SimpleValueExpected();
                return true;
            }
            result = default(SimpleValueNode);
            return false;
        }
        private bool Substitute(out QualifiableNameNode result) {
            if (Keyword(SubstitutesKeyword)) {
                TokenExpected(':');
                result = QualifiableNameExpected();
                return true;
            }
            result = default(QualifiableNameNode);
            return false;
        }

        private void Annotations<T>(ItemGetter<T> getter, out T value, string errMsg) {
            value = default(T);
            if (Token('<')) {
                getter(out value);
                if (!Token('>')) {
                    ErrorDiagnosticAndThrow(errMsg);
                }
            }
        }
        private void Annotations<T1, T2>(ItemGetter<T1> getter1, ItemGetter<T2> getter2, out T1 value1, out T2 value2, string errMsg) {
            value1 = default(T1);
            value2 = default(T2);
            if (Token('<')) {
                var hasValue1 = getter1(out value1);
                var hasValue2 = getter2(out value2);
                if (hasValue2 && !hasValue1) {
                    getter1(out value1);
                }
                if (!Token('>')) {
                    ErrorDiagnosticAndThrow(errMsg);
                }
            }
        }

    }
}
