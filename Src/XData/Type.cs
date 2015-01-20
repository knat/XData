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
        //
        internal static TypeInfo GetTypeInfo(Context context, ProgramInfo programInfo, QualifiableNameNode typeQName,
            TypeInfo declTypeInfo, TextSpan declTypeTextSpan) {
            TypeInfo typeInfo = null;
            if (typeQName.IsValid) {
                var typeFullName = typeQName.FullName;
                typeInfo = programInfo.TryGetGlobalObject(typeFullName) as TypeInfo;
                if (typeInfo == null) {
                    context.AddErrorDiag(new DiagMsg(DiagCode.InvalidTypeName, typeFullName.ToString()),
                        typeQName.TextSpan);
                    return null;
                }
                if (!typeInfo.IsEqualToOrDeriveFrom(declTypeInfo)) {
                    context.AddErrorDiag(new DiagMsg(DiagCode.TypeDoesNotEqualToOrDeriveFrom,
                        typeFullName.ToString(), declTypeInfo.FullName.ToString()), typeQName.TextSpan);
                    return null;
                }
            }
            var effTypeInfo = typeInfo ?? declTypeInfo;
            if (effTypeInfo.IsAbstract) {
                context.AddErrorDiag(new DiagMsg(DiagCode.TypeIsAbstract, effTypeInfo.FullName.ToString()),
                    typeInfo != null ? typeQName.TextSpan : declTypeTextSpan);
                return null;
            }
            return effTypeInfo;
        }

    }
}
