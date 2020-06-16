﻿Feature: Get all of the catalogue solutions for an order
    As a Buyer User
    I want to view all of the catalogue solutions for a given order
    So that I can ensure that the information is correct

Background:
    Given Orders exist
        | Description  | OrderStatusId | LastUpdatedBy                        | OrganisationId                       |
        | Description1 | 1             | 335392e4-4bb1-413b-9de5-36a85c9c0422 | 4af62b99-638c-4247-875e-965239cd0c48 |
    And the user is logged in with the Buyer role for organisation 4af62b99-638c-4247-875e-965239cd0c48

@4631
Scenario: 1. Get an orders catalogue solutions
    When the user makes a request to retrieve the order catalogue solutions section with the Order Description Description1
    Then a response with status code 200 is returned
    And the catalogue solutions response contains the order description Description1
    And the catalogue solutions response contains no solutions

@4631
Scenario: 2. A non existent orderId returns not found
    When the user makes a request to retrieve the order catalogue solutions section with the ID -999
    Then a response with status code 404 is returned

@4631
Scenario: 3. If a user is not authorised then they cannot access the order catalogue solutions
    Given no user is logged in
    When the user makes a request to retrieve the order catalogue solutions section with the Order Description Description1
    Then a response with status code 401 is returned

@4631
Scenario: 4. A non buyer user cannot access the order catalogue solutions
    Given the user is logged in with the Authority role for organisation 4af62b99-638c-4247-875e-965239cd0c48
    When the user makes a request to retrieve the order catalogue solutions section with the Order Description Description1
    Then a response with status code 403 is returned

@4631
Scenario: 5. A buyer user cannot access the order catalogue solutions for an organisation they don't belong to
    Given the user is logged in with the Buyer role for organisation e6ea864e-ef1b-41aa-a4d5-04fc6fce0933
    When the user makes a request to retrieve the order catalogue solutions section with the Order Description Description1
    Then a response with status code 403 is returned

@4631
Scenario: 6. Service Failure
    Given the call to the database will fail
    When the user makes a request to retrieve the order catalogue solutions section with the Order Description Description1
    Then a response with status code 500 is returned
