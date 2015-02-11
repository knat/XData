using System;

namespace XData {
    public struct FullName : IEquatable<FullName> {
        public FullName(string uri, string name) {
            Uri = uri;
            Name = name;
        }
        public readonly string Uri;
        public readonly string Name;
        public bool IsQualified {
            get {
                return !IsUnqualified;
            }
        }
        public bool IsUnqualified {
            get {
                return string.IsNullOrEmpty(Uri);
            }
        }
        public bool IsValid {
            get {
                return !string.IsNullOrEmpty(Name);
            }
        }
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
            return Extensions.UriEquals(Uri, other.Uri) && Name == other.Name;
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
}
