# NASA Meteorite Landings Service

![Project Architecture](docs/architecture.png)

## 📌 Особенности

- **Data Sync Service**: Настраиваемая автоматическая синхронизация с Retry-механизмом
- **REST API**: Кеширование запросов с помощью Redis. Настроенная CORS-policy для Frontend-приложения
- **Resilient Architecture**: Docker-compose, health checks

## 🛠️ Технологии

### Backend
- .NET 9
- Entity Framework Core
- PostgreSQL
- Redis

### Frontend
- React
- Tanstack Query
- Tanstack Table
- openapi-fetch (автоматическая генерация типов OpenAPI)
- Shadcn (tailwind)

## 🚀 Запуск проекта

### Команды
```bash
cd docker/
docker compose up --build
```

### Настройки синхронизации

Retry-интервал для `Background Service` настраивается через поле `ResyncIntervalSeconds` в секции `NasaDataset` в `appsettings.json` проекта `NasaClientService`. Интервал между обращениями к ресурсу настраивается там же через поле `SyncIntervalSeconds`. Для корректной инвалидации кеша, при **ручном** изменении `SyncIntervalSeconds` в `NasaClientService`, изменения нужно также продублировать в настройках `API` проекта `AlreadyMedia`.

```json
"NasaDataset": {
  "SyncIntervalSeconds": 15,
  "ResyncIntervalSeconds": 5
}
```
