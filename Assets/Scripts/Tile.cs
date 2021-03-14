using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public AnimationCurve speadEasing;
    public AnimationCurve sizeEasing;
    protected Vector2 firstTouch;
    protected Vector2 finalTouch;
    public Board board;
    public bool ready;
    public Vector2 pos;
    public float duration;
    public int id;
    public ParticleSystem shatter;
    public bool explode = false;

    private void Awake()
    {
        board = FindObjectOfType<Board>();
        ready = false;
    }
    private void Update()
    {
       

    }


    protected void OnMouseDown()
    {
        if(!ready)
        {
            return;
        }
        firstTouch = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
    protected void OnMouseUp()
    {
        if (!ready)
        {
            return;
        }

        finalTouch = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float angle = Mathf.Atan2(finalTouch.y - firstTouch.y, finalTouch.x - firstTouch.x)*Mathf.Rad2Deg;
        
            board.Swipe(angle, gameObject);
        
       
    }
    public virtual void OnRemove()
    {
        if (explode)
        {
            Boom();
            return;
        }
        StartCoroutine(Remove()); 
    }
    
    public virtual void MoveTo(Vector3 newPosition)
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
        for (float i=0;i<=1;i+=Time.deltaTime/duration)
        {
            transform.position = Vector3.LerpUnclamped(startPosition,
                                   newPosition,speadEasing.Evaluate(i));  
            yield return null;
        }
        transform.position = newPosition;
        
        ready = true;
    }

    void Boom()
    {
        Instantiate(shatter,transform.position,Quaternion.identity);
        Destroy(gameObject);
    }

    
}
