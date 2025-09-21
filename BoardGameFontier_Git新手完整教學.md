# 🚀 BoardGameFontier 專案 Git 完整新手教學

## 📋 前置準備

### 必備條件
- ✅ 已安裝 **Visual Studio 2022**
- ✅ 已安裝 **Git**（通常隨 Visual Studio 安裝）
- ✅ 已有 **GitHub 帳號**
- ✅ 已被加入專案的 **Collaborator**（請聯絡組長 Rko36）

### 檢查是否已安裝 Git
開啟命令提示字元或 PowerShell，輸入：
```bash
git --version
```
如果顯示版本號（如 `git version 2.x.x`），表示已安裝成功。

---

## 🎯 第一次加入專案完整流程

### 方法一：使用 Visual Studio 2022（推薦新手）

#### 1. 複製專案到本地
1. 開啟 **Visual Studio 2022**
2. 選擇 **「複製存放庫」**
3. 在 **「存放庫位置」** 輸入：
   ```
   https://github.com/Rko36/BoardGameFontierNew.git
   ```
4. 在 **「路徑」** 選擇你想放置專案的資料夾，例如：
   ```
   C:\Projects\BoardGameFontier
   ```
5. 點擊 **「複製」**

#### 2. 開啟專案
1. 複製完成後，Visual Studio 會自動載入專案
2. 確認開啟的是 `BoardGameFontier.csproj`

#### 3. 建立你的功能分支
1. 在 Visual Studio 右下角，點選 **「main」** 分支
2. 選擇 **「新增分支」**
3. 輸入分支名稱（根據你的負責模組）：
   - `feature/home` → 劉益辰（桌遊首頁）
   - `feature/social` → 廖昊威（社群留言互動）
   - `feature/trade` → 洪苡芯（二手交易）
   - `feature/member` → 陳建宇（個人資料系統）
   - `feature/admin` → 王品瑄（管理後台）

4. 點擊 **「建立分支」**

### 方法二：使用命令列（進階）

#### 1. 複製專案
```bash
git clone https://github.com/Rko36/BoardGameFontierNew.git
cd BoardGameFontier
```

#### 2. 建立功能分支
```bash
# 建立並切換到你的功能分支（替換 xxx 為你的模組名稱）
git checkout -b feature/xxx

# 例如：
git checkout -b feature/home        # 劉益辰
git checkout -b feature/social      # 廖昊威
git checkout -b feature/trade       # 洪苡芯
git checkout -b feature/member      # 陳建宇
git checkout -b feature/admin       # 王品瑄
```

---

## 💻 日常開發流程

### 🔧 每次開發前（重要！）

#### 使用 Visual Studio：
1. 點擊 **Git** → **拉取**
2. 確認你在自己的分支上（右下角顯示 `feature/你的模組名稱`）

#### 使用命令列：
```bash
# 1. 切換到主分支
git checkout main

# 2. 拉取最新變更
git pull origin main

# 3. 切換回你的分支
git checkout feature/你的模組名稱

# 4. 合併最新變更到你的分支
git merge main
```

### 🎨 開發你的功能

1. **打開 Visual Studio**，開始編寫程式碼
2. **根據你的分工**，主要修改以下檔案：

| 負責人 | 負責模組 | 主要工作檔案 |
|--------|----------|-------------|
| 劉益辰 | 桌遊首頁 | `Controllers/HomeController.cs`<br>`Models/Game*.cs`<br>`Views/Home/` |
| 廖昊威 | 社群互動 | `Models/Like.cs`<br>`Models/Follow.cs`<br>`Models/Notification.cs` |
| 洪苡芯 | 二手交易 | `Areas/Trade/`<br>`Models/Trade*.cs`<br>`Models/Cart*.cs` |
| 陳建宇 | 會員系統 | `Areas/Member/`<br>`Models/User*.cs`<br>`Models/Favorite.cs` |
| 王品瑄 | 管理後台 | `Areas/Admin/`<br>`Models/Report.cs`<br>`Models/Announcement.cs` |

3. **定期測試**你的功能是否正常運作

### 📤 提交你的變更

#### 使用 Visual Studio：
1. 在 **Git 變更** 視窗中，查看你修改的檔案
2. 在 **「提交訊息」** 欄位輸入描述，例如：
   ```
   完成使用者註冊功能
   ```
3. 點擊 **「全部提交」**
4. 點擊 **「推送」**

#### 使用命令列：
```bash
# 1. 查看你修改了哪些檔案
git status

# 2. 加入所有修改的檔案
git add .

# 3. 提交變更（請寫有意義的訊息）
git commit -m "完成使用者註冊功能"

# 4. 推送到 GitHub
git push origin feature/你的模組名稱
```

