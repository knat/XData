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
        public FacetSetSymbol CreateSymbol(TypeKind typeKind, FacetSetSymbol baseFacetSet) {
            if (LengthRange.IsValid && !System.Linq.Enumerable.Contains(_lengthRangeTypeKinds, typeKind)) {
                DiagContextEx.ErrorDiag(new DiagMsgEx(DiagCodeEx.FacetNotApplicable), LengthRange.TextSpan);
            }
            if (typeKind != TypeKind.Decimal) {
                if (Precision.IsValid) {
                    DiagContextEx.ErrorDiag(new DiagMsgEx(DiagCodeEx.FacetNotApplicable), Precision.TextSpan);
                }
                if (Scale.IsValid) {
                    DiagContextEx.ErrorDiag(new DiagMsgEx(DiagCodeEx.FacetNotApplicable), Scale.TextSpan);
                }
            }
            if (ValueRange.IsValid && !System.Linq.Enumerable.Contains(_valueRangeTypeKinds, typeKind)) {
                DiagContextEx.ErrorDiag(new DiagMsgEx(DiagCodeEx.FacetNotApplicable), ValueRange.TextSpan);
            }
            if (Enum != null && !typeKind.IsConcreteAtomType()) {
                DiagContextEx.ErrorDiag(new DiagMsgEx(DiagCodeEx.FacetNotApplicable), Enum[0].TextSpan);
            }
            if (Pattern.IsValid && !typeKind.IsConcreteAtomType()) {
                DiagContextEx.ErrorDiag(new DiagMsgEx(DiagCodeEx.FacetNotApplicable), Pattern.TextSpan);
            }
            if (typeKind != TypeKind.ListType && ListItemTypeQName.IsValid) {
                DiagContextEx.ErrorDiag(new DiagMsgEx(DiagCodeEx.FacetNotApplicable), ListItemTypeQName.TextSpan);
            }
            DiagContextEx.ThrowIfHasErrors();
            ulong? minLength = baseFacetSet != null ? baseFacetSet.MinLength : null;
            ulong? maxLength = baseFacetSet != null ? baseFacetSet.MaxLength : null;
            byte? precision = baseFacetSet != null ? baseFacetSet.Precision : null;
            byte? scale = baseFacetSet != null ? baseFacetSet.Scale : null;
            ValueBoundaryInfo? minValue = baseFacetSet != null ? baseFacetSet.MinValue : null;
            ValueBoundaryInfo? maxValue = baseFacetSet != null ? baseFacetSet.MaxValue : null;
            EnumInfo? @enum = baseFacetSet != null ? baseFacetSet.Enum : null;
            PatternInfo[] patterns = baseFacetSet != null ? baseFacetSet.Patterns : null;
            if (LengthRange.IsValid) {
                if (LengthRange.MinValue.IsValid) {
                    if (LengthRange.MinValue.Value < minLength) {
                        DiagContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.MinLengthNotEqualToOrGreaterThanBaseMinLength,
                            LengthRange.MinValue.Value.ToInvString(), minLength.Value.ToInvString()), LengthRange.MinValue.TextSpan);
                    }
                    else {
                        minLength = LengthRange.MinValue.Value;
                    }
                }
                if (LengthRange.MaxValue.IsValid) {
                    if (LengthRange.MaxValue.Value > maxLength) {
                        DiagContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.MaxLengthNotEqualToOrLessThanBaseMaxLength,
                            LengthRange.MaxValue.Value.ToInvString(), maxLength.Value.ToInvString()), LengthRange.MaxValue.TextSpan);
                    }
                    else {
                        maxLength = LengthRange.MaxValue.Value;
                    }
                }
                if (minLength > maxLength) {
                    DiagContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.MaxLengthNotEqualToOrGreaterThanMinLength,
                        maxLength.Value.ToInvString(), minLength.Value.ToInvString()),
                        LengthRange.MaxValue.IsValid ? LengthRange.MaxValue.TextSpan : LengthRange.MinValue.TextSpan);
                }
            }
            if (Precision.IsValid || Scale.IsValid) {
                if (Precision.IsValid) {
                    if (Precision.Value > precision) {
                        DiagContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.PrecisionNotEqualToOrLessThanBasePrecision,
                            Precision.Value.ToInvString(), precision.Value.ToInvString()), Precision.TextSpan);
                    }
                    else {
                        precision = Precision.Value;
                    }
                }
                if (Scale.IsValid) {
                    if (Scale.Value > scale) {
                        DiagContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.ScaleNotEqualToOrLessThanBaseScale,
                            Scale.Value.ToInvString(), scale.Value.ToInvString()), Scale.TextSpan);
                    }
                    else {
                        scale = Scale.Value;
                    }
                }
                if (scale > precision) {
                    DiagContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.ScaleNotEqualToOrLessThanPrecision,
                        scale.Value.ToInvString(), precision.Value.ToInvString()),
                        Scale.IsValid ? Scale.TextSpan : Precision.TextSpan);
                }
            }
            if (Pattern.IsValid) {
                var patternValue = Pattern.Value;
                if (patterns == null) {
                    patterns = new PatternInfo[] { new PatternInfo(patternValue) };
                }
                else {
                    var found = false;
                    foreach (var pi in patterns) {
                        if (pi.Pattern == patternValue) {
                            found = true;
                            break;
                        }
                    }
                    if (!found) {
                        patterns = patterns.Add(new PatternInfo(patternValue));
                    }
                }
            }
            if (ValueRange.IsValid || Enum != null) {
                var typeObj = CreateXAtomType(typeKind);
                if (Enum != null) {
                    foreach (var item in Enum) {
                        var literal = item.Value.Value;
                        if (!typeObj.TryParseAndSet(literal)) {
                            DiagContextEx.ErrorDiagAndThrow(new DiagMsgEx(DiagCodeEx.InvalidLiteral, literal), item.TextSpan);
                        }
                        var valueObj = typeObj.GetValue();
                        var name = item.Name.Value;
                        if (@enum != null) {
                            var found = false;
                            foreach (var baseItem in @enum.Value.Items) {
                                if (typeObj.ValueEquals(baseItem.Value)) {
                                    found = true;
                                    break;
                                }
                            }
                            if (!found) {

                            }
                        }
                    }
                }


            }



            return null;
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
