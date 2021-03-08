echoLogin PostFX Studio - ReadMe


================================================================================
Getting Started
================================================================================


Watch tutorial videos for PostFX Studio
https://www.youtube.com/user/EchoLoginLLC


Demo Project is located in EchoLogin/PFX/SampleProjects/Demo


1. Drag the _EchoPFXManager into the scene (located in echoLogin/PFX/Prefab).
2. Click on the _echoPFXManager.




Editable Values


Manager Depth:
This value must be higher than the depth of any cameras that will have PostFX.
Any camera over this depth value will not use PostFX at all.


Frame rate:
Set this to your desired frame rate. No need to call Application.targetFrameRate in your script.
This is also used when setting auto detail mode to maintain a set frame-rate.


Enable PlayMaker Actions:
Toggle this on when using PlayMaker - it will add the PFX Actions under "echoLogin PFX" 
in the PlayMaker Action browser.


Export:
Saves all settings to an XML file for backup or to share your effects with others.


Import:
Loads a Saved XML file. This will replace all current settings with the backed up settings from a previous export.




Render Groups:
A Render Group is a group of cameras used for Post FX. You can use any amount of Render Groups, but using more will slightly impact the frame rate each time.


Add Render Group:
This button will add a new Render Group. Pressing the "-" button will remove it.


Post Effects:
Post Effects are built from effect options under the Build and Make tabs. See following section for more details.


========================================================================
Group Tab
========================================================================


This is where you can set the options for the selected Render Group.


Screen Mesh Quads: This is how many quads that will make up screen for Distortion and Shockwave effects.
If you're not using those options set this to the lowest number (2 is lowest).


Pass 1-
Blend Mode: blend mode for this pass, will automatically be disabled for the first Render Group.
FilterMode: Point, Bilinear or Trilinear


Pass 2-
Blend Mode: blend mode for this pass, will automatically be disabled for the first Render Group.
FilterMode: Point, Bilinear or Trilinear


Render Texture Size  ( Unity Pro Only ):
DEVICE_SIZE         = RenderTexture will be the size of the Device’s Screen.
DIVIDE                        = Divides Device Screen size by value.
AUTO_DETAIL        = Maintains 60 fps on slower machines by changing detail per frame.
CUSTOM                        = Set the ScreenSize to a custom value.








================================================================================
Build Tab
================================================================================
Under the Build tab are the global shader options to be included for the selected Render Group.


There is a maximum of 6 options per pass.
After choosing your effect options press compile.
Effect Options with more settings will appear as a button that will expose the editable parameters of the effect.


================================================================================
Make Tab
================================================================================
Use the Add Effect Option drop-down menu to select which shader options to use and expose the settings for that option.


Fade In:        This is how long it takes (in seconds) for the effect to fade in.
Sustain:          This is the time the effect will remain on. Enter -1 if you want effect to stay on this stage. To stop it make a script.
Fade out:         The time it takes for the effect to fade out.
Start Delay:         Time (in seconds) before this effect option will start.




Fade Options:


Min:   The minimum value for fading in effect  option  (0 = invisible).
Max: The Maximum value for fading an effect option.
Current:   Current fade value.




================================================================================
Scripting
================================================================================


There are just 3 methods in echoPFX class to remember.


Start()
Stop()
ShockWaveCenter()


Example


private EchoPFX _fx1;


//=============
void Start()
{
        // always do this on startup.
        _fx1 = new EchoPFX ( "RenderGroup:0", "MyEffect");
}


// normal effect
//=============
public void TriggerPostFX1()
{
        _fx1.Start();  // can also called  _fx1.Start( timescalevalue );
}


// if the fx was a shockwave 
//=============
public void TriggerPostFXShockwave( float ixper, float iyper )
{
        _fx1.ShockWaveCenter ( 0, ixper, iyper );


        _fx1.Start();  // can also called  _fx1.Start( timescalevalue );
}


// to stop an effect call Stop,  this is only needed for effects with a -1 sustain ( infinite )
//=============
public void StopPostFX1()
{
        _fx1.Stop();  // you can also call Stop fx1.Stop ( fadeouttime );
}


any questions email core@echologin.com, include your invoice number in subject.