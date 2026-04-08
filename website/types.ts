export type User = {
  name: string,
  imgUrl: string,
};

export type ChatMessage = {
  text: string,
  user: User,
};
