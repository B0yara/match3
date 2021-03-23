using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Play : MonoBehaviour


{


   public Animator anim;
    // Start is called before the first frame update
   

  
    public void Startgame()
    {
        anim.SetBool("Start", true);
        Debug.Log("Start");
    }
    
}
