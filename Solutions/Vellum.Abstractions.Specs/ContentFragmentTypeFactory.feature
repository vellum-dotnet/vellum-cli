Feature: ContentFragmentTypeFactory

Scenario: Converting a blog post
	Given I have a Command Line Parser
	And I have entered 60 into the calculator
	When I press add
	Then the result should be 120 on the screen
