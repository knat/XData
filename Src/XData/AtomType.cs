using System;
using XData.IO.Text;

namespace XData {
    public interface ITryComparable<in T> {
        bool TryCompareTo(T other, out int result);
    }

    public abstract class XAtomType : XSimpleType, ITryComparable<XAtomType> {
        protected XAtomType() { }
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
        protected override sealed bool TryValidateCore(Context context) {
            if (!base.TryValidateCore(context)) {
                return false;
            }
            var atomTypeInfo = (AtomTypeInfo)ObjectInfo;
            var restrictionSet = atomTypeInfo.RestrictionSet;
            if (restrictionSet != null) {

            }
            return true;
        }
        internal static bool TryCreate(Context context, AtomTypeInfo atomTypeInfo,
            AtomValueNode atomValueNode, out XAtomType result) {
            result = null;
            var atomType = atomTypeInfo.CreateInstance<XAtomType>();
            if (!atomType.TryParseAndSet(atomValueNode.Value)) {
                //context.AddErrorDiagnostic()
                return false;
            }
            if (!atomType.TryValidate(context)) {
                return false;
            }
            result = atomType;
            return true;
        }

        new public static readonly AtomTypeInfo ThisInfo = new AtomTypeInfo(typeof(XAtomType), true, TypeKind.AtomType.ToFullName(),
            XSimpleType.ThisInfo, null, TypeKind.AtomType);

    }

}
