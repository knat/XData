using System;
using System.Collections.Generic;
using System.IO;
using XData.IO.Text;

namespace XData.Compiler {

    public struct CompilationInput {
        public readonly List<string> SchemaFilePathList;
        public readonly List<string> CodeFilePathList;
    }
    public struct CompilationOutput {
        public readonly Context Context;
        public readonly string Code;
        public readonly bool IsValid;
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
            if (schemaFilePathList != null && schemaFilePathList.Count > 0) {
                var context = new ContextEx();
                var programNode = new ProgramNode();
                try {
                    foreach (var schemaFilePath in schemaFilePathList) {
                        using (var reader = new StreamReader(schemaFilePath)) {
                            CompilationUnitNode cuNode;
                            if (Parser.Parse(schemaFilePath, reader, context, programNode, out cuNode)) {
                                programNode.CompilationUnitList.Add(cuNode);
                            }
                            else {
                                goto Error;
                            }
                        }
                    }
                    var codeFilePathList = input.CodeFilePathList;
                    if (codeFilePathList != null && codeFilePathList.Count>0) {
                        programNode.CodeCompilationUnitList = new List<CodeCompilationUnitNode>();
                        foreach (var codeFilePath in codeFilePathList) {
                            using (var reader = new StreamReader(codeFilePath)) {
                                CodeCompilationUnitNode cuNode;
                                if (Parser.Parse(codeFilePath, reader, context, programNode, out cuNode)) {
                                    programNode.CodeCompilationUnitList.Add(cuNode);
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
                }
                catch (Exception ) {

                }
            Error:
                ;
            }

            return default(CompilationOutput);
        }
    }
}
