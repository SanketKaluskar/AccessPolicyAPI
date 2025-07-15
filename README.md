# AccessPolicyAPI

## Overview
AccessPolicyAPI is a .NET 9 Web API project designed to demonstrate fine-grained authorization using both Role-Based Access Control (RBAC) and Attribute-Based Access Control (ABAC). 
The API secures access to project resources by evaluating user roles and project-specific attributes.

## Authorization Model

### Role-Based Access Control (RBAC)
RBAC restricts access based on the roles assigned to users. In this project, roles are extracted from user claims and checked against allowed roles for each project. 
Example roles might include "Admin" or "User" for a given project.

### Attribute-Based Access Control (ABAC)
ABAC extends authorization by evaluating a wide range of attributes extracted from the access token (JWT) and runtime context. 
These attributes can include user properties (e.g., assigned project, department) as well as environmental conditions like time of day, location, and device posture. 
Access decisions are made dynamically based on these attributes, enabling context-aware policies that go beyond static roles.

### Combined RBAC & ABAC Enforcement
Authorization is enforced using custom policies and handlers:

- **Policies**: Defined in `Program.cs` (e.g., `JuhuAdminPolicy`, `JuhuUserPolicy`), requirements handled by `ProjectAndRoleHandler`.
- **Handler**: `ProjectAndRoleHandler` evaluates both role and project claims using `IAccessPolicyService.AccessCheck` implemented by `ConfigDrivenAccessPolicyService`.
- **Claims Used**: `Role`, `Project`, and `Scope`. `Scope` is an actor claim while `Role` and `Project` are Subject claims.

Example flow:
1. User requests a secure resource (e.g., `GET /api/secure/files`).
2. The API checks the policy requirements, which includes the user's claims for project, role and client app's consented scope.
3. Access is granted only if the role is any of the allowed roles and project, scope exactly match those required.

## Claims Mapping in Azure AD
AccessPolicyAPI uses Azure AD **claims mapping policies** to inject project-specific attributes directly into JWT access tokens. This eliminates the need for runtime directory lookups.

### How It Works
- An **Azure AD schema extension** (e.g., `ext6ed5rv6m_sankalDirectoryExtproject.project`) is used to assign a `"project"` value to a user (e.g., `"Juhu"`).
- A **claims mapping policy** is created to include this extension attribute as a custom JWT claim.
- The policy is attached to the resource app (`AccessPolicyAPI`) so every token issued to it for a user includes the claim:
  
  ```json
  "project": "Juhu"
  ```

### Real-World Use Case
When a user begins working on a specific project, a tenant admin (or provisioning script) updates their Azure AD profile with the project assignment. 
This assignment flows into every access token issued to the API, enabling attribute-based authorization without additional API or Graph calls.

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