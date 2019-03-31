

# Template.MVC
Serving as a template for **small self-contained business applications**, this template strives to be fast, secure and easy to understand.

## Architecture

 - Uses the latest version of Visual Studio 2019 and [ASP.NET Core 3](https://asp.net). 

## Database
- Database project targets SQL Server 2017 and uses the micro ORM [Dapper](https://github.com/StackExchange/Dapper). 
- Initial roll out script `V1.sql` is included and contains lookup data and an admin user. 
- Tables contain a soft-delete metadata column `Is_Deleted` to allow foreign key integrity. 
- Stored procedures are used to perform CRUD operations on the database.


## Features
### Authentication
- Cookie authentication has been used and cookies are not persisted by design.

### Sessions
- Sessions are recorded to the database. 
- Page tracking can be toggled and includes user input (can be obfuscated to mask any sensitive data).
- Session events are high level actions that users may perform. 
- Sessions can be viewed and give a detailed breakdown of what transpired

### Users
- Users can register, login and update their profile.

### Roles
- Roles are a grouping of permissions assigned to users. 
- A user can be assigned 1 or more roles.

### Permissions
- Permissions are access rights assigned to roles allowing access to otherwise restricted areas of the application.
- They can be created and updated via an admin page.
- They are hard-coded and therefore cannot be deleted once implemented.

### Admin
- Various pages that control the following features
	- Users (`create`, `update`, `unlock`, `disable`, `enable`)
	- Roles (`create`, `update`, `disable`, `enable`)
	- Permissions (`create`, `update`)
	- Configuration (`create`, `update`)
	- Session Events (`create`, `update`)
