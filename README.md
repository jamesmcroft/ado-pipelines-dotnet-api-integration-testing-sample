# .NET Web API Integration Testing with Docker Dependencies in Azure DevOps Pipelines

This sample demonstrates how to [use Docker Containers for local development environments](https://ruanmartinelli.com/posts/docker-compose-local-dev-environment/) as dependencies for [running integration tests in Azure DevOps Pipelines](https://learn.microsoft.com/en-us/azure/devops/pipelines/ecosystems/dotnet-core?view=azure-devops&tabs=yaml-editor#run-your-tests).

The sample project is a simple .NET 8 minimal Web API project that uses a SQL Server, as a local development container, to store and retrieve data. This is achieved using Entity Framework Core.

The integration tests are run against the Web API project [using the `WebApplicationFactory` class](https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-8.0). This approach allows the test runner to build and host the Web API and run the integration tests against it.

> [!NOTE]
> For the Azure DevOps pipelines, this sample only covers spinning up the Docker containers and running the tests on Microsoft-hosted agents. The build and deployment of the Web API project is not covered.

## Components

The sample project is composed of the following components:

- [.NET 8 Minimal Web API project](./src/Sample.API/), which contains 3 simple endpoints that interface with the SQL Server database via Entity Framework Core:
  - `GET /companies`: returns a list of companies from the database
  - `POST /companies`: creates a new company in the database
  - `DELETE /companies/{id}`: deletes a company from the database by its ID
- [NUnit integration tests project](./tests/Sample.API.Tests/), which contains 3 integration tests that exercise the endpoints of the Web API project:
  - `GET /companies`: tests that the endpoint returns a list of companies from the database
  - `POST /companies`: tests that the endpoint creates a new company in the database
  - `DELETE /companies/{id}`: tests that the endpoint deletes a company from the database by its ID
- [Docker Compose file](./docker-compose.yml), which defines the local development dependencies, i.e., SQL Server container
- [Azure DevOps Pipeline and re-usable templates](./build/), which define the steps to start the Docker containers and run the integration tests on Microsoft-hosted agents in Azure DevOps Pipelines

The .NET integration tests take advantage of the following open-source libraries:

- [Bogus](https://github.com/bchavez/Bogus), which is used to generate fake data for the tests.
- [Flurl](https://github.com/tmenier/Flurl), which is used to make HTTP requests to the Web API endpoints.
- [Respawn](https://github.com/jbogard/Respawn), which is used to reset the state of the database between tests.
- [Shouldly](https://github.com/shouldly/shouldly), which is used to assert the results of the tests.

## Getting Started

To run the sample project locally, you need to have [Docker](https://www.docker.com/) installed on your machine. Then, you can run the following command from the root of the repository:

```bash
docker-compose up
```

This will start a SQL Server container and a Zipkin container. The SQL Server will start at `localhost:1433` and the Zipkin at `localhost:9411`.

> [!NOTE]
> The Zipkin container is used to capture the traces from the Web API project using OpenTelemetry. It is not required to run the integration tests locally but is a useful tool when monitoring applications in a local development environment. This would be replaced by an Azure Application Insights instance in a production environment.

Once the containers are up and running, you can run the integration tests from Visual Studio or the command line:

```bash
dotnet test .\tests\Sample.API.Tests
```

This will run the integration tests, hosting the Web API using the `WebApplicationFactory` class, and using the running SQL Server container for the database. The tests will mock data to be inserted into the database and assert that the endpoints return the expected results.

### Setting up Azure DevOps Pipelines

To set up the Azure DevOps Pipelines, you need to have an Azure DevOps organization and project. You can create a free account [here](https://azure.microsoft.com/en-us/services/devops/).

Once you have an organization and project, you can create a new pipeline and select the repository where the sample project is hosted. You can then select the `Existing Azure Pipelines YAML file` option and select the [ci-api.yml](./build/ci-api.yml) file.

You can then run the pipeline and it will follow the flow described above as an automated process on Microsoft-hosted agents.
