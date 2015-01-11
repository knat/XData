using System;
using System.Collections.Generic;
using XData.TextIO;

namespace XData.Compiler {
    public sealed class Parser : ParserBase {
        public const string AbstractKeyword = "abstract";
        public const string AliasKeyword = "alias";
        public const string AsKeyword = "as";
        public const string AttributeKeyword = "attribute";
        public const string ElementKeyword = "element";

        public const string ImportKeyword = "import";
        public const string ListKeyword = "list";

        public const string NamespaceKeyword = "namespace";
        public const string SealedKeyword = "sealed";
        public const string TypeKeyword = "type";
        //
        //
        private Parser() {
            _uriAliasingGetter = UriAliasing;
            _namespaceGetter = Namespace;
            _importGetter = Import;
            _memberGetter = Member;
        }
        private delegate bool ItemNodeGetter<T>(Node parent, out T node);
        private readonly ItemNodeGetter<UriAliasingNode> _uriAliasingGetter;
        private readonly ItemNodeGetter<NamespaceNode> _namespaceGetter;
        private readonly ItemNodeGetter<ImportNode> _importGetter;
        private readonly ItemNodeGetter<MemberNode> _memberGetter;
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
                List(result, _memberGetter, out result.MemberList);
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
        private bool Member(Node parent, out MemberNode result) {
            TypeNode type;
            if (Type(parent, out type)) {
                result = type;
                return true;
            }
            ElementNode element;
            if (Element(parent, EntityDeclarationKind.Global, out element)) {
                result = element;
                return true;
            }
            AttributeNode attribute;
            if (Attribute(parent, EntityDeclarationKind.Global, out attribute)) {
                result = attribute;
                return true;
            }
            result = null;
            return false;
        }
        private bool Type(Node parent, out TypeNode result) {
            if (Keyword(TypeKeyword)) {
                result = new TypeNode(parent);
                result.Name = NameExpected();
                if (Token('<')) {
                    AbstractOrSealed(out result.Modifier);
                    if (!Token('>')) {
                        ErrorDiagnosticAndThrow(result.Modifier.IsValid ? "> expected." : "abstract, sealed or > expected.");
                    }
                }
                TypeListNode list;
                if (TypeList(parent, out list)) {
                    result.Body = list;
                }
                else {

                }

                return true;
            }
            result = null;
            return false;
        }
        private bool TypeList(Node parent, out TypeListNode result) {
            if (Keyword(ListKeyword)) {
                result = new TypeListNode(parent);
                result.ItemName = QualifiableNameExpected();
                return true;
            }
            result = null;
            return false;
        }

        private bool Attributes(Node parent, out AttributesNode result) {
            if (Token('[')) {
                result = new AttributesNode(parent);

            }
            result = null;
            return false;
        }
        private bool StructChildren(Node parent, out StructuralChildrenNode result) {
            if (Token('{')) {
                result = new StructuralChildrenNode(parent);

            }
            result = null;
            return false;
        }
        private bool RestrictedSimpleChild(Node parent, out RestrictedSimpleChildNode result) {
            if (Token(TokenKind.DollarOpenBrace)) {
                result = new RestrictedSimpleChildNode(parent);

            }
            result = null;
            return false;
        }

        private bool Attribute(Node parent, EntityDeclarationKind kind, out AttributeNode result) {
            if (Keyword(AttributeKeyword)) {
                result = new AttributeNode(parent, kind);
                result.Name = NameExpected();

                return true;
            }
            result = null;
            return false;
        }
        private bool Element(Node parent, EntityDeclarationKind kind, out ElementNode result) {
            if (Keyword(ElementKeyword)) {
                result = new ElementNode(parent, kind);
                result.Name = NameExpected();

                return true;
            }
            result = null;
            return false;
        }
        private bool AbstractOrSealed(out NameNode result) {
            if (Keyword(AbstractKeyword, out result)) {
                return true;
            }
            if (Keyword(SealedKeyword, out result)) {
                return true;
            }
            return false;
        }


    }
}
