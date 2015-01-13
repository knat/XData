using System;
using System.Collections.Generic;
using System.IO;

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
    public static class Compiler {
        public static CompilationOutput Compile(CompilationInput input) {
            var schemaFilePathList = input.SchemaFilePathList;
            if (schemaFilePathList != null && schemaFilePathList.Count > 0) {
                var context = new Context();
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