### 🔀 建立 Pull Request（合併請求）

1. 前往 **GitHub 網頁**：https://github.com/Rko36/BoardGameFontier
2. 你會看到黃色提示框顯示你剛推送的分支
3. 點擊 **「Compare & pull request」**
4. 填寫 Pull Request 內容：
   - **標題**：簡述你做了什麼（例如：「完成使用者註冊功能」）
   - **描述**：詳細說明你的修改內容
5. 確認 **「base: main」** ← **「compare: feature/你的模組名稱」**
6. 點擊 **「Create pull request」**
7. 等待組長審核並合併

---

## 🔄 同步其他人的最新修改

### 什麼時候需要同步？
- 開始新功能開發前
- 其他人的 Pull Request 被合併後
- 出現合併衝突時

### 如何同步：

#### 使用 Visual Studio：
1. 切換到 **main** 分支（右下角選擇）
2. 點擊 **Git** → **拉取**
3. 切換回你的分支（`feature/你的模組名稱`）
4. 點擊 **Git** → **合併**，選擇 **main** 分支

#### 使用命令列：
```bash
# 1. 切換到主分支
git checkout main

# 2. 拉取最新變更
git pull origin main

# 3. 切換回你的分支
git checkout feature/你的模組名稱

# 4. 合併主分支的變更
git merge main
```

---

## 🔧 常見問題與解決方法

### 問題 1：推送時出現錯誤
**錯誤訊息**：`Updates were rejected because the remote contains work that you do not have locally`

**解決方法**：
```bash
# 先拉取遠端變更
git pull origin feature/你的模組名稱

# 再推送
git push origin feature/你的模組名稱
```

### 問題 2：合併衝突
**症狀**：Git 說有檔案衝突

**解決方法**：
1. 打開 Visual Studio，會顯示衝突檔案
2. 點擊衝突檔案，選擇要保留的版本
3. 儲存檔案後重新提交

### 問題 3：不小心在 main 分支上修改
**解決方法**：
```bash
# 1. 暫存目前的修改
git stash

# 2. 切換到你的分支
git checkout feature/你的模組名稱

# 3. 恢復修改
git stash pop
```

### 問題 4：忘記自己在哪個分支
**解決方法**：
```bash
# 查看目前分支（有 * 的是目前分支）
git branch

# 或查看所有分支（包含遠端）
git branch -a
```

---

## 📚 Git 指令速查表

| 操作 | Visual Studio | 命令列 |
|------|-------------|--------|
| 複製專案 | 複製存放庫 | `git clone https://github.com/Rko36/BoardGameFontier.git` |
| 查看分支 | 右下角分支名稱 | `git branch` |
| 切換分支 | 點擊分支名稱選擇 | `git checkout 分支名稱` |
| 建立分支 | 新增分支 | `git checkout -b 新分支名稱` |
| 查看修改 | Git 變更視窗 | `git status` |
| 提交修改 | 全部提交 | `git add .` → `git commit -m "訊息"` |
| 推送 | 推送按鈕 | `git push origin 分支名稱` |
| 拉取 | 拉取按鈕 | `git pull origin 分支名稱` |

---

## 🎯 開發檢查清單

### 每次開發前：
- [ ] 確認在自己的分支上
- [ ] 拉取最新的 main 分支變更
- [ ] 合併到自己的分支

### 開發中：
- [ ] 定期提交變更（每完成一個小功能就提交）
- [ ] 提交訊息寫清楚做了什麼
- [ ] 確保程式碼可以編譯執行

### 完成功能後：
- [ ] 測試功能是否正常運作
- [ ] 提交所有變更
- [ ] 推送到 GitHub
- [ ] 建立 Pull Request
- [ ] 等待組長審核合併

---

## 🆘 需要幫助時

### 聯絡方式：
1. **團隊群組**：先在群組詢問
2. **GitHub Issues**：https://github.com/Rko36/BoardGameFontier/issues
3. **Rko36**：直接私訊

### 問題回報格式：
```
**問題描述**：
簡述遇到的問題

**錯誤訊息**：
貼上完整的錯誤訊息

**你嘗試的解決方法**：
說明你試過哪些方法

**截圖**：
如果有 UI 相關問題，請附上截圖
```

---

## 🎮 開始你的桌遊開發之旅！

現在你已經了解完整的 Git 流程，可以開始愉快地開發桌遊社群平台了！

**記住**：
- 🔄 **定期同步** main 分支的最新變更
- 💾 **頻繁提交** 你的修改
- 📝 **清楚描述** 你做了什麼
- 🤝 **團隊合作** 有問題隨時詢問

祝開發順利！讓我們一起打造最棒的桌遊社群平台！ 🎲✨
