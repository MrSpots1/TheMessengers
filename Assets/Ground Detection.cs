using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundDetection : MonoBehaviour
{
    [SerializeField] public CharecterController2D charecterController;
    public List<Collider2D> currentCollisions = new List<Collider2D>();
    // Start is called before the first frame update
    public bool OnHeadLocal;
    void Start()
    {
          
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        currentCollisions.Add(collision.collider);
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        currentCollisions.Remove(collision.collider);
    }
    // Update is called once per frame
    private void FixedUpdate()
    {
        charecterController.grounded = false;
        OnHeadLocal = false;
        foreach (Collider2D gObject in currentCollisions)
        {

            if (gObject.gameObject.layer == 6 || gObject.gameObject.layer == 7 || gObject.gameObject.layer == 3)
            {
                charecterController.grounded = true;
                if (gObject.gameObject.layer == 6)
                {
                    charecterController.surface = "Ground";
                }
                else if (gObject.gameObject.layer == 7)
                {
                    charecterController.surface = "Ice";
                } else if (gObject.gameObject.layer == 3)
                {
                    OnHeadLocal = true; 
                }
            }
        }
        if (OnHeadLocal)
        {
            charecterController.OnHead = true;
        }
        else
        {
            charecterController.OnHead = false;
        }
    }


}
