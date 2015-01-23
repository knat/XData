using System;
using System.Collections.Generic;
using XData.IO.Text;

namespace XData.Compiler {
    public sealed class SystemTypeNode : TypeNode {
        private SystemTypeNode() : base(null) { }
        private static readonly Dictionary<string, SystemTypeNode> _dict;
        static SystemTypeNode() {
            _dict = new Dictionary<string, SystemTypeNode>();
            var sysNs = NamespaceSymbol.System;
            for (var kind = InfoExtensions.TypeStart; kind <= InfoExtensions.TypeEnd; ++kind) {
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
    public class TypeNode : NamespaceMemberNode {
        public TypeNode(Node parent) : base(parent) { }
        public TypeBodyNode Body;
        public override void Resolve() {
            Body.Resolve();
        }
        protected override NamedObjectSymbol CreateSymbolCore(NamespaceSymbol parent, string csName, FullName fullName) {
            return Body.CreateSymbolCore(parent, csName, fullName, IsAbstract, IsSealed);
        }
    }
    public abstract class TypeBodyNode : Node {
        protected TypeBodyNode(Node parent) : base(parent) { }
        public abstract void Resolve();
        public abstract TypeSymbol CreateSymbolCore(NamespaceSymbol parent, string csName, FullName fullName, bool isAbstract, bool isSealed);
    }
    public sealed class TypeListNode : TypeBodyNode {
        public TypeListNode(Node parent) : base(parent) { }
        public QualifiableNameNode ItemTypeQName;
        public TypeNode ItemType;
        public ValueRestrictionsNode ValueRestrictions;//opt
        public override void Resolve() {
            ItemType = NamespaceAncestor.ResolveAsType(ItemTypeQName);
            if (ValueRestrictions != null && ValueRestrictions.ListItemTypeQName.IsValid) {
                ContextEx.ErrorDiag(new DiagMsgEx(DiagCodeEx.ValueRestrictionNotApplicable), ValueRestrictions.ListItemTypeQName.TextSpan);
            }
        }
        public override TypeSymbol CreateSymbolCore(NamespaceSymbol parent, string csName, FullName fullName, bool isAbstract, bool isSealed) {
            var itemSimpleTypeSymbol = ItemType.CreateSymbol() as SimpleTypeSymbol;
            if (itemSimpleTypeSymbol == null) {
                ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.SimpleTypeRequired), ItemTypeQName.TextSpan);
            }
            if (ValueRestrictions != null) {
                if (ValueRestrictions.ListItemTypeQName.IsValid) {

                }
            }

            return null;// new ListTypeSymbol(parent, csName, isAbstract, isSealed, fullName, itemSymbol);
        }
    }
    public sealed class AttributesChildrenNode : Node {
        public AttributesChildrenNode(Node parent) : base(parent) { }
        public AttributesNode Attributes;
        public RootComplexChildrenNode ComplexChildren;
        public QualifiableNameNode SimpleChildQName;
        public TypeNode SimpleChild;
        public SimpleTypeSymbol CreateSimpleChildSymbol() {
            var result = SimpleChild.CreateSymbol() as SimpleTypeSymbol;
            if (result == null) {
                ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.SimpleTypeRequired), SimpleChildQName.TextSpan);
            }
            return result;
        }
        public TextSpan OpenToken {
            get {
                if (Attributes != null) {
                    return Attributes.OpenBracketToken;
                }
                if (ComplexChildren != null) {
                    return ComplexChildren.OpenBraceToken;
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
    }
    public class TypeDirectnessNode : TypeBodyNode {
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
                    complexTypeSymbol.Attributes = AttributesChildren.Attributes.CreateSymbol(complexTypeSymbol, null, false);
                }
                if (AttributesChildren.ComplexChildren != null) {
                    complexTypeSymbol.Children = AttributesChildren.ComplexChildren.CreateSymbol(complexTypeSymbol, null, false);
                }
                else if (AttributesChildren.SimpleChild != null) {
                    complexTypeSymbol.Children = AttributesChildren.CreateSimpleChildSymbol();
                }
            }
            return complexTypeSymbol;
        }
    }
    public class TypeExtension : TypeDirectnessNode {
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
                ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.ComplexTypeRequired), BaseTypeQName.TextSpan);
            }
            if (baseComplexTypeSymbol.IsSealed) {
                ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.BaseTypeIsSealed, baseComplexTypeSymbol.FullName.ToString()), BaseTypeQName.TextSpan);
            }
            if (baseComplexTypeSymbol == NamespaceSymbol.SystemComplexType) {
                ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.CannotExtendOrRestrictSysComplexType), BaseTypeQName.TextSpan);
            }
            var complexTypeSymbol = new ComplexTypeSymbol(parent, csName, isAbstract, isSealed, fullName, baseComplexTypeSymbol);
            if (AttributesChildren != null) {
                if (AttributesChildren.Attributes != null) {
                    complexTypeSymbol.Attributes = AttributesChildren.Attributes.CreateSymbol(complexTypeSymbol, baseComplexTypeSymbol.Attributes, true);
                }
                if (AttributesChildren.ComplexChildren != null) {
                    if (baseComplexTypeSymbol.SimpleChild != null) {
                        ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.CannotExtendSimpleChildWithComplexChildren), AttributesChildren.ComplexChildren.OpenBraceToken);
                    }
                    complexTypeSymbol.Children = AttributesChildren.ComplexChildren.CreateSymbol(complexTypeSymbol, baseComplexTypeSymbol.ComplexChildren, true);
                }
                else if (AttributesChildren.SimpleChild != null) {
                    if (baseComplexTypeSymbol.Children != null) {
                        ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.CannotExtendChildrenWithSimpleChild), AttributesChildren.SimpleChildQName.TextSpan);
                    }
                    complexTypeSymbol.Children = AttributesChildren.CreateSimpleChildSymbol();
                }
            }
            return complexTypeSymbol;
        }
    }
    public sealed class TypeRestriction : TypeExtension {
        public TypeRestriction(Node parent) : base(parent) { }
        public ValueRestrictionsNode ValueRestrictions;
        public override void Resolve() {
            base.Resolve();
            if (ValueRestrictions != null) {
                ValueRestrictions.Resolve();
            }
        }
        //private static SimpleTypeSymbol CreateSimpleTypeSymbol(NamespaceSymbol parent, string csName, FullName fullName, bool isAbstract, bool isSealed,
        //    SimpleTypeSymbol baseSimpleTypeSymbol, ValueRestrictionsNode valueRestrictions) {

        //    return null;
        //}
        public override TypeSymbol CreateSymbolCore(NamespaceSymbol parent, string csName, FullName fullName, bool isAbstract, bool isSealed) {
            var baseTypeSymbol = BaseType.CreateSymbol();
            if (baseTypeSymbol.IsSealed) {
                ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.BaseTypeIsSealed, baseTypeSymbol.FullName.ToString()), BaseTypeQName.TextSpan);
            }
            var baseSimpleTypeSymbol = baseTypeSymbol as SimpleTypeSymbol;
            if (baseSimpleTypeSymbol != null) {
                if (baseSimpleTypeSymbol == NamespaceSymbol.SystemSimpleType || baseSimpleTypeSymbol == NamespaceSymbol.SystemAtomType || baseSimpleTypeSymbol == NamespaceSymbol.SystemListType) {
                    ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.CannotRestrictSysSimpleAtomListType), BaseTypeQName.TextSpan);
                }
                if (AttributesChildren != null) {
                    ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.CannotRestrictSimpleTypeWithAttributesOrChildren), AttributesChildren.OpenToken);
                }
                return null;// CreateSimpleTypeSymbol(parent, csName, fullName, isAbstract, isSealed, baseSimpleTypeSymbol, ValueRestrictions);
            }
            else {
                var baseComplexTypeSymbol = (ComplexTypeSymbol)baseTypeSymbol;
                if (baseComplexTypeSymbol == NamespaceSymbol.SystemComplexType) {
                    ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.CannotExtendOrRestrictSysComplexType), BaseTypeQName.TextSpan);
                }
                if (ValueRestrictions != null) {
                    ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.CannotRestrictComplexTypeWithValueRestrictions), ValueRestrictions.OpenBraceToken);
                }
                var complexTypeSymbol = new ComplexTypeSymbol(parent, csName, isAbstract, isSealed, fullName, baseComplexTypeSymbol);
                if (AttributesChildren != null) {
                    if (AttributesChildren.Attributes != null) {
                        complexTypeSymbol.Attributes = AttributesChildren.Attributes.CreateSymbol(complexTypeSymbol, baseComplexTypeSymbol.Attributes, false);
                    }
                    if (AttributesChildren.ComplexChildren != null) {
                        if (baseComplexTypeSymbol.SimpleChild != null) {
                            ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.CannotRestrictSimpleChildWithComplexChildren), AttributesChildren.ComplexChildren.OpenBraceToken);
                        }
                        complexTypeSymbol.Children = AttributesChildren.ComplexChildren.CreateSymbol(complexTypeSymbol, baseComplexTypeSymbol.ComplexChildren, false);
                    }
                    else if (AttributesChildren.SimpleChild != null) {
                        if (baseComplexTypeSymbol.ComplexChildren != null) {
                            ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.CannotRestrictComplexChildrenWithSimpleChild), AttributesChildren.SimpleChildQName.TextSpan);
                        }
                        var baseSimpleChildSymbol = baseComplexTypeSymbol.SimpleChild;
                        if (baseSimpleChildSymbol == null) {
                            ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.CannotRestrictNullSimpleChild), AttributesChildren.SimpleChildQName.TextSpan);
                        }
                        var simpleChildSymbol = AttributesChildren.CreateSimpleChildSymbol();
                        if (!simpleChildSymbol.IsEqualToOrDeriveFrom(baseSimpleChildSymbol)) {
                            ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.IsNotEqualToOrDeriveFrom, simpleChildSymbol.FullName.ToString(), baseSimpleChildSymbol.FullName.ToString()), AttributesChildren.SimpleChildQName.TextSpan);
                        }
                        complexTypeSymbol.Children = simpleChildSymbol;
                    }
                }
                return complexTypeSymbol;
            }
        }
    }
}
