using System;
using System.IO;
using XData;
using XData.TextIO;

namespace Test {
    class Program {
        static void Main(string[] args) {
            var text = File.ReadAllText("test.txt");
            var ctx = new Context();
            ElementNode element;
            Parser.Parse("test.txt", text.ToCharArray(), ctx, out element);
            foreach (var diag in ctx.Diagnostics) {
                Console.WriteLine(diag.ToString());
            }
        }
    }
}
