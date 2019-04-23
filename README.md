# Template.MVC
Intended for building **small self-contained business applications**, this template strives to be fast, secure and easy to understand

## Architecture
 - N-tier application with a focus on seperation of concerns
 - Uses MVC 6 with the latest version of Visual Studio 2019 and [ASP.NET Core 3](https://asp.net)
 - UI validation is shared with backend validation (client-side can only perform basic rules)

### Database
- Database project targets Microsoft SQL Server 2017 and uses the micro ORM [Dapper](https://github.com/StackExchange/Dapper)
- Initial roll out script `V1.sql` is included and contains lookup data and an admin user
- Tables contain a soft-delete metadata column `Is_Deleted` to allow foreign key integrity
- Stored procedures are used to perform CRUD-like operations on the database.
- Connecting to a MySQL database is supported

### Backend
- Uses the Request / Response pattern
- Each view has a dedicated `ViewModel`
- Business logic is contained within the Service layer
- Caching is used for lookup data via an `ICacheProvider`

### UI
- [JQuery](https://jquery.com/)
- [JQuery DataTables](https://datatables.net/)
- [Bootstrap 4](https://getbootstrap.com/)
- Custom tag helpers
	- [Multiselect](https://developer.snapappointments.com/bootstrap-select/) dropdown (`MultiselectTagHelper.cs`)
	- Authorization attribute (`AuthorizationTagHelper.cs`)

## Features
### Security
- Cookie authentication using authorization with permissions
	- Session / authentication cookies are **not** stored on the user's machine
- Passwords are hashed using [BCrypt](https://github.com/BcryptNet/bcrypt.net)
- Users are locked out after a configurable amount of invalid attempts
- All form posts are marked with a `[ValidateAntiForgeryToken]` attribute

### Sessions
- Custom session logging implementation which is recorded to the database
- Sessions can be viewed in detail on the `Sessions` page
- Session logs are recorded for each `GET` and `POST` request and include form data (sensitive data can be obfuscated)
- Session log events are high level actions that users may perform and may be useful for tracking / auditing user behavior

### Users, Roles, Permissions
- Users can register, login and update their profile.
- Users can perform a forgot password request and reset their password via an email containing an activation link
- Roles are a grouping of permissions assigned to users
- Permissions are access rights assigned to roles allowing access to otherwise restricted areas of the application

### Configuratation
- Configuration items are used to control various aspects of the application ranging from features to core settings
- Stored as strong MSSQL types (`boolean`, `datetime`, `date`, `time`, `decimal`, `int`, `money`, `string`)

### Admin
- Users can be created, updated, enabled and disabled
- Roles can be created, updated, enabled and disabled
- Permissions can be created and updated
- Configuration items can be created and updated
- Session log events can be created and updated
