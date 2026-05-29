using UnityEngine;
using UnityEngine.UI;

public class UICameraButtons : MonoBehaviour
{
    public PlayerBehaviour Player;

    public Button Left;
    public Button Right;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Left.gameObject.SetActive(false);
        Right.gameObject.SetActive(true);

        Left.onClick.AddListener(() =>
        {
            Right.gameObject.SetActive(true);
            Left.gameObject.SetActive(false);
            Player.SetCamera(0);
        });


        Right.onClick.AddListener(() =>
        {
            Right.gameObject.SetActive(false);
            Left.gameObject.SetActive(true);
            Player.SetCamera(1);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
