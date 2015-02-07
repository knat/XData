using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Language.StandardClassification;
using XData.Compiler;
using XData.MSBuild;

namespace XData.VisualStudio.Editors {
    internal static class ContentTypeDefinitions {
        //
        internal const string XDataSchemaContentType = "XDataSchema";
        internal const string XDataSchemaFileExtension = ".xds";
        [Export, BaseDefinition("code"), Name(XDataSchemaContentType)]
        internal static ContentTypeDefinition XDataSchemaContentTypeDefinition = null;
        [Export, ContentType(XDataSchemaContentType), FileExtension(XDataSchemaFileExtension)]
        internal static FileExtensionToContentTypeDefinition XDataSchemaFileExtensionDefinition = null;
        //
        internal const string XDataIndicatorContentType = "XDataIndicator";
        internal const string XDataIndicatorFileExtension = ".xdi";
        [Export, BaseDefinition("code"), Name(XDataIndicatorContentType)]
        internal static ContentTypeDefinition XCSharpContentTypeDefinition = null;
        [Export, ContentType(XDataIndicatorContentType), FileExtension(XDataIndicatorFileExtension)]
        internal static FileExtensionToContentTypeDefinition XDataIndicatorFileExtensionDefinition = null;
    }

    [Export(typeof(IClassifierProvider)),
        ContentType(ContentTypeDefinitions.XDataSchemaContentType),
        ContentType(ContentTypeDefinitions.XDataIndicatorContentType)]
    internal sealed class LanguageClassifierProvider : IClassifierProvider {
        [Import]
        internal IStandardClassificationService StandardService = null;
        public IClassifier GetClassifier(ITextBuffer textBuffer) {
            return textBuffer.Properties.GetOrCreateSingletonProperty<LanguageClassifier>(
                () => new LanguageClassifier(textBuffer, StandardService));
        }
    }
    internal sealed class LanguageClassifier : LanguageClassifierBase {
        internal LanguageClassifier(ITextBuffer textBuffer, IStandardClassificationService standardService)
            : base(textBuffer, standardService, ParserConstants.KeywordSet) {
        }
    }
    //
    //
    [Export(typeof(ITaggerProvider)), TagType(typeof(IErrorTag)),
        ContentType(ContentTypeDefinitions.XDataSchemaContentType),
        ContentType(ContentTypeDefinitions.XDataIndicatorContentType)]
    internal sealed class LanguageErrorTaggerProvider : LanguageErrorTaggerProviderBase {
        internal LanguageErrorTaggerProvider() : base(DiagStore.FileName, DiagStore.TryLoad) {
        }
    }


}
