/**
 * UIManager - 使用者介面管理模組
 * 
 * 📝 學習筆記：UI 基礎設施模式
 * 
 * 💡 設計理念：
 * - 集中式 UI 管理：統一處理 Toast、載入狀態、面板切換
 * - 與 Bootstrap 5 深度整合：使用官方元件 API
 * - 簡化呼叫介面：提供語意清晰的方法名稱
 * - 自動資源管理：自動建立容器和清理資源
 * 
 * 🚀 主要功能：
 * - Toast 通知系統：成功、錯誤、資訊訊息
 * - 載入狀態管理：統一的載入指示器控制
 * - 視圖切換：列表和詳情視圖的切換邏輯
 * - DOM 元素管理：自動建立和清理 UI 容器
 * 
 * 🔧 技術特色：
 * - 依賴注入友善：作為其他管理器的基礎依賴
 * - Bootstrap 5 API 整合：原生元件事件處理
 * - 記憶體管理：自動清理過期的 Toast 元素
 * - 容錯設計：處理 DOM 元素不存在的情況
 */
class UIManager {
    /**
     * 📝 學習筆記：DOM 元素快取模式
     * 
     * 💡 為什麼要快取 DOM 元素？
     * - 效能優化：避免重複查詢 DOM
     * - 一致性：確保操作的是同一個元素
     * - 容錯性：統一處理元素不存在的情況
     */
    constructor() {
        // 📝 快取常用的 DOM 元素參考
        this.loadingIndicator = document.getElementById('postsLoadingIndicator');
        this.postContentContainer = document.getElementById('postContentContainer');
        this.currentView = 'list'; // 追蹤目前的視圖狀態：'list' 或 'detail'
    }

    // ===== Toast 通知系統 =====

    /**
     * 📝 學習筆記：Bootstrap 5 Toast 整合
     * 
     * 💡 Toast 通知系統設計：
     * - 自動建立容器：首次使用時才建立 Toast 容器
     * - 唯一 ID 生成：使用時間戳避免 ID 衝突
     * - 自動清理：Toast 隱藏後自動從 DOM 移除
     * - 無障礙支援：使用 ARIA 標籤提升可訪問性
     * 
     * 🚀 Bootstrap 5 Toast API 使用：
     * - new bootstrap.Toast(element, options)：建立 Toast 實例
     * - toast.show()：顯示 Toast
     * - 'hidden.bs.toast' 事件：Toast 完全隱藏後觸發
     * 
     * @param {string} message - 訊息內容
     * @param {string} type - 訊息類型 (success, danger, info, warning)
     */
    showToast(message, type = 'info') {
        // 📝 第一步：確保 Toast 容器存在（延遲建立模式）
        let container = document.getElementById('toastContainer');
        if (!container) {
            container = document.createElement('div');
            container.id = 'toastContainer';
            container.className = 'toast-container position-fixed top-0 end-0 p-3';
            document.body.appendChild(container);
        }
        
        // 📝 第二步：生成唯一 ID 和 Toast HTML
        const toastId = 'toast-' + Date.now();  // 時間戳確保唯一性
        const toastHTML = `
            <div id="${toastId}" class="toast align-items-center text-bg-${type} border-0" role="alert" aria-live="assertive" aria-atomic="true">
                <div class="d-flex">
                    <div class="toast-body">${message}</div>
                    <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button>
                </div>
            </div>`;
        
        // 📝 第三步：插入 DOM 並初始化 Bootstrap Toast
        container.insertAdjacentHTML('beforeend', toastHTML);
        const toastElement = document.getElementById(toastId);
        const bsToast = new bootstrap.Toast(toastElement, { delay: 5000 });  // 5秒後自動隱藏
        
        // 📝 第四步：顯示 Toast 並設定清理機制
        bsToast.show();
        toastElement.addEventListener('hidden.bs.toast', () => toastElement.remove());  // 自動清理
    }

    /**
     * 📝 學習筆記：便利方法模式 (Convenience Methods)
     * 
     * 💡 設計目的：
     * - 簡化常用操作：不用每次都指定類型參數
     * - 語意清晰：方法名稱直接表達意圖
     * - 向下相容：提供多種命名風格的別名
     */
    
    /** 顯示成功訊息 */
    showSuccessToast(message) { 
        this.showToast(message, 'success'); 
    }

    /** 顯示錯誤訊息 */
    showErrorToast(message) { 
        this.showToast(message, 'danger'); 
    }

    /** 顯示資訊訊息 */
    showInfoToast(message) { 
        this.showToast(message, 'info'); 
    }

