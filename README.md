我主要負責[後台管理系統模組開發與維護](https://docs.google.com/document/d/1dFz_QiE1LIfY9cWmRNQ0ofMatLuK5n3YlE3_3qXoCRo/edit?usp=drive_link)
## 功能
### 後臺分析
- 提供後臺統計數據報表
### 會員狀態與權限角色管理
- 提供後臺會員管理功能，可控管**會員角色**以及**登入狀態(啟用/停用)**
- 提供會員**篩選**功能
### 貼文／留言管理
- 提供後臺檢舉審核與管理功能(變更審核狀態、貼文篩選)
- 與會員狀態連動：檢舉次數過多時，關閉登入狀態

## 技術與架構
### 前端
- 使用 `Vue.js` 建置介面
-  `bootstrap` 美化前端顯示
-  `chart.js` 顯示後臺數據統計圖表
### 頁面與路由承載
- 以 `ASP.NET Core MVC` 作為頁面載入與路由框架
### 後端
- 提供 RESTful API 服務 給前端使用
### 資料存取
- 透過 `Entity Framework Core` 與資料庫溝通
### 資料庫
- Microsoft SQL Server (MSSQL)
