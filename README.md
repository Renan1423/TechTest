# TechTest
Game programming technical test from the Educação Digital Games company. By Renan Nunes 

Technical Game Documentation: Educação Digital Games
Author: Renan Nunes

1. Defining the problem's solution
	The technical test is a practical way to show the applicant’s programming skills with C#, code architecture and handling the Unity game engine. With that in mind, in this exercise I’ve focused on displaying highly polished codes and good practices, thinking this exercise as a game that could be developed inside the company with other team members, while also creating flashy, yet simple, art directions with animations and a good UI/UX experience.
	The problem required the creation of an algorithm for creating and shuffling the puzzle, while also generating an image with the tiles on the board. To solve this, I’ve divided the exercise in the following topics that are shown on the summary:
Puzzle architecture
Puzzle shuffling algorithm
Images generation
Victory and Defeat
Score and Time Manager
Art direction and visual feedbacks
Mobile adaptation
Polishments

2. Puzzle architecture
In order to create the puzzle system, I’ve created the following classes:
Puzzle Manager: the main class of this system, acts as a mediator between all other classes responsible for the puzzle, handling the dependencies and connections between every class while also triggering the puzzle creation and victory and defeat states;
Puzzle Grid: responsible for handling the positions and references of every Puzzle Tile on the board. Contains methods that verifies empty adjacent tiles, which is useful for allowing or not the selection and movement of the Puzzle Tiles;
Puzzle Tiles Spawner: responsible for spawning the Puzzle Tiles and returning the matrix of tiles to the Puzzle Grid when required. In order to spawn the tiles, it uses the Object Pooling system in the project in order to reutilize the tiles along the gameplay. On the spawn method, this class spawns the tiles in the correct order (0 to 8), while maintaining the last tile empty.
Puzzle Tile: represents the tile itself. Inherits from the IDragHandler interfaces from Unity in order to make it easier to grab using the click of the mouse or the touch of the touchscreen. It contains events that are called when the selection is required by the player, asking the Puzzle Manager for permission to move around the board;
Puzzle Tile Visuals: handles the visual effects and references to components related to the visual of the puzzle tile.

3. Puzzle shuffling algorithm
In the exercise, a very important detail is highlighted: “The puzzle should NEVER start completed and, also, the puzzle must be possible to complete by the player using the empty tile logic”. In order to achieve that, I’ve used the “Drunkard’s Walk” algorithm. This algorithm simulates the movement the player could do around the board using the unique empty space present, creating a puzzle that is totally possible to solve, while also having a totally random approach in its movement in order to create the shuffling logic. Also, I’ve implemented an iteration counter for this algorithm, setting 100 as the default value, which means the empty tile of the board moves one hundred times when shuffling the tiles, making it probabilistically very hard to generate a puzzle that already starts completed.


4. Images generation
	When creating the image generation system, thinking as a project that I’ve could be making inside the company, I’ve created a system that reads a Json file, which can be generated using a spreadsheet, containing the informations of each level and image in the game, making it very dynamic for editing and balancing the levels of the game, easily connecting the programming and game design departments of the team. In order to achieve that, I’ve created a system that reads the Json files, getting its informations, and returns the images from the default Resources folder from Unity. When getting a valid image, the code also equally crops the given image and returns an array of sprites in order to apply them to each Puzzle Tile in the game.

5. Victory and Defeat
	To create victory and defeat conditions, I’ve made the following decision: in order to win, the player must complete the puzzle, which unlocks the next stage/puzzle, having the option to continue playing or returning to the main menu. For the defeat condition, I’ve created a timer that, when reaching to zero, ends the match and calls the defeat screen. The defeat screen contains a retry button, resetting the current puzzle.

6. Score and Time Manager
	In order to create the score and time manager, I’ve created two simple systems to handle those functionalities. The Score Manager is a simple counter that increases every time the player successfully completes a puzzle, while the Time Manager contains a Timer class component that handles the time counting and call events on start, tick and complete, while also presenting visual feedbacks for each second passed.

7. Art direction and visual feedbacks
	For the art direction, since its a programming technical project, I’ve decided to follow using a simple, yet beautiful, visual using free digital arts assets from the internet while also applying some shaders on top of it in order to generate visuals that resemble hand drawn arts. In order to use the shaders, I’ve used the paid asset “All in 1 Sprite Shader”. The intention here was to show what I can do to the visuals of a game if the company provides me the arts and the tools to do so.

8. Mobile adaptation
	For the mobile adaptation, I’ve made UI’s that are responsive and adapted to the sizes of cellphones and tablets. Also, the entire game works using UIs, which makes it automatically playable on mobile devices.

9. Polishments
	As polishments, besides the VFXs and SFXs, I’ve create a Cheat Manager class that handles editor’s development cheats, like showing tips of the correct positions of each tile when pressing the “O” key or automatically winning the stage when pressing the “V” key.



