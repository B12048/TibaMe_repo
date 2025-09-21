/**
 * PaginationManager - 分頁管理器
 * 
 * 📝 學習筆記：可重用元件設計模式
 * 
 * 💡 設計理念：
 * - 單一職責：專注於分頁邏輯，不涉及業務資料
 * - 高度可配置：透過 options 支援不同的分頁需求
 * - 事件驅動：使用回調函數與外部系統通訊
 * - 無狀態設計：不保存業務資料，只負責 UI 渲染
 * 
 * 🚀 主要功能：
 * - 智能頁碼範圍計算：動態調整顯示的頁碼範圍
 * - Bootstrap 5 整合：使用現代 UI 框架樣式
 * - 事件委託優化：高效能的事件處理機制
 * - 記憶體管理：完整的資源清理機制
 * 
 * 🔧 技術特色：
 * - ES6 Class 語法和靜態方法
 * - 配置物件模式 (Options Pattern)
 * - 工廠方法模式 (Factory Method)
 * - 觀察者模式 (Observer Pattern)
 * 
 * 使用範例：
 * ```javascript
 * const paginator = new PaginationManager('#pagination-container');
 * paginator.setOnPageChange(page => loadData(page));
 * paginator.render({ currentPage: 1, totalPages: 10, totalItems: 100 });
 * ```
 */
class PaginationManager {
    /**
     * 📝 學習筆記：配置物件模式 (Options Pattern)
     * 
     * 💡 這個模式的優點：
     * - 避免過多的建構函數參數
     * - 提供合理的預設值
     * - 支援部分配置覆蓋
     * - 易於未來擴展新選項
     * 
     * 🚀 展開運算子 (...options) 用法：
     * - 將傳入的 options 物件屬性合併到預設設定中
     * - 傳入的屬性會覆蓋預設值
     * - 實現了物件的「深度合併」效果
     */
    constructor(containerSelector, options = {}) {
        this.containerSelector = containerSelector;
        this.container = document.querySelector(containerSelector);
        
        // 📝 配置選項：使用物件展開語法合併預設值和自訂值
        this.options = {
            displayRange: 5,                    // 顯示多少個頁碼按鈕
            showFirstLast: true,                // 是否顯示 "第一頁" "最後頁" 
            showPrevNext: true,                 // 是否顯示 "上一頁" "下一頁"
            className: 'custom-pagination',     // CSS 類名前綴
            ...options  // 🔧 用戶自訂的選項會覆蓋預設值
        };
        
        // 📝 觀察者模式：儲存外部的回調函數
        this.onPageChange = null;  // 頁面變更時的事件處理器
    }
    
    /**
     * 📝 學習筆記：主要渲染方法 - 展示完整的元件生命週期
     * 
     * 💡 渲染流程分析：
     * 1. 輸入驗證：確保容器存在
     * 2. 資料解構：使用 ES6 解構賦值和預設值
     * 3. 邊界條件處理：只有一頁時隱藏分頁
     * 4. 計算邏輯：動態計算頁碼顯示範圍
     * 5. HTML 生成：組裝完整的分頁結構
     * 6. DOM 更新：設定 innerHTML
     * 7. 事件綁定：設定互動功能
     * 
     * 🚀 ES6 語法示範：
     * - 解構賦值：const { currentPage, totalPages } = paginationInfo
     * - 預設值：totalItems = 0
     * - 方法鏈：多個操作按順序執行
     * 
     * @param {Object} paginationInfo - 分頁資訊物件
     * @param {number} paginationInfo.currentPage - 當前頁碼
     * @param {number} paginationInfo.totalPages - 總頁數
     * @param {number} paginationInfo.totalItems - 總項目數 (可選)
     */
    render(paginationInfo) {
        // 📝 第一步：防禦性程式設計 - 檢查必要的 DOM 元素
        if (!this.container) {
            console.error('❌ 分頁容器不存在，選擇器:', this.containerSelector);
            return;
        }
        
        // 📝 第二步：ES6 解構賦值 + 預設值設定
        const { currentPage, totalPages, totalItems = 0 } = paginationInfo;
        
        // 📝 第三步：邊界條件處理 - UX 優化
        if (totalPages <= 1) {
            this.container.innerHTML = '';  // 只有一頁時不顯示分頁
            return;
        }
        
        // 📝 第四步：核心計算邏輯
        const pageRange = this.calculatePageRange(currentPage, totalPages);
        
        // 📝 第五步：HTML 結構生成
        const html = this.generatePaginationHTML(currentPage, totalPages, pageRange, totalItems);
        
        // 📝 第六步：DOM 更新
        this.container.innerHTML = html;
        
        // 📝 第七步：互動功能啟用
        this.bindEvents();
    }
    
