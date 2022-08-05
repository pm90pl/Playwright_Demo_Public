Feature: Articles

Articles are used by the dev team members to share and gain knowledge related to using Angular in front-end development. 
An unauthorized user is able only to browse notes when authorized actors have additional privileges to:
-create new items
-markdown formatting can be used during the article creation


#________Manual scenarios________#
#Create
@Manual
@E2E
Scenario: Priviliges to create an article
	When Unauthorized user accessed the app
	Then the user should not be able to create a new article

@Manual
@API
Scenario: Privileges to create an article via API
	When Unauthorized API user creates a new article
	Then the article creation should fail with Unauthorized access error

@Manual
@E2E
Scenario: Creating article with tags
	Given Authorized user started new article creation
	And tags were added
	When the user publishes the article
	Then the tags should be marking the article

#Read
@Manual
Scenario: Markdown article content is rendered properly
	Given an article with the body using markdown syntax was created
	When an user opens the markdown formatted article
	Then the article should be displayed with markdown formatting properly rendered
#________Manual scenarios________#


#Creation
@UITest
Scenario: Dev team member creates an article
	Given Authorized user has an article ready to be published
	When the user publishes the article
	Then the article should be accessible to the user from their profile page

@APITest
Scenario: API client creates new article
	When Authorized API client creates new article
	Then the article should be accessible to the API user

@APITest
#Required fields: title, description, body 
#Optional fields: tagList
Scenario Outline: API client new article creation validation
	When Authorized API client creates new article with null <Property>
	Then the article should not be created

Examples:
	| Property |
	| Title    |
	#| Description |
	#| Body     |
	#!!!Can create an article without description and body via API - that's why they were commented out



@UITest
Scenario Outline: Dev team member accesses an article from GlobalFeed
	Given an article was created
	When <User> user opens Global Feed
	Then the article should be accessible to the user

Examples:
	| User         |
	| Authorized   |
	| Unauthorized |

@UITest
Scenario: Dev team member accesses an article from YourFeed
	Given an Authorized user followed an article author
	When Authorized user opens Your Feed
	Then the article should be accessible to the user



#Delete
@UITest
Scenario Outline: Dev team member removes their article
	Given an Authorized user opened their article
	When the user deletes the article
	Then the article should not be accessible from the user's profile page
