using System;
using DialogueSystem;
using UnityEngine;

public class DialogueTutorial : MonoBehaviour
{
    public DialogueTable DialogueTableTut;
    public DialogueTable Diag_Lei_2;
    public DialogueTable Diag_Lei_3;
    public DialogueTable Diag_Lei_4;
    public DialogueTable Diag_Lei_5;
    public DialogueTable Diag_Lei_6;

    public DialogueTypewritter TypeWritter;
    public RadioDecrypter Decrypter;

    [Header("Paramètres Fréquence et Tolérance")]
    public float FreqTuto = 12700f;
    public float FreqTutoEnd = 6500f;
    public float Bandwidth = 500f;
    [Range(0f, 1f)]
    public float SignalThreshold = 0.7f;
    public float AngleThreshold = 5f;

    [Header("Angles Cibles")]
    public float AngleTuto1 = 30f;
    public float AngleTuto2 = 60f;
    public float AngleTuto3 = -60f;

    [Header("Broadcasts Audio à débloquer")]
    public RadioBroadcastBehaviour Broadcast30;
    public RadioBroadcastBehaviour Broadcast60;
    public RadioBroadcastBehaviour BroadcastMinus60;
    public RadioBroadcastBehaviour Broadcast6500;

    private DialoguePtr Lei_Diag;
    private DialoguePtr Lei_Diag2;
    private DialoguePtr Lei_Diag3;
    private DialoguePtr Lei_Diag4;
    private DialoguePtr Lei_Diag5;
    private DialoguePtr Lei_Diag6;

    private int m_currentStep = 0;
    private bool m_isSnapping = false;

    // Pour sauvegarder le vrai volume
    private float m_vol30;
    private float m_vol60;
    private float m_volMinus60;
    private float m_vol6500;

    void Start()
    {
        Lei_Diag = Dialogue.RegisterDialogue(DialogueTableTut);
        Lei_Diag2 = Dialogue.RegisterDialogue(Diag_Lei_2);
        Lei_Diag3 = Dialogue.RegisterDialogue(Diag_Lei_3);
        Lei_Diag4 = Dialogue.RegisterDialogue(Diag_Lei_4);
        Lei_Diag5 = Dialogue.RegisterDialogue(Diag_Lei_5);
        Lei_Diag6 = Dialogue.RegisterDialogue(Diag_Lei_6);

        // On sauvegarde le vrai volume et on force le volume à 0 au démarrage
        if (Broadcast30 != null) { m_vol30 = Broadcast30.Volume; Broadcast30.Volume = 0f; }
        if (Broadcast60 != null) { m_vol60 = Broadcast60.Volume; Broadcast60.Volume = 0f; }
        if (BroadcastMinus60 != null) { m_volMinus60 = BroadcastMinus60.Volume; BroadcastMinus60.Volume = 0f; }
        if (Broadcast6500 != null) { m_vol6500 = Broadcast6500.Volume; Broadcast6500.Volume = 0f; }

        Dialogue.Instance.OnDialogueCloseEvent += OnDialogueClosed;
        if (Decrypter != null) Decrypter.OnDecodeSuccess += OnDecodeSuccess;

        if (m_currentStep == 0)
        {
            Dialogue.PlayDialogue(Lei_Diag);
            m_currentStep = 1;
        }

        RadioManager.Instance.RadioBehaviour.FreqKnob.OnValueChange.AddListener(OnFreqChanged);
        RadioManager.Instance.RadioBehaviour.Antenna.OnValueChange.AddListener(OnAngleChanged);
    }

    void OnDestroy()
    {
        if (Dialogue.Instance != null) Dialogue.Instance.OnDialogueCloseEvent -= OnDialogueClosed;
        if (Decrypter != null) Decrypter.OnDecodeSuccess -= OnDecodeSuccess;
    }

    void OnFreqChanged(float _)
    {
        if (m_isSnapping) return;

        // Hard Lock : Bloqué sur 12700 pendant la recherche de l'antenne
        if (m_currentStep == 2)
        {
            DefendFreq(FreqTuto); // Utilisation du verrouillage SILENCIEUX
            return;
        }

        // Hard Lock : Bloqué sur 6500 pendant le tout dernier dialogue
        if (m_currentStep == 9)
        {
            DefendFreq(FreqTutoEnd); // Utilisation du verrouillage SILENCIEUX
            return;
        }

        VerifyProgress();
    }

    void OnAngleChanged(float _)
    {
        if (m_isSnapping) return;
        VerifyProgress();
    }

    void VerifyProgress()
    {
        if (TypeWritter.HasCommand || Dialogue.Instance.HasCommand) return;

        float currentFreq = RadioManager.Instance.RadioBehaviour.FreqKnob.Value;
        float currentAngle = RadioManager.Instance.RadioBehaviour.Antenna.Value;

        // ETAPE 1 : Trouver la fréquence (12700)
        if (m_currentStep == 1)
        {
            if (IsFreqOk(currentFreq, FreqTuto))
            {
                SnapFreq(FreqTuto);
                LockKnobInteraction(true);

                Dialogue.PlayDialogue(Lei_Diag2);
                m_currentStep = 2;
            }
        }
        // ETAPE 2 : Trouver 30° -> Dialogue 3 immédiat
        else if (m_currentStep == 2)
        {
            if (Broadcast30 != null && Broadcast30.Volume > 0f && IsFreqOk(currentFreq, FreqTuto) && IsAngleOk(currentAngle, AngleTuto1))
            {
                SnapAngle(AngleTuto1);
                LockKnobInteraction(false);

                Dialogue.PlayDialogue(Lei_Diag3);
                m_currentStep = 3;
            }
        }
        // ETAPE 4 : Trouver 60° -> Dialogue 4 immédiat
        else if (m_currentStep == 4)
        {
            if (Broadcast60 != null && Broadcast60.Volume > 0f && IsFreqOk(currentFreq, FreqTuto) && IsAngleOk(currentAngle, AngleTuto2))
            {
                SnapAngle(AngleTuto2);
                Dialogue.PlayDialogue(Lei_Diag4);
                m_currentStep = 5;
            }
        }
        // ETAPE 6 : Trouver -60° -> Dialogue 5 immédiat
        else if (m_currentStep == 6)
        {
            if (BroadcastMinus60 != null && BroadcastMinus60.Volume > 0f && IsFreqOk(currentFreq, FreqTuto) && IsAngleOk(currentAngle, AngleTuto3))
            {
                SnapAngle(AngleTuto3);
                Dialogue.PlayDialogue(Lei_Diag5);
                m_currentStep = 7;
            }
        }
        // ETAPE 8 : Trouver la fréquence finale (6500) -> Dialogue 6 immédiat
        else if (m_currentStep == 8)
        {
            if (Broadcast6500 != null && Broadcast6500.Volume > 0f && IsFreqOk(currentFreq, FreqTutoEnd))
            {
                SnapFreq(FreqTutoEnd);
                LockKnobInteraction(true);

                Dialogue.PlayDialogue(Lei_Diag6);
                m_currentStep = 9;
            }
        }
    }

    // --- REPONSE AU DECODAGE ---
    private void OnDecodeSuccess()
    {
        if (m_currentStep == 3)
        {
            if (Broadcast60 != null) Broadcast60.Volume = m_vol60;
            RefreshRadioSignal();
            m_currentStep = 4;
        }
        else if (m_currentStep == 5)
        {
            if (BroadcastMinus60 != null) BroadcastMinus60.Volume = m_volMinus60;
            RefreshRadioSignal();
            m_currentStep = 6;
        }
        else if (m_currentStep == 7)
        {
            if (Broadcast6500 != null) Broadcast6500.Volume = m_vol6500;
            RefreshRadioSignal();
            m_currentStep = 8;
        }
    }

    // --- FIN DE DIALOGUE ---
    private void OnDialogueClosed()
    {
        if (m_currentStep == 1)
        {
            if (Broadcast30 != null) Broadcast30.Volume = m_vol30;
            RefreshRadioSignal();
        }
        else if (m_currentStep == 9)
        {
            LockKnobInteraction(false);
            m_currentStep = 10; // Libération totale du joueur
        }
    }

    // --- VERROUILLAGE PHYSIQUE UI ---
    private void LockKnobInteraction(bool isLocked)
    {
        /*CanvasGroup cg = RadioManager.Instance.RadioBehaviour.FreqKnob.gameObject.GetComponent<CanvasGroup>();
        if (cg == null)
        {
            cg = RadioManager.Instance.RadioBehaviour.FreqKnob.gameObject.AddComponent<CanvasGroup>();
        }
        cg.blocksRaycasts = !isLocked;
        cg.interactable = !isLocked; // Stoppe net le clic/drag en cours !*/
    }

    private void RefreshRadioSignal()
    {
        m_isSnapping = true;
        float currentFreq = RadioManager.Instance.RadioBehaviour.Frequence;
        RadioManager.Instance.RadioBehaviour.FreqKnob.OnValueChange?.Invoke(currentFreq);
        m_isSnapping = false;
    }

    private bool IsFreqOk(float freq, float target)
    {
        float delta = Mathf.Abs(freq - target);
        float signal = Mathf.Exp(-Mathf.Pow(delta / (Bandwidth / 2f), 2));
        return signal >= SignalThreshold;
    }

    private bool IsAngleOk(float angle, float target)
    {
        return Mathf.Abs(angle - target) <= AngleThreshold;
    }

    // --- SNAPS ---
    private void SnapFreq(float target)
    {
        /*m_isSnapping = true;
        RadioManager.Instance.RadioBehaviour.Frequence = target;
        RadioManager.Instance.RadioBehaviour.FreqKnob.SetValue(target);

        // PING AUDIO (Utilisé SEULEMENT quand on trouve la fréquence la première fois)
        RadioManager.Instance.RadioBehaviour.FreqKnob.OnValueChange?.Invoke(target);

        m_isSnapping = false;*/
    }

    // --- NOUVEAU : DEFENSE SILENCIEUSE POUR LE HARD LOCK ---
    private void DefendFreq(float target)
    {
        /*m_isSnapping = true;
        RadioManager.Instance.RadioBehaviour.Frequence = target;
        RadioManager.Instance.RadioBehaviour.FreqKnob.SetValue(target);

        // AUCUN PING AUDIO ICI ! On repousse juste la molette silencieusement sans couper le son.

        m_isSnapping = false;*/
    }

    private void SnapAngle(float target)
    {
        /*m_isSnapping = true;
        RadioManager.Instance.RadioBehaviour.Orientation = target;
        RadioManager.Instance.RadioBehaviour.Antenna.SetValue(target);
        RadioManager.Instance.RadioBehaviour.Antenna.OnValueChange?.Invoke(target);
        m_isSnapping = false;*/
    }
}