using System;
using System.Collections.Generic;
using XData.TextIO;

namespace XData.Schema {
    public class SchemaParser : ParserBase {
        public const string AbstractKeyword = "abstract";
        public const string AliasKeyword = "alias";
        public const string AsKeyword = "as";
        public const string AttributeKeyword = "attribute";
        public const string ElementKeyword = "element";

        public const string ImportKeyword = "import";

        public const string NamespaceKeyword = "namespace";
        public const string SealedKeyword = "sealed";
        public const string TypeKeyword = "type";
        //
        //
        private SchemaParser() {
            _uriAliasingGetter = UriAliasing;
            _namespaceGetter = Namespace;
            _importGetter = Import;
            _memberGetter = Member;
        }
        private readonly ItemNodeGetter<UriAliasingNode> _uriAliasingGetter;
        private readonly ItemNodeGetter<NamespaceNode> _namespaceGetter;
        private readonly ItemNodeGetter<ImportNode> _importGetter;
        private readonly ItemNodeGetter<MemberNode> _memberGetter;


        private CompilationUnitNode CompilationUnit() {
            List<UriAliasingNode> uriAliasinglist;
            List(_uriAliasingGetter, out uriAliasinglist);
            List<NamespaceNode> namespacelist;
            List(_namespaceGetter, out namespacelist);
            return new CompilationUnitNode(uriAliasinglist, namespacelist);
        }
        private bool UriAliasing(out UriAliasingNode result) {
            if (Keyword(AliasKeyword)) {
                var value = StringValueExpected();
                KeywordExpected(AsKeyword);
                result = new UriAliasingNode(value, NameExpected());
                return true;
            }
            result = default(UriAliasingNode);
            return false;
        }
        private bool Namespace(out NamespaceNode result) {
            if (Keyword(NamespaceKeyword)) {
                var uri = UriExpected();
                TokenExpected('{');
                List<ImportNode> importList;
                List(_importGetter, out importList);
                List<MemberNode> memberList;
                List(_memberGetter, out memberList);
                TokenExpected('}');
                result = new NamespaceNode(importList, memberList);
                return true;
            }
            result = default(NamespaceNode);
            return false;
        }
        private bool Import(out ImportNode result) {
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
        private bool Member(out MemberNode result) {
            TypeNode type;
            if (Type(out type)) {
                result = type;
                return true;
            }
            ElementNode element;
            if (Element(true, out element)) {
                result = element;
                return true;
            }
            AttributeNode attribute;
            if (Attribute(true, out attribute)) {
                result = attribute;
                return true;
            }
            result = null;
            return false;
        }
        private bool Type(out TypeNode result) {
            if (Keyword(TypeKeyword)) {
                var name = NameExpected();
            }
            result = null;
            return false;
        }
        private bool Attribute(bool isGlobal, out AttributeNode result) {
            if (Keyword(AttributeKeyword)) {
                var name = NameExpected();

            }
            result = null;
            return false;
        }
        private bool Element(bool isGlobal, out ElementNode result) {
            if (Keyword(ElementKeyword)) {
                var name = NameExpected();

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
