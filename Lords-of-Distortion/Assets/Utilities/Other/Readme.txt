Gerardo Perez

-----------
Copy Meta
-----------
Usage: 
CopyMeta [file1] [file2]
e.g. "CopyMeta spriteSheet1.png spriteSheet2.png"

Purpose:
Overwrites the contents of the file2's .meta file with those of file1's. Retains the  guid of file2, so that the same file is referenced in Unity.

-----------------
Replace Sprites 
-----------------

Usage:
ReplaceSprites [animation] [spritesheet]
e.g. "ReplaceSprites jumpAnimation.anim jumpSheetRed.png"

Purpose:
Replaces every single reference to a sprite sheet in the original animation with references to the new sprite sheet.