# Testing Blazor components with bUnit

A small Blazor Server app (a list of upcoming baseball games that need umpires) and an xUnit test project that tests its components with [bUnit](https://bunit.dev/): rendering, events, injected services, `AuthorizeView` and JavaScript interop.

Companion code for the blog post: [Testing Blazor components with bUnit](https://www.gabrielmongeon.ca/en/blog/testing-blazor-components-bunit) ([version française](https://www.gabrielmongeon.ca/blog/tester-composants-blazor-bunit))

## Project Structure

```
blazor-bunit-testing/
  Arbitres.slnx
  Arbitres.Web/                    # Blazor Web App (net10.0, interactive server)
    Game.cs                        # Game record (MissingUmpires computed property)
    IGameService.cs                # Service used by GameList
    InMemoryGameService.cs         # In-memory implementation so the app can run
    Components/Games/GameCard.razor  # One game: missing umpires, volunteer button, copy-link button (JS interop)
    Components/Games/GameList.razor  # Loads games in OnInitializedAsync, AuthorizeView greeting
    Components/Pages/Home.razor    # Page that displays GameList
  Arbitres.Web.Tests/              # xUnit + bunit 2.11
    GameCardTests.cs               # Render, MarkupMatches, Click, EventCallback, strict JSInterop
    GameListTests.cs               # Fake service, WaitForAssertion, FindComponents, AddAuthorization
    FakeGameService.cs             # IGameService backed by a TaskCompletionSource
```

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)

## Running the tests

```bash
dotnet test
```

## Running the app

```bash
dotnet run --project Arbitres.Web
```

The app has no authentication configured, so `AuthorizeView` always shows the "Se connecter" link (the link itself goes nowhere). The authenticated case is covered by the tests with `AddAuthorization()` and `SetAuthorized()`.

## Notes

- The examples use bUnit 2.x. If you're coming from bUnit 1.x: `TestContext` is now `BunitContext`, `RenderComponent<T>()` is now `Render<T>()` and `AddTestAuthorization()` is now `AddAuthorization()`.
- Tests are plain C# classes, so the test project keeps the `Microsoft.NET.Sdk` SDK from the xUnit template. To write tests in `.razor` files, switch it to `Microsoft.NET.Sdk.Razor`.
- The components live in the `Arbitres.Web.Components.Games` namespace; the test project imports it with a global `<Using>` in its `.csproj`.
- bUnit's JSInterop runs in strict mode by default: any JavaScript call that isn't set up fails the test.
