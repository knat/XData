using System;
using System.Collections.Generic;
using XData.IO.Text;

namespace XData.Compiler {
    public sealed class SystemTypeNode : TypeNode {
        private SystemTypeNode() : base(null) {
        }
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
        protected override NamedObjectSymbol CreateSymbolCore() {
            throw new NotImplementedException();
        }
    }
    public class TypeNode : NamespaceMemberNode {
        public TypeNode(Node parent) : base(parent) { }
        public NameNode AbstractOrSealed;
        public TypeBodyNode Body;
        public bool IsAbstract {
            get {
                return AbstractOrSealed.Value == Parser.AbstractKeyword;
            }
        }
        public bool IsSealed {
            get {
                return AbstractOrSealed.Value == Parser.SealedKeyword;
            }
        }
        public override void Resolve() {
            Body.Resolve();
        }
        protected override NamedObjectSymbol CreateSymbolCore() {
            return Body.CreateSymbolCore(NamespaceAncestor.LogicalNamespace.NamespaceSymbol, CSName, FullName, IsAbstract, IsSealed);
        }
    }
    public abstract class TypeBodyNode : Node {
        protected TypeBodyNode(Node parent) : base(parent) { }
        public abstract void Resolve();
        public abstract TypeSymbol CreateSymbolCore(NamespaceSymbol parent, string csName, FullName fullName, bool isAbstract, bool isSealed);
    }
    public sealed class TypeListNode : TypeBodyNode {
        public TypeListNode(Node parent) : base(parent) { }
        public QualifiableNameNode ItemQName;
        public TypeNode ItemType;
        public ValueRestrictionsNode ValueRestrictions;//opt
        public override void Resolve() {
            ItemType = NamespaceAncestor.ResolveAsType(ItemQName);
            if (ValueRestrictions != null && ValueRestrictions.ListItemTypeQName.IsValid) {
                ContextEx.ErrorDiag(new DiagMsgEx(DiagCodeEx.ValueRestrictionNotApplicable), ValueRestrictions.ListItemTypeQName.TextSpan);
            }
        }
        public override TypeSymbol CreateSymbolCore(NamespaceSymbol parent, string csName, FullName fullName, bool isAbstract, bool isSealed) {
            var itemSymbol = ItemType.CreateSymbol() as SimpleTypeSymbol;
            if (itemSymbol == null) {
                ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.SimpleTypeRequired), ItemQName.TextSpan);
            }
            if (ValueRestrictions != null) {
                if (ValueRestrictions.ListItemTypeQName.IsValid) {

                }
            }

