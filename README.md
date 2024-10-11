# Database Used :
SQL Server

# To run the Web API
a. Change the connection string in the appsettings.json

``` 

"ConnectionStrings": {
    "DefaultConnection": "Server=yourservername; Database=DbName;TrustServerCertificate=True;Trusted_Connection=True",
  },
  
  ```

b. apply migration 

    update-database
    
c. run the program
