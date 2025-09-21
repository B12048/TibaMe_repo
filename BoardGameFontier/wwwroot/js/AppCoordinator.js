/**
 * AppCoordinator - 應用程式協調器
 * 負責模組初始化、URL routing、全域函數橋接
 * 不處理業務邏輯，專注於應用程式層級的協調
 */
class AppCoordinator {
    constructor() {
        // 基本狀態
        this.currentUserId = this.getCurrentUserId();
        
        // 管理器實例 (輕量級初始化)
        this.uiManager = null;
        this.commentManager = null;
        
        this.init();
    }

    /**
     * 初始化 - 輕量級協調器
     */
    init() {
        this.setupEventListeners();
        
        // 檢查是否需要自動載入特定貼文 (與 Vue.js 協調)
        this.checkAutoLoadPost();
        
    }

    /**
     * 檢查並自動載入特定貼文
     * 與 Vue.js 應用程式協調處理直接連結
     */
    checkAutoLoadPost() {
        // 1. 檢查 URL query parameters
        const urlParams = new URLSearchParams(window.location.search);
        const postIdParam = urlParams.get('postId');
        if (postIdParam) {
            const postId = parseInt(postIdParam);
            if (!isNaN(postId)) {
                setTimeout(() => {
                    if (window.vueApp && window.vueApp.viewPostDetail) {
                        window.vueApp.viewPostDetail(postId);
                    }
                }, 1000);
                return;
            }
        }
        
        // 2. 檢查 URL hash 參數
        const hash = window.location.hash;
        if (hash && hash.startsWith('#post-')) {
            const postId = parseInt(hash.replace('#post-', ''));
            if (postId && !isNaN(postId)) {
                setTimeout(() => {
                    if (window.vueApp && window.vueApp.viewPostDetail) {
                        window.vueApp.viewPostDetail(postId);
                    }
                }, 1000);
                return;
            }
        }
        
        // 3. 檢查 data-auto-load-post-id 屬性
        const container = document.querySelector('.community-container');
        const autoLoadPostId = container?.getAttribute('data-auto-load-post-id');
        
        if (autoLoadPostId) {
            setTimeout(() => {
                if (window.vueApp && window.vueApp.viewPostDetail) {
                    window.vueApp.viewPostDetail(parseInt(autoLoadPostId));
                }
            }, 1200);
        }
    }

    /**
     * 處理 Hash 變化事件 - 與 Vue.js 協調
     */
    handleHashChange() {
        const hash = window.location.hash;
        
        if (hash && hash.startsWith('#post-')) {
            const postId = parseInt(hash.replace('#post-', ''));
            if (postId && !isNaN(postId)) {
                if (window.vueApp && window.vueApp.viewPostDetail) {
                    window.vueApp.viewPostDetail(postId);
                }
            }
        } else {
            if (window.vueApp && window.vueApp.isDetailView) {
                window.vueApp.returnToListView();
            }
        }
    }

    /**
     * 取得當前登入用戶ID
     */
    getCurrentUserId() {
        const userIdElement = document.getElementById('currentUserId');
        const isAuthenticated = document.getElementById('isAuthenticated')?.value === 'True';
        return isAuthenticated && userIdElement ? userIdElement.value : null;
    }

    /**
     * 設定事件監聽器 - 僅處理應用程式層級的協調
     */
    setupEventListeners() {
        // 監聽 URL hash 變化 (支援瀏覽器前進/後退)
        window.addEventListener('hashchange', () => {
            this.handleHashChange();
        });

    }

    /**
     * 委託給 Vue.js 或其他管理器的方法
     * 保持 API 相容性
     */
    showSuccessToast(message) { 
        if (window.showSuccessToast) {
            window.showSuccessToast(message);
        }
    }
    
    showErrorToast(message) { 
        if (window.showErrorToast) {
            window.showErrorToast(message);
        }
    }
}

// 初始化應用程式協調器
document.addEventListener('DOMContentLoaded', function() {
    window.appCoordinator = new AppCoordinator();
});

// ===== 全域輔助函數 =====
// 這些函數作為橋接，讓 Partial Views 可以與 Vue.js 應用程式通信

