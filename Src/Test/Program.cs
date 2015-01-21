using System;
using System.IO;
using System.Text;
using XData;
using XData.IO.Text;

namespace Test {
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
