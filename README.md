# Demo API — версия A

Учебный backend-сервис для курса «DevOps-инструменты». Сервис реализован на ASP.NET Core и предназначен для работы с проектами и задачами через REST API. Данные могут храниться в памяти приложения или в PostgreSQL.

## Основные endpoints

### Проекты

- `GET /api/projects` — получить список проектов.
- `GET /api/projects/{id}` — получить проект по идентификатору.
- `POST /api/projects` — создать проект.
- `DELETE /api/projects/{id}` — удалить проект.

### Задачи

- `GET /api/tasks` — получить список задач.
- `GET /api/tasks/{id}` — получить задачу по идентификатору.
- `POST /api/tasks` — создать задачу.
- `PUT /api/tasks/{id}` — изменить задачу.
- `PATCH /api/tasks/{id}/status` — изменить статус задачи.
- `DELETE /api/tasks/{id}` — удалить задачу.

### Служебные endpoints

- `GET /health/live` — проверка, что приложение запущено.
- `GET /health/ready` — проверка готовности приложения и PostgreSQL.
- `GET /metrics` — метрики Prometheus.
- `GET /swagger` — Swagger UI в режиме Development.

## Запуск через Docker Compose

Перед запуском создайте в корне проекта файл `.env` с переменными окружения, необходимыми для Docker Compose и подключения к PostgreSQL.

Запустите приложение и PostgreSQL:

```bash
docker compose up -d --build --wait
```

После запуска API доступно по адресу:

```text
http://localhost:8080
```

## Разработка

Для локальной разработки сначала запустите PostgreSQL:

```bash
docker compose up -d --wait db
```

После этого запустите проект из Visual Studio клавишей **F5**.

## Тестирование

Для запуска автоматических тестов выполните:

```bash
dotnet test
```

Тесты находятся в проекте `tests/DemoApi.Tests`.