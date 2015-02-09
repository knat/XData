using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System.Runtime.Serialization;

namespace XData.MSBuild {

    public sealed class TestBuilder : Task {
        public override bool Execute() {
            Log.LogMessage(MessageImportance.High, "hello");
            return true;
        }
    }
    public sealed class XDataTask : Task {
        [Required]
        public string ProjectDirectory { get; set; }
        public ITaskItem[] XDataSchemaFiles { get; set; }
        public ITaskItem[] XDataIndicatorFiles { get; set; }
        //
        public override bool Execute() {
            var diagStore = new DiagStore();
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
                DiagContext diagContext;
                string code;
                var res = XData.Compiler.XDataCompiler.Compile(schemaFilePathList, indicatorFilePathList, out diagContext, out code);
                if (diagContext != null) {
                    foreach (var diag in diagContext) {
                        LogDiag(diag, diagStore);
                    }
                }
                if (!res) {
                    return false;
                }
                if (code != null) {
                    File.WriteAllText(Path.Combine(ProjectDirectory, "__XDataGenerated.cs"), code);
                }
                return true;
            }
            catch (Exception ex) {
                Log.LogErrorFromException(ex, true, true, null);
                return false;
            }
            finally {
                diagStore.Save(ProjectDirectory);
            }
            //C:\Windows\Microsoft.NET\Framework\v4.0.30319\msbuild.exe D:\Test\TestPLX\TestPLX\TestPLX.csproj
            //  v:detailed
        }
        private void LogDiag(Diag diag, DiagStore diagStore) {
            string subCategory = "XData";
            var codeString = diag.RawCode.ToString(System.Globalization.CultureInfo.InvariantCulture);
            string helpKeyword = null, filePath = null;
            int startLine = 0, startCol = 0, endLine = 0, endCol = 0;
            var textSpan = diag.TextSpan;
            if (textSpan.IsValid) {
                filePath = textSpan.FilePath;
                startLine = textSpan.StartPosition.Line;
                startCol = textSpan.StartPosition.Column;
                endLine = textSpan.EndPosition.Line;
                endCol = textSpan.EndPosition.Column;
            }
            var message = diag.Message;
            switch (diag.Severity) {
                case DiagSeverity.Error:
                    Log.LogError(subCategory, codeString, helpKeyword, filePath, startLine, startCol, endLine, endCol, message);
                    break;
                case DiagSeverity.Warning:
                    Log.LogWarning(subCategory, codeString, helpKeyword, filePath, startLine, startCol, endLine, endCol, message);
                    break;
                case DiagSeverity.Info:
                    Log.LogMessage(subCategory, codeString, helpKeyword, filePath, startLine, startCol, endLine, endCol, MessageImportance.Normal, message);
                    break;
            }
            if (filePath != null) {
                DiagUnit diagUnit;
                if (!diagStore.TryGetUnit(filePath, out diagUnit)) {
                    diagUnit = new DiagUnit(filePath, File.GetLastWriteTime(filePath));
                    diagStore.Add(diagUnit);
                }
                diagUnit.DiagList.Add(diag);
            }
        }
    }
    [DataContract(Namespace = Extensions.SystemUri)]
    public sealed class DiagUnit {
        internal DiagUnit(string filePath, DateTime lastWriteTime) {
            FilePath = filePath;
            LastWriteTime = lastWriteTime;
            DiagList = new List<Diag>();
        }
        [DataMember]
        public readonly string FilePath;
        [DataMember]
        public readonly DateTime LastWriteTime;
        [DataMember]
        public readonly List<Diag> DiagList;
    }
    [CollectionDataContract(Namespace = Extensions.SystemUri)]
    public sealed class DiagStore : List<DiagUnit> {
        public bool TryGetUnit(string filePath, out DiagUnit result) {
            foreach (var item in this) {
                if (item.FilePath == filePath) {
                    result = item;
                    return true;
                }
            }
            result = null;
            return false;
        }
        public DiagUnit TryGetUnit(string filePath, DateTime lastWriteTime) {
            foreach (var item in this) {
                if (item.FilePath == filePath) {
                    if (item.LastWriteTime == lastWriteTime) {
                        return item;
                    }
                    return null;
                }
            }
            return null;
        }
        public const string FileName = "XDataBuildDiags.xml";
        private static readonly DataContractSerializer _dcs = new DataContractSerializer(typeof(DiagStore));
        internal void Save(string projectDirectory) {
            var filePath = Path.Combine(projectDirectory, "obj", FileName);
            File.Delete(filePath);
            using (var fs = File.Create(filePath)) {
                _dcs.WriteObject(fs, this);
            }
        }
        public static DiagStore TryLoad(string filePath) {
            try {
                using (var fs = File.OpenRead(filePath)) {
                    return (DiagStore)_dcs.ReadObject(fs);
                }
            }
            catch (Exception) {
                return null;
            }
        }

    }

}
