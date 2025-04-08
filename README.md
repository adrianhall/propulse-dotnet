# ProPulse

ProPulse is an enterprise-ready article publication and social media marketing web application that can be used to host an enterprise blog and market the articles via social media.

## Getting Started

### Prerequisites

- .NET 9 SDK
- PostgreSQL (or Docker for containerized database)
- Visual Studio 2025 or VS Code

### Building the Project

```bash
# Clone the repository
git clone https://github.com/yourusername/propulse-dotnet.git
cd propulse-dotnet

# Build the solution
dotnet build

# Run the tests
dotnet test
```

### Running the Application with Aspire

```bash
# Start the application using Aspire
cd src/ProPulse.AppHost
dotnet run
```

This will launch the Aspire dashboard and start all required services including the API, PostgreSQL database, and Redis cache.

## Personas

There are four personas, which translate to "Roles" within the system:

- **Reader** is a person who reads the articles.  This user may be anonymous (not signed in), but gains additional features if authenticated, including the ability to rate and comment on articles.

- **Author** writes articles and can schedule them for publication.  They can also respond to comments on their articles and see statistics on their articles (including the average ratings and the number of users who have rated an article a particular value).

- **Social Media Manager** is responsible for managing the social media feeds for the site.

- **Administrator** is responsible for assigning users to roles and doing other administrative tasks.

