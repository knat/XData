using System;
using System.Collections.Generic;
using XData.IO.Text;

namespace XData.Compiler {
    internal sealed class FacetsNode : Node {
        public FacetsNode(Node parent) : base(parent) { }
        public IntegerRangeNode<ulong> LengthRange;
        public IntegerNode<byte> Precision;
        public IntegerNode<byte> Scale;
        public ValueRangeNode ValueRange;
        public List<EnumItemNode> Enum;
        public AtomValueNode Pattern;
        public QualifiableNameNode ListItemTypeQName;
        public TypeNode ListItemType;
        public TextSpan OpenBraceTextSpan, CloseBraceTextSpan;
        public void Resolve() {
            if (ListItemTypeQName.IsValid) {
                ListItemType = NamespaceAncestor.ResolveAsType(ListItemTypeQName);
            }
        }
        public FacetSetSymbol CreateSymbol(SimpleTypeSymbol parent, TypeKind typeKind, FacetSetSymbol baseFacetSet) {
            if (LengthRange.IsValid && !System.Linq.Enumerable.Contains(_lengthRangeTypeKinds, typeKind)) {
                DiagContextEx.ErrorDiag(new DiagMsgEx(DiagCodeEx.FacetNotAllowed), LengthRange.TextSpan);
            }
            if (typeKind != TypeKind.Decimal) {
                if (Precision.IsValid) {
                    DiagContextEx.ErrorDiag(new DiagMsgEx(DiagCodeEx.FacetNotAllowed), Precision.TextSpan);
                }
                if (Scale.IsValid) {
                    DiagContextEx.ErrorDiag(new DiagMsgEx(DiagCodeEx.FacetNotAllowed), Scale.TextSpan);
                }
            }
            if (ValueRange.IsValid && !System.Linq.Enumerable.Contains(_valueRangeTypeKinds, typeKind)) {
                DiagContextEx.ErrorDiag(new DiagMsgEx(DiagCodeEx.FacetNotAllowed), ValueRange.TextSpan);
            }
            if (Enum != null && !typeKind.IsConcreteAtomType()) {
                DiagContextEx.ErrorDiag(new DiagMsgEx(DiagCodeEx.FacetNotAllowed), Enum[0].TextSpan);
            }
            if (Pattern.IsValid && !typeKind.IsConcreteAtomType()) {
                DiagContextEx.ErrorDiag(new DiagMsgEx(DiagCodeEx.FacetNotAllowed), Pattern.TextSpan);
            }
            if (typeKind != TypeKind.ListType && ListItemTypeQName.IsValid) {
                DiagContextEx.ErrorDiag(new DiagMsgEx(DiagCodeEx.FacetNotAllowed), ListItemTypeQName.TextSpan);
            }
            DiagContextEx.ThrowIfHasErrors();
            ulong? minLength = baseFacetSet != null ? baseFacetSet.MinLength : null;
            ulong? maxLength = baseFacetSet != null ? baseFacetSet.MaxLength : null;
            byte? precision = baseFacetSet != null ? baseFacetSet.Precision : null;
            byte? scale = baseFacetSet != null ? baseFacetSet.Scale : null;
            ValueBoundaryInfo? minValue = baseFacetSet != null ? baseFacetSet.MinValue : null;
            ValueBoundaryInfo? maxValue = baseFacetSet != null ? baseFacetSet.MaxValue : null;
            EnumInfoEx? @enum = baseFacetSet != null ? baseFacetSet.Enum : null;
            List<string> patternList = baseFacetSet != null ? baseFacetSet.PatternList : null;
            if (LengthRange.IsValid) {
                if (LengthRange.MinValue.IsValid) {
                    if (LengthRange.MinValue.Value < minLength) {
                        DiagContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.MinLengthNotGreaterThanOrEqualToBaseMinLength,
                            LengthRange.MinValue.Value.ToInvString(), minLength.Value.ToInvString()), LengthRange.MinValue.TextSpan);
                    }
                    else {
                        minLength = LengthRange.MinValue.Value;
                    }
                }
                if (LengthRange.MaxValue.IsValid) {
                    if (LengthRange.MaxValue.Value > maxLength) {
                        DiagContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.MaxLengthNotLessThanOrEqualToBaseMaxLength,
                            LengthRange.MaxValue.Value.ToInvString(), maxLength.Value.ToInvString()), LengthRange.MaxValue.TextSpan);
                    }
                    else {
                        maxLength = LengthRange.MaxValue.Value;
                    }
                }
                if (minLength > maxLength) {
                    DiagContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.MaxLengthNotGreaterThanOrEqualToMinLength,
                        maxLength.Value.ToInvString(), minLength.Value.ToInvString()),
                        LengthRange.MaxValue.IsValid ? LengthRange.MaxValue.TextSpan : LengthRange.MinValue.TextSpan);
                }
            }
            if (Precision.IsValid || Scale.IsValid) {
                if (Precision.IsValid) {
                    if (Precision.Value > precision) {
                        DiagContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.PrecisionNotLessThanOrEqualToBasePrecision,
                            Precision.Value.ToInvString(), precision.Value.ToInvString()), Precision.TextSpan);
                    }
                    else {
                        precision = Precision.Value;
                    }
                }
                if (Scale.IsValid) {
                    if (Scale.Value > scale) {
                        DiagContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.ScaleNotLessThanOrEqualToBaseScale,
                            Scale.Value.ToInvString(), scale.Value.ToInvString()), Scale.TextSpan);
                    }
                    else {
                        scale = Scale.Value;
                    }
                }
                if (scale > precision) {
                    DiagContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.ScaleNotLessThanOrEqualToPrecision,
                        scale.Value.ToInvString(), precision.Value.ToInvString()),
                        Scale.IsValid ? Scale.TextSpan : Precision.TextSpan);
                }
            }
            if (Pattern.IsValid) {
                var patternValue = Pattern.Value;
                if (patternList == null) {
                    patternList = new List<string> { patternValue };
                }
                else {
                    if (!patternList.Contains(patternValue)) {
                        patternList.Add(patternValue);
                    }
                }
            }
            if (Enum != null) {
                var typeObj = CreateXAtomType(typeKind);
                var itemList = new List<EnumItemInfo>();
                var textsb = EX.AcquireStringBuilder();
                for (var i = 0; i < Enum.Count; ++i) {
                    var item = Enum[i];
                    if (item.Name.IsValid && @enum != null) {
                        DiagContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.EnumItemNameNotAllowedInRestriction), item.Name.TextSpan);
                    }
                    var literal = item.Value.Value;
                    if (!typeObj.TryParseAndSet(literal)) {
                        DiagContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.InvalidLiteral, literal), item.TextSpan);
                    }
                    if (@enum != null) {
                        var found = false;
                        foreach (var baseItem in @enum.Value.ItemList) {
                            if (typeObj.ValueEquals(baseItem.Value)) {
                                found = true;
                                break;
                            }
                        }
                        if (!found) {
                            DiagContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.EnumItemNotInBaseEnum, literal), item.TextSpan);
                        }
                    }
                    itemList.Add(new EnumItemInfo(typeObj.GetValue(), item.Name.Value));
                    if (i > 0) {
                        textsb.Append(", ");
                    }
                    textsb.Append(literal);
                }
                @enum = new EnumInfoEx(itemList, textsb.ToStringAndRelease());
            }
            if (ValueRange.IsValid) {
                XAtomType minTypeObj = null, maxTypeObj = null;
                if (ValueRange.MinValue.IsValid) {
                    minTypeObj = CreateXAtomType(typeKind);
                    var literal = ValueRange.MinValue.Value.Value;
                    if (!minTypeObj.TryParseAndSet(literal)) {
                        DiagContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.InvalidLiteral, literal), ValueRange.MinValue.TextSpan);
                    }
                    if (minValue != null) {
                        var minValueV = minValue.Value;
                        int result;
                        if (!minTypeObj.TryCompareValueTo(minValueV.Value, out result)) {
                            throw new InvalidOperationException("!TryCompareValueTo()");
                        }
                        if (minValueV.IsInclusive) {
                            if (result < 0) {
                                DiagContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.MinValueNotGreaterThanOrEqualToBaseMinValue,
                                    literal, minValueV.Text), ValueRange.MinValue.TextSpan);
                            }
                        }
                        else if (result <= 0) {
                            DiagContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.MinValueNotGreaterThanBaseMinValue,
                                literal, minValueV.Text), ValueRange.MinValue.TextSpan);
                        }
                    }
                    minValue = new ValueBoundaryInfo(minTypeObj.GetValue(), literal, ValueRange.MinValue.IsInclusive);
                }
                if (ValueRange.MaxValue.IsValid) {
                    maxTypeObj = CreateXAtomType(typeKind);
                    var literal = ValueRange.MaxValue.Value.Value;
                    if (!maxTypeObj.TryParseAndSet(literal)) {
                        DiagContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.InvalidLiteral, literal), ValueRange.MaxValue.TextSpan);
                    }
                    if (maxValue != null) {
                        var maxValueV = maxValue.Value;
                        int result;
                        if (!maxTypeObj.TryCompareValueTo(maxValueV.Value, out result)) {
                            throw new InvalidOperationException("!TryCompareValueTo()");
                        }
                        if (maxValueV.IsInclusive) {
                            if (result > 0) {
                                DiagContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.MaxValueNotLessThanOrEqualToBaseMaxValue,
                                    literal, maxValueV.Text), ValueRange.MaxValue.TextSpan);
                            }
                        }
                        else if (result >= 0) {
                            DiagContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.MaxValueNotLessThanBaseMaxValue,
                                literal, maxValueV.Text), ValueRange.MaxValue.TextSpan);
                        }
                    }
                    maxValue = new ValueBoundaryInfo(maxTypeObj.GetValue(), literal, ValueRange.MaxValue.IsInclusive);
                }
                if (minValue != null && maxValue != null) {
                    var minValueV = minValue.Value;
                    var maxValueV = maxValue.Value;
                    int result;
                    if (minTypeObj != null) {
                        if (!minTypeObj.TryCompareValueTo(maxValueV.Value, out result)) {
                            throw new InvalidOperationException("!TryCompareValueTo()");
                        }
                        if (result > 0) {
                            DiagContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.MaxValueNotGreaterThanOrEqualToMinValue,
                                maxValueV.Text, minValueV.Text), ValueRange.MaxValue.IsValid ? ValueRange.MaxValue.TextSpan : ValueRange.MinValue.TextSpan);
                        }
                    }
                    else {
                        if (!maxTypeObj.TryCompareValueTo(minValueV.Value, out result)) {
                            throw new InvalidOperationException("!TryCompareValueTo()");
                        }
                        if (result < 0) {
                            DiagContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.MaxValueNotGreaterThanOrEqualToMinValue,
                                maxValueV.Text, minValueV.Text), ValueRange.MaxValue.IsValid ? ValueRange.MaxValue.TextSpan : ValueRange.MinValue.TextSpan);
                        }
                    }
                    if (result == 0) {
                        if (!minValueV.IsInclusive || !maxValueV.IsInclusive) {
                            DiagContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.MaxValueNotGreaterThanMinValue,
                                maxValueV.Text, minValueV.Text), ValueRange.MaxValue.IsValid ? ValueRange.MaxValue.TextSpan : ValueRange.MinValue.TextSpan);
                        }
                    }
                }
            }
            //
            return new FacetSetSymbol(parent, baseFacetSet, minLength, maxLength, precision, scale, minValue, maxValue, @enum, patternList);
        }
        private XAtomType CreateXAtomType(TypeKind typeKind) {
            switch (typeKind) {
                case TypeKind.String: return new XString();
                case TypeKind.IgnoreCaseString: return new XIgnoreCaseString();
                case TypeKind.Decimal: return new XDecimal();
                case TypeKind.Int64: return new XInt64();
                case TypeKind.Int32: return new XInt32();
                case TypeKind.Int16: return new XInt16();
                case TypeKind.SByte: return new XSByte();
                case TypeKind.UInt64: return new XUInt64();
                case TypeKind.UInt32: return new XUInt32();
                case TypeKind.UInt16: return new XUInt16();
                case TypeKind.Byte: return new XByte();
                case TypeKind.Double: return new XDouble();
                case TypeKind.Single: return new XSingle();
                case TypeKind.Boolean: return new XBoolean();
                case TypeKind.Binary: return new XBinary();
                case TypeKind.Guid: return new XGuid();
                case TypeKind.TimeSpan: return new XTimeSpan();
                case TypeKind.DateTimeOffset: return new XDateTimeOffset();

                default: throw new InvalidOperationException("Invalid type kind " + typeKind.ToString());
            }
        }
        //private sealed class XFakeListType : XSimpleType {
        //}

        private static readonly TypeKind[] _lengthRangeTypeKinds = new TypeKind[] { TypeKind.ListType, TypeKind.String, TypeKind.IgnoreCaseString, TypeKind.Binary };
        private static readonly TypeKind[] _valueRangeTypeKinds = new TypeKind[] { TypeKind.String, TypeKind.IgnoreCaseString, TypeKind.Decimal,
            TypeKind.Int64, TypeKind.Int32, TypeKind.Int16, TypeKind.SByte, TypeKind.UInt64, TypeKind.UInt32, TypeKind.UInt16, TypeKind.Byte,
            TypeKind.Double, TypeKind.Single, /*TypeKind.Guid,*/ TypeKind.TimeSpan, TypeKind.DateTimeOffset };
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
        public TextSpan TextSpan {
            get {
                return DotDotTextSpan;
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
        public ValueRangeNode(ValueBoundaryNode minValue, ValueBoundaryNode maxValue, TextSpan dotDotTextSpan) {
            MinValue = minValue;
            MaxValue = maxValue;
            DotDotTextSpan = dotDotTextSpan;
        }
        public readonly ValueBoundaryNode MinValue;
        public readonly ValueBoundaryNode MaxValue;
        public readonly TextSpan DotDotTextSpan;
        public bool IsValid {
            get {
                return DotDotTextSpan.IsValid;
            }
        }
        public TextSpan TextSpan {
            get {
                return DotDotTextSpan;
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
        public TextSpan TextSpan {
            get {
                return Value.TextSpan;
            }
        }
    }
    internal struct EnumItemNode {
        public EnumItemNode(AtomValueNode value, NameNode name) {
            Value = value;
            Name = name;
        }
        public readonly AtomValueNode Value;
        public readonly NameNode Name;//opt
        public bool IsValid {
            get {
                return Value.IsValid;
            }
        }
        public TextSpan TextSpan {
            get {
                return Value.TextSpan;
            }
        }
    }

}
