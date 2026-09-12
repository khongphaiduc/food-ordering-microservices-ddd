| Side | Desktop / Tablet | Mobile |
|------|------------------|--------|
| Customer | [▶️ Watch Demo](https://drive.google.com/file/d/1NjsbKt0ZSbpr7LE8tju6pzWbFlIOy0ks/view?usp=drive_link) | [▶️ Watch Video](https://drive.google.com/file/d/1s0AH2Ivy3C1r_RPxj5XRG-CHe2U0LvXq/view?usp=drive_link) |
| Admin | Coming Soon | Coming Soon |

# Foodly - Food Ordering Microservices System

[![.NET 8](https://img.shields.io/badge/.NET-8.0-512bd4)](https://dotnet.microsoft.com/)
[![Architecture](https://img.shields.io/badge/Architecture-Microservices-red)](https://microservices.io/)
[![DDD](https://img.shields.io/badge/Design-DDD-blue)](https://en.wikipedia.org/wiki/Domain-driven_design)
[![Messaging](https://img.shields.io/badge/Messaging-RabbitMQ%20%7C%20MassTransit-orange)](https://www.rabbitmq.com/)
[![gRPC](https://img.shields.io/badge/gRPC-High%20Performance-00A4EF)](https://grpc.io/)
[![Payment Gateway](https://img.shields.io/badge/Payment-PayOS-success)](https://payos.vn/)
[![Deployment](https://img.shields.io/badge/Deployment-Docker%20%7C%20Jenkins-blueviolet)](https://www.docker.com/)

Foodly is a modern, production-ready backend food ordering platform built with **.NET 8** and a **Microservices Architecture**. Designed using **Clean Architecture** and **Domain-Driven Design (DDD)** principles, the system decouples core business capabilities into 8 independent microservices handling authentication, user profiles, food catalog & inventory, shopping cart, order lifecycle, payments, notifications, and central API routing.

The project demonstrates advanced enterprise backend engineering patterns including **gRPC inter-service communication**, **Event-Driven Architecture with MassTransit & RabbitMQ**, **Outbox / Inbox Transactional Messaging Pattern**, **PayOS payment gateway integration**, **MinIO S3-compatible object storage**, **Redis caching**, **SignalR real-time updates**, and **Jenkins CI/CD automation**.

---

## 📸 Project & Web UI Screenshots

### Architecture Preview

<img width="1753" height="897" alt="55bb0710-26ab-44ce-9b57-7ab544f5de98" src="https://github.com/user-attachments/assets/313d747a-46be-4b98-bc56-a5dbe2497613" />
### Desktop & Tablet UI

<img width="1863" height="951" alt="Foodly home app screenshot" src="https://github.com/user-attachments/assets/ed2f4db0-0b66-4335-94bf-c0ca750a7d9e" />

### Mobile Phone UI

<img width="475" height="938" alt="Foodly mobile app 1" src="https://github.com/user-attachments/assets/72730bdb-a14d-45f7-ad65-cee3d84e3219" /> <img width="478" height="940" alt="Foodly mobile app 2" src="https://github.com/user-attachments/assets/433a5f47-3686-49e0-a470-c6400c3b8ae4" />

### Admin Management App
<img width="1916" height="917" alt="Screenshot 2026-09-12 153603" src="https://github.com/user-attachments/assets/bd1df971-39d7-4fb2-a0d9-25c6a7077bea" />


> Frontend repository: [Food Ordering Microservices Frontend](https://github.com/khongphaiduc/food-ordering-microservices-frontend)

---

##  Architecture Overview

The system consists of **8 distinct microservices** coordinated through an **API Gateway (Ocelot)** for client-facing HTTP/REST endpoints, **gRPC** for low-latency internal service-to-service RPCs, and **RabbitMQ via MassTransit** for asynchronous event publishing and transactional Outbox messaging.

```mermaid
flowchart TB
    subgraph Clients["Clients"]
        Web["Web App"]
        Mobile["Mobile App"]
    end

    subgraph Ingress["API Gateway Layer"]
        Gateway["ApiGateway (Ocelot - Port 9080)"]
    end

    subgraph Services["Microservices Domain Layer"]
        Auth["Auth Service"]
        User["User Service"]
        Food["Food Service"]
        Cart["Cart Service"]
        Order["Order Service"]
        Payment["Payment Service"]
        Notif["Notification Service"]
    end

    subgraph Messaging["Message Broker & Realtime"]
        RabbitMQ[("RabbitMQ / MassTransit")]
        SignalR["SignalR Hub"]
    end

    subgraph Storage["Polyglot Storage & Infrastructure"]
        SQLServer[("SQL Server")]
        PostgreSQL[("PostgreSQL")]
        Redis[("Redis Cache")]
        MinIO[("MinIO Object Storage")]
    end

    %% Client & Gateway
    Web --> Gateway
    Mobile --> Gateway
    Gateway --> Auth
    Gateway --> User
    Gateway --> Food
    Gateway --> Cart
    Gateway --> Order
    Gateway --> Payment

    %% gRPC Service Connections
    Auth -- "gRPC (user_info_service.proto)" --> User
    Cart -- "gRPC (Products.proto)" --> Food
    Order -- "gRPC (InformationCart.proto)" --> Cart
    Order -- "gRPC (GetAddress.proto)" --> User
    Order -- "gRPC (ProductInventory.proto)" --> Food
    Order -- "gRPC (CreatePaymentForOrder.proto)" --> Payment
    Payment -- "gRPC (UpdateStatusOrderPaid.proto)" --> Order

    %% Event Driven & Messaging
    Order -- "Publish Events (Outbox)" --> RabbitMQ
    Payment -- "Publish Webhook Events" --> RabbitMQ
    RabbitMQ -- "OrderCompletedEvent" --> Notif
    RabbitMQ -- "CancelPaymentConsumer / PaymentConsumer" --> Payment

    %% Databases & Storage
    Auth --> SQLServer
    User --> SQLServer
    Order --> SQLServer
    Payment --> SQLServer
    
    Food --> PostgreSQL
    Cart --> PostgreSQL
    Notif --> PostgreSQL

    Food --> MinIO
    Food --> Redis
    Cart --> Redis
    Auth --> Redis
    Order --> SignalR
```

---

##  Microservices Breakdown

| Service | Subfolder | Primary Responsibility | Inter-Service Protocol | Databases & Infra |
| :--- | :--- | :--- | :--- | :--- |
| **API Gateway** | `ApiGateway/` | Entry point, reverse proxy, routing, rate limiting & CORS handling | REST (Ocelot) | Docker (`:9080`) |
| **Auth Service** | `auth-services/` | Identity, signup, login, JWT token generation, refresh tokens & staff RBAC | REST, gRPC Client | SQL Server, Redis |
| **User Service** | `user-service/` | Customer profiles & delivery address book management | REST, gRPC Server | SQL Server |
| **Food Service** | `food-service/` | Product catalog, food categories, daily inventory, food recommendations & MinIO image uploads | REST, gRPC Server/Client, MassTransit Consumer | PostgreSQL, Redis, MinIO |
| **Cart Service** | `cart-service/` | Shopping cart lifecycle, item add/remove/update | REST, gRPC Server/Client | PostgreSQL, Redis |
| **Order Service** | `order-service/` | Order creation, status lifecycle, history, SignalR real-time updates & automated expiration background worker | REST, gRPC Client, MassTransit Outbox/Inbox, SignalR | SQL Server, RabbitMQ |
| **Payment Service** | `payment-service/` | PayOS payment gateway integration, payment link creation & webhook status callbacks | REST, gRPC Server/Client, MassTransit Outbox/Inbox | SQL Server, RabbitMQ, PayOS API |
| **Notification Service** | `notification-service/` | Asynchronous email confirmations upon order completion | MassTransit Consumer | PostgreSQL, RabbitMQ, SMTP |

---

##  gRPC Inter-Service Communication

Low-latency synchronous service calls are handled via gRPC Protocol Buffers:

```text
[Auth Service]   ---- user_info_service.proto ---->  [User Service]   (Retrieve user profile for authentication claims)
[Cart Service]   ---- Products.proto ------------->  [Food Service]   (Validate product details & live prices)
[Order Service]  ---- InformationCart.proto ------->  [Cart Service]   (Fetch active cart items during checkout)
[Order Service]  ---- GetAddress.proto ----------->  [User Service]   (Retrieve customer shipping address)
[Order Service]  ---- ProductInventory.proto ------>  [Food Service]   (Verify stock & reserve items)
[Order Service]  <--- CreatePaymentForOrder.proto -> [Payment Service] (Initiate PayOS payment transaction)
[Payment Service] --- UpdateStatusOrderPaid.proto -> [Order Service]  (Mark order as Paid upon payment completion)
```

---

##  Event-Driven Messaging & Outbox Pattern

Asynchronous operations are managed using **RabbitMQ** and **MassTransit**:
- **Transactional Outbox/Inbox Pattern**: Integrated via EF Core in `Order Service` and `Payment Service` to guarantee message delivery without distributed transaction failures.
- **PayOS Webhooks**: Webhook callbacks received by `Payment Service` trigger `PaymentConsumer` and `HandleOrderPaySuccessfullyConsumer` to transition order states safely.
- **Email Notifications**: Upon order completion, an `OrderCompletedEvent` is published to RabbitMQ. `Notification Service` (`SendEmailConsumer`) processes the event and dispatches rich HTML email receipts via SMTP.
- **Order Expiration Worker**: A background worker (`CheckExpireOrder`) periodically scans for unpaid pending orders and marks them as expired.

---

##  Infrastructure & Port Mappings

The project provisions all required infrastructure services via **Docker Compose**:

| Container | Image | Host Port | Purpose |
| :--- | :--- | :---: | :--- |
| **API Gateway** | `ptrungduc1011/foodlyapigateway` | `9080:8080` | Central public entry point |
| **SQL Server** | `mcr.microsoft.com/mssql/server:2022-latest` | `1888:1433` | Data store for Auth, User, Order, Payment services |
| **PostgreSQL** | `postgres:17-alpine` | `5555:5432` | Data store for Food, Cart, Notification services |
| **RabbitMQ** | `rabbitmq:3-management-alpine` | `5672`, `15672` | AMQP message broker & Management Console |
| **Redis** | `redis:alpine` | `6379:6379` | High-speed cache for catalog & cart sessions |
| **MinIO** | `minio/minio` | `9000`, `9001` | S3-compatible image object storage & console UI |

---

##  Repository Structure

```text
.
├── ApiGateway/                  # Ocelot API Gateway project
├── auth-services/               # Identity, Auth & JWT Service (Clean Architecture)
├── user-service/                # User Profile & Address Service
├── food-service/                # Product, Category & Storage (MinIO) Service
├── cart-service/                # Shopping Cart Service
├── order-service/               # Order Management, SignalR & Worker Service
├── payment-service/             # PayOS Payment Integration & Webhook Service
├── notification-service/        # Asynchronous Email Notification Service (MassTransit)
├── Foodly.Tests/                # xUnit unit testing project for core services
├── docker-compose.yml           # Multi-container orchestration specification
├── Jenkinsfile                  # Automated CI/CD build & deploy pipeline script
└── food-ordering-microservices-system.sln
```

---

##  Getting Started

### Option 1: Quick Run (Without Cloning Source Code)

You can run the full system instantly using pre-built Docker Hub images without downloading or compiling the source code.

#### Deployment Folder Structure:
```text
.
├── docker-compose.yml
├── .example.env          # Main environment variables template
├── auth.example.env      # Auth Service environment variables
├── cart.example.env      # Cart Service environment variables
├── food.example.env      # Food Service environment variables
├── gateway.example.env   # API Gateway environment variables
├── order.example.env     # Order Service environment variables
├── payment.example.env   # Payment Service environment variables
└── user.example.env      # User Service environment variables
```

#### Setup Steps:
1. Open the deployment folder.
2. Rename all `*.example.env` files to remove `.example`:
   - `.example.env` ➔ `.env`
   - `auth.example.env` ➔ `auth.env`
   - `cart.example.env` ➔ `cart.env`
   - `food.example.env` ➔ `food.env`
   - `gateway.example.env` ➔ `gateway.env`
   - `order.example.env` ➔ `order.env`
   - `payment.example.env` ➔ `payment.env`
   - `user.example.env` ➔ `user.env`
3. Start all services (pre-built images `ptrungduc1011/foodly*` will be pulled automatically from Docker Hub):
   ```bash
   docker compose up -d
   ```

---

### Option 2: Build & Run from Source Code

#### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- [Git](https://git-scm.com/)

#### Steps:
1. **Clone the Repository**:
   ```bash
   git clone https://github.com/khongphaiduc/food-ordering-microservices-system.git
   cd food-ordering-microservices-system
   ```

2. **Build and Start Containers**:
   ```bash
   docker compose up --build -d
   ```

---

###  Service Access Endpoints

- **API Gateway (Public Entry Point)**: `http://localhost:9080`
- **RabbitMQ Management Dashboard**: `http://localhost:15672` (Default login: `guest` / `guest`)
- **MinIO Console**: `http://localhost:9001`

---

##  Author

**Pham Trung Duc**  
- Email: ptrungduc1011@gmail.com  
- GitHub: [@khongphaiduc](https://github.com/khongphaiduc)  

---

##  License

This project is open-source and intended for learning, software architecture demonstration, and portfolio purposes.
