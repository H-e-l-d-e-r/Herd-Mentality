using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement; // Requis pour charger les scènes
using DialogueSystem;

public class WakeUpCinematic : MonoBehaviour
{
    [Header("UI Elements (Paupières)")]
    public RectTransform TopEyelid;
    public RectTransform BottomEyelid;

    [Header("Cinematic Settings")]
    public float InitialDelay = 1.0f;
    public float AnimationSpeed = 1.0f;

    [Header("Dialogue Integration")]
    public DialogueTable WakeUpDialogue;

    [Header("Scene Transition")]
    [Tooltip("Le nom exact de la scène de radio à charger après le dialogue.")]
    public string NextSceneName = "NomDeTaSceneRadio";

    private float m_screenHeight;
    private Vector2 m_topInitialPos;
    private Vector2 m_bottomInitialPos;

    void Start()
    {
        m_screenHeight = TopEyelid.rect.height;
        m_topInitialPos = TopEyelid.anchoredPosition;
        m_bottomInitialPos = BottomEyelid.anchoredPosition;

        StartCoroutine(WakeUpRoutine());
    }

    private IEnumerator WakeUpRoutine()
    {
        yield return new WaitForSeconds(InitialDelay);

        yield return MoveEyelids(0.15f, 0.3f / AnimationSpeed);
        yield return new WaitForSeconds(0.1f / AnimationSpeed);
        yield return MoveEyelids(0.0f, 0.2f / AnimationSpeed);
        yield return new WaitForSeconds(0.6f / AnimationSpeed);

        yield return MoveEyelids(0.4f, 0.4f / AnimationSpeed);
        yield return new WaitForSeconds(0.2f / AnimationSpeed);
        yield return MoveEyelids(0.0f, 0.2f / AnimationSpeed);
        yield return new WaitForSeconds(0.5f / AnimationSpeed);

        yield return MoveEyelids(1.0f, 1.5f / AnimationSpeed);

        TopEyelid.gameObject.SetActive(false);
        BottomEyelid.gameObject.SetActive(false);

        TriggerDialogue();
    }

    private IEnumerator MoveEyelids(float targetOpenPercent, float duration)
    {
        float elapsed = 0f;
        Vector2 startTopPos = TopEyelid.anchoredPosition;
        Vector2 startBottomPos = BottomEyelid.anchoredPosition;

        Vector2 targetTopPos = m_topInitialPos + new Vector2(0, m_screenHeight * targetOpenPercent);
        Vector2 targetBottomPos = m_bottomInitialPos + new Vector2(0, -m_screenHeight * targetOpenPercent);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);

            TopEyelid.anchoredPosition = Vector2.Lerp(startTopPos, targetTopPos, t);
            BottomEyelid.anchoredPosition = Vector2.Lerp(startBottomPos, targetBottomPos, t);

            yield return null;
        }

        TopEyelid.anchoredPosition = targetTopPos;
        BottomEyelid.anchoredPosition = targetBottomPos;
    }

    private void TriggerDialogue()
    {
        if (WakeUpDialogue != null)
        {
            // On s'abonne à l'événement : "Quand le dialogue se ferme, appelle la fonction GoToNextScene"
            Dialogue.Instance.OnDialogueCloseEvent += GoToNextScene;

            // On lance le dialogue
            DialoguePtr dialoguePtr = Dialogue.RegisterDialogue(WakeUpDialogue);
            Dialogue.PlayDialogue(dialoguePtr);
        }
        else
        {
            Debug.LogWarning("Aucun dialogue assigné ! Passage direct à la scène suivante.");
            GoToNextScene();
        }
    }

    private void GoToNextScene()
    {
        // On se désabonne pour éviter des bugs de mémoire si on revient sur cette scène plus tard
        if (Dialogue.Instance != null)
        {
            Dialogue.Instance.OnDialogueCloseEvent -= GoToNextScene;
        }

        // On charge la scène de radio
        if (!string.IsNullOrEmpty(NextSceneName))
        {
            SceneManager.LoadScene(NextSceneName);
        }
        else
        {
            Debug.LogError("Le nom de la scène suivante n'est pas défini dans l'inspecteur !");
        }
    }
}