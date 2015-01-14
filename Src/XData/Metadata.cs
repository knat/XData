using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
//using System.Collections.Immutable;
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
        public NamedObjectInfo TryGetGlobalObject(FullName fullName) {
            var ns = TryGetNamespace(fullName.Uri);
            if (ns != null) {
                return ns.TryGetGlobalObject(fullName);
            }
            return null;
        }
    }
    public sealed class NamespaceInfo {
        public NamespaceInfo(string uri, NamedObjectInfo[] globalObjects) {
            if (uri == null) throw new ArgumentNullException("uri");
            if (globalObjects == null) throw new ArgumentNullException("globalObjects");
            Uri = uri;
            GlobalObjects = globalObjects;
        }
        public readonly string Uri;
        public readonly NamedObjectInfo[] GlobalObjects;
        public bool IsSystem {
            get {
                return Uri == SystemUri;
            }
        }
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

    public abstract class ObjectInfo {
        protected ObjectInfo(Type clrType, bool isAbstract) {
            if (clrType == null) throw new ArgumentNullException("clrType");
            ClrType = clrType;
            IsAbstract = isAbstract;
            //IsAbstract = clrType.IsAbstract;
            //var ttt = typeof(object);
            //ttt.FullName
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
            return (XObject)Activator.CreateInstance(ClrType);// Extensions.CreateInstance(ClrType);
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
    public abstract class NamedObjectInfo : ObjectInfo {
        public NamedObjectInfo(Type clrType, bool isAbstract, FullName fullName)
            : base(clrType, isAbstract) {
            FullName = fullName;
        }
        public readonly FullName FullName;
    }
    //DOT NOT EDIT the order
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

    public class TypeInfo : NamedObjectInfo {
        public TypeInfo(Type clrType, bool isAbstract, FullName fullName, TypeKind kind, TypeInfo baseType)
            : base(clrType, isAbstract, fullName) {
            _kind = kind;

            BaseType = baseType;
        }
        private readonly TypeKind _kind;
        public TypeKind Kind {
            get {
                return _kind;
            }
        }

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

    public interface ISimpleTypeInfo {
        TypeKind Kind { get; }
        Type ValueClrType { get; }
        FacetSetInfo? FacetSet { get; }
        ISimpleTypeInfo ItemType { get; }//for listed simple type
    }
    public struct FacetSetInfo {
        public FacetSetInfo(
            ulong? minLength = null,
            ulong? maxLength = null,
            byte? totalDigits = null,
            byte? fractionDigits = null,
            ValueBoundaryInfo? minValue = null,
            ValueBoundaryInfo? maxValue = null,
            EnumerationsInfo? enumerations = null,
            PatternInfo? pattern = null) {
            MinLength = minLength;
            MaxLength = maxLength;
            TotalDigits = totalDigits;
            FractionDigits = fractionDigits;
            MinValue = minValue;
            MaxValue = maxValue;
            Enumerations = enumerations;
            Pattern = pattern;
        }
        public readonly ulong? MinLength;
        public readonly ulong? MaxLength;
        public readonly byte? TotalDigits;
        public readonly byte? FractionDigits;
        public readonly ValueBoundaryInfo? MinValue;
        public readonly ValueBoundaryInfo? MaxValue;
        public readonly EnumerationsInfo? Enumerations;
        public readonly PatternInfo? Pattern;
        //public bool EnumerationsContains(object value) {
        //    if (Enumerations != null)
        //        foreach (var i in Enumerations)
        //            if (SimpleType.ValueEquals(i.Value, value)) return true;
        //    return false;
        //}
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
    public struct EnumerationsInfo {
        public EnumerationsInfo(ValueTextInfo[] items, string totalText) {
            Items = items;
            TotalText = totalText;
        }
        public readonly ValueTextInfo[] Items;
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
    public struct PatternInfo {
        public PatternInfo(string pattern) {
            if (pattern == null) throw new ArgumentNullException("pattern");
            Pattern = pattern;
        }
        public readonly string Pattern;
        //private static readonly ConcurrentDictionary<string, Regex> _regexDict = new ConcurrentDictionary<string, Regex>();
        //public Regex Regex { get { return _regexDict.GetOrAdd(Pattern, p => new Regex(p)); } }
    }
    public class SimpleTypeInfo : TypeInfo, ISimpleTypeInfo {
        public SimpleTypeInfo(Type clrType, bool isAbstract, FullName fullName, TypeKind kind, TypeInfo baseType, Type valueClrType, FacetSetInfo? facetSet = null)
            : base(clrType, isAbstract, fullName, kind, baseType) {
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
    public sealed class AtomicTypeInfo : SimpleTypeInfo {
        public AtomicTypeInfo(Type clrType, bool isAbstract, FullName fullName, TypeKind kind, SimpleTypeInfo baseType, Type valueClrType, FacetSetInfo? facetSet = null)
            : base(clrType, isAbstract, fullName, kind, baseType, valueClrType, facetSet) { }
        new public SimpleTypeInfo BaseType { get { return (SimpleTypeInfo)base.BaseType; } }
    }
    public sealed class ListTypeInfo : SimpleTypeInfo {
        public ListTypeInfo(Type clrType, bool isAbstract, FullName fullName, SimpleTypeInfo itemType)
            : base(clrType, isAbstract, fullName, TypeKind.ListType, XSimpleType.ThisInfo, typeof(XListTypeValue<>)) {
            if (itemType == null) throw new ArgumentNullException("itemType");
            _itemType = itemType;
        }
        public ListTypeInfo(Type clrType, bool isAbstract, FullName fullName, ListTypeInfo baseType, FacetSetInfo facetSet)
            : base(clrType, isAbstract, fullName, TypeKind.ListType, baseType, typeof(XListTypeValue<>), facetSet) { }
        new public SimpleTypeInfo BaseType { get { return (SimpleTypeInfo)base.BaseType; } }
        private readonly SimpleTypeInfo _itemType;//opt
        public override SimpleTypeInfo ItemType { get { return _itemType ?? BaseType.ItemType; } }
    }
    public sealed class ComplexTypeInfo : TypeInfo {
        public ComplexTypeInfo(Type clrType, bool isAbstract, FullName fullName, TypeInfo baseType,
            AttributeSetInfo attributes, ObjectInfo children)
            : base(clrType, isAbstract, fullName, TypeKind.ComplexType, baseType) {
            Attributes = attributes;
            Children = children;
        }
        public readonly AttributeSetInfo Attributes;//opt
        public readonly ObjectInfo Children;//opt, ChildSetInfo or SimpleTypeInfo
        //public ChildSetInfo AsChildSet { get { return Children as ChildSetInfo; } }
        //public SimpleTypeInfo AsSimpleType { get { return Children as SimpleTypeInfo; } }
    }

    public sealed class AttributeSetInfo : ObjectInfo {
        public AttributeSetInfo(Type clrType, AttributeInfo[] attributes)
            : base(clrType, false) {
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
    public abstract class EntityInfo : NamedObjectInfo {
        public EntityInfo(Type clrType, bool isAbstract, FullName fullName, string displayName, TypeInfo type,
            EntityDeclarationKind declarationKind, EntityInfo referentialEntity,
            bool isNullable)
            : base(clrType, isAbstract, fullName) {
            _displayName = displayName;
            Type = type;
            DeclarationKind = declarationKind;
            IsNullable = isNullable;
            ReferentialEntity = referentialEntity;
        }
        private readonly string _displayName;
        public string DisplayName {
            get {
                return _displayName;
            }
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
    public sealed class AttributeInfo : EntityInfo {
        public AttributeInfo(Type clrType, FullName fullName, string displayName, SimpleTypeInfo type,
            EntityDeclarationKind declarationKind, AttributeInfo referentialAttribute,
            bool isNullable)
            : base(clrType, false, fullName, displayName, type, declarationKind, referentialAttribute, isNullable) {
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
        public readonly ValueTextInfo? DefaultValue;

    }
    public interface IChildInfo {
        string DisplayName { get; }
        int Order { get; }
        bool IsEffectiveOptional { get; }
    }

    public sealed class ElementInfo : EntityInfo, IChildInfo {
        public ElementInfo(Type clrType, bool isAbstract, FullName fullName, string displayName, TypeInfo type,
            EntityDeclarationKind declarationKind, ElementInfo referentialElement, bool isNullable,
            int order, bool isEffectiveOptional,
            ElementInfo substitutedElement, FullName[] directSubstitutingElementFullNames,
            ProgramInfo program)
            : base(clrType, isAbstract, fullName, displayName, type, declarationKind, referentialElement, isNullable) {
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
    public abstract class ChildContainerInfo : ObjectInfo, IChildInfo {
        protected ChildContainerInfo(Type clrType, string displayName, int order, bool isEffectiveOptional)
            : base(clrType, false) {
            _displayName = displayName;
            _order = order;
            _isEffectiveOptional = isEffectiveOptional;
        }
        private readonly string _displayName;
        public string DisplayName {
            get {
                return _displayName;
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
    }
    public enum ChildSetKind : byte {
        None = 0,
        Sequence,
        Choice
    }
    public sealed class ChildSetInfo : ChildContainerInfo {
        public ChildSetInfo(Type clrType, string displayName, int order, bool isEffectiveOptional,
            ChildSetKind kind, IChildInfo[] members)
            : base(clrType, displayName, order, isEffectiveOptional) {
            Kind = kind;
            Members = members;
        }
        public readonly ChildSetKind Kind;
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
    public sealed class ChildListInfo : ChildContainerInfo {
        public ChildListInfo(Type clrType, string displayName, int order, bool isEffectiveOptional,
            ulong minOccurs, ulong maxOccurs, IChildInfo item)
            : base(clrType, displayName, order, isEffectiveOptional) {
            MinOccurs = minOccurs;
            MaxOccurs = maxOccurs;
            Item = item;
        }
        public readonly ulong MinOccurs;
        public readonly ulong MaxOccurs;
        public readonly IChildInfo Item;
    }

}
