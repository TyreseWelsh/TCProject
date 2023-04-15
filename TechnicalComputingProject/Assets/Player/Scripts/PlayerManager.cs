using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerManager : MonoBehaviour, IDamageable
{
    int playerHealth;
    [SerializeField] TMP_Text healthText;
    [SerializeField] TMP_Text soulsText;
    int playerSouls = 1000;
    int turretCost = 100;

    public enum TurretTypes
    {
        Basic,
        Instant,
        Slow
    }
    public TurretTypes currentTurretType = TurretTypes.Basic;

    private void Start()
    {
        playerHealth = 20;
        healthText.text = "Health: " + playerHealth;

        soulsText.text = "Souls: " + playerSouls;
    }

    public void SetTurretType(int _turretType)
    {
        currentTurretType = (TurretTypes)_turretType;
        switch(currentTurretType)
        {
            case (TurretTypes.Basic):
                turretCost = 100;
                break;
            case (TurretTypes.Instant):
                turretCost = 200;
                break;
            case (TurretTypes.Slow):
                turretCost = 80;
                break;
        }
    }

    public TurretTypes GetTurretType()
    {
        return currentTurretType;
    }

    public void SetSouls(int newSouls)
    {
        playerSouls += newSouls;
        soulsText.text = "Souls: " + playerSouls;
    }

    public int GetSouls()
    {
        return playerSouls;
    }

    public int GetTurretCost()
    {
        return turretCost;
    }

    public void TakeDamage(int damage)
    {
        playerHealth -= damage;
        healthText.text = "Health: " + playerHealth;
    }
}
