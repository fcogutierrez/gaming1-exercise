# 🚀 HiLo game

## 📖 Description

Game where the player has to guess a number between a min and a max. The game will tell the player if the guess is too high or too low.

Once the players are created, the system will start the game and assign the first player to make a guess.

Then, on the subsequent player guesses, the system will orchestrate every turn and round, indicating in the response the result of the guess and the next player to play.

The game ends when one of the players successfully guesses the correct number.

## 📦 Tech details

The project has been built in .NET 9 using C# and consists on a web API that can serve a web client or an API-to-API client.

Using DDD and a clean architecture approach, the solution has been designed in order to differenciate four main layers:

- **Domain**: Contains the core business logic and domain model (e.g. the `GameAggregate`, the `Player` entity or other value objects).
- **Application**: Contains the orchestration logic and use cases. It is responsable for coordinating the flow of data between the domain and the infrastructure layers. All the use cases have been implemented as commands, which are executed by the command handlers and return a specific result.
- **Infrastructure**: Concrete implementations of the interfaces defined in the application layer. In this project, it just contains a library with an in-memory implementation of the event storage - whose the application/domain layer are agnostic.
- **API**: Contains the web API. This layer is responsible for exposing the application layer to the outside world, using HTTP endpoints. It contains the controllers and the DTOs (data transfer objects) used to communicate with the clients.

## 🧪 Tests

As the core of the logic is holded in the domain layer, the project contains a reasonable set of tests that cover all the business logic and the different corner cases, including the validation of the commands and the events. 

The tests are located in the `tests` folder. The tests are using **xUnit** and **Moq** as the mocking library.

## 🛠️ How to run the project

### Pre-requisites
- Docker installed and running
- Visual Studio 2022 or later with .NET 9 SDK installed

First, let's clone the repository:

```bash 
git clone https://github.com/fcogutierrez/gaming1-exercise.git
```

Now, open the solution `HiLoGame/HiLoGame.sln` with Visual Studio, set the project `HiLoGame.Api` as startup project, and run it with **Docker**. 

You can start playing the game by opening any of the `.http` files below - preferably using the **REST Client** extension for Visual Studio Code and following the instructions in the comments.

- `HiLoGame/HiLoGame.SinglePlayer.http`, to play a single player game.
- `HiLoGame/HiLoGame.MultiPlayer.http`, to play a multiplayer game. By default it is configured to play with 2 players, but you can adapt it to play with more.

Please note that the game is designed to be played using the REST API, so you can use any HTTP client to play the game. The `.http` files are just a convenience to make it easier the demo.

## 🧩 Endpoints

### `POST /games/hilo`
Create a new game. The request body should contain the following body:

```json
{
  "min": 1,
  "max": 100
}
```

The response will contain the game ID.

```json
{
  "id": "1d2f3e4a-5b6c-7d8e-9a0b-b1c2d3e4f5g6"
}
```

### `POST /games/hilo/:id/players`
Add the players to the game. The request body should contain the following body:

```json
{
  "players": [
    {
      "name": "Player 1"
    },
    {
      "name": "Player 2"
    }
  ]
}
```

Once the players are added, the game will start and the endpoint returns the list of the created players - with its corresponding `id` and `order` - and the turn of the first player to guess.

```json
{
  "players": [
    {
      "id": "16bf63a1-0047-4663-8bad-b477991098b0",
      "order": 1,
      "name": "Player 1"
    },
    {
      "id": "70a5bc71-93aa-471e-ab80-e38d72d52e7c",
      "order": 2,
      "name": "Player 2"
    }
  ],
  "playerTurn": {
    "id": "16bf63a1-0047-4663-8bad-b477991098b0"
  },
  "currentRound": 1
}
```

### `POST /games/hilo/:id/players/:playerId/guess`

Make a guess. The request body should contain the following body:

```json
{
  "guess": 50
}
```

Now, there are two potential scenarios:

1. The guess is correct. The response will contain the winner's id and reveal the mistery value:

```json
{
  "winner": {
    "id": "16bf63a1-0047-4663-8bad-b477991098b0"
  },
  "value": 64
}
```

2. The guess is incorrect. The response will contain the result of the guess (`tooHigh` or `tooLow`), the next player to play and also the current round: 

```json
{
  "playerTurn": {
    "id": "70a5bc71-93aa-471e-ab80-e38d72d52e7c"
  },
  "currentRound": 2,
  "previousPlayerGuessStatus": "TooHigh"
}
```

Please note that, once the players are created, the game has started and it is not allowed to make changes in the players - e.g. update the players.

The game will start at the round 1, and it is incremented once the last player in the list makes his/her guess. Then, a new round will start again and will be turn for the first player - the order of the players is preserved and will be always the same across all the rounds.

The system will not allow that the players guess out of order or guess on a finished game.

## 👨‍💻 Potential improvements

This project is a proof of concept and has been designed to be simple and easy to understand. However, there are some areas for improvement:

- Validation of the input parameters in the API layer using `FluentValidation` or similar.
- Add some decorators to the command handlers in order to provide some logging/metrics, and to handle the **Application** unexpected exceptions in a generic way - with a proper retry policy. Possibility to use  libraries as `Mediatr` to handle the communication between **HiLoGame.Api** and **Application**. We can use `Polly` for the retries.
- Treat the domain errros as potential type of results in a `OneOf` approach, so the command handlers can return a result with the error instead of throwing an exception. This way, the API layer can handle the errors in a more generic way and return a proper response to the client.
- Implementation of tests in the endpoints, to test the whole flow and the integration with the application layer.
- Implementation of the `GET /games/hilo/:id` in order to get the game state at any point.
- Add a discriminator to the response of the `POST /games/hilo/:id/players/:playerId/guess` endpoint in order to help the client to differenciate between the two possible responses, since the response will be polymorfic.