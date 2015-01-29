using System;
using XData.IO.Text;

namespace XData {
    public interface ITryComparable<in T> {
        bool TryCompareTo(T other, out int result);
    }

    public abstract class XAtomType : XSimpleType, ITryComparable<XAtomType> {
        public abstract bool TryParseAndSet(string literal);
        public virtual bool TryCompareTo(XAtomType other, out int result) {
            result = 0;
            return false;
        }
        public virtual bool TryCompareValueTo(object other, out int result) {
            result = 0;
            return false;
        }
        public static bool operator <(XAtomType x, XAtomType y) {
            if ((object)x == null) return false;
            int result;
            if (x.TryCompareTo(y, out result) && result < 0) {
                return true;
            }
            return false;
        }
        public static bool operator <=(XAtomType x, XAtomType y) {
            if ((object)x == null) {
                return (object)y == null;
            }
            int result;
            if (x.TryCompareTo(y, out result) && result <= 0) {
                return true;
            }
            return false;
        }
        public static bool operator >(XAtomType x, XAtomType y) {
            if ((object)x == null) return false;
            int result;
            if (x.TryCompareTo(y, out result) && result > 0) {
                return true;
            }
            return false;
        }
        public static bool operator >=(XAtomType x, XAtomType y) {
            if ((object)x == null) {
                return (object)y == null;
            }
            int result;
            if (x.TryCompareTo(y, out result) && result >= 0) {
                return true;
            }
            return false;
        }
        public AtomTypeInfo AtomTypeInfo {
            get {
                return (AtomTypeInfo)ObjectInfo;
            }
        }
        new public static readonly AtomTypeInfo ThisInfo = new AtomTypeInfo(typeof(XAtomType), true, TypeKind.AtomType.ToFullName(),
            XSimpleType.ThisInfo, null, TypeKind.AtomType);
        //
        internal virtual bool TryGetValuePrecisionAndScale(out byte precision, out byte scale) {
            precision = 0;
            scale = 0;
            return false;
        }
        internal override sealed void TryValidateFacetsEx(DiagContext context, FacetSetInfo facets) {
            var precision = facets.Precision;
            var scale = facets.Scale;
            if (precision != null || scale != null) {
                byte valuePrecision, valueScale;
                if (!TryGetValuePrecisionAndScale(out valuePrecision, out valueScale)) {
                    throw new InvalidOperationException("!TryGetValuePrecisionAndScale()");
                }
                if (precision != null) {
                    if (valuePrecision > precision) {
                        context.AddErrorDiag(new DiagMsg(DiagCode.PrecisionNotLessThanOrEqualTo,
                            valuePrecision.ToInvString(), precision.Value.ToInvString()), this);
                    }
                }
                if (scale != null) {
                    if (valueScale > scale) {
                        context.AddErrorDiag(new DiagMsg(DiagCode.ScaleNotLessThanOrEqualTo,
                            valueScale.ToInvString(), scale.Value.ToInvString()), this);
                    }
                }
            }
            string str = null;
            if (facets.MinValue != null) {
                var minValue = facets.MinValue.Value;
                int result;
                if (!TryCompareValueTo(minValue.ValueText.Value, out result)) {
                    throw new InvalidOperationException("!TryCompareValueTo()");
                }
                if (minValue.IsInclusive) {
                    if (result < 0) {
                        context.AddErrorDiag(new DiagMsg(DiagCode.ValueNotGreaterThanOrEqualTo,
                            GetString(ref str), minValue.ValueText.Text), this);
                    }
                }
                else if (result <= 0) {
                    context.AddErrorDiag(new DiagMsg(DiagCode.ValueNotGreaterThan,
                        GetString(ref str), minValue.ValueText.Text), this);
                }
            }
            if (facets.MaxValue != null) {
                var maxValue = facets.MaxValue.Value;
                int result;
                if (!TryCompareValueTo(maxValue.ValueText.Value, out result)) {
                    throw new InvalidOperationException("!TryCompareValueTo()");
                }
                if (maxValue.IsInclusive) {
                    if (result > 0) {
                        context.AddErrorDiag(new DiagMsg(DiagCode.ValueNotLessThanOrEqualTo,
                            GetString(ref str), maxValue.ValueText.Text), this);
                    }
                }
                else if (result >= 0) {
                    context.AddErrorDiag(new DiagMsg(DiagCode.ValueNotLessThan,
                        GetString(ref str), maxValue.ValueText.Text), this);
                }
            }
            if (facets.Enum != null) {
                var @enum = facets.Enum.Value;
                var found = false;
                foreach (var item in @enum.Items) {
                    if (ValueEquals(item.Value)) {
                        found = true;
                        break;
                    }
                }
                if (!found) {
                    context.AddErrorDiag(new DiagMsg(DiagCode.ValueNotInEnumeration,
                        GetString(ref str), @enum.TotalText), this);
                }
            }
            if (facets.Patterns != null) {
                GetString(ref str);
                foreach (var pattern in facets.Patterns) {
                    var match = pattern.Regex.Match(str);
                    if (!(match.Success && match.Index == 0 && match.Length == str.Length)) {
                        context.AddErrorDiag(new DiagMsg(DiagCode.LiteralNotMatchWithPattern, str, pattern.Pattern), this);
                    }
                }
            }
        }
        private string GetString(ref string str) {
            if (str == null) {
                str = ToString();
            }
            return str;
        }
        internal static bool TryCreate(DiagContext context, AtomTypeInfo atomTypeInfo,
            AtomValueNode atomValueNode, out XAtomType result) {
            result = null;
            var atomType = atomTypeInfo.CreateInstance<XAtomType>();
            atomType.TextSpan = atomValueNode.TextSpan;
            var literal = atomValueNode.Value;
            if (!atomType.TryParseAndSet(literal)) {
                context.AddErrorDiag(new DiagMsg(DiagCode.InvalidAtomTypeLiteral, atomTypeInfo.Kind.ToString(), literal), atomValueNode.TextSpan);
                return false;
            }
            if (!atomType.TryValidateFacets(context)) {
                return false;
            }
            result = atomType;
            return true;
        }
    }

}
