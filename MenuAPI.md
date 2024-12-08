# Function Menu API

## 簡介
- **API 名稱**: Function Menu API
- **描述**: 登錄後根據用戶 ID 查詢其功能選單，返回多筆結果。
- **版本**: v1
- **基礎 URL**: `https://api.example.com`

---

## 路徑與方法

| 方法  | 路徑                   | 描述                    |
|-------|------------------------|-------------------------|
| POST  | `/api/v1/function-menu` | 根據 ID 查詢功能選單列表 |

---

## 請求規範

### Headers
| 名稱           | 必填 | 類型          | 描述                       |
|-----------------|------|---------------|----------------------------|
| `Authorization` | 是   | `string`      | 登錄時獲取的 Bearer Token  |
| `Content-Type`  | 是   | `string`      | 請求數據格式，固定為 `application/json` |

### Request Body
- 格式：`JSON`

#### 範例
```json
{
  "id": "testUser"
}
```
### Response
```json
HTTP/1.1 200 OK
Content-Type: application/json

{
  "status": "success",
  "data": [
    {
      "FunctionID": "F001",
      "FunctionName": "Dashboard"
    },
    {
      "FunctionID": "F002",
      "FunctionName": "User Management"
    },
    {
      "FunctionID": "F003",
      "FunctionName": "Reports"
    }
  ],
  "message": "Function menu retrieved successfully"
}


