using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextBubble : MonoBehaviour
{
    // Start is called before the first frame update
    private TextMesh textMesh;
    private SpriteRenderer spriteRenderer;
    private Animation anim;

    void Start()
    {
        anim = GetComponent<Animation>();
        textMesh = GetComponentInChildren<TextMesh>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        textMesh.text = "";
        gameObject.SetActive(false);
    }

    public void DisableBubble()
    {
        anim.Stop();
        textMesh.text = "";
        spriteRenderer.size = new Vector2(2, 2);
        gameObject.SetActive(false);
    }

    public void ShowText(string text)
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
            textMesh.text = text;
            spriteRenderer.size = new Vector2(text.Length / 3 + 0.1f, 2);
            anim.Play();
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Camera.main.transform.rotation;
    }
}
