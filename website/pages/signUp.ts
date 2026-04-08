import { send } from "clientUtilities";

var usernameInput = document.querySelector<HTMLInputElement>("#usernameInput")!;
var passwordInput = document.querySelector<HTMLInputElement>("#passwordInput")!;
var confirmInput = document.querySelector<HTMLInputElement>("#confirmInput")!;
var imageInput = document.querySelector<HTMLInputElement>("#imageInput")!;
var profileImg = document.querySelector<HTMLImageElement>("#profileImg")!;
var submitButton = document.querySelector<HTMLButtonElement>("#submitButton")!;
var errorDiv = document.querySelector<HTMLDivElement>("#errorDiv")!;

var profileImageSrc = "/website/images/profile_default.svg";

imageInput.oninput = function () {
  profileImg.src = imageInput.value;
  profileImageSrc = imageInput.value;
};

profileImg.onerror = function () {
  profileImg.src = "/website/images/profile_default.svg";
  profileImageSrc = "/website/images/profile_default.svg";
};

submitButton.onclick = async function () {
  if (passwordInput.value != confirmInput.value) {
    errorDiv.innerText = "Passwords do not match.";
    return;
  }

  var userId = await send<string | null>("signUp", usernameInput.value, passwordInput.value, profileImageSrc);

  if (userId == null) {
    errorDiv.innerText = "A user with that username already exists.";
    return;
  }

  localStorage.setItem("userId", userId);
  location.href = "chat.html";
};