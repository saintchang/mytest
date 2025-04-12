# Login API 說明文件

## 📘 功能簡介
使用者透過輸入 `id` 和 `password` 進行身分驗證，驗證成功後系統會回傳使用者資訊與 JWT Token。

---

## 📥 API 請求資訊

- **方法**：`POST`
- **URL**：`/api/v1/login`
- **Content-Type**：`application/json`

### ✅ 請求參數

| 參數名稱   | 型別     | 必填 | 說明           |
|------------|----------|------|----------------|
| id         | string   | 是   | 使用者帳號，對應資料表欄位 `MM_NO` |
| password   | string   | 是   | 使用者密碼，對應資料表欄位 `PSW`   |

#### 📦 請求範例
```json
{
  "id": "testUser",
  "password": "abc123"
}
