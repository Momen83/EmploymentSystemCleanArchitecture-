# EmploymentSystem
This project aims to build an Employment System where employers can post job vacancies and applicants can search and apply for jobs. The system follows a Clean Architecture approach and is implemented using .NET Core with MS SQL Server as the database.

System Features
- Authentication & User Management
    - Self-registration for Employers and Applicants
    - Secure login system with JWT authentication

 -  Employer Features
    - Create, Read, Update, Delete (CRUD) vacancies
    - Set the maximum number of allowed applications per vacancy
    - Post & deactivate vacancies
    - Vacancies have an expiry date
    - View the list of applicants for a specific vacancy

 - Applicant Features
    - Search for vacancies
    - Apply for a vacancy (ensuring it does not exceed the maximum allowed applications)
    - Restriction: An applicant can apply only once per day (24 hours)

  - System Functionalities
     - Archiving mechanism for expired vacancies
     - Fully functional RESTful APIs
     - Caching & Logging for performance and debugging
  - Security best practices to protect user data
## Related Repositories
 - Front-end Repository: https://github.com/Momen83/EmploymentSystem_Front
