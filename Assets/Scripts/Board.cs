using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    public struct Lastswipe
    {
        public Tile tile;
        public float angle;
    }

    
    public int rows;
    public int cols;
    Lastswipe lastswipe;
    int summ =0;
   int score = 0;
    public GameObject scale;
    int multiplexor= 0;
    public Text scoreLabel;
    Tile[,] spawn;
    public Tile[] prefabTiles;
    public Tile[] prefabStars;
    Tile[,] tiles;
    Tile[,] matched;
    public AudioSource blast;
        public bool swipeDesabled = true;
    public enum Actions
    {
        Swipe,
        Refresh,
        CheckMatch,
        Refill,
        Randomizer,
        Revert,
        Spawn,
        Nothing,
        Destroy
    }
    public Actions lastAction;

    delegate bool Delegate(Tile[,] checkedMassive);
    // Start is called before the first frame update
    private void Awake()
    {
        
        spawn = new Tile[cols, rows];
        tiles = new Tile[cols, rows];
        matched = new Tile[cols, rows];
    }
    void Start()
    {
        Spawner();

    }

    // Update is called once per frame

    public void Swipe(float angle, GameObject tile)
    {
        if (lastAction != Actions.Revert)
        {
            if (swipeDesabled)
            {
                return;
            }
            lastAction = Actions.Swipe;

        }


        int x = (int)tile.transform.position.x;
        int y = (int)tile.transform.position.y;

        Tile buffer;

        if (angle < 45 && angle > -45)
        {   //Right
            if(x+1>=cols)
            {
                return;
            }
            buffer = tiles[x + 1, y];
            tiles[x + 1, y] = tiles[x, y];
            tiles[x, y] = buffer;
            lastswipe.tile = buffer;

        }
        if (angle < 135 && angle > 45)
        {
            //Up
            if (y + 1 >= rows)
            {
                return;
            }
            buffer = tiles[x, y + 1];
            tiles[x, y + 1] = tiles[x, y];
            tiles[x, y] = buffer;
            lastswipe.tile = buffer;

        }
        if (angle < -135 || angle > 135)
        {
            //Left
            if (x - 1<0)
            {
                return;
            }
            buffer = tiles[x - 1, y];
            tiles[x - 1, y] = tiles[x, y];
            tiles[x, y] = buffer;
            lastswipe.tile = buffer;
        }

        if (angle < -45 && angle > -135)
        {
            //Down
            if (y - 1 < 0)
            {
                return;
            }
            buffer = tiles[x, y - 1];
            tiles[x, y - 1] = tiles[x, y];
            tiles[x, y] = buffer;
            lastswipe.tile = buffer;
        }
        lastswipe.angle = angle;

        Refresh();
    }

    public bool CheckMatch(Tile[,] checkedArray)
    {
        Clear(matched);
        bool matchFinded = false;
        int match;
        for (int x = 0; x < cols; x++)
        {

            match = 1;
            for (int y = 0; y < rows; y++)
            {
                if (y - 1 < 0)
                {
                    continue;
                }
                if (checkedArray[x, y - 1].CompareTag(checkedArray[x, y].tag))
                {
                    match++;
                }
                else if (match >= 3)
                {
                    for (int i = 1; i <= match; i++)
                    {
                        UseAbility(checkedArray[x, y - i].GetType().ToString(),x,y-i);
                        matched[x, y - i] = checkedArray[x, y - i];
                        Debug.Log(matched[x, y - i]);
                    }
                    matchFinded = true;
                    match = 1;
                }
                else
                {
                    match = 1;
                }



            }
            if (match >= 3)
            {
                for (int i = 0; i < match; i++)
                {
                    UseAbility(checkedArray[x, rows - 1 - i].GetType().ToString(), x, rows - 1 - i);
                    matched[x, rows - 1 - i] = checkedArray[x, rows - i - 1];

                    Debug.Log(matched[x, rows - 1 - i]);
                }
                matchFinded = true;

            }
        }
        for (int y = 0; y < rows; y++)

        {

            match = 1;
            for (int x = 0; x < cols; x++)
            {
                if (x - 1 < 0)
                {
                    continue;
                }
                if (checkedArray[x - 1, y].CompareTag(checkedArray[x, y].tag))
                {
                    match++;
                }
                else if (match >= 3)
                {
                    for (int i = 1; i <= match; i++)
                    {
                        if(matched[x - i, y]!=null
                             && matched[x  - i, y].GetType().ToString() != "Star"
                              && !matched[x - i, y].explode)
                        {
                            ReplaceTile(x - i, y);
                            matched[x - i, y] = null;
                            continue;
                        }
                        UseAbility(checkedArray[x - i, y].GetType().ToString(),  x - i, y);
                        matched[x - i, y] = checkedArray[x - i, y];
                        Debug.Log(matched[x - i, y]);
                    }
                    matchFinded = true;
                    match = 1;
                }
                else
                {
                    match = 1;
                }
            }
            if (match >= 3)
            {
                for (int i = 0; i < match; i++)
                {
                    if (matched[cols - 1 - i, y] != null
                        && matched[cols - 1 - i, y].GetType().ToString()!="Star"
                        && !matched[cols - 1 - i, y].explode)
                    {
                        ReplaceTile(cols - 1 - i, y);
                        matched[cols - 1 - i, y] = null;
                        continue;
                    }
                    UseAbility(checkedArray[cols - 1 - i, y].GetType().ToString(), cols - 1 - i, y);
                    
                    matched[cols - 1 - i, y] = checkedArray[cols - 1 - i, y];
                    Debug.Log(matched[cols - 1 - i, y]);
                }
                matchFinded = true;
            }
        }
        return matchFinded;
    }

    private void UseAbility(string type,int x,int y)
    {
        switch (type)
        {
            case "Star":
                Blast(x,y);
                blast.PlayOneShot(blast.clip);
                break;
            default:
                break;
        }
    }

    private void Blast(int epX,int epY)
    {
        for (int x = epX-2; x <= epX + 2; x++)
        {
            for (int y = epY-2; y <=epY+2; y++)
            {

                if(x>=0 && y>=0 && x<cols && y<rows)
                {
                    if (x == epX && y == epY)
                    {
                        tiles[x, y].explode = true;
                        continue;
                    }
                    matched[x, y] = tiles[x, y];
                    if (!matched[x, y].explode)
                    {
                        matched[x, y].explode = true;
                        UseAbility(matched[x, y].GetType().ToString(), x, y);
                    }
                    matched[x, y].explode = true;
                }
            }

        }
    }

    private void ReplaceTile(int x,int y)
    {   if(tiles[x,y]!=null)
        {
            int id = tiles[x, y].id ;
        Destroy(tiles[x, y].gameObject);
        
            bool flagLeft=true;
            bool flagRight = true;
            bool flagUp = true;
            bool flagDown = true;
            
            int i = 1;
            Vector3 newPosition = new Vector3(x, y, 0);
            while (flagDown||flagLeft||flagRight||flagUp)
            {
                if(x-i>=0&& tiles[x,y].CompareTag(tiles[x-i, y].tag)&&flagLeft)
                {
                    tiles[x - i, y].MoveTo(newPosition);
                }
                else
                {
                    flagLeft = false;
                }
                if (x + i <cols && tiles[x, y].CompareTag(tiles[x + i, y].tag) && flagRight)
                {
                    tiles[x + i, y].MoveTo(newPosition);
                }
                else
                {
                    flagRight = false;
                }
                if (y-i>=0 && tiles[x, y].CompareTag(tiles[x , y- i].tag) && flagDown)
                {
                    tiles[x , y-i].MoveTo(newPosition);
                }
                else
                {
                    flagDown = false;
                }
                if (y+i<rows && tiles[x, y].CompareTag(tiles[x , y+i].tag) && flagUp)
                {
                    tiles[x , y+ i].MoveTo(newPosition);
                }
                else
                {
                    flagUp = false;
                }

                i++;
            }
            tiles[x, y] = Instantiate(prefabStars[id],
                    new Vector3(x, y), Quaternion.identity);

        }
    else
        {
            Destroy(spawn[x, y].gameObject);
            spawn[x, y] = Instantiate(prefabTiles[ Random.Range(0,prefabTiles.Length)],
                        new Vector3(x, y + rows - 1, 0), Quaternion.identity);
        }
        
    }

    public void Refill()
    {
        Tile[] refil = new Tile[rows];
        int refilIndex;
        int empty;
        

        for (int x = 0; x < cols; x++)
        {
            empty = 0;
            refilIndex = 0;
            for (int y = 0; y < rows; y++)
            {
                if(tiles[x,y]==null)
                {
                    
                    empty++;
                }
                
                else 
                {
                    refil[refilIndex] = tiles[x, y];
                    refilIndex++;
                }
            }
           
            for (int i=0;i<empty; i++)
            {
                int id = Random.Range(0, prefabTiles.Length );
                refil[refilIndex] = Instantiate(prefabTiles[id],
                    new Vector3(x,rows+i, 0) , Quaternion.identity);
                refil[refilIndex].id = id;
                 refilIndex++;
            }
            for (int y=0; y<rows; y++)
            {
                tiles[x, y] = refil[y];
            }

        }
        
            Refresh();
        

        lastAction = Actions.Refill;
       
    }
 
    private void Refresh()
    { 
            for (int x = 0; x < cols; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    
                    Vector3 newPosition = new Vector3(x, y, 0);
              
                tiles[x, y].MoveTo(newPosition);
                }
                
            }
    
        StartCoroutine(WaitingMoving());
    }

    void Spawner()
    {
        for (int x = 0; x < cols; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                int id = Random.Range(0, prefabTiles.Length / 2);
                spawn[x, y] = Instantiate(prefabTiles[id],
                    new Vector3(x, y + rows-1, 0), Quaternion.identity);
                spawn[x, y].id = id;
            }
        }
        int iteration = 0;
        while (CheckMatch(spawn))
        {
            iteration++;
            
            Randomizer();
            
        }
        tiles = spawn;
        lastAction = Actions.Spawn;
       Refresh();
    }
    void Randomizer()
    {
        
            for (int x = 0; x < cols; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    if (matched[x, y] != null)
                    {
                        matched[x, y] = null;
                        Destroy(spawn[x, y].gameObject);
                    int id = Random.Range(0, prefabTiles.Length);
                        spawn[x, y] = Instantiate(prefabTiles[id],
                        new Vector3(x, y + rows-1, 0), Quaternion.identity);
                    spawn[x, y].id = id;


                }

                }
            }
        
    }
    
    public IEnumerator WaitingMoving()
    {
        yield return new WaitForSeconds(0.3f);
        bool ready = false;
        while(!ready)
        {
            ready = true;
            for (int x = 0; x < cols; x++)
            {
                for (int y = 0; y < rows; y++)
                {

                  if(!tiles[x,y].ready)
                    {
                        ready = false;
                    }
                }

            }
            yield return null;
        }

            Controller();  
    }
    
    void TileDestroyer()
    {
        for (int x = 0; x < cols; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                if (matched[x, y] != null)
                {
                    
                    matched[x, y] = null;
                     tiles[x, y].OnRemove();
                    tiles[x, y] = null;
                    summ += 10;


                }

            }
        }
        StartCoroutine(Await());
       
    }

    private IEnumerator Await()
    {
        yield return new WaitForSeconds(0.4f);
        Refill();
    }

    public void Controller()
    {
        Debug.Log(lastAction);
        if (lastAction == Actions.Spawn)
        {
            swipeDesabled = false;
            return;
        }
        if (lastAction == Actions.Swipe)
        {
            swipeDesabled = true;
            if (CheckMatch(tiles))
            {
                TileDestroyer();
                multiplexor += 1;
            }
            else
            {
                lastAction = Actions.Revert;
                Swipe(lastswipe.angle, lastswipe.tile.gameObject);
            }
            return;
        }
        if (lastAction == Actions.Revert)
        {
            swipeDesabled = false;
            lastAction = Actions.Nothing;
            return;
        }
        if (lastAction == Actions.Refill)
        {
            if (CheckMatch(tiles))
            {
                TileDestroyer();
                multiplexor += 1;
            }
            else
            {
                swipeDesabled = false;
                lastAction = Actions.Nothing;
                score+= summ * multiplexor;
                summ = 0;
                multiplexor = 0;
                scoreLabel.text = score.ToString();
            }
            return;
        }
    }
    private void Clear(Tile[,] cleared)
    {
        for (int x = 0; x < cols; x++)
        { 
            for (int y = 0; y < rows; y++)
            {
                
                    cleared[x, y] = null;
               

            }
        }
    }
}

