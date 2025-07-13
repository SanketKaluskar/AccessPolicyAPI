# AccessPolicyAPI

## Overview
AccessPolicyAPI is a .NET 9 Web API project designed to demonstrate fine-grained authorization using both Role-Based Access Control (RBAC) and Attribute-Based Access Control (ABAC). 
The API secures access to project resources by evaluating user roles and project-specific attributes.

## Authorization Model

### Role-Based Access Control (RBAC)
RBAC restricts access based on the roles assigned to users. In this project, roles are extracted from user claims and checked against allowed roles for each project. 
Example roles might include "Admin" or "User" for a given project.

### Attribute-Based Access Control (ABAC)
ABAC extends authorization by considering additional attributes, such as the user's assigned project. 
These attributes are also extracted from user claims and used to make access decisions.

### Combined RBAC & ABAC Enforcement
Authorization is enforced using custom policies and handlers:

- **Policies**: Defined in `Program.cs` (e.g., `JuhuAdminPolicy`, `JuhuUserPolicy`), requirements handled by `ProjectAndRoleHandler`.
- **Handler**: `ProjectAndRoleHandler` evaluates both role and project claims using `IAccessPolicyService.AccessCheck` implemented by `ConfigDrivenAccessPolicyService`.
- **Claims Used**: `Role`, `Project`, and `Scope`. `Scope` is a actor claim while `Role` and `Project` are Subject claims.

Example flow:
1. User requests a secure resource (e.g., `GET /api/secure/files`).
2. The API checks the policy requirements, which includes the user's claims for project, role and client app's consented scope.
3. Access is granted only if the role is any of the allowed roles and project, scope exactly match those required.

## Key Files
- `Authorization/ProjectAndRoleRequirement.cs`: Defines the custom authorization requirement.
- `Authorization/ProjectAndRoleHandler.cs`: Implements the logic for RBAC and ABAC checks.
- `Models/ProjectPolicyOptions.cs`: Configures allowed roles and required project/scope.
- `Controllers/SecureController.cs`: Example controller using the authorization policy.

## Usage
1. Configure claims to include `Role`, `Project` and `Scope`.
2. Apply authorization policies to controllers or actions.
3. Make a GET request to the API and include a JWT in the Authorization header as a Bearer token.
1. The API will automatically enforce RBAC and ABAC rules for each request.

## Technologies
- .NET 9
- ASP.NET Core Authorization
- Swagger/OpenAPI

## License
This project is provided for demonstration purposes.