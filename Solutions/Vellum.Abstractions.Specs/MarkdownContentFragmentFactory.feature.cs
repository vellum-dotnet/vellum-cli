﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (https://www.specflow.org/).
//      SpecFlow Version:3.9.0.0
//      SpecFlow Generator Version:3.9.0.0
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
namespace Vellum.Abstractions.Specs
{
    using TechTalk.SpecFlow;
    using System;
    using System.Linq;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "3.9.0.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [NUnit.Framework.TestFixtureAttribute()]
    [NUnit.Framework.DescriptionAttribute("MarkdownContentFragmentFactory")]
    public partial class MarkdownContentFragmentFactoryFeature
    {
        
        private TechTalk.SpecFlow.ITestRunner testRunner;
        
        private static string[] featureTags = ((string[])(null));
        
#line 1 "MarkdownContentFragmentFactory.feature"
#line hidden
        
        [NUnit.Framework.OneTimeSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "", "MarkdownContentFragmentFactory", null, ProgrammingLanguage.CSharp, featureTags);
            testRunner.OnFeatureStart(featureInfo);
        }
        
        [NUnit.Framework.OneTimeTearDownAttribute()]
        public virtual void FeatureTearDown()
        {
            testRunner.OnFeatureEnd();
            testRunner = null;
        }
        
        [NUnit.Framework.SetUpAttribute()]
        public void TestInitialize()
        {
        }
        
        [NUnit.Framework.TearDownAttribute()]
        public void TestTearDown()
        {
            testRunner.OnScenarioEnd();
        }
        
        public void ScenarioInitialize(TechTalk.SpecFlow.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioInitialize(scenarioInfo);
            testRunner.ScenarioContext.ScenarioContainer.RegisterInstanceAs<NUnit.Framework.TestContext>(NUnit.Framework.TestContext.CurrentContext);
        }
        
        public void ScenarioStart()
        {
            testRunner.OnScenarioStart();
        }
        
        public void ScenarioCleanup()
        {
            testRunner.CollectScenarioErrors();
        }
        
        public virtual void FeatureBackground()
        {
#line 3
#line hidden
            TechTalk.SpecFlow.Table table5 = new TechTalk.SpecFlow.Table(new string[] {
                        "document",
                        "file"});
            table5.AddRow(new string[] {
                        "How serverless is replacing the data warehouse",
                        "azure-synapse-analytics-how-serverless-is-replacing-the-data-warehouse.md"});
#line 5
  testRunner.Given("the following markdown files", ((string)(null)), table5, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table6 = new TechTalk.SpecFlow.Table(new string[] {
                        "document",
                        "file"});
            table6.AddRow(new string[] {
                        "How serverless is replacing the data warehouse",
                        "azure-synapse-analytics-how-serverless-is-replacing-the-data-warehouse.html"});
#line 9
  testRunner.Given("the following html files", ((string)(null)), table6, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table7 = new TechTalk.SpecFlow.Table(new string[] {
                        "ContentType",
                        "Id",
                        "SpecPath"});
            table7.AddRow(new string[] {
                        "application/vnd.vellum.content.blogs+md",
                        "Blogs",
                        "../../azure-synapse-analytics-how-serverless-is-replacing-the-data-warehouse.md"});
#line 13
  testRunner.Given("the following content blocks", ((string)(null)), table7, "Given ");
#line hidden
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("Converting a Markdown Document into a ContentFragment")]
        public void ConvertingAMarkdownDocumentIntoAContentFragment()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Converting a Markdown Document into a ContentFragment", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 17
this.ScenarioInitialize(scenarioInfo);
#line hidden
            if ((TagHelper.ContainsIgnoreTag(tagsOfScenario) || TagHelper.ContainsIgnoreTag(featureTags)))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 3
this.FeatureBackground();
#line hidden
#line 18
  testRunner.Given("the \"Blogs\" ContentBlocks", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 19
  testRunner.And("the \"How serverless is replacing the data warehouse\" document", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 20
  testRunner.When("Create a Content Fragment", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
                TechTalk.SpecFlow.Table table8 = new TechTalk.SpecFlow.Table(new string[] {
                            "ContentType",
                            "Date",
                            "Hash",
                            "Id",
                            "Position",
                            "PublicationStatus",
                            "BodyHtmlDocumentName"});
                table8.AddRow(new string[] {
                            "application/vnd.vellum.content.blogs+md",
                            "7/15/2020 6:30:00 AM",
                            "6c52e2a15f812f646885bc2ef0e04a82dffe97ef8cf1af5a8a15817656c7f915",
                            "Blogs",
                            "0",
                            "Published",
                            "How serverless is replacing the data warehouse"});
#line 21
  testRunner.Then("Content Fragment should contain", ((string)(null)), table8, "Then ");
#line hidden
                TechTalk.SpecFlow.Table table9 = new TechTalk.SpecFlow.Table(new string[] {
                            "Title",
                            "Slug",
                            "Author",
                            "HeaderImageUrl",
                            "Excerpt",
                            "FilePath"});
                table9.AddRow(new string[] {
                            "Azure Synapse Analytics: How serverless is replacing the data warehouse",
                            "azure-synapse-analytics-how-serverless-is-replacing-the-data-warehouse",
                            "James.Broome",
                            "/assets/images/blog/2020/07/header-azure-synapse-analytics-how-severless-is-repla" +
                                "cing-the-data-warehouse.png",
                            "Serverless data architectures enable leaner data insights and operations. How do " +
                                "you reap the rewards while avoiding the potential pitfalls?",
                            "C:\\_Projects\\OSS\\vellum-dotnet\\vellum-cli\\Solutions\\Vellum.Abstractions.Specs\\Mar" +
                                "kdownDocuments\\azure-synapse-analytics-how-serverless-is-replacing-the-data-ware" +
                                "house.md"});
#line 24
  testRunner.And("the Content Fragment MetaData should contain", ((string)(null)), table9, "And ");
#line hidden
                TechTalk.SpecFlow.Table table10 = new TechTalk.SpecFlow.Table(new string[] {
                            "Value"});
                table10.AddRow(new string[] {
                            "Azure"});
                table10.AddRow(new string[] {
                            "Analytics"});
                table10.AddRow(new string[] {
                            "Big Compute"});
                table10.AddRow(new string[] {
                            "Big Data"});
                table10.AddRow(new string[] {
                            "Azure Synapse Analytics"});
                table10.AddRow(new string[] {
                            "Innovation"});
                table10.AddRow(new string[] {
                            "Architecture"});
                table10.AddRow(new string[] {
                            "Strategy"});
#line 27
  testRunner.And("the Content Fragment MetaData Category should contain", ((string)(null)), table10, "And ");
#line hidden
                TechTalk.SpecFlow.Table table11 = new TechTalk.SpecFlow.Table(new string[] {
                            "Key"});
                table11.AddRow(new string[] {
                            "Azure"});
                table11.AddRow(new string[] {
                            "Data"});
                table11.AddRow(new string[] {
                            "Analytics"});
                table11.AddRow(new string[] {
                            "Serverless"});
                table11.AddRow(new string[] {
                            "Azure Synapse"});
                table11.AddRow(new string[] {
                            "Azure Synapse Analytics"});
                table11.AddRow(new string[] {
                            "Azure Synapse Pipelines"});
                table11.AddRow(new string[] {
                            "Synapse Pipelines"});
                table11.AddRow(new string[] {
                            "Azure Data Factory"});
                table11.AddRow(new string[] {
                            "Data Factory"});
                table11.AddRow(new string[] {
                            "SQL Serverless"});
                table11.AddRow(new string[] {
                            "SQL on-Demand"});
                table11.AddRow(new string[] {
                            "Synapse Studio"});
                table11.AddRow(new string[] {
                            "Data Engineering"});
                table11.AddRow(new string[] {
                            "Data Prep"});
                table11.AddRow(new string[] {
                            "Azure Synapse Analytics Jumpstart"});
                table11.AddRow(new string[] {
                            "CSV"});
                table11.AddRow(new string[] {
                            "Parquet"});
                table11.AddRow(new string[] {
                            "Json"});
                table11.AddRow(new string[] {
                            "Azure Data Lake Store"});
                table11.AddRow(new string[] {
                            "ADLS"});
#line 37
  testRunner.And("the Content Fragment MetaData Tags should contain", ((string)(null)), table11, "And ");
#line hidden
                TechTalk.SpecFlow.Table table12 = new TechTalk.SpecFlow.Table(new string[] {
                            "Question",
                            "Answer"});
                table12.AddRow(new string[] {
                            "How do you run an Azure Synapse SQL on-Demand query from Azure Data Factory?",
                            @"Azure Synapse Analytics comes with tabular data stream (TDS) endpoint for SQL on-Demand, meaning you can run SQL queries as if you were talking to any SQL Server or Azure SQL Database. It's therefore possible to use a standard <a href=""https://docs.microsoft.com/en-us/azure/data-factory/copy-activity-overview"">Copy Activity</a> in the same way as you would were you to copy data from <a href=""https://docs.microsoft.com/en-us/azure/data-factory/connector-sql-server"">a Azure SQL Database</a>. The TDS endpoint can be found on the workspace overview tab of your Synapse workspace and is in the format <code><workspace-name>-ondemand.sql.azuresynapse.net</code>. Note that you will be constrained by the language features available with SQL on-Demand. In the future, it is likely that there will be tighter workspace integration along with stored procedure support. This means that you will be able to take advantage of SQL on-Demand features such as <a href=""https://docs.microsoft.com/en-us/azure/synapse-analytics/sql/develop-tables-cetas"">CETAS</a>."});
#line 60
    testRunner.And("the Content Fragment MetaData FAQs should contain", ((string)(null)), table12, "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion
