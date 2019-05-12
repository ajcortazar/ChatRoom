# ChatRoom

Tarea

1. Allow registered users to log in and talk with other users in a chatroom. 	**Realizado**

2. Allow users to post messages as commands into the chatroom with the following format /stock=stock_code. 	**Realizado**

3. Create a decoupled bot that will call an API using the stock_code as a parameter (https://stooq.com/q/l/?s=aapl.us&f=sd2t2ohlcv&h&e=csv, here aapl.us is the stock_code). **Realizado**

4. The bot should parse the received CSV file and then it should send a message back into the chatroom using a message broker like RabbitMQ. The message will be a stock quote using the following format: “APPL.US quote is $93.42 per share”. The post owner will be the bot. **Realizado**

5. Have the chat messages ordered by their timestamps and show only the last 50 messages. **Realizado**

6. Unit test the functionality you prefer. # 	**No Realizado**

7. Use .NET identity for users authentication. **Realizado**

8. Handle messages that are not understood or any exceptions raised within the bot. **No Realizado**

9. Build an installer. **No Realizado**
