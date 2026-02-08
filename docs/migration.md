# 迁移变更日志与升级指引

## 迁移变更日志

- 将 GorgeStudio 客户端代码迁移至 DremuChartHelper 的 GorgeLinker 模块
- 移除 GorgeStudio 项目引用与解决方案配置
- 统一 JSON-RPC 协议与数据模型在主项目内维护
- 增加回归测试覆盖 JSON-RPC 请求与数据模型序列化

## 升级指引

1. 更新到本次提交后的代码
2. 确保 `DREMU_SERVER_URL` 指向可用的 GorgeStudio 服务端
3. 执行 `dotnet test` 验证回归测试
4. 执行 `dotnet run --project DremuChartHelper/DremuChartHelper.csproj` 启动应用
