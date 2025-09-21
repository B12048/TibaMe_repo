(function(window) {
    'use strict';

    let currentImages = [];
    let currentIndex = 0;
    
    // 將變數移到外部，讓它們可以被多個函數共享，但依然保持在模組作用域內
    let imagePreviewModal = null;
    let previewImageElement = null;
    let imageNavigationElement = null;
    let imageCounterElement = null;

    /**
     * 初始化模態框相關的 DOM 元素，這個函數只會在第一次需要時被呼叫
     */
    function initializeModalElements() {
        const modalEl = document.getElementById('imagePreviewModal');
        if (modalEl) {
            imagePreviewModal = new bootstrap.Modal(modalEl);
            previewImageElement = document.getElementById('previewImage');
            imageNavigationElement = document.getElementById('imageNavigation');
            imageCounterElement = document.getElementById('imageCounter');
            return true; // 初始化成功
        } else {
            console.error('在 DOM 中找不到 #imagePreviewModal 元素。');
            return false; // 初始化失敗
        }
    }

    /**
     * 更新圖片導航的計數器
     */
    function updateImageCounter() {
        if (imageCounterElement) {
            imageCounterElement.textContent = `${currentIndex + 1} / ${currentImages.length}`;
        }
    }

    /**
     * 從點擊的元素開啟圖片預覽模態框
     * @param {HTMLElement} element - 被點擊的 <img> 元素
     * @param {number} startIndex - 要顯示的第一張圖片的索引
     */
    function openImageModalFromElement(element, startIndex) {
        // **即時初始化**：如果模態框尚未初始化，則立即執行初始化
        // 這個 if 判斷確保了初始化代碼只會執行一次
        if (!imagePreviewModal) {
            if (!initializeModalElements()) {
                // 如果初始化失敗，則直接返回，不繼續執行
                return;
            }
        }

        const imageUrlsJson = element.getAttribute('data-images-json');
        if (!imageUrlsJson) {
            console.error('找不到 data-images-json 屬性。');
            return;
        }

        try {
            currentImages = JSON.parse(imageUrlsJson);
        } catch (e) {
            console.error("解析圖片URL JSON失敗:", e, "原始JSON:", imageUrlsJson);
            return; 
        }
        
        currentIndex = startIndex;

        previewImageElement.src = currentImages[currentIndex];

        if (currentImages.length > 1) {
            if(imageNavigationElement) imageNavigationElement.style.display = 'block';
            updateImageCounter();
        } else {
            if(imageNavigationElement) imageNavigationElement.style.display = 'none';
        }

        imagePreviewModal.show();
    }

    /**
     * 顯示上一張圖片
     */
    function previousImage() {
        if (currentIndex > 0) {
            currentIndex--;
            previewImageElement.src = currentImages[currentIndex];
            updateImageCounter();
        }
    }

    /**
     * 顯示下一張圖片
     */
    function nextImage() {
        if (currentIndex < currentImages.length - 1) {
            currentIndex++;
            previewImageElement.src = currentImages[currentIndex];
            updateImageCounter();
        }
    }

    // 將需要被 HTML onclick 呼叫的函數附加到 window 物件上
    window.openImageModalFromElement = openImageModalFromElement;
    window.previousImage = previousImage;
    window.nextImage = nextImage;

})(window);