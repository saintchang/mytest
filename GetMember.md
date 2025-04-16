# Member 資料查詢 API

## 📘 功能簡介
使用者透過登入後取得的 Token，查詢自身的完整會員資料。

---

## 📥 API 請求資訊

- **方法**：`POST`
- **URL**：`/api/v1/member-info`
- **Content-Type**：`application/json`

### ✅ 請求參數

| 參數名稱 | 型別   | 必填 | 說明                                  |
|----------|--------|------|---------------------------------------|
| id       | string | 是   | 使用者帳號，對應資料表欄位 `MM_NO`    |
| token    | string | 是   | 登入後取得的 JWT Token，用於授權驗證  |

#### 📦 請求範例
```json
{
  "id": "testUser",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

## 🔹 data 欄位內容（依據 `member.txt`）

| 欄位名稱     | 資料型別 | 說明                       |
|--------------|-----------|----------------------------|
| MM_NO        | string    | 會員帳號（登入使用）       |
| MM_NAME      | string    | 會員姓名                   |
| PSW          | string    | 密碼（加密或明文）         |
| SEX          | string    | 性別（M/F）                |
| BIRTHDAY     | string    | 生日（yyyy-MM-dd）         |
| ID           | string    | 身分證字號                 |
| TEL          | string    | 電話號碼                   |
| PHONE        | string    | 行動電話                   |
| EMAIL        | string    | 電子郵件                   |
| ZIP          | string    | 郵遞區號                   |
| ADDR         | string    | 地址                       |
| DEPT         | string    | 所屬部門                   |
| POSITION     | string    | 職稱                       |
| ENABLED      | boolean   | 帳號是否啟用（true/false） |
| CREATE_DATE  | string    | 建立日期（yyyy-MM-dd）     |
| LAST_LOGIN   | string    | 最後登入時間（ISO 格式）   |
| GOV_ID       | string    | 政府平台登入帳號           |
| GOV_PSW      | string    | 政府平台登入密碼           |
| REMARK       | string    | 備註                       |
| LOGTIME      | string    | 本次登入時間（由後端產生） |

> 📌 說明：
> - `LOGTIME` 為登入當下時間（系統動態產生）。
> - 所有日期/時間欄位建議使用 ISO 8601 格式。
> - 欄位名稱與資料表一致，前端可直接對應使用。

```json
{
  "status": true,
  "return_code": "0000",
  "message": "Query successful",
  "tokenn": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "data": {
    "MM_NO": "testUser001",
    "MM_NAME": "王小明",
    "PSW": "encrypted_password_hash",
    "SEX": "M",
    "BIRTHDAY": "1990-05-20",
    "ID": "A123456789",
    "TEL": "02-23456789",
    "PHONE": "0912345678",
    "EMAIL": "testuser@example.com",
    "ZIP": "100",
    "ADDR": "台北市中正區仁愛路一段1號",
    "DEPT": "資訊部",
    "POSITION": "工程師",
    "ENABLED": true,
    "CREATE_DATE": "2020-01-01",
    "LAST_LOGIN": "2025-04-11T22:30:00+08:00",
    "GOV_ID": "gov_user_001",
    "GOV_PSW": "gov_password_hash",
    "REMARK": "VIP 會員",
    "LOGTIME": "2025-04-12T10:30:00+08:00"
  }
}
```
