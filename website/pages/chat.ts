import type { ChatMessage, User } from "types";
import { send } from "clientUtilities";
import { create, get } from "componentUtilities";

var profileImg = get("img", "profileImg");
var nameDiv = get("div", "nameDiv");
var messagesDiv = get("div", "messagesDiv");
var messageInput = get("input", "messageInput");
var sendButton = get("button", "sendButton");
var logOutButton = get("button", "logOutButton");

var messagesCount = 0;

var userId = localStorage.getItem("userId");
var user = await send<User | null>("getUser", userId);
if (user == null) {
  localStorage.removeItem("userId");
  location.href = "index.html";
}

user = user!;
profileImg.src = user.imgUrl;
nameDiv.innerText = user.name;

await loadMessages();

setInterval(async function () {
  var newMessagesCount = await send<number>("getMessagesCount");
  if (messagesCount == newMessagesCount) {
    return;
  }
  messagesCount = newMessagesCount;

  await loadMessages();

}, 5000);

async function loadMessages() {
  var messages = await send<ChatMessage[]>("getMessages");

  messagesDiv.innerHTML = "";

  if (messages.length == 0) {
    messagesDiv.append(create("div", { className: "emptyDiv", innerText: "No messages yet." }));
    return;
  }

  for (var message of messages) {
    messagesDiv.append(
      create("div", { className: "messageDiv" },
        create("img", { className: "messageImg", src: message.user.imgUrl }),
        create("div", { className: "bubbleDiv" },
          create("div", { className: "bubbleNameDiv", innerText: message.user.name }),
          create("div", { innerText: message.text })
        )
      )
    );
  }

  messagesDiv.scrollTop = messagesDiv.scrollHeight;
}

logOutButton.onclick = function () {
  localStorage.removeItem("userId");
  location.href = "index.html";
};

sendButton.onclick = async function () {
  await send<boolean | null>("addMessage", userId, messageInput.value);
  messageInput.value = "";
  await loadMessages();
};
