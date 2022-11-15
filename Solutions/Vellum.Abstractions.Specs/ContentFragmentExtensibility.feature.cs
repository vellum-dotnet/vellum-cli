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
    [NUnit.Framework.DescriptionAttribute("ContentFragmentExtensibility")]
    public partial class ContentFragmentExtensibilityFeature
    {
        
        private TechTalk.SpecFlow.ITestRunner testRunner;
        
        private static string[] featureTags = ((string[])(null));
        
#line 1 "ContentFragmentExtensibility.feature"
#line hidden
        
        [NUnit.Framework.OneTimeSetUpAttribute()]
        public virtual void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "", "ContentFragmentExtensibility", null, ProgrammingLanguage.CSharp, featureTags);
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
            TechTalk.SpecFlow.Table table1 = new TechTalk.SpecFlow.Table(new string[] {
                        "document",
                        "file"});
            table1.AddRow(new string[] {
                        "Blog with extensions",
                        "blog-with-extensions.md"});
#line 5
  testRunner.Given("the following markdown files", ((string)(null)), table1, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table2 = new TechTalk.SpecFlow.Table(new string[] {
                        "document",
                        "file"});
            table2.AddRow(new string[] {
                        "Blog with extensions",
                        "blog-with-extensions.html"});
#line 8
  testRunner.Given("the following html files", ((string)(null)), table2, "Given ");
#line hidden
            TechTalk.SpecFlow.Table table3 = new TechTalk.SpecFlow.Table(new string[] {
                        "ContentType",
                        "Id",
                        "SpecPath"});
            table3.AddRow(new string[] {
                        "application/vnd.vellum.content.blogs+md",
                        "Blogs",
                        "../../blog-with-extensions.md"});
#line 11
  testRunner.Given("the following content blocks", ((string)(null)), table3, "Given ");
#line hidden
#line 14
  testRunner.Given("the \"Blogs\" ContentBlocks", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 15
  testRunner.And("the \"Blog with extensions\" document", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 16
  testRunner.And("we Create a Content Fragment", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
        }
        
        [NUnit.Framework.TestAttribute()]
        [NUnit.Framework.DescriptionAttribute("A markdown file with extensions gets converted into a Content Fragment with Exten" +
            "sions")]
        public void AMarkdownFileWithExtensionsGetsConvertedIntoAContentFragmentWithExtensions()
        {
            string[] tagsOfScenario = ((string[])(null));
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("A markdown file with extensions gets converted into a Content Fragment with Exten" +
                    "sions", null, tagsOfScenario, argumentsOfScenario, featureTags);
#line 18
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
                TechTalk.SpecFlow.Table table4 = new TechTalk.SpecFlow.Table(new string[] {
                            "ContentType",
                            "Date",
                            "Hash",
                            "Id",
                            "Position",
                            "PublicationStatus",
                            "BodyHtmlDocumentName"});
                table4.AddRow(new string[] {
                            "application/vnd.vellum.content.blogs+md",
                            "11/5/2022 6:30:00 AM",
                            "b749b45d87d8b06b2a64afaf9f1a0c0498cb317144eff241a45584444b35c262",
                            "Blogs",
                            "0",
                            "Published",
                            "Blog with extensions"});
#line 19
  testRunner.Then("Content Fragment should contain", ((string)(null)), table4, "Then ");
#line hidden
                TechTalk.SpecFlow.Table table5 = new TechTalk.SpecFlow.Table(new string[] {
                            "ContentType"});
                table5.AddRow(new string[] {
                            "application/vnd.vellum.content.series+md"});
                table5.AddRow(new string[] {
                            "application/vnd.vellum.content.recommendations+md"});
#line 22
  testRunner.And("the Content Fragment should contain the following Extensions:", ((string)(null)), table5, "And ");
#line hidden
            }
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion
