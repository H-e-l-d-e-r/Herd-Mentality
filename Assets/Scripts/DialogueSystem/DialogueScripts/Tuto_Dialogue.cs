using DialogueSystem;
using UnityEngine;

public class DialogueTutorial : MonoBehaviour
{
    public DialogueTable DialogueTableTut;
    public DialogueTable Diag_Lei_2;
    public DialogueTable Diag_Lei_3;
    public DialogueTypewritter TypeWritter;

    [Header("Diag 2 - Fréquence")]
    public float FreqTuto;
    public float Bandwidth = 500f;
    [Range(0f, 1f)]
    public float SignalThreshold = 0.7f;

    [Header("Diag 3 - Antenne")]
    public float AngleTuto = 30f;
    public float AngleThreshold = 5f; // ± degrés de tolérance

    private DialoguePtr Lei_Diag;
    private DialoguePtr Lei_Diag2;
    private DialoguePtr Lei_Diag3;

    private bool m_diag1Played = false;
    private bool m_diag2Played = false;
    private bool m_diag3Played = false;

    void Start()
    {
        Lei_Diag = Dialogue.RegisterDialogue(DialogueTableTut);
        Lei_Diag2 = Dialogue.RegisterDialogue(Diag_Lei_2);
        Lei_Diag3 = Dialogue.RegisterDialogue(Diag_Lei_3);

        if (!m_diag1Played)
        {
            Dialogue.PlayDialogue(Lei_Diag);
            m_diag1Played = true;
        }

        RadioManager.Instance.RadioBehaviour.FreqKnob.OnValueChange.AddListener(SuiteTuto);
        RadioManager.Instance.RadioBehaviour.Antenna.OnValueChange.AddListener(SuiteTuto3);
    }

    void SuiteTuto(float frequence)
    {
        if (m_diag2Played) return;

        float delta = Mathf.Abs(frequence - FreqTuto);
        float signal = Mathf.Exp(-Mathf.Pow(delta / (Bandwidth / 2f), 2));

        if (signal >= SignalThreshold)
        {
            Dialogue.PlayDialogue(Lei_Diag2);
            m_diag2Played = true;
        }
    }

    void SuiteTuto3(float angle)
    {
        if (m_diag3Played) return;
        if (!m_diag2Played) return;
        if (TypeWritter.HasCommand) return;
        if (Dialogue.Instance.HasCommand) return;

        // Vérifie la fréquence ET l'angle en même temps
        float currentFreq = RadioManager.Instance.RadioBehaviour.FreqKnob.Value;
        float deltaFreq = Mathf.Abs(currentFreq - FreqTuto);
        float signal = Mathf.Exp(-Mathf.Pow(deltaFreq / (Bandwidth / 2f), 2));

        bool freqOk = signal >= SignalThreshold;
        bool angleOk = Mathf.Abs(angle - AngleTuto) <= AngleThreshold;

        if (freqOk && angleOk)
        {
            Dialogue.PlayDialogue(Lei_Diag3);
            m_diag3Played = true;
        }
    }
}