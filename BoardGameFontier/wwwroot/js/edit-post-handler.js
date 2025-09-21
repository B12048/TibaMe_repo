(function(window) {
    'use strict';

    let vueInstance = null;
    let imagePreviewContainer = null;
    let imageInputElement = null;

    function renderImagePreviews() {
        if (!imagePreviewContainer || !vueInstance) return;
        const imageUrls = vueInstance.editForm.imageUrls || [];
        imagePreviewContainer.innerHTML = '';

        imageUrls.forEach((url, index) => {
            const colDiv = document.createElement('div');
            colDiv.className = 'col-auto position-relative';
            colDiv.innerHTML = `
                <img src="${url}" class="img-thumbnail" style="width: 100px; height: 100px; object-fit: cover;" alt="圖片 ${index + 1}">
                <button type="button" class="btn btn-danger btn-sm position-absolute top-0 end-0 rounded-circle" style="width: 24px; height: 24px; padding: 0; margin: 2px;" title="刪除圖片">
                    <i class="bi bi-x" style="font-size: 12px;"></i>
                </button>
            `;
            colDiv.querySelector('button').addEventListener('click', () => handleRemoveEditImage(index));
            imagePreviewContainer.appendChild(colDiv);
        });

        if (imageUrls.length < 5) {
            const addDiv = document.createElement('div');
            addDiv.className = 'col-auto d-flex align-items-center';
            addDiv.innerHTML = `
                <label for="editImageInput" class="btn btn-outline-primary btn-sm d-flex align-items-center justify-content-center" style="width: 100px; height: 100px; cursor: pointer;">
                    <i class="bi bi-plus-circle" style="font-size: 24px;"></i>
                </label>
            `;
            imagePreviewContainer.appendChild(addDiv);
        }
    }

    function handleRemoveEditImage(index) {
        if (vueInstance && vueInstance.editForm.imageUrls) {
            vueInstance.editForm.imageUrls.splice(index, 1);
            renderImagePreviews();
        }
    }

    function handleImageSelection(event) {
        const files = Array.from(event.target.files);
        if (!files.length || !vueInstance) return;
        const currentImages = vueInstance.editForm.imageUrls || [];
        if (currentImages.length + files.length > 5) {
            alert('最多只能上傳5張圖片');
            event.target.value = '';
            return;
        }
        uploadImages(files, event.target);
    }

    async function uploadImages(files, inputElement) {
        if (!vueInstance) return;
        vueInstance.isUploading = true;
        const formData = new FormData();
        files.forEach(file => formData.append('files', file));
        try {
            const response = await fetch('/api/ImageUploadApi/upload-multiple', { method: 'POST', body: formData });
            const result = await response.json();
            if (result.success && result.urls) {
                vueInstance.editForm.imageUrls.push(...result.urls);
                renderImagePreviews();
            } else {
                throw new Error(result.error || '上傳失敗');
            }
        } catch (error) {
            console.error('上傳圖片失敗:', error);
            alert('上傳圖片失敗: ' + error.message);
        } finally {
            vueInstance.isUploading = false;
            if(inputElement) inputElement.value = '';
        }
    }

    /**
     * 初始化函數，由主Vue實例呼叫
     */
    function initEditPostModal() {
        vueInstance = window.myPostAppInstance;
        if (!vueInstance) {
            console.error('主Vue實例 (myPostAppInstance) 未找到。');
            return;
        }

        const modal = document.getElementById('editPostModal');
        if (!modal) {
            console.error('編輯模態框 #editPostModal 未找到。');
            return;
        }

        // 找到模態框內的DOM元素
        imagePreviewContainer = modal.querySelector('#editImagePreview');
        imageInputElement = modal.querySelector('#editImageInput');

        // 手動填充表單資料
        const form = modal.querySelector('#editPostForm');
        const editData = vueInstance.editForm;

        form.querySelector('input[placeholder*="標題"]').value = editData.title;
        form.querySelector('textarea[placeholder*="內容"]').value = editData.content;
        const typeSelect = form.querySelector('#postTypeSelect');
        typeSelect.value = editData.type;
        typeSelect.disabled = editData.hasComments;

        // 處理交易欄位
        const tradeFields = form.querySelector('#tradeFields');
        if (tradeFields) {
            tradeFields.style.display = (editData.type == 2) ? 'block' : 'none';
            if (editData.type == 2) {
                form.querySelector('input[type="number"]').value = editData.price || 0;
                form.querySelector('input[placeholder*="交易地點"]').value = editData.tradeLocation || '';
                form.querySelector('textarea[placeholder*="交易相關注意事項"]').value = editData.tradeNotes || '';
            }
        }
        
        // 監聽類型變更以切換交易欄位顯示
        typeSelect.removeEventListener('change', handleTypeChange);
        typeSelect.addEventListener('change', handleTypeChange);

        // 渲染初始圖片
        renderImagePreviews();

        // 綁定圖片上傳事件
        imageInputElement.removeEventListener('change', handleImageSelection);
        imageInputElement.addEventListener('change', handleImageSelection);
    }

    function handleTypeChange(event) {
        const tradeFields = document.getElementById('tradeFields');
        if (tradeFields) {
            tradeFields.style.display = (event.target.value == 2) ? 'block' : 'none';
        }
    }

    // 將初始化函數暴露到全域
    window.initEditPostModal = initEditPostModal;

})(window);