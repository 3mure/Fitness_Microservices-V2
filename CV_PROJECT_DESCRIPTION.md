# Elevate Fitness Microservices Platform

## Project Summary
A comprehensive fitness management platform built using microservices architecture, enabling users to track workouts, manage nutrition, monitor progress, and receive personalized fitness recommendations. The system demonstrates expertise in distributed systems, containerization, API gateway patterns, and event-driven architecture.

## Key Highlights

### 🎯 Progress Tracking Service (Core Feature)
- **Designed and implemented** a comprehensive progress tracking microservice using .NET 8, Entity Framework Core, and CQRS pattern with MediatR
- **Developed** real-time progress analytics including workout history, weight tracking with automatic BMI calculation, achievement system, and weekly statistics
- **Integrated** RabbitMQ message broker for asynchronous event processing and decoupled service communication
- **Implemented** RESTful APIs with comprehensive DTOs for progress dashboard, weight management, workout logging, and user statistics
- **Architected** domain-driven design with clean separation of concerns, repository pattern, and unit of work implementation
- **Features**: User progress dashboard, weight history tracking, workout logging, achievement system, BMI calculations, streak tracking

### 🐳 Docker Compose Orchestration
- **Orchestrated** 7 microservices using Docker Compose with proper service dependencies and networking
- **Configured** multi-stage Docker builds for optimized production images
- **Implemented** service discovery using container names and internal Docker networking
- **Managed** persistent data volumes for SQL Server database
- **Set up** development and production configurations with environment-specific overrides
- **Services**: Authentication, Workout, Nutrition, Progress Tracking, Fitness Calculation, Smart Coach, API Gateway

### 🌐 API Gateway Implementation
- **Implemented** API Gateway using Ocelot for centralized request routing and authentication
- **Configured** JWT-based authentication and authorization for protected routes
- **Implemented** rate limiting (5 requests/second) to prevent API abuse
- **Handled** SSL/TLS certificates for secure service-to-service communication
- **Designed** route configuration for all microservices with path-based routing
- **Note**: Architecture supports migration to YARP (Yet Another Reverse Proxy) for enhanced performance

## Technical Stack
- **Backend**: .NET 8, ASP.NET Core Web API, Entity Framework Core 8.0
- **Architecture**: Microservices, CQRS (MediatR), Domain-Driven Design, Repository Pattern
- **Infrastructure**: Docker, Docker Compose, SQL Server 2022, RabbitMQ
- **API Gateway**: Ocelot (YARP-ready architecture)
- **Message Broker**: RabbitMQ for asynchronous event-driven communication
- **API Documentation**: Swagger/OpenAPI

## Technical Achievements
- ✅ Designed and implemented scalable microservices architecture
- ✅ Containerized all services with Docker and orchestrated with Docker Compose
- ✅ Implemented CQRS pattern using MediatR for clean separation of commands and queries
- ✅ Integrated message broker (RabbitMQ) for event-driven architecture
- ✅ Built comprehensive progress tracking system with real-time analytics
- ✅ Configured API Gateway with authentication, rate limiting, and routing
- ✅ Implemented domain-driven design with proper separation of concerns
- ✅ Created RESTful APIs with proper error handling and DTOs

## Project Impact
- Demonstrates expertise in modern .NET development and microservices architecture
- Showcases containerization and orchestration skills with Docker Compose
- Highlights understanding of API gateway patterns and service communication
- Illustrates event-driven architecture implementation
- Proves ability to design and implement complex domain logic

## Repository Structure
```
Fitness_Microservices/
├── ProgressTrackingService/    # Core progress tracking microservice
├── AuthenticationService/      # JWT-based authentication
├── WorkoutService/            # Workout management
├── NutritionService/          # Nutrition planning
├── FitnessCalculationService/ # BMI and fitness calculations
├── FitnessAPIGateway/         # Ocelot API Gateway
├── docker-compose.yml         # Service orchestration
└── README.md                  # Project documentation
```

## Skills Demonstrated
- Microservices Architecture & Design
- Docker & Docker Compose
- API Gateway Patterns (Ocelot/YARP)
- CQRS & Event-Driven Architecture
- Domain-Driven Design
- Message Broker Integration (RabbitMQ)
- RESTful API Design
- Entity Framework Core
- .NET 8 Development
- SQL Server Database Design

## CV-Ready Bullet Points

### For Resume/Job Applications:

• Developed Progress Tracking Service microservice using .NET 8, implementing CQRS pattern with MediatR, 
  Entity Framework Core, and RabbitMQ for asynchronous event processing, enabling real-time fitness progress 
  analytics, weight tracking with automatic BMI calculation, and achievement system

• Orchestrated 7 microservices using Docker Compose with multi-stage builds, service discovery, persistent 
  volumes, and environment-specific configurations, ensuring scalable and maintainable containerized deployment

• Implemented API Gateway using Ocelot with JWT authentication, rate limiting, and SSL/TLS handling, 
  providing centralized routing and security for all microservices (architecture supports YARP migration)

