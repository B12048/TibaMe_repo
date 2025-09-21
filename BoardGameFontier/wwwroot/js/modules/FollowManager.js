/**
 * FollowManager - 追蹤功能管理模組
 * 
 * 功能說明：
 * 1. 處理追蹤/取消追蹤的 API 呼叫
 * 2. 管理追蹤按鈕的 UI 狀態更新
 * 3. 整合防偽 Token 驗證機制
 * 4. 提供批次查詢和統計功能
 * 
 * 設計模式：
 * - 模組化設計，支援依賴注入（uiManager）
 * - 全域函數註冊，方便 .cshtml 頁面直接調用
 * - 錯誤處理和使用者回饋機制
 */
class FollowManager {
    constructor(uiManager) {
        this.uiManager = uiManager;
        this.init();
    }

    /**
     * 初始化追蹤管理器
     */
    init() {
        this.setupGlobalFunctions();
    }

    /**
     * 設定全域追蹤函數
     * 
     * 將核心功能暴露到全域，方便 Razor 頁面直接調用
     * 這種模式確保了模組化設計與傳統 ASP.NET MVC 視圖的相容性
     */
    setupGlobalFunctions() {
        // 主要追蹤切換函數，供按鈕點擊事件調用
        window.handleDetailPageFollowToggle = async (authorId, isCurrentlyFollowed) => {
            return await this.toggleFollow(authorId, isCurrentlyFollowed);
        };

        // 輔助工具函數，供其他模組使用
        window.getAntiForgeryToken = () => this.getAntiForgeryToken();
        window.updateDetailPageFollowButton = (authorId, isFollowed) => {
            this.updateFollowButton(authorId, isFollowed);
        };
    }

    /**
     * 追蹤/取消追蹤切換
     * @param {string} authorId - 作者ID
     * @param {boolean|string} isCurrentlyFollowed - 當前追蹤狀態
     */
    async toggleFollow(authorId, isCurrentlyFollowed) {
        if (!authorId) return false;

        const button = document.querySelector(`[data-author-id="${authorId}"]`);
        if (!button || button.disabled) return false;

        // 防止重複點擊
        button.disabled = true;

        try {
            // 確定當前狀態
            const currentlyFollowed = isCurrentlyFollowed === 'true' || isCurrentlyFollowed === true;
            
            // 構建 RESTful API 端點
            let endpoint, method, body = null;
            
            if (currentlyFollowed) {
                // 取消追蹤：DELETE /api/follows/{followeeId}
                endpoint = `/api/follows/${authorId}`;
                method = 'DELETE';
            } else {
                // 建立追蹤：POST /api/follows
                endpoint = '/api/follows';
                method = 'POST';
                body = JSON.stringify({ followeeId: authorId });
            }
            
            // 準備請求標頭
            const headers = { 'Content-Type': 'application/json' };
            const token = this.getAntiForgeryToken();
            if (token) {
                headers['RequestVerificationToken'] = token;
            }
            
            // 發送 API 請求
            const response = await fetch(endpoint, { method, headers, body });

            if (!response.ok) {
                const errorText = await response.text().catch(() => '未知錯誤');
                throw new Error(`HTTP ${response.status}: ${errorText}`);
            }

            const result = await response.json();
            
            if (result.success) {
                // 更新按鈕狀態
                const newFollowState = !currentlyFollowed;
                this.updateFollowButton(authorId, newFollowState);
                
                // 顯示成功訊息
                const message = newFollowState ? '追蹤成功' : '已取消追蹤';
                this.showSuccessMessage(message);
                
                // 更新側邊欄粉絲數（不重新載入頁面）
                await this.updateSidebarFollowerCount();
                
                return true;
            } else {
                throw new Error(result.message || '操作失敗');
            }
        } catch (error) {
            const errorMsg = `操作失敗: ${error.message}`;
            this.showErrorMessage(errorMsg);
            return false;
        } finally {
            button.disabled = false;
        }
    }

