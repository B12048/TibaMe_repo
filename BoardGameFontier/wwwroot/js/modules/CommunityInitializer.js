/**
 * CommunityInitializer - 社群頁面初始化協調器
 * 
 *  筆記：應用程式初始化模式
 * 
 * 設計理念：
 * - 中央集權式初始化：統一管理所有模組的初始化流程
 * - 依賴檢查機制：確保所有必要模組都已載入
 * - 錯誤處理友善：提供清楚的錯誤訊息和恢復選項
 * - 非同步協調：管理複雜的初始化時序
 * 
 * 主要職責：
 * - Vue.js 應用程式初始化和掛載
 * - 外部管理器實例化和依賴注入
 * - Modal 和表單元件設定
 * - 全域錯誤處理和回復機制
 * 
 * 技術特色：
 * - ES6 Class 語法，符合現代 JavaScript 標準
 * - 完整的生命週期管理
 * - 模組化依賴檢查系統
 * - 與 ASP.NET Core 頁面載入週期整合
 */
class CommunityInitializer {
    /**
     * 筆記：ES6 Class Constructor
     * 
     * 初始化實例狀態：
     * - vueInstance: 保存 Vue 應用程式實例的參考
     * - isInitialized: 追蹤初始化狀態，避免重複初始化
     */
    constructor() {
        this.vueInstance = null;     // Vue 實例參考
        this.isInitialized = false;  // 初始化狀態標記
    }

    /**
     * 筆記：主要初始化流程協調
     * 
     * 非同步初始化模式：
     * - 使用 async/await 處理非同步操作
     * - 按順序執行各個初始化步驟
     * - 統一的錯誤處理和使用者回饋
     * 
     * 初始化步驟：
     * 1. 設定 UI 元件初始狀態
     * 2. 創建並掛載 Vue 應用程式
     * 3. 初始化外部管理器模組
     * 4. 設定表單和事件處理器
     */
    async initialize() {
        try {
            // 第一步：準備 UI 環境
            this.setupModalInitialState();

            // 第二步：初始化 Vue.js 核心應用程式
            await this.initializeVueApp();

            // 第三步：初始化輔助功能模組
            this.initializeManagers();

            // 第四步：設定使用者互動元件
            this.initializeCreatePostForm();

            // 第五步：標記初始化完成
            this.isInitialized = true;
            // console.log('社群頁面初始化成功'); // 生產環境移除

        } catch (error) {
            console.error('社群頁面初始化失敗:', error);
            this.showInitializationError();
        }
    }

    /**
     * 筆記：Bootstrap Modal 初始化
     * 
     * 解決 Bootstrap 與 Vue.js 整合問題：
     * - 確保 Modal 在頁面載入時是隱藏狀態
     * - 避免與 Vue.js 的條件渲染衝突
     */
    setupModalInitialState() {
        const modal = document.getElementById('createPostModal');
        if (modal) {
            modal.style.display = 'none';  // 強制隱藏，避免初始化閃爍
        }
    }

    /**
     * 筆記：Vue.js 3 應用程式初始化
     * 
     * Vue.js 3 建立應用程式的標準流程：
     * 1. 檢查依賴模組是否都已載入
     * 2. 使用 createApp() 建立應用程式實例
     * 3. 將應用程式掛載到指定的 DOM 元素
     * 4. 暴露實例給其他模組使用
     * 
     * 與 Vue 2 的差異：
     * - Vue 3: createApp(component).mount(selector)
     * - Vue 2: new Vue({ el: selector, ...component })
     */
    async initializeVueApp() {
        // 依賴檢查：確保所有必要模組都已載入
        const dependencies = this.checkDependencies();
        if (!dependencies.allLoaded) {
            throw new Error('必要模組未載入: ' + dependencies.missing.join(', '));
        }

        // Vue.js 3 應用程式建立和掛載
        const { createApp } = Vue;                            // 解構 Vue 3 的 createApp 函數
        const app = createApp(window.CommunityPostsApp);       // 建立應用程式實例
        this.vueInstance = app.mount('#community-posts-app'); // 掛載到 DOM 元素

        // 全域暴露：讓其他模組可以存取 Vue 實例
        window.vueApp = this.vueInstance;

        return this.vueInstance;
    }

    /**
     * 筆記：依賴注入模式實作
     * 
     * 設計模式：Service Locator + Dependency Injection
     * - 檢查全域模組是否存在，避免未定義錯誤
     * - 建立實例並注入依賴關係
     * - 暴露到全域作用域，讓其他模組可以使用
     * 
     * 管理器層級架構：
     * - UIManager: 基礎 UI 操作（Toast、Modal 等）
     * - FollowManager: 業務邏輯（追蹤功能）
     * - 其他管理器依照需要擴展
     */
    initializeManagers() {
        //基礎 UI 管理器（其他管理器的依賴）
        if (window.UIManager) {
            window.uiManager = new window.UIManager();
            // console.log('UIManager 初始化成功'); // 生產環境移除
        } else {
            console.warn('UIManager 模組未載入');
        }

        //追蹤功能管理器（依賴 UIManager）
        if (window.FollowManager && window.uiManager) {
            window.followManager = new window.FollowManager(window.uiManager);
            // console.log('FollowManager 初始化成功'); // 生產環境移除
        } else {
            console.warn('FollowManager 或其依賴未載入');
        }

        //未來可以在這裡初始化更多管理器...
        // if (window.NotificationManager) { ... }
        // if (window.ChatManager) { ... }
    }

