# Multiplayer Snake Game in C#

## 🐍 Overview
This project is a networked **multiplayer Snake game** built in C#. It features a GUI-based client and a server backend capable of managing game state and multiple connected players in real time.

The application demonstrates core principles in event-driven programming, game loop management, and client-server architecture.

---

## 🧱 Project Structure

### 🎮 `GUI`
The primary user interface built with Windows Forms or WPF. Renders the snake game, handles input events (arrow keys), and communicates with the server to update the game state.

### 🧠 `GUI.Client`
Handles all client-side networking logic — connecting to the server, sending user input (e.g., movement), and receiving updated game state (e.g., snake positions, food, scores).

### 🌐 `WebServer`
A custom-built server that maintains the authoritative game state, processes input from multiple clients, handles collisions, spawns food, and broadcasts updates to all clients.

---

## 🚀 Features
- Real-time multiplayer snake game
- Smooth keyboard input and snake movement
- Game state synced across connected players
- Ability to add AI-controlled players 
- Simple networking using sockets or TCP-based communication
- Modular architecture separating UI, client, and server logic

---

## 🛠️ Technologies Used
- C# (.NET 6+)
- Windows Forms or WPF
- `System.Net.Sockets` for TCP networking
- Visual Studio 2022+

---

## ▶️ How to Run

1. Open the solution in Visual Studio 2022 or later.
2. Set the `WebServer` project as the startup project and run it to start the game server.
3. In a separate instance of Visual Studio (or on another machine):
   - Set `GUI` as the startup project and run it.
   - Enter the server IP and port (if prompted) to connect.
4. Use arrow keys to control your snake.
5. Open additional GUI clients to join multiplayer mode.

---

## ⚠️ Notes
- Make sure the server IP and port are correctly configured in the client.
- Firewall/antivirus may need to be configured to allow network traffic.
- This version is for local testing; scaling to remote play would require NAT/firewall handling.

---

## ✍️ Authors
Brian Keller & Wyatt Young 
Biomedical Engineering BS/MS | Minor in Computer Science  
University of Utah
