# API 文档

## JSON-RPC 接口

服务端地址：`http://localhost:14324`（可通过 `DREMU_SERVER_URL` 覆盖）

### getScoreInformation

返回谱面信息：

- `staves`: `StaffInformation[]`

### getPeriodElements

参数：

- `staffName`: 谱面名称
- `periodName`: 周期方法名

返回：

- `ElementInformation[]`

## 数据模型

### ScoreInformation

- `staves`: `StaffInformation[]`

### StaffInformation

- `name`: string
- `displayName`: string
- `form`: string
- `periods`: `PeriodInformation[]`

### PeriodInformation

- `name`: string
- `timeOffset`: float

### ElementInformation

- `className`: string

### EditableElementInformation / EditableInjectorInformation

- `className`: string
- `displayName`: string
- `type`（仅 EditableElementInformation）: string
- `fields`: `EditableInjectorFieldInformation[]`

### EditableInjectorFieldInformation

- `fieldType`: string
- `fieldName`: string
- `displayName`: string
- `description`: string
- `allowDefault`: bool

### 音符模型

- `TapNote` / `TaplikNote` / `DragNote`
  - `insert_index`: int
  - `hit_time`: float
  - `position`: float
- `HoldNote`
  - `insert_index`: int
  - `hit_time`: float
  - `position`: float
  - `hold_time`: float
- `Element`
  - `insert_index`: int
  - `injector_code`: string
