using System;
using System.Text;

namespace XData {
    public class IndentedStringBuilder {
        public IndentedStringBuilder(StringBuilder stringBuilder, string indentString = "\t", string newLineString = "\n") {
            if (stringBuilder == null) throw new ArgumentNullException("stringBuilder");
            if (string.IsNullOrEmpty(indentString)) throw new ArgumentNullException("indentString");
            if (string.IsNullOrEmpty(newLineString)) throw new ArgumentNullException("newLineString");
            StringBuilder = stringBuilder;
            IndentString = indentString;
            NewLineString = newLineString;
            _isNewLine = true;
        }
        public readonly StringBuilder StringBuilder;
        public readonly string IndentString;
        public readonly string NewLineString;
        private int _indentCount;
        private bool _isNewLine;
        public int IndentCount {
            get {
                return _indentCount;
            }
        }
        public IndentedStringBuilder PushIndent(int count = 1) {
            if ((_indentCount += count) < 0) throw new ArgumentOutOfRangeException("count");
            return this;
        }
        public IndentedStringBuilder PopIndent(int count = 1) {
            if ((_indentCount -= count) < 0) throw new ArgumentOutOfRangeException("count");
            return this;
        }
        private void AppendIndents() {
            if (_isNewLine) {
                for (var i = 0; i < _indentCount; ++i) {
                    StringBuilder.Append(IndentString);
                }
                _isNewLine = false;
            }
        }
        public IndentedStringBuilder AppendLine() {
            AppendIndents();
            StringBuilder.Append(NewLineString);
            _isNewLine = true;
            return this;
        }
        public IndentedStringBuilder Append(string s) {
            AppendIndents();
            StringBuilder.Append(s);
            return this;
        }
        public IndentedStringBuilder AppendLine(string s) {
            AppendIndents();
            StringBuilder.Append(s);
            StringBuilder.Append(NewLineString);
            _isNewLine = true;
            return this;
        }
        public IndentedStringBuilder Append(char ch) {
            AppendIndents();
            StringBuilder.Append(ch);
            return this;
        }
        public IndentedStringBuilder AppendLine(char ch) {
            AppendIndents();
            StringBuilder.Append(ch);
            StringBuilder.Append(NewLineString);
            _isNewLine = true;
            return this;
        }


    }
}
