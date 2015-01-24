using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

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
                if (ns.Uri == uri) {
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
                return Uri == InfoExtensions.SystemUri;
            }
        }
        public IGlobalObjectInfo TryGetGlobalObject(FullName fullName) {
            foreach (var obj in GlobalObjects) {
                if (obj.FullName == fullName) {
                    return obj;
                }
            }
            return null;
        }
        //
        public static readonly NamespaceInfo System = new NamespaceInfo(InfoExtensions.SystemUri, new IGlobalObjectInfo[] {


        });
    }

    public abstract class ObjectInfo {
        protected ObjectInfo(Type clrType, bool isAbstract) {
            if (clrType == null) throw new ArgumentNullException("clrType");
            ClrType = clrType;
            IsAbstract = isAbstract;
        }
        public readonly Type ClrType;
        public readonly bool IsAbstract;
        public T CreateInstance<T>(bool @try = false) where T : XObject {
            if (IsAbstract) {
                if (@try) {
                    return null;
                }
                throw new InvalidOperationException("Object '{0}' is abstract.".InvFormat(ClrType.FullName));
            }
            return (T)Activator.CreateInstance(ClrType);
        }


        //public bool IsAssignableFrom(ObjectInfo other) {
        //    return ClrType.IsAssignableFrom(other.ClrType);
        //}

        //public bool IsAssignableFrom(XObject obj) {
        //    if (obj == null) throw new ArgumentNullException("obj");
        //    return ClrType.IsAssignableFrom(obj.GetType());
        //}

        //public bool IsAssignableFrom(Object obj, Context context) {
        //    if (IsAssignableFrom(obj)) return true;
        //    new Diagnostic(context, obj, DiagnosticCode.InvalidObjectClrType, obj.GetType().FullName, ClrType.FullName);
        //    return false;
        //}


        //public T CreateInstance<T>(bool @try = false) where T : XObject {
        //    if (IsAbstract) {
        //        if (@try) {
        //            return null;
        //        }
        //        throw new InvalidOperationException("Clr type '{0}' is abstract.".InvariantFormat(ClrType.FullName));
        //    }
        //    if (Extensions.IsAssignableTo(typeof(T), ClrType, @try)) {
        //        return (T)Extensions.CreateInstance(ClrType);
        //    }
        //    return null;
        //}
        //public T CreateInstance<T>(Location? location, bool @try = false) where T : Object {
        //    var obj = CreateInstance<T>(@try);
        //    if (obj != null && location != null) obj.Location = location;
        //    return obj;
        //}
    }
    public interface IGlobalObjectInfo {
        FullName FullName { get; }
    }
    //public abstract class NamedObjectInfo : ObjectInfo {
    //    public NamedObjectInfo(Type clrType, bool isAbstract, FullName fullName)
    //        : base(clrType, isAbstract) {
    //        FullName = fullName;
    //    }
    //    public readonly FullName FullName;
    //}

    public abstract class TypeInfo : ObjectInfo, IGlobalObjectInfo {
        protected TypeInfo(Type clrType, bool isAbstract, FullName fullName, TypeInfo baseType)
            : base(clrType, isAbstract) {
            FullName = fullName;
            BaseType = baseType;
        }
        public FullName FullName { get; private set; }
        public readonly TypeInfo BaseType;
        public bool IsEqualToOrDeriveFrom(TypeInfo other) {
            if (other == null) throw new ArgumentNullException("other");
            for (var info = this; info != null; info = info.BaseType) {
                if (info == other) {
                    return true;
                }
            }
            return false;
        }

    }

    public sealed class ValueFacetSetInfo {
        public ValueFacetSetInfo(
            ulong? minLength = null,
            ulong? maxLength = null,
            byte? totalDigits = null,
            byte? fractionDigits = null,
            ValueBoundaryInfo? minValue = null,
            ValueBoundaryInfo? maxValue = null,
            EnumsInfo? enums = null,
            PatternInfo? pattern = null) {
            MinLength = minLength;
            MaxLength = maxLength;
            TotalDigits = totalDigits;
            FractionDigits = fractionDigits;
            MinValue = minValue;
            MaxValue = maxValue;
            Enums = enums;
            Pattern = pattern;
        }
        public readonly ulong? MinLength;
        public readonly ulong? MaxLength;
        public readonly byte? TotalDigits;
        public readonly byte? FractionDigits;
        public readonly ValueBoundaryInfo? MinValue;
        public readonly ValueBoundaryInfo? MaxValue;
        public readonly EnumsInfo? Enums;
        public readonly PatternInfo? Pattern;
    }
    public struct ValueBoundaryInfo {
        public ValueBoundaryInfo(ValueTextInfo valueText, bool isInclusive) {
            ValueText = valueText;
            IsInclusive = isInclusive;
        }
        public readonly ValueTextInfo ValueText;
        public readonly bool IsInclusive;
    }
    public struct ValueTextInfo {
        public ValueTextInfo(object value, string text) {
            Value = value;
            Text = text;
        }
        public readonly object Value;
        public readonly string Text;
    }
    public struct EnumsInfo {
        public EnumsInfo(ValueTextInfo[] items, string totalText) {
            Items = items;
            TotalText = totalText;
        }
        public readonly ValueTextInfo[] Items;
        public readonly string TotalText;
    }
    public struct PatternInfo {
        public PatternInfo(string pattern) {
            if (pattern == null) throw new ArgumentNullException("pattern");
            Pattern = pattern;
        }
        public readonly string Pattern;
        //private static readonly ConcurrentDictionary<string, Regex> _regexDict = new ConcurrentDictionary<string, Regex>();
        //public Regex Regex { get { return _regexDict.GetOrAdd(Pattern, p => new Regex(p)); } }
    }
    public class SimpleTypeInfo : TypeInfo {
        public SimpleTypeInfo(Type clrType, bool isAbstract, FullName fullName, SimpleTypeInfo baseType,
            ValueFacetSetInfo valueRestrictions)
            : base(clrType, isAbstract, fullName, baseType) {
            if (baseType == null) throw new ArgumentNullException("baseType");
            //if (valueClrType == null) throw new ArgumentNullException("valueClrType");
            //ValueClrType = valueClrType;
            ValueRestrictions = valueRestrictions;
        }
        //public Type ValueClrType { get; private set; }
        public readonly ValueFacetSetInfo ValueRestrictions;// { get; private set; }
    }
    public enum TypeKind : byte {
        None = 0,
        //DOT NOT CHANGE THE ORDER
        ComplexType,
        SimpleType,
        AtomType,
        ListType,
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
    public static class InfoExtensions {
        public const string SystemUri = "http://xdata-lang.org";
        public const TypeKind TypeStart = TypeKind.ComplexType;
        public const TypeKind TypeEnd = TypeKind.DateTimeOffset;
        public const TypeKind ConcreteAtomTypeStart = TypeKind.String;
        public const TypeKind ConcreteAtomTypeEnd = TypeKind.DateTimeOffset;
        public static bool IsConcreteAtomType(this TypeKind kind) {
            return kind >= ConcreteAtomTypeStart && kind <= ConcreteAtomTypeEnd;
        }
        public static FullName ToFullName(this TypeKind kind) {
            return new FullName(SystemUri, kind.ToString());
        }
        public static AtomTypeInfo ToAtomTypeInfo(this TypeKind kind, Type clrType, AtomTypeInfo baseType) {
            return new AtomTypeInfo(clrType, false, ToFullName(kind), baseType, null, kind);
        }


    }

    public sealed class AtomTypeInfo : SimpleTypeInfo {
        public AtomTypeInfo(Type clrType, bool isAbstract, FullName fullName, SimpleTypeInfo baseType,
            ValueFacetSetInfo valueRestrictions, TypeKind kind)
            : base(clrType, isAbstract, fullName, baseType, valueRestrictions) {
            Kind = kind;
        }
        public readonly TypeKind Kind;

        //new public SimpleTypeInfo BaseType { get { return (SimpleTypeInfo)base.BaseType; } }
    }
    public sealed class ListTypeInfo : SimpleTypeInfo {
        public ListTypeInfo(Type clrType, bool isAbstract, FullName fullName, SimpleTypeInfo baseType, SimpleTypeInfo itemType,
            ValueFacetSetInfo valueRestrictions)
            : base(clrType, isAbstract, fullName, baseType, valueRestrictions) {
            ItemType = itemType;
        }
        //new public SimpleTypeInfo BaseType {
        //    get {
        //        return (SimpleTypeInfo)base.BaseType;
        //    }
        //}
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
        public AttributeSetInfo(Type clrType, AttributeInfo[] attributes)
            : base(clrType, false) {
            Attributes = attributes;
        }
        //public readonly AttributeSetInfo BaseAttributeSet;
        //public readonly bool IsExtension;
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
        public AttributeInfo(Type clrType, string name, string displayName, bool isOptional, bool isNullable,
            SimpleTypeInfo type/*, AttributeInfo restrictedAttribute*/)
            : base(clrType, false) {
            Name = name;
            DisplayName = displayName;
            IsOptional = isOptional;
            IsNullable = isNullable;
            Type = type;
            //RestrictedAttribute = restrictedAttribute;
        }
        public readonly string Name;
        public readonly string DisplayName;
        public readonly bool IsOptional;
        public readonly bool IsNullable;
        public readonly SimpleTypeInfo Type;
        //public readonly AttributeInfo RestrictedAttribute;
    }

    public abstract class ChildInfo : ObjectInfo {
        protected ChildInfo(Type clrType, bool isAbstract, string displayName, bool isOptional, int order)
            : base(clrType, isAbstract) {
            DisplayName = displayName;
            IsOptional = isOptional;
            Order = order;
        }
        public readonly string DisplayName;
        public readonly bool IsOptional;
        public readonly int Order;
    }

    //public abstract class EntityInfo : NamedObjectInfo {
    //    public EntityInfo(Type clrType, bool isAbstract, FullName fullName, string displayName, TypeInfo type,
    //        ElementKind declKind, EntityInfo referencedEntity, EntityInfo substitutedEntity, bool isNullable, bool isOptional)
    //        : base(clrType, isAbstract, fullName) {
    //        DisplayName = displayName;
    //        Type = type;
    //        DeclKind = declKind;
    //        ReferencedEntity = referencedEntity;
    //        SubstitutedEntity = substitutedEntity;
    //        IsNullable = isNullable;
    //        IsOptional = isOptional;
    //    }
    //    public string DisplayName { get; private set; }
    //    public readonly TypeInfo Type;
    //    public readonly ElementKind DeclKind;
    //    public readonly EntityInfo ReferencedEntity;
    //    public readonly EntityInfo SubstitutedEntity;
    //    public readonly bool IsNullable;
    //    public bool IsOptional { get; private set; }
    //    public bool IsLocal {
    //        get {
    //            return DeclKind == ElementKind.Local;
    //        }
    //    }
    //    public bool IsGlobal {
    //        get {
    //            return DeclKind == ElementKind.Global;
    //        }
    //    }
    //    public bool IsReference {
    //        get {
    //            return DeclKind == ElementKind.Reference;
    //        }
    //    }
    //}

    //public interface IChildInfo {
    //    string DisplayName { get; }
    //    int Order { get; }
    //    bool IsOptional { get; }
    //}
    public enum ElementKind : byte {
        None = 0,
        Local,
        Global,
        Reference
    }
    public sealed class ElementInfo : ChildInfo, IGlobalObjectInfo {
        public ElementInfo(Type clrType, bool isAbstract, string displayName, bool isOptional, int order,
            FullName fullName, ElementKind kind, bool isNullable, TypeInfo type,
             ElementInfo referencedElement, ElementInfo substitutedElement, FullName[] directSubstitutingElementFullNames, ProgramInfo program)
            : base(clrType, isAbstract, displayName, isOptional, order) {
            FullName = fullName;
            Kind = kind;
            IsNullable = isNullable;
            Type = type;
            ReferencedElement = referencedElement;
            SubstitutedElement = substitutedElement;
            _directSubstitutingElementFullNames = directSubstitutingElementFullNames;
            Program = program;
        }
        public FullName FullName { get; private set; }
        public readonly ElementKind Kind;
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
                return Kind == ElementKind.Local;
            }
        }
        public bool IsGlobal {
            get {
                return Kind == ElementKind.Global;
            }
        }
        public bool IsReference {
            get {
                return Kind == ElementKind.Reference;
            }
        }
        public bool IsMatch(FullName fullName, out ElementInfo globalElement) {
            if (FullName == fullName) {
                globalElement = ReferencedElement;
                return true;
            }
            if (!IsLocal) {
                globalElement = TryGet(fullName);
                return globalElement != null;
            }
            globalElement = null;
            return false;
        }
        private ElementInfo TryGet(FullName fullName) {
            if (IsReference) {
                return ReferencedElement.TryGet(fullName);
            }
            if (FullName == fullName) {
                return this;
            }
            var directSubstitutingElements = DirectSubstitutingElements;
            if (directSubstitutingElements != null) {
                foreach (var i in directSubstitutingElements) {
                    var info = i.TryGet(fullName);
                    if (info != null) {
                        return info;
                    }
                }
            }
            return null;
        }

    }
    public abstract class ChildContainerInfo : ChildInfo {
        protected ChildContainerInfo(Type clrType, string displayName, bool isOptional, int order)
            : base(clrType, false, displayName, isOptional, order) {
        }
    }
    public enum ChildSetKind : byte {
        None = 0,
        Sequence,
        Choice
    }
    public sealed class ChildSetInfo : ChildContainerInfo {
        public ChildSetInfo(Type clrType, string displayName, bool isOptional, int order,
            ChildSetKind kind, ChildInfo[] members)
            : base(clrType, displayName, isOptional, order) {
            Kind = kind;
            Members = members;
        }
        public readonly ChildSetKind Kind;
        public readonly ChildInfo[] Members;
        public ChildInfo TryGetMember(int order) {
            if (Members != null) {
                foreach (var member in Members) {
                    if (member.Order == order) {
                        return member;
                    }
                }
            }
            return null;
        }
    }
    public sealed class ChildListInfo : ChildContainerInfo {
        public ChildListInfo(Type clrType, string displayName, bool isOptional, int order,
            ulong minOccurs, ulong maxOccurs, ChildInfo item)
            : base(clrType, displayName, isOptional, order) {
            MinOccurs = minOccurs;
            MaxOccurs = maxOccurs;
            Item = item;
        }
        public readonly ulong MinOccurs;
        public readonly ulong MaxOccurs;
        public readonly ChildInfo Item;
    }

}
