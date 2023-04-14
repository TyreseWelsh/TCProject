using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerManager : MonoBehaviour, IDamageable
{
    int playerHealth;
    [SerializeField] TMP_Text healthText;

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
    }

    public void SetTurretType(int _turretType)
    {
        currentTurretType = (TurretTypes)_turretType;
    }

    public TurretTypes GetTurretType()
    {
        return currentTurretType;
    }

    public void TakeDamage(int damage)
    {
        playerHealth -= damage;
        healthText.text = "Health: " + playerHealth;
    }
}
