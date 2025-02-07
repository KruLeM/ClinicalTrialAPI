# ClinicalTrialAPI service

# MS SQL Database Setup

The service uses an **MS SQL** database.  

## DB setup Steps  

1. **Check the MS SQL server**  
   Ensure that the **MS SQL server** is running and the required database is accessible.  

2. **Configure the connection string**  
   Set up the SQL connection string in the **API project** inside the `appsettings.json` file under the property `"DBConnection"`.  

3. **Apply database migrations**  
   The initial migration has already been created. Open the **Package Manager Console**, select the **Infrastructure** project, and run the following command:  

   ```powershell
   Update-Database
<br />

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
