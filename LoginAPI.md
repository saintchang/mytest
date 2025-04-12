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
```

## 📤 API 回應資訊

### 回傳欄位

| 欄位名稱     | 型別     | 說明                                                                 |
|--------------|----------|----------------------------------------------------------------------|
| status       | boolean  | 登入結果，驗證成功為 `true`，失敗為 `false`                         |
| return_code  | string   | 功能執行結果碼：成功為 `0000`，其他為錯誤代碼                       |
| message      | string   | 對應 `return_code` 的說明文字                                       |
| tokenn       | string   | 登入成功後產生的 JWT Token                                          |
| data         | object   | 登入成功時的使用者資料物件，失敗則為 `null`                         |

---

## 🔹 data 欄位內容

| 欄位名稱  | 型別   | 說明                 | 對應資料表欄位 |
|-----------|--------|----------------------|----------------|
| mm_name   | string | 使用者名稱           | MM_NAME        |
| logtime   | string | 登入時間（ISO 格式） | 登入時伺服器產生 |
| gov_id    | string | 政府帳號 ID          | GOV_ID         |
| gov_psw   | string | 政府帳號密碼         | GOV_PSW        |

---

## 🎯 成功回應範例（登入成功）

```json
{
  "status": true,
  "return_code": "0000",
  "message": "Login successful",
  "tokenn": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "data": {
    "mm_name": "王小明",
    "logtime": "2025-04-12T10:15:30+08:00",
    "gov_id": "gov12345",
    "gov_psw": "encrypted_password"
  }
}
```

## ❌ 成功回應範例（登入成功）

```json

{
  "status": false,
  "return_code": "1001",
  "message": "Invalid ID or password",
  "tokenn": "",
  "data": null
}
```

