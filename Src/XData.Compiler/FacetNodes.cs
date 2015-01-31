using System.Collections.Generic;
using XData.IO.Text;

namespace XData.Compiler {
    internal sealed class FacetsNode : Node {
        public FacetsNode(Node parent) : base(parent) { }
        public IntegerRangeNode<ulong> LengthRange;
        public IntegerNode<byte> Precision;
        public IntegerNode<byte> Scale;
        public ValueRangeNode ValueRange;
        public List<SimpleValueNode> Enum;
        public AtomValueNode Pattern;
        public QualifiableNameNode ListItemTypeQName;
        public TypeNode ListItemType;
        public TextSpan OpenBraceTextSpan, CloseBraceTextSpan;
        public void Resolve() {
            if (ListItemTypeQName.IsValid) {
                ListItemType = NamespaceAncestor.ResolveAsType(ListItemTypeQName);
            }
        }

        public void CheckApplicabilities(TypeKind kind) {
            if (LengthRange.IsValid && !Contains(_lengthsTypeKinds, kind)) {
                DiagContextEx.ErrorDiag(new DiagMsgEx(DiagCodeEx.ValueRestrictionNotApplicable), LengthRange.DotDotTextSpan);
            }
            //if (PrecisionAndScale.IsValid) {
            //    if (PrecisionAndScale.MinValue != null && !Contains(_totalDigitsTypeKinds, kind)) {
            //        DiagContextEx.ErrorDiag(new DiagMsgEx(DiagCodeEx.ValueRestrictionNotApplicable), PrecisionAndScale.MinValueNode.TextSpan);
            //    }
            //    if (PrecisionAndScale.MaxValue != null && kind != TypeKind.Decimal) {
            //        DiagContextEx.ErrorDiag(new DiagMsgEx(DiagCodeEx.ValueRestrictionNotApplicable), PrecisionAndScale.MaxValueNode.TextSpan);
            //    }
            //}
            if (ValueRange.IsValid && !Contains(_valuesTypeKinds, kind)) {
                DiagContextEx.ErrorDiag(new DiagMsgEx(DiagCodeEx.ValueRestrictionNotApplicable), ValueRange.DotDotToken);
            }
            if (Enum != null && (!kind.IsConcreteAtomType())) {
                DiagContextEx.ErrorDiag(new DiagMsgEx(DiagCodeEx.ValueRestrictionNotApplicable), Enum[0].TextSpan);
            }
            if (Pattern.IsValid && !kind.IsConcreteAtomType()) {
                DiagContextEx.ErrorDiag(new DiagMsgEx(DiagCodeEx.ValueRestrictionNotApplicable), Pattern.TextSpan);
            }
            if (ListItemTypeQName.IsValid && kind != TypeKind.ListType) {
                DiagContextEx.ErrorDiag(new DiagMsgEx(DiagCodeEx.ValueRestrictionNotApplicable), ListItemTypeQName.TextSpan);
            }
            DiagContextEx.ThrowIfHasErrors();
        }
        private static readonly TypeKind[] _lengthsTypeKinds = new TypeKind[] { TypeKind.ListType, TypeKind.String, TypeKind.IgnoreCaseString, TypeKind.Binary };
        private static readonly TypeKind[] _totalDigitsTypeKinds = new TypeKind[] { TypeKind.Decimal, TypeKind.Int64, TypeKind.Int32, TypeKind.Int16, TypeKind.SByte, TypeKind.UInt64, TypeKind.UInt32, TypeKind.UInt16, TypeKind.Byte };
        //private static readonly TypeKind[] _fractionDigitsTypeKinds = new TypeKind[] { TypeKind.Decimal };
        private static readonly TypeKind[] _valuesTypeKinds = new TypeKind[] { TypeKind.String, TypeKind.IgnoreCaseString, TypeKind.Decimal, TypeKind.Int64, TypeKind.Int32, TypeKind.Int16, TypeKind.SByte, TypeKind.UInt64, TypeKind.UInt32, TypeKind.UInt16, TypeKind.Byte, TypeKind.Double, TypeKind.Single, TypeKind.Guid, TypeKind.TimeSpan, TypeKind.DateTimeOffset };
        private static bool Contains(TypeKind[] array, TypeKind kind) {
            foreach (var item in array) {
                if (item == kind) {
                    return true;
                }
            }
            return false;
        }
    }

    internal struct IntegerRangeNode<T> where T : struct {
        public IntegerRangeNode(IntegerNode<T> minValue, IntegerNode<T> maxValue, TextSpan dotDotTextSpan) {
            MinValue = minValue;
            MaxValue = maxValue;
            DotDotTextSpan = dotDotTextSpan;
        }
        public readonly IntegerNode<T> MinValue;
        public readonly IntegerNode<T> MaxValue;
        public readonly TextSpan DotDotTextSpan;
        public bool IsValid {
            get {
                return DotDotTextSpan.IsValid;
            }
        }
    }

    internal struct IntegerNode<T> where T : struct {
        public IntegerNode(AtomValueNode node, T value) {
            Node = node;
            Value = value;
        }
        public readonly AtomValueNode Node;
        public readonly T Value;
        public bool IsValid {
            get {
                return Node.IsValid;
            }
        }
        public TextSpan TextSpan {
            get {
                return Node.TextSpan;
            }
        }
    }

    internal struct ValueRangeNode {
        public ValueRangeNode(ValueBoundaryNode? minValue, ValueBoundaryNode? maxValue, TextSpan dotDotToken) {
            MinValue = minValue;
            MaxValue = maxValue;
            DotDotToken = dotDotToken;
        }
        public readonly ValueBoundaryNode? MinValue;
        public readonly ValueBoundaryNode? MaxValue;
        public readonly TextSpan DotDotToken;
        public bool IsValid {
            get {
                return DotDotToken.IsValid;
            }
        }

    }
    internal struct ValueBoundaryNode {
        public ValueBoundaryNode(AtomValueNode value, bool isInclusive) {
            Value = value;
            IsInclusive = isInclusive;
        }
        public readonly AtomValueNode Value;
        public readonly bool IsInclusive;
        public bool IsValid {
            get {
                return Value.IsValid;
            }
        }
    }
}
