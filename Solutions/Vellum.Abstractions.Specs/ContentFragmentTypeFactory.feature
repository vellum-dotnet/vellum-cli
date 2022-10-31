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
  