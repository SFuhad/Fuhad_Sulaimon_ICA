# 📝 Game Documentation

A place for nots on any classes created for our game in the engine.

---

## ⚙️ Custom Systems

### ⛰️ [Scene Controller](../GDGame/Scripts/Systems/SceneController.cs)
- Created in `Main`
- Stores the current scene and any new ones created
- In `Main` the current scene is gotten from `SceneController.CurrentScene`

### 🏡 [Scene Generator](../GDGame/Scripts/Systems/SceneGenerator.cs)
- Created in `Main`
- Generates the Skybox and Ground

### 🗽 [Model Generator](../GDGame/Scripts/Systems/ModelGenerator.cs)
- Created in `Main`
- Returns created Models

### 🎮 [Input Manager](../GDGame/Scripts/Systems/InputManager.cs)
- Created in `Main`
- Creates the Input System based off of the given settings
- Sends out events through the `InputEventChannel`

### 🖌️ [Material Generator](../GDGame/Scripts/Systems/MaterialGenerator.cs)
- Created in `Main`
- Generates the Lit, Unlit and UnlitGround Materials

### 🕐 [Time Controller](../GDGame/Scripts/Systems/TimeController.cs)
- Created in `Main`
- Controls the `Time.TimeScale` which pauses and unpauses the game

### 🌍 [Localisation Controller](../GDGame/Scripts/Systems/LocalisationController.cs)
- Initialised in `Main`
- Creates Dictionaries for each language option
- English is used as the key and fallback option
- Options are stored in `.csv` files in the `Languages` folder
- When accessing text to display in game, you use `LocalisationController.Get("text")`

### 🎥 [Cinematic Camera Controller](../GDGame/Scripts/Systems/CineCamController.cs)
- Created in `Main`
- Controls the cinematic intro to the game with the moving camera
- Plays narrator audio through the `AudioEventChannel`
- Switches to the `PlayerCamera` upon completion

### 💽 [Game State Manager](../GDGame/Scripts/Systems/GameStateManager.cs)
- Created in `Main`
- Stores the current state of the game which include `GameActive`, `GameInactive`, `GameOver` and `GameWon`
- Triggers related events upon state change

---

## ⚙️ Player Systems

### 🕹️ [Player Controller](../GDGame/Scripts/Player/PlayerController.cs)
- Created in `Main`
- Stores the `PlayerMovement` and `PlayerCamera` logic

### 🎥 [Player Camera](../GDGame/Scripts/Player/PlayerCamera.cs)
- Created in `PlayerController`
- Handles the first person perspective logic

### 🚶 [Player Movement](../GDGame/Scripts/Player/PlayerMovement.cs)
- Created in `PlayerController`
- Handles the physics based player movement

### ♥️ [Player Stats](../GDGame/Scripts/Player/PlayerStas.cs)
- Created in `PlayerController`
- Stores the player stats such as Health and Orbs Collected

---

## 🖥️ User Interface Systems

### 💻 [User Interface Controller](../GDGame/Scripts/UI/UserInterfaceController.cs)
- Created in `Main`
- Takes in the Fonts and 2D Textures on construction

### 🖱️ [Cursor Controller](../GDGame/Scripts/UI/UserInterfaceController.cs)
- Created in `UserInterfaceController`
- Creates the Reticle in in the middle of the screen.
- Loads the texture named `reticle` from the JSON

### ❤️ [Player HUD](../GDGame/Scripts/UI/PlayerHUD.cs)
- Created in `UserInterfaceController`
- Creates the player HUD in the top left of the screen
- Displays the data from `PlayerStats` and updates each frame

### ⏸️ [Pause Menu](../GDGame/Scripts/UI/PauseMenu.cs)
- Created in `UserInterfaceController`
- Creates the pause screen that toggles when called
- Displays a resume and quit options

### 🥘 [Menu Controller](../GDGame/Scripts/UI/MenuController.cs)
- Created in `UserInterfaceController`
- Lightly modified version of `MenuManager`
- Controls the main menu display
- Updates Music and SFX volume through sliders

---

## 🎮 Game Systems

### ⚙️ [Trap Manager](../GDGame/Scripts/Traps/TrapManager.cs)
- Created in `Main`
- Creates a list of `TrapBase` to add to the scene
- Initialises all of the traps
- Updates the list of traps each frame

### 🏹 [Trap Base](../GDGame/Scripts/Traps/TrapBase.cs)
- Base abstract class all traps inherit from
- Holds the trap `GameObject` and core abstract functions all traps need

### 🪓 [Moving Trap](../GDGame/Scripts/Traps/MovingTrap.cs)
- Inherits from `TrapBase`
- Holds the logic for a moving trap such as platforms or axes

### 🛞 [Rotating Trap](../GDGame/Scripts/Traps/RotatingTrap.cs)
- Inherits from `TrapBase`
- Holds the logic for a rotating trap such as the guillotine

---

## 🎹 Audio Systems

### 🔉[Audio Controller](../GDGame/Scripts/Audio/AudioController.cs)
- Created in `Main`
- Encapsulates functions for playing audio to clear up main
- Takes in the Sounds Dictionary when constructed

### 🔊 [3D Audio Controller](../GDGame/Scripts/Audio/3DAudioController.cs)
- Created in `AudioController` and attached to a `GameObject`
- Takes in a sound effect, spawn position, volume and radius
- Plays 3D Audio in the Game Scene

---

## 🛥️ Event Systems

### ⛵ [Event Base](../GDGame/Scripts/Events/Channels/EventBase.cs)
- Uses C# `event Action` as a base
- Uses `Subscribe` to add a listener to the event
- Uses `Unsubscribe` to remove a listener from the event
- Uses `Raise` to call the event

### 🕹️ [Input Event Channel](../GDGame/Scripts/Events/Channels/InputEventChannel.cs)
- Created in `EventChannelManager`
- Events created from `EventBase`
- Controls Fullscreen Toggle, Pause Toggle, Movement and Exit Events

### 🧔 [Player Event Channel](../GDGame/Scripts/Events/Channels/PlayerEventChannl.cs)
- Created in `EventChannelManager`
- Events created from `EventBase`
- Controls Game Over and Game Won events

### 📀 [Game Event Channel](../GDGame/Scripts/Events/Channels/GameEventChannl.cs)
- Created in `EventChannelManager`
- Events created from `EventBase`
- Controls Game Specific Events such as the Game Starting for the first time

### 📻 [Audio Event Channel](../GDGame/Scripts/Events/Channels/AudioEventChannel.cs)
- Created in `EventChannelManager`
- Events created from `EventBase`
- Controls Audio requests such as Music, Narration and Sound Effects

### 🧰 [Event Channel Manager](../GDGame/Scripts/Events/Channels/EventChannelManager.cs)
- Static Class that can be accessed anywhere
- Initialised in `Main`
- Creates a `PlayerEventChannel` and `InputEventChannel`

---
