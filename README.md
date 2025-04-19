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