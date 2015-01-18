using System;
using XData.IO.Text;

namespace XData {
    public abstract class XType : XObject {
        protected XType() { }
        public TypeInfo TypeInfo {
            get {
                return (TypeInfo)ObjectInfo;
            }
        }
        //public static readonly TypeInfo ThisInfo = new TypeInfo(typeof(XType), AtomicTypeKind.Type.ToFullName(), AtomicTypeKind.Type, null);
        //
        internal static TypeInfo GetTypeInfo(Context context, ProgramInfo programInfo, QualifiableNameNode typeQName,
            TypeInfo declTypeInfo, TextSpan declTypeTextSpan) {
            TypeInfo typeInfo = null;
            if (typeQName.IsValid) {
                var typeFullName = typeQName.FullName;
                typeInfo = programInfo.TryGetGlobalObject(typeFullName) as TypeInfo;
                if (typeInfo == null) {
                    context.AddErrorDiagnostic(DiagnosticCode.InvalidTypeName, "Invalid type name '{0}'.".InvFormat(typeFullName.ToString()),
                        typeQName.TextSpan);
                    return null;
                }
                if (!typeInfo.IsEqualToOrDeriveFrom(declTypeInfo)) {
                    context.AddErrorDiagnostic(DiagnosticCode.TypeDoesNotEqualToOrDeriveFrom,
                        "Type '{0}' does not equal to or derive from '{1}'.".InvFormat(typeFullName.ToString(), declTypeInfo.FullName.ToString()),
                        typeQName.TextSpan);
                    return null;
                }
            }
            var effTypeInfo = typeInfo ?? declTypeInfo;
            if (effTypeInfo.IsAbstract) {
                context.AddErrorDiagnostic(DiagnosticCode.TypeIsAbstract, "Type '{0}' is abstract.".InvFormat(effTypeInfo.FullName.ToString()),
                    typeInfo != null ? typeQName.TextSpan : declTypeTextSpan);
                return null;
            }
            return effTypeInfo;
        }

    }
}
