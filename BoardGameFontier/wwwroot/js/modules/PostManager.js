/**
 * PostManager - 貼文管理模組
 * 負責貼文的 CRUD 操作、載入、分頁、搜尋等功能
 */
class PostManager {
    constructor(uiManager) {
        this.uiManager = uiManager;
        this.currentPage = 1;
        this.pageSize = 6;
        this.currentPostType = null;
        this.currentSearchKeyword = '';
        this.postsCache = new Map();
    }

    /**
     * 載入貼文列表
     * @param {string} type - 貼文類型
     * @param {number} page - 頁碼
     * @param {string} keyword - 搜尋關鍵字
     */
    async loadPosts(type = 'all', page = 1, keyword = '') {
        try {
            this.uiManager.showLoadingIndicator();
            
            const url = this.buildPostsUrl(type, page, keyword);
            const cacheKey = `${type}-${page}-${keyword}`;

            // 檢查快取
            if (this.postsCache.has(cacheKey)) {
                const cachedData = this.postsCache.get(cacheKey);
                this.renderPosts(cachedData.posts);
                // ✅ 分頁由 Vue.js 處理，移除過時的 renderPagination 調用
                this.uiManager.hideLoadingIndicator();
                return;
            }

            const response = await fetch(url);
            if (!response.ok) {
                throw new Error(`HTTP ${response.status}`);
            }

            const result = await response.json();
            
            if (result.success) {
                // 快取結果 (僅快取貼文資料，分頁由 Vue.js 管理)
                this.postsCache.set(cacheKey, {
                    posts: result.data.posts
                });

                this.currentPage = page;
                this.currentPostType = type;
                this.currentSearchKeyword = keyword;

                this.renderPosts(result.data.posts);
                // ✅ 分頁由 Vue.js 處理，移除過時的 renderPagination 調用
            } else {
                throw new Error(result.message || '載入失敗');
            }
        } catch (error) {
            console.error('載入貼文時發生網路錯誤:', error);
            this.uiManager.showErrorMessage('載入貼文失敗，請稍後再試');
            this.uiManager.showPostsError();
        } finally {
            this.uiManager.hideLoadingIndicator();
        }
    }

    /**
     * 建立貼文 API URL - 統一使用 Posts API 標準 RESTful API
     */
    buildPostsUrl(type, page, keyword) {
        const params = new URLSearchParams();
        params.append('page', page);
        params.append('pageSize', this.pageSize);
        
        // 只有當類型不是 'all' 時才添加 postType 參數
        if (type && type !== 'all') {
            const typeMapping = {
                'review': 0,
                'question': 1, 
                'trade': 2,
                'creation': 3,
                '0': 0, '1': 1, '2': 2, '3': 3
            };
            const mappedType = typeMapping[type] !== undefined ? typeMapping[type] : parseInt(type);
            if (!isNaN(mappedType)) {
                params.append('postType', mappedType);
            }
        }
        
        // 只有當有搜尋關鍵字時才添加 searchKeyword 參數
        if (keyword && keyword.trim()) {
            params.append('searchKeyword', keyword.trim());
        }
        
        return `/api/posts?${params}`;
    }

    /**
     * 渲染貼文列表
     */
    renderPosts(posts) {
        const container = document.getElementById('dynamicPostsContainer');
        if (!container) return;

        if (!posts || posts.length === 0) {
            this.uiManager.showNoPosts();
            return;
        }

        const postsHTML = posts.map(post => this.createPostHTML(post)).join('');
        container.innerHTML = postsHTML;

        // 綁定貼文點擊事件
        this.bindPostClickEvents();
    }

