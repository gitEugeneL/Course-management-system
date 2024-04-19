# Course-management-system

This is a rest api service for course management at the university.

The project implements minimal api with api versioning, repository pattern, custom JWT authorization, fluent validation, integration and unit tests.


## 👷 Frameworks, Libraries and Technologies

- [xUnit](https://github.com/xunit/xunit)
- [.NET 8](https://github.com/dotnet/core)
- [ASP.NET Core 8](https://github.com/dotnet/aspnetcore)
- [Entity Framework Core 8](https://github.com/dotnet/efcore)
- [PostgreSQL](https://github.com/postgres)
- [FluentValidation](https://github.com/FluentValidation/FluentValidation)
- [IdentityModel](https://github.com/IdentityModel)
- [Asp.Versioning](https://github.com/dotnet/aspnet-api-versioning)
- [Swashbuckle](https://github.com/domaindrivendev/Swashbuckle.AspNetCore)
- [Docker](https://github.com/docker)


## 🩺 How to run tests

*Allows you to run all integration and unit tests.*
   ```sh
   > dotnet test  # donet SKD is required
   ```

## 🐳 List of docker containers

- **api.app** - container for all application layers

- **api.database** - postgresql database container

## 🚜 How to run the server

1. Build and start Docker images based on the configuration defined in the docker-compose.yml

   ```sh
   > make up  # docker-compose up --build
   ```

2. Stop and remove containers

   ```sh
   > make down  # docker-compose down
   ```

## 🔐 Local access

| container    | port | login   | password | GUI                                      |
|--------------|------|---------|----------|------------------------------------------|
| api.database | 5432 | user    | password | -                                        |
| api.app      | 8000 | -       | -        | http://localhost:8000/swagger/index.html |    

## 🖨️ Swagger documentation

1. Swagger UI

        http://localhost:8000/swagger/index.html

2. Swagger static file

        [swagger static file](https://github.com/gitEugeneL/Course-management-system/blob/dev/swagger.json)


## 🔧 Implementation features

### Authentication

*Authentication is implemented using a JWT access token and refresh token.*

*AccessToken is used to authorize users, the refresh token is used to update a pair of tokens.*

*RefreshToken is recorded in the database and allows each user to have 5 active devices at the same time.*

#### Register

<details>
<summary>
    <code>POST</code> <code><b>/api/v1/auth/register</b></code><code>(allows you to register)</code>
</summary>

##### Body
> | name             | type         | data type    |                                                           
> |------------------|--------------|--------------|
> | email            | required     | string       |
> | password         | required     | string       |
> | firstName        | not required | string       |
> | lastName         | not required | string       |
> | universityNumber | not required | string       |

##### Responses
> | http code | content-type       | response                                                                                                                                           |
> |-----------|--------------------|----------------------------------------------------------------------------------------------------------------------------------------------------|
> | `201`     | `application/json` | `{"userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6", "email": "string", "firstName": "string", "lastName": "string", "universityNumber": "string"}` |
> | `400`     | `application/json` | `array`                                                                                                                                            | 
> | `409`     | `application/json` | `string`                                                                                                                                           |
</details>

#### Login
<details>
<summary>
    <code>POST</code> <code><b>/api/v1/auth/register</b></code><code>(allows you to login, issues accessToken and refreshToken)</code>
</summary>

##### Body
> | name     | type       | data type    |                                                           
> |----------|------------|--------------|
> | email    | required   | string       |
> | password | required   | string       |


##### Responses
> | http code | content-type       | response                                                                                                                              |
> |-----------|--------------------|---------------------------------------------------------------------------------------------------------------------------------------|
> | `200`     | `application/json` | `{"accessTokenType": "string", "accessToken": "string", "refreshToken": "string", "refreshTokenExpires": "2024-04-19T18:14:59.908Z"}` |
> | `400`     | `application/json` | `array`                                                                                                                               |
> | `404`     | `application/json` | `string`                                                                                                                              |
</details>

#### Refresh

<details>
<summary>
    <code>POST</code> <code><b>/api/v1/auth/refresh</b></code><code>(allows to refresh access and refresh tokens)</code>
</summary>

##### Body
> | name            | type       | data type    |                                                           
> |-----------------|------------|--------------|
> | "refreshToken"  | required   | string       |

##### Responses
> | http code | content-type       | response                                                                                                                              |
> |-----------|--------------------|---------------------------------------------------------------------------------------------------------------------------------------|
> | `200`     | `application/json` | `{"accessTokenType": "string", "accessToken": "string", "refreshToken": "string", "refreshTokenExpires": "2024-04-19T18:14:59.908Z"}` |
> | `401`     | `application/json` | `string`                                                                                                                              |
</details>

#### Logout

<details>
<summary>
    <code>POST</code> <code><b>/api/v1/auth/refresh</b></code><code>(allows to logout and deactivates refresh token)</code>
</summary>

##### Body
> | name            | type       | data type    |                                                           
> |-----------------|------------|--------------|
> | "refreshToken"  | required   | string       |

##### Responses
> | http code | content-type       | response    |
> |-----------|--------------------|-------------|
> | `204`     | `application/json` | `NoContent` |
> | `401`     | `application/json` | `string`    |
</details>

### Course

*Functionality that allows to manage and interact with courses*

#### Create new course (*Token required*, 🔒professor auth policy)

<details>
<summary>
    <code>POST</code> <code><b>/api/v1/courses</b></code><code>(allows to create new course 🔒️[professor auth policy])</code>
</summary>

##### Body
> | name              | type       | data type |                                                           
> |-------------------|------------|-----------|
> | "name"            | required   | string    |
> | "description"     | required   | string    |
> | "maxParticipants" | required   | int       |

##### Responses
> | http code | content-type       | response                                                                                                                                                                                                                                                      |
> |-----------|--------------------|---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
> | `201`     | `application/json` | `{"courseId": "3fa85f64-5717-4562-b3fc-2c963f66afa6","ownerId": "3fa85f64-5717-4562-b3fc-2c963f66afa6", "name": "string", "description": "string", "maxParticipants": 0, "countParticipants": 0, "finalized": true, "createdAt": "2024-04-19T18:37:43.448Z"}` |
> | `400`     | `application/json` | `array`                                                                                                                                                                                                                                                       |
> | `401`     | `application/json` | `string`                                                                                                                                                                                                                                                      |
> | `403`     | `application/json` | `string`                                                                                                                                                                                                                                                      |
> | `409`     | `application/json` | `string`                                                                                                                                                                                                                                                      |
</details>

#### Update courses (*Token required*, 🔒professor auth policy)
<details>
<summary>
    <code>PUT</code> <code><b>/api/v1/courses</b></code><code>(allows to update your courses 🔒️[professor auth policy])</code>
</summary>

##### Body
> | name              | type       | data type |                                                           
> |-------------------|------------|-----------|
> | "courseId"        | required   | uuid      |
> | "description"     | required   | string    |
> | "maxParticipants" | required   | int       |
> | "finalize"        | required   | boolean   |

##### Responses
> | http code | content-type       | response                                                                                                                                                                                                                                                       |
> |-----------|--------------------|----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
> | `200`     | `application/json` | `{"courseId": "3fa85f64-5717-4562-b3fc-2c963f66afa6", "ownerId": "3fa85f64-5717-4562-b3fc-2c963f66afa6", "name": "string", "description": "string", "maxParticipants": 0, "countParticipants": 0, "finalized": true, "createdAt": "2024-04-19T18:50:26.257Z"}` |
> | `400`     | `application/json` | `array`                                                                                                                                                                                                                                                        |
> | `401`     | `application/json` | `string`                                                                                                                                                                                                                                                       |
> | `403`     | `application/json` | `string`                                                                                                                                                                                                                                                       |
> | `404`     | `application/json` | `string`                                                                                                                                                                                                                                                       | 
</details>

#### Delete your courses (*Token required*, 🔒professor auth policy)
<details>
<summary>
    <code>PUT</code> <code><b>/api/v1/courses/{ courseName:string }</b></code><code>(allows to delete your courses 🔒️[professor auth policy])</code>
</summary>

##### Responses
> | http code | content-type       | response    |
> |-----------|--------------------|-------------|
> | `204`     | `application/json` | `NoContent` |
> | `401`     | `application/json` | `string`    |
> | `403`     | `application/json` | `string`    |
> | `404`     | `application/json` | `string`    | 
</details>

#### Join the course (*Token required*, 🔒student auth policy)
<details>
<summary>
    <code>PATCH</code> <code><b>/api/v1/courses/join/{ courseName:string }</b></code><code>(allows to join the course 🔒️[student auth policy])</code>
</summary>

##### Responses
> | http code | content-type       | response |
> |-----------|--------------------|----------|
> | `200`     | `application/json` | `stging` |
> | `401`     | `application/json` | `string` |
> | `403`     | `application/json` | `string` |
> | `404`     | `application/json` | `string` | 
> | `409`     | `application/json` | `string` |
</details>

#### Leave the course (*Token required*, 🔒student auth policy)
<details>
<summary>
    <code>PATCH</code> <code><b>/api/v1/courses/leave/{ courseName:string }</b></code><code>(allows to leave the course 🔒️[student auth policy])</code>
</summary>

##### Responses
> | http code | content-type       | response |
> |-----------|--------------------|----------|
> | `200`     | `application/json` | `stging` |
> | `401`     | `application/json` | `string` |
> | `403`     | `application/json` | `string` |
> | `404`     | `application/json` | `string` | 
> | `409`     | `application/json` | `string` |
</details>

#### Get all courses (*Token required*, 🔒base auth policy)
<details>
<summary>
    <code>GET</code> <code><b>/api/v1/courses</b></code><code>(allows you to get all courses 🔒️[base auth policy])</code>
</summary>

##### Parameters
> | name                    | type         | data type |                                                           
> |-------------------------|--------------|-----------|
> | SortByCreated           | not required | boolean   |
> | SortByAvailableCourses  | not required | boolean   |
> | SortByMyCourses         | not required | boolean   |
> | PageNumber              | not required | int32     |
> | PageSize                | not required | int32     |

##### Responses
> | http code | content-type       | response                                                                                                                                                                                                                                                                                                                                  |
> |-----------|--------------------|-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
> | `200`     | `application/json` | `{ "items": [ { "courseId": "3fa85f64-5717-4562-b3fc-2c963f66afa6", "ownerId": "3fa85f64-5717-4562-b3fc-2c963f66afa6", "name": "string", "description": "string", "maxParticipants": 0, "countParticipants": 0, "finalized": true, "createdAt": "2024-04-19T19:04:00.291Z" } ], "pageNumber": 0, "totalPages": 0, "totalItemsCount": 0 }` |
> | `401`     | `application/json` | `string`                                                                                                                                                                                                                                                                                                                                  |
> | `403`     | `application/json` | `string`                                                                                                                                                                                                                                                                                                                                  |
</details>

#### Get one course by name (*Token required*, 🔒base auth policy)
<details>
<summary>
    <code>GET</code> <code><b>/api/v1/courses/{ courseName:string }</b></code><code>(allows you to get one course by name 🔒️[base auth policy])</code>
</summary>

##### Responses
> | http code | content-type       | response                                                                                                                                                                                                                                                         |
> |-----------|--------------------|------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
> | `200`     | `application/json` | `{"courseId": "3fa85f64-5717-4562-b3fc-2c963f66afa6", "ownerId": "3fa85f64-5717-4562-b3fc-2c963f66afa6", "name": "string", "description": "string", "maxParticipants": 0, "countParticipants": 0, "finalized": true, "createdAt": "2024-04-19T19:07:27.865Z"}`   |
> | `401`     | `application/json` | `string`                                                                                                                                                                                                                                                         |
> | `403`     | `application/json` | `string`                                                                                                                                                                                                                                                         |
> | `404`     | `application/json` | `string`                                                                                                                                                                                                                                                         |
</details>

### Participant

*Functionality that allows to manage course participants*

#### Grade students (*Token required*, 🔒professor auth policy)
<details>
<summary>
    <code>PATCH</code> <code><b>/api/v1/participants</b></code><code>(allows you to grade the student 🔒️[professor auth policy])</code>
</summary>

##### Body
> | name            | type       | data type |                                                           
> |-----------------|------------|-----------|
> | "userId"        | required   | uuid      |
> | "courseId"      | required   | uuid      |
> | "grade"         | required   | int       |
> | "professorNote" | required   | string    |

##### Responses
> | http code | content-type       | response                                                                                                                                                                                                                                                                                                                                                                                                                                                                        |
> |-----------|--------------------|---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
> | `200`     | `application/json` | `{"course": { "courseId": "3fa85f64-5717-4562-b3fc-2c963f66afa6", "ownerId": "3fa85f64-5717-4562-b3fc-2c963f66afa6", "name": "string", "description": "string", "maxParticipants": 0, "countParticipants": 0, "finalized": true, "createdAt": "2024-04-19T19:40:56.771Z" }, "user": { "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6", "email": "string", "firstName": "string", "lastName": "string", "universityNumber": "string" }, "grade": 0, "professorNote": "string"}` |
> | `401`     | `application/json` | `string`                                                                                                                                                                                                                                                                                                                                                                                                                                                                        |
> | `403`     | `application/json` | `string`                                                                                                                                                                                                                                                                                                                                                                                                                                                                        |
> | `404`     | `application/json` | `string`                                                                                                                                                                                                                                                                                                                                                                                                                                                                        |
</details>


#### Get all participants by course name (*Token required*, 🔒professor auth policy)
<details>
<summary>
    <code>GET</code> <code><b>/api/v1/participants/{ courseName:string }</b></code><code>(allows you to grade the student 🔒️[professor auth policy])</code>
</summary>

##### Parameters
> | name                    | type         | data type |                                                           
> |-------------------------|--------------|-----------|
> | PageNumber              | not required | int32     |
> | PageSize                | not required | int32     |

##### Responses
> | http code | content-type       | response                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  |
> |-----------|--------------------|-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
> | `200`     | `application/json` | `{"items": [ { "course": { "courseId": "3fa85f64-5717-4562-b3fc-2c963f66afa6", "ownerId": "3fa85f64-5717-4562-b3fc-2c963f66afa6", "name": "string", "description": "string", "maxParticipants": 0, "countParticipants": 0, "finalized": true, "createdAt": "2024-04-19T19:50:54.089Z" }, "user": { "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6", "email": "string", "firstName": "string", "lastName": "string", "universityNumber": "string" } ,"grade": 0, "professorNote": "string" } ], "pageNumber": 0, "totalPages": 0, "totalItemsCount": 0 }` |
> | `401`     | `application/json` | `string`                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  |
> | `403`     | `application/json` | `string`                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  |
> | `404`     | `application/json` | `string`                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  |
</details>

#### Get all your participants and courses (*Token required*, 🔒student auth policy)
<details>
<summary>
    <code>GET</code> <code><b>/api/v1/participants</b></code><code>(allows you to get all your participants and courses 🔒️[student auth policy])</code>
</summary>

##### Responses
> | http code | content-type       | response                                                                                                                                                                                                                                                                                                                                                                                                                                                                          |
> |-----------|--------------------|-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
> | `200`     | `application/json` | `{{"course": { "courseId": "3fa85f64-5717-4562-b3fc-2c963f66afa6", "ownerId": "3fa85f64-5717-4562-b3fc-2c963f66afa6", "name": "string", "description": "string", "maxParticipants": 0, "countParticipants": 0, "finalized": true, "createdAt": "2024-04-19T19:43:59.773Z" }, "user": { "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6", "email": "string", "firstName": "string", "lastName": "string", "universityNumber": "string" }, "grade": 0, "professorNote": "string"}}` |
> | `401`     | `application/json` | `string`                                                                                                                                                                                                                                                                                                                                                                                                                                                                          |
> | `403`     | `application/json` | `string`                                                                                                                                                                                                                                                                                                                                                                                                                                                                          |
</details>