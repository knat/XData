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
            //NamespaceList.Add(NamespaceSymbol.System);
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
        public NamedObjectSymbol TryGetGlobalObject(FullName fullName) {
            foreach (var obj in GlobalObjectList) {
                if (obj.FullName == fullName) {
                    return obj;
                }
            }
            return null;
        }


        //
        public static readonly NamespaceSymbol System;
        static NamespaceSymbol() {
            System = new NamespaceSymbol(InfoExtensions.SystemUri, new CSNamespaceNameNode { "XData" }, true);
            var list = System.GlobalObjectList;
            var SimpleType = new SimpleTypeSymbol(System, TypeKind.SimpleType.ToClassName(), true, false, null, null, TypeKind.SimpleType.ToFullName(), TypeKind.SimpleType, null, null);
            list.Add(SimpleType);
            var AtomType = new AtomTypeSymbol(System, TypeKind.AtomType.ToClassName(), true, false, TypeKind.AtomType.ToFullName(), TypeKind.AtomType, SimpleType, null, null);
            list.Add(AtomType);
            var StringBase = new AtomTypeSymbol(System, TypeKind.StringBase.ToClassName(), true, false, TypeKind.StringBase.ToFullName(), TypeKind.StringBase, AtomType, null, CS.StringType);
            list.Add(StringBase);
            CreateAndAdd(StringBase, TypeKind.String, CS.StringType);
            CreateAndAdd(StringBase, TypeKind.IgnoreCaseString, CS.StringType);
            var Decimal = CreateAndAdd(AtomType, TypeKind.Decimal, CS.DecimalType);
            var Int64 = CreateAndAdd(Decimal, TypeKind.Int64, CS.LongType);
            var Int32 = CreateAndAdd(Int64, TypeKind.Int32, CS.IntType);
            var Int16 = CreateAndAdd(Int32, TypeKind.Int16, CS.ShortType);
            CreateAndAdd(Int16, TypeKind.SByte, CS.SByteType);
            var UInt64 = CreateAndAdd(Decimal, TypeKind.UInt64, CS.ULongType);
            var UInt32 = CreateAndAdd(UInt64, TypeKind.UInt32, CS.UIntType);
            var UInt16 = CreateAndAdd(UInt32, TypeKind.UInt16, CS.UShortType);
            CreateAndAdd(UInt16, TypeKind.Byte, CS.ByteType);
            var Double = CreateAndAdd(AtomType, TypeKind.Double, CS.DoubleType);
            CreateAndAdd(Double, TypeKind.Single, CS.FloatType);
            CreateAndAdd(AtomType, TypeKind.Boolean, CS.BoolType);
            CreateAndAdd(AtomType, TypeKind.Binary, CS.ByteArrayType);
            CreateAndAdd(AtomType, TypeKind.Guid, CS.GuidName);
            CreateAndAdd(AtomType, TypeKind.TimeSpan, CS.TimeSpanName);
            CreateAndAdd(AtomType, TypeKind.DateTimeOffset, CS.DateTimeOffsetName);
            var ListType = new ListTypeSymbol(System, TypeKind.ListType.ToClassName(), true, false, TypeKind.ListType.ToFullName(), TypeKind.ListType, SimpleType, null, null, null);
            list.Add(ListType);


        }
        private static AtomTypeSymbol CreateAndAdd(AtomTypeSymbol baseType, TypeKind kind, TypeSyntax valueCSFullName) {
            var symbol = new AtomTypeSymbol(System, kind.ToClassName(), false, false, kind.ToFullName(), kind, baseType, null, valueCSFullName);
            System.GlobalObjectList.Add(symbol);
            return symbol;
        }
    }
    public abstract class ObjectSymbol : ObjectBaseSymbol {
        protected ObjectSymbol(ObjectBaseSymbol parent, string csName, bool isAbstract, bool isSealed, bool isCSOverride,
            NameSyntax csBaseFullName, NameSyntax[] csInterfaceNames)
            : base(CS.QualifiedName(parent.CSFullName, csName), CS.MemberAccessExpr(parent.CSFullExp, csName)) {
            Parent = parent;
            CSName = csName;
            IsAbstract = isAbstract;
            IsSealed = isSealed;
            IsCSOverride = isCSOverride;
            CSBaseFullName = csBaseFullName;
            CSInterfaceNames = csInterfaceNames;
        }
        public readonly ObjectBaseSymbol Parent;
        public readonly string CSName;//C# classs escaped name
        public readonly bool IsAbstract;
        public readonly bool IsSealed;
        public readonly bool IsCSOverride;//same name inner class? (need 'new' modifier)
        public readonly NameSyntax CSBaseFullName;
        public readonly NameSyntax[] CSInterfaceNames;//opt
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
        protected NamedObjectSymbol(ObjectBaseSymbol parent, string csName, bool isAbstract, bool isSealed, bool isCSOverride,
            NameSyntax csBaseFullName, NameSyntax[] csInterfaceNames, FullName fullName)
            : base(parent, csName, isAbstract, isSealed, isCSOverride, csBaseFullName, csInterfaceNames) {
            FullName = fullName;
        }
        public readonly FullName FullName;
    }
    public abstract class TypeSymbol : NamedObjectSymbol {
        protected TypeSymbol(ObjectBaseSymbol parent, string csName, bool isAbstract, bool isSealed,
            NameSyntax csBaseFullName, NameSyntax[] csInterfaceNames, FullName fullName, TypeKind kind, TypeSymbol baseType)
            : base(parent, csName, isAbstract, isSealed, false, csBaseFullName, csInterfaceNames, fullName) {
            Kind = kind;
            BaseType = baseType;
        }
        public readonly TypeKind Kind;
        public readonly TypeSymbol BaseType;
        public bool IsEqualToOrDeriveFrom(TypeSymbol other) {
            if (other == null) throw new ArgumentNullException("other");
            for (var info = this; info != null; info = info.BaseType) {
                if (info == other) {
                    return true;
                }
            }
            return false;
        }

    }
    public class SimpleTypeSymbol : TypeSymbol {
        public SimpleTypeSymbol(ObjectBaseSymbol parent, string csName, bool isAbstract, bool isSealed,
            NameSyntax csBaseFullName, NameSyntax[] csInterfaceNames, FullName fullName, TypeKind kind, TypeSymbol baseType,
            SimpleTypeRestrictionSetInfo restrictionSet)
            : base(parent, csName, isAbstract, isSealed, csBaseFullName, csInterfaceNames, fullName, kind, baseType) {
            RestrictionSet = restrictionSet;
        }
        public readonly SimpleTypeRestrictionSetInfo RestrictionSet;
    }
    public sealed class AtomTypeSymbol : SimpleTypeSymbol {
        public AtomTypeSymbol(ObjectBaseSymbol parent, string csName, bool isAbstract, bool isSealed,
            FullName fullName, TypeKind kind, SimpleTypeSymbol baseType, SimpleTypeRestrictionSetInfo restrictionSet,
            TypeSyntax valueCSFullName)
            : base(parent, csName, isAbstract, isSealed, baseType.CSFullName, null, fullName, kind, baseType, restrictionSet) {
            ValueCSFullName = valueCSFullName;
        }
        public readonly TypeSyntax ValueCSFullName;
    }
    public sealed class ListTypeSymbol : SimpleTypeSymbol {
        public ListTypeSymbol(ObjectBaseSymbol parent, string csName, bool isAbstract, bool isSealed,
            FullName fullName, TypeKind kind, SimpleTypeSymbol baseType, SimpleTypeRestrictionSetInfo restrictionSet,
            NameSyntax[] csInterfaceNames, SimpleTypeSymbol itemType)
            : base(parent, csName, isAbstract, isSealed, baseType.CSFullName, csInterfaceNames, fullName, kind, baseType, restrictionSet) {
            ItemType = itemType;
        }
        public readonly SimpleTypeSymbol ItemType;
    }

    //public sealed class ComplexTypeSymbol : TypeSymbol {
    //    //public ComplexTypeSymbol(ObjectBaseSymbol parent, string csName, bool isAbstract, bool isSealed, NameSyntax csBaseFullName,
    //    //    FullName fullName, TypeSymbol baseType)
    //    //    : base(parent, csName, isAbstract, isSealed, csBaseFullName, fullName, AtomicTypeKind.ComplexType, baseType) {

    //    //}
    //}
}