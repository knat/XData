using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace XData {
    public static class Extensions {
        public const string SystemUri = "http://xdata-io.org";
        //
        [System.Diagnostics.ConditionalAttribute("DEBUG")]
        public static void PublicParameterlessConstructorRequired<T>() where T : new() { }
        //
        public static IEnumerable<T> Ancestors<T>(this IEnumerable<XObject> source,
            Func<T, bool> filter = null) where T : XObject {
            if (source == null) throw new ArgumentNullException("source");
            foreach (var i in source) {
                foreach (var j in i.Ancestors(filter)) {
                    yield return j;
                }
            }
        }
        #region complex type
        public static IEnumerable<T> SelfAttributes<T>(this IEnumerable<XComplexType> source,
            Func<T, bool> filter = null) where T : XAttribute {
            if (source == null) throw new ArgumentNullException("source");
            foreach (var i in source) {
                foreach (var j in i.SelfAttributes(filter)) {
                    yield return j;
                }
            }
        }
        public static IEnumerable<T> SelfAttributeTypes<T>(this IEnumerable<XComplexType> source,
            Func<XAttribute, bool> attributeFilter = null, Func<T, bool> typeFilter = null) where T : XSimpleType {
            if (source == null) throw new ArgumentNullException("source");
            foreach (var i in source) {
                foreach (var j in i.SelfAttributeTypes(attributeFilter, typeFilter)) {
                    yield return j;
                }
            }
        }
        public static IEnumerable<T> SubElements<T>(this IEnumerable<XComplexType> source,
            Func<T, bool> filter = null) where T : XElement {
            if (source == null) throw new ArgumentNullException("source");
            foreach (var i in source) {
                foreach (var j in i.SubElements(filter)) {
                    yield return j;
                }
            }
        }
        public static IEnumerable<T> SubElementTypes<T>(this IEnumerable<XComplexType> source,
            Func<XElement, bool> elementFilter = null, Func<T, bool> typeFilter = null) where T : XType {
            if (source == null) throw new ArgumentNullException("source");
            foreach (var i in source) {
                foreach (var j in i.SubElementTypes(elementFilter, typeFilter)) {
                    yield return j;
                }
            }
        }
        public static IEnumerable<T> SubAttributes<T>(this IEnumerable<XComplexType> source,
            Func<XElement, bool> elementFilter = null, Func<T, bool> attributeFilter = null) where T : XAttribute {
            if (source == null) throw new ArgumentNullException("source");
            foreach (var i in source) {
                foreach (var j in i.SubElementAttributes(elementFilter, attributeFilter)) {
                    yield return j;
                }
            }
        }
        public static IEnumerable<T> SubElementAttributeTypes<T>(this IEnumerable<XComplexType> source,
            Func<XElement, bool> elementFilter = null, Func<XAttribute, bool> attributeFilter = null,
            Func<T, bool> typeFilter = null) where T : XSimpleType {
            if (source == null) throw new ArgumentNullException("source");
            foreach (var i in source) {
                foreach (var j in i.SubElementAttributeTypes(elementFilter, attributeFilter, typeFilter)) {
                    yield return j;
                }
            }
        }
        public static IEnumerable<T> SubElementChildren<T>(this IEnumerable<XComplexType> source,
            Func<XElement, bool> elementFilter = null, Func<T, bool> childrenFilter = null) where T : XObject {
            if (source == null) throw new ArgumentNullException("source");
            foreach (var i in source) {
                foreach (var j in i.SubElementChildren(elementFilter, childrenFilter)) {
                    yield return j;
                }
            }
        }
        public static IEnumerable<T> DescendantElements<T>(this IEnumerable<XComplexType> source,
            Func<T, bool> filter = null) where T : XElement {
            if (source == null) throw new ArgumentNullException("source");
            foreach (var i in source) {
                foreach (var j in i.DescendantElements(filter)) {
                    yield return j;
                }
            }
        }
        public static IEnumerable<T> DescendantElementTypes<T>(this IEnumerable<XComplexType> source,
            Func<XElement, bool> elementFilter = null, Func<T, bool> typeFilter = null) where T : XType {
            if (source == null) throw new ArgumentNullException("source");
            foreach (var i in source) {
                foreach (var j in i.DescendantElementTypes(elementFilter, typeFilter)) {
                    yield return j;
                }
            }
        }
        public static IEnumerable<T> DescendantElementAttributes<T>(this IEnumerable<XComplexType> source,
            Func<XElement, bool> elementFilter = null, Func<T, bool> attributeFilter = null) where T : XAttribute {
            if (source == null) throw new ArgumentNullException("source");
            foreach (var i in source) {
                foreach (var j in i.DescendantElementAttributes(elementFilter, attributeFilter)) {
                    yield return j;
                }
            }
        }
        public static IEnumerable<T> DescendantElementAttributeTypes<T>(this IEnumerable<XComplexType> source,
            Func<XElement, bool> elementFilter = null, Func<XAttribute, bool> attributeFilter = null,
            Func<T, bool> typeFilter = null) where T : XSimpleType {
            if (source == null) throw new ArgumentNullException("source");
            foreach (var i in source) {
                foreach (var j in i.DescendantElementAttributeTypes(elementFilter, attributeFilter, typeFilter)) {
                    yield return j;
                }
            }
        }
        public static IEnumerable<T> DescendantElementChildren<T>(this IEnumerable<XComplexType> source,
            Func<XElement, bool> elementFilter = null, Func<T, bool> childrenFilter = null) where T : XObject {
            if (source == null) throw new ArgumentNullException("source");
            foreach (var i in source) {
                foreach (var j in i.DescendantElementChildren(elementFilter, childrenFilter)) {
                    yield return j;
                }
            }
        }

        #endregion complex type
        #region element
        public static IEnumerable<T> SelfAttributes<T>(this IEnumerable<XElement> source,
            Func<T, bool> filter = null) where T : XAttribute {
            if (source == null) throw new ArgumentNullException("source");
            foreach (var i in source) {
                foreach (var j in i.SelfAttributes(filter)) {
                    yield return j;
                }
            }
        }
        public static IEnumerable<T> SelfAttributeTypes<T>(this IEnumerable<XElement> source,
            Func<XAttribute, bool> attributeFilter = null, Func<T, bool> typeFilter = null) where T : XSimpleType {
            if (source == null) throw new ArgumentNullException("source");
            foreach (var i in source) {
                foreach (var j in i.SelfAttributeTypes(attributeFilter, typeFilter)) {
                    yield return j;
                }
            }
        }
        public static IEnumerable<T> SubElements<T>(this IEnumerable<XElement> source,
            Func<T, bool> filter = null) where T : XElement {
            if (source == null) throw new ArgumentNullException("source");
            foreach (var i in source) {
                foreach (var j in i.SubElements(filter)) {
                    yield return j;
                }
            }
        }
        public static IEnumerable<T> SubElementTypes<T>(this IEnumerable<XElement> source,
            Func<XElement, bool> elementFilter = null, Func<T, bool> typeFilter = null) where T : XType {
            if (source == null) throw new ArgumentNullException("source");
            foreach (var i in source) {
                foreach (var j in i.SubElementTypes(elementFilter, typeFilter)) {
                    yield return j;
                }
            }
        }
        public static IEnumerable<T> SubAttributes<T>(this IEnumerable<XElement> source,
            Func<XElement, bool> elementFilter = null, Func<T, bool> attributeFilter = null) where T : XAttribute {
            if (source == null) throw new ArgumentNullException("source");
            foreach (var i in source) {
                foreach (var j in i.SubElementAttributes(elementFilter, attributeFilter)) {
                    yield return j;
                }
            }
        }
        public static IEnumerable<T> SubElementAttributeTypes<T>(this IEnumerable<XElement> source,
            Func<XElement, bool> elementFilter = null, Func<XAttribute, bool> attributeFilter = null,
            Func<T, bool> typeFilter = null) where T : XSimpleType {
            if (source == null) throw new ArgumentNullException("source");
            foreach (var i in source) {
                foreach (var j in i.SubElementAttributeTypes(elementFilter, attributeFilter, typeFilter)) {
                    yield return j;
                }
            }
        }
        public static IEnumerable<T> SubElementChildren<T>(this IEnumerable<XElement> source,
            Func<XElement, bool> elementFilter = null, Func<T, bool> childrenFilter = null) where T : XObject {
            if (source == null) throw new ArgumentNullException("source");
            foreach (var i in source) {
                foreach (var j in i.SubElementChildren(elementFilter, childrenFilter)) {
                    yield return j;
                }
            }
        }
        public static IEnumerable<T> DescendantElements<T>(this IEnumerable<XElement> source,
            Func<T, bool> filter = null) where T : XElement {
            if (source == null) throw new ArgumentNullException("source");
            foreach (var i in source) {
                foreach (var j in i.DescendantElements(filter)) {
                    yield return j;
                }
            }
        }
        public static IEnumerable<T> DescendantElementTypes<T>(this IEnumerable<XElement> source,
            Func<XElement, bool> elementFilter = null, Func<T, bool> typeFilter = null) where T : XType {
            if (source == null) throw new ArgumentNullException("source");
            foreach (var i in source) {
                foreach (var j in i.DescendantElementTypes(elementFilter, typeFilter)) {
                    yield return j;
                }
            }
        }
        public static IEnumerable<T> DescendantElementAttributes<T>(this IEnumerable<XElement> source,
            Func<XElement, bool> elementFilter = null, Func<T, bool> attributeFilter = null) where T : XAttribute {
            if (source == null) throw new ArgumentNullException("source");
            foreach (var i in source) {
                foreach (var j in i.DescendantElementAttributes(elementFilter, attributeFilter)) {
                    yield return j;
                }
            }
        }
        public static IEnumerable<T> DescendantElementAttributeTypes<T>(this IEnumerable<XElement> source,
            Func<XElement, bool> elementFilter = null, Func<XAttribute, bool> attributeFilter = null,
            Func<T, bool> typeFilter = null) where T : XSimpleType {
            if (source == null) throw new ArgumentNullException("source");
            foreach (var i in source) {
                foreach (var j in i.DescendantElementAttributeTypes(elementFilter, attributeFilter, typeFilter)) {
                    yield return j;
                }
            }
        }
        public static IEnumerable<T> DescendantElementChildren<T>(this IEnumerable<XElement> source,
            Func<XElement, bool> elementFilter = null, Func<T, bool> childrenFilter = null) where T : XObject {
            if (source == null) throw new ArgumentNullException("source");
            foreach (var i in source) {
                foreach (var j in i.DescendantElementChildren(elementFilter, childrenFilter)) {
                    yield return j;
                }
            }
        }

        #endregion element

        //
        internal const string DefaultIndentString = "\t";
        internal const string DefaultNewLineString = "\n";
        //
        //
        //
        private const int _stringBuilderCount = 4;
        private const int _stringBuilderCapacity = 128;
        private static readonly StringBuilder[] _stringBuilders = new StringBuilder[_stringBuilderCount];
        internal static StringBuilder AcquireStringBuilder() {
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
        internal static void ReleaseStringBuilder(this StringBuilder sb) {
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
        internal static string ToStringAndRelease(this StringBuilder sb) {
            var str = sb.ToString();
            ReleaseStringBuilder(sb);
            return str;
        }
        internal static string InvFormat(this string format, params string[] args) {
            return AcquireStringBuilder().AppendFormat(CultureInfo.InvariantCulture, format, args).ToStringAndRelease();
        }
        //
        internal static string ToInvString(this decimal i) {
            return i.ToString(CultureInfo.InvariantCulture);
        }
        internal static string ToInvString(this long i) {
            return i.ToString(CultureInfo.InvariantCulture);
        }
        internal static string ToInvString(this int i) {
            return i.ToString(CultureInfo.InvariantCulture);
        }
        internal static string ToInvString(this short i) {
            return i.ToString(CultureInfo.InvariantCulture);
        }
        internal static string ToInvString(this sbyte i) {
            return i.ToString(CultureInfo.InvariantCulture);
        }
        internal static string ToInvString(this ulong i) {
            return i.ToString(CultureInfo.InvariantCulture);
        }
        internal static string ToInvString(this uint i) {
            return i.ToString(CultureInfo.InvariantCulture);
        }
        internal static string ToInvString(this ushort i) {
            return i.ToString(CultureInfo.InvariantCulture);
        }
        internal static string ToInvString(this byte i) {
            return i.ToString(CultureInfo.InvariantCulture);
        }
        internal static bool TryToInvDecimal(this string s, out decimal result) {
            return decimal.TryParse(s, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, NumberFormatInfo.InvariantInfo, out result);
        }
        internal static bool TryToInvInt64(this string s, out long result) {
            return long.TryParse(s, NumberStyles.AllowLeadingSign, NumberFormatInfo.InvariantInfo, out result);
        }
        internal static bool TryToInvInt32(this string s, out int result) {
            return int.TryParse(s, NumberStyles.AllowLeadingSign, NumberFormatInfo.InvariantInfo, out result);
        }
        internal static bool TryToInvInt16(this string s, out short result) {
            return short.TryParse(s, NumberStyles.AllowLeadingSign, NumberFormatInfo.InvariantInfo, out result);
        }
        internal static bool TryToInvSByte(this string s, out sbyte result) {
            return sbyte.TryParse(s, NumberStyles.AllowLeadingSign, NumberFormatInfo.InvariantInfo, out result);
        }
        internal static bool TryToInvUInt64(this string s, out ulong result) {
            return ulong.TryParse(s, NumberStyles.AllowLeadingSign, NumberFormatInfo.InvariantInfo, out result);
        }
        internal static bool TryToInvUInt32(this string s, out uint result) {
            return uint.TryParse(s, NumberStyles.AllowLeadingSign, NumberFormatInfo.InvariantInfo, out result);
        }
        internal static bool TryToInvUInt16(this string s, out ushort result) {
            return ushort.TryParse(s, NumberStyles.AllowLeadingSign, NumberFormatInfo.InvariantInfo, out result);
        }
        internal static bool TryToInvByte(this string s, out byte result) {
            return byte.TryParse(s, NumberStyles.AllowLeadingSign, NumberFormatInfo.InvariantInfo, out result);
        }
        //
        internal static void GetLiteral(string value, StringBuilder sb) {
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
        internal static string ToLiteral(this string value) {
            var sb = AcquireStringBuilder();
            GetLiteral(value, sb);
            return sb.ToStringAndRelease();
        }

        //
        internal static int AggregateHash(int hash, int newValue) {
            unchecked {
                return hash * 31 + newValue;
            }
        }
        internal static int CombineHash(int a, int b) {
            unchecked {
                int hash = 17;
                hash = hash * 31 + a;
                hash = hash * 31 + b;
                return hash;
            }
        }
        internal static int CombineHash(int a, int b, int c) {
            unchecked {
                int hash = 17;
                hash = hash * 31 + a;
                hash = hash * 31 + b;
                hash = hash * 31 + c;
                return hash;
            }
        }
        //
        internal static void CreateAndAdd<T>(ref List<T> list, T item) {
            if (list == null) {
                list = new List<T>();
            }
            list.Add(item);
        }
        internal static int CountOrZero<T>(this List<T> list) {
            return list == null ? 0 : list.Count;
        }
        //
        internal static bool UriEquals(string uri1, string uri2) {
            if (string.IsNullOrEmpty(uri1)) {
                return string.IsNullOrEmpty(uri2);
            }
            return uri1 == uri2;
        }
        //
        //
        //

        internal const TypeKind TypeStart = TypeKind.ComplexType;
        internal const TypeKind TypeEnd = TypeKind.DateTimeOffset;
        internal const TypeKind ConcreteAtomTypeStart = TypeKind.String;
        internal const TypeKind ConcreteAtomTypeEnd = TypeKind.DateTimeOffset;
        internal static bool IsConcreteAtomType(this TypeKind kind) {
            return kind >= ConcreteAtomTypeStart && kind <= ConcreteAtomTypeEnd;
        }
        internal static FullName ToFullName(this TypeKind kind) {
            return new FullName(Extensions.SystemUri, kind.ToString());
        }
        internal static AtomTypeInfo ToAtomTypeInfo(this TypeKind kind, Type clrType, AtomTypeInfo baseType) {
            return new AtomTypeInfo(clrType, false, ToFullName(kind), baseType, null, kind);
        }


    }
}
