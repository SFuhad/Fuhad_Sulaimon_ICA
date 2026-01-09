# 📝 Engine Documentation

A shared place for quick notes, how-tos, reminders, and useful information discovered during development.

---

## 📌 Quick Tips

- Be careful when editing main, try to avoid it as much as possible to prevent conflicts. 
- Make sure all of our content is loaded into the dictonaries at the top of main. 
- To update the engine, copy over the **GDEngine** folder to the repo only.


## 🛠️ How-To Guides

### 🏠 Importing Models
   Steps:
   
   1. Open content.mcgb in Visual Studio
   2. Click into Assets -> Models
   3. With Models selected, click on Add Existing Item at the top *(Grey Box with Black Border and Green + Sign)*
   4. Select the .fbx file
   5. Click Build -> Build after it is successfully added
   6. Re-open Visual Studio
   7. Go to Content -> Assets -> Data and open `asset_manifest.json`
   8. Create a new entry in the models section
   9. Give it a name to be referenced by in the model dictionary
   10. Insert the path to the model
   11. Create through `multi_model_spawn.json` or individually in main


### 🖼️ Importing Textures
   Steps:
   
   1. Open content.mcgb in Visual Studio
   2. Click into Assets -> Textures
   3. With Textures selected, click on Add Existing Item at the top *(Grey Box with Black Border and Green + Sign)*
   4. Select the texture file
   5. Click Build -> Build after it is successfully added
   6. Re-open Visual Studio
   7. Go to Content -> Assets -> Data and open `asset_manifest.json`
   8. Create a new entry in the textures section
   9. Give it a name to be referenced by in the texture dictionary
   10. Insert the path to the texture
   11. When creating models, assign it this texture name


### 🔊 Importing Sounds
   Steps:
   
   1. Open content.mcgb in Visual Studio
   2. Click into Assets -> Sounds
   3. With Sounds selected, click on Add Existing Item at the top *(Grey Box with Black Border and Green + Sign)*
   4. Select the sound file
   5. In the properties panel, change import to `.wav` and processor to `sound effect`
   6. Click Build -> Build after it is successfully added
   7. Re-open Visual Studio
   8. Go to Content -> Assets -> Data and open `asset_manifest.json`
   9. Create a new entry in the sounds section
   10. Give it a name to be referenced by in the sound dictionary
   11. Insert the path to the sound 
   12. When playing audio in the `AudioSystem`, pass through the name you gave it


### 🖋️ Importing Fonts
   Steps:
   
   1. Open content.mcgb in Visual Studio
   2. Click into Assets -> Fonts
   3. With Fonts selected, click on `New Item` at the top *(Grey Box with Black Border and Yellow + Sign)*
   4. Select SpriteFont and give it a name
   5. Once created, open up the SpriteFont file in Notepad or Visual Studio
   6. Change the font name to your desired font
   7. If the font is not native to the PC, you will need to download and install it
   8. We use Inter since it supports English, Czech and Ukranian which means we had to download it from Google Fonts *(Fully Free)*
   9. Go to Content -> Assets -> Data and open `asset_manifest.json`
   10. Create a new entry in the fonts section
   11. Give it a name to be referenced by in the fonts dictionary
   12. Insert path to font
   13. When setting fonts in the `UserInterfaceController`, pass through the name you gave it


### 👂 The Event Channel System
   Steps:
   
   1. All event scripts are stored in Scripts -> Events
   2. There are seperate channels containing `EventBase` objects such as `InputEventChannel`
   3. These are then created and accessed through the `EventChannelManager`
   4. Each Event has `Subscribe()`, `Unsubscribe()` and `Raise()` functions
   5. You pass through functions with the `Subscribe()` method which are all called with the `Raise()` method
      ```cs
      /// Referencing the Input Event Channel
      _inputEventChannel = EventChannelManager.Instance.InputEvents;
   
      /// Creating the Fullscreen Toggle function
      private void HandleFullscreenToggle() => _graphics.ToggleFullScreen();
      
      /// The Code for Subscribing
      _inputEventChannel.FullscreenToggle.Subscribe(HandleFullscreenToggle);
   
      /// The Code for Calling
      _inputEventChannel.FullscreenToggle.Raise();
      ``` 
   6. The raise function will be called elsewhere, for example right now it is called through the `InputManager` class
   7. It is important to unsubsribe methods when they are disabled.
   8. Usually this is done in Unity through `OnDisable` but for our engine its in `Main.Dispose()`


