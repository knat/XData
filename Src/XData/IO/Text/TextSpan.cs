namespace XData.IO.Text {

    public struct TextSpan {
        public TextSpan(string filePath, int startIndex, int length, TextPosition startPosition, TextPosition endPosition) {
            //if (filePath == null) throw new ArgumentNullException("filePath");
            //if (startIndex < 0) throw new ArgumentOutOfRangeException("startIndex");
            //if (length < 0) throw new ArgumentOutOfRangeException("length");
            FilePath = filePath;
            StartIndex = startIndex;
            Length = length;
            StartPosition = startPosition;
            EndPosition = endPosition;
        }
        public TextSpan(string filePath, Token token)
            : this(filePath, token.StartIndex, token.Length, token.StartPosition, token.EndPosition) {
        }
        public readonly string FilePath;
        public readonly int StartIndex;
        public readonly int Length;
        public readonly TextPosition StartPosition;
        public readonly TextPosition EndPosition;
        public bool IsValid {
            get {
                return FilePath != null;
            }
        }
        public override string ToString() {
            if (IsValid) {
                return FilePath + ": (" + StartPosition.ToString() + ")-(" + EndPosition.ToString() + ")";
            }
            return null;
        }
    }

    public struct TextPosition {
        public TextPosition(int line, int column) {
            //if (line < 1) throw new ArgumentOutOfRangeException("line");
            //if (column < 1) throw new ArgumentOutOfRangeException("column");
            Line = line;
            Column = column;
        }
        public readonly int Line;//1-based
        public readonly int Column;//1-based
        public bool IsValid {
            get {
                return Line > 0 && Column > 0;
            }
        }
        public override string ToString() {
            return Line + "," + Column;
        }
    }

}
