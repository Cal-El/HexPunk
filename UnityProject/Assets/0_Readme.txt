Folder Structure:

1_Scenes: All production and test scenes are stored here. Any appropriate to the build should be added through the build settings.
2_Scripts: C# programming scripts can be found in the 2_Scripts folder.
3_Models: Models can be found in the 3_Models folder. Each is stored in its own subfolder with its unity Material/s and Texture/s.
4_Prefabs: All prefabs can be found in the 4_Prefabs folder. Please check here before importing a model directly into the scene, as there may already be a prefab.
5_HUD: Any and all textures and sprites can be found in the 5_HUD folder. Ensure you manipulate the Player_Camera prefab when editing the HUD elements.


File Naming Conventions:

Models:		[Model Name]_[Type]*_[Variation]			e.g. Wall_C_I_3 = The third variation(3) of an inner(I) corner(C) wall(Wall)
Materials: 	M_[Name of Model]					e.g. M_Wall_C_I_3 = A material for the model in the previous example
Textures:	T_[Name of Model]_[Material Reference]_[Texture Type]	e.g. T_Wall_C_I_3_DefaultMaterial_Normal = The normal map for the material in the previous example
Audio:		[Audio Type]_[Name of File]_[Variation]			e.g. Music_Lobby_2 = The second variation of the lobby music
C# Scripts:	[Script Name].cs					e.g. Don't try to change this unless your name is "Joe" or "Callum"


Other notes:

When you are dealing with a prefab such as Enemy units, ensure you hit apply in the top right of your screen; otherwise the changes will only be made for those you editted directly. 
Instead of copy/pasting or duplicating (Ctrl+D) an object, make a prefab and use that, that way you can edit one and apply the changes to many/all.
(!) When making major changes to a Scene, duplicate the current scene first, in the project menu, so you don't fuck up the project.