## 📌 API Endpoints

### ✅ Auth
| Method | Endpoint | Description |
|-------|----------|-------------|
| POST | `/api/auth/register` | Register new user |
| POST | `/api/auth/send-otp` | Send OTP to email |
| POST | `/api/auth/verify-otp` | Verify OTP and activate user |
| POST | `/api/auth/login` | Login with JWT + refresh token |
| POST | `/api/auth/refresh-token` | Generate new JWT |

### ✅ Organization
| Method | Endpoint | Description |
|-------|----------|-------------|
| GET | `/api/company` | List companies |
| POST | `/api/company` | Create company |
| GET | `/api/department` | List departments |
| POST | `/api/department` | Create department |

### ✅ Project Management
| Method | Endpoint | Description |
|-------|----------|-------------|
| GET | `/api/project` | List projects |
| POST | `/api/project` | Create project |
| GET | `/api/module` | List modules |
| POST | `/api/module` | Create module |
| GET | `/api/task` | List tasks |
| POST | `/api/task` | Create task |

### ✅ Dependency Graph
| Method | Endpoint | Description |
|-------|----------|-------------|
| POST | `/api/dependency/module` | Add module dependency |
| POST | `/api/dependency/task` | Add task dependency |

### ✅ Team
| Method | Endpoint | Description |
|-------|----------|-------------|
| GET | `/api/team/me` | My profile |
| PUT | `/api/team/me` | Update profile |
| POST | `/api/team/assign` | Assign user to project |

### ✅ Technology
| Method | Endpoint | Description |
|-------|----------|-------------|
| POST | `/api/technology` | Add new technology |
| POST | `/api/technology/project/{id}` | Assign tech to project |
