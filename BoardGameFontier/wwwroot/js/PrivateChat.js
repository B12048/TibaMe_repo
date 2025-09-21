var connectionPM = new signalR
    .HubConnectionBuilder().withUrl("/hubs/privateChat")
    .withAutomaticReconnect([0, 1000, 5000, null])
    .build();

connectionPM.on("ReceiveUserConnected", function (userId, userName) {
    addMessage(`${userName} 已上線`);

})

function addMessage(msg) {
    if (msg == null && msg == '') {
        return;
    }
    let ui = document.getElementById('messagesList');
    let li = document.createElement("li");
    li.innerHTML = msg;
    ul.appendChild(li);
}


connectionPM.start();