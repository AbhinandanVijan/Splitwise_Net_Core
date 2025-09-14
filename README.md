# Splitwise Clone – .NET 8 + React

A full-stack **expense sharing app** inspired by Splitwise.  
Backend is built with **.NET 8 Web API + EF Core**, uses **PostgreSQL** (with `xmin`-based optimistic concurrency), and supports **JWT authentication**.  
Frontend is built with **React + Vite**, featuring a clean, responsive UI for managing groups, expenses, and balances.

---

## 🚀 Features

- **Authentication & Authorization**
  - Secure login & signup with hashed passwords (BCrypt)
  - JWT Bearer authentication
- **Groups & Members**
  - Create groups, invite members, and view balances
- **Expense Management**
  - Add expenses with **equal, unequal, or percentage-based splits**
  - Tracks who owes whom automatically
- **Transactions**
  - View detailed balances per group and per user
- **Concurrency-safe Updates**
  - PostgreSQL `xmin` tokens to prevent race conditions on updates
- **Modern Frontend**
  - React + Axios + Vite + TailwindCSS
  - Proxy setup for seamless API calls in dev

---

## 🛠️ Tech Stack

**Backend**
- .NET 8 Web API  
- EF Core (Code-First + Migrations)  
- PostgreSQL  
- BCrypt.Net (password hashing)  
- JWT Authentication  
- Serilog (structured logging)

**Frontend**
- React + Vite  
- Axios (with centralized API client + auth interceptor)  
- TailwindCSS  
- React Router (for navigation)

**Infrastructure**
- Docker & Docker Compose (optional)  
- Swagger/OpenAPI (interactive API docs)

---

## 🏗️ Architecture

```text
React (Vite) ──> Vite Dev Proxy (/api/*)
                       │
                       ▼
                .NET 8 API (Kestrel)
                       │
                       ▼
               PostgreSQL Database
```
## ⚡ Getting Started
1. Clone the repository
git clone https://github.com/yourusername/splitwise-clone.git
cd splitwise-clone

2. Backend Setup

Configure Database

Update appsettings.Development.json with your PostgreSQL connection string.

Run Migrations

dotnet ef database update


Run API

cd src/Splitwise.Api
dotnet run


API will be available at:
Swagger: http://localhost:5085/swagger/index.html

3. Frontend Setup
cd client
npm install
npm run dev


Frontend runs on: http://localhost:5173

Note: vite.config.js is preconfigured to proxy /api to http://localhost:5085.

🔑 Authentication Example

Login

POST /api/auth/login
{
  "email": "user@example.com",
  "password": "secret"
}


Use Token
Add to every request:

Authorization: Bearer <JWT_TOKEN>


React Axios Setup

api.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token) config.headers.Authorization = `Bearer ${token}`;
  return config;
});
