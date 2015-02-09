using System;
using System.Collections.Generic;

namespace XData {
    public abstract class ProgramInfo {
        private volatile List<NamespaceInfo> _namespaces;
        public List<NamespaceInfo> Namespaces {
            get {
                if (_namespaces == null) {
                    var list = GetNamespaces();
                    list.Add(NamespaceInfo.System);
                    _namespaces = list;
                }
                return _namespaces;
            }
        }
        protected abstract List<NamespaceInfo> GetNamespaces();
        public NamespaceInfo TryGetNamespace(string uri) {
            foreach (var ns in Namespaces) {
                if (EX.UriEquals(ns.Uri, uri)) {
                    return ns;
                }
            }
            return null;
        }
        public IGlobalObjectInfo TryGetGlobalObject(FullName fullName) {
            var ns = TryGetNamespace(fullName.Uri);
            if (ns != null) {
                return ns.TryGetGlobalObject(fullName);
            }
            return null;
        }
    }
    public sealed class NamespaceInfo {
        public NamespaceInfo(string uri, IGlobalObjectInfo[] globalObjects) {
            if (uri == null) throw new ArgumentNullException("uri");
            if (globalObjects == null) throw new ArgumentNullException("globalObjects");
            Uri = uri;
            GlobalObjects = globalObjects;
        }
        public readonly string Uri;
        public readonly IGlobalObjectInfo[] GlobalObjects;
        public bool IsSystem {
            get {
                return Uri == Extensions.SystemUri;
            }
        }
        public IGlobalObjectInfo TryGetGlobalObject(FullName fullName) {
            if (GlobalObjects != null) {
                foreach (var obj in GlobalObjects) {
                    if (obj.FullName == fullName) {
                        return obj;
                    }
                }
            }
            return null;
        }
        //
        public static readonly NamespaceInfo System = new NamespaceInfo(Extensions.SystemUri, new IGlobalObjectInfo[] {


        });
    }

    public abstract class ObjectInfo {
        protected ObjectInfo(Type clrType, bool isAbstract, string displayName) {
            if (clrType == null) throw new ArgumentNullException("clrType");
            ClrType = clrType;
            IsAbstract = isAbstract;
            DisplayName = displayName;
        }
        public readonly Type ClrType;
        public readonly bool IsAbstract;
        public readonly string DisplayName;
        public T CreateInstance<T>(bool @try = false) where T : XObject {
            if (IsAbstract) {
                if (@try) {
                    return null;
                }
                throw new InvalidOperationException("Object '{0}' is abstract.".InvFormat(DisplayName));
            }
            return (T)Activator.CreateInstance(ClrType);
        }

    }
    public interface IGlobalObjectInfo {
        FullName FullName { get; }
    }

    public abstract class TypeInfo : ObjectInfo, IGlobalObjectInfo {
        protected TypeInfo(Type clrType, bool isAbstract, FullName fullName, TypeInfo baseType)
            : base(clrType, isAbstract, fullName.ToString()) {
            FullName = fullName;
            BaseType = baseType;
        }
        public FullName FullName { get; private set; }
        public readonly TypeInfo BaseType;
        public bool EqualToOrDeriveFrom(TypeInfo other) {
            if (other == null) throw new ArgumentNullException("other");
            for (var info = this; info != null; info = info.BaseType) {
                if (info == other) {
                    return true;
                }
            }
            return false;
        }
    }