    /**
     * 更新追蹤按鈕的顯示狀態
     * @param {string} authorId - 作者ID
     * @param {boolean} isFollowed - 是否已追蹤
     */
    updateFollowButton(authorId, isFollowed) {
        const button = document.querySelector(`[data-author-id="${authorId}"]`);
        if (!button) return;

        const icon = button.querySelector('i');
        const text = button.querySelector('.follow-text');
        
        // 更新 data 屬性
        button.dataset.followed = isFollowed.toString();
        
        // 更新按鈕樣式和內容
        if (isFollowed) {
            button.className = 'follow-btn followed';
            if (icon) icon.className = 'bi bi-person-check-fill';
            if (text) text.textContent = '已追蹤';
        } else {
            button.className = 'follow-btn';
            if (icon) icon.className = 'bi bi-person-plus';
            if (text) text.textContent = '追蹤';
        }
    }

    /**
     * 批次檢查追蹤狀態
     * @param {Array<string>} userIds - 用戶ID列表
     */
    async getFollowStatusBatch(userIds) {
        if (!userIds || userIds.length === 0) return {};

        try {
            const response = await fetch('/api/follows/status/batch', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': this.getAntiForgeryToken()
                },
                body: JSON.stringify({ userIds })
            });

            if (!response.ok) {
                throw new Error(`HTTP ${response.status}`);
            }

            const result = await response.json();
            return result.success ? result.data : {};
        } catch (error) {
            console.error('批次查詢追蹤狀態失敗:', error);
            return {};
        }
    }

    /**
     * 取得用戶的追蹤統計
     * @param {string} userId - 用戶ID
     */
    async getFollowStats(userId) {
        if (!userId) return { followersCount: 0, followingCount: 0 };

        try {
            const response = await fetch(`/api/follows/stats/${userId}`);
            
            if (!response.ok) {
                throw new Error(`HTTP ${response.status}`);
            }

            const result = await response.json();
            return result.success ? result.data : { followersCount: 0, followingCount: 0 };
        } catch (error) {
            console.error('取得追蹤統計失敗:', error);
            return { followersCount: 0, followingCount: 0 };
        }
    }

    /**
     * 取得防偽 Token
     */
    getAntiForgeryToken() {
        const tokenElement = document.querySelector('input[name="__RequestVerificationToken"]');
        return tokenElement ? tokenElement.value : '';
    }

    /**
     * 更新側邊欄的粉絲數
     * 
     * 在追蹤狀態改變後，即時更新 UI 中的統計數據
     * 避免重新載入頁面，提供更流暢的使用者體驗
     */
    async updateSidebarFollowerCount() {
        try {
            const currentUserId = document.getElementById('currentUserId')?.value;
            if (!currentUserId) return;

            // 透過 API 取得最新的追蹤統計
            const stats = await this.getFollowStats(currentUserId);
            
            // 更新主要統計區域的粉絲數字
            const followersCountElement = document.querySelector('.sidebar-card .user-stats .stat-item:nth-child(3) .stat-number');
            if (followersCountElement) {
                followersCountElement.textContent = stats.followersCount;
            }

            // 更新追蹤者卡片標題中的數字（如果存在）
            const followersHeaderElement = document.querySelector('.sidebar-card.followers-card .sidebar-header small');
            if (followersHeaderElement) {
                followersHeaderElement.textContent = `(${stats.followersCount})`;
            }

        } catch (error) {
            console.error('更新側邊欄粉絲數時發生錯誤:', error);
        }
    }

    /**
     * 顯示成功訊息
     * 
     * 優先使用注入的 uiManager，提供一致的 UI 體驗
     * 若無可用管理器則使用全域函數作為備用方案
     */
    showSuccessMessage(message) {
        if (this.uiManager && typeof this.uiManager.showSuccessToast === 'function') {
            this.uiManager.showSuccessToast(message);
        } else if (window.showSuccessToast) {
            window.showSuccessToast(message);
        }
    }

    /**
     * 顯示錯誤訊息
     * 
     * 多層備用機制確保錯誤訊息能正常顯示
     * 最終回到原生 alert 作為最後保障
     */
    showErrorMessage(message) {
        if (this.uiManager && typeof this.uiManager.showErrorToast === 'function') {
            this.uiManager.showErrorToast(message);
        } else if (window.showErrorToast) {
            window.showErrorToast(message);
        } else {
            alert(message); // 最後備用方案
        }
    }
}

// 導出模組
window.FollowManager = FollowManager;