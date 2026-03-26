#  Foodly - Food Ordering Microservices Ecosystem

[![Microservices](https://img.shields.io/badge/Architecture-Microservices-red)](https://microservices.io/)
[![DDD](https://img.shields.io/badge/Design-DDD-blue)](https://en.wikipedia.org/wiki/Domain-driven_design)
[![Event-Driven](https://img.shields.io/badge/Messaging-Event--Driven-orange)](https://rabbitmq.com/)
[![Docker](https://img.shields.io/badge/Deployment-Docker-blueviolet)](https://www.docker.com/)
[![.NET 8](https://img.shields.io/badge/.NET-8.0-512bd4)](https://dotnet.microsoft.com/)

**Foodly** là một hệ thống đặt đồ ăn trực tuyến (F&B) toàn diện, được thiết kế theo kiến trúc **Microservices** hiện đại. Dự án tập trung vào khả năng mở rộng (scalability), hiệu suất cao thông qua **gRPC** và tích hợp trí tuệ nhân tạo (**AI**) để tối ưu hóa quy trình vận chuyển.
<img width="1522" height="739" alt="structures" src="https://github.com/user-attachments/assets/008c7549-f719-4170-ae3e-fbb6c9b6be21" />
---

##  Project Demonstrations

*   **Main Business Flow:** [Xem Video Demo](https://drive.google.com/file/d/1wa2lRapwf5uGK1VuBtZM_LTCbnHfdtef/view?usp=sharing)
*   **Sub-system Workflows:** [Xem Video Demo](https://drive.google.com/file/d/1wa2lRapwf5uGK1VuBtZM_LTCbnHfdtef/view?usp=sharing)

---

##  Core Microservices Architecture

Hệ thống bao gồm các dịch vụ độc lập, giao tiếp thông qua **REST (HTTPS)** cho Client và **gRPC** cho giao tiếp nội bộ để đạt độ trễ thấp nhất.

| Service | Port (HTTPS) | gRPC Port | Data Persistence | Responsibility |
| :--- | :---: | :---: | :--- | :--- |
| ** ApiGateWay** | `7150` | `6001` | - | Reverse proxy, request routing & rate limiting. |
| ** Auth Service** | `7223` | - | **SQL Server** | Identity management, JWT-based Auth & RBAC. |
| ** User Service** | `7199` | - | **SQL Server** | User profiles and account management. |
| ** Food Service** | `7081` | `6002` | **MySQL** | Product catalog, menu & category management. |
| ** Cart Service** | `7185` | `6005` | **PostgreSQL** | High-concurrency cart & session persistence. |
| ** Order Service** | `7264` | `6007` | **PostgreSQL** | Order lifecycle & complex business logic (DDD). |
| ** Search Service** | `7060` | - | **Elasticsearch** | Full-text search & advanced filtering engine. |
| ** Payment Service** | `7251` | - | **PostgreSQL** | Transactions & 3rd-party payment gateways. |
| ** Notification** | `5003` | `6003` | **PostgreSQL** | Multi-channel alerts (Email, SMS, Push). |
| ** Tracking (AI)** | `7139` | - | **PostgreSQL** | **AI-Powered:** Route optimization & Smart ETA. |

---

## ⚙️ Infrastructure & External Services

Sử dụng chiến lược **Polyglot Persistence** (đa cơ sở dữ liệu) và **Message Broker** để xử lý các tác vụ bất đồng bộ.

| Component | Port(s) | Role | Management URL |
| :--- | :---: | :--- | :--- |
| ** PostgreSQL** | `5433` | Primary Relational DB | `localhost:5433` |
| ** SQL Server** | `1434` | Enterprise Data Storage | `localhost:1434` |
| ** MySQL** | `3307` | Catalog Data | `localhost:3307` |
| ** Redis** | `6380` | Distributed Cache | `localhost:6380` |
| ** RabbitMQ** | `15673` | Event-Driven Broker | [RabbitMQ Console](http://localhost:15673) |
| ** MinIO** | `9001` | Object Storage (S3) | [MinIO Dashboard](http://localhost:9001) |
| ** Elasticsearch** | `9200` | Analytics & Search | [Elastic API](http://localhost:9200) |

---

##  Technical Highlights

###  AI-Powered Logistics
**Tracking Service** tích hợp các mô hình Machine Learning (như Regression/LSTM) để dự đoán chính xác thời gian giao hàng (ETA) dựa trên dữ liệu lịch sử, thời tiết và tọa độ GPS thời gian thực.

###  Event-Driven Architecture
Sử dụng **RabbitMQ** để thực hiện giao tiếp bất đồng bộ giữa các Service. 
*Ví dụ: Khi một Order được thanh toán thành công, một Event sẽ được phát đi để Notification Service gửi Email và Tracking Service bắt đầu lộ trình.*

###  High-Performance Communication
Hệ thống sử dụng **gRPC** (Protocol Buffers) cho các giao tiếp liên dịch vụ (Inter-service), giúp giảm đáng kể kích thước gói tin và tăng tốc độ xử lý so với JSON/HTTP thông thường.

### Domain-Driven Design (DDD)
Cấu trúc Source Code được tổ chức chặt chẽ theo các Bounded Context, giúp tách biệt logic nghiệp vụ phức tạp và dễ dàng bảo trì, mở rộng trong tương lai.

###  Optimized Search
Sử dụng **Elasticsearch** để Index dữ liệu từ Food Service, cho phép người dùng tìm kiếm món ăn với tốc độ cực nhanh ngay cả khi tập dữ liệu lớn.

---

##  Tech Stack

*   **Backend:** .NET 8, ASP.NET Core Web API, gRPC.
*   **Frontend:** React (for Admin/User Dashboard).
*   **Database:** PostgreSQL, SQL Server, MySQL, Redis, Elasticsearch.
*   **DevOps:** Docker, Docker Compose, IIS.
*   **Messaging:** RabbitMQ (MassTransit).
*   **AI/ML:** Python/Scikit-learn (integrated within Tracking Service).
*   **Storage:** MinIO (S3 Compatible).

---

© 2026 Pham Trung Duc. All rights reserved.
