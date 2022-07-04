
using UnityEngine;
using UnityEngine.UI;
public class FloatingText
{
    public bool active;
    public GameObject go;
    public float duration;
    public float lastShown;
    public Text txt;
    public Vector3 motion;
    
    //show text
    public void Show()
    {
        active = true;  
        lastShown = Time.time;
        go.SetActive(true);
    }
    
    //hide text
    public void hide()
    {
        active= false;  
        go.SetActive(false);    
    }

    //update text
    public void UpdateFloatingText()
    {
        if (!active)
            return;

        if (Time.time - lastShown > duration)
            hide();
        go.transform.position += motion * Time.deltaTime;
    }

}
