using UnityEngine;
using System.Collections;


[RequireComponent(typeof(Movement))]
public class Pacman : MonoBehaviour
{
    public AnimatedSprite deathSequence;
    public SpriteRenderer spriteRenderer { get; private set; }
    public new Collider2D collider { get; private set; }
    public Movement movement { get; private set; }

    private ArrayList moveArr = new ArrayList();
    private ArrayList moveArrGhost = new ArrayList();

    public GhostMovement ghostMovement { get; private set; }
    public int counter = 0;
    public float score = 100;
    public bool start = true;
    public Vector3 ghostPos;
    public string chosenAction = "" ;
    public string ghostChosenAction = "" ;


    public Ghost ghost;


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        collider = GetComponent<Collider2D>();
        movement = GetComponent<Movement>();
        ghostMovement = FindObjectOfType<Ghost>().GetComponent<GhostMovement>();

        //moveArr = movement.AStarSearch();
            
        chosenAction = (string)movement.globalMinimax(0, 0, true, true, ghost.transform.position,false,Mathf.NegativeInfinity,Mathf.Infinity)[1];
        ghostChosenAction = (string)ghostMovement.globalMinimax(1, 0, false, true, movement.getPacmanPosition(),false,Mathf.NegativeInfinity,Mathf.Infinity)[1];

