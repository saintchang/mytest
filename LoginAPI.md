# Login API

## 簡介
- **API 名稱**: Login API
- **描述**: 驗證用戶的 ID 和 Password，成功後返回授權 Token。
- **版本**: v1
- **基礎 URL**: `https://api.example.com`

---

## 路徑與方法

| 方法  | 路徑             | 描述                     |
|-------|------------------|--------------------------|
| POST  | `/api/v1/login`  | 驗證用戶登錄信息並返回 Token |

---

## 請求規範

### Headers
| 名稱           | 必填 | 類型          | 描述                       |
|-----------------|------|---------------|----------------------------|
| `Content-Type` | 是   | `string`      | 請求數據格式，固定為 `application/json` |

## 錯誤碼清單

| 錯誤碼   | 描述                                   |
|----------|----------------------------------------|
| `4001`   | 缺少必要參數（如 ID 或 Password）。     |
| `4002`   | 輸入格式錯誤（如 ID 長度超過限制）。   |
| `4011`   | 賬號或密碼錯誤。                      |
| `5001`   | 伺服器內部錯誤。                      |


### Request Body
- 格式：`JSON`

## 使用案例

### 成功請求
**Request**
```http
POST /api/v1/login HTTP/1.1
Host: api.example.com
Content-Type: application/json

{
  "id": "testUser",
  "password": "123456"
}
```

### 失敗回應

#### 1. 資料錯誤
- **狀態碼**: `400 Bad Request`
- **範例**
```json
{
  "status": "error",
  "message": "ID and Password are required"
}
