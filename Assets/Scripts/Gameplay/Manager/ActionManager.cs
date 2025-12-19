using System.Collections;
using System.Collections.Generic;
using ShabuStudio.Data;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace ShabuStudio.Gameplay
{
    public class ActionManager : MonoBehaviour
    {
        private ActionBar _actionBar;
        private List<ActionDisplay> _actionSequence = new List<ActionDisplay>();
        private CombatManager _combatManager;
        public PlayButtonController playButton;
        
        public void Initialize(ActionBar actionBar,CombatManager combatManager)
        {
            _actionBar = actionBar;
            _actionSequence = actionBar.displayActions;
            _combatManager = combatManager;
            playButton.gameObject.SetActive(false);
        }

        public async UniTask StartActionSequence()
        {
            var token = this.GetCancellationTokenOnDestroy();
            while (_actionSequence.Count > 0)
            {
                ActionData actionData = _actionSequence[0].ActionData;
                
                bool targetDead = await _combatManager.PlayCard(actionData,token) ;
                await _actionBar.RemoveCardWaitFinish(0,token);

                if (targetDead && actionData.ownerEntity.unitType == ActionOwner.Player) //Remove All Enemy card
                {
                    await _actionBar.RemoveCardSameOwner(ActionOwner.Enemy,token);
                }
                else if (targetDead && actionData.ownerEntity.unitType == ActionOwner.Enemy) //Remove All Player card
                {
                    await _actionBar.RemoveCardSameOwner(ActionOwner.Player,token);
                }
                
                _actionBar.UpdateUI();
            }
        }
        
        
    }
}