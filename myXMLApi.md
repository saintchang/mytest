# .NET Web API 中繼代理層規範

## 概述
此 API 作為內部服務的代理（Proxy），隱藏後端 XML Server 的實體路徑與認證資訊，並負責傳遞上行與下行的 XML 資料封包。

## 技術架構
  - 框架: .NET 10 Web API。
  - 通訊組件: IHttpClientFactory (避免 Socket 耗盡)。
  - 注入模式: 採用 Scoped 或 Singleton 管理遠端服務配置。

## API 定義 (Endpoint Definition)
  - Route: POST /api/v1/proxy/dispatch
  - Request Header:
    - Content-Type: application/xml
    - X-Correlation-ID: (選填) 用於跨系統日誌追蹤。
  - Request Body: 原始 XML Data (Raw)。

## 邏輯流程 (Internal Logic)
  - 輸入攔截: 檢查 Request.Body 是否為空。
  - 標頭映射: 將前端傳入的特定標頭（如語系、Token）映射至發往 XML Server 的請求中。
  - 轉發執行:
    - 使用 PostAsync 或 SendAsync。
    - 實作 Circuit Breaker (熔斷機制)，若遠端服務異常則立即回傳 503。
  - 回應處理:
    - 將遠端 XML Server 的 StatusCode 透傳回 React 或是進行封裝轉換。
    - 確保回傳的 Content-Type 維持為 application/xml。

## 監控與安全
  - Logging: 記錄每次請求的 RequestSize、ResponseTime 以及 CorrelationID。
  - Security: 限制僅允許特定來源的網域（CORS Policy）進行調用。
  - Memory: 針對大檔案應直接操作 Stream 進行轉發，不進行字串序列化以節省記憶體空間。
