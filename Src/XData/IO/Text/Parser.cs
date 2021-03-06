﻿using System;
using System.Collections.Generic;
using System.IO;

namespace XData.IO.Text {
    internal abstract class ParserBase {
        protected ParserBase() {
            _simpleValueGetter = SimpleValue;
        }
        protected delegate bool NodeGetter<T>(out T node);
        protected delegate bool NodeGetterWithList<T>(List<T> list, out T node);
        protected readonly NodeGetter<SimpleValueNode> _simpleValueGetter;
        protected void Set(string filePath, TextReader reader, DiagContext context) {
            if (filePath == null) throw new ArgumentNullException("filePath");
            if (context == null) throw new ArgumentNullException("context");
            _lexer = Lexer.Get(reader);
            _token = null;
            _filePath = filePath;
            _context = context;
        }
        protected virtual void Clear() {
            if (_lexer != null) {
                _lexer.Clear();
            }
            _token = null;
            _filePath = null;
            _context = null;
        }
        private Lexer _lexer;
        private Token? _token;
        protected string _filePath;
        protected DiagContext _context;
        protected sealed class ParsingException : Exception { }
        protected static readonly ParsingException _parsingException = new ParsingException();
        protected void ErrorDiagAndThrow(string errMsg, TextSpan textSpan) {
            _context.AddDiag(DiagSeverity.Error, (int)DiagCode.Parsing, errMsg, textSpan, null);
            throw _parsingException;
        }
        protected void ErrorDiagAndThrow(string errMsg, Token token) {
            ErrorDiagAndThrow(errMsg ?? token.Value, token.ToTextSpan(_filePath));
        }
        protected void ErrorDiagAndThrow(string errMsg) {
            ErrorDiagAndThrow(errMsg, GetToken());
        }
        protected void ErrorDiagAndThrow(DiagMsg diagMsg, TextSpan textSpan) {
            _context.AddErrorDiag(diagMsg, textSpan);
            throw _parsingException;
        }
        private static bool IsTrivalToken(TokenKind kind) {
            return kind == TokenKind.WhitespaceOrNewLine || kind == TokenKind.SingleLineComment || kind == TokenKind.MultiLineComment;
        }
        protected Token GetToken() {
            if (_token != null) {
                return _token.Value;
            }
            while (true) {
                var token = _lexer.GetToken();
                var kind = token.Kind;
                if (!IsTrivalToken(kind)) {
                    if (kind == TokenKind.Error) {
                        ErrorDiagAndThrow(null, token);
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
        protected bool PeekToken(int rawKind) {
            return GetToken().RawKind == rawKind;
        }
        protected bool PeekToken(int rawKind1, int rawKind2) {
            var tokenKind = GetToken().RawKind;
            return tokenKind == rawKind1 || tokenKind == rawKind2;
        }
        protected bool PeekToken(int rawKind1, int rawKind2, int rawKind3) {
            var tokenKind = GetToken().RawKind;
            return tokenKind == rawKind1 || tokenKind == rawKind2 || tokenKind == rawKind3;
        }
        protected bool PeekToken(int rawKind1, int rawKind2, int rawKind3, int rawKind4) {
            var tokenKind = GetToken().RawKind;
            return tokenKind == rawKind1 || tokenKind == rawKind2 || tokenKind == rawKind3 || tokenKind == rawKind4;
        }
        protected bool PeekToken(int rawKind1, int rawKind2, int rawKind3, int rawKind4, int rawKind5) {
            var tokenKind = GetToken().RawKind;
            return tokenKind == rawKind1 || tokenKind == rawKind2 || tokenKind == rawKind3 || tokenKind == rawKind4 || tokenKind == rawKind5;
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
                ErrorDiagAndThrow(ch.ToString() + " expected.");
            }
        }
        protected void TokenExpected(int rawKind, string errMsg) {
            if (!Token(rawKind)) {
                ErrorDiagAndThrow(errMsg);
            }
        }
        protected void TokenExpected(TokenKind kind, string errMsg) {
            TokenExpected((int)kind, errMsg);
        }
        protected void TokenExpected(char ch, out TextSpan textSpan) {
            if (!Token(ch, out textSpan)) {
                ErrorDiagAndThrow(ch.ToString() + " expected.");
            }
        }
        protected void TokenExpected(int rawKind, string errMsg, out TextSpan textSpan) {
            if (!Token(rawKind, out textSpan)) {
                ErrorDiagAndThrow(errMsg);
            }
        }
        protected void EndOfFileExpected() {
            TokenExpected(char.MaxValue, "End of file expected.");
        }
        protected bool Name(out NameNode result) {
            var token = GetToken();
            var kind = token.Kind;
            if (kind == TokenKind.Name || kind == TokenKind.VerbatimName) {
                result = new NameNode(token.Value, token.ToTextSpan(_filePath));
                ConsumeToken();
                return true;
            }
            result = default(NameNode);
            return false;
        }
        protected NameNode NameExpected() {
            NameNode name;
            if (!Name(out name)) {
                ErrorDiagAndThrow("Name expected.");
            }
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
                ErrorDiagAndThrow(keywordValue + " expetced.");
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
        protected virtual bool QualifiableName(out QualifiableNameNode result) {
            NameNode name;
            if (Name(out name)) {
                if (Token(':')) {
                    result = new QualifiableNameNode(name, NameExpected());
                }
                else {
                    result = new QualifiableNameNode(default(NameNode), name);
                }
                return true;
            }
            result = default(QualifiableNameNode);
            return false;
        }
        protected QualifiableNameNode QualifiableNameExpected() {
            QualifiableNameNode qName;
            if (!QualifiableName(out qName)) {
                ErrorDiagAndThrow("Qualifiable name expected.");
            }
            return qName;
        }
        protected bool AtomValue(out AtomValueNode result, AtomValueKind expectedKind = AtomValueKind.None) {
            var kind = AtomValueKind.None;
            var token = GetToken();
            switch (token.Kind) {
                case TokenKind.StringValue:
                case TokenKind.VerbatimStringValue:
                    kind = AtomValueKind.String;
                    break;
                //case TokenKind.CharValue:
                //    kind = AtomValueKind.Char;
                //    break;
                case TokenKind.Name:
                    if (token.Value == "true" || token.Value == "false") {
                        kind = AtomValueKind.Boolean;
                    }
                    break;
                case TokenKind.IntegerValue:
                    kind = AtomValueKind.Integer;
                    break;
                case TokenKind.DecimalValue:
                    kind = AtomValueKind.Decimal;
                    break;
                case TokenKind.RealValue:
                    kind = AtomValueKind.Real;
                    break;
            }
            if (kind != AtomValueKind.None && (expectedKind == AtomValueKind.None || kind == expectedKind)) {
                result = new AtomValueNode(kind, token.Value, token.ToTextSpan(_filePath));
                ConsumeToken();
                return true;
            }
            result = default(AtomValueNode);
            return false;
        }
        protected AtomValueNode AtomValueExpected(AtomValueKind expectedKind = AtomValueKind.None) {
            AtomValueNode atomValue;
            if (!AtomValue(out atomValue, expectedKind)) {
                ErrorDiagAndThrow(expectedKind == AtomValueKind.None ? "Atom value expected." :
                    expectedKind.ToString() + " value expected.");
            }
            return atomValue;
        }
        protected bool StringValue(out AtomValueNode result) {
            return AtomValue(out result, AtomValueKind.String);
        }
        protected AtomValueNode StringValueExpected() {
            return AtomValueExpected(AtomValueKind.String);
        }
        protected bool IntegerValue(out AtomValueNode result) {
            return AtomValue(out result, AtomValueKind.Integer);
        }
        protected AtomValueNode IntegerValueExpected() {
            return AtomValueExpected(AtomValueKind.Integer);
        }
        protected bool TypeIndicator(out QualifiableNameNode result) {
            if (Token('(')) {
                result = QualifiableNameExpected();
                TokenExpected(')');
                return true;
            }
            result = default(QualifiableNameNode);
            return false;
        }
        protected bool SimpleValue(QualifiableNameNode typeQName, out SimpleValueNode result) {
            AtomValueNode atom;
            NodeList<SimpleValueNode> list = null;
            if (!AtomValue(out atom)) {
                if (!List((int)TokenKind.HashOpenBracket, ']', _simpleValueGetter, "Simple value or ] expected.", out list)) {
                    result = default(SimpleValueNode);
                    return false;
                }
            }
            result = new SimpleValueNode(typeQName, atom, list);
            return true;
        }
        protected /*virtual*/ bool SimpleValue(out SimpleValueNode result) {
            QualifiableNameNode typeQName;
            var hasTypeQName = TypeIndicator(out typeQName);
            if (SimpleValue(typeQName, out result)) {
                return true;
            }
            if (hasTypeQName) {
                ErrorDiagAndThrow("Atom value or list value expetced.");
            }
            return false;
        }
        protected SimpleValueNode SimpleValueExpected() {
            SimpleValueNode value;
            if (!SimpleValue(out value)) {
                ErrorDiagAndThrow("Simple value expected.");
            }
            return value;
        }
        protected bool List<T>(int openTokenKind, int closeTokenKind, NodeGetter<T> nodeGetter, string errorMsg, out NodeList<T> result) {
            TextSpan openTokenTextSpan, closeTokenTextSpan;
            if (Token(openTokenKind, out openTokenTextSpan)) {
                var list = new NodeList<T>(openTokenTextSpan);
                while (true) {
                    T item;
                    if (nodeGetter(out item)) {
                        list.Add(item);
                    }
                    else if (Token(closeTokenKind, out closeTokenTextSpan)) {
                        list.CloseTokenTextSpan = closeTokenTextSpan;
                        result = list;
                        return true;
                    }
                    else {
                        ErrorDiagAndThrow(errorMsg);
                    }
                }
            }
            result = null;
            return false;
        }
        protected bool List<T>(int openTokenKind, int closeTokenKind, NodeGetterWithList<T> nodeGetterWithList, string errorMsg, out NodeList<T> result) {
            TextSpan openTokenTextSpan, closeTokenTextSpan;
            if (Token(openTokenKind, out openTokenTextSpan)) {
                var list = new NodeList<T>(openTokenTextSpan);
                while (true) {
                    T item;
                    if (nodeGetterWithList(list, out item)) {
                        list.Add(item);
                    }
                    else if (Token(closeTokenKind, out closeTokenTextSpan)) {
                        list.CloseTokenTextSpan = closeTokenTextSpan;
                        result = list;
                        return true;
                    }
                    else {
                        ErrorDiagAndThrow(errorMsg);
                    }
                }
            }
            result = null;
            return false;
        }
        protected bool List<T>(NodeGetterWithList<T> nodeGetterWithList, out List<T> result) {
            result = null;
            T item;
            while (nodeGetterWithList(result, out item)) {
                if (result == null) {
                    result = new List<T>();
                }
                result.Add(item);
            }
            return result != null;
        }

    }
    internal sealed class Parser : ParserBase {
        [ThreadStatic]
        private static Parser _instance;
        public static bool Parse(string filePath, TextReader reader, DiagContext context, out ElementNode result) {
            return (_instance ?? (_instance = new Parser())).ParsingUnit(filePath, reader, context, out result);
        }
        private Parser() {
            _uriAliasingGetter = UriAliasing;
            _attributeGetter = Attribute;
            _elementGetter = Element;
            _uriAliasingListStack = new Stack<NodeList<UriAliasingNode>>();
        }
        private readonly NodeGetterWithList<UriAliasingNode> _uriAliasingGetter;
        private readonly NodeGetterWithList<AttributeNode> _attributeGetter;
        private readonly NodeGetter<ElementNode> _elementGetter;
        private readonly Stack<NodeList<UriAliasingNode>> _uriAliasingListStack;
        private bool _getFullName;
        protected override void Clear() {
            base.Clear();
            _uriAliasingListStack.Clear();
        }
        private bool ParsingUnit(string filePath, TextReader reader, DiagContext context, out ElementNode result) {
            Set(filePath, reader, context);
            _uriAliasingListStack.Clear();
            try {
                if (Element(out result)) {
                    EndOfFileExpected();
                    return true;
                }
                else {
                    ErrorDiagAndThrow("Element expected.");
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
        private bool UriAliasingList() {
            NodeList<UriAliasingNode> list;
            if (List('<', '>', _uriAliasingGetter, "Uri aliasing or > expected.", out list)) {
                _uriAliasingListStack.Push(list);
            }
            return false;
        }
        private bool UriAliasing(List<UriAliasingNode> list, out UriAliasingNode result) {
            NameNode alias;
            if (Name(out alias)) {
                if (alias.Value == "sys") {
                    ErrorDiagAndThrow(new DiagMsg(DiagCode.AliasSysIsReserved), alias.TextSpan);
                }
                if (list.CountOrZero() > 0) {
                    foreach (var item in list) {
                        if (item.Alias == alias) {
                            ErrorDiagAndThrow(new DiagMsg(DiagCode.DuplicateUriAlias, alias.ToString()), alias.TextSpan);
                        }
                    }
                }
                TokenExpected('=');
                result = new UriAliasingNode(alias, StringValueExpected());
                return true;
            }
            result = default(UriAliasingNode);
            return false;
        }
        protected override bool QualifiableName(out QualifiableNameNode result) {
            NameNode name;
            if (Name(out name)) {
                if (Token(':')) {
                    result = new QualifiableNameNode(name, NameExpected());
                }
                else {
                    result = new QualifiableNameNode(default(NameNode), name);
                }
                if (_getFullName) {
                    GetFullName(ref result);
                }
                return true;
            }
            result = default(QualifiableNameNode);
            return false;
        }
        private void GetFullName(ref QualifiableNameNode qName) {
            string uri = null;
            if (qName.Alias.IsValid) {
                uri = GetUri(qName.Alias);
            }
            qName.FullName = new FullName(uri, qName.Name.Value);
        }
        private string GetUri(NameNode alias) {
            if (alias.Value == "sys") {
                return Extensions.SystemUri;
            }
            foreach (var uaList in _uriAliasingListStack) {
                foreach (var ua in uaList) {
                    if (ua.Alias == alias) {
                        return ua.Uri.Value;
                    }
                }
            }
            ErrorDiagAndThrow(new DiagMsg(DiagCode.InvalidUriReference, alias.ToString()), alias.TextSpan);
            return null;
        }
        private bool Element(out ElementNode result) {
            QualifiableNameNode qName;
            _getFullName = false;
            var hasQName = QualifiableName(out qName);
            _getFullName = true;
            if (hasQName) {
                var hasUriAliasingList = UriAliasingList();
                GetFullName(ref qName);
                var elementValue = default(ElementValueNode);
                TextSpan equalsTextSpan;
                if (Token('=', out equalsTextSpan)) {
                    if (!ElementValue(equalsTextSpan, out elementValue)) {
                        ErrorDiagAndThrow("Element value expected.");
                    }
                }
                if (hasUriAliasingList) {
                    _uriAliasingListStack.Pop();
                }
                result = new ElementNode(qName, elementValue);
                return true;
            }
            result = default(ElementNode);
            return false;
        }
        private bool ElementValue(TextSpan equalsTextSpan, out ElementValueNode result) {
            QualifiableNameNode typeQName;
            var hasTypeQName = TypeIndicator(out typeQName);
            ComplexValueNode complexValue;
            var simpleValue = default(SimpleValueNode);
            if (!ComplexValue(equalsTextSpan, typeQName, out complexValue)) {
                if (!SimpleValue(typeQName, out simpleValue)) {
                    if (hasTypeQName) {
                        ErrorDiagAndThrow("Complex value or simple value expetced.");
                    }
                    result = default(ElementValueNode);
                    return false;
                }
            }
            result = new ElementValueNode(complexValue, simpleValue);
            return true;
        }
        private bool ComplexValue(TextSpan equalsTextSpan, QualifiableNameNode typeQName, out ComplexValueNode result) {
            NodeList<AttributeNode> attributeList;
            List('[', ']', _attributeGetter, "Attribute or ] expected.", out attributeList);
            NodeList<ElementNode> elementList = null;
            var simpleChild = default(SimpleValueNode);
            if (Token('$')) {
                simpleChild = SimpleValueExpected();
            }
            else {
                List('{', '}', _elementGetter, "Element or } expected.", out elementList);
            }
            var semicolonTextSpan = default(TextSpan);
            if (attributeList == null && elementList == null && !simpleChild.IsValid) {
                if (!Token(';', out semicolonTextSpan)) {
                    result = default(ComplexValueNode);
                    return false;
                }
            }
            result = new ComplexValueNode(equalsTextSpan, typeQName, attributeList, elementList, simpleChild, semicolonTextSpan);
            return true;
        }
        private bool Attribute(List<AttributeNode> list, out AttributeNode result) {
            NameNode name;
            if (Name(out name)) {
                if (list.CountOrZero() > 0) {
                    foreach (var item in list) {
                        if (item.NameNode == name) {
                            ErrorDiagAndThrow(new DiagMsg(DiagCode.DuplicateAttributeName, name.ToString()), name.TextSpan);
                        }
                    }
                }
                var value = default(SimpleValueNode);
                if (Token('=')) {
                    value = SimpleValueExpected();
                }
                result = new AttributeNode(name, value);
                return true;
            }
            result = default(AttributeNode);
            return false;
        }

    }
}
