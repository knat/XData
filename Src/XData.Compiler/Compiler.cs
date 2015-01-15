using System;
using System.Collections.Generic;
using System.IO;
using XData.IO.Text;

namespace XData.Compiler {

    public struct CompilationInput {
        public readonly List<string> SchemaFilePathList;
        public readonly List<string> CSNSIndicatorFilePathList;
    }
    public struct CompilationOutput {
        public readonly Context Context;
        public readonly string Code;
        public bool IsValid {
            get {
                return Context != null;
            }
        }
    }
    public enum DiagnosticCodeEx {
        None = 0,
        AliasIsReserved = -2000,
        UriIsReserved,
        DuplicateUriAlias,
        InvalidUriAlias,
        DuplicateImportAlias,
        InvalidImportUri,
        DuplicateNamespaceMember,
        InvalidQualifiableNameAlias,
        AmbiguousNameReference,
        InvalidNameReference,
        InvalidTypeNameReference,
        InvalidAttributeNameReference,
        InvalidElementNameReference,
        UInt64ValueRequired,
        ByteValueRequired,
        MaxValueMustEqualToOrBeGreaterThanMinValue,
        MaxValueMustBeGreaterThanZero,
        DuplicateMemberName,
        DuplicateAttributeFullName,
        InvalidIndicatorNamespaceName,
        InconsistentCSharpNamespaceName,
        CSNamespaceNameNotSpecifiedForNamespace,
        CircularReferenceDetected,
    }

    public sealed class ContextEx : Context {
        [ThreadStatic]
        public static ContextEx Current;
        public sealed class ContextException : Exception { }
        private static readonly ContextException _contextException = new ContextException();
        public static void ErrorDiagnostic(DiagnosticCodeEx code, string errMsg, TextSpan textSpan) {
            Current.AddDiagnostic(DiagnosticSeverity.Error, (int)code, errMsg, textSpan, null);
        }
        public static void ErrorDiagnosticAndThrow(DiagnosticCodeEx code, string errMsg, TextSpan textSpan) {
            ErrorDiagnostic(code, errMsg, textSpan);
            throw _contextException;
        }
        public static void ThrowIfHasErrors() {
            if (Current.HasErrorDiagnostics) {
                throw _contextException;
            }
        }
        public static void WarningDiagnostic(DiagnosticCodeEx code, string errMsg, TextSpan textSpan) {
            Current.AddDiagnostic(DiagnosticSeverity.Warning, (int)code, errMsg, textSpan, null);
        }

    }
    public static class Compiler {
        public static CompilationOutput Compile(CompilationInput input) {
            var schemaFilePathList = input.SchemaFilePathList;
            var csNSIndicatorFilePathList = input.CSNSIndicatorFilePathList;
            var hasSchemaFile = schemaFilePathList != null && schemaFilePathList.Count > 0;
            var hasCSNSIndicatorFile = csNSIndicatorFilePathList != null && csNSIndicatorFilePathList.Count > 0;
            if (!hasSchemaFile && !hasCSNSIndicatorFile) {
                return default(CompilationOutput);
            }
            var context = new ContextEx();
            var programNode = new ProgramNode();
            try {
                if (hasSchemaFile) {
                    programNode.CompilationUnitList = new List<CompilationUnitNode>();
                    foreach (var filePath in schemaFilePathList) {
                        using (var reader = new StreamReader(filePath)) {
                            CompilationUnitNode cuNode;
                            if (Parser.Parse(filePath, reader, context, programNode, out cuNode)) {
                                programNode.CompilationUnitList.Add(cuNode);
                            }
                            else {
                                goto Error;
                            }
                        }
                    }
                }
                if (hasCSNSIndicatorFile) {
                    programNode.CSNSIndicatorCompilationUnitList = new List<CSNSIndicatorCompilationUnitNode>();
                    foreach (var filePath in csNSIndicatorFilePathList) {
                        using (var reader = new StreamReader(filePath)) {
                            CSNSIndicatorCompilationUnitNode cuNode;
                            if (Parser.Parse(filePath, reader, context, programNode, out cuNode)) {
                                programNode.CSNSIndicatorCompilationUnitList.Add(cuNode);
                            }
                            else {
                                goto Error;
                            }
                        }
                    }
                }
                //
                ContextEx.Current = context;
                try {
                    programNode.Analyze();
                }
                catch (ContextEx.ContextException) { }
            Error:
                ;
            }
            catch (Exception) {

            }

            return default(CompilationOutput);
        }
    }
}
