using System;
using System.Globalization;
using System.Text;

namespace XData.TextIO {
    public struct Token {
        public Token(int kind, int startIndex, int length, TextPosition startPosition, TextPosition endPosition, string value) {
            RawKind = kind;
            StartIndex = startIndex;
            Length = length;
            StartPosition = startPosition;
            EndPosition = endPosition;
            Value = value;
        }
        public readonly int RawKind;
        public readonly int StartIndex;
        public readonly int Length;
        public readonly TextPosition StartPosition;
        public readonly TextPosition EndPosition;
        public readonly string Value;//for name, value or error token
        public TokenKind Kind { get { return (TokenKind)RawKind; } }
        public bool IsEndOfFile { get { return RawKind == char.MaxValue; } }
        public bool IsError { get { return Kind == TokenKind.Error; } }
        public TextSpan ToTextSpan(string filePath) {
            return new TextSpan(filePath, this);
        }
        public string GetText(char[] data) {
            return new string(data, StartIndex, Length);
        }
    }
    public enum TokenKind {
        Error = -1000,
        WhitespaceOrNewLine,
        SingleLineComment,
        MultiLineComment,
        VerbatimName,
        Name,
        VerbatimStringValue,
        StringValue,
        CharValue,
        IntegerValue,
        DecimalValue,
        RealValue,
        DotDot,
        EqualsEquals,
        HashOpenBracket,
        DollarOpenBrace,
    }
    public sealed class Lexer {
        [ThreadStatic]
        private static Lexer _instance;
        public static Lexer GetInstance(char[] data) {
            return (_instance ?? (_instance = new Lexer())).Set(data);
        }
        private Lexer() {
            _stringBuilder = new StringBuilder();
        }
        private Lexer Set(char[] data) {
            if (data == null) throw new ArgumentNullException("data");
            _data = data;
            _count = data.Length;
            _index = 0;
            _lastLine = _lastColumn = _line = _column = 1;
            return this;
        }
        private char[] _data;
        private int _count;
        private int _index;
        private int _lastLine, _lastColumn, _line, _column;
        private readonly StringBuilder _stringBuilder;
        private char GetChar() {
            if (_index < _count) return _data[_index];
            return char.MaxValue;
        }
        private char GetNextChar() {
            if (_index + 1 < _count) return _data[_index + 1];
            return char.MaxValue;
        }
        private char GetNextNextChar() {
            if (_index + 2 < _count) return _data[_index + 2];
            return char.MaxValue;
        }
        private void AdvanceChar() {
            _lastLine = _line;
            _lastColumn = _column;
            if (_index < _count) {
                var ch = _data[_index++];
                if (IsNewLine(ch)) {
                    if (ch == '\r' && _index < _count && _data[_index] == '\n') {
                        ++_index;
                    }
                    ++_line;
                    _column = 1;
                }
                else {
                    ++_column;
                }
            }
        }
        private enum StateKind : byte {
            None = 0,
            InWhitespace,
            InSingleLineComment,
            InMultiLineComment,
            InVerbatimName,
            InName,
            InVerbatimStringValue,
            InStringValue,
            InCharValue,
            InNumericValueInteger,
            InNumericValueFraction,
            InNumericValueExponent,

        }
        private struct State {
            internal State(StateKind kind, int index, int line, int column) {
                Kind = kind;
                Index = index;
                Line = line;
                Column = column;
            }
            internal StateKind Kind;
            internal readonly int Index;
            internal readonly int Line;
            internal readonly int Column;
        }
        private State CreateState(StateKind kind) {
            return new State(kind, _index, _line, _column);
        }
        private Token CreateToken(TokenKind kind, State state, string value = null) {
            var startIndex = state.Index;
            var length = _index - startIndex;
            if (kind == TokenKind.Name || kind == TokenKind.IntegerValue || kind == TokenKind.DecimalValue || kind == TokenKind.RealValue) {
                value = new string(_data, startIndex, length);
            }
            else if (kind == TokenKind.VerbatimName) {
                value = new string(_data, startIndex + 1, length - 1);
            }
            return new Token((int)kind, startIndex, length, new TextPosition(state.Line, state.Column), new TextPosition(_lastLine, _lastColumn), value);
        }
        private Token CreateTokenAndAdvanceChar(char ch) {
            var pos = new TextPosition(_line, _column);
            var token = new Token(ch, _index, _index < _count ? 1 : 0, pos, pos, null);
            AdvanceChar();
            return token;
        }
        private Token CreateErrorToken(string errorMessage) {
            var pos = new TextPosition(_line, _column);
            return new Token((int)TokenKind.Error, _index, _index < _count ? 1 : 0, pos, pos, errorMessage);
        }
        public Token GetToken() {
            var state = default(State);
            while (true) {
                var ch = GetChar();
                var stateKind = state.Kind;
                if (stateKind == StateKind.InWhitespace) {
                    if (IsWhitespace(ch) || IsNewLine(ch)) {
                        AdvanceChar();
                    }
                    else {
                        return CreateToken(TokenKind.WhitespaceOrNewLine, state);
                    }
                }
                else if (stateKind == StateKind.InSingleLineComment) {
                    if (IsNewLine(ch) || ch == char.MaxValue) {
                        return CreateToken(TokenKind.SingleLineComment, state);
                    }
                    else {
                        AdvanceChar();
                    }
                }
                else if (stateKind == StateKind.InMultiLineComment) {
                    if (ch == '*') {
                        AdvanceChar();
                        ch = GetChar();
                        if (ch == '/') {
                            AdvanceChar();
                            return CreateToken(TokenKind.MultiLineComment, state);
                        }
                    }
                    else if (ch == char.MaxValue) {
                        return CreateErrorToken("*/ expected.");
                    }
                    else {
                        AdvanceChar();
                    }
                }
                else if (stateKind == StateKind.InName || stateKind == StateKind.InVerbatimName) {
                    if (IsNamePartCharacter(ch)) {
                        AdvanceChar();
                    }
                    else {
                        return CreateToken(stateKind == StateKind.InName ? TokenKind.Name : TokenKind.VerbatimName, state);
                    }
                }
                else if (stateKind == StateKind.InVerbatimStringValue) {
                    if (ch == '"') {
                        AdvanceChar();
                        ch = GetChar();
                        if (ch == '"') {
                            _stringBuilder.Append('"');
                            AdvanceChar();
                        }
                        else {
                            return CreateToken(TokenKind.VerbatimStringValue, state, _stringBuilder.ToString());
                        }
                    }
                    else if (ch == char.MaxValue) {
                        return CreateErrorToken("\" expected.");
                    }
                    else {
                        _stringBuilder.Append(ch);
                        AdvanceChar();
                    }
                }
                else if (stateKind == StateKind.InStringValue) {
                    if (ch == '\\') {
                        AdvanceChar();
                        Token errToken;
                        if (!ProcessCharEscSeq(out errToken)) {
                            return errToken;
                        }
                    }
                    else if (ch == '"') {
                        AdvanceChar();
                        return CreateToken(TokenKind.StringValue, state, _stringBuilder.ToString());
                    }
                    else if (IsNewLine(ch) || ch == char.MaxValue) {
                        return CreateErrorToken("\" expected.");
                    }
                    else {
                        _stringBuilder.Append(ch);
                        AdvanceChar();
                    }
                }
                else if (stateKind == StateKind.InCharValue) {
                    if (ch == '\\') {
                        AdvanceChar();
                        Token errToken;
                        if (!ProcessCharEscSeq(out errToken)) {
                            return errToken;
                        }
                    }
                    else if (ch == '\'') {
                        if (_stringBuilder.Length == 1) {
                            AdvanceChar();
                            return CreateToken(TokenKind.CharValue, state, _stringBuilder.ToString());
                        }
                        else {
                            return CreateErrorToken("character expected.");
                        }
                    }
                    else if (IsNewLine(ch) || ch == char.MaxValue) {
                        return CreateErrorToken("' expected.");
                    }
                    else {
                        if (_stringBuilder.Length == 0) {
                            _stringBuilder.Append(ch);
                            AdvanceChar();
                        }
                        else {
                            return CreateErrorToken("' expected.");
                        }
                    }
                }
                else if (stateKind == StateKind.InNumericValueInteger) {
                    if (IsDecDigit(ch)) {
                        AdvanceChar();
                    }
                    else if (ch == '.') {
                        var nextch = GetNextChar();
                        if (IsDecDigit(nextch)) {
                            state.Kind = StateKind.InNumericValueFraction;
                            AdvanceChar();
                            AdvanceChar();
                        }
                        else if (nextch == '.') {
                            return CreateToken(TokenKind.IntegerValue, state);
                        }
                        else {
                            AdvanceChar();
                            return CreateErrorToken("decimal digit expected.");
                        }
                    }
                    else if (ch == 'E' || ch == 'e') {
                        AdvanceChar();
                        ch = GetChar();
                        if (ch == '+' || ch == '-') {
                            AdvanceChar();
                            ch = GetChar();
                        }
                        if (IsDecDigit(ch)) {
                            state.Kind = StateKind.InNumericValueExponent;
                            AdvanceChar();
                        }
                        else {
                            return CreateErrorToken("decimal digit expected.");
                        }
                    }
                    else {
                        return CreateToken(TokenKind.IntegerValue, state);
                    }
                }
                else if (stateKind == StateKind.InNumericValueFraction) {
                    if (IsDecDigit(ch)) {
                        AdvanceChar();
                    }
                    else if (ch == 'E' || ch == 'e') {
                        AdvanceChar();
                        ch = GetChar();
                        if (ch == '+' || ch == '-') {
                            AdvanceChar();
                            ch = GetChar();
                        }
                        if (IsDecDigit(ch)) {
                            state.Kind = StateKind.InNumericValueExponent;
                            AdvanceChar();
                        }
                        else {
                            return CreateErrorToken("decimal digit expected.");
                        }
                    }
                    else {
                        return CreateToken(TokenKind.DecimalValue, state);
                    }
                }
                else if (stateKind == StateKind.InNumericValueExponent) {
                    if (IsDecDigit(ch)) {
                        AdvanceChar();
                    }
                    else {
                        return CreateToken(TokenKind.RealValue, state);
                    }
                }
                //
                //
                else if (ch == char.MaxValue) {
                    return CreateTokenAndAdvanceChar(ch);
                }
                else if (IsWhitespace(ch) || IsNewLine(ch)) {
                    state = CreateState(StateKind.InWhitespace);
                    AdvanceChar();
                }
                else if (ch == '/') {
                    var nextch = GetNextChar();
                    if (nextch == '/') {
                        state = CreateState(StateKind.InSingleLineComment);
                        AdvanceChar();
                        AdvanceChar();
                    }
                    else if (nextch == '*') {
                        state = CreateState(StateKind.InMultiLineComment);
                        AdvanceChar();
                        AdvanceChar();
                    }
                    else {
                        return CreateTokenAndAdvanceChar(ch);
                    }
                }
                else if (ch == '@') {
                    var nextch = GetNextChar();
                    if (nextch == '"') {
                        state = CreateState(StateKind.InVerbatimStringValue);
                        AdvanceChar();
                        AdvanceChar();
                        _stringBuilder.Clear();
                    }
                    else if (IsNameStartCharacter(nextch)) {
                        state = CreateState(StateKind.InVerbatimName);
                        AdvanceChar();
                        AdvanceChar();
                    }
                    else {
                        return CreateTokenAndAdvanceChar(ch);
                    }
                }
                else if (IsNameStartCharacter(ch)) {
                    state = CreateState(StateKind.InName);
                    AdvanceChar();
                }
                else if (ch == '"') {
                    state = CreateState(StateKind.InStringValue);
                    AdvanceChar();
                    _stringBuilder.Clear();
                }
                else if (ch == '\'') {
                    state = CreateState(StateKind.InCharValue);
                    AdvanceChar();
                    _stringBuilder.Clear();
                }
                else if (IsDecDigit(ch)) {
                    state = CreateState(StateKind.InNumericValueInteger);
                    AdvanceChar();
                }
                else if (ch == '+' || ch == '-') {
                    var nextch = GetNextChar();
                    if (IsDecDigit(nextch)) {
                        state = CreateState(StateKind.InNumericValueInteger);
                        AdvanceChar();
                        AdvanceChar();
                    }
                    else if (nextch == '.') {
                        var nextnextch = GetNextNextChar();
                        if (IsDecDigit(nextnextch)) {
                            state = CreateState(StateKind.InNumericValueFraction);
                            AdvanceChar();
                            AdvanceChar();
                            AdvanceChar();
                        }
                        else {
                            return CreateTokenAndAdvanceChar(ch);
                        }
                    }
                    else {
                        return CreateTokenAndAdvanceChar(ch);
                    }
                }
                else if (ch == '.') {
                    var nextch = GetNextChar();
                    if (IsDecDigit(nextch)) {
                        state = CreateState(StateKind.InNumericValueFraction);
                        AdvanceChar();
                        AdvanceChar();
                    }
                    else if (nextch == '.') {
                        state = CreateState(StateKind.None);
                        AdvanceChar();
                        AdvanceChar();
                        return CreateToken(TokenKind.DotDot, state);
                    }
                    else {
                        return CreateTokenAndAdvanceChar(ch);
                    }
                }
                else if (ch == '=') {
                    var nextch = GetNextChar();
                    if (nextch == '=') {
                        state = CreateState(StateKind.None);
                        AdvanceChar();
                        AdvanceChar();
                        return CreateToken(TokenKind.EqualsEquals, state);
                    }
                    else {
                        return CreateTokenAndAdvanceChar(ch);
                    }
                }
                else if (ch == '#') {
                    var nextch = GetNextChar();
                    if (nextch == '[') {
                        state = CreateState(StateKind.None);
                        AdvanceChar();
                        AdvanceChar();
                        return CreateToken(TokenKind.HashOpenBracket, state);
                    }
                    else {
                        return CreateTokenAndAdvanceChar(ch);
                    }
                }
                else if (ch == '$') {
                    var nextch = GetNextChar();
                    if (nextch == '{') {
                        state = CreateState(StateKind.None);
                        AdvanceChar();
                        AdvanceChar();
                        return CreateToken(TokenKind.DollarOpenBrace, state);
                    }
                    else {
                        return CreateTokenAndAdvanceChar(ch);
                    }
                }
                else {
                    return CreateTokenAndAdvanceChar(ch);
                }
            }

        }
        private bool ProcessCharEscSeq(out Token errToken) {
            var ch = GetChar();
            switch (ch) {
                case '\'': _stringBuilder.Append('\''); break;
                case '"': _stringBuilder.Append('"'); break;
                case '\\': _stringBuilder.Append('\\'); break;
                case '0': _stringBuilder.Append('\0'); break;
                case 'a': _stringBuilder.Append('\a'); break;
                case 'b': _stringBuilder.Append('\b'); break;
                case 'f': _stringBuilder.Append('\f'); break;
                case 'n': _stringBuilder.Append('\n'); break;
                case 'r': _stringBuilder.Append('\r'); break;
                case 't': _stringBuilder.Append('\t'); break;
                case 'v': _stringBuilder.Append('\v'); break;
                case 'u':
                    {
                        AdvanceChar();
                        int value = 0;
                        for (var i = 0; i < 4; ++i) {
                            ch = GetChar();
                            if (IsHexDigit(ch)) {
                                value <<= 4;
                                value |= HexValue(ch);
                                AdvanceChar();
                            }
                            else {
                                errToken = CreateErrorToken("invalid character escape sequence.");
                                return false;
                            }
                        }
                        _stringBuilder.Append((char)value);
                        errToken = default(Token);
                        return true;
                    }
                default:
                    errToken = CreateErrorToken("invalid character escape sequence.");
                    return false;
            }
            AdvanceChar();
            errToken = default(Token);
            return true;
        }
        #region helpers
        private static bool IsNewLine(char ch) {
            return ch == '\r'
                || ch == '\n'
                || ch == '\u0085'
                || ch == '\u2028'
                || ch == '\u2029';
        }
        private static bool IsWhitespace(char ch) {
            return ch == ' '
                || ch == '\t'
                || ch == '\v'
                || ch == '\f'
                || ch == '\u00A0'
                || ch == '\uFEFF'
                || ch == '\u001A'
                || (ch > 255 && CharUnicodeInfo.GetUnicodeCategory(ch) == UnicodeCategory.SpaceSeparator);
        }
        private static bool IsDecDigit(char ch) {
            return ch >= '0' && ch <= '9';
        }
        private static bool IsHexDigit(char ch) {
            return (ch >= '0' && ch <= '9') ||
                   (ch >= 'A' && ch <= 'F') ||
                   (ch >= 'a' && ch <= 'f');
        }
        private static int DecValue(char ch) {
            return ch - '0';
        }
        private static int HexValue(char ch) {
            return (ch >= '0' && ch <= '9') ? ch - '0' : (ch & 0xdf) - 'A' + 10;
        }

