import { send } from "clientUtilities";

var usernameInput = document.querySelector<HTMLInputElement>("#usernameInput")!;
var passwordInput = document.querySelector<HTMLInputElement>("#passwordInput")!;
var submitButton = document.querySelector<HTMLButtonElement>("#submitButton")!;
var errorDiv = document.querySelector<HTMLDivElement>("#errorDiv")!;

submitButton.onclick = async function () {
  var userToken = await send<string | null>("logIn", usernameInput.value, passwordInput.value);

  if (userToken == null) {
    errorDiv.innerText = "Wrong username or password.";
    return;
  }

  localStorage.setItem("userToken", userToken);
  location.href = "chat.html";
};
