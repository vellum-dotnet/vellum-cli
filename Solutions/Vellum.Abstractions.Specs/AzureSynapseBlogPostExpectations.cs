
using System.Collections.Generic;

namespace Vellum.Abstractions.Specs;

/// <summary>
/// Expected values for the azure-synapse-analytics-how-serverless-is-replacing-the-data-warehouse.md fixture,
/// shared by the content fragment and blog post conversion tests.
/// </summary>
internal static class AzureSynapseBlogPostExpectations
{
    public const string MarkdownFileName = "azure-synapse-analytics-how-serverless-is-replacing-the-data-warehouse.md";

    public const string HtmlFileName = "azure-synapse-analytics-how-serverless-is-replacing-the-data-warehouse.html";

    public const string Hash = "a8297eeb1841adf4d4f71efe2d7a43f5cba3a357eda570feeb02f5527619a130";

    public const string Title = "Azure Synapse Analytics: How serverless is replacing the data warehouse";

    public const string Slug = "azure-synapse-analytics-how-serverless-is-replacing-the-data-warehouse";

    public const string Author = "James.Broome";

    public const string HeaderImageUrl = "/assets/images/blog/2020/07/header-azure-synapse-analytics-how-severless-is-replacing-the-data-warehouse.png";

    public const string Excerpt = "Serverless data architectures enable leaner data insights and operations. How do you reap the rewards while avoiding the potential pitfalls?";

    public const string FaqOneQuestion = "How do you run an Azure Synapse SQL on-Demand query from Azure Data Factory?";

    public const string FaqOneAnswer = """Azure Synapse Analytics comes with tabular data stream (TDS) endpoint for SQL on-Demand, meaning you can run SQL queries as if you were talking to any SQL Server or Azure SQL Database. It's therefore possible to use a standard <a href="https://docs.microsoft.com/en-us/azure/data-factory/copy-activity-overview">Copy Activity</a> in the same way as you would were you to copy data from <a href="https://docs.microsoft.com/en-us/azure/data-factory/connector-sql-server">a Azure SQL Database</a>. The TDS endpoint can be found on the workspace overview tab of your Synapse workspace and is in the format <code><workspace-name>-ondemand.sql.azuresynapse.net</code>. Note that you will be constrained by the language features available with SQL on-Demand. In the future, it is likely that there will be tighter workspace integration along with stored procedure support. This means that you will be able to take advantage of SQL on-Demand features such as <a href="https://docs.microsoft.com/en-us/azure/synapse-analytics/sql/develop-tables-cetas">CETAS</a>.""";

    public const string FaqTwoQuestion = "Question 2";

    public const string FaqTwoAnswer = "Answer 2";

    public static readonly List<string> Categories =
    [
        "Azure",
        "Analytics",
        "Big Compute",
        "Big Data",
        "Azure Synapse Analytics",
        "Innovation",
        "Architecture",
        "Strategy",
    ];

    public static readonly List<string> Tags =
    [
        "Azure",
        "Data",
        "Analytics",
        "Serverless",
        "Azure Synapse",
        "Azure Synapse Analytics",
        "Azure Synapse Pipelines",
        "Synapse Pipelines",
        "Azure Data Factory",
        "Data Factory",
        "SQL Serverless",
        "SQL on-Demand",
        "Synapse Studio",
        "Data Engineering",
        "Data Prep",
        "Azure Synapse Analytics Jumpstart",
        "CSV",
        "Parquet",
        "Json",
        "Azure Data Lake Store",
        "ADLS",
    ];
}