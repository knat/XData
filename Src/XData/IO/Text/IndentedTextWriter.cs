using System;
using System.IO;

namespace XData.IO.Text {
    public sealed class IndentedTextWriter {
        public IndentedTextWriter(TextWriter textWriter, string indentString = "\t", string newLineString = "\n") {
            if (textWriter == null) throw new ArgumentNullException("textWriter");
            if (string.IsNullOrEmpty(indentString)) throw new ArgumentNullException("indentString");
            if (string.IsNullOrEmpty(newLineString)) throw new ArgumentNullException("newLineString");
            TextWriter = textWriter;
            IndentString = indentString;
            NewLineString = newLineString;
            _isNewLine = true;
        }
        public readonly TextWriter TextWriter;
        public readonly string IndentString;
        public readonly string NewLineString;
        private int _indentCount;
        private bool _isNewLine;
        public int IndentCount {
            get {
                return _indentCount;
            }
        }
        public IndentedTextWriter PushIndent(int count = 1) {
            if ((_indentCount += count) < 0) throw new ArgumentOutOfRangeException("count");
            return this;
        }
        public IndentedTextWriter PopIndent(int count = 1) {
            if ((_indentCount -= count) < 0) throw new ArgumentOutOfRangeException("count");
            return this;
        }
        private void WriteIndent() {
            if (_isNewLine) {
                for (var i = 0; i < _indentCount; ++i) {
                    TextWriter.Write(IndentString);
                }
                _isNewLine = false;
            }
        }
        public IndentedTextWriter WriteLine() {
            WriteIndent();
            TextWriter.Write(NewLineString);
            _isNewLine = true;
            return this;
        }
        public IndentedTextWriter Write(string s) {
            WriteIndent();
            TextWriter.Write(s);
            return this;
        }
        public IndentedTextWriter WriteLine(string s) {
            WriteIndent();
            TextWriter.Write(s);
            TextWriter.Write(NewLineString);
            _isNewLine = true;
            return this;
        }
        public IndentedTextWriter Write(char ch) {
            WriteIndent();
            TextWriter.Write(ch);
            return this;
        }
        public IndentedTextWriter WriteLine(char ch) {
            WriteIndent();
            TextWriter.Write(ch);
            TextWriter.Write(NewLineString);
            _isNewLine = true;
            return this;
        }


    }
}
