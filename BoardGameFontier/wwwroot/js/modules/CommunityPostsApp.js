/**
 * CommunityPostsApp - 社群貼文管理 Vue 應用程式
 * 
 * 筆記：Vue.js 3 Options API 架構展示
 * 
 * 設計理念：
 * - 混合式架構：Vue.js 處理前端互動，ASP.NET Core 處理資料和頁面渲染
 * - Options API：使用物件導向風格，符合 C# 開發者思維
 * - 模組化設計：與其他 Manager 類別協作，職責清晰分離
 * 
 * 主要功能：
 * - 響應式資料管理：posts[], loading, error 等狀態自動更新 UI
 * - 分頁與篩選：整合 PaginationManager，提供流暢的使用體驗
 * - 樂觀更新：按讚功能立即反應，提升使用者體驗
 * - 單頁應用體驗：詳情視圖切換不重新載入頁面
 * 
 * 技術特色：
 * - CDN 引入 Vue.js，無需 Node.js 建置工具
 * - 與 Bootstrap 5 深度整合
 * - CSRF 保護和使用者認證狀態管理
 * 
 * 依賴：Vue 3, PaginationManager.js, PostManager.js, CommentManager.js
 */
const CommunityPostsApp = {
    /**
     * 筆記：Vue.js data() 函數
     * 
     * 響應式資料系統：
     * - data() 回傳的物件中的所有屬性都會變成響應式
     * - 當資料改變時，所有使用該資料的 DOM 元素會自動更新
     * - 這就是 Vue.js 的「資料驅動」核心概念
     */
    data() {
        return {
            // 貼文列表相關的響應式資料
            posts: [],           // 貼文陣列，對應 v-for="post in posts"
            loading: false,      // 載入中狀態，對應 v-if="loading"
            error: null,         // 錯誤訊息，對應 v-if="error"
            
            // 分頁功能的狀態管理
            currentPage: 1,      // 目前頁數
            totalPages: 1,       // 總頁數
            totalItems: 0,       // 總筆數
            pageSize: 6,         // 每頁顯示數量
            
            // 篩選與搜尋功能
            currentFilter: 'all', // 目前篩選類型
            searchKeyword: '',    // 搜尋關鍵字，對應 v-model="searchKeyword"
            searchTimer: null,    // 防抖動計時器，避免過於頻繁的 API 呼叫
            
            // 單頁應用視圖切換
            isDetailView: false,  // 是否顯示詳情頁，對應 v-show="isDetailView"
            detailLoading: false, // 詳情載入中
            selectedPost: null,   // 目前查看的貼文物件
            detailHtml: '',       // 詳情頁面的 HTML 內容，對應 v-html="detailHtml"
            
            // 外部管理器實例（依賴注入模式）
            commentManager: null,    // 留言管理器
            paginationManager: null, // 分頁管理器
            postManager: null,       // 貼文管理器

            // 使用者認證狀態（從隱藏欄位讀取）
            currentUserId: this.getCurrentUserId(),     // 目前使用者 ID
            isAuthenticated: this.getAuthStatus(),      // 是否已登入
            
            // 圖片預覽相關狀態
            currentPreviewImages: [],      // 目前預覽的圖片陣列
            currentPreviewIndex: 0,        // 目前預覽的圖片索引
            
            // 篩選按鈕設定（對應 v-for="filter in filterButtons"）
            filterButtons: [
                { type: 'all', text: '全部', active: true },
                { type: '0', text: '心得分享', active: false },
                { type: '1', text: '詢問求助', active: false },
                { type: '2', text: '二手交易', active: false },
                { type: '3', text: '創作展示', active: false }
            ]
        };
    },
    
    /**
     * 筆記：Vue.js computed 計算屬性
     * 
     * 計算屬性特點：
     * - 基於依賴的響應式資料自動重新計算
     * - 有快取機制，只有依賴改變時才重新計算
     * - 比 methods 更有效率，適合複雜的資料轉換
     * - 在模板中可以像 data 一樣使用：{{ hasPosts }}
     */
    computed: {
        /**
         * 範例：檢查是否有貼文資料
         * 用途：控制空狀態和有資料狀態的顯示
         * 對應模板：v-if="hasPosts"
         */
        hasPosts() {
            return this.posts && this.posts.length > 0;
        }
    },
    
    /**
     * 筆記：Vue.js methods 方法
     * 
     * 方法特點：
     * - 包含所有的業務邏輯和事件處理函數
     * - 可以修改 data 中的響應式資料
     * - 每次呼叫都會重新執行（沒有快取）
     * - 透過 this 存取 data 和其他 methods
     * - 對應模板事件：v-on:click="viewPostDetail(post.id)"
     */
    methods: {
        /**
         * 學習筆記：混合式 SPA 架構實作
         * 
         * 設計理念：
         * - 結合 Vue.js 的視圖切換和 ASP.NET Core 的頁面渲染
         * - 使用 Promise.all 並行載入 HTML 和 JSON，提升效能
         * - 透過 v-html 動態注入後端渲染的 HTML
         * - $nextTick 確保 DOM 更新完成後再初始化元件
         */
        async viewPostDetail(postId) {
            this.isDetailView = true;
            this.detailLoading = true;
            this.selectedPost = null; // 先清空

            try {
                // 同時取得 HTML 面板 和 JSON 資料
                const [htmlResponse, jsonResponse] = await Promise.all([
                    fetch(`/Community/Post/${postId}`),
                    fetch(`/api/posts/${postId}`)
                ]);

                if (!htmlResponse.ok) throw new Error(`HTML面板載入失敗: ${htmlResponse.status}`);
                if (!jsonResponse.ok) throw new Error(`貼文資料載入失敗: ${jsonResponse.status}`);

                const html = await htmlResponse.text();
                const jsonResult = await jsonResponse.json();

                if (jsonResult.success) {
                    this.selectedPost = jsonResult.data;
                } else {
                    throw new Error(jsonResult.message || '貼文資料格式錯誤');
                }

                // 使用 Vue 響應式更新內容
                this.detailHtml = html;
                this.detailLoading = false;

                // 等待 Vue 完成 DOM 更新後初始化留言區域
                await this.$nextTick();
                if (this.commentManager) {
                    this.commentManager.initializeCommentSection();
                }

            } catch (error) {
                console.error('載入貼文詳情錯誤:', error);
                this.detailHtml = '<div class="alert alert-danger">無法載入貼文詳情，請返回列表後再試一次。</div>';
                this.detailLoading = false;
            }
        },

        /**
         * 返回文章列表
         */
        returnToListView() {
            // 先清理留言管理器
            if (this.commentManager) {
                this.commentManager.destroyCommentSection();
            }
            
            // 清理 Vue 狀態
            this.isDetailView = false;
            this.selectedPost = null;
            this.detailHtml = '';
        },

        /**
         * 筆記：樂觀更新 (Optimistic Updates) 設計模式
         * 
         * 什麼是樂觀更新？
         * - 先更新 UI，再發送 API 請求
         * - 假設操作會成功，提供即時的使用者回饋
         * - 如果失敗，再回滾到原來的狀態
         * 
         * 優點：
         * - 使用者體驗更流暢，沒有等待時間
         * - 減少網路延遲對 UX 的影響
         * - 適合高成功率的操作（如按讚）
         * 
         * 注意事項：
         * - 需要完善的錯誤處理和回滾機制
         * - 要同步更新所有相關的 UI 元素
         */
        async toggleLike(postId) {
            // 第一步：防重複點擊檢查
            const requestKey = `like_${postId}`;
            if (this.pendingRequests && this.pendingRequests.has(requestKey)) {
                // console.log('按讚請求進行中，忽略重複點擊'); // 生產環境移除
                return;
            }

            // 第二步：驗證使用者權限
            if (!this.currentUserId) {
                if (this.uiManager) {
                    this.uiManager.showErrorMessage('請先登入才能按讚');
                } else {
                    this.showErrorToast('請先登入才能按讚');
                }
                return;
            }

            // 第三步：準備樂觀更新所需的狀態資料
            const post = this.posts.find(p => p.id === postId);
            if (!post) return;

            // 標記請求進行中
            if (!this.pendingRequests) this.pendingRequests = new Set();
            this.pendingRequests.add(requestKey);

            // 記錄原始狀態，以便失敗時回滾
            const previousLikedState = post.isLikedByCurrentUser;
            const previousLikeCount = post.likeCount;
            
            // 計算樂觀更新後的狀態
            const optimisticLikedState = !previousLikedState;
            const optimisticLikeCount = previousLikeCount + (optimisticLikedState ? 1 : -1);

            // 第三步：立即更新 UI（樂觀更新的核心）
            this.updateVuePostLikeState(postId, optimisticLikedState, optimisticLikeCount);
            
            // 同步更新其他相關 UI 元素
            this.updateSidebarLikeCount(optimisticLikedState);

            try {
                // 第四步：使用新的 RESTful API 發送實際請求
                let endpoint, method;
                let body = null;

                if (previousLikedState) {
                    // 取消按讚：DELETE /api/likes/Post/123
                    endpoint = `/api/likes/Post/${postId}`;
                    method = 'DELETE';
                } else {
                    // 建立按讚：POST /api/likes
                    endpoint = '/api/likes';
                    method = 'POST';
                    body = JSON.stringify({
                        itemType: 'Post',
                        itemId: postId
                    });
                }

                const response = await fetch(endpoint, {
                    method: method,
                    headers: {
                        'Content-Type': 'application/json',
                        'RequestVerificationToken': this.getAntiForgeryToken() // 🔒 CSRF 保護
                    },
                    body: body
                });

                const result = await response.json();

                // 第五步：處理伺服器回應
                if (result.success) {
                    // 成功：檢查伺服器回傳的狀態是否與樂觀更新一致
                    if (result.data.isLiked !== optimisticLikedState || 
                        result.data.likeCount !== optimisticLikeCount) {
                        // 如有差異，以伺服器資料為準
                        this.updateVuePostLikeState(postId, result.data.isLiked, result.data.likeCount);
                    }
                } else {
                    // 失敗：回滾到原始狀態
                    this.updateVuePostLikeState(postId, previousLikedState, previousLikeCount);
                    this.updateSidebarLikeCount(previousLikedState);
                    
                    if (this.uiManager) {
                        this.uiManager.showErrorMessage(result.message || '按讚失敗');
                    } else {
                        this.showErrorToast(result.message || '按讚失敗');
                    }
                }
            } catch (error) {
                // 第六步：處理網路錯誤（回滾樂觀更新）
                console.error('按讚請求失敗:', error);
                this.updateVuePostLikeState(postId, previousLikedState, previousLikeCount);
                this.updateSidebarLikeCount(previousLikedState);
                
                if (this.uiManager) {
                    this.uiManager.showErrorMessage('網路連線問題，請稍後再試');
                } else {
                    this.showErrorToast('網路連線問題，請稍後再試');
                }
            } finally {
                // 第七步：清除請求狀態，允許後續請求
                if (this.pendingRequests) {
                    this.pendingRequests.delete(requestKey);
                }
            }
        },

        /**
         * 更新 Vue 響應式資料和按讚狀態
         */
        updateVuePostLikeState(postId, isLiked, likeCount) {
            // 更新 Vue 資料模型（如果存在）
            if (this.posts && Array.isArray(this.posts)) {
                const postIndex = this.posts.findIndex(p => p.id === postId);
                if (postIndex !== -1) {
                    // 使用 Vue 響應式更新
                    this.posts[postIndex].isLikedByCurrentUser = isLiked;
                    this.posts[postIndex].likeCount = likeCount;
                }
            }
            
            // 同步更新詳情視圖中的狀態（如果存在）
            if (this.selectedPost && this.selectedPost.id === postId) {
                if (this.selectedPost.stats) {
                    this.selectedPost.stats.likeCount = likeCount;
                }
                this.selectedPost.isLikedByCurrentUser = isLiked;
            }
            
            // 強制更新 DOM（在詳情頁面中必要）
            this.updateLikeButtonDOM(postId, isLiked, likeCount);
        },

        /**
         * 直接 DOM 操作更新按讚按鈕（備用方法）
         */
        updateLikeButtonDOM(postId, isLiked, likeCount) {
            const likeBtn = document.querySelector(`.post-card[onclick*="viewPostDetail(${postId})"] .like-btn`);
            if (likeBtn) {
                const icon = likeBtn.querySelector('.post-action-icon');
                const count = likeBtn.querySelector('.post-action-count');
                
                if (icon) {
                    icon.className = isLiked ? 'post-action-icon bi bi-heart-fill' : 'post-action-icon bi bi-heart';
                }
                if (count) {
                    count.textContent = likeCount;
                }
                
                likeBtn.classList.toggle('liked', isLiked);
            }
        },

        /**
         * 筆記：API 資料載入與狀態管理
         * 
         * 設計模式：
         * - 統一的載入狀態管理（loading, error, success）
         * - 彈性的參數設計（type, page, keyword 都有預設值）
         * - 資料格式轉換（統一前後端的屬性名稱差異）
         * - 錯誤處理和使用者回饋
         * 
         * 效能優化：
         * - 只在需要時才加入查詢參數
         * - 避免不必要的 API 呼叫
         * - 整合分頁管理器更新 UI
         */
        async loadPosts(type = 'all', page = 1, keyword = '') {
            // 第一步：設定載入狀態（觸發 UI 更新）
            // 優化：如果已有內容且是分頁切換，延遲顯示載入動畫
            const isPageChange = page > 1 && this.posts.length > 0;
            const loadingDelay = isPageChange ? 300 : 0; // 分頁切換延遲300ms顯示載入動畫
            
            let loadingTimeout;
            if (loadingDelay > 0) {
                loadingTimeout = setTimeout(() => {
                    this.loading = true;
                }, loadingDelay);
            } else {
                this.loading = true;  // 初次載入立即顯示
            }
            
            this.error = null;    // 清除之前的錯誤
            
            try {
                // 第二步：動態建構查詢參數
                const params = new URLSearchParams();
                params.append('page', page);
                params.append('pageSize', this.pageSize);
                
                // 只在需要時才加入參數，避免不必要的查詢條件
                if (type && type !== 'all') {
                    const typeMapping = {
                        '0': 0, '1': 1, '2': 2, '3': 3  // 字串轉數字對應
                    };
                    const mappedType = typeMapping[type] !== undefined ? typeMapping[type] : parseInt(type);
                    if (!isNaN(mappedType)) {
                        params.append('postType', mappedType);
                    }
                }
                
                // 搜尋功能：只在有關鍵字時才加入參數
                if (keyword && keyword.trim()) {
                    params.append('searchKeyword', keyword.trim());
                }
                
                // 第三步：發送 API 請求
                const response = await fetch(`/api/posts?${params}`);
                const result = await response.json();
                
                if (result.success) {
                    // 第四步：資料格式轉換與標準化
                    // 前後端資料格式統一：解決屬性名稱大小寫差異
                    this.posts = (result.data.posts || []).map(post => ({
                        ...post,
                        // 將 IsLiked 轉換為 isLikedByCurrentUser
                        isLikedByCurrentUser: post.isLiked || false,
                        // 將 Author 屬性名稱轉為小寫
                        author: {
                            id: post.author?.id || post.Author?.Id,
                            userName: post.author?.userName || post.Author?.UserName,
                            displayName: post.author?.displayName || post.Author?.DisplayName,
                            profilePictureUrl: post.author?.profilePictureUrl || post.Author?.ProfilePictureUrl
                        },
                        // 統一轉換屬性名稱
                        likeCount: post.likeCount || post.LikeCount || 0,
                        commentCount: post.commentCount || post.CommentCount || 0,
                        // 移除 viewCount 處理，因為沒有實際記數功能
                        type: post.type !== undefined ? post.type : post.Type,
                        createdAt: post.createdAt || post.CreatedAt,
                        // 處理 TradeInfo/tradeInfo
                        tradeInfo: post.tradeInfo || post.TradeInfo ? {
                            price: (post.tradeInfo || post.TradeInfo).price || (post.tradeInfo || post.TradeInfo).Price || 0,
                            currency: (post.tradeInfo || post.TradeInfo).currency || (post.tradeInfo || post.TradeInfo).Currency || "NT$",
                            location: (post.tradeInfo || post.TradeInfo).location || (post.tradeInfo || post.TradeInfo).Location,
                            notes: (post.tradeInfo || post.TradeInfo).notes || (post.tradeInfo || post.TradeInfo).Notes
                        } : null,
                        // 處理 RelatedGame/relatedGame
                        relatedGame: post.relatedGame || post.RelatedGame ? {
                            id: (post.relatedGame || post.RelatedGame).id || (post.relatedGame || post.RelatedGame).Id,
                            name: (post.relatedGame || post.RelatedGame).name || (post.relatedGame || post.RelatedGame).Name
                        } : null
                    }));
                    
                    // 修正：API 回傳的分頁資訊在 pagination 物件中
                    const pagination = result.data.pagination || {};
                    this.currentPage = pagination.currentPage || 1;
                    this.totalPages = pagination.totalPages || 1;
                    this.totalItems = pagination.totalCount || 0;
                    
                    
                    // 更新分頁 UI
                    this.updatePagination();
                } else {
                    throw new Error(result.message || '載入失敗');
                }
            } catch (error) {
                console.error('載入貼文錯誤:', error);
                this.error = '載入貼文失敗：' + error.message;
                this.posts = [];
            } finally {
                // 清除載入延遲計時器
                if (loadingTimeout) {
                    clearTimeout(loadingTimeout);
                }
                this.loading = false;
            }
        },

        /**
         * 更新分頁 UI
         */
        updatePagination() {
            if (!this.paginationManager && window.PaginationManager) {
                // 初始化分頁管理器
                this.paginationManager = new window.PaginationManager('#posts-pagination');
                this.paginationManager.setOnPageChange((page) => {
                    // 防止在載入中時重複點擊
                    if (this.loading) {
                        // console.log('🚫 載入中，忽略分頁點擊'); // 生產環境移除
                        return;
                    }
                    this.currentPage = page;
                    this.loadPosts(this.currentFilter, page, this.searchKeyword);
                });
            }
            
            if (this.paginationManager) {
                this.paginationManager.render({
                    currentPage: this.currentPage,
                    totalPages: this.totalPages,
                    totalItems: this.totalItems
                });
            }
        },

        /**
         * 篩選貼文
         */
        filterPosts(type) {
            this.currentFilter = type;
            this.currentPage = 1;
            
            // 更新按鈕狀態
            this.filterButtons.forEach(btn => {
                btn.active = (btn.type === type);
            });
            
            this.loadPosts(type, 1, this.searchKeyword);
        },

        /**
         * 搜尋貼文
         */
        searchPosts() {
            // 防抖動處理
            if (this.searchTimer) {
                clearTimeout(this.searchTimer);
            }
            
            this.searchTimer = setTimeout(() => {
                this.currentPage = 1;
                this.loadPosts(this.currentFilter, 1, this.searchKeyword);
            }, 300); // 從500ms縮短到300ms，提升搜尋反應速度
        },

        /**
         * 清除搜尋
         */
        clearSearch() {
            this.searchKeyword = '';
            this.currentPage = 1;
            this.loadPosts(this.currentFilter, 1, '');
        },

        /**
         * 重新載入貼文
         */
        refreshPosts() {
            this.loadPosts(this.currentFilter, this.currentPage, this.searchKeyword);
        },

        /**
         * 取得頭像 URL
         */
        getAvatarUrl(profilePictureUrl) {
            return profilePictureUrl || '/img/noPortrait.png';
        },

        /**
         * 格式化時間
         */
        formatTimeAgo(dateString) {
            const date = new Date(dateString);
            const now = new Date();
            const diff = now - date;
            
            const minutes = Math.floor(diff / 60000);
            const hours = Math.floor(diff / 3600000);
            const days = Math.floor(diff / 86400000);
            
            if (days > 0) return `${days}天前`;
            if (hours > 0) return `${hours}小時前`;
            if (minutes > 0) return `${minutes}分鐘前`;
            return '剛剛';
        },

        /**
         * 截取文字
         */
        truncateText(text, maxLength) {
            if (!text) return '';
            if (text.length <= maxLength) return text;
            return text.substring(0, maxLength) + '...';
        },

        /**
         * 取得貼文類型資訊
         */
        getPostTypeInfo(type) {
            const typeMap = {
                0: { text: '心得分享', class: 'review' },
                1: { text: '詢問求助', class: 'question' },
                2: { text: '二手交易', class: 'trade' },
                3: { text: '創作展示', class: 'creation' }
            };
            return typeMap[type] || typeMap[0];
        },

        /**
         * 取得當前用戶 ID
         */
        getCurrentUserId() {
            const userIdElement = document.getElementById('currentUserId');
            return userIdElement ? userIdElement.value : null;
        },

        /**
         * 取得認證狀態
         */
        getAuthStatus() {
            const authElement = document.getElementById('isAuthenticated');
            return authElement ? authElement.value === 'True' : false;
        },

        /**
         * 取得防偽 Token
         */
        getAntiForgeryToken() {
            const tokenElement = document.querySelector('input[name="__RequestVerificationToken"]');
            return tokenElement ? tokenElement.value : '';
        },

        /**
         * 分享貼文功能
         */
        sharePost(postId) {
            if (navigator.share) {
                navigator.share({
                    title: '桌遊前線 - 社群討論',
                    text: '來看看這篇有趣的貼文！',
                    url: `${window.location.origin}/Community#post-${postId}`
                });
            } else {
                // 複製連結到剪貼板
                const url = `${window.location.origin}/Community#post-${postId}`;
                navigator.clipboard.writeText(url).then(() => {
                    this.showSuccessToast('連結已複製到剪貼板');
                }).catch(() => {
                    this.showInfoToast('請手動複製連結：' + url);
                });
            }
        },

        /**
         * 打開用戶個人資料頁面
         * @param {string} userId 用戶 ID
         */
        openUserProfile(userId) {
            if (!userId) {
                console.warn('無效的用戶 ID');
                return;
            }
            
            // 在新分頁中打開用戶個人資料頁面
            const profileUrl = `/Member/PlayerProfile/${userId}`;
            window.open(profileUrl, '_blank');
        },

        /**
         * 成功訊息吐司
         */
        showSuccessToast(message) {
            this.showToast(message, 'success');
        },

        /**
         * 錯誤訊息吐司
         */
        showErrorToast(message) {
            this.showToast(message, 'error');
        },

        /**
         * 資訊訊息吐司
         */
        showInfoToast(message) {
            this.showToast(message, 'info');
        },

        /**
         * 顯示吐司通知 (使用 Bootstrap Toast)
         */
        showToast(message, type = 'info') {
            // 創建吐司容器 (如果不存在)
            let toastContainer = document.getElementById('toast-container');
            if (!toastContainer) {
                toastContainer = document.createElement('div');
                toastContainer.id = 'toast-container';
                toastContainer.className = 'toast-container position-fixed top-0 end-0 p-3';
                toastContainer.style.zIndex = '9999';
                document.body.appendChild(toastContainer);
            }

            // 創建吐司元素
            const toastId = 'toast-' + Date.now();
            const typeClass = type === 'success' ? 'text-bg-success' : 
                            type === 'error' ? 'text-bg-danger' : 'text-bg-info';
            const icon = type === 'success' ? 'bi-check-circle' :
                        type === 'error' ? 'bi-exclamation-triangle' : 'bi-info-circle';

            const toastHtml = `
                <div id="${toastId}" class="toast ${typeClass}" role="alert" aria-live="assertive" aria-atomic="true">
                    <div class="toast-header">
                        <i class="bi ${icon} me-2"></i>
                        <strong class="me-auto">桌遊前線</strong>
                        <button type="button" class="btn-close btn-close-white" data-bs-dismiss="toast" aria-label="Close"></button>
                    </div>
                    <div class="toast-body">
                        ${message}
                    </div>
                </div>
            `;

            toastContainer.insertAdjacentHTML('beforeend', toastHtml);

            // 顯示吐司
            const toastElement = document.getElementById(toastId);
            if (toastElement && typeof bootstrap !== 'undefined') {
                const toast = new bootstrap.Toast(toastElement, {
                    autohide: true,
                    delay: 4000
                });
                toast.show();

                // 自動清理
                toastElement.addEventListener('hidden.bs.toast', () => {
                    toastElement.remove();
                });
            }
        },

        /**
         * 處理發表貼文 - 委託給 PostManager 處理
         */
        async handleCreatePost(event) {
            if (event) event.preventDefault();
            
            const form = document.getElementById('createPostForm');
            if (!form || !this.postManager) {
                this.showErrorToast('表單或管理器未初始化');
                return false;
            }

            // 修正按鈕選擇器 - 查找模態框內的提交按鈕
            const submitButton = document.querySelector('#createPostModal button[type="submit"], #createPostModal .btn-primary[form="createPostForm"]');
            
            if (!submitButton) {
                console.warn('找不到提交按鈕，繼續執行...');
            }
            
            const originalText = submitButton?.innerHTML || '';
            
            try {
                // 顯示載入狀態
                if (submitButton) {
                    submitButton.disabled = true;
                    submitButton.innerHTML = '<i class="bi bi-hourglass-split"></i> 發表中...';
                }
                
                // 使用 PostManager 收集表單資料
                const postData = this.postManager.collectFormData(form);
                
                // 基本驗證
                if (!postData.content || postData.content.trim() === '') {
                    throw new Error('請輸入貼文內容');
                }
                
                // 交易類型驗證
                if (postData.type === 2) {
                    if (!postData.tradeLocation) {
                        throw new Error('請輸入交易地點');
                    }
                }
                
                // 呼叫 PostManager 建立貼文
                const result = await this.postManager.createPost(postData);
                
                if (result.success) {
                    // 關閉模態框
                    const modal = bootstrap.Modal.getInstance(document.getElementById('createPostModal'));
                    if (modal) modal.hide();
                    
                    // 清空表單
                    form.reset();
                    const tradeFields = document.getElementById('tradeFields');
                    if (tradeFields) tradeFields.style.display = 'none';
                    
                    // 顯示成功訊息
                    this.showSuccessToast(result.message);
                    
                    // 重新載入貼文列表
                    this.refreshPosts();
                    
                    return true;
                } else {
                    throw new Error(result.message);
                }
            } catch (error) {
                console.error('發表貼文錯誤:', error);
                this.showErrorToast('發表失敗：' + error.message);
                return false;
            } finally {
                // 恢復按鈕狀態
                if (submitButton) {
                    submitButton.disabled = false;
                    submitButton.innerHTML = originalText;
                }
            }
        },

        /**
         * 設定發表貼文表單提交處理器
         */
        setupCreatePostFormHandler() {
            const form = document.getElementById('createPostForm');
            const submitButton = document.querySelector('#createPostModal button[type="submit"], #createPostModal .btn-primary[form="createPostForm"]');
            
            if (form) {
                // 綁定表單提交事件
                const handleSubmit = (e) => {
                    e.preventDefault();
                    this.handleCreatePost(e);
                };
                
                form.addEventListener('submit', handleSubmit);
            }
            
            if (submitButton) {
                // 額外綁定按鈕點擊事件作為備用
                const handleClick = (e) => {
                    e.preventDefault();
                    this.handleCreatePost(e);
                };
                
                submitButton.addEventListener('click', handleClick);
            }
        },

        /**
         * 設定全域的詳情面板按讚函數
         */
        setupGlobalDetailFunctions() {
            // 全域詳情面板按讚函數
            window.togglePostDetailLike = async (postId) => {
                await this.toggleLike(postId);
                // 同步更新詳情面板的 DOM
                this.updateDetailPanelLikeButton(postId);
            };

            // 全域留言按讚函數
            window.toggleCommentLike = async (commentId) => {
                await this.toggleCommentLike(commentId);
            };
        },

        /**
         * 更新詳情面板的按讚按鈕 DOM
         */
        updateDetailPanelLikeButton(postId) {
            const post = this.posts.find(p => p.id === postId);
            if (!post) return;

            const likeBtn = document.querySelector(`[data-post-id="${postId}"].like-btn`);
            if (likeBtn) {
                const icon = likeBtn.querySelector('.post-action-icon');
                const count = likeBtn.querySelector('.post-action-count');
                
                if (icon) {
                    icon.className = post.isLikedByCurrentUser ? 
                        'post-action-icon bi bi-heart-fill' : 'post-action-icon bi bi-heart';
                }
                if (count) {
                    count.textContent = post.likeCount;
                }
                
                likeBtn.classList.toggle('liked', post.isLikedByCurrentUser);
            }
        },

        /**
         * 留言按讚功能（樂觀更新版本）
         */
        async toggleCommentLike(commentId) {
            // 防重複點擊檢查
            const requestKey = `comment_like_${commentId}`;
            if (this.pendingRequests && this.pendingRequests.has(requestKey)) {
                // console.log('留言按讚請求進行中，忽略重複點擊'); // 生產環境移除
                return;
            }

            if (!this.currentUserId) {
                if (this.uiManager) {
                    this.uiManager.showErrorMessage('請先登入才能按讚');
                } else {
                    this.showErrorToast('請先登入才能按讚');
                }
                return;
            }

            // 標記請求進行中
            if (!this.pendingRequests) this.pendingRequests = new Set();
            this.pendingRequests.add(requestKey);

            // 🚀 樂觀更新：立即更新 UI
            const commentBtn = document.querySelector(`[data-comment-id="${commentId}"] .like-btn`);
            let previousLikedState = false;
            let previousLikeCount = 0;
            
            if (commentBtn) {
                previousLikedState = commentBtn.classList.contains('liked');
                const countElement = commentBtn.querySelector('.post-action-count');
                previousLikeCount = parseInt(countElement?.textContent || '0');
            }
            
            const optimisticLikedState = !previousLikedState;
            const optimisticLikeCount = previousLikeCount + (optimisticLikedState ? 1 : -1);
            
            // 立即更新 UI
            this.updateCommentLikeState(commentId, optimisticLikedState, Math.max(0, optimisticLikeCount));
            
            // 🚀 同時樂觀更新左側面板獲讚數
            this.updateSidebarLikeCount(optimisticLikedState);

            try {
                // 使用新的 RESTful API 設計
                let endpoint, method;
                let body = null;

                if (previousLikedState) {
                    // 取消按讚：DELETE /api/likes/Comment/123
                    endpoint = `/api/likes/Comment/${commentId}`;
                    method = 'DELETE';
                } else {
                    // 建立按讚：POST /api/likes
                    endpoint = '/api/likes';
                    method = 'POST';
                    body = JSON.stringify({
                        itemType: 'Comment',
                        itemId: commentId
                    });
                }

                const response = await fetch(endpoint, {
                    method: method,
                    headers: {
                        'Content-Type': 'application/json',
                        'RequestVerificationToken': this.getAntiForgeryToken()
                    },
                    body: body
                });

                const result = await response.json();

                if (result.success) {
                    // ✅ 伺服器確認成功，如果有差異則修正
                    if (result.data.isLiked !== optimisticLikedState || 
                        result.data.likeCount !== optimisticLikeCount) {
                        this.updateCommentLikeState(commentId, result.data.isLiked, result.data.likeCount);
                    }
                } else {
                    // ❌ 伺服器拒絕，回滾樂觀更新
                    this.updateCommentLikeState(commentId, previousLikedState, previousLikeCount);
                    
                    // 🔄 回滾左側面板獲讚數
                    this.updateSidebarLikeCount(previousLikedState);
                    
                    if (this.uiManager) {
                        this.uiManager.showErrorMessage(result.message || '按讚失敗');
                    } else {
                        this.showErrorToast(result.message || '按讚失敗');
                    }
                }
            } catch (error) {
                // 🔄 網路錯誤，回滾樂觀更新
                console.error('留言按讚錯誤:', error);
                this.updateCommentLikeState(commentId, previousLikedState, previousLikeCount);
                
                // 🔄 回滾左側面板獲讚數
                this.updateSidebarLikeCount(previousLikedState);
                
                if (this.uiManager) {
                    this.uiManager.showErrorMessage('網路連線問題，請稍後再試');
                } else {
                    this.showErrorToast('網路連線問題，請稍後再試');
                }
            } finally {
                // 清除請求狀態，允許後續請求
                if (this.pendingRequests) {
                    this.pendingRequests.delete(requestKey);
                }
            }
        },

        /**
         * 更新留言按讚狀態
         */
        updateCommentLikeState(commentId, isLiked, likeCount) {
            // 如果有留言管理器，使用它來更新
            if (this.commentManager && typeof this.commentManager.updateCommentLikeState === 'function') {
                this.commentManager.updateCommentLikeState(commentId, isLiked, likeCount);
            } else {
                // 直接更新 DOM
                const likeBtn = document.querySelector(`[data-comment-id="${commentId}"] .like-btn`);
                if (likeBtn) {
                    const icon = likeBtn.querySelector('.post-action-icon');
                    const count = likeBtn.querySelector('.post-action-count');
                    
                    if (icon) {
                        icon.className = isLiked ? 
                            'post-action-icon bi bi-heart-fill' : 'post-action-icon bi bi-heart';
                    }
                    if (count) {
                        count.textContent = likeCount;
                    }
                    
                    likeBtn.classList.toggle('liked', isLiked);
                }
            }
        },

        /**
         * 🚀 更新左側面板獲讚數（樂觀更新）
         */
        updateSidebarLikeCount(isNewLike) {
            // 更精確的選擇器，選取「獲讚」統計項目
            const likesStatItems = document.querySelectorAll('.sidebar-card .user-stats .stat-item');
            let likesCountElement = null;
            
            // 尋找包含「獲讚」標籤的統計項目
            for (let item of likesStatItems) {
                const label = item.querySelector('.stat-label');
                if (label && label.textContent.includes('獲讚')) {
                    likesCountElement = item.querySelector('.stat-number');
                    break;
                }
            }
            
            if (likesCountElement) {
                let currentCount = parseInt(likesCountElement.textContent) || 0;
                let newCount = currentCount;
                
                if (isNewLike) {
                    newCount = currentCount + 1; // 獲讚數增加
                } else {
                    newCount = Math.max(0, currentCount - 1); // 獲讚數減少，確保不為負數
                }
                
                likesCountElement.textContent = newCount;
            }
        },

        /**
         * 圖片預覽功能
         * 開啟圖片預覽模態框
         */
        openImagePreview(images, startIndex) {
            this.currentPreviewImages = images;
            this.currentPreviewIndex = startIndex;
            
            // 設定預覽圖片
            const previewImg = document.getElementById('communityPreviewImage');
            if (previewImg) {
                previewImg.src = images[startIndex];
            }
            
            // 如果有多張圖片，顯示導航
            const navigation = document.getElementById('communityImageNavigation');
            if (navigation) {
                if (images.length > 1) {
                    navigation.style.display = 'block';
                    this.updateCommunityImageCounter();
                } else {
                    navigation.style.display = 'none';
                }
            }
            
            // 顯示模態框
            const modal = new bootstrap.Modal(document.getElementById('communityImagePreviewModal'));
            modal.show();
        },

        /**
         * 處理圖片載入錯誤
         */
        handleImageError(event) {
            event.target.style.display = 'none';
        },

        /**
         * 更新圖片計數顯示
         */
        updateCommunityImageCounter() {
            const counter = document.getElementById('communityImageCounter');
            if (counter && this.currentPreviewImages) {
                counter.textContent = `${this.currentPreviewIndex + 1} / ${this.currentPreviewImages.length}`;
            }
        },

        /**
         * 處理貼文圖片URLs字符串，轉為圖片數組
         * @param {string} imageUrls 以分號分隔的圖片URL字符串
         * @returns {Array} 圖片URL數組
         */
        parsePostImages(imageUrls) {
            if (!imageUrls || typeof imageUrls !== 'string') {
                return [];
            }
            return imageUrls.split(';').filter(url => url.trim() !== '');
        },

        /**
         * 取得貼文第一張圖片縮圖
         * @param {string} imageUrls 圖片URLs字符串
         * @returns {string|null} 第一張圖片URL，無圖片時回傳null
         */
        getPostThumbnail(imageUrls) {
            const images = this.parsePostImages(imageUrls);
            return images.length > 0 ? images[0] : null;
        },

        /**
         * 檢查貼文是否有圖片
         * @param {string} imageUrls 圖片URLs字符串  
         * @returns {boolean} 有圖片回傳true
         */
        hasPostImages(imageUrls) {
            const images = this.parsePostImages(imageUrls);
            return images.length > 0;
        },

        /**
         * 點擊貼文縮圖放大預覽
         * @param {Object} post 貼文物件
         * @param {Event} event 點擊事件
         */
        openPostImagePreview(post, event) {
            // 阻止事件冒泡，避免觸發查看貼文詳情
            event.stopPropagation();
            
            const images = this.parsePostImages(post.imageUrls);
            if (images.length > 0) {
                this.openImagePreview(images, 0);
            }
        }
    },
    
    /**
     * 筆記：Vue.js 生命週期 - mounted
     * 
     * mounted 是什麼？
     * - Vue 實例掛載到 DOM 後執行的生命週期鉤子
     * - 此時可以安全地操作 DOM 元素
     * - 適合初始化第三方函式庫、設定事件監聽器
     * - 相當於 jQuery 的 $(document).ready()
     * 
     * 執行順序：
     * 1. data() 初始化響應式資料
     * 2. 模板編譯和初次渲染
     * 3. mounted() 執行
     * 4. 頁面完全就緒，可開始與使用者互動
     */
    mounted() {
        // 第一步：設定全域函數（讓後端渲染的 HTML 可以呼叫 Vue 方法）
        this.setupGlobalDetailFunctions();
        
        // 第二步：初始化外部管理器（依賴注入模式）
        if (window.UIManager) {
            this.uiManager = new window.UIManager();
        }
        
        if (window.PostManager && this.uiManager) {
            this.postManager = new window.PostManager(this.uiManager);
        }
        
        if (window.CommentManager && this.uiManager) {
            this.commentManager = new window.CommentManager(this.uiManager);
        }
        
        // 第三步：載入初始資料
        // 這會觸發 loading 狀態，使用者看到載入動畫
        this.loadPosts();
    },
    
    /**
     * 學習筆記：Vue.js 生命週期 - beforeUnmount
     * 
     * beforeUnmount 是什麼？
     * - Vue 實例銷毀前執行的生命週期鉤子
     * - 用於清理資源，避免記憶體洩漏
     * - 清理計時器、移除事件監聽器、關閉連線等
     * - 相當於元件的「垃圾回收」處理
     * 
     * 為什麼需要清理？
     * - JavaScript 的計時器不會自動清理
     * - 未清理的資源會造成記憶體洩漏
     * - 影響應用程式效能和穩定性
     */
    beforeUnmount() {
        // 清理搜尋防抖計時器
        if (this.searchTimer) {
            clearTimeout(this.searchTimer);
        }
        
        // 清理分頁管理器
        if (this.paginationManager) {
            this.paginationManager.destroy();
        }
        
        // 📝 清理留言管理器
        if (this.commentManager) {
            this.commentManager.destroyCommentSection();
        }
    }
};

