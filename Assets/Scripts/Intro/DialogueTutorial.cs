using System;
using DialogueSystem;
using UnityEngine;

public class DialogueTutorial : MonoBehaviour
{
    public bool DisableMendatoryTuto;

    [Header("Dialogues Existants")]
    public DialogueTable DialogueTableTut;
    public DialogueTable Diag_Lei_2;
    public DialogueTable Diag_Lei_3;
    public DialogueTable Diag_Lei_4;
    public DialogueTable Diag_Lei_5;
    public DialogueTable Diag_Lei_6;

    [Header("Nouveaux Dialogues (7 à 13)")]
    public DialogueTable Diag_Lei_7;
    public DialogueTable Diag_Lei_8;
    public DialogueTable Diag_Lei_9;
    public DialogueTable Diag_Lei_10;
    public DialogueTable Diag_Lei_11;
    public DialogueTable Diag_Lei_12;
    public DialogueTable Diag_Lei_13;

    public DialogueTypewritter TypeWritter;
    public RadioDecrypter Decrypter;

    [Header("Paramètres Fréquence et Tolérance")]
    public float FreqTuto = 12700f;
    public float FreqTutoEnd = 7500f;
    public float Bandwidth = 500f;
    [Range(0f, 1f)]
    public float SignalThreshold = 0.7f;
    public float AngleThreshold = 5f;
    public float FristAngleThreshold = 25f;

    [Header("Angles Cibles (Tuto de base)")]
    public float AngleTuto0 = 00f;
    public float AngleTuto1 = 30f;
    public float AngleTuto2 = 60f;
    public float AngleTuto3 = -60f;

    [Header("Cibles Décodage (Dialogues Évolutifs)")]
    public float TargetFreqDiag7 = 5100f;
    public float TargetFreqDiag9 = 4100f;
    public float TargetFreqDiag11 = 6700f;
    // public float TargetAngleDiag9 = 20f;
    // public float TargetAngleDiag11 = 45f;
    // public float TargetAngleDiag13 = -60f;

    public float TargetFreqDiag8 = 11000f;
    public float TargetAngleDiag8 = 0f;
    public DecryptionModes ModeRequiredForDiag8; // À configurer sur "FromAudio" dans l'Inspector

    public float TargetFreqDiag10 = 11400f;
    public float TargetAngleDiag10 = 12f;
    public DecryptionModes ModeRequiredForDiag10; // À configurer sur "FromCaesar" dans l'Inspector

    public float TargetFreqDiag12 = 8800f;
    public float TargetAngleDiag12 = -60f;
    public DecryptionModes ModeRequiredForDiag12; // À configurer sur "FromCaesar" dans l'Inspector

    public float FinalFreq = 3900;
    public float FinalAngle = -52;

    [Header("Modes de Décryptage Attendus (Tuto)")]
    public DecryptionModes ModeRequiredFor30;
    public DecryptionModes ModeRequiredFor60;
    public DecryptionModes ModeRequiredForMinus60;

    [Header("Broadcasts Audio à débloquer")]
    public RadioBroadcastBehaviour Broadcast30;
    public RadioBroadcastBehaviour Broadcast60;
    public RadioBroadcastBehaviour BroadcastMinus60;
    public RadioBroadcastBehaviour Broadcast7500;

    private DialoguePtr Lei_Diag;
    private DialoguePtr Lei_Diag2;
    private DialoguePtr Lei_Diag3;
    private DialoguePtr Lei_Diag4;
    private DialoguePtr Lei_Diag5;
    private DialoguePtr Lei_Diag6;

    private DialoguePtr Lei_Diag7;
    private DialoguePtr Lei_Diag8;
    private DialoguePtr Lei_Diag9;
    private DialoguePtr Lei_Diag10;
    private DialoguePtr Lei_Diag11;
    private DialoguePtr Lei_Diag12;
    private DialoguePtr Lei_Diag13;

    [SerializeField]
    private int m_currentStep = 0;
    private bool m_isSnapping = false;

    private float m_vol30;
    private float m_vol60;
    private float m_volMinus60;
    private float m_vol7500;

