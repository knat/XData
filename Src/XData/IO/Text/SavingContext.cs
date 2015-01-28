using System;
using System.Collections.Generic;
using System.Text;

namespace XData.IO.Text {
    public class IndentedStringBuilder {
        public IndentedStringBuilder(StringBuilder stringBuilder, string indentString = DefaultIndentString, string newLineString = DefaultNewLineString) {
            if (stringBuilder == null) throw new ArgumentNullException("stringBuilder");
            if (string.IsNullOrEmpty(indentString)) throw new ArgumentNullException("indentString");
            if (string.IsNullOrEmpty(newLineString)) throw new ArgumentNullException("newLineString");
            StringBuilder = stringBuilder;
            IndentString = indentString;
            NewLineString = newLineString;
            _atNewLine = true;
        }
        public const string DefaultIndentString = "\t";
        public const string DefaultNewLineString = "\n";
        public readonly StringBuilder StringBuilder;
        public readonly string IndentString;
        public readonly string NewLineString;
        private int _indentCount;
        private bool _atNewLine;
        public int IndentCount {
            get {
                return _indentCount;
            }
        }
        public bool AtNewLine {
            get {
                return _atNewLine;
            }
        }
        public void PushIndent(int count = 1) {
            if ((_indentCount += count) < 0) throw new ArgumentOutOfRangeException("count");
        }
        public void PopIndent(int count = 1) {
            if ((_indentCount -= count) < 0) throw new ArgumentOutOfRangeException("count");
        }
        public void AppendIndents() {
            if (_atNewLine) {
                for (var i = 0; i < _indentCount; ++i) {
                    StringBuilder.Append(IndentString);
                }
                _atNewLine = false;
            }
        }
        public void Append(string s) {
            AppendIndents();
            StringBuilder.Append(s);
        }
        public void Append(char ch) {
            AppendIndents();
            StringBuilder.Append(ch);
        }
        public void AppendLine() {
            StringBuilder.Append(NewLineString);
            _atNewLine = true;
        }
        public void AppendLine(string s) {
            Append(s);
            AppendLine();
        }
        public void AppendLine(char ch) {
            Append(ch);
            AppendLine();
        }
    }
    public class SavingContext : IndentedStringBuilder {
        public SavingContext(StringBuilder stringBuilder, string indentString = DefaultIndentString, string newLineString = DefaultNewLineString) :
            base(stringBuilder, indentString, newLineString) {
            _aliasUriList = new List<AliasUri>();
        }
        public SavingContext(string indentString = DefaultIndentString, string newLineString = DefaultNewLineString) :
            this(new StringBuilder(StringBuilderCapacity), indentString, newLineString) {
        }
        public const int StringBuilderCapacity = 1024 * 2;
        private struct AliasUri {
            public AliasUri(string alias, string uri) {
                Alias = alias;
                Uri = uri;
            }
            public readonly string Alias, Uri;
        }
        private readonly List<AliasUri> _aliasUriList;
        public string AddUri(string uri) {
            if (string.IsNullOrEmpty(uri)) {
                return null;
            }
            if (uri == InfoExtensions.SystemUri) {
                return "sys";
            }
            foreach (var au in _aliasUriList) {
                if (au.Uri == uri) {
                    return au.Alias;
                }
            }
            var alias = "a" + _aliasUriList.Count.ToInvString();
            _aliasUriList.Add(new AliasUri(alias, uri));
            return alias;
        }
        public void Append(FullName fullName) {
            var alias = AddUri(fullName.Uri);
            if (alias != null) {
                Append(alias);
                StringBuilder.Append(':');
            }
            Append(fullName.Name);
        }


    }

}
