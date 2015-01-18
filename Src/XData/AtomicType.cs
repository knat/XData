using System;
using XData.IO.Text;

namespace XData {
    public interface ITryComparable<in T> {
        bool TryCompareTo(T other, out int result);
    }

    public abstract class XAtomicType : XSimpleType, ITryComparable<XAtomicType> {
        protected XAtomicType() { }
        public abstract bool TryParseAndSet(string literal);
        public virtual bool TryCompareTo(XAtomicType other, out int result) {
            result = 0;
            return false;
        }
        public virtual bool TryCompareValueTo(object other, out int result) {
            result = 0;
            return false;
        }
        public static bool operator <(XAtomicType x, XAtomicType y) {
            if ((object)x == null) return false;
            int result;
            if (x.TryCompareTo(y, out result) && result < 0) {
                return true;
            }
            return false;
        }
        public static bool operator <=(XAtomicType x, XAtomicType y) {
            if ((object)x == null) return false;
            int result;
            if (x.TryCompareTo(y, out result) && result <= 0) {
                return true;
            }
            return false;
        }
        public static bool operator >(XAtomicType x, XAtomicType y) {
            return y < x;
        }
        public static bool operator >=(XAtomicType x, XAtomicType y) {
            return y <= x;
        }
        protected override sealed bool TryValidateCore(Context context) {
            if (!base.TryValidateCore(context)) {
                return false;
            }
            var atomicTypeInfo = (AtomicTypeInfo)ObjectInfo;
            var restrictionSet = atomicTypeInfo.RestrictionSet;
            if (restrictionSet != null) {

            }
            return true;
        }
        internal static bool TryCreate(Context context, AtomicTypeInfo atomicTypeInfo,
            AtomicValueNode atomicValueNode, out XAtomicType result) {
            result = null;
            var atomicType = atomicTypeInfo.CreateInstance<XAtomicType>();
            if (!atomicType.TryParseAndSet(atomicValueNode.Value)) {
                //context.AddErrorDiagnostic()
                return false;
            }
            if (!atomicType.TryValidate(context)) {
                return false;
            }
            result = atomicType;
            return true;
        }

        new public static readonly AtomicTypeInfo ThisInfo = new AtomicTypeInfo(typeof(XAtomicType), true, Extensions.AtomicTypeFullName,
            XSimpleType.ThisInfo, null, AtomicTypeKind.None);

    }

}