    /**
     * 📝 學習筆記：智能分頁範圍計算演算法
     * 
     * 💡 演算法目標：
     * - 以當前頁為中心，平衡顯示左右頁碼
     * - 接近邊界時自動調整，確保顯示足夠的頁碼
     * - 避免顯示超出範圍的頁碼
     * 
     * 🚀 演算法步驟：
     * 1. 計算理想的顯示範圍（以當前頁為中心）
     * 2. 邊界修正：確保不超出總頁數
     * 3. 反向調整：當接近尾端時，向前補足頁碼
     * 
     * 📊 範例：
     * - 總頁數10，當前第5頁，範圍5 → [3,4,5,6,7]
     * - 總頁數10，當前第9頁，範圍5 → [6,7,8,9,10]（反向調整）
     * - 總頁數3，當前第2頁，範圍5 → [1,2,3]（邊界限制）
     */
    calculatePageRange(currentPage, totalPages) {
        const { displayRange } = this.options;
        const halfRange = Math.floor(displayRange / 2);  // 📝 中心點左右的頁數
        
        // 📝 第一步：以當前頁為中心計算理想範圍
        let startPage = Math.max(1, currentPage - halfRange);
        let endPage = Math.min(totalPages, startPage + displayRange - 1);
        
        // 📝 第二步：反向調整 - 當接近尾端時向前補足頁碼
        // 確保總是顯示 displayRange 個頁碼（除非總頁數不足）
        if (endPage - startPage + 1 < displayRange && totalPages >= displayRange) {
            startPage = Math.max(1, endPage - displayRange + 1);
        }
        
        return { startPage, endPage };
    }
    
    /**
     * 生成分頁 HTML 結構
     * 使用 Bootstrap 5 的分頁樣式
     */
    generatePaginationHTML(currentPage, totalPages, pageRange, totalItems) {
        const { className, showFirstLast, showPrevNext } = this.options;
        const { startPage, endPage } = pageRange;
        
        let html = `<nav class="${className}" aria-label="分頁導航">`;
        html += '<ul class="pagination justify-content-center">';
        
        // 上一頁按鈕
        if (showPrevNext) {
            const disabled = currentPage <= 1;
            html += `
                <li class="page-item ${disabled ? 'disabled' : ''}">
                    <button class="page-link" data-page="${currentPage - 1}" ${disabled ? 'disabled' : ''} 
                            title="上一頁">
                        <i class="bi bi-chevron-left"></i> 上一頁
                    </button>
                </li>
            `;
        }
        
        // 第一頁 + 省略號 (當起始頁大於1時顯示)
        if (showFirstLast && startPage > 1) {
            html += `<li class="page-item"><button class="page-link" data-page="1" title="第1頁">1</button></li>`;
            if (startPage > 2) {
                html += `<li class="page-item disabled"><span class="page-link">...</span></li>`;
            }
        }
        
        // 頁碼按鈕 (顯示計算出的頁碼範圍)
        for (let i = startPage; i <= endPage; i++) {
            const active = i === currentPage;
            html += `
                <li class="page-item ${active ? 'active' : ''}">
                    <button class="page-link" data-page="${i}" title="第${i}頁"
                            ${active ? 'aria-current="page"' : ''}>${i}</button>
                </li>
            `;
        }
        
        // 最後一頁 + 省略號 (當結束頁小於總頁數時顯示)
        if (showFirstLast && endPage < totalPages) {
            if (endPage < totalPages - 1) {
                html += `<li class="page-item disabled"><span class="page-link">...</span></li>`;
            }
            html += `<li class="page-item"><button class="page-link" data-page="${totalPages}" title="第${totalPages}頁">${totalPages}</button></li>`;
        }
        
        // 下一頁按鈕
        if (showPrevNext) {
            const disabled = currentPage >= totalPages;
            html += `
                <li class="page-item ${disabled ? 'disabled' : ''}">
                    <button class="page-link" data-page="${currentPage + 1}" ${disabled ? 'disabled' : ''} 
                            title="下一頁">
                        下一頁 <i class="bi bi-chevron-right"></i>
                    </button>
                </li>
            `;
        }
        
        html += '</ul>';
        
        // 頁面資訊 (顯示當前項目範圍)
        if (totalItems > 0) {
            const pageSize = 6; // 每頁顯示數量，調整為6個卡片
            const startItem = (currentPage - 1) * pageSize + 1;
            const endItem = Math.min(currentPage * pageSize, totalItems);
            html += `
                <div class="pagination-info text-center mt-2 text-muted small">
                    <i class="bi bi-info-circle"></i> 
                    顯示第 ${startItem} - ${endItem} 筆，共 ${totalItems} 筆資料
                </div>
            `;
        }
        
        html += '</nav>';
        return html;
    }
    
