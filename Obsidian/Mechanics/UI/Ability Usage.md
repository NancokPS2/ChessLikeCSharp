---
tags:
  - mechanical
---
[[Ability|Abilities]] use a targeting system that is activated from the UI during a [[Unit]]s turn.
1. User selects an [[Ability]]
2. Enter Targeting state. (UsageParameters must have an Action defined.)
3. ActionEventTargeter shows the tiles that can be targeted.
4. Once position(s) have been targeted. Displays the actually affected tiles, known as AoE. 
5. **TODO:** Show which [[Unit]]s will be affected.
6. If a selected tile is selected again, it confirms the usage.
7. Signals the ActionEventRunner to add the selected ability to a queue, which may prompt others to also get added to the queue.

