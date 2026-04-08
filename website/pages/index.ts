import type { User } from "types";
import { send } from "clientUtilities";

var userToken = localStorage.getItem("userToken");
var user = await send<User | null>("getUser", userToken);
if (user != null) {
  location.href = "chat.html";
}