    /** 顯示成功訊息 (別名方法，相容不同的命名習慣) */
    showSuccessMessage(message) {
        this.showSuccessToast(message);
    }

    /** 顯示錯誤訊息 (別名方法，相容不同的命名習慣) */
    showErrorMessage(message) {
        this.showErrorToast(message);
    }

    // ===== 載入狀態管理 =====

    /**
     * 顯示載入指示器
     */
    showLoadingIndicator() {
        if (this.loadingIndicator) {
            this.loadingIndicator.style.display = 'block';
        }
    }

    /**
     * 隱藏載入指示器
     */
    hideLoadingIndicator() {
        if (this.loadingIndicator) {
            this.loadingIndicator.style.display = 'none';
        }
    }

    /**
     * 顯示貼文載入錯誤
     */
    showPostsError() {
        const container = document.getElementById('dynamicPostsContainer');
        if (container) {
            container.innerHTML = `
                <div class="text-center py-5 text-muted">
                    <i class="bi bi-exclamation-triangle" style="font-size: 3rem;"></i>
                    <h5 class="mt-3">載入失敗</h5>
                    <p>無法載入貼文，請稍後再試</p>
                    <button class="btn btn-outline-primary" onclick="window.communityApp.loadPosts()">
                        <i class="bi bi-arrow-clockwise me-1"></i>重新載入
                    </button>
                </div>
            `;
        }
    }

    /**
     * 顯示無貼文狀態
     */
    showNoPosts() {
        const container = document.getElementById('dynamicPostsContainer');
        if (container) {
            container.innerHTML = `
                <div class="text-center py-5 text-muted">
                    <i class="bi bi-file-earmark-text" style="font-size: 3rem;"></i>
                    <h5 class="mt-3">暫無貼文</h5>
                    <p>目前沒有符合條件的貼文</p>
                </div>
            `;
        }
    }

    // ===== 面板切換管理 =====

    /**
     * 顯示貼文列表（從詳細頁面返回）- 教學專案簡化版
     */
    showPostsList() {
        const vueApp = document.getElementById('community-posts-app');
        const fullPanel = document.getElementById('postDetailFullPanel');
        
        if (vueApp && fullPanel) {
            // 清理詳情面板內容，避免 CSS 樣式污染
            fullPanel.innerHTML = '';
            fullPanel.style.display = 'none';
            vueApp.style.display = 'block';
            
            // 清除 URL hash
            if (window.location.hash) {
                history.replaceState(null, null, window.location.pathname);
            }
            
            this.currentView = 'list';
            // console.log('✅ 已返回 Vue.js 貼文列表'); // 生產環境移除
        }
    }

    /**
     * 顯示貼文詳細面板 - 教學專案簡化版
     */
    showPostDetail() {
        const vueApp = document.getElementById('community-posts-app');
        const fullPanel = document.getElementById('postDetailFullPanel');
        
        if (vueApp && fullPanel) {
            vueApp.style.display = 'none';
            fullPanel.style.display = 'block';
            this.currentView = 'detail';
            // console.log('✅ 已切換到貼文詳細面板'); // 生產環境移除
        }
    }

    /**
     * 顯示面板錯誤
     * @param {string} errorMessage - 錯誤訊息
     */
    showPanelError(errorMessage = '載入失敗，請稍後再試') {
        const fullPanel = document.getElementById('postDetailFullPanel');
        if (fullPanel) {
            fullPanel.innerHTML = `
                <div class="text-center py-5 text-muted">
                    <i class="bi bi-exclamation-triangle" style="font-size: 3rem;"></i>
                    <h5 class="mt-3">載入失敗</h5>
                    <p>${errorMessage}</p>
                    <button class="btn btn-outline-primary" onclick="window.communityApp.showPostsList()">
                        <i class="bi bi-arrow-left me-1"></i>返回列表
                    </button>
                </div>
            `;
            this.showPostDetail();
        }
    }

    /**
     * 渲染貼文詳細內容到面板
     * @param {string} html - 詳細內容 HTML
     */
    renderPostDetailInPanel(html) {
        const fullPanel = document.getElementById('postDetailFullPanel');
        if (fullPanel) {
            fullPanel.innerHTML = html;
            this.showPostDetail();
        }
    }

    /**
     * 在貼文詳情面板顯示載入中動畫
     */
    showPostDetailLoading() {
        const fullPanel = document.getElementById('postDetailFullPanel');
        if (fullPanel) {
            fullPanel.innerHTML = `
                <div class="text-center py-5 text-muted">
                    <div class="spinner-border text-primary" style="width: 3rem; height: 3rem;" role="status"></div>
                    <h5 class="mt-3">載入中...</h5>
                    <p>正在取得貼文內容，請稍候</p>
                </div>
            `;
            this.showPostDetail();
        }
    }