    /**
     * 筆記：表單初始化委託模式
     * 
     * 為什麼使用委託？
     * - 表單邏輯可能在不同的 JavaScript 檔案中
     * - 避免直接依賴，提高模組化程度
     * - 可以動態決定要使用哪個表單初始化函數
     */
    initializeCreatePostForm() {
        if (window.initializeCreatePostForm) {
            window.initializeCreatePostForm();
            // console.log('發表貼文表單初始化成功'); // 生產環境移除
        } else {
            console.warn('initializeCreatePostForm 函數不存在，表單功能可能受限');
        }
    }

    /**
     * 筆記：模組依賴檢查系統
     * 
     * 依賴檢查的重要性：
     * - 避免 "xxx is not defined" 的執行時錯誤
     * - 提供清楚的錯誤訊息協助除錯
     * - 確保載入順序正確性
     * 
     * 檢查策略：
     * - typeof 檢查：適用於可能未定義的全域變數
     * - !! 轉換：將 undefined 轉為 false，其他轉為 true
     * - 集中管理：統一定義所有必要的依賴模組
     * 
     * @returns {Object} 包含依賴狀態的詳細報告
     */
    checkDependencies() {
        // 定義所有必要的依賴模組清單
        const dependencies = {
            Vue: typeof Vue !== 'undefined',                    // Vue.js 3 核心庫
            CommunityPostsApp: !!window.CommunityPostsApp,      // 主要 Vue 應用程式元件
            PaginationManager: !!window.PaginationManager,      // 分頁功能管理器
            UIManager: !!window.UIManager,                      // 基礎 UI 操作管理器
            FollowManager: !!window.FollowManager               // 追蹤功能管理器
        };

        // 找出未載入的模組
        const missing = Object.keys(dependencies).filter(dep => !dependencies[dep]);
        const allLoaded = missing.length === 0;

        // 除錯資訊：在開發環境顯示依賴狀態
        // console.log('依賴檢查結果:', {
        //     總計: Object.keys(dependencies).length,
        //     已載入: Object.keys(dependencies).length - missing.length,
        //     未載入: missing.length,
        //     狀態: dependencies
        // }); // 生產環境移除

        return {
            dependencies,  // 詳細的依賴狀態物件
            missing,       // 未載入的模組名稱陣列
            allLoaded      // 是否全部載入成功
        };
    }

    /**
     * 顯示初始化錯誤
     */
    showInitializationError() {
        const dependencies = this.checkDependencies();
        
        console.error('必要模組未載入:', dependencies);
        
        // 顯示用戶友好的錯誤訊息
        const errorMessage = '社群頁面載入失敗，請重新整理頁面。';
        
        // 創建錯誤提示元素
        const errorDiv = document.createElement('div');
        errorDiv.className = 'alert alert-danger';
        errorDiv.innerHTML = `
            <i class="bi bi-exclamation-triangle"></i>
            ${errorMessage}
            <button class="btn btn-outline-danger btn-sm ms-2" onclick="window.location.reload()">
                <i class="bi bi-arrow-clockwise"></i> 重新整理
            </button>
        `;

        // 插入到主容器中
        const mainContent = document.getElementById('mainContentArea');
        if (mainContent) {
            mainContent.insertBefore(errorDiv, mainContent.firstChild);
        }
    }

    /**
     * 取得 Vue 實例
     * @returns {Object} Vue 實例
     */
    getVueInstance() {
        return this.vueInstance;
    }

    /**
     * 檢查是否已初始化
     * @returns {boolean} 初始化狀態
     */
    isReady() {
        return this.isInitialized;
    }
}

/**
 * 總結：CommunityInitializer 設計模式
 * 
 * 展示的設計模式：
 * 1. **初始化器模式 (Initializer Pattern)**：集中管理複雜的初始化邏輯
 * 2. **依賴注入 (Dependency Injection)**：管理模組間的依賴關係
 * 3. **服務定位器 (Service Locator)**：透過全域註冊來查找服務
 * 4. **錯誤處理模式**：統一的錯誤捕獲和使用者回饋
 * 
 * 與 ASP.NET Core 整合特色：
 * - DOMContentLoaded 事件確保 DOM 完全載入
 * - 全域暴露模式讓 Razor 頁面可以存取 JavaScript 物件
 * - 錯誤處理提供使用者友善的重新載入選項
 * 
 * 可擴展性設計：
 * - 模組化依賴檢查，新增模組時容易擴展
 * - 統一的管理器初始化模式
 * - 非同步支援，適合載入外部資源
 * 
 * 最佳實踐：
 * - ES6 Class 語法提供清晰的物件導向結構
 * - 詳細的日誌記錄協助開發除錯
 * - 防禦性程式設計避免執行時錯誤
 */

// 將類別註冊到全域作用域，供其他模組使用
window.CommunityInitializer = CommunityInitializer;

/**
 * 筆記：自動初始化模式
 * 
 * DOMContentLoaded vs window.onload：
 * - DOMContentLoaded: DOM 解析完成就觸發（較快）
 * - window.onload: 所有資源（圖片、CSS）都載入完成才觸發
 * 
 * 初始化時機選擇：
 * - 這裡選擇 DOMContentLoaded 因為 Vue.js 主要需要 DOM 結構
 * - 圖片和 CSS 的載入不影響 JavaScript 邏輯執行
 */
document.addEventListener('DOMContentLoaded', function() {
    // console.log('開始初始化社群頁面...'); // 生產環境移除
    
    const initializer = new CommunityInitializer();
    window.communityInitializer = initializer;  // 全域暴露，方便除錯和其他模組存取
    
    // 開始非同步初始化流程
    initializer.initialize();
});