/**
 * 全域 Toast 成功訊息函數 - 橋接到 Vue.js
 */
window.showSuccessToast = function(message) {
    if (window.vueApp && typeof window.vueApp.showSuccessToast === 'function') {
        window.vueApp.showSuccessToast(message);
    }
};

/**
 * 全域 Toast 錯誤訊息函數 - 橋接到 Vue.js
 */
window.showErrorToast = function(message) {
    if (window.vueApp && typeof window.vueApp.showErrorToast === 'function') {
        window.vueApp.showErrorToast(message);
    }
};

/**
 * 全域 Toast 資訊訊息函數 - 橋接到 Vue.js
 */
window.showInfoToast = function(message) {
    if (window.vueApp && typeof window.vueApp.showInfoToast === 'function') {
        window.vueApp.showInfoToast(message);
    }
};

/**
 * 全域刪除貼文函數 - 橋接到 PostManager
 */
window.deletePost = async function(postId) {
    if (window.vueApp && window.vueApp.postManager && typeof window.vueApp.postManager.handleDeletePost === 'function') {
        return await window.vueApp.postManager.handleDeletePost(postId);
    } else if (window.PostManager) {
        // 直接使用 PostManager 的靜態方法
        const postManager = new window.PostManager();
        return await postManager.handleDeletePost(postId);
    }
    console.error('PostManager 未找到，無法刪除貼文');
};

/**
 * 全域發表貼文函數 - 橋接到 PostManager
 */
window.handleCreatePost = async function() {
    if (window.vueApp && window.vueApp.postManager && typeof window.vueApp.postManager.handleCreatePost === 'function') {
        return await window.vueApp.postManager.handleCreatePost();
    } else if (window.PostManager) {
        // 直接使用 PostManager 的靜態方法
        const postManager = new window.PostManager();
        return await postManager.handleCreatePost();
    }
    console.error('PostManager 未找到，無法發表貼文');
};

/**
 * 全域詳細頁面按讚函數 - 樂觀更新版
 */
