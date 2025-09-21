// BoardGameFrontier - 全域 JavaScript 功能
// 處理整個網站的共用功能和 Layout 相關的互動效果

document.addEventListener('DOMContentLoaded', function() {
    
    // 背景視差滾動效果
    // 當使用者滾動頁面時，背景圖片會有視差效果
    window.addEventListener('scroll', () => {
        const bgImg = document.querySelector(".bgimg");
        if (bgImg) {
            // 使用 0.5 倍速製造視差效果
            bgImg.style.backgroundPosition = `center ${window.scrollY * 0.5}px`;
        }
    });
    
    // 可以在這裡添加其他全域功能
    // 例如：導覽列互動、載入動畫、全域錯誤處理等
    
    // console.log('BoardGameFrontier - 全域 JavaScript 已載入'); // 生產環境移除
});
