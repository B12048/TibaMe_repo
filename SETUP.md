# 🛠️ 開發環境設定指南

本文件將指導團隊成員如何設定本地開發環境。

## 📋 環境需求檢查

### 必要軟體
- ✅ **Visual Studio 2022** (Community 版本即可)
- ✅ **.NET 8.0 SDK**
- ✅ **SQL Server LocalDB** (通常隨 Visual Studio 安裝)
- ✅ **Git** (通常隨 Visual Studio 安裝)

### 檢查是否已安裝
開啟命令提示字元或 PowerShell，執行以下指令：

```bash
# 檢查 .NET 版本
dotnet --version
# 應該顯示 8.0.x

# 檢查 SQL Server LocalDB
sqllocaldb info
# 應該顯示 MSSQLLocalDB 或類似項目

# 檢查 Git
git --version
# 應該顯示 git version x.x.x
```

## 🚀 專案設定步驟

### 1. Clone 專案

#### 方法一：使用 Visual Studio 2022
1. 開啟 Visual Studio 2022
2. 選擇「複製存放庫」
3. 輸入存放庫 URL：`https://github.com/Rko36/BoardGameFontierNew.git`
4. 選擇本地資料夾位置（建議：`C:\Projects\BoardGameFontier`）
5. 點選「複製」

#### 方法二：使用 Git 指令
```bash
git clone https://github.com/Rko36/BoardGameFontierNew.git
cd BoardGameFontier
```

### 2. 開啟專案
1. 在 Visual Studio 2022 中開啟 `BoardGameFontier.sln` 方案檔案
   - 或者開啟 `BoardGameFontier/BoardGameFontier.csproj` 專案檔案
2. 等待 Visual Studio 載入專案和相依套件

### 3. 還原 NuGet 套件
如果自動還原沒有執行：
1. 在 Visual Studio 中按右鍵「方案」
2. 選擇「還原 NuGet 套件」

或在 Package Manager Console 執行：
```bash
dotnet restore
```

### 4. 設定資料庫

#### 檢查連接字串
確認 `appsettings.json` 中的資料庫連接字串：
```json
{
  "ConnectionStrings": {
    "DefaultConnection": ""
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```
#### 建立個人密碼JSON字串
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=SQL1004.site4now.net;Database=db_abbdbc_tjm102db;User Id=個人帳號;Password=個人密碼;Encrypt=True;TrustServerCertificate=True;MultipleActiveResultSets=True;"
  }
}


### 5. 第一次執行
1. 在 Visual Studio 中按 **F5** 或點選「▶ 開始偵錯」
2. 瀏覽器應該自動開啟 `https://localhost:xxxx`
3. 看到桌遊開拓站首頁表示設定成功！

## 🌿 Git 分支設定

### 建立個人開發分支
依照您的負責模組建立分支：

```bash
# 先切換到 develop 分支
git checkout develop

# 建立並切換到個人功能分支
git checkout -b feature/[你的模組]

# 例如：
# 劉益辰：git checkout -b feature/home
# 廖昊威：git checkout -b feature/social  
# 洪苡芯：git checkout -b feature/trade
# 陳建宇：git checkout -b feature/member
# 王品瑄：git checkout -b feature/admin
```

### 設定上游分支
```bash
git push -u origin feature/[你的模組]
```

## 🔧 Visual Studio 個人設定

### 1. Git 設定
在 Visual Studio 中：
1. **工具** → **選項**
2. **原始檔控制** → **Git 全域設定**
3. 設定您的姓名和 Email

### 2. 預設分支設定
確保您的預設工作分支是自己的功能分支，避免直接在 `main` 或 `develop` 上工作。

### 3. 自動格式化設定
1. **工具** → **選項** → **文字編輯器** → **C#** → **程式碼樣式**
2. 確認縮排設定為 4 個空格
3. 建議啟用「儲存時格式化文件」

## 📂 開發檔案架構

### 各負責人主要工作目錄

