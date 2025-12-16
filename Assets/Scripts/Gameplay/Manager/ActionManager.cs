using System.Collections;
using System.Collections.Generic;
using ShabuStudio.Data;
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

        public IEnumerator StartActionSequence()
        {
            while (_actionSequence.Count > 0)
            {
                ActionData actionData = _actionSequence[0].ActionData;
                bool targetDead = false;
                yield return StartCoroutine(_combatManager.PlayCard(actionData,(resultIsDead) => targetDead = resultIsDead)) ;
                yield return StartCoroutine(_actionBar.RemoveCardWaitFinish(0));

                if (targetDead && actionData.ownerEntity.unitType == ActionOwner.Player) //Remove All Enemy card
                {
                    yield return StartCoroutine(_actionBar.RemoveCardSameOwner(ActionOwner.Enemy));
                }
                else if (targetDead && actionData.ownerEntity.unitType == ActionOwner.Enemy) //Remove All Player card
                {
                    yield return StartCoroutine(_actionBar.RemoveCardSameOwner(ActionOwner.Player));
                }
                
                _actionBar.UpdateUI();
            }
        }
        
        
    }
}