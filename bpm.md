
```
BpmSystem.sln
├── 01.Domain (核心領域層)
│   └── BpmSystem.Domain.csproj (POCOs, Enums, Interfaces)
├── 02.Core (核心引擎層)
│   └── BpmSystem.Engine.csproj (StateMachine, WorkflowRuntime)
├── 03.Infrastructure (基礎設施層)
│   ├── BpmSystem.Persistence.csproj (EF Core, Oracle/SQLite Providers)
│   ├── BpmSystem.RuleEngine.csproj (DMN/Expression Parser)
│   └── BpmSystem.Integration.csproj (ERP/HR API Adapters)
├── 04.Application (應用服務層)
│   └── BpmSystem.Application.csproj (DTOs, Services, TaskLogic)
├── 05.Presentation (呈現層)
│   ├── BpmSystem.WebApi.csproj (ASP.NET Core Controllers)
│   └── bpm-client-app (React + TypeScript SPA)
└── 06.Tests (測試專案)
    ├── BpmSystem.Engine.Tests.csproj
    └── BpmSystem.RuleEngine.Tests.csproj
```