    // ===== 搜尋介面管理 =====

    /**
     * 更新搜尋狀態顯示
     * @param {string} keyword - 搜尋關鍵字
     * @param {number} resultCount - 結果數量
     */
    updateSearchStatus(keyword, resultCount) {
        // 可以在這裡添加搜尋狀態的 UI 更新
        if (keyword) {
            // console.log(`搜尋 "${keyword}" 找到 ${resultCount} 個結果`); // 生產環境移除
        }
    }

    /**
     * 清除搜尋狀態
     */
    clearSearchStatus() {
        const searchInput = document.querySelector('.search-input');
        if (searchInput) {
            searchInput.value = '';
        }
    }

    // ===== 模態框管理 =====

    /**
     * 關閉指定模態框
     * @param {string} modalId - 模態框 ID
     */
    closeModal(modalId) {
        const modalElement = document.getElementById(modalId);
        if (modalElement && typeof bootstrap !== 'undefined') {
            const bsModal = bootstrap.Modal.getInstance(modalElement);
            if (bsModal) {
                bsModal.hide();
            }
        }
    }

    /**
     * 開啟指定模態框
     * @param {string} modalId - 模態框 ID
     */
    openModal(modalId) {
        const modalElement = document.getElementById(modalId);
        if (modalElement && typeof bootstrap !== 'undefined') {
            const bsModal = new bootstrap.Modal(modalElement);
            bsModal.show();
        }
    }

    // ===== 狀態檢查方法 =====

    /**
     * 檢查當前視圖狀態
     * @returns {string} 當前視圖類型
     */
    getCurrentView() {
        return this.currentView;
    }

    /**
     * 是否為列表視圖
     * @returns {boolean}
     */
    isListView() {
        return this.currentView === 'list';
    }

    /**
     * 是否為詳細視圖
     * @returns {boolean}
     */
    isDetailView() {
        return this.currentView === 'detail';
    }

    // ===== 滾動管理 =====

    /**
     * 滾動到頁面頂部
     */
    scrollToTop() {
        window.scrollTo({ 
            top: 0, 
            behavior: 'smooth' 
        });
    }

    /**
     * 滾動到留言區域
     */
    scrollToComments() {
        const commentSection = document.getElementById('commentSection');
        if (commentSection) {
            commentSection.scrollIntoView({ 
                behavior: 'smooth', 
                block: 'nearest' 
            });
        }
    }

    // ===== 輔助方法 =====

    /**
     * 顯示確認對話框
     * @param {string} message - 確認訊息
     * @returns {boolean} 用戶確認結果
     */
    confirm(message) {
        return window.confirm(message);
    }

    /**
     * 重新導向到指定頁面
     * @param {string} url - 目標 URL
     */
    redirectTo(url) {
        window.location.href = url;
    }
}

/**
 * 📝 學習總結：UIManager 設計模式展示
 * 
 * 🎯 展示的設計模式和概念：
 * 1. **基礎設施模式 (Infrastructure Pattern)**：提供基礎 UI 服務
 * 2. **DOM 元素快取**：效能優化的常見手法
 * 3. **便利方法模式**：簡化常用操作的介面設計
 * 4. **狀態管理**：追蹤和管理 UI 狀態
 * 5. **容錯設計**：防禦性程式設計處理異常情況
 * 
 * 💡 實用的 JavaScript 技巧：
 * - insertAdjacentHTML：比 innerHTML 更安全的 DOM 操作
 * - Bootstrap 5 API 整合：原生 JavaScript 與 CSS 框架協作
 * - 事件監聽器清理：避免記憶體洩漏的重要實踐
 * - 滾動 API：smooth scrolling 提升使用者體驗
 * 
 * 🚀 模組化設計特色：
 * - 無外部依賴：只依賴 Bootstrap 5 和瀏覽器 API
 * - 可測試性：方法職責清晰，易於單元測試
 * - 擴展性：易於新增新的 UI 管理功能
 * - 重用性：可用於其他專案的 UI 基礎設施
 * 
 * 🔧 與其他模組的協作：
 * - 作為其他 Manager 的基礎依賴
 * - 提供統一的使用者回饋機制
 * - 支援 Vue.js 混合式架構
 * - 整合 ASP.NET Core 的後端渲染頁面
 */

// 📝 模組導出：條件式全域註冊，支援不同的執行環境
if (typeof window !== 'undefined') {
    window.UIManager = UIManager;
}