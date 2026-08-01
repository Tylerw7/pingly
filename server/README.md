##Current work flow##


curl
 │
 ▼
PublishController
 │
 ▼
PublishService
 │
 ├── Save Message
 │
 └── Channel.WriteAsync()
           │
           ▼
     HTTP 202 Accepted


Meanwhile...


NotificationWorker
 │
 ▼
Read Channel
 │
 ▼
Create SSE event
 │
 ▼
SseConnectionManager
 │
 ▼
Find all connections
 │
 ├─────────────┐
 │             │
 ▼             ▼
Browser A    Browser B




##Current Pingly Architecture

                       ┌──────────────┐
                       │    Neon      │
                       │ PostgreSQL   │
                       └──────▲───────┘
                              │
                              │
curl ──► PublishController     │
              │                │
              ▼                │
        PublishService ────────┘
              │
              ▼
      NotificationChannel
              │
              ▼
      NotificationWorker
              │
              ▼
      SseConnectionManager
          │     │     │
          ▼     ▼     ▼
       Web 1  Web 2  Web 3