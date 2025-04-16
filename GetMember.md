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
```json
