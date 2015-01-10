using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using XData.TextIO;

namespace XData {

    public struct FullName : IEquatable<FullName> {
        public FullName(string uri, string name) {
            Uri = uri;
            Name = name;
        }
        public readonly string Uri;
        public readonly string Name;
        public bool IsQualified { get { return !IsUnqualified; } }
        public bool IsUnqualified { get { return string.IsNullOrEmpty(Uri); } }
        public bool IsValid { get { return !string.IsNullOrEmpty(Name); } }
        public override string ToString() {
            if (IsValid) {
                if (IsUnqualified) {
                    return Name;
                }
                return "{" + Uri + "}" + Name;
            }
            return null;
        }
        public bool Equals(FullName other) {
            if (IsUnqualified) {
                return other.IsUnqualified && Name == other.Name;
            }
            return Uri == other.Uri && Name == other.Name;
        }
        public override bool Equals(object obj) {
            return obj is FullName && Equals((FullName)obj);
        }
        public override int GetHashCode() {
            var nameHash = Name != null ? Name.GetHashCode() : 0;
            if (IsUnqualified) {
                return nameHash;
            }
            return Extensions.CombineHash(Uri.GetHashCode(), nameHash);
        }
        public static bool operator ==(FullName left, FullName right) {
            return left.Equals(right);
        }
        public static bool operator !=(FullName left, FullName right) {
            return !left.Equals(right);
        }
    }



    public static class Extensions {
        private const int _stringBuilderMaxCount = 8;
        [ThreadStatic]
        private static StringBuilder[] _stringBuilders;
        [ThreadStatic]
        private static int _stringBuilderIndex;
        public static StringBuilder AcquireStringBuilder() {
            if (_stringBuilderIndex < _stringBuilderMaxCount) {
                var sbs = _stringBuilders ?? (_stringBuilders = new StringBuilder[_stringBuilderMaxCount]);
                var sb = sbs[_stringBuilderIndex];
                if (sb == null) {
                    sb = new StringBuilder();
                    sbs[_stringBuilderIndex] = sb;
                }
                ++_stringBuilderIndex;
                sb.Clear();
                return sb;
            }
            return new StringBuilder();
        }
        public static void ReleaseStringBuilder() {
            if (_stringBuilderIndex > 0) {
                --_stringBuilderIndex;
            }
        }
        public static string ToStringAndRelease(this StringBuilder sb) {
            ReleaseStringBuilder();
            return sb.ToString();
        }
        public static string InvFormat(this string format, params string[] args) {
            return AcquireStringBuilder().AppendFormat(CultureInfo.InvariantCulture, format, args).ToStringAndRelease();
        }
        public static string ToInvString(this int i) {
            return i.ToString(CultureInfo.InvariantCulture);
        }
        public static string ToInvString(this ulong i) {
            return i.ToString(CultureInfo.InvariantCulture);
        }


        //public static object CreateInstance(Type type) {
        //    return Activator.CreateInstance(type);//, true);
        //}

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

        public static FullName ToFullName(this TypeKind kind) {
            return new FullName(NamespaceInfo.SystemUri, kind.ToString());
        }

        //
        //
        //public static bool IsAssignableTo(Type to, Type from, bool @try) {
        //    if (to == null) throw new ArgumentNullException("to");
        //    if (to.IsAssignableFrom(from)) return true;
        //    if (@try) return false;
        //    throw new InvalidOperationException("Invalid object clr type '{0}'. '{1}' or its base type expected.".InvariantFormat(to.FullName, from.FullName));
        //}
        //

        //public static bool IsEmpty(this XNamespace xnamespace) {
        //    if (xnamespace == null) throw new ArgumentNullException("xnamespace");
        //    return xnamespace == XNamespace.None;
        //}
        //public static bool IsUnqualified(this XName name) {
        //    if (name == null) throw new ArgumentNullException("name");
        //    return name.Namespace.IsEmpty();
        //}
        //public static bool IsQualified(this XName name) { return !name.IsUnqualified(); }

        //public static TValue TryGetValue<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dict, TKey key) where TValue : class {
        //    if (dict == null) {
        //        throw new ArgumentNullException("dict");
        //    }
        //    TValue value;
        //    if (dict.TryGetValue(key, out value)) {
        //        return value;
        //    }
        //    return null;
        //}
        //public static void CopyTo<T>(IReadOnlyList<T> list, T[] array, int arrayIndex) {
        //    if (list == null) {
        //        throw new ArgumentNullException("list");
        //    }
        //    if (array == null) {
        //        throw new ArgumentNullException("array");
        //    }
        //    if (arrayIndex < 0 || arrayIndex > array.Length) {
        //        throw new ArgumentOutOfRangeException("arrayIndex");
        //    }
        //    var listCount = list.Count;
        //    if (array.Length - arrayIndex < listCount) {
        //        throw new ArgumentException("insufficient array space.");
        //    }
        //    for (var i = 0; i < listCount; i++) {
        //        array[arrayIndex++] = list[i];
        //    }
        //}

        //
        //

        //todo: move
        public static bool IsValid(this DiagnosticSeverity severity) {
            return severity == DiagnosticSeverity.Error || severity == DiagnosticSeverity.Warning || severity == DiagnosticSeverity.Info;
        }
        public static bool IsValid(this DiagnosticCode code) {
            return code >= DiagnosticCode.Parsing && code < DiagnosticCode.Max;
        }
        public static bool IsTrivalToken(this TokenKind kind) {
            return kind == TokenKind.WhitespaceOrNewLine || kind == TokenKind.SingleLineComment || kind == TokenKind.MultiLineComment;
        }

    }
}
