using System.Collections.Generic;
using XData.IO.Text;

namespace XData.Compiler {
    internal sealed class SystemTypeNode : TypeNode {
        private SystemTypeNode() : base(null) { }
        private static readonly Dictionary<string, SystemTypeNode> _dict;
        static SystemTypeNode() {
            _dict = new Dictionary<string, SystemTypeNode>();
            var sysNs = NamespaceSymbol.System;
            for (var kind = Extensions.TypeStart; kind <= Extensions.TypeEnd; ++kind) {
                var name = kind.ToString();
                _dict.Add(name, new SystemTypeNode() { _objectSymbol = sysNs.TryGetGlobalObject(name) });
            }
        }
        public static SystemTypeNode TryGet(string name) {
            SystemTypeNode result;
            _dict.TryGetValue(name, out result);
            return result;
        }
    }
    internal class TypeNode : NamespaceMemberNode {
        public TypeNode(Node parent) : base(parent) { }
        public TypeBodyNode Body;
        public override void Resolve() {
            Body.Resolve();
        }
        protected override IGlobalObjectSymbol CreateSymbolCore(NamespaceSymbol parent, string csName, FullName fullName, bool isAbstract, bool isSealed) {
            return Body.CreateSymbolCore(parent, csName, fullName, isAbstract, isSealed);
        }
    }
    internal abstract class TypeBodyNode : ObjectNode {
        protected TypeBodyNode(Node parent) : base(parent) { }
        public abstract void Resolve();
        public abstract TypeSymbol CreateSymbolCore(NamespaceSymbol parent, string csName, FullName fullName, bool isAbstract, bool isSealed);
    }
    internal sealed class TypeListNode : TypeBodyNode {
        public TypeListNode(Node parent) : base(parent) { }
        public QualifiableNameNode ItemTypeQName;
        public TypeNode ItemType;
        public FacetsNode Facets;//opt
        public override void Resolve() {
            ItemType = NamespaceAncestor.ResolveAsType(ItemTypeQName);
            if (Facets != null && Facets.ListItemTypeQName.IsValid) {
                DiagContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.FacetNotAllowedForType, TypeKind.ListType.ToString()),
                    Facets.ListItemTypeQName.TextSpan);
            }
        }
        public override TypeSymbol CreateSymbolCore(NamespaceSymbol parent, string csName, FullName fullName, bool isAbstract, bool isSealed) {
            var itemTypeSymbol = ItemType.CreateSymbol() as SimpleTypeSymbol;
            if (itemTypeSymbol == null) {
                DiagContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.SimpleTypeRequired), ItemTypeQName.TextSpan);
            }
            var listTypeSymbol = new ListTypeSymbol(parent, csName, isAbstract, isSealed, fullName, NamespaceSymbol.SystemListType,
                CSEX.XListTypeOf(itemTypeSymbol.CSFullName), null, itemTypeSymbol);
            if (Facets != null) {
                listTypeSymbol.Facets = Facets.CreateSymbol(listTypeSymbol, TypeKind.ListType, null);
            }
            return listTypeSymbol;
        }
    }
    internal sealed class AttributesChildrenNode : ObjectNode {
        public AttributesChildrenNode(Node parent) : base(parent) { }
        public AttributesNode Attributes;
        public ComplexChildrenNode ComplexChildren;
        public QualifiableNameNode SimpleChildQName;
        public TypeNode SimpleChild;
        public TextSpan OpenTextSpan {
            get {
                if (Attributes != null) {
                    return Attributes.OpenBracketTextSpan;
                }
                if (ComplexChildren != null) {
                    return ComplexChildren.OpenBraceTextSpan;
                }
                return SimpleChildQName.TextSpan;
            }
        }
        public void Resolve() {
            if (Attributes != null) {
                Attributes.Resolve();
            }
            if (ComplexChildren != null) {
                ComplexChildren.Resolve();
            }
            else if (SimpleChildQName.IsValid) {
                SimpleChild = NamespaceAncestor.ResolveAsType(SimpleChildQName);
            }
        }
        public SimpleTypeSymbol CreateSimpleChildSymbol() {
            var result = SimpleChild.CreateSymbol() as SimpleTypeSymbol;
            if (result == null) {
                DiagContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.SimpleTypeRequired), SimpleChildQName.TextSpan);
            }
            return result;
        }
    }
    internal class TypeDirectnessNode : TypeBodyNode {
        public TypeDirectnessNode(Node parent) : base(parent) { }
        public AttributesChildrenNode AttributesChildren;
        public override void Resolve() {
            if (AttributesChildren != null) {
                AttributesChildren.Resolve();
            }
        }
        public override TypeSymbol CreateSymbolCore(NamespaceSymbol parent, string csName, FullName fullName, bool isAbstract, bool isSealed) {
            var complexTypeSymbol = new ComplexTypeSymbol(parent, csName, isAbstract, isSealed, fullName, NamespaceSymbol.SystemComplexType);
            if (AttributesChildren != null) {
                if (AttributesChildren.Attributes != null) {
                    complexTypeSymbol.Attributes = AttributesChildren.Attributes.CreateSymbol(complexTypeSymbol, null, true);
                }
                if (AttributesChildren.ComplexChildren != null) {
                    complexTypeSymbol.Children = AttributesChildren.ComplexChildren.CreateSymbol(complexTypeSymbol, null, true);
                }
                else if (AttributesChildren.SimpleChild != null) {
                    complexTypeSymbol.Children = AttributesChildren.CreateSimpleChildSymbol();
                }
            }
            return complexTypeSymbol;
        }
    }
    internal class TypeExtension : TypeDirectnessNode {
        public TypeExtension(Node parent) : base(parent) { }
        public QualifiableNameNode BaseTypeQName;
        public TypeNode BaseType;
        public override void Resolve() {
            BaseType = NamespaceAncestor.ResolveAsType(BaseTypeQName);
            base.Resolve();
        }
        public override TypeSymbol CreateSymbolCore(NamespaceSymbol parent, string csName, FullName fullName, bool isAbstract, bool isSealed) {
            var baseComplexTypeSymbol = BaseType.CreateSymbol() as ComplexTypeSymbol;
            if (baseComplexTypeSymbol == null) {
                DiagContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.ComplexTypeRequired),
                    BaseTypeQName.TextSpan);
            }
            if (baseComplexTypeSymbol.IsSealed) {
                DiagContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.BaseTypeSealed, baseComplexTypeSymbol.DisplayName),
                    BaseTypeQName.TextSpan);
            }
            if (baseComplexTypeSymbol == NamespaceSymbol.SystemComplexType) {
                DiagContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.CannotExtendOrRestrictSysComplexType),
                    BaseTypeQName.TextSpan);
            }
            var complexTypeSymbol = new ComplexTypeSymbol(parent, csName, isAbstract, isSealed, fullName, baseComplexTypeSymbol) {
                Attributes = baseComplexTypeSymbol.Attributes,
                Children = baseComplexTypeSymbol.Children
            };
            if (AttributesChildren != null) {
                if (AttributesChildren.Attributes != null) {
                    complexTypeSymbol.Attributes = AttributesChildren.Attributes.CreateSymbol(complexTypeSymbol,
                        baseComplexTypeSymbol.Attributes, true);
                }
                if (AttributesChildren.ComplexChildren != null) {
                    if (baseComplexTypeSymbol.SimpleChild != null) {
                        DiagContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.CannotExtendSimpleChildWithComplexChildren),
                            AttributesChildren.ComplexChildren.OpenBraceTextSpan);
                    }
                    complexTypeSymbol.Children = AttributesChildren.ComplexChildren.CreateSymbol(complexTypeSymbol, 
                        baseComplexTypeSymbol.ComplexChildren, true);
                }
                else if (AttributesChildren.SimpleChild != null) {
                    if (baseComplexTypeSymbol.Children != null) {
                        DiagContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.CannotExtendChildrenWithSimpleChild),
                            AttributesChildren.SimpleChildQName.TextSpan);
                    }
                    complexTypeSymbol.Children = AttributesChildren.CreateSimpleChildSymbol();
                }
            }
            return complexTypeSymbol;
        }
    }
    internal sealed class TypeRestriction : TypeExtension {
        public TypeRestriction(Node parent) : base(parent) { }
        public FacetsNode Facets;
        public override void Resolve() {
            base.Resolve();
            if (Facets != null) {
                Facets.Resolve();
            }
        }
        public override TypeSymbol CreateSymbolCore(NamespaceSymbol parent, string csName, FullName fullName, bool isAbstract, bool isSealed) {
            var baseTypeSymbol = (TypeSymbol)BaseType.CreateSymbol();
            if (baseTypeSymbol.IsSealed) {
                DiagContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.BaseTypeSealed, baseTypeSymbol.DisplayName),
                    BaseTypeQName.TextSpan);
            }
            var baseSimpleTypeSymbol = baseTypeSymbol as SimpleTypeSymbol;
            if (baseSimpleTypeSymbol != null) {
                if (baseSimpleTypeSymbol == NamespaceSymbol.SystemSimpleType || baseSimpleTypeSymbol == NamespaceSymbol.SystemAtomType
                    || baseSimpleTypeSymbol == NamespaceSymbol.SystemListType) {
                    DiagContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.CannotRestrictSysSimpleAtomListType), BaseTypeQName.TextSpan);
                }
                if (AttributesChildren != null) {
                    DiagContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.AttributesChildrenNotAllowedInSimpleTypeRestriction),
                        AttributesChildren.OpenTextSpan);
                }
                var typeKind = baseSimpleTypeSymbol.Kind;
                if (typeKind == TypeKind.ListType) {
                    SimpleTypeSymbol itemTypeSymbol = null;
                    var baseListTypeSymbol = (ListTypeSymbol)baseSimpleTypeSymbol;
                    if (Facets != null && Facets.ListItemType != null) {
                        itemTypeSymbol = Facets.ListItemType.CreateSymbol() as SimpleTypeSymbol;
                        if (itemTypeSymbol == null) {
                            DiagContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.SimpleTypeRequired), Facets.ListItemTypeQName.TextSpan);
                        }
                        if (itemTypeSymbol == baseListTypeSymbol.ItemType) {
                            itemTypeSymbol = null;
                        }
                        else if (!itemTypeSymbol.EqualToOrDeriveFrom(baseListTypeSymbol.ItemType)) {
                            DiagContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.TypeNotEqualToOrDeriveFrom,
                                itemTypeSymbol.DisplayName, baseListTypeSymbol.ItemType.DisplayName), Facets.ListItemTypeQName.TextSpan);
                        }
                    }
                    var listTypeSymbol = new ListTypeSymbol(parent, csName, isAbstract, isSealed, fullName, baseListTypeSymbol,
                        baseListTypeSymbol.CSFullName,
                        itemTypeSymbol == null ? null : CSEX.IListAndIReadOnlyListOf(itemTypeSymbol.CSFullName),
                        itemTypeSymbol ?? baseListTypeSymbol.ItemType);
                    listTypeSymbol.Facets = Facets == null ? baseSimpleTypeSymbol.Facets :
                        Facets.CreateSymbol(listTypeSymbol, typeKind, baseSimpleTypeSymbol.Facets);
                    return listTypeSymbol;
                }
                else {
                    var atomTypeSymbol = new AtomTypeSymbol(parent, csName, isAbstract, isSealed, fullName, typeKind, baseSimpleTypeSymbol,
                       ((AtomTypeSymbol)baseSimpleTypeSymbol).ValueCSFullName);
                    atomTypeSymbol.Facets = Facets == null ? baseSimpleTypeSymbol.Facets :
                        Facets.CreateSymbol(atomTypeSymbol, typeKind, baseSimpleTypeSymbol.Facets);
                    return atomTypeSymbol;
                }
            }
            else {
                var baseComplexTypeSymbol = (ComplexTypeSymbol)baseTypeSymbol;
                if (baseComplexTypeSymbol == NamespaceSymbol.SystemComplexType) {
                    DiagContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.CannotExtendOrRestrictSysComplexType),
                        BaseTypeQName.TextSpan);
                }
                if (Facets != null) {
                    DiagContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.FacetsNotAllowedInComplexTypeRestriction),
                        Facets.OpenBraceTextSpan);
                }
                var complexTypeSymbol = new ComplexTypeSymbol(parent, csName, isAbstract, isSealed, fullName, baseComplexTypeSymbol) {
                    Attributes = baseComplexTypeSymbol.Attributes,
                    Children = baseComplexTypeSymbol.Children
                };
                if (AttributesChildren != null) {
                    if (AttributesChildren.Attributes != null) {
                        complexTypeSymbol.Attributes = AttributesChildren.Attributes.CreateSymbol(complexTypeSymbol, 
                            baseComplexTypeSymbol.Attributes, false);
                    }
                    if (AttributesChildren.ComplexChildren != null) {
                        if (baseComplexTypeSymbol.SimpleChild != null) {
                            DiagContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.CannotRestrictSimpleChildWithComplexChildren),
                                AttributesChildren.ComplexChildren.OpenBraceTextSpan);
                        }
                        complexTypeSymbol.Children = AttributesChildren.ComplexChildren.CreateSymbol(complexTypeSymbol,
                            baseComplexTypeSymbol.ComplexChildren, false);
                    }
                    else if (AttributesChildren.SimpleChild != null) {
                        if (baseComplexTypeSymbol.ComplexChildren != null) {
                            DiagContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.CannotRestrictComplexChildrenWithSimpleChild),
                                AttributesChildren.SimpleChildQName.TextSpan);
                        }
                        var baseSimpleChildSymbol = baseComplexTypeSymbol.SimpleChild;
                        if (baseSimpleChildSymbol == null) {
                            DiagContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.CannotRestrictNullSimpleChild),
                                AttributesChildren.SimpleChildQName.TextSpan);
                        }
                        var simpleChildSymbol = AttributesChildren.CreateSimpleChildSymbol();
                        if (!simpleChildSymbol.EqualToOrDeriveFrom(baseSimpleChildSymbol)) {
                            DiagContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.TypeNotEqualToOrDeriveFrom,
                                simpleChildSymbol.DisplayName, baseSimpleChildSymbol.DisplayName), AttributesChildren.SimpleChildQName.TextSpan);
                        }
                        complexTypeSymbol.Children = simpleChildSymbol;
                    }
                }
                return complexTypeSymbol;
            }
        }
    }
}
