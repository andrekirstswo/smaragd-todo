# smaragd-todo

With this app we want to prepare us for the **AZ-204** certifiction. But instead of watching videos and train braindumps we will create an app with, in our eyes, important patterns, to achieve an app that is **secure**, **reliable**, **scalable** (geo-redundant) and **cost-efficient**.

## The App

Our app will be a task management similiar to Trello or Assana.

### Requirements

- A `User` can register to get access to the app (*US1*)
- A `User` can create a `Board` with a `Name` (*US2*)
  - The `Board` has initially 3 `Task states` (**New**, **In progress**, **Done**)
  - The `User` who is creating the `Board` is the owner
  - *Task states are initially static - maybe later to create custom task states*
- A `User` can create a `Task` on a `Board` with `Title`, assignee (default **me**) and a optional `Description`. The initial `Task state` is **New** (*US3*)
- A `User` can change the `Title` of an `Task` (*US4*)
- A `User` can change the `Description` of an `Task` (*US5*)
- A `User` can add file attachments (*US6*)
- A `User` can invite a other or new `User` to a `Board`. Maybe via E-Mail or known username (*US7*)
- A `User` can assign `Task` to mutliple `Users` (*US8*)
- A `User` can change the `Task state` (*US9*)
- A `User` can mark the `Task` as closed (*US10*)
  - Closed `Tasks` will be automatically removed after n days (*US11*)
- Remove `User` of an `Task` (*US12*)
- A `User` can get a list of `Boards` (*US13*)
- A `User` can get the `Tasks` of a `Board` (*US14*)
  - A `User` can search for `Tasks` in `Title` and `Description` (*US15*)

## API

|Verb|Endpoint|Parameters|
|---|---|---|
|`POST`|`/user/register`|*tbd*|
|`POST`|`/board`|name: `string`|
|`GET`|`board`||
|`POST`|`/board/{BoardId}/task`|title: `string`, assignee: `UserId`, description: `string`|
|`GET`|`/board/{BoardId}/`|filter: ``string``|
|`PATCH`|`/board/{BoardId}/task/{TaskId}/title`|title: `string`|
|`PATCH`|`/board/{BoardId}/task/{TaskId}/description`|description: `string`|
|`POST`|`/board/{BoardId}/task/{TaskId}/attachment`|stream: `FileStream`|
|`POST`|`/board/{BoardId}/invite`|identifier: `string`|
|`POST`|`/board/{BoardId}/task/{TaskId}/assign/{UserId}`||
|`PATCH`|`/board/{BoardId}/task/{TaskId}/state`|state: `TaskState`|
|`POST`|`/board/{BoardId}/task/{TaskId}/close`||
|`DELETE`|`/board/{BoardId}/task/{TaskId}/user/{UserId}`||

## Events

## Pattern that should be implemented

### Must use of patterns

#### Asynchronous Request-Reply

Example: [Async-Request-Reply](https://github.com/mspnp/cloud-design-patterns/tree/main/async-request-reply)

#### Cache-Aside

Solve it with: Azure Redis Cache

#### Circuit Breaker

Solve it with: Microsoft.Extensions.Resillience

#### Competing Consumers

#### External Configuration Store

#### Federated Identity

Solve it with: Microsoft Entra?

#### Gatekeeper

#### Gateway Routing

Solve it with: Azure API Gateway

#### Geode

#### Health Endpoint Monitoring

#### Publisher-Subscriber

#### Queue-Based Load Leveling

#### Rate Limiting

#### Retry

Solve it with: Microsoft.Extensions.Resillience

### Possibly using of pattern

#### Bulkhead

#### Choreography

#### Priority Queue

#### Scheduler Agent

#### Backends for Frontends

#### Event Sourcing

#### Sharding

#### Claim-Check

## Azure Services

- Azure ComsosDb
- Azure Redis Cache
- Azure Functions
- Azure Service Bus
- Azure BlobStorage
- Azure API Gateway
- Azure App Services
- Azure Application Insights
- Azure AI Search

## CI/CD

- GitHub Actions
- Terraform