# DremuChartHelper

DremuChartHelper 是基于 Avalonia UI 的 Windows 桌面应用，用于处理音乐节奏游戏谱面数据。项目通过内置的 GorgeLinker JSON-RPC 客户端与 GorgeStudio 服务端通信。

## 运行与构建

```bash
dotnet build
dotnet run --project DremuChartHelper/DremuChartHelper.csproj
```

服务端地址可通过环境变量 `DREMU_SERVER_URL` 配置，默认值为 `http://localhost:14324`。

## 测试

```bash
dotnet test
```

## 文档

- API 文档：docs/api.md
- 运维手册：docs/ops.md
- 用户指南：docs/user-guide.md
- 迁移变更日志与升级指引：docs/migration.md
