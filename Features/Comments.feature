Feature: Comments
Comments are used by authorized dev team members to share feedback or give additional information about the covered topic.
Comments can be read by unauthorized app users



#________Manual scenarios________#
@Manual
@UI
Scenario: Unauthorized user adds a comment
	When Unauthorized user opens an article
	Then the user should be required to log in to add comments

@Manual
@UI
Scenario: User deletes another user's comment
	Given Authorized user opened article commented by another user
	Then the user should not be able to remove another user's comment


@Manual
@UI
Scenario: User deletes their comments
	Given Authorized user opened article that commented
	Then the user should be able to remove their comments

#________Manual scenarios________#


@UITest
Scenario Outline: Dev team member accesses an article with comments added
	Given an article was commented
	When <User> opens the article
	Then the comment should be accessible to the user

Examples:
	| User         |
	| Authorized   |
	| Unauthorized |

@UITest
Scenario: Dev team member comments an article
	When Authorized user comments the article
	Then the comment should be accessible to the user

@APITest
Scenario: Unauthorized API user adds a comment
	When Unauthorized API user comments the article
	Then the comment should not be created via API


@APITest
Scenario: API user removes another user's comment
	Given an article was commented by another user
	When Authorized API user removes the comment
	Then the comment should not be removed via API

