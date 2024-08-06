Feature: CommandLineParser

Scenario: Call Root Command
	Given I have a Command Line Parser
	And I have entered 70 into the calculator
	When I press add
	Then the result should be 120 on the screen