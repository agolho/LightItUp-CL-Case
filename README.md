# LightItUpQubic

## Added feature *Seeker Missiles*
- Missile feature can be configured from {Assets/_Game/Resources/SeekerMissileData.asset}
- All scripts added can be found in {Assets/_Game/Scripts/Game/PowerUps}
 - Missiles count indicates how many missiles will be fired on each button press
 - Missile speed can be lowered to build intensity and excitement
 - Power Up use limit indicates how many times this feature can be called on each level. Set to 0 to disable Seeker Missiles
 - Avoid Obstacles is a feature that enables missiles to avoid Lit and Unlit blocks. Uses raycasts and can be costly. Turn it on and off to see effect.
     - Avoid Obstacles can cause missiles to get stuck in edge cases. This can be improved in the future.

## Added Scripts
- PowerUpConfiguration.cs
  - Configuration for SeekerMissile power up
- PowerUpService.cs
  - Configuration for SeekerMissile power up
  - Initialization of SeekerMissileController
- SeekerMissileController.cs
   - Controller for SeekerMissile
   - Targeting and firing missiles
- SeekerMissile.cs
  - SeekerMissile class
  - Movement and collision detection

## Updated Scripts
- ObjectPool.cs
  - Lines 37, 91-106
  - Added SeekerMissile prefab to the pool
- BlockController.cs
  - Lines 1491-1501
  - Methods needed to check for standard blocks
- CameraFocus.cs
  - Lines 145, 384-420
  - Methods for adding and removing missile focus targets
  - Updating camera rect to focus on missiles
- GameManager.cs
  - Line 251 
  - Initialized PowerUpService on new level load
- UI_Game.cs
  - Lines 186-201 
  - Added SeekerMissile button
  - Methods to show/hide the button

## Contact
- Mail: agolho@gmail.com
- LinkedIn: https://www.linkedin.com/in/yusufbektas/