    /**
     * 建立單一貼文的 HTML - 修正版
     * 產生的 class 名稱和結構與 community.css 完全匹配
     */
    createPostHTML(post) {
        const typeMap = {
            0: { text: '心得', class: 'review', icon: '📝' },
            1: { text: '詢問', class: 'question', icon: '❓' },
            2: { text: '交易', class: 'trade', icon: '💰' },
            3: { text: '創作', class: 'creation', icon: '🎨' }
        };
        
        const postType = typeMap[post.type] || { text: '討論', class: 'discussion', icon: '💬' };
        const timeAgo = this.getTimeAgo(new Date(post.createdAt));
        const avatarHtml = this.generateAvatarHtml(post.author?.profilePictureUrl, post.author?.displayName || post.author?.userName);

        // 根據 CSS 預期的結構重新產生 HTML
        return `
            <div class="post-card" data-post-id="${post.id}" data-type="${post.type}">
                <!-- 右上角分類標籤 -->
                <div class="post-category-tag ${postType.class}">
                    <span class="category-icon">${postType.icon}</span>
                    <span class="category-text">${postType.text}</span>
                </div>
                
                <div class="post-content-area">
                    <div class="post-header">
                        <div class="post-avatar">
                            ${avatarHtml}
                        </div>
                        <div class="post-user-info">
                            <div class="post-username">${post.author?.displayName || post.author?.userName || '匿名用戶'}</div>
                            <div class="post-time">${timeAgo}</div>
                        </div>
                        ${this.createFollowButton(post.author)}
                    </div>

                    <h3 class="post-title">${post.title || '無標題'}</h3>
                    <p class="post-preview">${this.truncateText(post.content, 100)}</p>

                    ${post.tradeInfo ? `
                        <div class="trade-info-preview">
                            <span class="trade-price">
                                ${post.tradeInfo.price > 0 ? `${post.tradeInfo.currency}${post.tradeInfo.price}` : '價格面議'}
                            </span>
                            ${post.tradeInfo.location ? `<span class="trade-location">📍 ${post.tradeInfo.location}</span>` : ''}
                        </div>
                    ` : ''}
                </div>

                <div class="post-footer">
                    <div class="post-actions">
                        <div class="post-action like-btn ${post.isLikedByCurrentUser ? 'liked' : ''}">
                            <i class="post-action-icon bi ${post.isLikedByCurrentUser ? 'bi-heart-fill' : 'bi-heart'}"></i>
                            <span class="post-action-count">${post.likeCount || 0}</span>
                        </div>
                        <div class="post-action comment-btn">
                            <i class="post-action-icon bi bi-chat-dots"></i>
                            <span class="post-action-count">${post.commentCount || 0}</span>
                        </div>
                        <!-- ✅ 移除觀看次數顯示，因為沒有實際記數功能 -->
                    </div>
                </div>
            </div>
        `;
    }

    /**
     * 綁定貼文點擊事件
     */
    bindPostClickEvents() {
        document.querySelectorAll('.post-card').forEach(card => {
            card.addEventListener('click', (e) => {
                const postId = parseInt(card.getAttribute('data-post-id'));
                if (postId) {
                    // 通知主應用程式載入貼文詳情
                    if (window.communityApp && window.communityApp.loadPostDetail) {
                        window.communityApp.loadPostDetail(postId);
                    }
                }
            });
        });
    }

    // ✅ 已移除過時的 renderPagination 方法 
    // 分頁功能現在統一由 Vue.js + PaginationManager 處理

    /**
     * 建立貼文 - 完整的表單處理和 API 呼叫
     */
    async handleCreatePost() {
        const form = document.getElementById('createPostForm');
        const button = document.querySelector('#createPostModal button[onclick*="handleCreatePost"]');
        
        if (!form) {
            this.uiManager?.showErrorMessage('找不到表單');
            return false;
        }
        
        const originalText = button?.innerHTML || '';
        
        try {
            // 顯示載入狀態
            if (button) {
                button.disabled = true;
                button.innerHTML = '<i class="bi bi-hourglass-split"></i> 發表中...';
            }
            
            // 收集和驗證資料
            const postData = this.collectFormData(form);
            
            if (!postData.Content || postData.Content.trim() === '') {
                throw new Error('請輸入貼文內容');
            }
            
            // 呼叫 API
            const response = await fetch('/api/posts', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': this.getAntiForgeryToken()
                },
                body: JSON.stringify(postData)
            });

            const result = await response.json();
            
