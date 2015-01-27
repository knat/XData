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
        public readonly DiagContext Context;
        public readonly string Code;
        public bool IsValid {
            get {
                return Context != null;
            }
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
            var context = new DiagContextEx();
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
                DiagContextEx.Current = context;
                try {
                    programNode.Analyze();
                }
                catch (DiagContextEx.ContextException) { }
            Error:
                ;
            }
            catch (Exception) {

            }

            return default(CompilationOutput);
        }
    }
}
