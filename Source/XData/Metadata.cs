using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
//using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace XData {
    [Serializable]
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
        public NamedObjectInfo TryGetGlobalObject(FullName fullName) {
            var ns = TryGetNamespace(fullName.Uri);
            if (ns != null) {
                return ns.TryGetGlobalObject(fullName);
            }
            return null;
        }
    }
    [Serializable]
    public sealed class NamespaceInfo {
        public NamespaceInfo(string uri, NamedObjectInfo[] globalObjects) {
            if (uri == null) throw new ArgumentNullException("uri");
            if (globalObjects == null) throw new ArgumentNullException("globalObjects");
            Uri = uri;
            GlobalObjects = globalObjects;
        }
        public readonly string Uri;
        public readonly NamedObjectInfo[] GlobalObjects;
        public bool IsSystem { get { return Uri == SystemUri; } }
        public NamedObjectInfo TryGetGlobalObject(FullName fullName) {
            foreach (var obj in GlobalObjects) {
                if (obj.FullName == fullName) {
                    return obj;
                }
            }
            return null;
        }

        //
        public const string SystemUri = "http://xdata-lang.org";
        private static volatile NamespaceInfo _system;
        public static NamespaceInfo System {
            get {
                return _system ?? (_system = new NamespaceInfo(SystemUri, new TypeInfo[] {
                    XType.ThisInfo,
                }));
            }
        }
    }

    [Serializable]
    public abstract class ObjectInfo {
        protected ObjectInfo(Type clrType) {
            if (clrType == null) throw new ArgumentNullException("clrType");
            ClrType = clrType;
            IsAbstract = clrType.IsAbstract;
        }
        public readonly Type ClrType;
        public readonly bool IsAbstract;
        public XObject CreateInstance(bool @try = false) {
            if (IsAbstract) {
                if (@try) {
                    return null;
                }
                throw new InvalidOperationException("Object '{0}' is abstract.".InvFormat(ClrType.FullName));
            }
            return (XObject)Extensions.CreateInstance(ClrType);
        }
        public bool IsAssignableFrom(ObjectInfo other) {
            return ClrType.IsAssignableFrom(other.ClrType);
        }

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
    [Serializable]
    public abstract class NamedObjectInfo : ObjectInfo {
        public NamedObjectInfo(Type clrType, FullName fullName)
            : base(clrType) {
            FullName = fullName;
        }
        public readonly FullName FullName;
    }


    public enum TypeKind : byte {
        None = 0,
        Type,//not allowed for use
        SimpleType,
        AtomicType,
        ListType,
        ComplexType,
        String,
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
        Duration,
        DateTime,
        Date,
        Time,
    }

    [Serializable]
    public class TypeInfo : NamedObjectInfo {
        public TypeInfo(Type clrType, FullName fullName, TypeKind kind, TypeInfo baseType)
            : base(clrType, fullName) {
            _kind = kind;

            BaseType = baseType;
        }
        private readonly TypeKind _kind;
        public TypeKind Kind { get { return _kind; } }

        public readonly TypeInfo BaseType;//opt
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

    [Serializable]
    public struct FacetSetInfo {
        public FacetSetInfo(
            ulong? minLength = null,
            ulong? maxLength = null,
            byte? totalDigits = null,
            byte? fractionDigits = null,
            ValueAndBoundaryInfo? lowerValue = null,
            ValueAndBoundaryInfo? upperValue = null,
            EnumerationsInfo? enumerations = null,
            PatternItemInfo[] patterns = null) {
            MinLength = minLength;
            MaxLength = maxLength;
            TotalDigits = totalDigits;
            FractionDigits = fractionDigits;
            LowerValue = lowerValue;
            UpperValue = upperValue;
            Enumerations = enumerations;
            Patterns = patterns;
        }
        public readonly ulong? MinLength;
        public readonly ulong? MaxLength;
        public readonly byte? TotalDigits;
        public readonly byte? FractionDigits;
        public readonly ValueAndBoundaryInfo? LowerValue;
        public readonly ValueAndBoundaryInfo? UpperValue;
        public readonly EnumerationsInfo? Enumerations;
        public readonly PatternItemInfo[] Patterns;
        //public bool EnumerationsContains(object value) {
        //    if (Enumerations != null)
        //        foreach (var i in Enumerations)
        //            if (SimpleType.ValueEquals(i.Value, value)) return true;
        //    return false;
        //}
    }
    [Serializable]
    public struct ValueAndBoundaryInfo {
        public ValueAndBoundaryInfo(ValueAndTextInfo valueAndText, bool isInclusive) {
            ValueAndText = valueAndText;
            IsInclusive = isInclusive;
        }
        public readonly ValueAndTextInfo ValueAndText;
        public readonly bool IsInclusive;
    }
    [Serializable]
    public struct ValueAndTextInfo {
        public ValueAndTextInfo(object value, string text) {
            Value = value;
            Text = text;
        }
        public readonly object Value;
        public readonly string Text;
    }
    [Serializable]
    public struct EnumerationsInfo {
        public EnumerationsInfo(ValueAndTextInfo[] enumerations, string totalText) {
            Enumerations = enumerations;
            TotalText = totalText;
        }
        public readonly ValueAndTextInfo[] Enumerations;
        public readonly string TotalText;
    }

    //[Serializable]
    //public struct EnumerationItemInfo {
    //    public EnumerationItemInfo(string name, object value) {
    //        if (value == null) throw new ArgumentNullException("value");
    //        Name = name;
    //        Value = value;
    //    }
    //    public readonly string Name;//opt
    //    public readonly object Value;
    //}
    [Serializable]
    public sealed class PatternItemInfo {
        public PatternItemInfo(string pattern) {
            if (pattern == null) throw new ArgumentNullException("pattern");
            Pattern = pattern;
        }
        public readonly string Pattern;
        private static readonly ConcurrentDictionary<string, Regex> _regexDict = new ConcurrentDictionary<string, Regex>();
        public Regex Regex { get { return _regexDict.GetOrAdd(Pattern, p => new Regex(p)); } }
    }
    public interface ISimpleTypeInfo {
        TypeKind Kind { get; }
        Type ValueClrType { get; }
        FacetSetInfo? FacetSet { get; }
        ISimpleTypeInfo ItemType { get; }//for listed simple type
    }
    [Serializable]
    public class SimpleTypeInfo : TypeInfo, ISimpleTypeInfo {
        public SimpleTypeInfo(Type clrType, FullName fullName, TypeKind kind, TypeInfo baseType, Type valueClrType, FacetSetInfo? facetSet = null)
            : base(clrType, fullName, kind, baseType) {
            if (baseType == null) throw new ArgumentNullException("baseType");
            if (valueClrType == null) throw new ArgumentNullException("valueClrType");
            _valueClrType = valueClrType;
            _facetSet = facetSet;
        }
        private readonly Type _valueClrType;
        public Type ValueClrType { get { return _valueClrType; } }
        private readonly FacetSetInfo? _facetSet;
        public FacetSetInfo? FacetSet { get { return _facetSet; } }
        public virtual SimpleTypeInfo ItemType { get { return null; } }
        ISimpleTypeInfo ISimpleTypeInfo.ItemType { get { return ItemType; } }
    }
    [Serializable]
    public sealed class AtomicTypeInfo : SimpleTypeInfo {
        public AtomicTypeInfo(Type clrType, FullName fullName, TypeKind kind, SimpleTypeInfo baseType, Type valueClrType, FacetSetInfo? facetSet = null)
            : base(clrType, fullName, kind, baseType, valueClrType, facetSet) { }
        new public SimpleTypeInfo BaseType { get { return (SimpleTypeInfo)base.BaseType; } }
    }
    [Serializable]
    public sealed class ListTypeInfo : SimpleTypeInfo {
        public ListTypeInfo(Type clrType, FullName fullName, SimpleTypeInfo itemType)
            : base(clrType, fullName, TypeKind.ListType, XSimpleType.ThisInfo, typeof(XListTypeValue<>)) {
            if (itemType == null) throw new ArgumentNullException("itemType");
            _itemType = itemType;
        }
        public ListTypeInfo(Type clrType, FullName fullName, ListTypeInfo baseType, FacetSetInfo facetSet)
            : base(clrType, fullName, TypeKind.ListType, baseType, typeof(XListTypeValue<>), facetSet) { }
        new public SimpleTypeInfo BaseType { get { return (SimpleTypeInfo)base.BaseType; } }
        private readonly SimpleTypeInfo _itemType;//opt
        public override SimpleTypeInfo ItemType { get { return _itemType ?? BaseType.ItemType; } }
    }
    [Serializable]
    public sealed class ComplexTypeInfo : TypeInfo {
        public ComplexTypeInfo(Type clrType, FullName fullName, TypeInfo baseType,
            AttributeSetInfo attributes, ObjectInfo children)
            : base(clrType, fullName, TypeKind.ComplexType, baseType) {
            Attributes = attributes;
            Children = children;
        }
        public readonly AttributeSetInfo Attributes;//opt
        public readonly ObjectInfo Children;//opt, ChildSetInfo or SimpleTypeInfo
        //public ChildSetInfo AsChildSet { get { return Children as ChildSetInfo; } }
        //public SimpleTypeInfo AsSimpleType { get { return Children as SimpleTypeInfo; } }
    }

    [Serializable]
    public sealed class AttributeSetInfo : ObjectInfo {
        public AttributeSetInfo(Type clrType, AttributeInfo[] attributes)
            : base(clrType) {
            Attributes = attributes;
        }
        public readonly AttributeInfo[] Attributes;
        public AttributeInfo TryGetAttribute(FullName fullName) {
            foreach (var attribute in Attributes) {
                if (attribute.FullName == fullName) {
                    return attribute;
                }
            }
            return null;
        }
    }
    public enum EntityDeclarationKind : byte {
        None = 0,
        Local,
        Global,
        Reference
    }
    [Serializable]
    public abstract class EntityInfo : NamedObjectInfo {
        public EntityInfo(Type clrType, FullName fullName, TypeInfo type,
            EntityDeclarationKind declarationKind, EntityInfo referentialEntity,
            bool isNullable)
            : base(clrType, fullName) {
            Type = type;
            DeclarationKind = declarationKind;
            IsNullable = isNullable;
            ReferentialEntity = referentialEntity;
        }
        public readonly TypeInfo Type;
        public readonly EntityDeclarationKind DeclarationKind;
        public readonly EntityInfo ReferentialEntity;
        public readonly bool IsNullable;
        public bool IsLocal {
            get {
                return DeclarationKind == EntityDeclarationKind.Local;
            }
        }
        public bool IsGlobal {
            get {
                return DeclarationKind == EntityDeclarationKind.Global;
            }
        }
        public bool IsReference {
            get {
                return DeclarationKind == EntityDeclarationKind.Reference;
            }
        }
    }
    [Serializable]
    public sealed class AttributeInfo : EntityInfo {
        public AttributeInfo(Type clrType, FullName fullName, SimpleTypeInfo type,
            EntityDeclarationKind declarationKind, AttributeInfo referentialAttribute,
            bool isNullable)
            : base(clrType, fullName, type, declarationKind, referentialAttribute, isNullable) {

        }
        new public SimpleTypeInfo Type {
            get {
                return (SimpleTypeInfo)base.Type;
            }
        }
        public AttributeInfo ReferentialAttribute {
            get {
                return (AttributeInfo)ReferentialEntity;
            }
        }
        public readonly bool IsOptional;
        public readonly ValueAndTextInfo? DefaultValue;

    }
    public interface IChildInfo {
        int Order { get; }
        bool IsEffectiveOptional { get; }
    }

    [Serializable]
    public sealed class ElementInfo : EntityInfo, IChildInfo {
        public ElementInfo(Type clrType, FullName fullName, TypeInfo type,
            EntityDeclarationKind declarationKind, ElementInfo referentialElement, bool isNullable,
            int order, bool isEffectiveOptional,
            ElementInfo substitutedElement, FullName[] directSubstitutingElementFullNames,
            ProgramInfo program)
            : base(clrType, fullName, type, declarationKind, referentialElement, isNullable) {
            _order = order;
            _isEffectiveOptional = isEffectiveOptional;
            SubstitutedElement = substitutedElement;
            _directSubstitutingElementFullNames = directSubstitutingElementFullNames;
            Program = program;
        }
        public ElementInfo ReferentialElement {
            get {
                return (ElementInfo)ReferentialEntity;
            }
        }
        private readonly int _order;
        public int Order {
            get {
                return _order;
            }
        }
        private readonly bool _isEffectiveOptional;
        public bool IsEffectiveOptional {
            get {
                return _isEffectiveOptional;
            }
        }
        public readonly ElementInfo SubstitutedElement;//opt, for global element
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
        public bool IsMatch(FullName fullName, out ElementInfo globalElement) {
            if (FullName == fullName) {
                globalElement = ReferentialElement;
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
                return ReferentialElement.TryGet(fullName);
            }
            if (FullName == FullName) {
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
    public enum ChildSetKind : byte {
        None = 0,
        Sequence,
        Choice
    }
    [Serializable]
    public sealed class ChildSetInfo : ObjectInfo, IChildInfo {
        public ChildSetInfo(Type clrType, ChildSetKind kind, int order, bool isEffectiveOptional,
            IChildInfo[] members)
            : base(clrType) {
            Kind = kind;
            _order = order;
            _isEffectiveOptional = isEffectiveOptional;
            Members = members;
        }
        public readonly ChildSetKind Kind;
        private readonly int _order;
        public int Order {
            get {
                return _order;
            }
        }
        private readonly bool _isEffectiveOptional;
        public bool IsEffectiveOptional {
            get {
                return _isEffectiveOptional;
            }
        }
        public readonly IChildInfo[] Members;
        public IChildInfo TryGetMember(int order) {
            foreach (var member in Members) {
                if (member.Order == order) {
                    return member;
                }
            }
            return null;
        }
    }
    [Serializable]
    public sealed class ChildListInfo : ObjectInfo, IChildInfo {
        public ChildListInfo(Type clrType, int order, bool isEffectiveOptional,
            ulong minOccurs, ulong maxOccurs, IChildInfo item)
            : base(clrType) {
            _order = order;
            _isEffectiveOptional = isEffectiveOptional;
            MinOccurs = minOccurs;
            MaxOccurs = maxOccurs;
            Item = item;
        }
        private readonly int _order;
        public int Order {
            get {
                return _order;
            }
        }
        private readonly bool _isEffectiveOptional;
        public bool IsEffectiveOptional {
            get {
                return _isEffectiveOptional;
            }
        }
        public readonly ulong MinOccurs;
        public readonly ulong MaxOccurs;
        public readonly IChildInfo Item;
    }

}
