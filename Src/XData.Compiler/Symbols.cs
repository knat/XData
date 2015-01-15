using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace XData.Compiler {
    public abstract class Symbol {

    }
    public sealed class ProgramSymbol : Symbol {
        public ProgramSymbol(bool needGenCode) {
            NeedGenCode = needGenCode;
            NamespaceList = new List<NamespaceSymbol>();
        }
        public readonly bool NeedGenCode;
        public readonly List<NamespaceSymbol> NamespaceList;
    }
    public abstract class ObjectBaseSymbol : Symbol {
        protected ObjectBaseSymbol(NameSyntax csFullName, ExpressionSyntax csFullExp) {
            CSFullName = csFullName;
            CSFullExp = csFullExp;
        }
        public readonly NameSyntax CSFullName;//C# qualified name
        public readonly ExpressionSyntax CSFullExp;//C# member access expr
        private List<MemberDeclarationSyntax> _csMemberList;
        public List<MemberDeclarationSyntax> CSMemberList {
            get {
                return _csMemberList ?? (_csMemberList = new List<MemberDeclarationSyntax>());
            }
        }
    }
    public sealed class NamespaceSymbol : ObjectBaseSymbol {
        public NamespaceSymbol(string uri, CSNamespaceNameNode csNamespaceName, bool isCSNamespaceRef)
            : base(csNamespaceName.CSFullName, csNamespaceName.CSFullExp) {
            Uri = uri;
            CSNamespaceName = csNamespaceName;
            IsCSNamespaceRef = isCSNamespaceRef;
            GlobalObjectList = new List<NamedObjectSymbol>();
        }
        public readonly string Uri;
        public readonly CSNamespaceNameNode CSNamespaceName;
        public readonly bool IsCSNamespaceRef;
        public readonly List<NamedObjectSymbol> GlobalObjectList;
        //
        public static NamespaceSymbol System;
    }
    public abstract class ObjectSymbol : ObjectBaseSymbol {
        protected ObjectSymbol(ObjectBaseSymbol parent, string csName, bool isAbstract, bool isSealed, bool isCSOverride, NameSyntax csBaseFullName)
            : base(CS.QualifiedName(parent.CSFullName, csName), CS.MemberAccessExpr(parent.CSFullExp, csName)) {
            Parent = parent;
            CSName = csName;
            IsAbstract = isAbstract;
            IsSealed = isSealed;
            IsCSOverride = isCSOverride;
            CSBaseFullName = csBaseFullName;
        }
        public readonly ObjectBaseSymbol Parent;
        public readonly string CSName;//C# classs escaped name
        public readonly bool IsAbstract;
        public readonly bool IsSealed;
        public readonly bool IsCSOverride;//same name inner class? (need 'new' modifier)
        public readonly NameSyntax CSBaseFullName;
        //
        private ExpressionSyntax _thisInfoExp;
        public ExpressionSyntax ThisInfoExp {
            get {
                return _thisInfoExp ?? (_thisInfoExp = CS.MemberAccessExpr(CSFullExp, "ThisInfo"));
            }
        }

        private bool _isGenerated;
        public void GenerateCSClass() {
            if (_isGenerated == false) {
                _isGenerated = true;
            }
        }

    }
    public abstract class NamedObjectSymbol : ObjectSymbol {
        protected NamedObjectSymbol(ObjectBaseSymbol parent, string csName, bool isAbstract, bool isSealed, bool isCSOverride, NameSyntax csBaseFullName,
            FullName fullName)
            : base(parent, csName, isAbstract, isSealed, isCSOverride, csBaseFullName) {
            FullName = fullName;
        }
        public readonly FullName FullName;
    }
    public class TypeSymbol : NamedObjectSymbol {
        protected TypeSymbol(ObjectBaseSymbol parent, string csName, bool isAbstract, bool isSealed, NameSyntax csBaseFullName,
            FullName fullName, TypeKind kind, TypeSymbol baseType)
            : base(parent, csName, isAbstract, isSealed, false, csBaseFullName, fullName) {
            Kind = kind;
            BaseType = baseType;
        }
        public TypeKind Kind { get; private set; }
        public readonly TypeSymbol BaseType;
        public static readonly TypeSymbol Instance = new TypeSymbol(NamespaceSymbol.System, XType.ThisInfo.ClrTypeName, true, false, null, XType.ThisInfo.FullName, TypeKind.Type, null);

    }
    public class SimpleTypeSymbol : TypeSymbol {
        protected SimpleTypeSymbol(ObjectBaseSymbol parent, string csName, bool isAbstract, bool isSealed, NameSyntax csBaseFullName,
            FullName fullName, TypeKind kind, TypeSymbol baseType,
            TypeSyntax valueCSFullName, TypeSyntax nullableValueCSFullName, Type valueClrType, bool isValueClrTypeRef, SimpleTypeRestrictionSetInfo? restrictionSet)
            : base(parent, csName, isAbstract, isSealed, csBaseFullName, fullName, kind, baseType) {
            ValueCSFullName = valueCSFullName;
            NullableValueCSFullName = nullableValueCSFullName ?? valueCSFullName;
            ValueClrType = valueClrType;
            IsValueClrTypeRef = isValueClrTypeRef;
            RestrictionSet = restrictionSet;
        }
        public readonly TypeSyntax ValueCSFullName;
        public readonly TypeSyntax NullableValueCSFullName;//if value is ref type, same as ValueCSFullName
        public Type ValueClrType { get; private set; }
        public readonly bool IsValueClrTypeRef;
        public SimpleTypeRestrictionSetInfo? RestrictionSet { get; private set; }
        //
        new public static readonly SimpleTypeSymbol Instance = new SimpleTypeSymbol(NamespaceSymbol.System, XSimpleType.ThisInfo.ClrTypeName, true, false, TypeSymbol.Instance.CSFullName, XSimpleType.ThisInfo.FullName, TypeKind.SimpleType, TypeSymbol.Instance,
            CS.ObjectType, null, typeof(object), true, null);
    }
    public sealed class AtomicTypeSymbol : SimpleTypeSymbol {
        public AtomicTypeSymbol(ObjectBaseSymbol parent, string csName, bool isAbstract, bool isSealed, NameSyntax csBaseFullName,
            FullName fullName, TypeKind kind, TypeSymbol baseType,
            TypeSyntax valueCSFullName, TypeSyntax nullableValueCSFullName, Type valueClrType, bool isValueClrTypeRef, SimpleTypeRestrictionSetInfo? restrictionSet)
            : base(parent, csName, isAbstract, isSealed, csBaseFullName, fullName, kind, baseType,
                  valueCSFullName, nullableValueCSFullName, valueClrType, isValueClrTypeRef, restrictionSet) {
        }
    }
    public sealed class ListTypeSymbol : SimpleTypeSymbol {
        public ListTypeSymbol(ObjectBaseSymbol parent, string csName, bool isAbstract, bool isSealed,
            FullName fullName, SimpleTypeSymbol itemType)
            : base(parent, csName, isAbstract, isSealed, SimpleTypeSymbol.Instance.CSFullName, fullName, TypeKind.ListType, SimpleTypeSymbol.Instance,
                 null, null, null, true, null) {

            ItemType = itemType;
        }
        public readonly SimpleTypeSymbol ItemType;
    }
    public sealed class ComplexTypeSymbol : TypeSymbol {
        public ComplexTypeSymbol(ObjectBaseSymbol parent, string csName, bool isAbstract, bool isSealed, NameSyntax csBaseFullName,
            FullName fullName, TypeSymbol baseType)
            : base(parent, csName, isAbstract, isSealed, csBaseFullName, fullName, TypeKind.ComplexType, baseType) {

        }
    }
}