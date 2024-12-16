---
tags:
  - mechanical
---
[[Ability|Abilities]] use a targeting system that is activated from the UI during a [[Unit]]s turn.
1. User selects an [[Ability]]
2. Enter Targeting state. (UsageParameters must have an Action defined.)
3. OnEnter() display how far the ability can be targeted from the current location.
4. Once position(s) have been selected, display the affected locations. 
5. **TODO:** Show which [[Unit]]s will be affected.
6. Show a popup to confirm the selected positions.
7. Change state to RunningAction to use ActionEvent.ActionRunner.

