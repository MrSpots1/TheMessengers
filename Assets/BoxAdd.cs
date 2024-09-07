using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxAdd : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] CharecterController2D charecterController;
    void Awake()
    {
        DynamicBox box = new DynamicBox();
        box.gameobject = gameObject;
        box.Xposition = transform.position.x;
        box.Yposition = transform.position.y;
        charecterController.dynamicBoxes.Add(box);
    }

    
}
