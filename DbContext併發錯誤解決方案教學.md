# DbContext 併發錯誤解決方案教學

## 📋 問題背景

在實作**追蹤功能**時，我們遇到了一個嚴重的 Entity Framework Core 併發錯誤：

```
System.InvalidOperationException: A second operation was started on this context instance before a previous operation was completed. 
This is usually caused by different threads using the same instance of DbContext concurrently, which is not supported.
```

## 🔍 問題發現過程

### 1. **觸發場景**
- **追蹤功能**：用戶在社群頁面進行追蹤/取消追蹤操作
- **背景服務**：Hangfire 定期執行資料統計任務
- **Partial View 渲染**：`_LoginPartial.cshtml` 多次調用 `CurrentUserService.GetUserDisplayNameAsync()`

### 2. **錯誤徵象**
```csharp
// 在 FollowService.GetFollowCountsAsync() 中出現
using var context = _context; // ❌ 錯誤：共享 DbContext 實例
var followersTask = context.Set<Follow>().CountAsync(...);
var followingTask = context.Set<Follow>().CountAsync(...); // ⚠️ 第二個操作觸發併發錯誤
```

### 3. **根本原因分析**

#### **併發衝突來源：**
1. **Scoped DbContext 生命週期**：同一個請求內所有服務共享同一個 DbContext 實例
2. **並行查詢操作**：在 `GetFollowCountsAsync()` 中同時執行多個 `CountAsync()` 操作
3. **Partial View 重複渲染**：導致多個組件同時存取同一個 DbContext

#### **技術細節：**
- EF Core 的 `DbContext` **不是執行緒安全的**
- `AddDbContext` 註冊為 **Scoped** 生命週期，每個請求共享一個實例
- Hangfire 背景任務與 HTTP 請求同時存取時產生競爭條件

## 💡 解決方案實作

### **階段一：採用 DbContextFactory 模式**

#### **1. 修改服務註冊 (Program.cs)**

```csharp
// ❌ 問題原始碼
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});

// ✅ 解決方案
builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
           .ConfigureWarnings(warnings => 
               warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.MultipleCollectionIncludeWarning));
});

// 向後相容性包裝器
builder.Services.AddScoped<ApplicationDbContext>(provider =>
{
    var factory = provider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
    return factory.CreateDbContext();
});
```

#### **2. 重構服務類別注入**

```csharp
// ❌ 原始 FollowService
public class FollowService : IFollowService
{
    private readonly ApplicationDbContext _context; // ⚠️ 共享實例

    public FollowService(ApplicationDbContext context) // ❌ Scoped 注入
    {
        _context = context;
    }
}

// ✅ 改善後 FollowService
public class FollowService : IFollowService
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory; // ✅ Factory 注入

    public FollowService(IDbContextFactory<ApplicationDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }
}
```

### **階段二：修正併發查詢邏輯**

#### **問題方法重構：**

```csharp
// ❌ 併發錯誤版本
public async Task<(int FollowersCount, int FollowingCount)> GetFollowCountsAsync(string userId)
{
    var followersTask = _context.Set<Follow>().CountAsync(f => f.FolloweeId == userId);
    var followingTask = _context.Set<Follow>().CountAsync(f => f.FollowerId == userId);
    // ⚠️ 同一個 DbContext 實例執行多個並行操作 → 併發錯誤
    
    var results = await Task.WhenAll(followersTask, followingTask);
    return (results[0], results[1]);
}

// ✅ 解決方案版本
public async Task<(int FollowersCount, int FollowingCount)> GetFollowCountsAsync(string userId)
{
    /*
     * 併發安全設計：
     * 1. 使用兩個獨立的 DbContext 實例避免並發衝突
     * 2. 透過 Task.WhenAll 同時執行查詢提升效能
     * 3. 使用 using 確保資源正確釋放
     */
    using var context1 = _contextFactory.CreateDbContext();
    using var context2 = _contextFactory.CreateDbContext();

    var followersTask = context1.Set<Follow>()
        .CountAsync(f => f.FolloweeId == userId);  // 獨立 DbContext 1

    var followingTask = context2.Set<Follow>()
        .CountAsync(f => f.FollowerId == userId);  // 獨立 DbContext 2

    var results = await Task.WhenAll(followersTask, followingTask);
    return (results[0], results[1]);
}
```

### **階段三：查詢分割行為配置**

```csharp
// 解決 QuerySplittingBehavior 警告
builder.Services.AddDbContextFactory<ApplicationDbContext>(options =>
{
    options.UseSqlServer(connectionString)
           .ConfigureWarnings(warnings => 
               warnings.Ignore(RelationalEventId.MultipleCollectionIncludeWarning));
});
```

## 🎯 解決效果

### **Before (問題狀態):**
```
❌ System.InvalidOperationException: A second operation was started on this context instance
❌ QuerySplittingBehavior 未配置警告
❌ 追蹤功能在高併發下不穩定
❌ 背景服務與前台功能衝突
```

### **After (解決後):**
```
✅ DbContext 併發錯誤完全解決
✅ 查詢分割警告消除
✅ 追蹤功能穩定運行
✅ 背景服務與前台功能和諧共存
✅ 效能提升（並行查詢）
```

## 📈 架構優勢

### **1. DbContextFactory 模式優點**
- **執行緒安全**：每個操作使用獨立的 DbContext 實例
- **效能優化**：支援真正的並行查詢
- **資源管理**：明確的生命週期控制
- **可擴展性**：適合高併發場景

### **2. 相容性保證**
```csharp
// 向後相容包裝器確保現有程式碼無需大幅修改
builder.Services.AddScoped<ApplicationDbContext>(provider =>
{
    var factory = provider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
    return factory.CreateDbContext();
});
```

## 🔧 實作檢查清單

### **服務層修改：**
- [ ] 將 `ApplicationDbContext` 注入改為 `IDbContextFactory<ApplicationDbContext>`
- [ ] 所有資料庫操作使用 `using var context = _contextFactory.CreateDbContext();`
- [ ] 並行查詢使用獨立的 DbContext 實例

### **Program.cs 配置：**
- [ ] 註冊 `AddDbContextFactory` 取代 `AddDbContext`
- [ ] 配置 `ConfigureWarnings` 處理查詢分割警告
- [ ] 加入向後相容性包裝器

### **測試驗證：**
- [ ] 高併發追蹤操作測試
- [ ] Partial View 多重渲染測試
- [ ] 背景服務與前台功能同時運行測試

## 📚 學習重點

1. **DbContext 不是執行緒安全的**，共享實例會導致併發錯誤
2. **DbContextFactory 模式**是處理高併發場景的最佳實作
3. **並行查詢**需要使用獨立的 DbContext 實例
4. **依賴注入生命週期**選擇會直接影響併發行為
5. **向後相容性設計**可以降低重構成本

## 🎓 結論

這次 DbContext 併發錯誤的解決過程展示了：
- **問題診斷**：從錯誤訊息追蹤到根本原因
- **架構重構**：採用更適合的設計模式
- **漸進式改善**：保持向後相容性的同時進行架構升級
- **效能優化**：在解決問題的同時提升系統效能

透過 DbContextFactory 模式，我們不僅解決了併發問題，還為未來的高併發需求建立了更穩固的基礎架構。

---

*此文件記錄了追蹤功能實作過程中遇到的 DbContext 併發錯誤及其完整解決方案，作為未來類似問題的參考指南。*