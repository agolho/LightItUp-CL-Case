# LightItUpQubic

## Added Feature: *Seeker Missiles*
- The missile feature can be configured from `{Assets/_Game/Resources/SeekerMissileData.asset}`.
- All added scripts can be found in `{Assets/_Game/Scripts/Game/PowerUps}`.
  - **Missiles Count:** Indicates how many missiles will be fired on each button press.
  - **Missile Speed:** Can be adjusted to build intensity and excitement.
  - **Power-Up Use Limit:** Indicates how many times this feature can be used in each level. Set to 0 to disable Seeker Missiles.
  - **Avoid Obstacles:** Enables missiles to avoid lit and unlit blocks using raycasts. This can be costly, so toggle it on and off to see the effect.
    - Note: Avoid Obstacles can cause missiles to get stuck in edge cases. This can be improved in the future.

## Added Scripts
- **PowerUpConfiguration.cs**
  - Configuration for the SeekerMissile power-up.
- **PowerUpService.cs**
  - Configuration for the SeekerMissile power-up.
  - Initialization of the SeekerMissileController.
- **SeekerMissileController.cs**
  - Controller for SeekerMissile.
  - Handles targeting and firing missiles.
- **SeekerMissile.cs**
  - SeekerMissile class.
  - Handles movement and collision detection.

## Updated Scripts
- **ObjectPool.cs**
  - Lines 37, 91-106: Added SeekerMissile prefab to the pool.
- **BlockController.cs**
  - Lines 1491-1501: Added methods needed to check for standard blocks.
- **CameraFocus.cs**
  - Lines 145, 384-420: Added methods for adding and removing missile focus targets.
  - Updated camera rect to focus on missiles.
- **GameManager.cs**
  - Line 251: Initialized PowerUpService on new level load.
- **UI_Game.cs**
  - Lines 186-201: Added SeekerMissile button.
  - Added methods to show/hide the button.

## Contact
- Email: [agolho@gmail.com](mailto:agolho@gmail.com)
- LinkedIn: [Yusuf Bektas](https://www.linkedin.com/in/yusufbektas/)
