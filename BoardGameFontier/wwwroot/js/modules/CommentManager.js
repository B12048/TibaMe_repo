/**
 * CommentManager - 留言管理模組
 * 負責 Vue 3 留言功能的初始化和管理
 *
 * 更新：
 * - canDeleteComment(): 留言作者 || 貼文作者 || Admin
 * - canReportPost(): 非作者 && 非Admin 才能檢舉貼文
 * - canReportComment(): 非留言作者 && 非Admin 才能檢舉留言
 */
class CommentManager {
    constructor(uiManager) {
        this.uiManager = uiManager;
        this.vueComponent = null;
    }

    initializeCommentSection() {
        if (typeof Vue === 'undefined') {
            console.error('Vue 3 未載入！');
            return;
        }

        const commentSectionElement = document.getElementById('commentSection');
        if (!commentSectionElement) return;
        if (commentSectionElement._vueInitialized) return;

        const postId = parseInt(commentSectionElement.dataset.postId) || 0;
        const currentUserId = commentSectionElement.dataset.currentUserId || '';
        const isAuthenticated = commentSectionElement.dataset.isAuthenticated === 'True' || commentSectionElement.dataset.isAuthenticated === 'true';
        const initialComments = JSON.parse(commentSectionElement.dataset.initialComments || '[]');
        const postAuthorId = commentSectionElement.dataset.postAuthorId || '';
        const isAdmin = commentSectionElement.dataset.isAdmin === 'True' || commentSectionElement.dataset.isAdmin === 'true';

        const { createApp } = Vue;
        const commentManagerInstance = this;

        const commentApp = createApp({
            data() {
                return {
                    postId,
                    currentUserId,
                    isAuthenticated,
                    comments: [...initialComments],
                    newComment: '',
                    isSubmitting: false,
                    isCommentsCollapsed: true,
                    modalId: 'commentModal_' + postId,
                    postAuthorId,
                    isAdmin
                };
            },

            methods: {
                // ====== 檢舉控制 ======
                canReportPost() {
                    if (!this.isAuthenticated || !this.currentUserId) return false;
                    if (this.currentUserId === this.postAuthorId) return false;
                    if (this.isAdmin) return false;
                    return true;
                },
                canReportComment(comment) {
                    if (!this.isAuthenticated || !this.currentUserId) return false;
                    if (this.currentUserId === (comment?.author?.id || comment?.authorId)) return false;
                    if (this.isAdmin) return false;
                    return true;
                },

                normalizeReportedType(t) {
                    var s = String(t || '').trim();
                    if (/^post$/i.test(s)) return 'Post';
                    if (/^comment$/i.test(s)) return 'Comment';
                    if (/^user$/i.test(s)) return 'User';
                    return s;
                },
                openReport(type, id) {
                    var normalized = this.normalizeReportedType(type);
                    var numericId = Number(id);

                    if (typeof Swal === 'undefined') {
                        alert('SweetAlert 未載入');
                        return;
                    }

                    var modalHtml = [
                        '<div class="d-flex flex-column align-items-center">',
                        '  <select id="reason" class="form-select mb-3" style="width: 300px;">',
                        '    <option value="不當言論（仇恨、歧視、謾罵）">不當言論（仇恨、歧視、謾罵）</option>',
                        '    <option value="色情或裸露內容">色情或裸露內容</option>',
                        '    <option value="暴力或血腥內容">暴力或血腥內容</option>',
                        '    <option value="垃圾訊息 / 廣告">垃圾訊息 / 廣告</option>',
                        '    <option value="騷擾或霸凌">騷擾或霸凌</option>',
                        '    <option value="錯誤資訊 / 假消息">錯誤資訊 / 假消息</option>',
                        '    <option value="侵權內容（盜圖、盜影片、未授權音樂等）">侵權內容（盜圖、盜影片、未授權音樂等）</option>',
                        '    <option value="其他">其他（請備註）</option>',
                        '  </select>',
                        '  <textarea id="remark" class="form-control" placeholder="請輸入備註（最多 200 字）" style="width: 300px; height: 100px;"></textarea>',
                        '  <div id="charCount" class="text-end text-muted mt-1" style="width: 300px;">0 / 200</div>',
                        '</div>'
                    ].join('');

                    Swal.fire({
                        title: '檢舉原因',
                        html: modalHtml,
                        showCancelButton: true,
                        confirmButtonText: '確認',
                        cancelButtonText: '取消',
                        didOpen: function () {
                            var remarkInput = Swal.getPopup().querySelector('#remark');
                            var charCount = Swal.getPopup().querySelector('#charCount');
                            remarkInput.addEventListener('input', function () {
                                if (this.value.length > 200) this.value = this.value.slice(0, 200);
                                charCount.textContent = this.value.length + ' / 200';
                            });
                        },
                        preConfirm: function () {
                            var reason = document.getElementById('reason').value;
                            var remark = document.getElementById('remark').value.trim();
                            if (!reason) { Swal.showValidationMessage('請選擇檢舉原因'); return false; }
                            if (remark.length > 200) { Swal.showValidationMessage('備註內容不能超過 200 字'); return false; }
                            return { reason: reason, remark: remark };
                        }
                    }).then((result) => {
                        if (!result.isConfirmed) return;

                        var payload = {
                            reportedType: normalized,
                            reportedId: numericId,
                            reason: result.value.reason,
                            description: result.value.remark
                        };

                        fetch('/api/reports/createReport', {
                            method: 'POST',
                            headers: {
                                'Content-Type': 'application/json',
                                'RequestVerificationToken': this.getToken()
                            },
                            body: JSON.stringify(payload)
                        })
                            .then(res => res.json().catch(() => ({})).then(data => ({ ok: res.ok, status: res.status, data })))
                            .then(resp => {
                                if (resp.status === 401) {
                                    Swal.fire('未登入', (resp.data && resp.data.message) || '請先登入', 'warning');
                                    return;
                                }
                                if (resp.ok && resp.data && resp.data.success) {
                                    Swal.fire({ icon: 'success', title: '檢舉已送出' });
                                } else {
                                    Swal.fire('錯誤', (resp.data && resp.data.message) || '檢舉失敗', 'error');
                                }
                            })
                            .catch(err => {
                                console.error(err);
                                Swal.fire('錯誤', '伺服器發生錯誤，請稍後再試', 'error');
                            });
                    });
                },

                toggleCommentsCollapse() {
                    this.isCommentsCollapsed = !this.isCommentsCollapsed;
                },

                openCommentModal() {
                    if (this.isCommentsCollapsed) {
                        this.isCommentsCollapsed = false;
                    }

                    setTimeout(() => {
                        const modalElement = document.getElementById(this.modalId);
                        if (modalElement && typeof bootstrap !== 'undefined') {
                            const bsModal = new bootstrap.Modal(modalElement);
                            bsModal.show();

                            modalElement.addEventListener('shown.bs.modal', () => {
                                const textarea = modalElement.querySelector('textarea');
                                if (textarea) textarea.focus();
                            }, { once: true });
                        }
                    }, 100);
                },

                canDeleteComment(comment) {
                    if (!this.currentUserId) return false;
                    return (
                        this.currentUserId === (comment?.author?.id || comment?.authorId) ||
                        this.currentUserId === this.postAuthorId ||
                        this.isAdmin
                    );
                },
                async submitComment() {
                    if (!this.newComment.trim() || this.isSubmitting) return;

                    this.isSubmitting = true;

                    try {
                        const response = await fetch('/api/comments', {
                            method: 'POST',
                            headers: {
                                'Content-Type': 'application/json',
                                'RequestVerificationToken': this.getToken()
                            },
                            body: JSON.stringify({
                                postId: this.postId,
                                content: this.newComment.trim()
                            })
                        });

                        const result = await response.json();

                        // 格式相容性處理
                        const isSuccess = result.Success !== undefined ? result.Success : result.success;
                        const data = result.Data || result.data;
                        const errorMessage = result.ErrorMessage || result.message;

                        if (isSuccess) {
                            // 添加新留言到列表
                            this.comments.push({
                                id: data.id,
                                content: data.content,
                                createdAt: data.createdAt,
                                likeCount: data.likeCount || 0,
                                isLikedByCurrentUser: false,
                                author: data.author
                            });

                            this.newComment = '';
                            this.closeModal();
                            this.showSuccessMessage('留言發表成功！');
                        } else {
                            throw new Error(errorMessage || '留言失敗');
                        }
                    } catch (error) {
                        console.error('留言錯誤:', error);
                        this.showErrorMessage('留言失敗：' + error.message);
                    } finally {
                        this.isSubmitting = false;
                    }
                },

                async deleteComment(commentId) {
                    if (!confirm('確定要刪除這則留言嗎？')) return;

                    try {
                        const response = await fetch('/api/comments/' + commentId, {
                            method: 'DELETE',
                            headers: {
                                'Content-Type': 'application/json',
                                'RequestVerificationToken': this.getToken()
                            }
                        });

                        const result = await response.json();
                        const isSuccess = result.Success !== undefined ? result.Success : result.success;

                        if (isSuccess) {
                            this.comments = this.comments.filter(c => c.id !== commentId);
                            this.showSuccessMessage('留言已刪除');
                        } else {
                            throw new Error(result.ErrorMessage || result.message || '刪除失敗');
                        }
                    } catch (error) {
                        console.error('刪除留言錯誤:', error);
                        this.showErrorMessage('刪除失敗：' + error.message);
                    }
                },

                async toggleLike(comment) {
                    if (!this.isAuthenticated) {
                        this.showErrorMessage('請先登入才能按讚');
                        return;
                    }
                    if (comment.isProcessing) return;

                    comment.isProcessing = true;
                    const originalLiked = comment.isLikedByCurrentUser;
                    const originalCount = comment.likeCount;
                    const newLikedState = !originalLiked;

                    // 樂觀更新
                    comment.isLikedByCurrentUser = newLikedState;
                    comment.likeCount = originalLiked ? originalCount - 1 : originalCount + 1;

                    // 🚀 同時樂觀更新左側面板獲讚數
                    commentManagerInstance.updateSidebarLikeCount(newLikedState);

                    try {
                        // 使用新的 RESTful API 設計
                        let endpoint, method;
                        let body = null;

                        if (originalLiked) {
                            // 取消按讚：DELETE /api/likes/Comment/123
                            endpoint = `/api/likes/Comment/${comment.id}`;
                            method = 'DELETE';
                        } else {
                            // 建立按讚：POST /api/likes
                            endpoint = '/api/likes';
                            method = 'POST';
                            body = JSON.stringify({
                                itemType: 'Comment',
                                itemId: comment.id
                            });
                        }

                        const response = await fetch(endpoint, {
                            method: method,
                            headers: {
                                'Content-Type': 'application/json',
                                'RequestVerificationToken': this.getToken()
                            },
                            body: body
                        });

                        const result = await response.json();

                        if (result.success) {
                            comment.isLikedByCurrentUser = result.data.isLiked;
                            comment.likeCount = Math.max(0, result.data.likeCount);
                        } else {
                            // 回滾
                            comment.isLikedByCurrentUser = originalLiked;
                            comment.likeCount = originalCount;
                            // 🔄 回滾左側面板獲讚數
                            commentManagerInstance.updateSidebarLikeCount(originalLiked);
                            this.showErrorMessage(result.message || '按讚失敗');
                        }
                    } catch (error) {
                        // 回滾
                        comment.isLikedByCurrentUser = originalLiked;
                        comment.likeCount = originalCount;
                        // 🔄 回滾左側面板獲讚數
                        commentManagerInstance.updateSidebarLikeCount(originalLiked);
                        console.error('按讚錯誤:', error);
                        this.showErrorMessage('按讚失敗，請稍後再試');
                    } finally {
                        comment.isProcessing = false;
                    }
                },

                closeModal() {
                    const modalElement = document.getElementById(this.modalId);
                    if (modalElement && typeof bootstrap !== 'undefined') {
                        const bsModal = bootstrap.Modal.getInstance(modalElement);
                        if (bsModal) bsModal.hide();
                    }
                },

                getToken() {
                    const metaToken = document.querySelector('meta[name="__RequestVerificationToken"]');
                    if (metaToken) return metaToken.getAttribute('content');

                    const tokenElement = document.querySelector('input[name="__RequestVerificationToken"]');
                    return tokenElement ? tokenElement.value : '';
                },

                clearComment() {
                    this.newComment = '';
                },

                formatDate(dateString) {
                    const date = new Date(dateString);
                    return date.toLocaleDateString('zh-TW', {
                        month: '2-digit',
                        day: '2-digit',
                        hour: '2-digit',
                        minute: '2-digit'
                    });
                },

                showSuccessMessage(message) {
                    if (typeof window.showSuccessToast === 'function') {
                        window.showSuccessToast(message);
                    } else if (window.vueApp && typeof window.vueApp.showSuccessToast === 'function') {
                        window.vueApp.showSuccessToast(message);
                    } else {
                        alert(message); // 最後備方案
                    }
                },

                showErrorMessage(message) {
                    if (typeof window.showErrorToast === 'function') {
                        window.showErrorToast(message);
                    } else if (window.vueApp && typeof window.vueApp.showErrorToast === 'function') {
                        window.vueApp.showErrorToast('錯誤：' + message);
                    } else {
                        alert('錯誤：' + message); // 最後備方案
                    }
                }
            }
        });

        // 掛載 Vue 應用
        try {
            this.vueComponent = commentApp.mount('#postDetailPanel');

            // 設定全域的展開/收合函式
            window.expandComments = () => {
                if (!this.vueComponent) return;

                const commentSection = document.getElementById('commentSection');
                const isCollapsed = this.vueComponent.isCommentsCollapsed;

                if (isCollapsed) {
                    this.vueComponent.toggleCommentsCollapse();
                    setTimeout(() => {
                        if (commentSection) commentSection.scrollIntoView({ behavior: 'smooth', block: 'nearest' });
                    }, 50);
                } else {
                    this.vueComponent.toggleCommentsCollapse();
                }
            };

        } catch (error) {
            console.error('Vue 掛載錯誤:', error);
        }

        // 標記已初始化
        commentSectionElement._vueInitialized = true;
    }

