using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : Tile
{
  

   

    private void Awake()
    {
        
        board = FindObjectOfType<Board>();
        ready = false;
    }
    private void Update()
    {


    }

    public override void OnRemove()
    {
        StartCoroutine(Remove());
      
    }

    public override void MoveTo(Vector3 newPosition)
    {
        pos = newPosition;
        ready = false;
      
        StartCoroutine(Moving(newPosition));
    }

    private IEnumerator Remove()
    {
        Vector3 startSize = transform.localScale;

        for (float i = 0; i <= 1; i += Time.deltaTime / 0.3f)
        {

            transform.localScale = new Vector3(startSize.x * sizeEasing.Evaluate(i),
                startSize.y * sizeEasing.Evaluate(i), 1);
            yield return null;

        }
        Destroy(gameObject);


    }
    private IEnumerator Moving(Vector3 newPosition)
    {

        Vector3 startPosition = transform.position;
        ready = false;
        for (float i = 0; i <= 1; i += Time.deltaTime / duration)
        {
            transform.position = Vector3.LerpUnclamped(startPosition,
                                   newPosition, speadEasing.Evaluate(i));
            yield return null;
        }
        transform.position = newPosition;

        ready = true;
    }


}