    void Start()
    {
        Lei_Diag = Dialogue.RegisterDialogue(DialogueTableTut);
        Lei_Diag2 = Dialogue.RegisterDialogue(Diag_Lei_2);
        Lei_Diag3 = Dialogue.RegisterDialogue(Diag_Lei_3);
        Lei_Diag4 = Dialogue.RegisterDialogue(Diag_Lei_4);
        Lei_Diag5 = Dialogue.RegisterDialogue(Diag_Lei_5);
        Lei_Diag6 = Dialogue.RegisterDialogue(Diag_Lei_6);

        Lei_Diag7 = Dialogue.RegisterDialogue(Diag_Lei_7);
        Lei_Diag8 = Dialogue.RegisterDialogue(Diag_Lei_8);
        Lei_Diag9 = Dialogue.RegisterDialogue(Diag_Lei_9);
        Lei_Diag10 = Dialogue.RegisterDialogue(Diag_Lei_10);
        Lei_Diag11 = Dialogue.RegisterDialogue(Diag_Lei_11);
        Lei_Diag12 = Dialogue.RegisterDialogue(Diag_Lei_12);
        Lei_Diag13 = Dialogue.RegisterDialogue(Diag_Lei_13);

        if (Broadcast30 != null) { m_vol30 = Broadcast30.Volume; Broadcast30.Volume = 0f; }
        if (Broadcast60 != null) { m_vol60 = Broadcast60.Volume; Broadcast60.Volume = 0f; }
        if (BroadcastMinus60 != null) { m_volMinus60 = BroadcastMinus60.Volume; BroadcastMinus60.Volume = 0f; }
        if (Broadcast7500 != null) { m_vol7500 = Broadcast7500.Volume; Broadcast7500.Volume = 0f; }

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

        // Désactive la vérification par molette dès que le tuto est fini (les étapes suivantes utilisent OnDecodeSuccess)
        //if (m_currentStep == 2 || m_currentStep >= 9) return;

        VerifyProgress();
    }

    void OnAngleChanged(float _)
    {
        if (m_isSnapping) return;

        VerifyProgress();
    }

    // Uniquement pour le tutoriel de base (Fréquences et Angles simples)
    void VerifyProgress()
    {
        if (TypeWritter.HasCommand || Dialogue.Instance.HasCommand) return;

        float currentFreq = RadioManager.Instance.RadioBehaviour.FreqKnob.Value;
        float currentAngle = RadioManager.Instance.RadioBehaviour.Antenna.Value;

        if (m_currentStep == 1)
        {
            if (IsFreqOk(currentFreq, FreqTuto) && IsAngleOk(currentAngle, AngleTuto0, FristAngleThreshold))
            {
                Dialogue.PlayDialogue(Lei_Diag2);
                m_currentStep = 2;
            } 
        }
        else if (m_currentStep == 2)
        {
            if (IsFreqOk(currentFreq, FreqTuto) && IsAngleOk(currentAngle, AngleTuto1))
            {
                Dialogue.PlayDialogue(Lei_Diag3);
                m_currentStep = 3;
            }
        }
        else if (m_currentStep == 4)
        {
            if (IsFreqOk(currentFreq, FreqTuto) && IsAngleOk(currentAngle, AngleTuto2))
            {
                Dialogue.PlayDialogue(Lei_Diag4);
                m_currentStep = 5;
            } 
        }
        else if (m_currentStep == 6)
        {
            if (IsFreqOk(currentFreq, FreqTuto) && IsAngleOk(currentAngle, AngleTuto3))
            {
                Dialogue.PlayDialogue(Lei_Diag5);
                m_currentStep = 7;
            }
        }
        else if (m_currentStep == 8)
        {
            if (IsFreqOk(currentFreq, FreqTutoEnd))
            {
                Dialogue.PlayDialogue(Lei_Diag6);
                m_currentStep = 9;
            }
        }
        else if (m_currentStep == 9)
        {
            if(IsFreqOk(currentFreq, TargetFreqDiag7) && IsAngleOk(currentAngle, 0, FristAngleThreshold))
            {
                Dialogue.PlayDialogue(Lei_Diag7);
                m_currentStep = 10;
            }
        }
        else if (m_currentStep == 11)
        {
            if(IsFreqOk(currentFreq, TargetFreqDiag9) && IsAngleOk(currentAngle, 0, FristAngleThreshold))
            {
                Dialogue.PlayDialogue(Lei_Diag9);
                m_currentStep = 12;
            }
            
        }
        else if (m_currentStep == 13)
        {
            if(IsFreqOk(currentFreq, TargetFreqDiag11) && IsAngleOk(currentAngle, 0, FristAngleThreshold))
            {
                Dialogue.PlayDialogue(Lei_Diag11);
                m_currentStep = 14;
            }
        }
        else if(m_currentStep == 15)
        { 
            if(IsFreqOk(currentFreq, FinalFreq) && IsAngleOk(currentAngle, FinalAngle, FristAngleThreshold))
            {
                Dialogue.PlayDialogue(Lei_Diag13);
                m_currentStep = 17;
            }
        }
    }

