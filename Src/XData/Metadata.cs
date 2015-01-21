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
                return Uri == InfoExtensions.SystemUri;
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
        public static readonly NamespaceInfo System = new NamespaceInfo(InfoExtensions.SystemUri, new NamedObjectInfo[] {


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
    public abstract class NamedObjectInfo : ObjectInfo {
        public NamedObjectInfo(Type clrType, bool isAbstract, FullName fullName)
            : base(clrType, isAbstract) {
            FullName = fullName;
        }
        public readonly FullName FullName;
    }

    public abstract class TypeInfo : NamedObjectInfo {
        protected TypeInfo(Type clrType, bool isAbstract, FullName fullName, TypeInfo baseType)
            : base(clrType, isAbstract, fullName) {
            BaseType = baseType;
        }
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

    public sealed class ValueRestrictionSetInfo {
        public ValueRestrictionSetInfo(
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
            ValueRestrictionSetInfo valueRestrictions)
            : base(clrType, isAbstract, fullName, baseType) {
            if (baseType == null) throw new ArgumentNullException("baseType");
            //if (valueClrType == null) throw new ArgumentNullException("valueClrType");
            //ValueClrType = valueClrType;
            ValueRestrictions = valueRestrictions;
        }
        //public Type ValueClrType { get; private set; }
        public readonly ValueRestrictionSetInfo ValueRestrictions;// { get; private set; }
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
            ValueRestrictionSetInfo valueRestrictions, TypeKind kind)
            : base(clrType, isAbstract, fullName, baseType, valueRestrictions) {
            Kind = kind;
        }
        public readonly TypeKind Kind;

        //new public SimpleTypeInfo BaseType { get { return (SimpleTypeInfo)base.BaseType; } }
    }
    public sealed class ListTypeInfo : SimpleTypeInfo {
        public ListTypeInfo(Type clrType, bool isAbstract, FullName fullName, SimpleTypeInfo baseType, SimpleTypeInfo itemType,
            ValueRestrictionSetInfo valueRestrictions)
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
        public readonly AttributeInfo[] Attributes;
        public AttributeInfo TryGetAttribute(FullName fullName) {
            if (Attributes != null) {
                foreach (var attribute in Attributes) {
                    if (attribute.FullName == fullName) {
                        return attribute;
                    }
                }
            }
            return null;
        }
    }
    public enum EntityDeclKind : byte {
        None = 0,
        Local,
        Global,
        Reference
    }
    public abstract class EntityInfo : NamedObjectInfo {
        public EntityInfo(Type clrType, bool isAbstract, FullName fullName, string displayName, TypeInfo type,
            EntityDeclKind declKind, EntityInfo referencedEntity, bool isNullable, bool isOptional)
            : base(clrType, isAbstract, fullName) {
            DisplayName = displayName;
            Type = type;
            DeclKind = declKind;
            ReferencedEntity = referencedEntity;
            IsNullable = isNullable;
            IsOptional = isOptional;
        }
        public string DisplayName { get; private set; }
        public readonly TypeInfo Type;
        public readonly EntityDeclKind DeclKind;
        public readonly EntityInfo ReferencedEntity;
        public readonly bool IsNullable;
        public bool IsOptional { get; private set; }
        public bool IsLocal {
            get {
                return DeclKind == EntityDeclKind.Local;
            }
        }
        public bool IsGlobal {
            get {
                return DeclKind == EntityDeclKind.Global;
            }
        }
        public bool IsReference {
            get {
                return DeclKind == EntityDeclKind.Reference;
            }
        }
    }
    public sealed class AttributeInfo : EntityInfo {
        public AttributeInfo(Type clrType, FullName fullName, string displayName, SimpleTypeInfo type,
            EntityDeclKind declKind, AttributeInfo referencedAttribute, bool isNullable, bool isOptional)
            : base(clrType, false, fullName, displayName, type, declKind, referencedAttribute, isNullable, isOptional) {
        }
        new public SimpleTypeInfo Type {
            get {
                return (SimpleTypeInfo)base.Type;
            }
        }
        public AttributeInfo ReferencedAttribute {
            get {
                return (AttributeInfo)ReferencedEntity;
            }
        }

    }
    public interface IChildInfo {
        string DisplayName { get; }
        int Order { get; }
        bool IsOptional { get; }
    }

    public sealed class ElementInfo : EntityInfo, IChildInfo {
        public ElementInfo(Type clrType, bool isAbstract, FullName fullName, string displayName, TypeInfo type,
            EntityDeclKind declKind, ElementInfo referencedElement, bool isNullable, bool isOptional,
            int order, ElementInfo substitutedElement, FullName[] directSubstitutingElementFullNames,
            ProgramInfo program)
            : base(clrType, isAbstract, fullName, displayName, type, declKind, referencedElement, isNullable, isOptional) {
            Order = order;
            SubstitutedElement = substitutedElement;
            _directSubstitutingElementFullNames = directSubstitutingElementFullNames;
            Program = program;
        }
        public int Order { get; private set; }
        public ElementInfo ReferencedElement {
            get {
                return (ElementInfo)ReferencedEntity;
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
    public abstract class ChildContainerInfo : ObjectInfo, IChildInfo {
        protected ChildContainerInfo(Type clrType, string displayName, int order, bool isOptional)
            : base(clrType, false) {
            DisplayName = displayName;
            Order = order;
            IsOptional = isOptional;
        }
        public string DisplayName { get; private set; }
        public int Order { get; private set; }
        public bool IsOptional { get; private set; }
    }
    public enum ChildSetKind : byte {
        None = 0,
        Sequence,
        Choice
    }
    public sealed class ChildSetInfo : ChildContainerInfo {
        public ChildSetInfo(Type clrType, string displayName, int order, bool isOptional,
            ChildSetKind kind, IChildInfo[] members)
            : base(clrType, displayName, order, isOptional) {
            Kind = kind;
            Members = members;
        }
        public readonly ChildSetKind Kind;
        public readonly IChildInfo[] Members;
        public IChildInfo TryGetMember(int order) {
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
        public ChildListInfo(Type clrType, string displayName, int order, bool isOptional,
            ulong minOccurs, ulong maxOccurs, IChildInfo item)
            : base(clrType, displayName, order, isOptional) {
            MinOccurs = minOccurs;
            MaxOccurs = maxOccurs;
            Item = item;
        }
        public readonly ulong MinOccurs;
        public readonly ulong MaxOccurs;
        public readonly IChildInfo Item;
    }

}
