using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace XData {
    internal static class EX {
        private const int _stringBuilderCount = 4;
        private const int _stringBuilderCapacity = 128;
        private static readonly StringBuilder[] _stringBuilders = new StringBuilder[_stringBuilderCount];
        public static StringBuilder AcquireStringBuilder() {
            var sbs = _stringBuilders;
            StringBuilder sb = null;
            lock (_stringBuilders) {
                for (var i = 0; i < _stringBuilderCount; ++i) {
                    sb = sbs[i];
                    if (sb != null) {
                        sbs[i] = null;
                        break;
                    }
                }
            }
            if (sb != null) {
                sb.Clear();
                return sb;
            }
            return new StringBuilder(_stringBuilderCapacity);
        }
        public static void ReleaseStringBuilder(this StringBuilder sb) {
            if (sb != null && sb.Capacity <= _stringBuilderCapacity * 8) {
                var sbs = _stringBuilders;
                lock (_stringBuilders) {
                    for (var i = 0; i < _stringBuilderCount; ++i) {
                        if (sbs[i] == null) {
                            sbs[i] = sb;
                            return;
                        }
                    }
                }
            }
        }
        public static string ToStringAndRelease(this StringBuilder sb) {
            var str = sb.ToString();
            ReleaseStringBuilder(sb);
            return str;
        }
        public static string InvFormat(this string format, params string[] args) {
            return AcquireStringBuilder().AppendFormat(CultureInfo.InvariantCulture, format, args).ToStringAndRelease();
        }
        //
        public static string ToInvString(this decimal i) {
            return i.ToString(CultureInfo.InvariantCulture);
        }
        public static string ToInvString(this long i) {
            return i.ToString(CultureInfo.InvariantCulture);
        }
        public static string ToInvString(this int i) {
            return i.ToString(CultureInfo.InvariantCulture);
        }
        public static string ToInvString(this short i) {
            return i.ToString(CultureInfo.InvariantCulture);
        }
        public static string ToInvString(this sbyte i) {
            return i.ToString(CultureInfo.InvariantCulture);
        }
        public static string ToInvString(this ulong i) {
            return i.ToString(CultureInfo.InvariantCulture);
        }
        public static string ToInvString(this uint i) {
            return i.ToString(CultureInfo.InvariantCulture);
        }
        public static string ToInvString(this ushort i) {
            return i.ToString(CultureInfo.InvariantCulture);
        }
        public static string ToInvString(this byte i) {
            return i.ToString(CultureInfo.InvariantCulture);
        }
        public static bool TryToInvDecimal(this string s, out decimal result) {
            return decimal.TryParse(s, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, NumberFormatInfo.InvariantInfo, out result);
        }
        public static bool TryToInvInt64(this string s, out long result) {
            return long.TryParse(s, NumberStyles.AllowLeadingSign, NumberFormatInfo.InvariantInfo, out result);
        }
        public static bool TryToInvInt32(this string s, out int result) {
            return int.TryParse(s, NumberStyles.AllowLeadingSign, NumberFormatInfo.InvariantInfo, out result);
        }
        public static bool TryToInvInt16(this string s, out short result) {
            return short.TryParse(s, NumberStyles.AllowLeadingSign, NumberFormatInfo.InvariantInfo, out result);
        }
        public static bool TryToInvSByte(this string s, out sbyte result) {
            return sbyte.TryParse(s, NumberStyles.AllowLeadingSign, NumberFormatInfo.InvariantInfo, out result);
        }
        public static bool TryToInvUInt64(this string s, out ulong result) {
            return ulong.TryParse(s, NumberStyles.AllowLeadingSign, NumberFormatInfo.InvariantInfo, out result);
        }
        public static bool TryToInvUInt32(this string s, out uint result) {
            return uint.TryParse(s, NumberStyles.AllowLeadingSign, NumberFormatInfo.InvariantInfo, out result);
        }
        public static bool TryToInvUInt16(this string s, out ushort result) {
            return ushort.TryParse(s, NumberStyles.AllowLeadingSign, NumberFormatInfo.InvariantInfo, out result);
        }
        public static bool TryToInvByte(this string s, out byte result) {
            return byte.TryParse(s, NumberStyles.AllowLeadingSign, NumberFormatInfo.InvariantInfo, out result);
        }
        //
        public static void GetLiteral(string value, StringBuilder sb) {
            var length = value.Length;
            if (length == 0) {
                sb.Append("\"\"");
            }
            else {
                sb.Append("@\"");
                for (var i = 0; i < length; ++i) {
                    var ch = value[i];
                    if (ch == '"') {
                        sb.Append("\"\"");
                    }
                    else {
                        sb.Append(ch);
                    }
                }
                sb.Append('"');
            }
        }
        public static string ToLiteral(this string value) {
            var sb = AcquireStringBuilder();
            GetLiteral(value, sb);
            return sb.ToStringAndRelease();
        }

        //
        public static int AggregateHash(int hash, int newValue) {
            unchecked {
                return hash * 31 + newValue;
            }
        }
        public static int CombineHash(int a, int b) {
            unchecked {
                int hash = 17;
                hash = hash * 31 + a;
                hash = hash * 31 + b;
                return hash;
            }
        }
        public static int CombineHash(int a, int b, int c) {
            unchecked {
                int hash = 17;
                hash = hash * 31 + a;
                hash = hash * 31 + b;
                hash = hash * 31 + c;
                return hash;
            }
        }
        //
        public static void CreateAndAdd<T>(ref List<T> list, T item) {
            if (list == null) {
                list = new List<T>();
            }
            list.Add(item);
        }
        public static int CountOrZero<T>(this List<T> list) {
            return list == null ? 0 : list.Count;
        }
        public static bool UriEquals(string uri1, string uri2) {
            if (string.IsNullOrEmpty(uri1)) {
                return string.IsNullOrEmpty(uri2);
            }
            return uri1 == uri2;
        }
        //
        //
        //
        public const TypeKind TypeStart = TypeKind.ComplexType;
        public const TypeKind TypeEnd = TypeKind.DateTimeOffset;
        public const TypeKind ConcreteAtomTypeStart = TypeKind.String;
        public const TypeKind ConcreteAtomTypeEnd = TypeKind.DateTimeOffset;
        public static bool IsConcreteAtomType(this TypeKind kind) {
            return kind >= ConcreteAtomTypeStart && kind <= ConcreteAtomTypeEnd;
        }
        public static FullName ToFullName(this TypeKind kind) {
            return new FullName(Extensions.SystemUri, kind.ToString());
        }
        public static AtomTypeInfo ToAtomTypeInfo(this TypeKind kind, Type clrType, AtomTypeInfo baseType) {
            return new AtomTypeInfo(clrType, false, ToFullName(kind), baseType, null, kind);
        }


    }
}
