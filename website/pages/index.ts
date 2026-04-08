import type { User } from "types";
import { send } from "clientUtilities";

var userId = localStorage.getItem("userId");
var user = await send<User | null>("getUser", userId);
if (user != null) {
  location.href = "chat.html";
}

