using System;
using System.Collections.Generic;
using System.IO;

namespace XData.IO.Text {
    public abstract class ParserBase {
        protected ParserBase() {
            _simpleValueGetter = SimpleValue;
        }
        protected delegate bool ItemGetter<T>(out T node);
        protected delegate bool ItemGetterEx<T>(List<T> list, out T node);
        protected readonly ItemGetter<SimpleValueNode> _simpleValueGetter;
        protected void Set(string filePath, TextReader reader, Context context) {
            if (filePath == null) {
                throw new ArgumentNullException("filePath");
            }
            if (context == null) {
                throw new ArgumentNullException("context");
            }
            _lexer = Lexer.Get(reader);
            _filePath = filePath;
            _context = context;
            _token = null;
        }
        protected virtual void Clear() {
            if (_lexer != null) {
                _lexer.Clear();
            }
            _filePath = null;
            _context = null;
            _token = null;
        }
        private Lexer _lexer;
        private string _filePath;
        private Context _context;
        private Token? _token;
        protected sealed class ParsingException : Exception { }
        private static readonly ParsingException _parsingExceptionObj = new ParsingException();
        protected void ErrorDiagnosticAndThrow(int rawCode, string errMsg, TextSpan textSpan) {
            _context.AddDiagnostic(DiagnosticSeverity.Error, rawCode, errMsg, textSpan, null);
            throw _parsingExceptionObj;
        }
        protected void ErrorDiagnosticAndThrow(DiagnosticCode code, string errMsg, TextSpan textSpan) {
            ErrorDiagnosticAndThrow((int)code, errMsg, textSpan);
        }
        protected void ErrorDiagnosticAndThrow(string errMsg, TextSpan textSpan) {
            ErrorDiagnosticAndThrow((int)DiagnosticCode.Parsing, errMsg, textSpan);
        }
        protected void ErrorDiagnosticAndThrow(string errMsg, Token token) {
            ErrorDiagnosticAndThrow(errMsg ?? token.Value, token.ToTextSpan(_filePath));
        }
        protected void ErrorDiagnosticAndThrow(string errMsg) {
            ErrorDiagnosticAndThrow(errMsg, GetToken());
        }
        protected Token GetToken() {
            if (_token != null) {
                return _token.Value;
            }
            while (true) {
                var token = _lexer.GetToken();
                var kind = token.Kind;
                if (!kind.IsTrivalToken()) {
                    if (kind == TokenKind.Error) {
                        ErrorDiagnosticAndThrow(null, token);
                    }
                    else {
                        _token = token;
                        return token;
                    }
                }
            }
        }
        protected void ConsumeToken() {
            _token = null;
        }
        protected bool Token(int rawKind, out TextSpan textSpan) {
            var token = GetToken();
            if (token.RawKind == rawKind) {
                textSpan = token.ToTextSpan(_filePath);
                ConsumeToken();
                return true;
            }
            textSpan = default(TextSpan);
            return false;
        }
        protected bool Token(int rawKind) {
            if (GetToken().RawKind == rawKind) {
                ConsumeToken();
                return true;
            }
            return false;
        }
        protected bool Token(TokenKind kind) {
            return Token((int)kind);
        }
        protected void TokenExpected(char ch) {
            if (!Token(ch)) {
                ErrorDiagnosticAndThrow(ch.ToString() + " expected.");
            }
        }
        protected void TokenExpected(int rawKind, string errMsg) {
            if (!Token(rawKind)) {
                ErrorDiagnosticAndThrow(errMsg);
            }
        }
        protected void TokenExpected(TokenKind kind, string errMsg) {
            TokenExpected((int)kind, errMsg);
        }
        protected void TokenExpected(char ch, out TextSpan textSpan) {
            if (!Token(ch, out textSpan)) {
                ErrorDiagnosticAndThrow(ch.ToString() + " expected.");
            }
        }
        protected void TokenExpected(int rawKind, string errMsg, out TextSpan textSpan) {
            if (!Token(rawKind, out textSpan)) {
                ErrorDiagnosticAndThrow(errMsg);
            }
        }
        protected void EndOfFileExpected() {
            TokenExpected(char.MaxValue, "End of file expected.");
        }
        protected bool Name(out NameNode name) {
            var token = GetToken();
            var kind = token.Kind;
            if (kind == TokenKind.Name || kind == TokenKind.VerbatimName) {
                name = new NameNode(token.Value, token.ToTextSpan(_filePath));
                ConsumeToken();
                return true;
            }
            name = default(NameNode);
            return false;
        }
        protected NameNode NameExpected() {
            NameNode name;
            if (Name(out name)) {
                return name;
            }
            ErrorDiagnosticAndThrow("Name expected.");
            return name;
        }
        protected bool Keyword(string keywordValue) {
            var token = GetToken();
            if (token.Kind == TokenKind.Name && token.Value == keywordValue) {
                ConsumeToken();
                return true;
            }
            return false;
        }
        protected void KeywordExpected(string keywordValue) {
            if (!Keyword(keywordValue)) {
                ErrorDiagnosticAndThrow(keywordValue + " expetced.");
            }
        }
        protected bool Keyword(string keywordValue, out NameNode keyword) {
            var token = GetToken();
            if (token.Kind == TokenKind.Name && token.Value == keywordValue) {
                keyword = new NameNode(keywordValue, token.ToTextSpan(_filePath));
                ConsumeToken();
                return true;
            }
            keyword = default(NameNode);
            return false;
        }
        protected bool Keyword(string keywordValue, out TextSpan textSpan) {
            var token = GetToken();
            if (token.Kind == TokenKind.Name && token.Value == keywordValue) {
                textSpan = token.ToTextSpan(_filePath);
                ConsumeToken();
                return true;
            }
            textSpan = default(TextSpan);
            return false;
        }
        protected virtual bool QualifiableName(out QualifiableNameNode qName) {
            NameNode name;
            if (Name(out name)) {
                if (Token(':')) {
                    qName = new QualifiableNameNode(name, NameExpected());
                }
                else {
                    qName = new QualifiableNameNode(default(NameNode), name);
                }
                return true;
            }
            qName = default(QualifiableNameNode);
            return false;
        }
        protected QualifiableNameNode QualifiableNameExpected() {
            QualifiableNameNode qName;
            if (QualifiableName(out qName)) {
                return qName;
            }
            ErrorDiagnosticAndThrow("Qualifiable name expected.");
            return qName;
        }
        protected bool AtomicValue(out AtomicValueNode atomicValue, AtomicValueKind expectedKind = AtomicValueKind.None) {
            var kind = AtomicValueKind.None;
            var token = GetToken();
            switch (token.Kind) {
                case TokenKind.StringValue:
                case TokenKind.VerbatimStringValue:
                    kind = AtomicValueKind.String;
                    break;
                case TokenKind.CharValue:
                    kind = AtomicValueKind.Char;
                    break;
                case TokenKind.Name:
                    if (token.Value == "true" || token.Value == "false") {
                        kind = AtomicValueKind.Boolean;
                    }
                    break;
                case TokenKind.IntegerValue:
                    kind = AtomicValueKind.Integer;
                    break;
                case TokenKind.DecimalValue:
                    kind = AtomicValueKind.Decimal;
                    break;
                case TokenKind.RealValue:
                    kind = AtomicValueKind.Real;
                    break;
            }
            if (kind != AtomicValueKind.None && (expectedKind == AtomicValueKind.None || kind == expectedKind)) {
                atomicValue = new AtomicValueNode(kind, token.Value, token.ToTextSpan(_filePath));
                ConsumeToken();
                return true;
            }
            atomicValue = default(AtomicValueNode);
            return false;
        }
        protected AtomicValueNode AtomicValueExpected(AtomicValueKind expectedKind = AtomicValueKind.None) {
            AtomicValueNode atomicValue;
            if (AtomicValue(out atomicValue, expectedKind)) {
                return atomicValue;
            }
            ErrorDiagnosticAndThrow(expectedKind == AtomicValueKind.None ? "Atomic value expected." :
                expectedKind.ToString() + " value expected.");
            return atomicValue;
        }
        protected bool StringValue(out AtomicValueNode atomicValue) {
            return AtomicValue(out atomicValue, AtomicValueKind.String);
        }
        protected AtomicValueNode StringValueExpected() {
            return AtomicValueExpected(AtomicValueKind.String);
        }
        protected bool IntegerValue(out AtomicValueNode atomicValue) {
            return AtomicValue(out atomicValue, AtomicValueKind.Integer);
        }
        protected AtomicValueNode IntegerValueExpected() {
            return AtomicValueExpected(AtomicValueKind.Integer);
        }

