# LiftControl System

This is a simulation of a smart elevator system built with C# with Console App and Blazor for presentation. It supports multiple elevators, handles passenger routing logic, and includes a web-based dashboard to visualize elevator movement in real time.

## How it is structured

This solution is made up of multiple class projects to keep things organized:

- LiftControl.Application - Coordinates app-level logic like routing
- LiftControl.BlazorUI - Web dashboard to visualize elevator activity
- LiftControl.Config - Contains shared configuration values
- LiftControl.ConsoleApp - Runs the simulation via console
- LiftControl.Domain - Core business logic (LiftUnit, Request handling, etc.)
- LiftControl.Infrastructure - Logging and exporting tools (e.g., JSON snapshots)
- LiftControl.Shared - Shared models like LiftSnapshot
- LiftControl.Tests - Unit tests to validate logic

## Features

- Simulates multiple elevators moving independently
- Smart passenger request routing based on direction, distance, and load
- Filters duplicate requests (same pickup and destination)
- Rejects overcapacity lifts and wrong-direction assignments
- Automatically generates simulation data
- Web UI updates live from JSON snapshot file
- Responsive and animated dashboard (Blazor)

## How to run

### Console Simulation (with JSON export)
1. Set **LiftControl.ConsoleApp** as the startup project
2. Run the app
3. It will simulate elevator movement and generate a file at:
/LiftControl.BlazorUI/wwwroot/snapshot.json this file will be used by the BlazorUI to show lift activity.


### Web Dashboard
1. Open **LiftControl.BlazorUI** in your browser
2. The UI shows lift status, queue, and passengers
3. Data updates every second from the snapshot

## How to test

1. Set **LiftControl.Tests** as the test project
2. Run tests via Test Explorer or `dotnet test`

---