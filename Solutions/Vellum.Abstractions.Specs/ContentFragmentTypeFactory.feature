Feature: ContentFragmentTypeFactory

Background:

  Given the following markdown files
    | document                                       | file                                                                      |
    | How serverless is replacing the data warehouse | azure-synapse-analytics-how-serverless-is-replacing-the-data-warehouse.md |

  Given the following html files
    | document                                       | file                                                                        |
    | How serverless is replacing the data warehouse | azure-synapse-analytics-how-serverless-is-replacing-the-data-warehouse.html |

  Given the following content blocks
    | ContentType                             | Id    | SpecPath                                                                        |
    | application/vnd.vellum.content.blogs+md | Blogs | ../../azure-synapse-analytics-how-serverless-is-replacing-the-data-warehouse.md |

Scenario: Converting a blog post
  Given the "Blogs" ContentBlocks
  And the "How serverless is replacing the data warehouse" document
  When Create a Content Fragment
  Then Content Fragment should contain
    | ContentType                             | Date                 | Hash                                                             | Id    | Position | PublicationStatus | BodyHtmlDocumentName                           |
    | application/vnd.vellum.content.blogs+md | 7/15/2020 6:30:00 AM | 6c52e2a15f812f646885bc2ef0e04a82dffe97ef8cf1af5a8a15817656c7f915 | Blogs | 0        | Published         | How serverless is replacing the data warehouse |
  And the Content Fragment MetaData should contain
  | Title                                                                   | Slug                                                                   | Author       | HeaderImageUrl                                                                                               | Excerpt                                                                                                                                      | FilePath                                                                                                                                                                          |
  | Azure Synapse Analytics: How serverless is replacing the data warehouse | azure-synapse-analytics-how-serverless-is-replacing-the-data-warehouse | James.Broome | /assets/images/blog/2020/07/header-azure-synapse-analytics-how-severless-is-replacing-the-data-warehouse.png | Serverless data architectures enable leaner data insights and operations. How do you reap the rewards while avoiding the potential pitfalls? | C:\\_Projects\\OSS\\vellum-dotnet\\vellum-cli\\Solutions\\Vellum.Abstractions.Specs\\MarkdownDocuments\\azure-synapse-analytics-how-serverless-is-replacing-the-data-warehouse.md |
  And the Content Fragment MetaData Category should contain
  | Value                   |
  | Azure                   |
  | Analytics               |
  | Big Compute             |
  | Big Data                |
  | Azure Synapse Analytics |
  | Innovation              |
  | Architecture            |
  | Strategy                |
  And the Content Fragment MetaData Tags should contain 
  | Key                               |
  | Azure                             |
  | Data                              |
  | Analytics                         |
  | Serverless                        |
  | Azure Synapse                     |
  | Azure Synapse Analytics           |
  | Azure Synapse Pipelines           |
  | Synapse Pipelines                 |
  | Azure Data Factory                |
  | Data Factory                      |
  | SQL Serverless                    |
  | SQL on-Demand                     |
  | Synapse Studio                    |
  | Data Engineering                  |
  | Data Prep                         |
  | Azure Synapse Analytics Jumpstart |
  | CSV                               |
  | Parquet                           |
  | Json                              |
  | Azure Data Lake Store             |
  | ADLS                              |
    And the Content Fragment MetaData FAQs should contain 
  | Question                                                                     | Answer                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        |
  | How do you run an Azure Synapse SQL on-Demand query from Azure Data Factory? | Azure Synapse Analytics comes with tabular data stream (TDS) endpoint for SQL on-Demand, meaning you can run SQL queries as if you were talking to any SQL Server or Azure SQL Database. It's therefore possible to use a standard <a href="https://docs.microsoft.com/en-us/azure/data-factory/copy-activity-overview">Copy Activity</a> in the same way as you would were you to copy data from <a href="https://docs.microsoft.com/en-us/azure/data-factory/connector-sql-server">a Azure SQL Database</a>. The TDS endpoint can be found on the workspace overview tab of your Synapse workspace and is in the format <code><workspace-name>-ondemand.sql.azuresynapse.net</code>. Note that you will be constrained by the language features available with SQL on-Demand. In the future, it is likely that there will be tighter workspace integration along with stored procedure support. This means that you will be able to take advantage of SQL on-Demand features such as <a href="https://docs.microsoft.com/en-us/azure/synapse-analytics/sql/develop-tables-cetas">CETAS</a>. |
