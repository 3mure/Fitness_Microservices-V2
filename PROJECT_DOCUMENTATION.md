# Elevate Fitness Microservices - Complete Documentation

## Table of Contents
1. [Project Overview](#project-overview)
2. [Architecture](#architecture)
3. [Progress Tracking Service](#progress-tracking-service)
4. [Docker Compose Orchestration](#docker-compose-orchestration)
5. [API Gateway (Ocelot)](#api-gateway-ocelot)
6. [Technology Stack](#technology-stack)
7. [Getting Started](#getting-started)
8. [API Endpoints](#api-endpoints)
9. [Message Broker Integration](#message-broker-integration)

---

## Project Overview

**Elevate Fitness** is a comprehensive fitness management platform built using a microservices architecture. The system enables users to track workouts, manage nutrition, monitor progress, and receive personalized fitness recommendations. The architecture follows Domain-Driven Design (DDD) principles and implements CQRS pattern using MediatR.

### Key Features
- **User Authentication & Authorization** - JWT-based secure authentication
- **Workout Management** - Create, track, and manage workout routines
- **Nutrition Planning** - Meal planning and nutritional tracking
- **Progress Tracking** - Comprehensive progress monitoring with analytics
- **Fitness Calculations** - BMI, calorie burn, and macro calculations
- **Smart Coaching** - AI-powered personalized recommendations

---

## Architecture

### Microservices Architecture

The system consists of 7 independent microservices:

```
┌─────────────────────────────────────────────────────────┐
│              Fitness API Gateway (Ocelot)                 │
│                  Port: 8088 (HTTP), 8089 (HTTPS)        │
└─────────────────────────────────────────────────────────┘
                          │
        ┌─────────────────┼─────────────────┐
        │                 │                 │
        ▼                 ▼                 ▼
┌──────────────┐  ┌──────────────┐  ┌──────────────┐
│  Auth        │  │  Workout     │  │  Nutrition   │
│  Service     │  │  Service     │  │  Service     │
│  8090/8091   │  │  8080/8081   │  │  8082/8083   │
└──────────────┘  └──────────────┘  └──────────────┘
        │                 │                 │
        └─────────────────┼─────────────────┘
                          │
        ┌─────────────────┼─────────────────┐
        │                 │                 │
        ▼                 ▼                 ▼
┌──────────────┐  ┌──────────────┐  ┌──────────────┐
│  Progress    │  │  Calculation │  │  Smart Coach │
│  Tracking    │  │  Service     │  │  Service     │
│  8086/8087   │  │  8084/8085   │  │              │
└──────────────┘  └──────────────┘  └──────────────┘
        │
        ▼
┌─────────────────────────────────────────┐
│      SQL Server Database (Port 1433)    │
│      RabbitMQ Message Broker            │
└─────────────────────────────────────────┘
```

### Design Patterns
- **CQRS (Command Query Responsibility Segregation)** - Using MediatR
- **Repository Pattern** - Generic repository with Unit of Work
- **Dependency Injection** - Built-in .NET DI container
- **API Gateway Pattern** - Single entry point for all services

---

## Progress Tracking Service

### Overview
The **Progress Tracking Service** is a core microservice responsible for tracking and analyzing user fitness progress. It provides comprehensive analytics including workout history, weight tracking, achievements, and progress summaries.

### Key Features

#### 1. **User Progress Dashboard**
- **Summary Statistics**: Total workouts, weight change, current/longest streaks
- **Weight History**: Chronological weight entries with BMI calculations
- **Workout History**: Daily workout logs with exercise details
- **Weekly Statistics**: Weekly performance metrics
- **Achievements**: User achievement tracking and badges

#### 2. **Weight Management**
- Update current weight with automatic BMI calculation
- Weight goal tracking with progress percentage
- Historical weight data visualization
- Integration with Fitness Calculation Service for BMI computation

#### 3. **Workout Logging**
- Log completed workouts with exercise details
- Track workout duration and calories burned
- Achievement unlocking on workout completion
- Workout history aggregation

#### 4. **User Statistics**
- Create and update user statistics
- Track total workouts, calories burned
- Maintain workout streaks (current and longest)
- Progress percentage calculation toward goals

#### 5. **Message Broker Integration**
- **RabbitMQ Consumer**: Listens for weight update events
- **Event-Driven Architecture**: Asynchronous processing of weight updates
- **Publisher/Subscriber Pattern**: Decoupled service communication

### Technical Implementation

#### Architecture Components
```
ProgressTrackingService/
├── Domain/
│   ├── Entity/          # Domain entities (UserStatistics, WeightHistory, WorkoutLog, etc.)
│   └── Interfaces/      # Repository interfaces
├── Infrastructure/
│   ├── Data/            # DbContext and database configuration
│   └── Repositories/    # Repository implementations
├── Feature/             # CQRS features organized by domain
│   ├── GetUserProgress/
│   ├── LogWorkout/
│   ├── UserStatisticsfiles/
│   └── Waight/
├── MessageBroker/       # RabbitMQ integration
│   ├── Consumers/       # Message consumers
│   ├── Messages/        # Message DTOs
│   └── Publishers/      # Message publishers
└── Shared/              # Shared utilities and clients
```

#### Key Technologies
- **.NET 8.0** - Latest .NET framework
- **Entity Framework Core 8.0** - ORM for database operations
- **MediatR 8.1.0** - CQRS pattern implementation
- **RabbitMQ.Client 7.2.0** - Message broker client
- **SQL Server** - Database storage
- **Swagger/OpenAPI** - API documentation

#### Database Schema
- **UserStatistics**: User fitness statistics and goals
- **WeightHistory**: Historical weight entries with BMI
- **WorkoutLog**: Completed workout records
- **UserAchievement**: User achievement tracking
- **Achievement**: Achievement definitions

### API Endpoints

#### Get User Progress
```
GET /api/v1/progress/{userId}
```
Returns comprehensive user progress including:
- Summary (total workouts, weight change, streaks)
- Weight history with BMI
- Workout history
- Weekly statistics
- Achievements

#### Update Weight
```
POST /api/v1/waight/update-current-weight
```
Updates user's current weight and calculates BMI automatically.

#### Update Weight Goal
```
PUT /api/v1/waight/update-goal
```
Updates user's weight goal and recalculates progress percentage.

#### Log Workout
```
POST /api/v1/workout/log
```
Logs a completed workout and triggers achievement checks.

#### User Statistics
```
GET /api/v1/userstatistics/{userId}
POST /api/v1/userstatistics
PUT /api/v1/userstatistics/{id}
```

---

## Docker Compose Orchestration

### Overview
The entire microservices ecosystem is orchestrated using **Docker Compose**, enabling easy deployment, scaling, and management of all services in a containerized environment.

### Docker Compose Configuration

#### Services Orchestration
```yaml
version: '3.4'

services:
  # SQL Server Database
  sqlserver:
    image: mcr.microsoft.com/mssql/server:2022-latest
    ports: ["1433:1433"]
    volumes: [sql_data:/var/opt/mssql]
  
  # Microservices (each with HTTP/HTTPS ports)
  workoutservice:          # 8080/8081
  nutritionservice:         # 8082/8083
  fitnesscalculationservice: # 8084/8085
  progresstrackingservice:   # 8086/8087
  fitnessapigateway:        # 8088/8089
  authenticationservice:     # 8090/8091
```

### Key Features

#### 1. **Service Isolation**
- Each microservice runs in its own container
- Independent scaling and deployment
- Isolated dependencies and configurations

#### 2. **Network Configuration**
- Internal Docker network for service-to-service communication
- Port mapping for external access
- Service discovery using container names

#### 3. **Data Persistence**
- SQL Server data volume for persistent storage
- Database migrations handled per service

#### 4. **Development vs Production**
- `docker-compose.yml` - Base configuration
- `docker-compose.override.yml` - Development overrides
- Environment-specific settings

### Running the System

#### Start All Services
```bash
docker-compose up -d
```

#### View Logs
```bash
docker-compose logs -f progresstrackingservice
```

#### Stop All Services
```bash
docker-compose down
```

#### Rebuild Services
```bash
docker-compose build --no-cache
docker-compose up -d
```

### Dockerfile Structure
Each service uses a multi-stage Docker build:
1. **Base Stage**: Runtime image (aspnet:8.0)
2. **Build Stage**: SDK image for compilation
3. **Publish Stage**: Optimized production build
4. **Final Stage**: Minimal production image

---

## API Gateway (Ocelot)

### Overview
The **Fitness API Gateway** uses **Ocelot** to provide a single entry point for all client requests. It handles routing, authentication, rate limiting, and SSL certificate management.

### Features

#### 1. **Request Routing**
- Routes requests to appropriate microservices
- Path-based routing configuration
- Load balancing support

#### 2. **Authentication & Authorization**
- JWT token validation
- Bearer token authentication
- Protected route configuration

#### 3. **Rate Limiting**
- Global rate limiting (5 requests per second)
- Prevents API abuse
- Configurable per route

#### 4. **SSL/TLS Handling**
- HTTPS support for all services
- SSL certificate bypass for development
- Secure service-to-service communication

### Route Configuration
```json
{
  "Routes": [
    {
      "DownstreamPathTemplate": "/api/progress/{everything}",
      "DownstreamHostAndPorts": [{
        "Host": "progresstrackingservice",
        "Port": 8080
      }],
      "UpstreamPathTemplate": "/api/progress/{everything}",
      "UpstreamHttpMethod": ["GET", "POST", "PUT", "DELETE"],
      "AuthenticationOptions": {
        "AuthenticationProviderKey": "Bearer"
      }
    }
  ]
}
```

### Service Discovery
- Container name-based service discovery
- Internal Docker network communication
- Health check endpoints

### Note on YARP
While this project currently uses **Ocelot**, **YARP (Yet Another Reverse Proxy)** is Microsoft's modern, high-performance reverse proxy solution. YARP offers:
- Better performance and lower latency
- Native .NET integration
- More flexible configuration
- Active Microsoft support

**Migration Path**: The architecture supports easy migration to YARP by replacing Ocelot middleware with YARP's reverse proxy configuration.

---

## Technology Stack

### Backend
- **.NET 8.0** - Framework
- **ASP.NET Core Web API** - REST API framework
- **Entity Framework Core 8.0** - ORM
- **MediatR 8.1.0** - CQRS implementation
- **SQL Server 2022** - Database

### Infrastructure
- **Docker & Docker Compose** - Containerization and orchestration
- **RabbitMQ** - Message broker for async communication
- **Ocelot** - API Gateway
- **Swagger/OpenAPI** - API documentation

### Patterns & Practices
- **CQRS** - Command Query Responsibility Segregation
- **Repository Pattern** - Data access abstraction
- **Unit of Work** - Transaction management
- **Dependency Injection** - Loose coupling
- **Domain-Driven Design** - Domain modeling

---

## Getting Started

### Prerequisites
- .NET 8 SDK
- Docker Desktop
- SQL Server (for local development)
- Visual Studio 2022 or VS Code

### Setup Instructions

1. **Clone the Repository**
   ```bash
   git clone <repository-url>
   cd Fitness_Microservices
   ```

2. **Start Docker Compose**
   ```bash
   docker-compose up -d
   ```

3. **Run Database Migrations**
   ```bash
   cd ProgressTrackingService
   dotnet ef database update
   ```

4. **Access Services**
   - API Gateway: http://localhost:8088
   - Progress Tracking Service: http://localhost:8086/swagger
   - SQL Server: localhost:1433

### Environment Configuration
Update `appsettings.json` in each service:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=sqlserver;Database=FitnessApp;User Id=sa;Password=MyComplexP@ssw0rd2025;TrustServerCertificate=True",
    "RabbitMQ": "amqp://admin:admin123@rabbit:5672/"
  }
}
```

---

## Message Broker Integration

### RabbitMQ Configuration
The Progress Tracking Service integrates with RabbitMQ for asynchronous event processing:

#### Exchange & Queue
- **Exchange**: `progress.exchange.events` (Direct)
- **Queue**: `progress.tracking.weight.queue`
- **Routing Key**: `progress.weight.updated`

#### Message Flow
1. Weight update event published to exchange
2. RabbitMQ routes message to queue
3. Consumer service processes message
4. Updates user statistics asynchronously

#### Benefits
- **Decoupled Services**: Services don't directly depend on each other
- **Scalability**: Handle high-volume events
- **Reliability**: Message persistence and retry mechanisms
- **Asynchronous Processing**: Non-blocking operations

---

## API Endpoints

### Progress Tracking Service

#### Get User Progress
```
GET /api/v1/progress/{userId}
Response: UserProgressDto
```

#### Update Current Weight
```
POST /api/v1/waight/update-current-weight
Body: WeightEntryRequestDto
Response: UpdateWeightHestoryResponseDto (includes BMI)
```

#### Update Weight Goal
```
PUT /api/v1/waight/update-goal
Body: UpdateWaightGoalCommand
Response: UpdateGoalWaightDtoResponse
```

#### Log Workout
```
POST /api/v1/workout/log
Body: WorkoutLogDto
Response: WorkoutLogResponseDto
```

#### Get User Statistics
```
GET /api/v1/userstatistics/{userId}
Response: GetUserStatisticsQueryDto
```

---

## Development Guidelines

### Code Organization
- **Feature-based structure** - Each feature in its own folder
- **CQRS separation** - Commands and Queries separated
- **Domain-driven design** - Domain entities in Domain folder
- **Infrastructure isolation** - Infrastructure concerns separated

### Best Practices
- Use MediatR for all business operations
- Implement repository pattern for data access
- Use DTOs for API responses
- Implement proper error handling
- Add logging for critical operations
- Write unit tests for business logic

### Testing
- Unit tests for handlers and services
- Integration tests for API endpoints
- Message broker testing with test containers

---

## Future Enhancements

1. **YARP Migration** - Migrate from Ocelot to YARP for better performance
2. **Redis Caching** - Implement caching layer for frequently accessed data
3. **GraphQL Support** - Add GraphQL endpoint for flexible queries
4. **Real-time Updates** - SignalR integration for live progress updates
5. **Analytics Dashboard** - Advanced analytics and reporting
6. **Mobile App Integration** - Native mobile app support
7. **Kubernetes Deployment** - Container orchestration for production

---

## License
[Your License Here]

## Contributors
[Your Name/Team]

