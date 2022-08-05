# BBlog Test Report

## Manual test scenarios

Manual test cases were defined in ***.feature*** files (together with automated tests)
> they are marked with tag ***@Manual***
>
> feature files are located in dir **'Features'**: [Articles](../Features/Articles.feature), [Comments](../Features/Comments.feature)

## The issues found

### Bugs

1. Follow profile does not work (API level)
    - Preconditions: at least 2 users exist in the system; not logged in user created an article
    - Steps:
        - Log in to the app
        - Open an article created by another user
        - Click 'Follow {userName}'
    - Result:
        - After reentering the user's article it's visible that user is not followed
    - Expected result:
        - User should be followed, their articles should be shown on 'Your Feed' tab
    - Problem source: API (POST /api/profiles/:username/follow)

2. The default image is not loaded in 'Post comment' section (UI)
    - Preconditions: user does not have a defined profile picture, any article exists
    - Steps:
        - Log in to the app
        - Open an article
    - Result:
        - img element ***.comment-author-img*** does not have defined src attribute
    - Expected result
        - img element ***.comment-author-img*** should have defined = src="https://static.productionready.io/images/smiley-cyrus.jpg"

3. Marking article as favorite does not work (API level)
    - Preconditions: article created by another user exists
    - Steps:
        - Log in to the app
        - Open an article created by another user
        - Click 'Favorite article'
        - Go to profile page
        - Open 'Favorited Posts' tab
    - Result:
        - No favorited articles
    - Expected result:
        - Article marked as favorite should be shown in the 'Favorited Posts' tab on profile page
    - Problem source: API (POST /api/articles/:slug/favorite)

4. An empty comment can be added (UI level/API level)
    - Preconditions: any article exists
    - Steps:
        - Log in to the app
        - Open an article
        - Post a comment without filling the comment content 
    - Result:
        - The comment is created
    - Expected result
        - The body value should be validated on UI/API level to not be empty, the comment should not be created

5. An empty article can be added - without title, description and body (API/UI level)
    - Preconditions: user is logged in
    - Steps:
        - Start creating a new article
        - Post an article without filling any field
    - Result:
        - The article is created
    - Expected result
        - Body/title/description value should be validated on UI/API level to not be empty, the article should not be created

6. Fetching comments does not work (API level)
    - Preconditions: a commented article exists
    - Steps:
        - Open the commented article
    - Result:
        - Comments aren't shown
    - Expected result:
        - Comments should be shown
    - Problem source: API (GET /api/articles/:slug/comments)


8. Error when null title of article is passed (API level)
    - Result
        - StatusCode 500, error cannot read toString of null
    - Expected result
        - StatusCode 422, {title:"can't be empty")


### Other findings
#### Functional
1. Article filtering improvements (UI)
    - API supports article filtering that is not utilized on Home page (e.g. filtering by Author)

#### Accessibility/UX

1. Contrast of elements - the page is hard to read for people with vision disabilities (e.g. nav bar)
2. Skip to content link - there should be a link that allows skipping repetitive page elements like the navigation bar
3. New Article view - fields have only placeholders, no labels
4. Redirection after article removal -> Home/Your Feed is shown after the removal. User should see the previous page after the article deletion
5. Logout button location is strange and unintuitive

## Why did you choose the test cases to automate?

- The core functionality of the app is to provide a way of sharing posts among registered and unregistered users -> that's why automation was focused on validating the option of creating and accessing posts via authorized as well as unauthorized actors (unauthorized -> only reading posts)
- Derivative functionalities are the modification and deletion of texts. These options would not be in the scope of MVP -> that's why they were lower in the automation priority
- Following and marking articles as favorite are as well less important. When we consider the essential value delivered by the product -> their automation should be done later in the development cycle. But having them would be beneficial because these functions haven't passed tests and automation would speed up the feedback loop in case of retesting fixes
- Automation was focused on happy path scenarios -> covering more complex scenarios is time-consuming and the investment might not be justified

## Additional information e.g. risks, recommendations, approach, etc.

### Approach and risks

- UI tests use the API layer to create precondition ('One way E2E')
    - this approach might be problematic if API would be broke and -> context for a test case scenario won't be prepared.
    - In the case of web apps, it's still possible to test UI even if API has bugs - http call can be intercepted and a valid response could be provided. This workaround was applied in test ***'Dev team member accesses an article from YourFeed'***
    - Test isolation has to be considered e.g. it is possible to update a username
        - if tests aren't properly isolated if one test changes the username of the author, and another test tries to get articles of the modified author, the test will fail false negatively 
            - it is more problematic when tests running concurrently
- Test coverage risks:
    - because of API bugs, not all paths were able to test in an E2E manner e.g. removing another user's comments

