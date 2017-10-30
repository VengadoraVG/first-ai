using UnityEngine;
using System.Collections;

public class TestIndicator : MonoBehaviour {
    public float time = 0.5f;

    private Color initialColor;
    private Renderer rend;

    void Start () {
        rend = transform.GetComponent<Renderer>();
        // rend.material.shader = Shader.Find("Specular");
        initialColor = rend.material.GetColor("_Color");
    }


    public void Indicate (Color color, bool permanent = false) {
        StartCoroutine(StartIndicating(color, permanent));
    }

    public IEnumerator StartIndicating (Color color, bool permanent = false) {
        rend.material.SetColor("_Color", color);
        if (!permanent) {
            yield return new WaitForSeconds(time);
            rend.material.SetColor("_Color", initialColor);
        } else {
            yield return null;
        }
    }
}