### 🌲 Implementing IDisposable
   Steps:
   
   1. `IDisposable` is needed for MonoGame Components as they are unmanaged resources.
   2. You must release these unmanaged resouces yourself when they are no longer needed
   3. Add the `IDisposable` interface to any class that uses these by inserting `Class : IDisposable` at the top
   4. You will be prompted to add the `IDisposable` template and this will generate the functions needed
   5. The code should be structured like this, you will need to add your own `Clear()` function that handles disposal
      ```cs
      // Disposal Tracker
      private bool disposedValue = false;
   
      // Clear Function
      private void Clear()
      {
         _menuController?.Dispose();
         _menuController = null;
      }
   
      // Dispose Function
      protected virtual void Dispose(bool disposing)
      {
         if (disposedValue) return;
         
         if (disposing)
            Clear();
         
         disposedValue = true;
      }
   
      // Destructor
      ~UserInterfaceController()
      {
         Dispose(disposing: false);
      }
         
      public void Dispose()
      {
         Dispose(disposing: true);
         GC.SuppressFinalize(this);
      }
      ```
   6. Now with these implemented you can add the disposal call in your `Main` dispose function.


### 🖼️ Adding UI Text
   Steps:
   
   1. The UI Text is created in the `PlayerHUD` class and stored in a list called `_uiTextObjects`
   2. The `SpriteFont` and `PlayerStats` are passed through when the class is constructed in the `UserInterfaceController`
   3. Positions are created in a Dictionary at the top of the script so we can cleanly reference mulitple vectors
   4. The text is passed through as a `func()` not a `string` so you must assign the text with a function
   5. With the stats displays, we reference attributes from `PlayerStats`
   6. With the text, we pass through keys and fetch them from our `LocalisationController`
   7. To add localised text just call the below function and pass through a key and position:
       ```cs
       private void CreateText(string key, Vector2 pos)
       {
          // Creates a new GameObject to store the UI Text
          var textGO = new GameObject($"Text Object: {key}");
      
          // Construct the UI Text
          var uiText = new UIText
          {
             Color = _hudTextColour, // Text Colour
             Font = _hudFont, // Text Font
             LayerDepth = UILayer.HUD, // Text Layer
             TextProvider = () => LocalisationController.Instance.Get(key), // Text String Func
             PositionProvider = () => pos // Text Position
          };
      
          // Add the UI Text Component to the Game Object
          textGO.AddComponent(uiText);
      
          // Add the Text to the list of UI Text Objects
          _textObjects.Add(uiText);
      
          // Add to the current scene
          SceneController.AddToCurrentScene(textGO);
       }   
      ```

### 📓 Using AppData - Eliminating Magic Variables
   Steps:
   
   1. The `AppData` file is used to declare globally accessible variables.
   2. This is done with the aim of reducing the amount of magic numbers and strings in the project.
   3. `AppData` stores the name keys for Game Objetcs, Textures, Models, Sound Files etc.
   4. This makes it safe, easy to access and easy to understand.
```cs
   // Magic Number Version
   private void CreateText(string key)
   {
      CreateButton("Play");
   }

   /* -------------------------------------------*/
   // Declaration in AppData.cs
   public static readonly string PLAY_BUTTON_TEXT = "Play";
   /* -------------------------------------------*/

   // Improved Version
   private void CreateText(string key)
   {
      // This passes through "Play" just like above
      CreateButton(AppData.PLAY_BUTTON_TEXT);
   }
```

---

## 🧩 Common Problems & Fixes

### ❗ Problem: Model has no Texture 
**Fix:**  Load texture to dictionary and apply through code

---

## 📁 Project Workflow Notes

-  Engine is very basic, keep things simple.
-  Don't worry about lighting or animations at the moment.
-  Keep this updated as we progress.

---