        protected bool TypeIndicator(out QualifiableNameNode typeQName) {
            if (Token('(')) {
                typeQName = QualifiableNameExpected();
                TokenExpected(')');
                return true;
            }
            typeQName = default(QualifiableNameNode);
            return false;
        }
        protected bool SimpleValue(QualifiableNameNode typeQName, out SimpleValueNode simpleValue) {
            AtomicValueNode atom;
            DelimitedList<SimpleValueNode> list = null;
            var hasAtom = AtomicValue(out atom);
            var hasList = false;
            if (!hasAtom) {
                hasList = List((int)TokenKind.HashOpenBracket, ']', _simpleValueGetter, "Simple value or ] expected.", out list);
            }
            if (hasAtom || hasList) {
                simpleValue = new SimpleValueNode(typeQName, atom, list);
                return true;
            }
            simpleValue = default(SimpleValueNode);
            return false;
        }
        protected bool SimpleValue(out SimpleValueNode simpleValue) {
            QualifiableNameNode typeQName;
            var hasTypeQName = TypeIndicator(out typeQName);
            if (SimpleValue(typeQName, out simpleValue)) {
                return true;
            }
            if (hasTypeQName) {
                ErrorDiagnosticAndThrow("Atomic value or list value expetced.");
            }
            return false;
        }
        protected SimpleValueNode SimpleValueExpected() {
            SimpleValueNode value;
            if (SimpleValue(out value)) {
                return value;
            }
            ErrorDiagnosticAndThrow("Simple value expected.");
            return value;
        }
        protected bool List<T>(int startRawKind, int endRawKind, ItemGetter<T> itemGetter, string errorMsg, out DelimitedList<T> result) {
            TextSpan openTokenTextSpan, closeTokenTextSpan;
            if (Token(startRawKind, out openTokenTextSpan)) {
                var list = new DelimitedList<T>(openTokenTextSpan);
                while (true) {
                    T item;
                    if (itemGetter(out item)) {
                        list.Add(item);
                    }
                    else if (Token(endRawKind, out closeTokenTextSpan)) {
                        list.CloseTokenTextSpan = closeTokenTextSpan;
                        result = list;
                        return true;
                    }
                    else {
                        ErrorDiagnosticAndThrow(errorMsg);
                    }
                }
            }
            result = null;
            return false;
        }
        protected bool List<T>(int startRawKind, int endRawKind, ItemGetterEx<T> itemGetterEx, string errorMsg, out DelimitedList<T> result) {
            TextSpan openTokenTextSpan, closeTokenTextSpan;
            if (Token(startRawKind, out openTokenTextSpan)) {
                var list = new DelimitedList<T>(openTokenTextSpan);
                while (true) {
                    T item;
                    if (itemGetterEx(list, out item)) {
                        list.Add(item);
                    }
                    else if (Token(endRawKind, out closeTokenTextSpan)) {
                        list.CloseTokenTextSpan = closeTokenTextSpan;
                        result = list;
                        return true;
                    }
                    else {
                        ErrorDiagnosticAndThrow(errorMsg);
                    }
                }
            }
            result = null;
            return false;
        }