| 負責人 | 主要工作目錄 | 檔案類型 |
|--------|-------------|----------|
| 劉益辰 | `Controllers/HomeController.cs`<br>`Models/Game*.cs`<br>`Services/IGameService.cs`<br>`DTOs/GameDto.cs` | 首頁相關功能 |
| 廖昊威 | `Models/Like.cs, Follow.cs, Notification.cs`<br>`Services/ISocialService.cs`<br>`DTOs/SocialDto.cs` | 社群互動功能 |
| 洪苡芯 | `Areas/Trade/`<br>`Models/Trade*.cs, Cart*.cs`<br>`Services/ITradeService.cs`<br>`DTOs/TradeDto.cs` | 交易功能 |
| 陳建宇 | `Areas/Member/`<br>`Models/User*.cs, Favorite.cs`<br>`Services/IUserService.cs`<br>`DTOs/UserDto.cs` | 會員系統 |
| 王品瑄 | `Areas/Admin/`<br>`Models/Report.cs, Announcement.cs`<br>`Services/IAdminService.cs`<br>`DTOs/AdminDto.cs` | 管理後台 |

## ⚠️ 常見問題解決

### 問題1：NuGet 套件還原失敗
**解決方案：**
1. 清除 NuGet 快取：**工具** → **NuGet 套件管理員** → **套件管理員設定** → **清除所有 NuGet 快取**
2. 重新還原套件

### 問題2：Migration 名稱衝突錯誤
**錯誤訊息：** `The name 'InitialCreate' is used by an existing migration.`

**解決方案：**
```bash
# 方法1：移除現有 Migration
Remove-Migration

# 然後重新建立
Add-Migration InitialCreate
Update-Database

# 方法2：使用不同名稱
Add-Migration FixDatabaseSchema
Update-Database
```

### 問題3：循環刪除路徑錯誤
**錯誤訊息：** `Introducing FOREIGN KEY constraint may cause cycles or multiple cascade paths`

**解決方案：**
```bash
# 這個問題已在最新版本中修正
# 如果遇到此問題，請先更新專案：
git pull origin main

# 然後重新建立資料庫
Drop-Database
Add-Migration InitialCreate
Update-Database
```

### 問題4：命名空間錯誤
**錯誤訊息：** `The type or namespace name 'BoardGameCommunity' could not be found`

**解決方案：**
```bash
# 在 Visual Studio 中使用尋找和取代 (Ctrl+H)
# 尋找：BoardGameCommunity
# 取代：BoardGameFontier
# 範圍：整個方案
# 點選「全部取代」
```

### 問題5：HTTPS 憑證問題
**解決方案：**
```bash
dotnet dev-certs https --trust
```

### 問題6：無法啟動專案
**檢查清單：**
- [ ] 是否有編譯錯誤？
- [ ] 資料庫是否正確建立？
- [ ] appsettings.json 設定是否正確？
- [ ] 是否有衝突的進程佔用 port 5001？

## 📞 求助管道

### 遇到問題時的解決順序：
1. **查看本文件** 的常見問題解決
2. **查看專案 README.md** 和 **CONTRIBUTING.md**
3. **在團隊群組提問** 並附上錯誤訊息截圖
4. **在 GitHub Issues** 中建立問題回報

### 問題回報格式：
```
**問題描述：**
簡述遇到的問題

**環境資訊：**
- 作業系統：
- Visual Studio 版本：
- .NET 版本：

**錯誤訊息：**
貼上完整的錯誤訊息

**重現步驟：**
1. 
2. 
3. 

**截圖：**
（如果有 UI 相關問題）
```

## ✅ 設定完成檢查表

在開始開發前，請確認：

- [ ] 專案可以正常編譯（Build → Build Solution）
- [ ] 資料庫連接正常（可以執行 Update-Database）
- [ ] 網站可以正常啟動（F5 執行）
- [ ] Git 分支設定正確（在自己的 feature 分支上工作）
- [ ] 可以正常 commit 和 push 變更

## 🎯 開發前準備

### 了解你的任務
1. 閱讀 `功能模組與分工.md` 了解自己的職責
2. 查看 `Models/` 目錄中標註你名字的檔案
3. 完善你負責的資料模型和服務介面
4. 在團隊群組確認開發規格

### 開始開發
現在你可以開始愉快地開發了！記住：
- 定期 commit 變更
- 遵循 CONTRIBUTING.md 的協作規範
- 有問題隨時在群組討論

---

🎮 **祝開發順利！讓我們一起打造最棒的桌遊社群平台！**