    public sealed class FacetSetInfo {
        public FacetSetInfo(
            ulong? minLength, ulong? maxLength,
            byte? precision, byte? scale,
            ValueBoundaryInfo? minValue, ValueBoundaryInfo? maxValue,
            EnumInfo? @enum, PatternInfo[] patterns) {
            MinLength = minLength;
            MaxLength = maxLength;
            Precision = precision;
            Scale = scale;
            MinValue = minValue;
            MaxValue = maxValue;
            Enum = @enum;
            Patterns = patterns;
        }
        public readonly ulong? MinLength;
        public readonly ulong? MaxLength;
        public readonly byte? Precision;
        public readonly byte? Scale;
        public readonly ValueBoundaryInfo? MinValue;
        public readonly ValueBoundaryInfo? MaxValue;
        public readonly EnumInfo? Enum;
        public readonly PatternInfo[] Patterns;
    }
    public struct ValueBoundaryInfo {
        public ValueBoundaryInfo(object value, string text, bool isInclusive) {
            Value = value;
            Text = text;
            IsInclusive = isInclusive;
        }
        public readonly object Value;
        public readonly string Text;
        public readonly bool IsInclusive;
    }
    public struct EnumInfo {
        public EnumInfo(object[] items, string text) {
            Items = items;
            Text = text;
        }
        public readonly object[] Items;
        public readonly string Text;
    }
    public struct PatternInfo {
        public PatternInfo(string pattern) {
            if (pattern == null) throw new ArgumentNullException("pattern");
            Pattern = pattern;
        }
        public readonly string Pattern;
        private static readonly Dictionary<string, System.Text.RegularExpressions.Regex> _regexDict = new Dictionary<string, System.Text.RegularExpressions.Regex>();
        public System.Text.RegularExpressions.Regex Regex {
            get {
                lock (_regexDict) {
                    System.Text.RegularExpressions.Regex regex;
                    if (_regexDict.TryGetValue(Pattern, out regex)) {
                        return regex;
                    }
                    regex = new System.Text.RegularExpressions.Regex(Pattern, System.Text.RegularExpressions.RegexOptions.CultureInvariant);
                    _regexDict.Add(Pattern, regex);
                    return regex;
                }
            }
        }
    }
    public class SimpleTypeInfo : TypeInfo {
        public SimpleTypeInfo(Type clrType, bool isAbstract, FullName fullName, SimpleTypeInfo baseType,
            FacetSetInfo facets)
            : base(clrType, isAbstract, fullName, baseType) {
            //if (baseType == null) throw new ArgumentNullException("baseType");
            Facets = facets;
        }
        public readonly FacetSetInfo Facets;
    }
    public enum TypeKind : byte {
        None = 0,
        //DOT NOT CHANGE THE ORDER
        ComplexType,
        SimpleType,
        ListType,
        AtomType,
        String,
        IgnoreCaseString,
        Decimal,
        Int64,
        Int32,
        Int16,
        SByte,
        UInt64,
        UInt32,
        UInt16,
        Byte,
        Double,
        Single,
        Boolean,
        Binary,
        Guid,
        TimeSpan,
        DateTimeOffset,
        //Date,
        //Time,
    }

    public sealed class AtomTypeInfo : SimpleTypeInfo {
        public AtomTypeInfo(Type clrType, bool isAbstract, FullName fullName, SimpleTypeInfo baseType,
            FacetSetInfo facets, TypeKind kind)
            : base(clrType, isAbstract, fullName, baseType, facets) {
            Kind = kind;
        }
        public readonly TypeKind Kind;
    }
    public sealed class ListTypeInfo : SimpleTypeInfo {
        public ListTypeInfo(Type clrType, bool isAbstract, FullName fullName, SimpleTypeInfo baseType,
            FacetSetInfo facets, SimpleTypeInfo itemType)
            : base(clrType, isAbstract, fullName, baseType, facets) {
            ItemType = itemType;
        }
        public readonly SimpleTypeInfo ItemType;
    }
    public sealed class ComplexTypeInfo : TypeInfo {
        public ComplexTypeInfo(Type clrType, bool isAbstract, FullName fullName, TypeInfo baseType,
            AttributeSetInfo attributes, ObjectInfo children)
            : base(clrType, isAbstract, fullName, baseType) {
            Attributes = attributes;
            Children = children;
        }
        public readonly AttributeSetInfo Attributes;//opt
        public readonly ObjectInfo Children;//opt
        public SimpleTypeInfo SimpleChild {
            get {
                return Children as SimpleTypeInfo;
            }
        }
        public ChildSetInfo ComplexChildren {
            get {
                return Children as ChildSetInfo;
            }
        }
    }

    public sealed class AttributeSetInfo : ObjectInfo {
        public AttributeSetInfo(Type clrType, string displayName, AttributeInfo[] attributes)
            : base(clrType, false, displayName) {
            Attributes = attributes;
        }
        public readonly AttributeInfo[] Attributes;
        public AttributeInfo TryGetAttribute(string name) {
            if (Attributes != null) {
                foreach (var attribute in Attributes) {
                    if (attribute.Name == name) {
                        return attribute;
                    }
                }
            }
            return null;
        }
    }
    public sealed class AttributeInfo : ObjectInfo {
        public AttributeInfo(Type clrType, string displayName, string name, bool isOptional, bool isNullable,
            SimpleTypeInfo type)
            : base(clrType, false, displayName) {
            Name = name;
            IsOptional = isOptional;
            IsNullable = isNullable;
            Type = type;
        }
        public readonly string Name;
        public readonly bool IsOptional;
        public readonly bool IsNullable;
        public readonly SimpleTypeInfo Type;
    }

    public enum ChildKind : byte {
        None = 0,
        GlobalElement,
        LocalElement,
        GlobalElementRef,
        Sequence,
        Choice,
        List
    }
    public abstract class ChildInfo : ObjectInfo {
        protected ChildInfo(Type clrType, bool isAbstract, string displayName, ChildKind kind, bool isOptional, int order)
            : base(clrType, isAbstract, displayName) {
            Kind = kind;
            IsOptional = isOptional;
            Order = order;
        }
        public readonly ChildKind Kind;
        public readonly bool IsOptional;
        public readonly int Order;
    }

