var connectionChat = new signalR.HubConnectionBuilder().withUrl("/hubs/chat").build();
const ul = document.getElementById("messageList");

connectionChat.on("MessageReceived", function (userDisplayName, message, userPortraitUrl, senderUserName) {
    //判斷isMe
    const myUserName = document.getElementById("senderEmail").value;
    const isMe = senderUserName === myUserName;

    // 建立時間字串
    const time = new Date();
    const readableTime = time.toLocaleString();

    // 建立 DOM 元素
    const li = document.createElement("li");

    if (isMe)
    { li.className = "d-flex mb-3 justify-content-end"; }
    else
    { li.className = "d-flex mb-3 justify-content-start";   }

    const portraitDiv = document.createElement("div");
    if (isMe)
    { portraitDiv.className = "ms-2"; }
    else { portraitDiv.className = "me-2"; }

    const imgPortrait = document.createElement("img");
    imgPortrait.className = "rounded-circle chat-avatar";
    imgPortrait.src = userPortraitUrl; // 記得從hub把資料找到並傳來

    const bubble = document.createElement("div");
    if (isMe)
    { bubble.className = "chat-bubble bubble-me"; }
    else { bubble.className = "chat-bubble bubble-other"; }

    const name = document.createElement("div");
    name.innerText = userDisplayName;
    name.className = "sender-name";

    const msg = document.createElement("div");
    msg.innerText = message;
    msg.className = "message-text";

    const timeDiv = document.createElement("div");
    timeDiv.className = "message-time";
    timeDiv.textContent = readableTime;

    // 組合結構
    if (!isMe) {
        portraitDiv.appendChild(imgPortrait);
        li.appendChild(portraitDiv);
        bubble.appendChild(name);
        bubble.appendChild(msg);
        bubble.appendChild(timeDiv);
        li.appendChild(bubble);
    } else {
        portraitDiv.appendChild(imgPortrait);
        bubble.appendChild(name);
        bubble.appendChild(msg);
        bubble.appendChild(timeDiv);
        li.appendChild(bubble);
        li.appendChild(portraitDiv);  //如果是me的話，頭像要最後裝
    }
    ul.appendChild(li);
    if (ul.children.length >= 30) {
        ul.removeChild(ul.firstElementChild); // 移除最舊的訊息
    }
    ul.scrollTop = ul.scrollHeight;
});

$('#BtnSendMessage').on("click", function (event) {
    const sender = document.getElementById("senderEmail").value;
    const message = document.getElementById("sendMessage").value;

    if (message.length != 0) {
            connectionChat.send("SendMessageToAll", sender, message).catch(function (err) {
                return console.error(err.toString());
            });
        document.getElementById("sendMessage").value = ""; // 清空輸入框
        event.preventDefault();
    }
    else {
        alert("請輸入訊息內容");
    }
});


//SignalR連線開始時...
connectionChat.start().then(function () {
    ul.scrollTop = ul.scrollHeight;
});
