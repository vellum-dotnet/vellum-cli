namespace Vellum.Abstractions.Specs
{
    using System.IO;
    using TechTalk.SpecFlow;

    [Binding]
    public class ContentFragmentTypeFactorySteps
    {
        private readonly MarkdownDocumentRegistry markdownDocumentRegistry;
    
        public ContentFragmentTypeFactorySteps(MarkdownDocumentRegistry markdownDocumentRegistry)
        {
            this.markdownDocumentRegistry = markdownDocumentRegistry;
        }

        [Given(@"the following markdown files")]
        public void GivenTheFollowingMarkdownFiles(Table table)
        {
            foreach (TableRow row in table.Rows)
            {
                this.markdownDocumentRegistry.Register(row["document"], row["file"]);
            }
        }

        [Given(@"the ""([^""]*)"" document")]
        public void GivenTheDocument(string documentName)
        {
            Stream stream = this.markdownDocumentRegistry.GetTemplateStream(documentName);
        }
    }
}