Feature: ContentFragmentExtensibility

Background:

  Given the following markdown files
    | document             | file                    |
    | Blog with extensions | blog-with-extensions.md |
  Given the following html files
    | document             | file                      |
    | Blog with extensions | blog-with-extensions.html |
  Given the following content blocks
    | ContentType                             | Id    | SpecPath                      |
    | application/vnd.vellum.content.blogs+md | Blogs | ../../blog-with-extensions.md |
  Given the "Blogs" ContentBlocks
  And the "Blog with extensions" document
  And we Create a Content Fragment

Scenario: A markdown file with extensions gets converted into a Content Fragment with extensions
  Then Content Fragment should contain
    | ContentType                             | Date                 | Hash                                                             | Id    | Position | PublicationStatus | BodyHtmlDocumentName |
    | application/vnd.vellum.content.blogs+md | 11/5/2022 6:30:00 AM | 58754666f4d7b6578f70dea23f2a24a3aa2d6771a91e8baed08874501557e9b2 | Blogs | 0        | Published         | Blog with extensions |
  And the Content Fragment should contain the following Extensions:
    | ContentType                                 |
    | application/vnd.vellum.content.series+md    |
    | application/vnd.vellum.content.promotion+md |

Scenario: A Content Fragment with extensions generates a new dynamic extension type
  When we do something