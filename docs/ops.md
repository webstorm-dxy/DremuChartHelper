# 运维手册

## 环境要求

- .NET 9 SDK
- GorgeStudio 服务端可用并监听 `http://localhost:14324`

## 配置

- `DREMU_SERVER_URL`: 指定服务端地址

## 启动流程

1. 启动 GorgeStudio 服务端
2. 运行客户端：

```bash
dotnet run --project DremuChartHelper/DremuChartHelper.csproj
```

## 常见排查

- JSON-RPC 调用失败：确认服务端地址与端口，检查 `DREMU_SERVER_URL`
- 数据为空：确认服务端返回的 `staves` 非空