        public static bool IsNameStartCharacter(char ch) {
            // identifier-start-character:
            //   letter-character
            //   _ (the underscore character U+005F)

            if (ch < 'a') // '\u0061'
            {
                if (ch < 'A') // '\u0041'
                {
                    return false;
                }

                return ch <= 'Z'  // '\u005A'
                    || ch == '_'; // '\u005F'
            }

            if (ch <= 'z') // '\u007A'
            {
                return true;
            }

            if (ch <= '\u007F') // max ASCII
            {
                return false;
            }

            return IsLetterChar(CharUnicodeInfo.GetUnicodeCategory(ch));
        }
        public static bool IsNamePartCharacter(char ch) {
            // identifier-part-character:
            //   letter-character
            //   decimal-digit-character
            //   connecting-character
            //   combining-character
            //   formatting-character

            if (ch < 'a') // '\u0061'
            {
                if (ch < 'A') // '\u0041'
                {
                    return ch >= '0'  // '\u0030'
                        && ch <= '9'; // '\u0039'
                }

                return ch <= 'Z'  // '\u005A'
                    || ch == '_'; // '\u005F'
            }

            if (ch <= 'z') // '\u007A'
            {
                return true;
            }

            if (ch <= '\u007F') // max ASCII
            {
                return false;
            }

            UnicodeCategory cat = CharUnicodeInfo.GetUnicodeCategory(ch);
            return IsLetterChar(cat)
                || IsDecimalDigitChar(cat)
                || IsConnectingChar(cat)
                || IsCombiningChar(cat)
                || IsFormattingChar(cat);
        }
        private static bool IsLetterChar(UnicodeCategory cat) {
            // letter-character:
            //   A Unicode character of classes Lu, Ll, Lt, Lm, Lo, or Nl 
            //   A Unicode-escape-sequence representing a character of classes Lu, Ll, Lt, Lm, Lo, or Nl

            switch (cat) {
                case UnicodeCategory.UppercaseLetter:
                case UnicodeCategory.LowercaseLetter:
                case UnicodeCategory.TitlecaseLetter:
                case UnicodeCategory.ModifierLetter:
                case UnicodeCategory.OtherLetter:
                case UnicodeCategory.LetterNumber:
                    return true;
            }

            return false;
        }
        private static bool IsCombiningChar(UnicodeCategory cat) {
            // combining-character:
            //   A Unicode character of classes Mn or Mc 
            //   A Unicode-escape-sequence representing a character of classes Mn or Mc

            switch (cat) {
                case UnicodeCategory.NonSpacingMark:
                case UnicodeCategory.SpacingCombiningMark:
                    return true;
            }

            return false;
        }
        private static bool IsDecimalDigitChar(UnicodeCategory cat) {
            // decimal-digit-character:
            //   A Unicode character of the class Nd 
            //   A unicode-escape-sequence representing a character of the class Nd

            return cat == UnicodeCategory.DecimalDigitNumber;
        }
        private static bool IsConnectingChar(UnicodeCategory cat) {
            // connecting-character:  
            //   A Unicode character of the class Pc
            //   A unicode-escape-sequence representing a character of the class Pc

            return cat == UnicodeCategory.ConnectorPunctuation;
        }
        private static bool IsFormattingChar(UnicodeCategory cat) {
            // formatting-character:  
            //   A Unicode character of the class Cf
            //   A unicode-escape-sequence representing a character of the class Cf

            return cat == UnicodeCategory.Format;
        }
        #endregion helpers
    }


}
