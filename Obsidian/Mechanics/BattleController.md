---
tags:
  - mechanical/combat
---
A major class in charge of all combat related functionality.
### Loading an encounter.
1. First deletes all children of the [[BattleController]] to start from scratch.
2. Sets the grid from the [[Encounter]] and stores the encounter itself.
3. Sets up all components of the BattleController. (**TODO:** Order the setup of signals?)
	1. Mesh display for mobs and connects it to the turn manager for turn related visuals.
	2. Grid display to show the terrain.
	3. Camera.
	4. Combat UI to display mob information and connects its inputs for selecting actions and ending turns.
	5. CanvasLayer node for displaying arbitrary UI elements. (**TODO:** Add a function to automatically instantiate ISceneDependency objects)
	6. ActionRunner for calling action functions during the designated state and connect it to the turn manager.
4. Loads in every combatant defined in the encounter.
	1. Update their position to the starting location
	2. Register them under the turn manager and mob display manager.
	3. Set them as being in combat.
After the steps are done, it starts running its state machine in the PREPARATION state for selecting [[Unit]]s.

### Running an encounter
1. If a new state has been selected, run its entering state. Otherwise run its processing logic.
2. 