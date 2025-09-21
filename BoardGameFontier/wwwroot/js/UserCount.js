//建立singalR連線 - 優化連線和錯誤處理
var connectionUserCount = new signalR.HubConnectionBuilder()
    .withUrl("/hubs/userCount")
    .withAutomaticReconnect([0, 2000, 10000, 30000]) // 自動重連機制
    .build();

//與hub的方法連線
connectionUserCount.on("updateTotalViews", (value) => {
    var newCountSpan = document.getElementById("totalViewsCounter");
    if (newCountSpan) {
        newCountSpan.innerText = value.toString();
    }
});
connectionUserCount.on("updateTotalUsers", (value) => {
    var newCountSpan = document.getElementById("totalUsersCounter");
    if (newCountSpan) {
        newCountSpan.innerText = value.toString();
    }
});

//執行hub的方法
function newWindowLoadedOnClient() {
    if (connectionUserCount.state === signalR.HubConnectionState.Connected) {
        connectionUserCount.send("NewWindowLoaded").catch(err => {
            console.log("SignalR 傳送失敗:", err);
        });
    }
}

//連線成功處理
function fulfilled() {
    console.log("SignalR 連線成功");
    newWindowLoadedOnClient();
}

//連線失敗處理 - 不阻塞頁面載入
function rejected(error) {
    console.log("SignalR 連線失敗，但不影響頁面功能:", error);
    // 設定預設值，避免顯示空白
    var viewsCounter = document.getElementById("totalViewsCounter");
    var usersCounter = document.getElementById("totalUsersCounter");
    if (viewsCounter && viewsCounter.innerText === "00000") {
        viewsCounter.innerText = "載入中...";
    }
    if (usersCounter) {
        usersCounter.innerText = "載入中...";
    }
}

// 延遲啟動 SignalR 連線，避免阻塞頁面認證狀態顯示
document.addEventListener('DOMContentLoaded', function() {
    // 延遲 500ms 啟動，確保認證狀態先載入
    setTimeout(function() {
        connectionUserCount.start().then(fulfilled, rejected);
    }, 500);
});