# SWE-AT-2

## Week 3 (B Week) - Focus: Preliminary Planning and Research

**Monday 4/5/26** - Today, we had a class discussion about the assessment task and sharing ideas on what we want to create and what environment we are programming in. I am deciding to code in Unity and I am going to make a game based on *Hunt the Wumpus* with more enemies to add complexity.

**Wednesday 6/5/26** - Today, I created my Unity project and started creating one of my ideas for the game, which was a notebook where you could take notes and a map with pins that you could move. I successfully managed to add this feature, although it will need some polishing in its visuals sometime later as it is using the default font and circle sprites. I had some issues with making the pins draggable, but it was easily fixed by reordering the GameObjects in the Inspector so that the pins would render above everything else.

**Friday 8/5/26** - Today, I found a free font pack called *BoldPixels* that I will be using for my project, as it will compliment the pixel-art aesthetic that I will be going for. I also started both my documentation and some pixel art for the player sprite. For the pixel art, I wanted to try a new approach for character design, so I tried to design the character from a diagonal angle, which proved to be a little difficult.

**Weekend 9/5/26 - 10/5/26** - I spent a bit of time working on some prototype pixel art for the enemies. (Add an image later)

## Week 4 (A Week) - Focus: Identification of Classes, Objects, System Diagramming

**Monday 11/5/26** - Because I forgot to commit my changes, I wasn't able to work on the programming aspect of my project, so instead I mostly focused on catching up on the documentation since I was a little behind. I managed to finish most of the first section - Design of Characters and Environment.

**Wednesday 13/5/26** - *(No class due to Athletics Carnival)*

**Thursday 14/5/26** - Today, I completed more of my documentation. I finished the first section completely as well as the research on *The Genesis of Wumpus*. I also started one of the most vital parts of the programming aspect, which was the map for my game. I did this by creating a dictionary of each room with the rooms it attaches to, which likely isn't the most optimised solution but it would work for now.

**Friday 15/5/26** - Today I did more programming and added a transition between rooms, as well as detection for if there is an enemy or boss adjacent to your room.

## Week 5 (B Week) - Focus: Programming, Asset Creation/Identification and Journaling

**Monday 18/5/26** - I wanted to try and add animation, so I created an animated background for when you encountered an enemy or a boss. I watched a short tutorial to remind myself of how to use Unity's built-in animation system, and by the end of the lesson I was able to trigger an animation sequence. I also started working on a player sprite animation.

**Wednesday 20/5/26** - I decided that the background to my game made it feel very low-quality since nothing was moving and there were only buttons to click. To solve this, I decided to change the game so that, instead of a background and clicking arrows to move, there would be a movable player sprite. I created a player sprite similar to the one I used in my Year 9 game project, however I made it larger and with more detail (such as a bit of shading) to make it look better and more polished. I made it able to move and added the appropriate animations and the ability for it to have collisions. I also completed the Success Criteria and Procedural vs OOP table for my documentation.

**Friday 22/5/26** - Today, I created a map for the background of my game that the player would be in. I looked up some pixel art caves to base the colour scheme of mine off of, and about halfway through the lesson the map was done. I also smoothed out the room transitions by adding hitboxes in the corners of the map to allow the player to walk between rooms.

## Week 6 (A Week) - Focus: Programming, Asset Creation/Identification and Journaling

**Monday 25/5/26** - Today, I completed my Data Flow Diagram and continued coding. The data flow diagram was pretty easy to finish, although I will likely add some more detail to it later if it is too simple. I added hitboxes around the map to prevent the player from walking off the screen.

**Wednesday 27/5/26** - Today, I fully completed my structure chart. It took a while to finish, but I modelled it after the structure chart I made last term and read through Mr Clark's feedback to make sure it was improved. Mainly, I realised that structure charts do not use arrows, which was a mistake I made last term that I made sure to improve upon. I ran out of time for now, but in the future I need to restructure the chart to give it a tree-diagram-like shape, instead of the flow chart look that it currently has.

**Thursday 28/5/26** - I created a new animation that would play whenever the player listens to dialogue, which would be when they get given warnings of nearby obstacles. It took some time to add it into the animation cycle, but I managed to get it working. I also separated most of my variables and functions from my Game Manager file into the other Player and Obstacle objects to make the OOP more clear. I realised that most of my code was taking place inside one object since I'm very used to coding imperatively.

**Friday 29/5/26** - We played Minecraft: Education Edition. Also, I decided that there were some aspects of my game plan that needed to be changed in order for the game to work. Mainly, I realised that the idea is very boring to play and is way too similar to the original game. To change this, I decided to replace the enemies with rooms where groups of enemies would spawn and go for a more combat-focused style for the game.

## Week 7 (B Week) - Focus: Programming, Asset Creation/Identification and Journaling

