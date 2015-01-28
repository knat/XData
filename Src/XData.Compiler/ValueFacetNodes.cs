using System.Collections.Generic;
using XData.IO.Text;

namespace XData.Compiler {
    public sealed class ValueFacetsNode : Node {
        public ValueFacetsNode(Node parent) : base(parent) { }
        public IntegerRangeNode<ulong> Lengths;
        public IntegerRangeNode<byte> Digits;
        public ValueRangeNode Values;
        public List<SimpleValueNode> Enums;
        public AtomValueNode Pattern;
        public QualifiableNameNode ListItemTypeQName;
        public TypeNode ListItemType;
        public TextSpan OpenBraceToken, CloseBraceToken;
        //public bool HasValues {
        //    get {
        //        return Lengths.IsValid || Digits.IsValid || Values.IsValid || Enums != null || Pattern.IsValid || ListItemTypeQName.IsValid;
        //    }
        //}
        public void Resolve() {
            if (ListItemTypeQName.IsValid) {
                ListItemType = NamespaceAncestor.ResolveAsType(ListItemTypeQName);
            }
        }

        public void CheckApplicabilities(TypeKind kind) {
            if (Lengths.IsValid && !Contains(_lengthsTypeKinds, kind)) {
                DiagContextEx.ErrorDiag(new DiagMsgEx(DiagCodeEx.ValueRestrictionNotApplicable), Lengths.DotDotToken);
            }
            if (Digits.IsValid) {
                if (Digits.MinValue != null && !Contains(_totalDigitsTypeKinds, kind)) {
                    DiagContextEx.ErrorDiag(new DiagMsgEx(DiagCodeEx.ValueRestrictionNotApplicable), Digits.MinValueNode.TextSpan);
                }
                if (Digits.MaxValue != null && kind != TypeKind.Decimal) {
                    DiagContextEx.ErrorDiag(new DiagMsgEx(DiagCodeEx.ValueRestrictionNotApplicable), Digits.MaxValueNode.TextSpan);
                }
            }
            if (Values.IsValid && !Contains(_valuesTypeKinds, kind)) {
                DiagContextEx.ErrorDiag(new DiagMsgEx(DiagCodeEx.ValueRestrictionNotApplicable), Values.DotDotToken);
            }
            if (Enums != null && (!kind.IsConcreteAtomType())) {
                DiagContextEx.ErrorDiag(new DiagMsgEx(DiagCodeEx.ValueRestrictionNotApplicable), Enums[0].TextSpan);
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

    public struct IntegerRangeNode<T> where T : struct {
        public IntegerRangeNode(AtomValueNode minValueNode, T? minValue, AtomValueNode maxValueNode, T? maxValue, TextSpan dotDotToken) {
            MinValueNode = minValueNode;
            MinValue = minValue;
            MaxValueNode = maxValueNode;
            MaxValue = maxValue;
            DotDotToken = dotDotToken;
        }
        public readonly AtomValueNode MinValueNode;
        public readonly T? MinValue;
        public readonly AtomValueNode MaxValueNode;
        public readonly T? MaxValue;
        public readonly TextSpan DotDotToken;
        public bool IsValid {
            get {
                return DotDotToken.IsValid;
            }
        }
    }
    public struct ValueRangeNode {
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
    public struct ValueBoundaryNode {
        public ValueBoundaryNode(SimpleValueNode value, bool isInclusive) {
            Value = value;
            IsInclusive = isInclusive;
        }
        public readonly SimpleValueNode Value;
        public readonly bool IsInclusive;
        public bool IsValid {
            get {
                return Value.IsValid;
            }
        }
    }
}
