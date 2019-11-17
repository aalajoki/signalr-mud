"use strict";

//Disable send button until connection is established
document.getElementById("sendButton").disabled = true;
const input = document.getElementById("messageInput");
var connection = null;

document.getElementById("connectButton").addEventListener("click", function (event) {
    connect();
    event.preventDefault();
});

function connect() {
    connection = new signalR.HubConnectionBuilder().withUrl("/mainHub").build();
    connection.start().then(function () {

        let characterName = document.getElementById("nameInput").value;
        let characterRace = document.getElementById("raceInput").value;

        document.getElementById("connect-fields").classList.add("hidden");
        document.getElementById("message-fields").classList.remove("hidden");
        document.getElementById("sendButton").disabled = false;
    
        connection.invoke("createCharacter", characterName, characterRace);

        connection.on("ReceiveMessage", function (message) {
            var li = document.createElement("li");
            li.textContent = message;
            document.getElementById("messagesList").appendChild(li);
        });

        connection.onclose(function () {
            alert("You have lost connection to the server.");
        });
    
    }).catch(function (err) {
        return console.error(err.toString());
    });
}

document.getElementById("sendButton").addEventListener("click", function (event) {
    const message = input.value;
    input.value = "";

    connection.invoke("SendMessage", message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});

