﻿Feature: Display the Order Summary in an Authority Section
    As an Authority User
    I want to be able to view the summary for a given order
    So that I can ensure that the information is complete

Background:
    Given Orders exist
        | Description                | OrganisationId                       |
        | A Description for OrderId1 | 4af62b99-638c-4247-875e-965239cd0c48 |
    And the user is logged in with the Buyer role for organisation 4af62b99-638c-4247-875e-965239cd0c48

@5321
Scenario: 1. Get the order summary
    When the user makes a request to retrieve the order summary for the order with the description A Description for OrderId1
    Then a response with status code 200 is returned
    And the order summary is returned with the following values
        | OrganisationId                       | Description                |
        | 4af62b99-638c-4247-875e-965239cd0c48 | A Description for OrderId1 |
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

@4619
Scenario: 2. Get the order summary when the order has a primary ordering party contact
    Given Contacts exist
        | FirstName | LastName | EmailAddress            | TelephoneNumber |
        | Fred      | Robinson | Fred.robinson@email.com | 12312543212     |
    And Orders exist
        | Description                | OrganisationId                       | OrganisationContactEmail |
        | A Description for OrderId2 | 4af62b99-638c-4247-875e-965239cd0c48 | Fred.robinson@email.com  |
    When the user makes a request to retrieve the order summary for the order with the description A Description for OrderId2
    Then a response with status code 200 is returned
    And the order summary is returned with the following values
        | OrganisationId                       | Description                |
        | 4af62b99-638c-4247-875e-965239cd0c48 | A Description for OrderId2 |
    And the order Summary Sections have the following values
        | Id                  | Status     |
        | description         | complete   |
        | ordering-party      | complete   |
        | supplier            | incomplete |
        | commencement-date   | incomplete |
        | associated-services | incomplete |
        | service-recipients  | incomplete |
        | catalogue-solutions | incomplete |
        | additional-services | incomplete |
        | funding-source      | incomplete |

@4619
Scenario: 3. Get the order summary when the order has a primary supplier contact
    Given Contacts exist
        | FirstName | LastName | EmailAddress            | TelephoneNumber |
        | Fred      | Robinson | Fred.robinson@email.com | 12312543212     |
    And Orders exist
        | Description                | OrganisationId                       | SupplierContactEmail    |
        | A Description for OrderId2 | 4af62b99-638c-4247-875e-965239cd0c48 | Fred.robinson@email.com |
    When the user makes a request to retrieve the order summary for the order with the description A Description for OrderId2
    Then a response with status code 200 is returned
    And the order summary is returned with the following values
        | OrganisationId                       | Description                |
        | 4af62b99-638c-4247-875e-965239cd0c48 | A Description for OrderId2 |
    And the order Summary Sections have the following values
        | Id                  | Status     |
        | description         | complete   |
        | ordering-party      | incomplete |
        | supplier            | complete   |
        | commencement-date   | incomplete |
        | associated-services | incomplete |
        | service-recipients  | incomplete |
        | catalogue-solutions | incomplete |
        | additional-services | incomplete |
        | funding-source      | incomplete |

@4619
@ignore
Scenario: 4. Get the order summary when the order has a commencement date
    Given Orders exist
        | OrderId    | Description   | OrganisationId                       | CommencementDate |
        | C000015-01 | A Description | 4af62b99-638c-4247-875e-965239cd0c48 | 31/05/2020       |
    When the user makes a request to retrieve the order summary with the ID C000015-01
    Then a response with status code 200 is returned
    And the order summary is returned with the following values
        | OrderId    | OrganisationId                       | Description   |
        | C000015-01 | 4af62b99-638c-4247-875e-965239cd0c48 | A Description |
    And the order Summary Sections have the following values
        | Id                  | Status     |
        | description         | complete   |
        | ordering-party      | incomplete |
        | supplier            | incomplete |
        | commencement-date   | complete   |
        | associated-services | incomplete |
        | service-recipients  | incomplete |
        | catalogue-solutions | incomplete |
        | additional-services | incomplete |
        | funding-source      | incomplete |

@5321
Scenario: 5. If the order ID does not exist, return not found
	When the user makes a request to retrieve the order summary with the ID INVALID
	Then a response with status code 404 is returned

@5321
Scenario: 6. If a user is not authorised then they cannot access the order summary
	Given no user is logged in
	When the user makes a request to retrieve the order summary with the ID C000014-01
	Then a response with status code 401 is returned

@5321
Scenario: 7. A non buyer user cannot access the order summary
	Given the user is logged in with the Authority role for organisation 4af62b99-638c-4247-875e-965239cd0c48
	When the user makes a request to retrieve the order summary with the ID C000014-01
	Then a response with status code 403 is returned

@5321
Scenario: 8. A buyer user cannot access the order summary for an organisation they don't belong to
    Given the user is logged in with the Buyer role for organisation e6ea864e-ef1b-41aa-a4d5-04fc6fce0933
    When the user makes a request to retrieve the order summary with the ID C000014-01
    Then a response with status code 403 is returned

@5321
Scenario: 9. Service Failure
    Given the call to the database will fail
    When the user makes a request to retrieve the order summary with the ID C000014-01
    Then a response with status code 500 is returned
