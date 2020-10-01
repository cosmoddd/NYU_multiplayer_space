using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Zitta_TextRenderer : MonoBehaviour {
    public GameObject AnimBase;
    public GameObject Pivot;
    public TextMeshPro TEXT;
    public Animator Anim;
    public AudioSource AS;
    public string Value;
    public float Duration;
    public float CurrentTime;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Render();

        CurrentTime += Time.deltaTime;
        if (CurrentTime >= GetDuration())
            End();
    }

    public void FixedUpdate()
    {
        Render();
    }

    public void LateUpdate()
    {
        Render();
    }

    public void Ini(string value, GameObject Base, float height, float duration, AudioClip Audio)
    {
        transform.parent = Base.transform;
        transform.localPosition = new Vector3(0, height, 0);
        Value = value;
        Duration = duration;
        if (Audio)
            AS.PlayOneShot(Audio);
    }

    public void Render()
    {
        Pivot.transform.forward = -(GetCamera().transform.position - Pivot.transform.position);
        Pivot.transform.eulerAngles = new Vector3(0, Pivot.transform.eulerAngles.y, Pivot.transform.eulerAngles.z);
        TEXT.text = GetText();
    }

    public void End()
    {
        Destroy(gameObject, 0.1f);
        gameObject.SetActive(false);
    }

    public string GetText()
    {
        return Value;
    }

    public float GetDuration()
    {
        return Duration;
    }

    public Camera GetCamera()
    {
        return Camera.main;
    }


}
