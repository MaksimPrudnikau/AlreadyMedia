# NASA Meteorite Landings Service

## Особенности

- **Data Sync Service**: Настраиваемая автоматическая синхронизация с Retry-механизмом
- **REST API**: Кеширование запросов с помощью Redis. Настроенная CORS-policy для Frontend-приложения
- **Resilient Architecture**: Docker-compose, health checks
- **Frontend**: Кеширование запросов. Пагинация

## Технологии

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
### Настройки пагинации и отображения таблицы

Количество строк в таблице по умолчанию - **10**. Значение можно изменить в компоненте `TableProvider`: 
```ts
  const [filters, updateFilters] = useState<NasaDatasetFilters>({
    ItemsPerPage: 10,
  });
```
