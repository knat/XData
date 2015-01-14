﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using XData.IO.Text;

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
        private const int _stringBuilderCount = 4;
        private const int _stringBuilderCapacity = 64;
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
        public static void ReleaseStringBuilder(StringBuilder sb) {
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
        public static string ToInvString(this int i) {
            return i.ToString(CultureInfo.InvariantCulture);
        }
        public static string ToInvString(this ulong i) {
            return i.ToString(CultureInfo.InvariantCulture);
        }
        public static string ToInvString(this byte i) {
            return i.ToString(CultureInfo.InvariantCulture);
        }
        public static bool TryToInvUInt64(this string s, out ulong result) {
            return ulong.TryParse(s, NumberStyles.AllowLeadingSign, NumberFormatInfo.InvariantInfo, out result);
        }
        public static bool TryToInvByte(this string s, out byte result) {
            return byte.TryParse(s, NumberStyles.AllowLeadingSign, NumberFormatInfo.InvariantInfo, out result);
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

        public static void CreateAndAdd<T>(ref List<T> list, T item) {
            if (list == null) {
                list = new List<T>();
            }
            list.Add(item);
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
