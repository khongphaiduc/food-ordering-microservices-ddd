#  Foodly - Food Ordering Microservices Ecosystem

[![Microservices](https://img.shields.io/badge/Architecture-Microservices-red)](https://microservices.io/)
[![DDD](https://img.shields.io/badge/Design-DDD-blue)](https://en.wikipedia.org/wiki/Domain-driven_design)
[![Event-Driven](https://img.shields.io/badge/Messaging-Event--Driven-orange)](https://rabbitmq.com/)
[![Docker](https://img.shields.io/badge/Deployment-Docker-blueviolet)](https://www.docker.com/)
[![.NET 8](https://img.shields.io/badge/.NET-8.0-512bd4)](https://dotnet.microsoft.com/)

**Foodly** is a comprehensive online food ordering system (F&B), designed using a modern **Microservices architecture**. The project focuses on scalability, high performance through **gRPC**, and integratesIntegrates **AI** to develop personalized food recommendations based on user preferences and behavior

---

##  Project Demonstrations

* **Main Business Flow:** [Watch Demo Video](https://drive.google.com/file/d/1wa2lRapwf5uGK1VuBtZM_LTCbnHfdtef/view?usp=sharing)
* **Sub-system Workflows:** [Watch Demo Video](https://drive.google.com/file/d/1wa2lRapwf5uGK1VuBtZM_LTCbnHfdtef/view?usp=sharing)

---

<img width="1488" height="762" alt="image" src="https://github.com/user-attachments/assets/1745a384-6535-4d7a-8972-ba2bf606120b" />



---

##  Core Microservices Architecture

The system consists of independent services communicating via **REST (HTTPS)** for client interactions and **gRPC** for internal communication to achieve minimal latency.

| Service | Port (HTTPS) | gRPC Port | Data Persistence | Responsibility |
| :--- | :---: | :---: | :--- | :--- |
| **ApiGateWay** | `7150` | `-` | - | Reverse proxy, request routing & rate limiting. |
| **Auth Service** | `7223` | - | **SQL Server** | Identity management, JWT-based authentication & RBAC. |
| **User Service** | `7199` | `5001` | **SQL Server** | User profiles and account management. |
| **Food Service** | `7081` | `5002` | **MySQL** | Product catalog, menu & category management. |
| **Cart Service** | `7185` | `5005` | **PostgreSQL** | High-concurrency cart & session persistence. |
| **Order Service** | `7264` | `5007` | **PostgreSQL** | Order lifecycle & complex business logic (DDD). |
| **Search Service** | `7060` | - | **Elasticsearch** | Full-text search & advanced filtering engine. |
| **Payment Service** | `7251` | `5006` | **PostgreSQL** | Transactions & third-party payment gateway integration. |
| **Notification Service** | `5003` | `-` | **PostgreSQL** | Multi-channel notifications (Email, SMS, Push). |
| **Tracking Service (AI)** | `7139` | `5003` | **PostgreSQL** | AI-powered route optimization & smart ETA prediction. |

---
##  Web UI Screenshots
App.Home
<img width="1844" height="942" alt="Screenshot 2026-03-27 101221" src="https://github.com/user-attachments/assets/42a4ae24-9847-40d5-a416-cca6354cb1fb" />

App.Admin
<img width="1858" height="949" alt="image" src="https://github.com/user-attachments/assets/1cd96de6-c866-4a92-b184-b76c47024548" />

## Frontend Source
[Food Ordering Microservices Frontend](https://github.com/khongphaiduc/food-ordering-microservices-frontend)
---
## ⚙️ Infrastructure & External Services

The system uses a **Polyglot Persistence strategy** and a **Message Broker** to handle asynchronous processing.

| Component | Port(s) | Role | Management URL |
| :--- | :---: | :--- | :--- |
| **PostgreSQL** | `5433` | Primary relational database | `localhost:5433` |
| **SQLServer** | `1434` | Enterprise data storage | `localhost:1434` |
| **Redis** | `6380` | Distributed caching system | `localhost:6380` |
| **RabbitMQ** | `15673` | Event-driven message broker | http://localhost:15673 |
| **MinIO** | `9001` | Object storage (S3-compatible) | http://localhost:9001 |
| **Elasticsearch** | `9200` | Search & analytics engine | http://localhost:9200 |

---

##  Technical Highlights

###  AI-Powered Logistics
The **Tracking Service**  uses AI to deliver personalized food recommendations by analyzing user behavior such as browsing history, add-to-cart actions, and purchase patterns. Based on this data, Machine Learning models identify user preferences and suggest relevant dishes tailored to each individual.

###  Event-Driven Architecture
Uses **RabbitMQ** for asynchronous communication between services.  

*Example: When an order is successfully paid, an event is published so that the Notification Service sends emails and the Tracking Service starts route processing.*

###  High-Performance Communication
The system uses **gRPC (Protocol Buffers)** for inter-service communication, significantly reducing payload size and improving performance compared to traditional JSON/HTTP.

###  Domain-Driven Design (DDD)
The source code is organized using **Bounded Contexts**, helping isolate complex business logic and making the system easier to maintain and scale.

###  Optimized Search
Uses **Elasticsearch** to index data from the Food Service, enabling extremely fast search performance even with large datasets.

---

##  Tech Stack

* **Backend:** .NET 8, ASP.NET Core Web API, gRPC  
* **Frontend:** ReactJS (Admin/User/Staff Dashboard)  
* **Database:** PostgreSQL, SQL Server, Redis, Elasticsearch  
* **DevOps:** Docker, Docker Compose, Kestrel ,IIS  
* **Messaging:** RabbitMQ (MassTransit)  
* **AI:** **GEMINI_AI** 
* **Storage:** MinIO 

---

##  Author

**Pham Trung Duc**  2026   
---

##  License

This project is for learning and demonstration purposes.

---
### Thank you for checking out this project!  
**Phạm Trung Đức (PhamTrungDuc)**  
**Email:** ptrungduc1011@gmail.com
