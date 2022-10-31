namespace Vellum.Abstractions.Specs
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Microsoft.Extensions.DependencyInjection;
    using NDepend.Path;
    using Shouldly;
    using TechTalk.SpecFlow;
    using Vellum.Abstractions.Content;
    using Vellum.Abstractions.Content.ContentFactories;
    using Vellum.Abstractions.Content.Formatting;

    [Binding]
    public class MarkdownContentFragmentFactorySteps
    {
        private readonly ScenarioContext scenarioContext;
        private readonly HtmlDocumentRegistry htmlDocumentRegistry;
        private readonly MarkdownDocumentRegistry markdownDocumentRegistry;
        private readonly ContentBlockRegistry contentBlockRegistry;
        private readonly IServiceProvider serviceProvider;

        public MarkdownContentFragmentFactorySteps(
            MarkdownDocumentRegistry markdownDocumentRegistry,
            HtmlDocumentRegistry htmlDocumentRegistry,
            ContentBlockRegistry contentBlockRegistry,
            IServiceProvider serviceProvider,
            ScenarioContext scenarioContext)
        {
            this.markdownDocumentRegistry = markdownDocumentRegistry;
            this.htmlDocumentRegistry = htmlDocumentRegistry;
            this.contentBlockRegistry = contentBlockRegistry;
            this.serviceProvider = serviceProvider;
            this.scenarioContext = scenarioContext;
        }

        [Given(@"the following markdown files")]
        public void GivenTheFollowingMarkdownFiles(Table table)
        {
            foreach (TableRow row in table.Rows)
            {
                this.markdownDocumentRegistry.Register(row["document"], row["file"]);
            }
        }

        [Given(@"the following html files")]
        public void GivenTheFollowingHtmlFiles(Table table)
        {
            foreach (TableRow row in table.Rows)
            {
                this.htmlDocumentRegistry.Register(row["document"], row["file"]);
            }
        }

        [Given(@"the following content blocks")]
        public void GivenTheFollowingContentBlocks(Table table)
        {
            foreach (TableRow row in table.Rows)
            {
                this.contentBlockRegistry.Register(row["Id"], row["ContentType"], row["SpecPath"]);
            }
        }

        [Given(@"the ""([^""]*)"" ContentBlocks")]
        public void GivenTheContentBlocks(string id)
        {
            ContentBlock contentBlock = this.contentBlockRegistry.Get(id);
            this.scenarioContext.Set(contentBlock);
        }


        [Given(@"the ""([^""]*)"" document")]
        public void GivenTheDocument(string documentName)
        {
            FileInfo file = this.markdownDocumentRegistry.GetFile(documentName);
            string content = File.ReadAllText(file.FullName);
            IAbsoluteFilePath filePath = file.FullName.ToAbsoluteFilePath();

            this.scenarioContext.Set(content);
            this.scenarioContext.Set(filePath);
        }

        [Given(@"we Create a Content Fragment")]
        [When(@"Create a Content Fragment")]
        public void WhenCreateAContentFragment()
        {
            string content = this.scenarioContext.Get<string>();
            ContentBlock contentBlock = this.scenarioContext.Get<ContentBlock>();
            IAbsoluteFilePath filePath = this.scenarioContext.Get<IAbsoluteFilePath>();
            IContentFormatter contentFormatter = this.serviceProvider.GetRequiredService<IContentFormatter>();

            MarkdownContentFragmentFactory markdownContentFragmentFactory = new(contentFormatter);
            ContentFragment cf = markdownContentFragmentFactory.Create(contentBlock, content, filePath);

            this.scenarioContext.Set(cf);
        }

        [Then(@"Content Fragment should contain")]
        public void ThenContentFragmentShouldContain(Table table)
        {
            TableRow firstRow = table.Rows[0];

            string contentType = firstRow["ContentType"];
            DateTime date = DateTime.Parse(firstRow["Date"]);
            string hash = firstRow["Hash"];
            string id = firstRow["Id"];
            int position = int.Parse(firstRow["Position"]);
            PublicationStatus publicationStatus = PublicationStatusEnumParser.Parse(firstRow["PublicationStatus"]);
            string bodyHtmlDocumentName = firstRow["BodyHtmlDocumentName"];
            string bodyHtml = this.htmlDocumentRegistry.GetFileContent(bodyHtmlDocumentName);

            ContentFragment cf = this.scenarioContext.Get<ContentFragment>();

            cf.ContentType.ShouldBe(contentType);
            cf.Hash.ShouldBe(hash);
            cf.Id.ShouldBe(id);
            cf.Position.ShouldBe(position);
            cf.Date.ShouldBe(date);
            cf.PublicationStatus.ShouldBe(publicationStatus);
            cf.Body.ShouldBe(bodyHtml);
        }

        [Then(@"the Content Fragment MetaData should contain")]
        public void ThenTheContentFragmentMetaDataShouldContain(Table table)
        {
            ContentFragment cf = this.scenarioContext.Get<ContentFragment>();
            TableRow firstRow = table.Rows[0];
            ((string)cf.MetaData["Title"]).ShouldBe<string>(firstRow["Title"]);
            ((string)cf.MetaData["Slug"]).ShouldBe(firstRow["Slug"]);
            ((string)cf.MetaData["Author"]).ShouldBe(firstRow["Author"]);
            ((string)cf.MetaData["HeaderImageUrl"]).ShouldBe(firstRow["HeaderImageUrl"]);
            ((string)cf.MetaData["Excerpt"]).ShouldBe(firstRow["Excerpt"]);
            ((string)cf.MetaData["FilePath"]).ShouldBe(firstRow["FilePath"]);
        }

        [Then(@"the Content Fragment MetaData Category should contain")]
        public void ThenTheContentFragmentMetaDataCategoryShouldContain(Table table)
        {
            ContentFragment cf = this.scenarioContext.Get<ContentFragment>();

            List<string> list = new();

            foreach (TableRow row in table.Rows)
            {
                list.Add(row["Value"].Trim());
            }

            List<object> actual = cf.MetaData["Category"];

            actual.Cast<string>().ShouldBe(list);
        }

        [Then(@"the Content Fragment MetaData Tags should contain")]
        public void ThenTheContentFragmentMetaDataTagsShouldContain(Table table)
        {
            ContentFragment cf = this.scenarioContext.Get<ContentFragment>();

            List<string> list = new();

            foreach (TableRow row in table.Rows)
            {
                list.Add(row["Key"].Trim());
            }

            List<object> actual = cf.MetaData["Tags"];

            actual.Cast<string>().ShouldBe(list);
        }

        [Then(@"the Content Fragment MetaData FAQs should contain")]
        public void ThenTheContentFragmentMetaDataFAQsShouldContain(Table table)
        {
            ContentFragment cf = this.scenarioContext.Get<ContentFragment>();

            List<KeyValuePair<string, string>> expected = new();

            foreach (TableRow row in table.Rows)
            {
                expected.Add(new KeyValuePair<string, string>("Q", row["Question"]));
                expected.Add(new KeyValuePair<string, string>("A", row["Answer"]));
            }

            List<object> actual = cf.MetaData["FAQs"];

            IEnumerable<Dictionary<object, object>> output = actual.Cast<Dictionary<object, object>>();

            List<KeyValuePair<string, string>> actuals = new();

            foreach (Dictionary<object, object> dictionary in output)
            {
                foreach (KeyValuePair<object, object> keyValuePair in dictionary)
                {
                    string key = keyValuePair.Key as string;
                    string value = keyValuePair.Value as string;
                    actuals.Add(new KeyValuePair<string, string>(key, value));
                }
            }

            expected.ShouldBe(actuals);
        }

        [Given(@"we obtain a ContentFragmentTypeFactory for the Content Fragment Content Type")]
        public void GivenWeObtainAContentFragmentTypeFactoryForTheContentFragmentContentType()
        {
            ContentFragment cf = this.scenarioContext.Get<ContentFragment>();
            ContentFragmentTypeFactory<IBlogPost> cff = this.serviceProvider.GetContent<ContentFragmentTypeFactory<IBlogPost>>(cf.ContentType.AsContentFragmentFactory());

            this.scenarioContext.Set(cff);
        }

        [When(@"we create the BlogPost")]
        public void WhenWeCreateTheBlogPost()
        {
            ContentFragment cf = this.scenarioContext.Get<ContentFragment>();
            ContentFragmentTypeFactory<IBlogPost> cff = this.scenarioContext.Get<ContentFragmentTypeFactory<IBlogPost>>();
            IBlogPost blogPost = cff.Create(cf);
        }
    }
}