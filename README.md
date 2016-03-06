# HangarGrid
A plugin for Kerbal Space Program that adds a visual grid to hangars

**Description**

Have you ever been annoyed by ending up with misaligned engines or clubfooted landing gear after spending half-an-hour
trying just to guess where to rotate to get them right?
This mod adds a grid to SPH and VAB and "laser" guides for an active part to let you clearly see where the things are wrong and  quickly align it manually or with hotkeys.

**Installation**

Extract contents of the released archive into your KSP/GameData folder.

**Usage**

While in SPH or VAB press the button to enable the grid. Once there's a root part you'll see the grid with origin bound to the part.
If you try to add or edit another one you'll see the red "laser" guides representing all the three axes for it and all of its symmetry counterparts. Now you have all the planes and axes visualized and it's the matter of several clicks to align the things.

The guides have distinct colors for each direction. You can easily remember them in the following way:
* Red for Rechts (Right)
* Violet for Vorwarts (Forward)
* Pink for Up (almost the same as the thrust vector)

Once you have rotated a part to approximately face the desired side, use the following hotkeys for autoalign the desired part's axis to be parallel to the closest grid line (regardless of direction):
* **L** - aligns the part by a guide at the mouse pointer to the closest grid line
* **J** - aligns the part's up (pink guide) to the closest grid line
* **N** - aligns the part's forward (violet guide) to the closest grid line
* **M** - aligns the part's right (red guide) to the closest grid line
* **G** - select a part under mouse pointer as the grid's origin
* **K** - toggles guides for symmetry (deKlutter). May be useful when working with many radially symmetrical parts creating a bunch of guides which makes them complex to use. This button let's you see ones only for the original part.

When using the separate keys for alignment (J, N, M), if you are in the part's editing mode (rotation or offset) it is sufficient to just press a key. Otherwise you'll need to move the mouse pointer over the desired part and then press.

You can change the keys by editing HangarGrid.cfg in the mod's folder. Valid values are listed inside the file.

**Know issues**

Autoalignment of a part desynchronizes it's rotation and position to the current rotation gizmo. So if you press an alignment key while in rotation mode, it is recommended to re-enable the mode by pressing "1" - "3", for example. Otherwise once you touch the gizmo again the part jumps to the gizmo's present position and rotation.

**Planned features**

* A GUI for grid customization that let's you change the look and declutter the view abit.

