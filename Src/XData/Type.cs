using XData.IO.Text;

namespace XData {
    public abstract class XType : XObject {
        public TypeInfo TypeInfo {
            get {
                return (TypeInfo)ObjectInfo;
            }
        }
        internal static TypeInfo GetEffectiveTypeInfo(DiagContext context, ProgramInfo programInfo, QualifiableNameNode typeQName,
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
                if (!typeInfo.EqualToOrDeriveFrom(declTypeInfo)) {
                    context.AddErrorDiag(new DiagMsg(DiagCode.TypeNotEqualToOrDeriveFrom,
                        typeInfo.DisplayName, declTypeInfo.DisplayName), typeQName.TextSpan);
                    return null;
                }
            }
            var effTypeInfo = typeInfo ?? declTypeInfo;
            if (effTypeInfo.IsAbstract) {
                context.AddErrorDiag(new DiagMsg(DiagCode.TypeIsAbstract, effTypeInfo.DisplayName),
                    typeInfo != null ? typeQName.TextSpan : declTypeTextSpan);
                return null;
            }
            return effTypeInfo;
        }
        internal bool CheckEqualToOrDeriveFrom(DiagContext context, TypeInfo baseTypeInfo) {
            var typeInfo = TypeInfo;
            if (!typeInfo.EqualToOrDeriveFrom(baseTypeInfo)) {
                context.AddErrorDiag(new DiagMsg(DiagCode.TypeNotEqualToOrDeriveFrom,
                    typeInfo.DisplayName, baseTypeInfo.DisplayName), this);
                return false;
            }
            return true;
        }
        internal void Save(SavingContext context, TypeInfo declTypeInfo) {
            var typeInfo = TypeInfo;
            if (typeInfo != declTypeInfo) {
                var sb = context.StringBuilder;
                sb.Append('(');
                context.Append(typeInfo.FullName);
                sb.Append(')');
            }
            SaveValue(context);
        }
        internal abstract void SaveValue(SavingContext context);


    }
}
