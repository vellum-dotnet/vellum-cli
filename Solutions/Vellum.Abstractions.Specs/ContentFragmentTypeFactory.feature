Feature: ContentFragmentTypeFactory

Background:

  Given the following markdown files
    | document                                       | file                                                                      |
    | How serverless is replacing the data warehouse | azure-synapse-analytics-how-serverless-is-replacing-the-data-warehouse.md |

Scenario: Converting a blog post
  Given the "How serverless is replacing the data warehouse" document
  When I press add
  Then the result should be 120 on the screen