        //protected bool ListOrSingle<T>(int startRawKind, int endRawKind, TryGetter<T> itemGetter, string errorMsg, out ListOrSingleNode<T> listOrSingle) {
        //    TextSpan openTokenTextSpan, closeTokenTextSpan;
        //    if (Token(startRawKind, out openTokenTextSpan)) {
        //        var hasSingle = false;
        //        var single = default(T);
        //        List<T> list = null;
        //        while (true) {
        //            T item;
        //            if (itemGetter(out item)) {
        //                if (hasSingle) {
        //                    if (list == null) {
        //                        list = new List<T>();
        //                        list.Add(single);
        //                    }
        //                    list.Add(item);
        //                }
        //                else {
        //                    single = item;
        //                    hasSingle = true;
        //                }
        //            }
        //            else if (Token(endRawKind, out closeTokenTextSpan)) {
        //                listOrSingle = new ListOrSingleNode<T>(list, single, hasSingle, openTokenTextSpan, closeTokenTextSpan);
        //                return true;
        //            }
        //            else {
        //                ErrorDiagnosticAndThrow(errorMsg);
        //            }
        //        }
        //    }
        //    listOrSingle = default(ListOrSingleNode<T>);
        //    return false;
        //}

    }
    public sealed class Parser : ParserBase {
        [ThreadStatic]
        private static Parser _instance;
        public static bool Parse(string filePath, TextReader reader, Context context, out ElementNode result) {
            return (_instance ?? (_instance = new Parser())).ParsingUnit(filePath, reader, context, out result);
        }
        private Parser() {
            _uriAliasingGetter = UriAliasing;
            _attributeGetter = Attribute;
            _uriAliasingListStack = new Stack<DelimitedList<UriAliasingNode>>();
        }
        private readonly ItemGetterEx<UriAliasingNode> _uriAliasingGetter;
        private readonly ItemGetter<AttributeNode> _attributeGetter;
        private readonly Stack<DelimitedList<UriAliasingNode>> _uriAliasingListStack;
        private bool _getFullName;
        private bool _resolveNullAlias;
        protected override void Clear() {
            base.Clear();
            _uriAliasingListStack.Clear();
        }
        private bool ParsingUnit(string filePath, TextReader reader, Context context, out ElementNode result) {
            Set(filePath, reader, context);
            _uriAliasingListStack.Clear();
            _resolveNullAlias = true;
            try {
                if (Element(out result)) {
                    EndOfFileExpected();
                    return true;
                }
                else {
                    ErrorDiagnosticAndThrow("Element expected.");
                }
            }
            catch (ParsingException) {
            }
            finally {
                Clear();
            }
            result = default(ElementNode);
            return false;
        }
        //private bool UriAliasingList() {
        //    if (Token('<')) {
        //        var hasSingle = false;
        //        var single = default(UriAliasingNode);
        //        List<UriAliasingNode> list = null;
        //        while (true) {
        //            UriAliasingNode ua;
        //            if (UriAliasing(out ua)) {
        //                if (hasSingle) {
        //                    if (list == null) {
        //                        list = new List<UriAliasingNode>();
        //                        list.Add(single);
        //                    }
        //                    foreach (var item in list) {
        //                        if (item.IsDefault && ua.IsDefault) {
        //                            ErrorDiagnosticAndThrow("Duplicate default uri.", ua.Uri.TextSpan);
        //                        }
        //                        else if (item.Alias == ua.Alias) {
        //                            ErrorDiagnosticAndThrow("Duplicate uri alias '{0}'.".InvFormat(ua.Alias.ToString()), ua.Alias.TextSpan);
        //                        }
        //                    }
        //                    list.Add(ua);
        //                }
        //                else {
        //                    single = ua;
        //                    hasSingle = true;
        //                }
        //            }
        //            else if (Token('>')) {
        //                if (list != null || hasSingle) {
        //                    _uriAliasingListStack.Push(new ListOrSingleNode<UriAliasingNode>(list, single, hasSingle, default(TextSpan), default(TextSpan)));
        //                    return true;
        //                }
        //                return false;
        //            }
        //            else {
        //                ErrorDiagnosticAndThrow("Uri aliasing or > expected.");
        //            }
        //        }
        //    }
        //    return false;
        //}
        private bool UriAliasingList() {
            DelimitedList<UriAliasingNode> list;
            if (List('<', '>', _uriAliasingGetter, "Uri aliasing or > expected.", out list)) {
                _uriAliasingListStack.Push(list);
            }
            return false;
        }
        private bool UriAliasing(List<UriAliasingNode> list, out UriAliasingNode uriAliasing) {
            bool? isDefault = null;
            NameNode alias;
            if (Name(out alias)) {
                if (Token('=')) {
                    isDefault = false;
                }
                else {
                    TokenExpected(TokenKind.EqualsEquals, "= or == expected.");
                    isDefault = true;
                }
            }
            else {
                if (Token(TokenKind.EqualsEquals)) {
                    isDefault = true;
                }
            }
            if (isDefault != null) {
                var uri = StringValueExpected();
                var isDefaultValue = isDefault.Value;
                foreach (var item in list) {
                    if (item.IsDefault && isDefaultValue) {
                        ErrorDiagnosticAndThrow(DiagnosticCode.DuplicateDefaultUri,
                            "Duplicate default uri.", uri.TextSpan);
                    }
                    else if (item.Alias == alias) {
                        ErrorDiagnosticAndThrow(DiagnosticCode.DuplicateUriAlias,
                            "Duplicate uri alias '{0}'.".InvFormat(alias.ToString()), alias.TextSpan);
                    }
                }
                uriAliasing = new UriAliasingNode(alias, uri, isDefaultValue);
                return true;
            }
            uriAliasing = default(UriAliasingNode);
            return false;
        }
        protected override bool QualifiableName(out QualifiableNameNode qName) {
            NameNode name;
            if (Name(out name)) {
                if (Token(':')) {
                    qName = new QualifiableNameNode(name, NameExpected());
                }
                else {
                    qName = new QualifiableNameNode(default(NameNode), name);
                }
                if (_getFullName) {
                    GetFullName(ref qName);
                }
                return true;
            }
            qName = default(QualifiableNameNode);
            return false;
        }
        private void GetFullName(ref QualifiableNameNode qName) {
            string uri = null;
            if (qName.Alias.IsValid || _resolveNullAlias) {
                uri = GetUri(qName.Alias);
            }
            qName.FullName = new FullName(uri, qName.Name.Value);
        }
        private string GetUri(NameNode alias) {
            if (alias.Value == "sys") {
                return NamespaceInfo.SystemUri;
            }
            var isNull = !alias.IsValid;
            foreach (var uaList in _uriAliasingListStack) {
                foreach (var ua in uaList) {
                    if (ua.Alias == alias || (isNull && ua.IsDefault)) {
                        return ua.Uri.Value;
                    }
                }
            }
            if (!isNull) {
                ErrorDiagnosticAndThrow(DiagnosticCode.InvalidUriAlias,
                    "Invalid uri alias '{0}'.".InvFormat(alias.ToString()), alias.TextSpan);
            }
            return null;
        }
        private bool Element(out ElementNode element) {
            QualifiableNameNode qName;
            _getFullName = false;
            var hasQName = QualifiableName(out qName);
            _getFullName = true;
            if (hasQName) {
                var hasUriAliasingList = UriAliasingList();
                GetFullName(ref qName);
                var elementValue = default(ElementValueNode);
                TextSpan equalsTokenTextSpan;
                if (Token('=', out equalsTokenTextSpan)) {
                    if (!ElementValue(equalsTokenTextSpan, out elementValue)) {
                        ErrorDiagnosticAndThrow("Element value expected.");
                    }
                }
                if (hasUriAliasingList) {
                    _uriAliasingListStack.Pop();
                }
                element = new ElementNode(qName, elementValue);
                return true;
            }
            element = default(ElementNode);
            return false;
        }
        private bool ElementValue(TextSpan equalsTokenTextSpan, out ElementValueNode elementValue) {
            QualifiableNameNode typeQName;
            var hasTypeQName = TypeIndicator(out typeQName);
            ComplexValueNode complexValue;
            var simpleValue = default(SimpleValueNode);
            var hasComplexValue = ComplexValue(equalsTokenTextSpan, typeQName, out complexValue);
            var hasSimpleValue = false;
            if (!hasComplexValue) {
                hasSimpleValue = SimpleValue(typeQName, out simpleValue);
            }
            if (hasComplexValue || hasSimpleValue) {
                elementValue = new ElementValueNode(complexValue, simpleValue);
                return true;
            }
            else {
                if (hasTypeQName) {
                    ErrorDiagnosticAndThrow("Complex value or simple value expetced.");
                }
                elementValue = default(ElementValueNode);
                return false;
            }
        }
        private bool ComplexValue(TextSpan equalsTokenTextSpan, QualifiableNameNode typeQName, out ComplexValueNode complexValue) {
            DelimitedList<AttributeNode> attributeList;
            var hasAttributeList = List('[', ']', _attributeGetter, "Attribute or ] expected.", out attributeList);
            DelimitedList<ElementNode> elementList = null;
            var simpleValue = default(SimpleValueNode);
            TextSpan openTokenTextSpan, closeTokenTextSpan;
            if (Token('{', out openTokenTextSpan)) {
                if (SimpleValue(out simpleValue)) {
                    TokenExpected('}');
                }
                else {
                    elementList = new DelimitedList<ElementNode>(openTokenTextSpan);
                    while (true) {
                        ElementNode element;
                        if (Element(out element)) {
                            elementList.Add(element);
                        }
                        else if (Token('}', out closeTokenTextSpan)) {
                            elementList.CloseTokenTextSpan = closeTokenTextSpan;
                            break;
                        }
                        else {
                            ErrorDiagnosticAndThrow(elementList.Count > 0 ? "Element or } expected." :
                                "Element, simple value or } expected.");
                        }
                    }
                }
            }
            if (hasAttributeList || elementList != null || simpleValue.IsValid) {
                complexValue = new ComplexValueNode(equalsTokenTextSpan, typeQName, attributeList, elementList, simpleValue, default(TextSpan));
                return true;
            }
            else {
                TextSpan semicolonTokenTextSpan;
                if (Token(';', out semicolonTokenTextSpan)) {
                    complexValue = new ComplexValueNode(equalsTokenTextSpan, typeQName, null, null,
                        default(SimpleValueNode), semicolonTokenTextSpan);
                    return true;
                }
            }
            complexValue = default(ComplexValueNode);
            return false;
        }
        private bool Attribute(out AttributeNode attribute) {
            QualifiableNameNode qName;
            _resolveNullAlias = false;
            var hasQName = QualifiableName(out qName);
            _resolveNullAlias = true;
            if (hasQName) {
                var value = default(SimpleValueNode);
                if (Token('=')) {
                    value = SimpleValueExpected();
                }
                attribute = new AttributeNode(qName, value);
                return true;
            }
            attribute = default(AttributeNode);
            return false;
        }

    }
}