    public sealed class ElementInfo : ChildInfo, IGlobalObjectInfo {
        public ElementInfo(Type clrType, bool isAbstract, string displayName, ChildKind kind, bool isOptional, int order,
            FullName fullName, bool isNullable, TypeInfo type,
            ElementInfo referencedElement, ElementInfo substitutedElement, FullName[] directSubstitutingElementFullNames, ProgramInfo program)
            : base(clrType, isAbstract, displayName, kind, isOptional, order) {
            FullName = fullName;
            IsNullable = isNullable;
            Type = type;
            ReferencedElement = referencedElement;
            SubstitutedElement = substitutedElement;
            _directSubstitutingElementFullNames = directSubstitutingElementFullNames;
            Program = program;
        }
        public FullName FullName { get; private set; }
        public readonly bool IsNullable;
        public readonly TypeInfo Type;
        public readonly ElementInfo ReferencedElement;
        public readonly ElementInfo SubstitutedElement;
        private readonly FullName[] _directSubstitutingElementFullNames;//opt, for global element
        public readonly ProgramInfo Program;
        private volatile ElementInfo[] _directSubstitutingElements;
        public ElementInfo[] DirectSubstitutingElements {
            get {
                if (_directSubstitutingElements != null) {
                    return _directSubstitutingElements;
                }
                if (_directSubstitutingElementFullNames == null) {
                    return null;
                }
                var array = new ElementInfo[_directSubstitutingElementFullNames.Length];
                for (var i = 0; i < array.Length; ++i) {
                    array[i] = (ElementInfo)Program.TryGetGlobalObject(_directSubstitutingElementFullNames[i]);
                }
                return _directSubstitutingElements = array;
            }
        }
        public bool IsLocal {
            get {
                return Kind == ChildKind.LocalElement;
            }
        }
        public bool IsGlobal {
            get {
                return Kind == ChildKind.GlobalElement;
            }
        }
        public bool IsGlobalRef {
            get {
                return Kind == ChildKind.GlobalElementRef;
            }
        }
        public bool EqualToOrSubstituteFor(ElementInfo other) {
            if (other == null) throw new ArgumentNullException("other");
            for (var info = this; info != null; info = info.SubstitutedElement) {
                if (info == other) {
                    return true;
                }
            }
            return false;
        }
        public ElementInfo TryGetEffectiveElement(FullName fullName) {
            if (IsLocal) {
                return FullName == fullName ? this : null;
            }
            if (IsGlobalRef) {
                return ReferencedElement.TryGetSubstitutor(fullName);
            }
            return TryGetSubstitutor(fullName);
        }
        private ElementInfo TryGetSubstitutor(FullName fullName) {
            if (FullName == fullName) {
                return this;
            }
            var directSubstitutingElements = DirectSubstitutingElements;
            if (directSubstitutingElements != null) {
                foreach (var i in directSubstitutingElements) {
                    var info = i.TryGetSubstitutor(fullName);
                    if (info != null) {
                        return info;
                    }
                }
            }
            return null;
        }

    }
    public abstract class ChildContainerInfo : ChildInfo {
        protected ChildContainerInfo(Type clrType, string displayName, ChildKind kind, bool isOptional, int order)
            : base(clrType, false, displayName, kind, isOptional, order) {
        }
    }
    public sealed class ChildSetInfo : ChildContainerInfo {
        public ChildSetInfo(Type clrType, string displayName, ChildKind kind, bool isOptional, int order, ChildInfo[] children)
            : base(clrType, displayName, kind, isOptional, order) {
            Children = children;
        }
        public readonly ChildInfo[] Children;
        public bool IsSequence {
            get {
                return Kind == ChildKind.Sequence;
            }
        }
        public bool IsChoice {
            get {
                return Kind == ChildKind.Choice;
            }
        }
        public ChildInfo TryGetChild(int order) {
            if (Children != null) {
                foreach (var child in Children) {
                    if (child.Order == order) {
                        return child;
                    }
                }
            }
            return null;
        }
    }
    public sealed class ChildListInfo : ChildContainerInfo {
        public ChildListInfo(Type clrType, string displayName, bool isOptional, int order,
            ulong minOccurrence, ulong maxOccurrence, ChildInfo item)
            : base(clrType, displayName, ChildKind.List, isOptional, order) {
            MinOccurrence = minOccurrence;
            MaxOccurrence = maxOccurrence;
            Item = item;
        }
        public readonly ulong MinOccurrence;
        public readonly ulong MaxOccurrence;
        public readonly ChildInfo Item;
    }

}
