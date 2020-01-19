using UnityEngine;

public class NextTurnClickHandler : MonoBehaviour
{
    BattleManager battleManager;
    void Start()
    {
        battleManager = BattleManager.instance;
    }

    public void OnClick()
    {
        battleManager.EndTurn();
    }
}
