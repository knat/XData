using System;
using XData.IO.Text;

namespace XData {
    public abstract class XSimpleType : XType, IEquatable<XSimpleType> {
        public abstract bool Equals(XSimpleType other);
        public abstract bool ValueEquals(object other);
        public override sealed bool Equals(object obj) {
            return Equals(obj as XSimpleType);
        }
        public override int GetHashCode() {
            throw new NotImplementedException();
        }
        public static bool operator ==(XSimpleType left, XSimpleType right) {
            if ((object)left == null) {
                return (object)right == null;
            }
            return left.Equals(right);
        }
        public static bool operator !=(XSimpleType left, XSimpleType right) {
            return !(left == right);
        }
        public SimpleTypeInfo SimpleTypeInfo {
            get {
                return (SimpleTypeInfo)ObjectInfo;
            }
        }
        public static readonly SimpleTypeInfo ThisInfo = new SimpleTypeInfo(typeof(XSimpleType), true, TypeKind.SimpleType.ToFullName(), null, null);
        //
        internal override bool TryValidateCore(DiagContext context) {
            return TryValidateFacets(context);
        }
        internal bool TryValidateFacets(DiagContext context) {
            var facets = SimpleTypeInfo.Facets;
            if (facets != null) {
                var dMarker = context.MarkDiags();
                //
                var minLength = facets.MinLength;
                var maxLength = facets.MaxLength;
                if (minLength != null || maxLength != null) {
                    ulong length;
                    if (!TryGetValueLength(out length)) {
                        throw new InvalidOperationException("!TryGetValueLength()");
                    }
                    if (minLength == maxLength) {
                        if (length != minLength) {
                            context.AddErrorDiag(new DiagMsg(DiagCode.LengthNotEqualTo,
                                length.ToInvString(), minLength.Value.ToInvString()), this);
                        }
                    }
                    else if (length < minLength) {
                        context.AddErrorDiag(new DiagMsg(DiagCode.LengthNotGreaterThanOrEqualTo,
                            length.ToInvString(), minLength.Value.ToInvString()), this);
                    }
                    else if (length > maxLength) {
                        context.AddErrorDiag(new DiagMsg(DiagCode.LengthNotLessThanOrEqualTo,
                            length.ToInvString(), maxLength.Value.ToInvString()), this);
                    }
                }
                TryValidateFacetsEx(context, facets);
                return !dMarker.HasErrors;
            }
            return true;
        }
        internal virtual bool TryGetValueLength(out ulong result) {
            result = 0;
            return false;
        }
        internal virtual void TryValidateFacetsEx(DiagContext context, FacetSetInfo facets) { }
        internal override void SaveValue(SavingContext context) {
            context.Append(ToString());
        }
        internal static bool TryCreate(DiagContext context, ProgramInfo programInfo, SimpleTypeInfo simpleTypeInfo,
            SimpleValueNode simpleValueNode, out XSimpleType result) {
            result = null;
            var simpleValueTextSpan = simpleValueNode.TextSpan;
            var effSimpleTypeInfo = (SimpleTypeInfo)GetEffectiveTypeInfo(context, programInfo, simpleValueNode.TypeQName,
                simpleTypeInfo, simpleValueTextSpan);
            if (effSimpleTypeInfo == null) {
                return false;
            }
            var atomTypeInfo = effSimpleTypeInfo as AtomTypeInfo;
            if (atomTypeInfo != null) {
                var atomicValueNode = simpleValueNode.Atom;
                if (!atomicValueNode.IsValid) {
                    context.AddErrorDiag(new DiagMsg(DiagCode.TypeRequiresAtomValue, effSimpleTypeInfo.DisplayName), simpleValueTextSpan);
                    return false;
                }
                XAtomType atomicType;
                if (!XAtomType.TryCreate(context, atomTypeInfo, atomicValueNode, out atomicType)) {
                    return false;
                }
                result = atomicType;
            }
            else {
                var listNode = simpleValueNode.List;
                if (listNode == null) {
                    context.AddErrorDiag(new DiagMsg(DiagCode.TypeRequiresListValue, effSimpleTypeInfo.DisplayName), simpleValueTextSpan);
                    return false;
                }
                XListType listType;
                if (!XListType.TryCreate(context, programInfo, (ListTypeInfo)effSimpleTypeInfo, listNode, out listType)) {
                    return false;
                }
                result = listType;
            }
            return true;
        }


    }
}