        Debug.Log(moveArr);
        StartPlayback();
    }

    void StartPlayback()
    {
        StartCoroutine(Playback());
    }

    IEnumerator Playback()
    {
        // do{
        //     if (counter < moveArr.Count){
        //          if (moveArr[counter].Equals("s"))
        //         {
        //             movement.SetDirection(Vector2.down);
        //         }
        //         else if (moveArr[counter].Equals("n"))
        //         {
        //             movement.SetDirection(Vector2.up);
        //         }
        //         else if (moveArr[counter].Equals("e"))
        //         {
        //             movement.SetDirection(Vector2.right);
        //         }
        //         else if (moveArr[counter].Equals("w"))
        //         {
        //             movement.SetDirection(Vector2.left);
        //         }
        //         counter++;
        //     }
        //     // Rotate pacman to face the movement direction
        //     float angle = Mathf.Atan2(movement.direction.y, movement.direction.x);
        //     transform.rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, Vector3.forward);
             
        //     yield return new WaitForSeconds(0.1f);
        // }while(true);
        do
        {
            if (!movement.isAllFoodEaten())
            {

                moveArr.Add(chosenAction);
                moveArrGhost.Add(ghostChosenAction);

                Debug.Log("Movement element: " + ghostChosenAction);


                if (moveArr[0].Equals("s"))
                {
                    movement.SetDirection(Vector2.down);
                }
                else if (moveArr[0].Equals("n"))
                {
                    movement.SetDirection(Vector2.up);
                }
                else if (moveArr[0].Equals("e"))
                {
                    movement.SetDirection(Vector2.right);
                }
                else if (moveArr[0].Equals("w"))
                {
                    movement.SetDirection(Vector2.left);
                }


                if (moveArrGhost[0].Equals("s"))
                {
                    ghostMovement.SetDirection(Vector2.down);
                }
                else if (moveArrGhost[0].Equals("n"))
                {
                    ghostMovement.SetDirection(Vector2.up);
                }
                else if (moveArrGhost[0].Equals("e"))
                {
                    ghostMovement.SetDirection(Vector2.right);
                }
                else if (moveArrGhost[0].Equals("w"))
                {
                    ghostMovement.SetDirection(Vector2.left);
                }
                

                moveArr = new ArrayList();
                moveArrGhost = new ArrayList();
                chosenAction = (string)movement.globalMinimax(0, 0, true, false, ghost.transform.position,false,Mathf.NegativeInfinity,Mathf.Infinity)[1];
                ghostChosenAction = (string)ghostMovement.globalMinimax(1, 0, false, false, movement.getPacmanPosition(),false,Mathf.NegativeInfinity,Mathf.Infinity)[1];


        //         /*
        //         bool notAssigned = true;
        //         Vector3 newGhostPosition = new Vector3(0,0,0);
        //         while(notAssigned)
        //         {
        //             int randomMovement = Random.Range(0,4);
        //             newGhostPosition = new Vector3(0,0,0);
        //             Vector2 direction = new Vector2(0,0);
        //             if(randomMovement == 0)
        //             {
        //                 newGhostPosition = ghost.transform.position + new Vector3(1,0,0);
        //                 direction = new Vector2(1,0);
        //             }else if(randomMovement == 1)
        //             {
        //                 newGhostPosition = ghost.transform.position + new Vector3(-1,0,0);
        //                 direction = new Vector2(-1,0);

        //             }else if(randomMovement == 2)
        //             {
        //                 newGhostPosition = ghost.transform.position + new Vector3(0,1,0);
        //                 direction = new Vector2(0,1);

        //             }else if(randomMovement == 3)
        //             {
        //                 newGhostPosition = ghost.transform.position + new Vector3(0,-1,0);
        //                 direction = new Vector2(0,-1);
        //             }
        //             notAssigned = movement.OccupiedTried(new Vector2(newGhostPosition.x,newGhostPosition.y),direction);

        //         }
        //         ghost.transform.position = newGhostPosition;
        //         */
                
        //         counter++;
           }

            // Rotate pacman to face the movement direction
            float angle = Mathf.Atan2(movement.direction.y, movement.direction.x);
            transform.rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, Vector3.forward);
             
            yield return new WaitForSeconds(0.2f);
        } while (true);
    }

    void StopPlayback()
    {
        StopCoroutine(Playback());
    }

    private void Update()
    {
        /*
        if (!movement.isAllFoodEaten())
            {

                chosenAction = (string)movement.globalMinimax(0, 0, true, false, ghost.transform.position,false,Mathf.NegativeInfinity,Mathf.Infinity)[1];
                ghostChosenAction = (string)ghostMovement.globalMinimax(1, 0, false, false, movement.getPacmanPosition(),false,Mathf.NegativeInfinity,Mathf.Infinity)[1];

                moveArr.Add(chosenAction);
                moveArrGhost.Add(ghostChosenAction);

                Debug.Log("Movement element: " + ghostChosenAction);


                if (moveArr[0].Equals("s"))
                {
                    movement.SetDirection(Vector2.down);
                }
                else if (moveArr[0].Equals("n"))
                {
                    movement.SetDirection(Vector2.up);
                }
                else if (moveArr[0].Equals("e"))
                {
                    movement.SetDirection(Vector2.right);
                }
                else if (moveArr[0].Equals("w"))
                {
                    movement.SetDirection(Vector2.left);
                }


                if (moveArrGhost[0].Equals("s"))
                {
                    ghostMovement.SetDirection(Vector2.down);
                }
                else if (moveArrGhost[0].Equals("n"))
                {
                    ghostMovement.SetDirection(Vector2.up);
                }
                else if (moveArrGhost[0].Equals("e"))
                {
                    ghostMovement.SetDirection(Vector2.right);
                }
                else if (moveArrGhost[0].Equals("w"))
                {
                    ghostMovement.SetDirection(Vector2.left);
                }
                

                moveArr = new ArrayList();
                moveArrGhost = new ArrayList();


                
                bool notAssigned = true;
                Vector3 newGhostPosition = new Vector3(0,0,0);
                while(notAssigned)
                {
                    int randomMovement = Random.Range(0,4);
                    newGhostPosition = new Vector3(0,0,0);
                    Vector2 direction = new Vector2(0,0);
                    if(randomMovement == 0)
                    {
                        newGhostPosition = ghost.transform.position + new Vector3(1,0,0);
                        direction = new Vector2(1,0);
                    }else if(randomMovement == 1)
                    {
                        newGhostPosition = ghost.transform.position + new Vector3(-1,0,0);
                        direction = new Vector2(-1,0);

                    }else if(randomMovement == 2)
                    {
                        newGhostPosition = ghost.transform.position + new Vector3(0,1,0);
                        direction = new Vector2(0,1);

                    }else if(randomMovement == 3)
                    {
                        newGhostPosition = ghost.transform.position + new Vector3(0,-1,0);
                        direction = new Vector2(0,-1);
                    }
                    notAssigned = movement.OccupiedTried(new Vector2(newGhostPosition.x,newGhostPosition.y),direction);

                }
                ghost.transform.position = newGhostPosition;
                
                
                counter++;
                
            }

        // Rotate pacman to face the movement direction
        float angle = Mathf.Atan2(movement.direction.y, movement.direction.x);
        transform.rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, Vector3.forward);*/

    }

    public void MovePacman(string[] moves)
    {
        for (int i = 0; i < moves.Length; i++)
        {
            if (moves[i].Equals("s"))
            {
                movement.SetDirection(Vector2.down);
            }
            else if (moves[i].Equals("n"))
            {
                movement.SetDirection(Vector2.up);
            }
            else if (moves[i].Equals("e"))
            {
                movement.SetDirection(Vector2.right);
            }
            else if (moves[i].Equals("w"))
            {
                movement.SetDirection(Vector2.left);
            }
        }
    }

    public void ResetState()
    {
        enabled = true;
        spriteRenderer.enabled = true;
        collider.enabled = true;
        deathSequence.enabled = false;
        deathSequence.spriteRenderer.enabled = false;
        movement.ResetState();
        gameObject.SetActive(true);
    }

    public void DeathSequence()
    {
        enabled = false;
        spriteRenderer.enabled = false;
        collider.enabled = false;
        movement.enabled = false;
        deathSequence.enabled = true;
        deathSequence.spriteRenderer.enabled = true;
        deathSequence.Restart();
    }

}
