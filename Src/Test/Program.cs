using System;
using System.Collections.Generic;
using System.IO;
using XData;
using XData.IO.Text;

namespace Test {
    public class Int64List : XListType<XInt64> {

        public override ObjectInfo ObjectInfo {
            get {
                throw new NotImplementedException();
            }
        }
    }
    public class Int32List : Int64List, IList<XInt32>, IReadOnlyList<XInt32> {

        public bool Contains(XInt32 item) {
            return base.Contains(item);
        }
        public int IndexOf(XInt32 item) {
            return base.IndexOf(item);
        }
        public void Add(XInt32 item) {
            base.Add(item);
        }
        public void Insert(int index, XInt32 item) {
            base.Insert(index, item);
        }
        new public XInt32 this[int index] {
            get {
                return base[index] as XInt32;
            }

            set {
                base[index] = value;
            }
        }
        public bool Remove(XInt32 item) {
            return base.Remove(item);
        }
        new public IEnumerator<XInt32> GetEnumerator() {
            return GetEnumeratorCore<XInt32>();
        }
        public void CopyTo(XInt32[] array, int arrayIndex) {
            CopyToCore(array, arrayIndex);
        }



    }

    class Program {
        static void Main(string[] args) {
            new System.Data.SqlTypes.SqlDecimal();
            //TestLexer();
            TestParser();
        }

        static void TestLexer() {
            using (var reader = new StreamReader("test.txt")) {
                var lexer = Lexer.Get(reader);
                while (true) {
                    Token token = lexer.GetToken();
                    Console.WriteLine("Kind={0},startIdx={1},length={2},StartPos={3},EndPos={4},Value={5}",
                        token.RawKind, token.StartIndex, token.Length, token.StartPosition.ToString(), token.EndPosition.ToString(), token.Value);
                    if (token.IsError || token.IsEndOfFile) {
                        break;
                    }
                }
            }
        }
        static void TestParser() {
            var ctx = new Context();
            ElementNode element;
            using (var reader = new StreamReader("test.txt")) {
                Parser.Parse("test.txt", reader, ctx, out element);
            }
            foreach (var diag in ctx.DiagList) {
                Console.WriteLine(diag.ToString());
            }

        }
    }
}
