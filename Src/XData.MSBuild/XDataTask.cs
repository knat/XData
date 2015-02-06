using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using XData.Compiler;

namespace XData.MSBuild {
    public sealed class XDataTask : Task {
        public ITaskItem[] XDataSchemaFiles { get; set; }
        public ITaskItem[] XDataIndicatorFiles { get; set; }
        //[Output]
        //public ITaskItem[] OutputCSharpFiles { get; set; }
        //
        public override bool Execute() {
            //var errorStore = new XBuildErrorStore();
            try {
                List<string> schemaFilePathList = null;
                List<string> indicatorFilePathList = null;
                if (XDataSchemaFiles != null && XDataSchemaFiles.Length > 0) {
                    schemaFilePathList = new List<string>();
                    foreach (var item in XDataSchemaFiles) {
                        schemaFilePathList.Add(item.GetMetadata("FullPath"));
                    }
                }
                if (XDataIndicatorFiles != null && XDataIndicatorFiles.Length > 0) {
                    indicatorFilePathList = new List<string>();
                    foreach (var item in XDataIndicatorFiles) {
                        indicatorFilePathList.Add(item.GetMetadata("FullPath"));
                    }
                }
                DiagContext context;
                string code;
                if (!XDataCompiler.Compile(schemaFilePathList, indicatorFilePathList, out context, out code)) {

                }

                //var mxFilePath = CopyFile("Metah.X.cs", false);//EmbedSDOM);
                //CopyFile("Metah.X.dll", false);
                //if ((XDataCSNSIndicatorFiles == null || XDataCSNSIndicatorFiles.Length == 0) && (XDataSchemaFiles == null || XDataSchemaFiles.Length == 0)) {
                //    base.Log.LogMessage(MessageImportance.High, "Skip compilation");
                //    return true;
                //}
                //var xCSharpFileList = CreateCompilationInputFileList(XDataCSNSIndicatorFiles);
                //var xFileList = CreateCompilationInputFileList(XDataSchemaFiles);
                //var cSharpFileList = CreateCompilationInputFileList(CSharpFiles);
                //if (EmbedSDOM) cSharpFileList.Add(new CompilationInputFile(mxFilePath));
                //var preprocessorSymbolList = CreatePreprocessorSymbolList();
                //var metadataReferenceList = CreateMetadataReferenceList();
                //var compilationInput = new XCompilationInput(preprocessorSymbolList, cSharpFileList, metadataReferenceList, xCSharpFileList, xFileList);
                //var compilationOutput = XCompiler.Compile(compilationInput);
                //foreach (var error in compilationOutput.ErrorList) LogError(error, errorStore);
                //if (compilationOutput.HasErrors) return false;
                //var outputCSharpFileList = new List<TaskItem>();
                //if (EmbedSDOM) outputCSharpFileList.Add(new TaskItem(mxFilePath));
                //if (compilationOutput.Analyzer != null) {
                //    foreach (var compilationUnit in compilationOutput.Analyzer.CompilationUnits) {
                //        var filePath = compilationUnit.FilePath + ".cs";
                //        File.WriteAllText(filePath, compilationUnit.CSText);
                //        outputCSharpFileList.Add(new TaskItem(filePath));
                //    }
                //}
                //OutputCSharpFiles = outputCSharpFileList.ToArray();
                return true;
            }
            catch (Exception ex) {
                base.Log.LogErrorFromException(ex, true, true, null);
                return false;
            }
            finally {
                //errorStore.Save(ProjectDirectory);
            }
            //C:\Windows\Microsoft.NET\Framework\v4.0.30319\msbuild.exe D:\Test\TestPLX\TestPLX\TestPLX.csproj
            //v:detailed
        }
    }
    public sealed class BuildErrorUnit {
        internal BuildErrorUnit(string filePath, DateTime lastWriteTime) {
            FilePath = filePath;
            LastWriteTime = lastWriteTime;
            ErrorList = new List<Diag>();
        }
        public readonly string FilePath;
        public readonly DateTime LastWriteTime;
        public readonly List<Diag> ErrorList;
    }
    public sealed class BuildErrorStore : Dictionary<string, BuildErrorUnit> {//key:FilePath

    }

    }
