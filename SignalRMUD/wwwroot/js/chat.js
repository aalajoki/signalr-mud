"use strict";

//Disable send button until connection is established
document.getElementById("sendButton").disabled = true;
var connection = null;

function connect() {
    connection = new signalR.HubConnectionBuilder().withUrl("/mainHub").build();
    // + mechanics hub
    connection.start().then(function () {

        let characterName = document.getElementById("nameInput").value;
        let characterRace = document.getElementById("raceInput").value;

        document.getElementById("connect-fields").classList.add("hidden");
        document.getElementById("message-fields").classList.remove("hidden");
        document.getElementById("sendButton").disabled = false;
    
        alert(characterName);
    
        connection.invoke("createCharacter", characterName, characterRace);

        connection.on("ReceiveMessage", function (message) {
            // var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
            var li = document.createElement("li");
            li.textContent = message;
            document.getElementById("messagesList").appendChild(li);
        });
    
    }).catch(function (err) {
        return console.error(err.toString());
    });
}

// var connection = new signalR.HubConnectionBuilder().withUrl("/mainHub").build();

// connection.on("ReceiveMessage", function (user, message) {
//     var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
//     var encodedMsg = user + " says " + msg;
//     var li = document.createElement("li");
//     li.textContent = encodedMsg;
//     document.getElementById("messagesList").appendChild(li);
// });

document.getElementById("sendButton").addEventListener("click", function (event) {
    var message = document.getElementById("messageInput").value;
    connection.invoke("SendMessageGlobal", message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});

// document.getElementById("sendButton").addEventListener("click", function (event) {
//     // REMOVE USER
//     var user = document.getElementById("userInput").value;
//     var message = document.getElementById("messageInput").value;
//     connection.invoke("SendMessage", user, message).catch(function (err) {
//         return console.error(err.toString());
//     });
//     event.preventDefault();
// });

document.getElementById("connectButton").addEventListener("click", function (event) {
    connect();
    event.preventDefault();
});