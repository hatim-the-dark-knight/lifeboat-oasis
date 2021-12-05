using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterColor : MonoBehaviour
{
    private Material material;

    public ColorPicker color_picker;//

    private float timer = 0f;
    private float color_timer = 0.25f;
    // Start is called before the first frame update
    void Start()
    {
        material = GetComponent<SkinnedMeshRenderer>().material;
        material.color = new Color(1f, 1f, 1f, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer >= color_timer)
        {
            timer = 0f;
            material.color = color_picker.colorComponent.color;//
        }
    }
}
