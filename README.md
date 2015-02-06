# Match em Up - Main Game Play Code
Main game play code written for the game Match em Up in C# / Unity

These are the main gameplay scripts used to make a board game in Unity / C#.

This game is built for old people with Dementia to check and improve their memory.
Game was selected to be presented in “IT’s all in the game” event, on 4 march 2015 
organized by mental healthcare professionals, in The Netherlands.

You can see the trailer here: https://www.youtube.com/watch?v=fjwt4DWpRRI

The scripts in this repository are all written by me - Aldo Leka (with exception of Kinect support scripts).
They include:
* Game.cs to initialize the most important scripts.
* Global.cs to get global references such as the current level or to prepare for next level.
* RowsManager.cs to manage and create the rows shown in the left side of the screen.
* RowScript.cs to manage and create the items in a row.
* SelectItemsManager.cs to manage and create the selectable items on the right side of the screen.
* SelectItemScript.cs to provide the required behavior of a selectable item (with automatic / manual movement).
* ShowReelScript.cs to create a show reel where items can be pushed, which are than framed by the script and shown.
It supports player input to switch between different items (framed pictures) and exit.

Scripts for GUI / Kinect Input / User Data saving, loading / User Charts and User Pictures loading are not included since they are not written by me.
