﻿Feature: Display the Order Summary in an Authority Section
    As an Authority User
    I want to be able to view the summary for a given order
    So that I can ensure that the information is complete

Background:
    Given Orders exist
        | OrderId    | Description   | OrderStatusId | Created    | LastUpdated | LastUpdatedBy                        | OrganisationId                       |
        | C000014-01 | A Description | 1             | 05/05/2020 | 09/05/2020  | a11a46f9-ce6f-448a-95c2-fde6e61c804a | 4af62b99-638c-4247-875e-965239cd0c48 |
    And the user is logged in with the Buyer role for organisation 4af62b99-638c-4247-875e-965239cd0c48

@5321
Scenario: 1. Displaying the order summary, where the order sections are complete
    When the user makes a request to retrieve the order summary with the ID C000014-01
    Then a response with status code 200 is returned
    And the order summary is returned with the following values
        | OrderId    | OrganisationId                       | Description   |
        | C000014-01 | 4af62b99-638c-4247-875e-965239cd0c48 | A Description |
    And the order Summary Sections have the following values
        | Id                   | Status     |
        | description          | complete   |
        | ordering-party       | incomplete |
        | supplier             | incomplete |
        | commencement-date    | incomplete |
        | associated-services  | incomplete |
        | service-recipients   | incomplete |
        | catalogue-solutions  | incomplete |
        | additional-services  | incomplete |
        | funding-source       | incomplete |

@5321
Scenario: 2. If the order ID does not exist, return not found
	When the user makes a request to retrieve the order summary with the ID INVALID
	Then a response with status code 404 is returned

@5321
Scenario: 3. If a user is not authorised then they cannot access the order summary
	Given no user is logged in
	When the user makes a request to retrieve the order summary with the ID C000014-01
	Then a response with status code 401 is returned

@5321
Scenario: 4. A non buyer user cannot access the order summary
	Given the user is logged in with the Authority role for organisation 4af62b99-638c-4247-875e-965239cd0c48
	When the user makes a request to retrieve the order summary with the ID C000014-01
	Then a response with status code 403 is returned

@5321
Scenario: 5. A buyer user cannot access the order summary for an organisation they don't belong to
    Given the user is logged in with the Buyer role for organisation e6ea864e-ef1b-41aa-a4d5-04fc6fce0933
    When the user makes a request to retrieve the order summary with the ID C000014-01
    Then a response with status code 403 is returned

@5321
Scenario: 6. Service Failure
    Given the call to the database will fail
    When the user makes a request to retrieve the order summary with the ID C000014-01
    Then a response with status code 500 is returned