    /**
     * 🚀 更新左側面板獲讚數（樂觀更新）
     */
    updateSidebarLikeCount(isNewLike) {
        const likesStatItems = document.querySelectorAll('.sidebar-card .user-stats .stat-item');
        let likesCountElement = null;

        for (let item of likesStatItems) {
            const label = item.querySelector('.stat-label');
            if (label && label.textContent.includes('獲讚')) {
                likesCountElement = item.querySelector('.stat-number');
                break;
            }
        }

        if (likesCountElement) {
            let currentCount = parseInt(likesCountElement.textContent) || 0;
            let newCount = isNewLike ? currentCount + 1 : Math.max(0, currentCount - 1);
            likesCountElement.textContent = newCount;
        }
    }

    /**
     * 銷毀 Vue 應用
     */
    destroyCommentSection() {
        if (this.vueComponent && this.vueComponent.$.appContext.app) {
            this.vueComponent.$.appContext.app.unmount();
            this.vueComponent = null;
        }

        const commentSectionElement = document.getElementById('commentSection');
        if (commentSectionElement) {
            commentSectionElement._vueInitialized = false;
        }

        if (window.expandComments) {
            delete window.expandComments;
        }
    }
}

// 導出模組
if (typeof window !== 'undefined') {
    window.CommentManager = CommentManager;
}
