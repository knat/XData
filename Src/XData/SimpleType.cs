using System;
using XData.IO.Text;

namespace XData {
    public abstract class XSimpleType : XType, IEquatable<XSimpleType> {
        protected XSimpleType() { }
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
        public virtual bool TryGetValueLength(out ulong result) {
            result = 0;
            return false;
        }
        protected override bool TryValidateCore(Context context) {
            var simpleTypeInfo = (SimpleTypeInfo)ObjectInfo;
            var restrictionSet = simpleTypeInfo.RestrictionSet;
            if (restrictionSet != null) {
                var minLength = restrictionSet.MinLength;
                var maxLength = restrictionSet.MaxLength;
                if (minLength != null || maxLength != null) {
                    ulong length;
                    if (!TryGetValueLength(out length)) {
                        throw new InvalidOperationException("!TryGetValueLength()");
                    }

                }
                var n_enumerations = restrictionSet.Enumerations;
                if (n_enumerations != null) {
                    var enumerations = n_enumerations.Value;
                    var found = false;
                    foreach (var item in enumerations.Items) {
                        if (ValueEquals(item.Value)) {
                            found = true;
                            break;
                        }
                    }
                    if (!found) {

                    }
                }
            }
            return true;
        }

        //public SimpleTypeInfo SimpleTypeInfo {
        //    get {
        //        return (SimpleTypeInfo)ObjectInfo;
        //    }
        //}
        public static readonly SimpleTypeInfo ThisInfo = new SimpleTypeInfo(typeof(XSimpleType), true, Extensions.SimpleTypeFullName, null, null);
        //
        internal static bool TryCreate(Context context, ProgramInfo programInfo, SimpleTypeInfo simpleTypeInfo,
            SimpleValueNode simpleValueNode, out XSimpleType result) {
            result = null;
            var effSimpleTypeInfo = (SimpleTypeInfo)GetTypeInfo(context, programInfo, simpleValueNode.TypeQName, simpleTypeInfo, simpleValueNode.TextSpan);
            if (effSimpleTypeInfo == null) {
                return false;
            }
            var atomTypeInfo = effSimpleTypeInfo as AtomTypeInfo;
            if (atomTypeInfo != null) {
                var atomicValueNode = simpleValueNode.Atom;
                if (!atomicValueNode.IsValid) {
                    //context.AddErrorDiagnostic()
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
                    //context.AddErrorDiagnostic()
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
