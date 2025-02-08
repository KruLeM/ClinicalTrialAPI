# Clinical Trial Service

## Overview

The Clinical Trial Service is a web API built using ASP.NET Core that enables users to manage clinical trials. The service provides endpoints to add new clinical trials to the database, update existing ones, and retrieve details of previously created trials.

## Features

- Add a new clinical trial
- Update an existing clinical trial
- Retrieve a list of saved clinical trials with optional pagination parameters
- Retrieve details of a specific clinical trial by `trialId`
- Retrieve a list of clinical trials filtered by `status`, with optional pagination parameters

## Technologies Used

- ASP.NET Core
- MS SQL database
- MediatR
- FluentValidation
- xUnit for unit testing
- Docker file for containerization

<br />

# Getting Started

To run the project locally:
1. Clone the GitHub repository
2. Set up an MS SQL Server instance
3. Update the connection string in the Settings file
4. Apply database migrations
5. Run the API project

#

2. **Set up an MS SQL Server instance**

- If you don't have MS SQL Server installed, you can install **SQL Server Express** from <a href="https://www.microsoft.com/en-us/sql-server/sql-server-downloads" target="_blank">Microsoft's official website</a>.
- Create a database named `ClinicalTrialDB`.
- Create an SQL user `clinical_user` with the password `SecurePassword123`.

<br />

3. **Update the connection string in the settings file**

If you created the database and user as described in Step 2, no further action is needed in this step.  
Otherwise, you need to configure the database connection string.

To update the connection string:
- Open the `API` project.
- Locate the `appsettings.json` file.
- Find the `"DBConnection"` property and update its value according to your database configuration and SQL user.

<br />

4. **Apply Database Migrations**

The initial migration has already been created in the project. Now, you just need to update the database.

To apply the migration:
- Open the **Package Manager Console**.
- Select the `Infrastructure` project.
- Run the following command:

   ```powershell
   Update-Database
   ```

#

<br /><br />

# Upload File Content

This service uses `.json` files to add or update a clinical trial.

## Example of a valid JSON file:

```json
{
   "trialId": "123e",
   "title": "Test 1",
   "startDate": "2025-03-07",
   "participants": 15,
   "status": "Not Started"
}
```
## JSON Schema
```json
{
   "$schema": "http://json-schema.org/draft-07/schema#",
   "title": "ClinicalTrialMetadata",
   "type": "object",
   "properties": {
      "trialId": { "type": "string" },
      "title": { "type": "string" },
      "startDate": { "type": "string", "format": "date" },
      "endDate": { "type": "string", "format": "date" },
      "participants": { "type": "integer", "minimum": 1 },
      "status": { "type": "string", "enum": ["Not Started", "Ongoing", "Completed"] }
   },
   "required": ["trialId", "title", "startDate", "status"],
   "additionalProperties": false
}
```

<br />


## **JSON SCHEMA**