window.toggleDetailLike = async function(postId) {
    // 防重複點擊檢查
    const requestKey = `detail_like_${postId}`;
    if (!window.pendingLikeRequests) window.pendingLikeRequests = new Set();
    
    if (window.pendingLikeRequests.has(requestKey)) {
        // console.log('詳情頁按讚請求進行中，忽略重複點擊'); // 生產環境移除
        return;
    }

    if (!window.vueApp || !window.vueApp.isAuthenticated) {
        if (window.showErrorToast) {
            window.showErrorToast('請先登入才能按讚');
        }
        return;
    }

    // 標記請求進行中
    window.pendingLikeRequests.add(requestKey);

    // 獲取詳細頁面的按讚按鈕
    const likeBtn = document.querySelector(`[data-post-id="${postId}"] .like-btn, .like-btn[onclick*="${postId}"]`);
    if (!likeBtn) {
        console.error('找不到按讚按鈕');
        return;
    }

    // 獲取當前狀態
    const icon = likeBtn.querySelector('.post-action-icon');
    const count = likeBtn.querySelector('.post-action-count');
    const originalLiked = likeBtn.classList.contains('liked');
    const originalCount = parseInt(count?.textContent || '0');

    // 樂觀更新：立即更新 UI
    const newLiked = !originalLiked;
    const newCount = originalCount + (newLiked ? 1 : -1);

    // 立即更新按鈕狀態
    if (newLiked) {
        likeBtn.classList.add('liked');
        if (icon) icon.className = 'post-action-icon bi bi-heart-fill';
    } else {
        likeBtn.classList.remove('liked');
        if (icon) icon.className = 'post-action-icon bi bi-heart';
    }
    if (count) count.textContent = newCount;

    // 同步更新 Vue 狀態 (如果存在)
    if (window.vueApp && window.vueApp.selectedPost && window.vueApp.selectedPost.id === postId) {
        window.vueApp.selectedPost.isLiked = newLiked;
        window.vueApp.selectedPost.likeCount = newCount;
    }

    // 同步更新列表頁的貼文狀態 (如果存在)
    if (window.vueApp && window.vueApp.posts) {
        const listPost = window.vueApp.posts.find(p => p.id === postId);
        if (listPost) {
            listPost.isLiked = newLiked;
            listPost.likeCount = newCount;
        }
    }

    try {
        // 使用新的 RESTful API 設計
        let endpoint, method;
        let body = null;

        if (originalLiked) {
            // 取消按讚：DELETE /api/likes/Post/123
            endpoint = `/api/likes/Post/${postId}`;
            method = 'DELETE';
        } else {
            // 建立按讚：POST /api/likes
            endpoint = '/api/likes';
            method = 'POST';
            body = JSON.stringify({ itemType: 'Post', itemId: postId });
        }

        const response = await fetch(endpoint, {
            method: method,
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': window.vueApp ? window.vueApp.getAntiForgeryToken() : ''
            },
            body: body
        });

        const result = await response.json();
        
        if (result.success) {
            // 與伺服器狀態同步（通常與樂觀更新一致）
            const serverCount = result.data.likeCount;
            if (count && serverCount !== newCount) {
                count.textContent = serverCount;
                if (window.vueApp && window.vueApp.selectedPost && window.vueApp.selectedPost.id === postId) {
                    window.vueApp.selectedPost.likeCount = serverCount;
                }
                if (window.vueApp && window.vueApp.posts) {
                    const listPost = window.vueApp.posts.find(p => p.id === postId);
                    if (listPost) {
                        listPost.likeCount = serverCount;
                    }
                }
            }
        } else {
            throw new Error(result.message || '按讚失敗');
        }
    } catch (error) {
        console.error('按讚 API 失敗:', error);
        
        // 發生錯誤：回滾 UI 狀態
        if (originalLiked) {
            likeBtn.classList.add('liked');
            if (icon) icon.className = 'post-action-icon bi bi-heart-fill';
        } else {
            likeBtn.classList.remove('liked');
            if (icon) icon.className = 'post-action-icon bi bi-heart';
        }
        if (count) count.textContent = originalCount;

        // 回滾 Vue 狀態
        if (window.vueApp && window.vueApp.selectedPost && window.vueApp.selectedPost.id === postId) {
            window.vueApp.selectedPost.isLiked = originalLiked;
            window.vueApp.selectedPost.likeCount = originalCount;
        }
        if (window.vueApp && window.vueApp.posts) {
            const listPost = window.vueApp.posts.find(p => p.id === postId);
            if (listPost) {
                listPost.isLiked = originalLiked;
                listPost.likeCount = originalCount;
            }
        }

        if (window.showErrorToast) {
            window.showErrorToast('按讚失敗：' + error.message);
        }
    } finally {
        // 清除請求狀態
        if (window.pendingLikeRequests) {
            window.pendingLikeRequests.delete(requestKey);
        }
    }
};

/**
 * 處理交易欄位顯示的全域函數
 */
window.initializeCreatePostForm = function() {
    const postTypeSelect = document.getElementById('postType');
    if (postTypeSelect) {
        postTypeSelect.addEventListener('change', function() {
            const tradeFields = document.getElementById('tradeFields');
            if (tradeFields) {
                tradeFields.style.display = this.value === '2' ? 'block' : 'none';
            }
        });
    }
};

/**
 * 全域返回列表函數 - 橋接到 Vue.js
 */
window.returnToPostsList = function() {
    if (window.vueApp && typeof window.vueApp.returnToListView === 'function') {
        window.vueApp.returnToListView();
    }
};

/**
 * 全域分享貼文函數 - 橋接到 Vue.js
 */
window.sharePost = function(postId) {
    if (window.vueApp && typeof window.vueApp.sharePost === 'function') {
        window.vueApp.sharePost(postId);
    }
};

/**
 * 全域展開留言函數 - 橋接到 CommentManager
 */
window.expandComments = function() {
    // 這個函數已由 CommentManager 處理，保持原有邏輯
    if (typeof window.expandComments === 'function') {
        // 避免無限遞歸，檢查是否有 CommentManager 實現
    }
};