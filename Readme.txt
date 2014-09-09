Movement Experiments v0.4

Miniexperiment to test different movement systems to reduce simulator sickness.


Keyboard controls:

	Space: Recenter view
	1: Standard movement mode
	2: Canvas movement mode
	3: Third person movement mode
	4: Stroboscopic movement mode


Canvas mode:
	When you move or turn, a canvas that is anchored to your avatar's body is partly faded in. The idea is that this makes the VR world look like it is projected onto a screen and the screen provides a frame of reference: when you turn, the world doesn't turn around you, but only its projection on the screen turns. The screen is still relative to your avatar body, so your visual and vestibular system don't get conflicting input.
	Press G to cycle through different canvas textures.

Third person mode:
	To move, keep the right mouse button pressed. The environment will be shown in an ambient occlusion rendering style. With the right mouse button pressed, use WASD to move your avatar and the mouse to turn it. Your viewpoint itself doesn't move, but you can keep looking around with your head. To teleport your view into the avatar's new position, release the right mouse button.

Stroboscopic:
	Stroboscopic view when you move or turn. It strobes by showing x frames in a row, then showing darkness for y frames in a row. To decrease/increase x (shown frames), press 7 and 8. To decrease/increase y (black frames), press 9 and 0.


I get much less simulator sickness in both the Canvas and the 3rd person modes. The canvas mode is a bit more immersive, but the 3rd person mode lets me assume the avatar's identity more. Stroboscopic view doesn't work for me, maybe because it doesn't darken the entire screen all at once for some reason but only partly (vsync issue, timewarp or something?).


Credits:

Mansion: http://oo-fil-oo.deviantart.com/art/MANSION-MAIN-HALL-458053364
Avatar: http://www.deviantart.com/art/Darla-Sparda-345882769
Music: Kevin McLeod http://incompetech.com/music/royalty-free/index.html?isrc=USUAN1100689
Player's footsteps: cris http://freesound.org/people/cris/sounds/167686/



Changelog:

0.4: Added different canvas textures to cycle through. 3rd person mode starts a bit behind the avatar.
0.3: New Oculus SDK 0.4.2
0.2: Added stroboscopic mode.