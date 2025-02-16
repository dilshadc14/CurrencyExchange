# API Documentation

## User Policies
- `adminonly`
- `userandadmin`

## User Credentials

1. **Admin User**  
   **Username:** `admin`  
   **Password:** `admin123`

2. **Regular User**  
   **Username:** `User`  
   **Password:** `User123`

---

## API Endpoints

### 1. Get Token

#### Endpoint:
`POST /api/auth/login`

#### Description:
Returns a token for admin or user based on the credentials passed.

#### Request Body Parameters:
- `username`: The username of the user (e.g., `admin`, `User`).
- `password`: The password corresponding to the username.

Example request:

```json
{
  "username": "admin",
  "password": "admin123"
}


