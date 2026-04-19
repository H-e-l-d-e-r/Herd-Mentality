using UnityEngine;

public class RadioHijackModule : MonoBehaviour
{
    [Header("Composants")]
    public RadioBehaviour Radio;

    [Header("Parametres du Piratage")]
    [Range(0f, 1f)]
    public float SuccessChance = 0.5f; // 50% de chances de reussir
    public float CooldownDuration = 30f; // 30 secondes d'attente
    public float AudimatReward = 20f; // Ce qu'on gagne en cas de succes

    private float m_currentCooldown = 0f;
    public bool IsCooldownActive => m_currentCooldown > 0f;

    void Start()
    {
        NullComponents.ThrowIfNull(Radio);
    }

    void Update()
    {
        // Gestion du chronometre (Cooldown)
        if (m_currentCooldown > 0f)
        {
            m_currentCooldown -= Time.deltaTime;

            if (m_currentCooldown <= 0f)
            {
                m_currentCooldown = 0f;
                Debug.Log("[HIJACK] Le module de piratage est pret !");
            }
        }
    }

    // Fonction a appeler quand le joueur clique sur le bouton "PIRATER"
    public void ExecuteHijack()
    {
        if (IsCooldownActive)
        {
            Debug.LogWarning($"[HIJACK] En surchauffe. Attendez {m_currentCooldown:F1} secondes.");
            return;
        }

        // 1. On verifie ce que la radio capte en ce moment
        RadioBroadcastBehaviour targetBroadcast = Radio.GetTargetedBroadcast();

        if (targetBroadcast == null)
        {
            Debug.Log("[HIJACK] Echec : Aucune emission detectee sur cette frequence. Vous piratez du vide !");
            return;
        }

        Debug.Log($"[HIJACK] Tentative de piratage sur la frequence {targetBroadcast.Freq}...");

        // 2. On lance les des
        float roll = Random.value;

        if (roll <= SuccessChance)
        {
            // SUCCES
            GameManager.Instance.Statistics.GlobalAppreciation += AudimatReward;
            Debug.Log($"<color=green>[HIJACK] REUSSITE ! Le signal est a nous. +{AudimatReward} Auditeurs !</color>");
        }
        else
        {
            // ECHEC
            Debug.Log("<color=red>[HIJACK] ECHEC ! Le pare-feu de la station a bloque la tentative.</color>");
        }

        // 3. On declenche le cooldown
        m_currentCooldown = CooldownDuration;
    }
}