**Monday 1/6/26** - Today, I managed to get the dialogue system working, which will be used whenever the player decides to use the scanner. The dialogue box is essentially just a black box with text, so I’ll probably add a custom sprite for it sometime later.

**Wednesday 3/6/26** - I made some more optimisations to my code, mainly since I discovered the existence of being able to put headers in the attributes and SerializeField making private variables visible in the Inspector. These are both extremely useful and I’m annoyed I didn’t know about them until now. I also created a new sprite for the map so that it was much larger to allow for the player to move around more freely and make the map feel larger.

**Friday 5/6/26** - I made the enemies able to fire projectiles, however I didn’t really like the idea of the game turning into a bullet hell, so I ended up scrapping them and instead made the enemies able to attack the player on contact. I also added the ability for the player to dash to get around more quickly.

## Week 8 (A Week) - Focus: Programming, Journaling and Testing and Evaluating

**Monday 8/6/26** - Today, I finally decided to create sprites for the enemies. Before this point, the enemies shared the player’s sprite, so the game looked much better with unique sprites for them. I also designed a sprite for the Wumpus, which I decided would be the final boss. However, coming up with a design for the Wumpus took a while, mainly because the original Wumpus barely had any descriptions of what it looked like so I had to come up with something myself.

**Wednesday 10/6/26** - Today, I managed to make the player attack the enemies. To do this, I decided to go for a Brotato-style combat system, where the player would have a sword that floats around them and can attack enemies. To attack, the player would click and the sword would attack in the direction of the cursor. I tested the combat myself through subsystem testing and tweaked the range of the enemies and the players, so the combat ended up being mostly about positioning as you were guaranteed to take damage if you ended up being cornered or had to dash past the enemies' range.

**Thursday 11/6/26** - Today, I coded in a visual damage indicator, so that whenever something was damaged it would flash red. I started learning how to use Unity's particle system to also add a red particle effect on hit, however it came out looking a bit weird. Eventually, I tweaked the settings so that it looked decent. 

**Friday 12/6/26** - I started programming the boss, which I started by creating a boss child class that inherits everything from the enemy parent class. This child class will be used to add custom attacks and behaviours to the boss. I started by giving the boss a charge attack, however while testing it I noticed that the boss would always charge instantly, so I will fix this in the future.

**Weekend 13/6/26 - 14/26/26** - I added the tutorial and a title screen for the game, which would be definitely important for playtesting next week. I decided to make the tutorial a series of images from a Google Slides presentation so it would be a bit more engaging than a text-based tutorial, and I made some custom drawings to put on it (which took a fair bit and probably wasn't worth it). I also finally added a map hitbox and used different hitbox layers to prevent the player from colliding with enemies, since testing revealed that the enemies can surround the player and softlock them. I used this featuer to add a boundary for the enemies so they couldn't leave the map, and made sure it didn't affect the player. I also managed to get trap rooms working, which were similar to the original game's bottomless pits, however instead of an instant kill they deal exponential damage to encourage the player to leave instead of rushing through it.

## Week 9 (B Week) - Focus: Final Documentation, Creating Presentations and Testing and Evaluating

**Monday 15/6/26** - I finalised the Wumpus boss fight by adding two more attacks: a stomp which served as a more powerful base attack to encourage the player to keep a distance, and a projectile attack where the Wumpus would occasionally spawn five projectiles, which was good to make used of the unused projectiles from earlier. There was also a recurring issue with the Wumpus dying causing errors, so I linked it to the enemy group spawning code where it would appear as a group by itself. I tested the boss and had to mainly adjust the stomp radius since it was so large it was very unfair to play against.

**Tuesday 16/6/26** - I added a death screen and programmed it so that, once the player reached zero health, the death overlay would appear and the player would be able to retry (which would reload the scene) or return to the title screen. I also balanced the Wumpus to deal much more damage if the other enemy groups weren't cleared to encourage the player to deal with them first and avoid the Wumpus room. This pretty much wrapped up programming the game, as the only things I still need to do are playtesting and bugfixing, and maybe polishing up a bit such as adding a projectile sprite or a separate player sprite when the player dies. Since the game was finished, I fully completed my Data Dictionary and made sure that I didn't miss a single bit of data across all of my files. I plan to also update my Structure Chart, Class Diagram and DFD to match the new system for the game since they were still modelled after my original idea which I had changed. Afterwards, I played through the game to make sure there weren't any immediate issues, and I didn't find any bugs.

**Wednesday 17/6/26** - *(Sick)*

**Thursday 18/6/26** - score, structure chart + class diagram

**Friday 19/6/26** - Assessment Submission Due

## Week 10 (A Week)

**Whole Week 22/6/26 - 26/6/26** - Presentations