    /**
     * 📝 學習筆記：事件委託模式 (Event Delegation)
     * 
     * 💡 為什麼使用事件委託？
     * - 效能優化：只綁定一個事件監聽器，而不是每個按鈕都綁定
     * - 動態元素支援：新增的按鈕自動有事件處理能力
     * - 記憶體效率：減少事件監聽器的數量
     * 
     * 🚀 事件委託原理：
     * - 事件冒泡：子元素的事件會向上傳播到父元素
     * - 事件目標檢查：e.target 判斷實際被點擊的元素
     * - 統一處理：在父容器處理所有子元素的事件
     */
    bindEvents() {
        if (!this.container) return;
        
        // 📝 防止重複綁定：先移除舊的監聽器
        this.container.removeEventListener('click', this.handleClick);
        
        // 📝 綁定 this 上下文：確保回調函數中的 this 指向正確
        this.handleClick = this.handleClick.bind(this);
        this.container.addEventListener('click', this.handleClick);
    }
    
    /**
     * 處理點擊事件
     * @param {Event} e - 點擊事件
     */
    handleClick(e) {
        if (e.target.matches('button[data-page]')) {
            e.preventDefault();
            
            const page = parseInt(e.target.getAttribute('data-page'));
            const isDisabled = e.target.disabled || e.target.closest('.page-item.disabled');
            
            // 驗證頁碼有效性並觸發回調
            if (page && page > 0 && !isDisabled && this.onPageChange) {
                this.onPageChange(page);
            }
        }
    }
    
    /**
     * 設定頁面變更回調函數
     * @param {Function} callback - 回調函數，參數為新的頁碼
     */
    setOnPageChange(callback) {
        if (typeof callback === 'function') {
            this.onPageChange = callback;
        } else {
            console.warn('PaginationManager: callback 必須是函數');
        }
    }
    
    /**
     * 取得當前分頁狀態
     * @returns {number} 當前頁碼
     */
    getCurrentState() {
        const activeButton = this.container?.querySelector('.page-item.active .page-link');
        return activeButton ? parseInt(activeButton.textContent) : 1;
    }
    
    /**
     * 銷毀分頁元件 (清理事件監聽器)
     * 用於避免記憶體洩漏
     */
    destroy() {
        if (this.container && this.handleClick) {
            this.container.removeEventListener('click', this.handleClick);
            this.container.innerHTML = '';
        }
        this.onPageChange = null;
        this.handleClick = null;
    }
    
    /**
     * 📝 學習筆記：靜態工廠方法模式
     * 
     * 💡 工廠方法的優點：
     * - 簡化物件建立：提供更簡潔的建立方式
     * - 封裝建立邏輯：隱藏複雜的初始化過程
     * - 語意清晰：方法名稱表達明確的建立意圖
     * 
     * 🚀 使用範例：
     * ```javascript
     * // 傳統方式
     * const paginator = new PaginationManager('#container', { displayRange: 3 });
     * 
     * // 工廠方法
     * const paginator = PaginationManager.create('#container', { displayRange: 3 });
     * ```
     * 
     * @param {string} selector - 容器選擇器
     * @param {Object} options - 配置選項
     * @returns {PaginationManager} 分頁管理器實例
     */
    static create(selector, options = {}) {
        return new PaginationManager(selector, options);
    }
}

/**
 * 📝 學習總結：PaginationManager 設計模式展示
 * 
 * 🎯 展示的設計模式：
 * 1. **配置物件模式**：靈活的選項設定
 * 2. **觀察者模式**：事件回調機制
 * 3. **事件委託模式**：高效能事件處理
 * 4. **工廠方法模式**：簡化物件建立
 * 5. **單一職責原則**：專注於分頁邏輯
 * 
 * 💡 JavaScript 最佳實踐：
 * - ES6 Class 語法和解構賦值
 * - 防禦性程式設計和邊界條件處理
 * - 記憶體管理和資源清理
 * - 模組化設計和全域暴露
 * 
 * 🚀 可重用性特色：
 * - 無業務邏輯耦合，可用於任何分頁需求
 * - 完整的配置選項支援
 * - Bootstrap 5 樣式整合
 * - 完善的錯誤處理和日誌記錄
 */

// 📝 模組導出：註冊到全域作用域供其他模組使用
window.PaginationManager = PaginationManager;