Jebediah and Bill both like excitement, but Bob doesn't.  Bob gets scared every time he goes up in a rocket, 
afraid that something will go wrong.  So he modified the command modules in all vessels to automatically 
trigger the abort sequence if something goes wrong.

This mod gives you a reason to put on an escape tower;  while stock has a single escape tower, there is 
wonderful mod called Launch Escape System which adds 4 more towers, for different size vessels

What does it do?  The mod can detect the following events:

* Explosions
* Negative vertical velocity
* High G
* Excessive AoA (angle of attack)

When an event is detected, the abort sequence is triggered (action group).
After an abort, a secondary action group sequence can be triggered

There is a timeout, meaning that the mod will only be active at the beginning of a flight, for the specified 
amount of time.  When the timeout is exceeded, an action group can be triggered (for example, to jettison the escape tower)


3
2
1
Ignition
Liftoff.....

Jebediah, Bill and Bob glanced at each other as the engines roared.  The pressure mounted as the rocket 
lifted off the pad.
"Tower cleared", came the call from Mission Control.  The rocket rotated a bit.
"Roll program complete".
"All systems go"
The engines continued to fire.  All of a sudden, an alarm started sounding.  
Jebediah ignored it, kept looking out the window.  Bill wasn't much better, but Bob was concerned enough to speak up.
"Jebediah, that alarm is going off, don't you think you should abort?"
Jebediah replied "No, those alarms always go off"
"But, the last time THAT alarm went off, the rocket exploded"
Jeb said "That was last time, this time......."
That was as far as he got, because:

KAAABOOOMMMMMMM!!!!!

as the rocket exploded!

Later, in the revival chamber, as they were being reconstituted (again), Bob decided that he wasn't going 
to do that again.  Next time, he decided, he wasn't going to leave it in Jebediah's hands.

A few weeks later, the next launch.  Same rocket, they were assured by the technicians  that they had fixed the problem.
As usual, the two daredevils, Jebediah & Bill believed them without question, but Bob wasn't so sure and insisted that they 
install his new invention without telling .  He knew that Jebediah would throw a fit if he knew about it.

3, 2, 1, Ignition and Liftoff again

Things went fine for the first few minutes, and then another, different alarm started to sound.  This time, 
however, as soon as it sounded, the abort system was automatically triggered.  The capsule was blasted free 
of the exploding rocket by the LES (Launch Escape System).  A few seconds after it burned out, the LES was 
jettisoned, and then the parachutes deployed.  This time, they were gently lowered to the ground, no need 
for the revival chamber this time.  Bob turned to Jeb, smiled and said "That was my new invention, Bob's Panic Box'.  
We can now live better lives not having to be reconstituted every time something goes wrong."

------------------------------------

This mod gives you a reason to put on an escape tower;  while stock has a single escape tower, there is wonderful mod called Launch Escape System which adds 4 more towers, for different size vessels

What does it do?  The mod can detect the following events:

Explosions
Negative vertical velocity
High G
Excessive AoA (angle of attack)
When an event is detected, the abort sequence is triggered (action group).
After an abort, a secondary action group sequence can be triggered

There is a timeout, meaning that the mod will only be active at the beginning of a flight, for the specified amount of time.  When the timeout is exceeded, an action group can be triggered (for example, to jettison the escape tower).

Usage

Settings page
This is where you can set default options. The page has three columns
First column is the initial setting for the buildings and if you go directly to the launchpad/runway.
Second column are the initial settings which are set, currently the settings are shared between all the environments.
Third column is how to access the mod.
  Reveal hidden contents
  https://i.imgur.com/wVen1U4.png


Toolbar Button
https://i.imgur.com/MuxtQCi.png

Editor/Flight Configuration Window
  Reveal hidden contents
  https://i.imgur.com/6PMZ4ri.png

Kottabos Review
https://youtu.be/8FCgmCCCCJY

Usage

You can configure it to use either a toolbar button, a button on the PAW (Part Action Window), or both.    Clicking them will bring up the Editor/Flight Configuration Window

Most of the options are self explanatory

Arm Explosion Detection will detect any explosions on the vessel, NOT on any parts which were jettisoned.
Neg-Vel-Limit will detected if the vessel is going down at that speed
High-G-Limit will trigger if the G forces exceed the specified number
Max AoA will trigger if the Angle of Attack exceeds the specified value, useful to detect flips, etc.
Disable after flight time will disable the mod after the specified amount of time.  You can specify the time either in the text box or by a slider
Trigger action after timeout will activate the specified action group after the mod is disabled due to timing out
Trigger action after abort will activate the specified action group after an abort.  (see the next line)
The post-abort delay is the amount of time to wait after an abort before doing the Trigger action after abort
The disable BPB﻿ after altitude﻿﻿, will disable the mod after the specified altitude is reached 
When in flight, and additional button is shown, depending on whether an abort has been triggered or not

The  Apply button is shown when things are normal  It will only be active if there are changes to be saved.

If an abort has been triggered, the Apply button is replaced with either Acknowledge , which acknowledges an abort is in progress, or Disable Abort, to disable the abort sequence.  Disable Abort is shown after an abort is acknowledged

Additional Functionality

Toolbar button(s) flashes when abort is active on active vessel
Click the button to acknowledge the abort, button will stay red
Click a second time to cancel the post-abort sequence, button will go back to normal
Abort sequence now disables if no control source
Dependencies

Click Through Blocker
ToolbarController
Recommended

Launch Escape System
Availability

Source:https://github.com/linuxgurugamer/BobsPanicBox/
Download: https://github.com/linuxgurugamer/BobsPanicBox/releases
License: GPLv3
Now available in CKAN

