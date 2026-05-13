/*using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI; // Indispensable pour utiliser le ScrollRect

public class IngameTerminal : MonoBehaviour
{
    [Header("UI Reference")]
    public TMP_Text TerminalText;
    public ScrollRect TerminalScrollRect; // La reference a la zone de defilement

    [Header("Parametres d'affichage")]
    [Tooltip("On peut augmenter la limite vu qu'on a un scroll !")]
    public int MaxLines = 50;

    [Header("Controle du Scroll")]
    [Tooltip("Vitesse de defilement manuel")]
    public float ScrollSpeed = 2.0f;

    public bool ShowAllUnityLogs = false;

    private Queue<string> m_logQueue = new Queue<string>();

    private void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    private void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        // On ne garde que nos logs colores (ou les erreurs de code)
        if (!ShowAllUnityLogs && !logString.Contains("<color=") && type == LogType.Log)
        {
            return;
        }

        string prefix = "";
        if (type == LogType.Error || type == LogType.Exception) prefix = "<color=red>[ERREUR CRITIQUE]</color> ";
        if (type == LogType.Warning) prefix = "<color=orange>[ATTENTION]</color> ";

        string finalLog = prefix + logString;

        m_logQueue.Enqueue(finalLog);

        while (m_logQueue.Count > MaxLines)
        {
            m_logQueue.Dequeue();
        }

        UpdateTerminalDisplay();
    }

    private void UpdateTerminalDisplay()
    {
        if (TerminalText != null)
        {
            TerminalText.text = string.Join("\n", m_logQueue);

            
            if (TerminalScrollRect != null)
            {
                StartCoroutine(ForceScrollToBottom());
            }
        }
    }

    private IEnumerator ForceScrollToBottom()
    {
        
        yield return new WaitForEndOfFrame();

        // 0 =  bas, 1 = haut
        TerminalScrollRect.verticalNormalizedPosition = 0f;
    }

    private void Update()
    {
        if (TerminalScrollRect == null) return;

        // On utilise l'InputSystem pour lire la molette de la souris
        if (UnityEngine.InputSystem.Mouse.current != null)
        {
            float scrollInput = UnityEngine.InputSystem.Mouse.current.scroll.ReadValue().y;

            if (Mathf.Abs(scrollInput) > 0.1f)
            {
                // verticalNormalizedPosition va de 0 (tout en bas) a 1 (tout en haut)
                // La valeur de la molette est souvent tres grande (ex: 120 ou -120), on la reduit
                float delta = (scrollInput > 0 ? 1f : -1f) * ScrollSpeed * Time.deltaTime;

                TerminalScrollRect.verticalNormalizedPosition += delta;

                // On bloque la valeur entre 0 et 1 pour ne pas depasser
                TerminalScrollRect.verticalNormalizedPosition = Mathf.Clamp01(TerminalScrollRect.verticalNormalizedPosition);
            }
        }
    }
}*/