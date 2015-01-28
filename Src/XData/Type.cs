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
                if (!typeInfo.IsEqualToOrDeriveFrom(declTypeInfo)) {
                    context.AddErrorDiag(new DiagMsg(DiagCode.TypeNotEqualToOrDeriveFrom,
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
        public bool CheckObject(DiagContext context, TypeInfo baseTypeInfo) {
            var typeInfo = TypeInfo;
            if (!typeInfo.IsEqualToOrDeriveFrom(baseTypeInfo)) {
                context.AddErrorDiag(new DiagMsg(DiagCode.TypeNotEqualToOrDeriveFrom,
                    typeInfo.FullName.ToString(), baseTypeInfo.FullName.ToString()), this);
                return false;
            }
            return true;
        }
        public void Save(SavingContext context, TypeInfo declTypeInfo) {
            var typeInfo = TypeInfo;
            if (typeInfo != declTypeInfo) {
                context.Append('(');
                context.Append(typeInfo.FullName);
                context.StringBuilder.Append(')');
            }
            SaveValue(context);
        }
        public abstract void SaveValue(IndentedStringBuilder isb);


    }
}
