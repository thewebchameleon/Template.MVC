


# Template.MVC
Serving as a template for **small self-contained business applications**, this template strives to be fast, secure and easy to understand.

## Architecture

 - Uses MVC 6 with the latest version of Visual Studio 2019 and [ASP.NET Core 3](https://asp.net). 

### Database
- Database project targets SQL Server 2017 and uses the micro ORM [Dapper](https://github.com/StackExchange/Dapper). 
- Initial roll out script `V1.sql` is included and contains lookup data and an admin user. 
- Tables contain a soft-delete metadata column `Is_Deleted` to allow foreign key integrity. 
- Stored procedures are used to perform CRUD-like operations on the database.


## Features
### Authentication
- Cookie authentication (cookies are not persisted for added security).
- Passwords are hashed using [BCrypt]([https://github.com/BcryptNet/bcrypt.net](https://github.com/BcryptNet/bcrypt.net)).
- Users are locked out after a configurable amount of invalid attempts.
- All form posts are marked with an `ValidateAntiForgeryToken` attribute.

### Sessions
- Sessions are recorded to the database and be viewed in detail.
- Session logs are recorded for each `GET` and `POST` request (sensitive data can be obfuscated).
- Session log events are high level actions that users may perform and may be useful for tracking / auditing user behavior.

### Users and Roles
- Users can register, login and update their profile.
- Roles are a grouping of permissions assigned to users. 
- A user can be assigned 1 or more roles.

### Permissions
- Permissions are access rights assigned to roles allowing access to otherwise restricted areas of the application.
- They can be created and updated in admin.

### Admin
- Various pages that control the following features
	- Users (`create`, `update`, `unlock`, `disable`, `enable`)
	- Roles (`create`, `update`, `disable`, `enable`)
	- Permissions (`create`, `update`)
	- Configuration (`create`, `update`)
	- Session Events (`create`, `update`)
