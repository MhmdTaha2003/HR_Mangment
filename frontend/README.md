# HR System frontend

Run `npm install`, copy `.env.example` to `.env.local`, and set `VITE_API_BASE_URL` to the API origin without `/api`. Run `npm run dev` for development; `npm run build` and `npm run lint` for checks.

The backend HTTP launch profile uses `http://localhost:5099`; HTTPS uses `https://localhost:7259`. Set the API configuration key `Frontend:Origin` (environment variable `Frontend__Origin`) to the frontend origin, such as `http://localhost:5173`, to enable CORS. In production, set `VITE_API_BASE_URL` at build time. Login uses `POST /api/Auth/login`; the returned token, expiry, email, and roles are stored in local storage, cleared at expiry or logout. Use `authHeaders()` from `src/auth.ts` for future protected requests.

The Employee Dashboard sends the bearer token to `GET /api/Employees/me`, `GET /api/EmployeeLeaveBalances/me`, and `GET /api/LeaveRequests/me`. It loads leave type names from `GET /api/LeaveTypes`. The dashboard uses only the signed-in employee's `/me` records and displays the five most recent requests. Admin, HR, and Manager routes remain protected placeholders.
