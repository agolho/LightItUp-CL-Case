using System.Collections;
using System.Collections.Generic;
using HyperCasual.PsdkSupport;
using LightItUp.Data;
using LightItUp.Singletons;
using LightItUp.UI;
using UnityEngine;

namespace LightItUp.Game.PowerUps
{
    public enum PowerUpType
    {
        SeekerMissile
    }
    public class PowerUpService : SingletonCreate<PowerUpService>
    {
        public PowerUpConfiguration seekerMissileConfig;
        private SeekerMissilesController _seekerMissilesController;
        
        private GameManager _gameManager;
        private PlayerController _playerController;
        private UI_Game _uiGame;
        
        private int _powerUpUseLimit = 1;
        
        
        public void Start()
        {
            _gameManager = GameManager.Instance;
            _seekerMissilesController = gameObject.AddComponent<SeekerMissilesController>();
        }
        
        public void OnLoadLevel(GameLevel level)
        {
            _playerController = level.player;
            _uiGame = CanvasController.GetPanel<UI_Game>();
            _powerUpUseLimit = seekerMissileConfig.powerUpUseLimit;
            if(_powerUpUseLimit > 0)
                _uiGame.ShowMissileButton();
        }

        private void OnPowerUpUse()
        {
            _powerUpUseLimit--;
            if (_powerUpUseLimit <= 0)
            {
                _uiGame.HideMissileButton();
            }
        }
        
        
        public void ActivateSeekerPowerUp()
        {
            _seekerMissilesController.Init(this, _gameManager ,seekerMissileConfig);
            _seekerMissilesController.LaunchMissiles();
            OnPowerUpUse();
        }
        
    }
}