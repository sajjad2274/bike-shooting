StuntMode ReadMe

Please ensure that your Motor Vehicle is on the IgnoreRaycast layer. Also make sure that air time rotation sensitivity is more than 0 in the inspector.

You may perform weight transfer movements in air with the keys W and S where:

	- W/S for leaning in front / back [You may also use the respective arrow keys]


If you find that the snapping to the ground seems very evident, you can increase the height threshold so that the rider has enough distance (and time) from the ground to align himself to make the landing.
Likewise, you can also decrease the ground snap sensitivity to smoothen out the landing. This is a Quaternion lerp function which eases out the movement. Height threshold value triggers this lerp at a specific height.

Try making a perfect landing in the direction of movement. Takes a bit of practice to perform a seamless stunt transition.
Have Fun!