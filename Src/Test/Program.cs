using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        static void Main() {
            var list = new List<int> { 1, 2, 3, 4 };
            for(var i = 0; i < list.Count; ++i) {
                if (list[i] == 3) {
                    list.RemoveAt(i);
                    break;
                }
            }
        }

        static byte GetPAndS(decimal d, out byte s) {
            var sqld = new System.Data.SqlTypes.SqlDecimal(d);
            s = sqld.Scale;
            return sqld.Precision;
        }
        static byte GetPAndS2(decimal d, out byte s) {
            byte precision = 0;
            byte scale = 0;
            var inFraction = false;
            var isLeadingZero = true;
            foreach (var ch in d.ToInvString()) {
                if (inFraction) {
                    ++precision;
                    ++scale;
                }
                else if (ch == '.') {
                    isLeadingZero = false;
                    inFraction = true;
                }
                else if (ch >= '1' && ch <= '9') {
                    isLeadingZero = false;
                    ++precision;
                }
                else if (ch == '0' && !isLeadingZero) {
                    ++precision;
                }
            }
            if (isLeadingZero) {
                ++precision;
            }
            s = scale;
            return precision;
        }
        static void Display(decimal d) {
            byte p, s;
            p = GetPAndS(d, out s);
           // Console.WriteLine("d: {0}, P={1}, S={2}", d.ToInvString(), p.ToInvString(), s.ToInvString());
        }
        static void Display2(decimal d) {
            byte p, s;
            p = GetPAndS2(d, out s);
           // Console.WriteLine("d: {0}, P={1}, S={2}", d.ToInvString(), p.ToInvString(), s.ToInvString());
        }
        static void TestDecimal() {
            Stopwatch sw1 = new Stopwatch();
            sw1.Start();
            for (var i = 0; i < 10000; ++i) {
                Display(-0M);
                Display(0.0M);
                Display(0012M);
                Display(-0.1M);
                Display(-.12M);
                Display(+.120M);
                Display(-.120M);
                Display(12.34M);
                Display(12.0M);
                Display(12.3400M);
                Display(-12.3400M);
            }
            sw1.Stop();

            Console.WriteLine("======");
            Stopwatch sw2 = new Stopwatch();
            sw2.Start();
            for (var i = 0; i < 10000; ++i) {

                Display2(-0M);
                Display2(0.0M);
                Display2(0012M);
                Display2(-0.1M);
                Display2(-.12M);
                Display2(+.120M);
                Display2(-.120M);
                Display2(12.34M);
                Display2(12.0M);
                Display2(12.3400M);
                Display2(-12.3400M);
            }
            sw2.Stop();
            Console.WriteLine(sw1.ElapsedMilliseconds);
            Console.WriteLine(sw2.ElapsedMilliseconds);
            //var sb = new StringBuilder(128);
            //for (var i = 0; i < 800; ++i) {
            //    sb.Append('a');
            //}
            //sb.Capacity = 32;

            //new System.Data.SqlTypes.SqlDecimal();
            //TestLexer();
            //TestParser();
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
            var ctx = new DiagContext();
            ElementNode element;
            using (var reader = new StreamReader("test.txt")) {
                Parser.Parse("test.txt", reader, ctx, out element);
            }
            foreach (var diag in ctx) {
                Console.WriteLine(diag.ToString());
            }

        }
    }
}