            if (result.success) {
                // 關閉模態框
                const modal = bootstrap.Modal.getInstance(document.getElementById('createPostModal'));
                if (modal) modal.hide();
                
                // 清空表單
                form.reset();
                const tradeFields = document.getElementById('tradeFields');
                if (tradeFields) tradeFields.style.display = 'none';
                
                // 清除快取
                this.postsCache.clear();
                
                // 通知成功
                if (window.showSuccessToast) {
                    window.showSuccessToast('貼文發表成功！');
                }
                
                // 通知 Vue 重新載入
                if (window.vueApp && window.vueApp.refreshPosts) {
                    window.vueApp.refreshPosts();
                }
                
                return { success: true, data: result.data };
            } else {
                throw new Error(result.message || '發表失敗');
            }
        } catch (error) {
            console.error('發表貼文錯誤:', error);
            if (window.showErrorToast) {
                window.showErrorToast('發表失敗：' + error.message);
            }
            return { success: false, message: error.message };
        } finally {
            // 恢復按鈕狀態
            if (button) {
                button.disabled = false;
                button.innerHTML = originalText;
            }
        }
    }

    /**
     * 初始化發表貼文表單功能
     * 處理二手交易欄位的顯示/隱藏
     */
    initializeCreatePostForm() {
        const postTypeSelect = document.getElementById('postType');
        const tradeFields = document.getElementById('tradeFields');
        const createPostForm = document.getElementById('createPostForm');
        
        if (!postTypeSelect || !tradeFields) {
            console.warn('發表貼文相關元素未找到');
            return false;
        }
        
        // 監聽貼文類型變化，控制交易欄位顯示
        postTypeSelect.addEventListener('change', (e) => {
            this.toggleTradeFields(e.target.value, tradeFields);
        });
        
        // console.log('✅ 發表貼文表單功能已初始化'); // 生產環境移除
        return true;
    }

    /**
     * 控制交易欄位的顯示/隱藏
     */
    toggleTradeFields(postType, tradeFieldsElement) {
        if (postType === '2') { // PostType.Trade = 2
            // 顯示交易欄位
            tradeFieldsElement.style.display = 'block';
            
            // 添加必填驗證
            const priceInput = document.getElementById('price');
            const locationInput = document.getElementById('tradeLocation');
            
            if (priceInput) priceInput.setAttribute('required', 'required');
            if (locationInput) locationInput.setAttribute('required', 'required');
        } else {
            // 隱藏交易欄位
            tradeFieldsElement.style.display = 'none';
            
            // 移除必填驗證並清空值
            const priceInput = document.getElementById('price');
            const locationInput = document.getElementById('tradeLocation');
            const notesInput = document.getElementById('tradeNotes');
            
            if (priceInput) {
                priceInput.removeAttribute('required');
                priceInput.value = '';
            }
            if (locationInput) {
                locationInput.removeAttribute('required');
                locationInput.value = '';
            }
            if (notesInput) {
                notesInput.value = '';
            }
        }
    }

    /**
     * 收集表單資料並格式化為 PostCreateDto 格式
     */
    collectFormData(formElement) {
        const formData = new FormData(formElement);
        const postType = parseInt(formData.get('type'));
        
        const postData = {
            Type: postType, // 大寫開頭符合 C# 慣例
            Title: formData.get('title') || null,
            Content: formData.get('content'),
            GameDetailId: formData.get('gameId') ? parseInt(formData.get('gameId')) : null,
        };
        
        // 🖼️ 新增：處理圖片URLs
        const imageUrls = formData.get('imageUrls') || document.getElementById('postImageUrls')?.value || '';
        if (imageUrls && imageUrls.trim()) {
            postData.ImageUrls = imageUrls.trim();
            // console.log('✅ 收集到圖片URLs:', imageUrls); // 除錯用 - 生產環境移除
        }
        
        // 如果是二手交易類型，添加交易資訊
        if (postType === 2) {
            postData.Price = formData.get('price') ? parseFloat(formData.get('price')) : null;
            postData.TradeLocation = formData.get('tradeLocation') || null;
            postData.TradeNotes = formData.get('tradeNotes') || null;
        } else {
            // 非交易類型，確保交易相關欄位為 null
            postData.Price = null;
            postData.TradeLocation = null;
            postData.TradeNotes = null;
        }
        
        // console.log('📤 準備發送的貼文資料:', postData); // 除錯用 - 生產環境移除
        return postData;
    }

    /**
     * 刪除貼文 - 完整的確認和處理
     */
    async handleDeletePost(postId) {
        // 使用 Promise 來處理模態框確認
        const confirmed = await this.showDeleteConfirmModal();
        if (!confirmed) {
            return { success: false, message: '使用者取消操作' };
        }
        
        try {
            const response = await fetch(`/api/posts/${postId}`, {
                method: 'DELETE',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': this.getAntiForgeryToken()
                }
            });

            const result = await response.json();
            
            if (result.success) {
                // 清除快取
                this.postsCache.clear();
                
                // 通知成功
                if (window.showSuccessToast) {
                    window.showSuccessToast('貼文已成功刪除');
                }
                
                // 如果在詳細頁面，返回列表
                if (window.vueApp && window.vueApp.isDetailView) {
                    window.vueApp.returnToListView();
                }
                
                // 重新載入貼文列表
                if (window.vueApp && window.vueApp.refreshPosts) {
                    window.vueApp.refreshPosts();
                }
                
                return { success: true, data: result.data };
            } else {
                throw new Error(result.message || '刪除失敗');
            }
        } catch (error) {
            console.error('刪除貼文錯誤:', error);
            if (window.showErrorToast) {
                window.showErrorToast('刪除失敗：' + error.message);
            }
            return { success: false, message: error.message };
        }
    }

    /**
     * 顯示刪除確認模態框
     * @returns {Promise<boolean>} - 使用者確認結果
     */
    showDeleteConfirmModal() {
        return new Promise((resolve) => {
            if (confirm('確定要刪除這篇貼文嗎？此操作無法復原。')) {
                resolve(true);
            } else {
                resolve(false);
            }
        });
    }

    /**
     * 工具方法：獲取防偽 Token
     */
    getAntiForgeryToken() {
        const tokenElement = document.querySelector('input[name="__RequestVerificationToken"]');
        return tokenElement ? tokenElement.value : '';
    }

    /**
     * 工具方法：計算時間差
     */
    getTimeAgo(date) {
        const now = new Date();
        const diffMs = now.getTime() - date.getTime();
        const diffMins = Math.floor(diffMs / (1000 * 60));
        const diffHours = Math.floor(diffMins / 60);
        const diffDays = Math.floor(diffHours / 24);

        if (diffMins < 1) return '剛剛';
        if (diffMins < 60) return `${diffMins} 分鐘前`;
        if (diffHours < 24) return `${diffHours} 小時前`;
        if (diffDays < 7) return `${diffDays} 天前`;
        
        return date.toLocaleDateString('zh-TW', {
            year: 'numeric',
            month: '2-digit',
            day: '2-digit'
        });
    }

    /**
     * 工具方法：截斷文字
     */
    truncateText(text, maxLength) {
        if (!text) return '';
        if (text.length <= maxLength) return text;
        return text.substring(0, maxLength) + '...';
    }

    /**
     * 工具方法：生成頭像 HTML
     */
    generateAvatarHtml(avatarUrl, userName, size = 32) {
        if (avatarUrl && avatarUrl !== '/img/noPortrait.png') {
            return `<img src="${avatarUrl}" width="${size}" height="${size}" alt="${userName}的頭像" class="rounded-circle" onerror="this.src='/img/noPortrait.png'">`;
        }
        return `<img src="/img/noPortrait.png" width="${size}" height="${size}" alt="${userName}的頭像" class="rounded-circle">`;
    }

    /**
     * 建立追蹤按鈕 HTML
     */
    createFollowButton(author) {
        // 檢查是否為當前用戶自己的貼文
        const currentUserId = document.getElementById('currentUserId')?.value;
        const isAuthenticated = document.getElementById('isAuthenticated')?.value === 'True';
        
        if (!isAuthenticated || !author || author.id === currentUserId) {
            return ''; // 未登入或是自己的貼文，不顯示追蹤按鈕
        }

        const isFollowed = author.isFollowed || false;
        const buttonClass = isFollowed ? 'follow-btn followed' : 'follow-btn';
        const buttonText = isFollowed ? '已追蹤' : '追蹤';
        const iconClass = isFollowed ? 'bi-person-check-fill' : 'bi-person-plus';

        return `
            <button class="${buttonClass}" 
                    data-author-id="${author.id}" 
                    data-followed="${isFollowed}"
                    onclick="postManager.handleFollowToggle('${author.id}', ${isFollowed})">
                <i class="bi ${iconClass}"></i>
                <span class="follow-text">${buttonText}</span>
            </button>
        `;
    }

    /**
     * 處理追蹤/取消追蹤操作
     */
    async handleFollowToggle(authorId, isCurrentlyFollowed) {
        try {
            let endpoint, method, body = null;
            
            if (isCurrentlyFollowed) {
                // 取消追蹤：DELETE /api/follows/{followeeId}
                endpoint = `/api/follows/${authorId}`;
                method = 'DELETE';
            } else {
                // 建立追蹤：POST /api/follows
                endpoint = '/api/follows';
                method = 'POST';
                body = JSON.stringify({ followeeId: authorId });
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
                // 更新按鈕狀態
                this.updateFollowButton(authorId, !isCurrentlyFollowed);
                
                // 顯示成功訊息
                const message = isCurrentlyFollowed ? '已取消追蹤' : '追蹤成功';
                this.uiManager.showToast(message, 'success');
                
                // 清除快取，確保下次載入時獲得最新狀態
                this.postsCache.clear();
            } else {
                this.uiManager.showToast(result.message || '操作失敗', 'error');
            }
        } catch (error) {
            console.error('追蹤操作失敗:', error);
            this.uiManager.showToast('操作失敗，請稍後再試', 'error');
        }
    }

    /**
     * 更新追蹤按鈕的顯示狀態
     */
    updateFollowButton(authorId, isFollowed) {
        const button = document.querySelector(`[data-author-id="${authorId}"]`);
        if (!button) return;

        const icon = button.querySelector('i');
        const text = button.querySelector('.follow-text');
        
        button.dataset.followed = isFollowed;
        
        if (isFollowed) {
            button.className = 'follow-btn followed';
            icon.className = 'bi bi-person-check-fill';
            text.textContent = '已追蹤';
        } else {
            button.className = 'follow-btn';
            icon.className = 'bi bi-person-plus';
            text.textContent = '追蹤';
        }
    }
}

// 導出模組
if (typeof window !== 'undefined') {
    window.PostManager = PostManager;
}