    // Gestion des déblocages par Décryptage (Dialogues 8, 10 et 12)
    private void OnDecodeSuccess(DecryptionModes modeUsed)
    {
        float currentFreq = RadioManager.Instance.RadioBehaviour.FreqKnob.Value;
        float currentAngle = RadioManager.Instance.RadioBehaviour.Antenna.Value;

        // --- TUTO BASE ---
        if (m_currentStep == 3 && modeUsed == ModeRequiredFor30)
        {
            if (Broadcast60 != null) Broadcast60.Volume = m_vol60;
            RefreshRadioSignal();
            m_currentStep = 4;
        }
        else if (m_currentStep == 5 && modeUsed == ModeRequiredFor60)
        {
            if (BroadcastMinus60 != null) BroadcastMinus60.Volume = m_volMinus60;
            RefreshRadioSignal();
            m_currentStep = 6;
        }
        else if (m_currentStep == 7 && modeUsed == ModeRequiredForMinus60)
        {
            if (Broadcast7500 != null) Broadcast7500.Volume = m_vol7500;
            RefreshRadioSignal();
            m_currentStep = 8;
        }

        // --- NOUVEAUX DIALOGUES ---
        

        // Dialogue 8 : Débloqué après décodage Audio (11000 Hz / 0°)
        else if (m_currentStep == 10)
        {
            if (IsFreqOk(currentFreq, TargetFreqDiag8))
            {
                Dialogue.PlayDialogue(Lei_Diag8);
                m_currentStep = 11;
            }
        }
        
        // Dialogue 10 : Débloqué après décodage César (11400 Hz / 12°)
        else if (m_currentStep == 12 && modeUsed == ModeRequiredForDiag10)
        {
            if (IsFreqOk(currentFreq, TargetFreqDiag10))
            {
                Dialogue.PlayDialogue(Lei_Diag10);
                m_currentStep = 13;
            }
        }
         
        // Dialogue 12 : Débloqué après décodage César (8800 Hz / -60°)
        else if (m_currentStep == 14 && modeUsed == ModeRequiredForDiag12)
        {
            if (IsFreqOk(currentFreq, TargetFreqDiag12))
            {
                Dialogue.PlayDialogue(Lei_Diag12);
                m_currentStep = 15;
            }
        }
    }

    // Gestion des enchaînements automatiques après lecture (Dialogues 7, 9, 11)
    private void OnDialogueClosed()
    {
        if (m_currentStep == 1)
        {
            if (Broadcast30 != null) Broadcast30.Volume = m_vol30;
            RefreshRadioSignal();
        }
        // Dialogue 7 : Se lance dès que le tuto (Dialogue 6) est fini
        
        // Dialogue 9 : Se lance dès que le Dialogue 8 se ferme
        
        // Dialogue 11 : Se lance dès que le Dialogue 10 se ferme
        
        // Étape après le Dialogue 12
        //else if (m_currentStep == 15)
        //{
        //    m_currentStep = 16;
        //    // ÉTAPE 16 (DIALOGUE 13) LAISSÉE VIDE COMME DEMANDÉ.
        //    // Tu pourras implémenter ta propre condition ici ou ailleurs.
        //}
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

    private bool IsAngleOk(float angle, float target) => IsAngleOk(angle, target, AngleThreshold);

    private bool IsAngleOk(float angle, float target, float treshold)
    {
        return Mathf.Abs(angle - target) <= treshold;
    }
}