/**
 * 總結：CommunityPostsApp 架構設計
 * 
 * 核心概念示範：
 * 1. **響應式資料系統**：data() → 自動 UI 更新
 * 2. **Options API 架構**：data, computed, methods, mounted 清晰分離
 * 3. **樂觀更新模式**：提升使用者體驗的進階技巧
 * 4. **混合式架構**：Vue.js + ASP.NET Core 的最佳實踐
 * 5. **生命週期管理**：mounted 初始化、beforeUnmount 清理
 * 
 * ASP.NET Core 整合特色：
 * - 使用隱藏欄位傳遞伺服器資料到前端
 * - CSRF Token 保護機制
 * - v-html 動態載入後端渲染的 Partial View
 * - 模組化設計與其他 Manager 類別協作
 * 
 * 使用者體驗優化：
 * - 載入狀態管理（loading, error, success）
 * - 搜尋防抖動（避免過多 API 呼叫）
 * - 樂觀更新（按讚立即回應）
 * - 錯誤處理與回滾機制
 * 
 * 技術實踐：
 * - CDN 引入 Vue.js（符合課程要求）
 * - Options API（物件導向風格）
 * - Promise.all 並行處理
 * - Bootstrap Toast 整合
 * - 資源清理避免記憶體洩漏
 */

// 將 Vue 應用程式註冊到全域，供 CommunityInitializer 使用
window.CommunityPostsApp = CommunityPostsApp;