            return null;// new ListTypeSymbol(parent, csName, isAbstract, isSealed, fullName, itemSymbol);
        }
    }
    public sealed class ChildrenNode : Node {
        public ChildrenNode(Node parent) : base(parent) { }
        public RootComplexChildrenNode ComplexChildren;
        public QualifiableNameNode SimpleChildQName;
        public TypeNode SimpleChild;
        public void Resolve() {
            if (ComplexChildren != null) {
                ComplexChildren.Resolve();
            }
            else {
                SimpleChild = NamespaceAncestor.ResolveAsType(SimpleChildQName);
            }
        }
    }
    public sealed class TypeDirectnessNode : TypeBodyNode {
        public TypeDirectnessNode(Node parent) : base(parent) { }
        public AttributesNode Attributes;
        public ChildrenNode Children;
        public override void Resolve() {
            if (Attributes != null) {
                Attributes.Resolve();
            }
            if (Children != null) {
                Children.Resolve();
            }
        }
        public override TypeSymbol CreateSymbolCore(NamespaceSymbol parent, string csName, FullName fullName, bool isAbstract, bool isSealed) {
            AttributeSetSymbol attributesSymbol = null;
            if (Attributes != null) {
                attributesSymbol = Attributes.CreateSymbol(null, false);
            }
            ObjectSymbol childrenSymbol = null;
            if (Children != null) {
                if (Children.ComplexChildren != null) {
                    childrenSymbol = Children.ComplexChildren.CreateSymbol(null, false);
                }
                else {
                    childrenSymbol = Children.SimpleChild.CreateSymbol();
                }
            }
            return new ComplexTypeSymbol(parent, csName, isAbstract, isSealed, fullName, null, attributesSymbol, childrenSymbol);
        }
    }
    public abstract class TypeDerivation : TypeBodyNode {
        public TypeDerivation(Node parent) : base(parent) { }
        public QualifiableNameNode BaseTypeQName;
        public AttributesNode Attributes;
        public TypeNode BaseType;
        public override void Resolve() {
            BaseType = NamespaceAncestor.ResolveAsType(BaseTypeQName);
            if (Attributes != null) {
                Attributes.Resolve();
            }
        }
    }
    public sealed class TypeExtension : TypeDerivation {
        public TypeExtension(Node parent) : base(parent) { }
        public ChildrenNode Children;
        public override void Resolve() {
            base.Resolve();
            if (Children != null) {
                Children.Resolve();
            }
        }
        public override TypeSymbol CreateSymbolCore(NamespaceSymbol parent, string csName, FullName fullName, bool isAbstract, bool isSealed) {
            var baseTypeSymbol = BaseType.CreateSymbol();
            var baseComplexTypeSymbol = baseTypeSymbol as ComplexTypeSymbol;
            if (baseComplexTypeSymbol == null) {
                ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.BaseTypeIsNotAComplexType, baseTypeSymbol.FullName.ToString()), BaseTypeQName.TextSpan);
            }
            if (baseComplexTypeSymbol.IsSealed) {
                ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.BaseTypeIsSealed, baseComplexTypeSymbol.FullName.ToString()), BaseTypeQName.TextSpan);
            }
            AttributeSetSymbol attributesSymbol = null;
            if (Attributes != null) {
                attributesSymbol = Attributes.CreateSymbol(baseComplexTypeSymbol.Attributes, true);
            }
            ObjectSymbol childrenSymbol = null;
            if (Children != null) {
                if (Children.ComplexChildren != null) {
                    if (baseComplexTypeSymbol.SimpleChild != null) {
                        ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.ThisTypeContainsComplexChildrenButBaseTypeContainsSimpleChild), Children.ComplexChildren.OpenBraceToken);
                    }
                    childrenSymbol = Children.ComplexChildren.CreateSymbol(baseComplexTypeSymbol.ComplexChildren, true);
                }
                else {
                    if (baseComplexTypeSymbol.Children != null) {
                        ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.ExtendingWithSimpleChildRequiresBaseTypeHasNoChildren), Children.SimpleChildQName.TextSpan);
                    }
                    childrenSymbol = Children.SimpleChild.CreateSymbol();
                }
            }
            return new ComplexTypeSymbol(parent, csName, isAbstract, isSealed, fullName, baseComplexTypeSymbol, attributesSymbol, childrenSymbol);
        }
    }
    public sealed class TypeRestriction : TypeDerivation {
        public TypeRestriction(Node parent) : base(parent) { }
        public RootComplexChildrenNode ComplexChildren;
        public ValueRestrictionsNode ValueRestrictions;
        public override void Resolve() {
            base.Resolve();
            if (ComplexChildren != null) {
                ComplexChildren.Resolve();
            }
            else if (ValueRestrictions != null) {
                ValueRestrictions.Resolve();
            }
        }
        private static SimpleTypeSymbol CreateSimpleTypeSymbol(NamespaceSymbol parent, string csName, FullName fullName, bool isAbstract, bool isSealed,
            SimpleTypeSymbol baseTypeSymbol, ValueRestrictionsNode valueRestrictions) {
            if (baseTypeSymbol == null) {

            }

            return null;
        }
        public override TypeSymbol CreateSymbolCore(NamespaceSymbol parent, string csName, FullName fullName, bool isAbstract, bool isSealed) {
            var baseTypeSymbol = BaseType.CreateSymbol();
            if (baseTypeSymbol.IsSealed) {
                ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.BaseTypeIsSealed, baseTypeSymbol.FullName.ToString()), BaseTypeQName.TextSpan);
            }
            var baseSimpleTypeSymbol = baseTypeSymbol as SimpleTypeSymbol;
            if (baseSimpleTypeSymbol != null) {
                if (Attributes != null) {
                    ContextEx.ErrorDiag(new DiagMsgEx(DiagCodeEx.AttributesNotAllowedInSimpleTypeRestriction), Attributes.OpenBracketToken);
                }
                if (ComplexChildren != null) {
                    ContextEx.ErrorDiag(new DiagMsgEx(DiagCodeEx.ComplexChildrenNotAllowedInSimpleTypeRestriction), ComplexChildren.OpenBraceToken);
                }
                ContextEx.ThrowIfHasErrors();
                return CreateSimpleTypeSymbol(parent, csName, fullName, isAbstract, isSealed, baseSimpleTypeSymbol, ValueRestrictions);
            }
            else {
                var baseComplexTypeSymbol = (ComplexTypeSymbol)baseTypeSymbol;
                AttributeSetSymbol attributesSymbol = null;
                if (Attributes != null) {
                    attributesSymbol = Attributes.CreateSymbol(baseComplexTypeSymbol.Attributes, false);
                }
                ObjectSymbol childrenSymbol = null;
                if (ComplexChildren != null) {
                    if (baseComplexTypeSymbol.SimpleChild != null) {
                        ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.ThisTypeContainsComplexChildrenButBaseTypeContainsSimpleChild), ComplexChildren.OpenBraceToken);
                    }
                    childrenSymbol = ComplexChildren.CreateSymbol(baseComplexTypeSymbol.ComplexChildren, false);
                }
                else if (ValueRestrictions != null) {
                    if (baseComplexTypeSymbol.ComplexChildren != null) {
                        ContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.ThisTypeContainsSimpleChildButBaseTypeContainsComplexChildren), ValueRestrictions.OpenBraceToken);
                    }
                    var simpleTypeSymbol = CreateSimpleTypeSymbol(parent, csName + "_SimpleChild", new FullName(fullName.Uri, fullName.Name + "_SimpleChild"),
                        false, isSealed, baseComplexTypeSymbol.SimpleChild, ValueRestrictions);
                    parent.GlobalObjectList.Add(simpleTypeSymbol);
                    childrenSymbol = simpleTypeSymbol;
                }
                return new ComplexTypeSymbol(parent, csName, isAbstract, isSealed, fullName, baseComplexTypeSymbol, attributesSymbol, childrenSymbol);
            }
        }

    }
}
