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

  Given the "Blogs" ContentBlocks
  And the "How serverless is replacing the data warehouse" document
  And we Create a Content Fragment

Scenario: Converting a ContentFragment into a BlogPost using ContentFragmentTypeFactory
  Given we obtain a ContentFragmentTypeFactory for the Content Fragment Content Type
  When we create the BlogPost
  Then the BlogPost should contain
      | Title                                                                   | Slug                                                                   | Author       | Date                 | PublicationStatus | HeaderImageUrl                                                                                               | BodyHtmlDocumentName                           | Excerpt                                                                                                                                      |
      | Azure Synapse Analytics: How serverless is replacing the data warehouse | azure-synapse-analytics-how-serverless-is-replacing-the-data-warehouse | James.Broome | 7/15/2020 6:30:00 AM | Published         | /assets/images/blog/2020/07/header-azure-synapse-analytics-how-severless-is-replacing-the-data-warehouse.png | How serverless is replacing the data warehouse | Serverless data architectures enable leaner data insights and operations. How do you reap the rewards while avoiding the potential pitfalls? |