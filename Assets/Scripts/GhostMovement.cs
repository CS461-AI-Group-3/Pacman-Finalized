using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D))]
public class GhostMovement : MonoBehaviour
{
    public float speed = 8f;
    public float speedMultiplier = 1f;
    public Vector2 initialDirection;
    public LayerMask obstacleLayer;

    public new Rigidbody2D rigidbody { get; private set; }
    public Vector2 direction { get; private set; }
    public Vector2 nextDirection { get; private set; }
    public Vector3 startingPosition { get; private set; }

    public float totalScore;

    public float score2;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        startingPosition = transform.position;
        totalScore = 0;
    }

    public struct SuccessorNode
    {
        public Vector2 state;
        public ArrayList actions;
        public ArrayList actionCoordinates;
        public int cost;

        public bool[] visited;
    }

    public bool OccupiedTried(Vector2 nextPos, Vector2 direction)
    {
        //Debug.Log("obstacle: " + direction);
        // If no collider is hit then there is no obstacle in that direction
        Vector3 pos = new Vector3(nextPos.x, nextPos.y, 0);
        RaycastHit2D hit = Physics2D.BoxCast(pos, Vector2.one * 0.5f, 0f, direction, 1f, obstacleLayer);
        return hit.collider != null;
    }

    public int manhattanDistance(Vector2 a, Vector2 b)
    {
        float xCoordCurrent = Mathf.Round(a.x);
        float yCoordCurrent = Mathf.Round(a.y);
        float xCoordGoal = Mathf.Round(b.x);
        float yCoordGoal = Mathf.Round(b.y);

        float result = Mathf.Abs(xCoordCurrent - xCoordGoal) + Mathf.Abs(yCoordCurrent - yCoordGoal);

        Debug.Log("Man distance: " + (int)Mathf.Round(result));
        
        return (int)Mathf.Round(result);
    }

    public ArrayList generateSuccessor(Vector2 currentPos)
    {
        Vector2 north = new Vector2(0, 1f);
        Vector2 south = new Vector2(0, -1f);
        Vector2 east = new Vector2(1f, 0);
        Vector2 west = new Vector2(-1f, 0);

        Vector2 translationNorth = north;
        Vector2 translationSouth = south;
        Vector2 translationEast = east;
        Vector2 translationWest = west;

        ArrayList successors = new ArrayList();

        Debug.Log("n: " + OccupiedTried((currentPos + translationNorth), north) + "s: " + OccupiedTried((currentPos + translationSouth), south) + "e: " + OccupiedTried((currentPos + translationEast), east) + "w: " + OccupiedTried((currentPos + translationWest), west));

        if (!OccupiedTried((currentPos + translationNorth), north))
        {
            SuccessorNode node = new SuccessorNode();
            node.state = (currentPos + translationNorth);
            ArrayList tempAction = new ArrayList();
            ArrayList tempCoordinates = new ArrayList();
            tempAction.Add("n");
            tempCoordinates.Add((currentPos + translationNorth));
            node.actions = tempAction;
            node.actionCoordinates = tempCoordinates;
            node.cost = 1;
            successors.Add(node);
        }
        if (!OccupiedTried((currentPos + translationSouth), south))
        {
            SuccessorNode node = new SuccessorNode();
            node.state = (currentPos + translationSouth);
            ArrayList tempAction = new ArrayList();
            ArrayList tempCoordinates = new ArrayList();
            tempAction.Add("s");
            tempCoordinates.Add((currentPos + translationSouth));
            node.actions = tempAction;
            node.actionCoordinates = tempCoordinates;
            node.cost = 1;
            successors.Add(node);
        }
        if (!OccupiedTried((currentPos + translationEast), east))
        {
            SuccessorNode node = new SuccessorNode();
            node.state = (currentPos + translationEast);
            ArrayList tempAction = new ArrayList();
            ArrayList tempCoordinates = new ArrayList();
            tempAction.Add("e");
            tempCoordinates.Add((currentPos + translationEast));
            node.actions = tempAction;
            node.actionCoordinates = tempCoordinates;
            node.cost = 1;
            successors.Add(node);
        }
        if (!OccupiedTried((currentPos + translationWest), west))
        {
            SuccessorNode node = new SuccessorNode();
            node.state = (currentPos + translationWest);
            ArrayList tempAction = new ArrayList();
            ArrayList tempCoordinates = new ArrayList();
            tempAction.Add("w");
            tempCoordinates.Add((currentPos + translationWest));
            node.actions = tempAction;
            node.actionCoordinates = tempCoordinates;
            node.cost = 1;
            successors.Add(node);
        }

        return successors;
    }

    public float evaluationFunction(Vector3 pacmanPos, Vector3 ghostPos,float score)
    {
        Vector2 pacman = new Vector2(pacmanPos.x,pacmanPos.y);
        Vector2 ghost = new Vector2(ghostPos.x,ghostPos.y);
        float distance = manhattanDistance(pacman,ghost);
        
        return -distance/4f ;
    }

    public ArrayList globalMinimax(int player,int depth, bool maximizing, bool start, Vector3 ghostPos,bool pruning, float alpha, float beta)
    {
        SuccessorNode currentNode;
        currentNode = new SuccessorNode();
        currentNode.state = rigidbody.position;
        currentNode.actions = new ArrayList();
        score2 = -9999f;

        return minimax(currentNode,player,depth,maximizing,start,ghostPos, pruning, alpha, beta);

    }

    

    public ArrayList minimax(SuccessorNode state, int player,int depth, bool maximizing, bool start, Vector3 ghostPos, bool pruning, float alpha, float beta){
        //startNode var
        if(depth < 2){
            if(start){
                if( maximizing){
                    return maximize(state,depth,0,ghostPos, pruning, alpha, beta);
                }else{
                    return minimize(state,depth,1,ghostPos, pruning, alpha, beta);
                }
            }else{
                if( maximizing){
                    return maximize(state,depth,0,ghostPos, pruning, alpha, beta);
                }else{
                    return minimize(state,depth,1,ghostPos, pruning, alpha, beta);
                }
            }
        }
        else{
            ArrayList r = new ArrayList();
            totalScore= evaluationFunction(state.state, ghostPos,totalScore);
            r.Add(totalScore);
            r.Add("f");
            return r;
        }
        
    }
    public ArrayList maximize (SuccessorNode stateNode, int depth, int player, Vector3 ghostPos, bool pruning, float alpha, float beta)
    {
        Debug.Log("Maxixmize in");
        string chosenAction = "";
        float value = -9999f;
        int nextAgent = 1;
        float score;
        ArrayList items;
        int currDepth = depth+1;

        ArrayList successorsTemp = generateSuccessor(stateNode.state);
        foreach(SuccessorNode n in successorsTemp){
            items = minimax(n, nextAgent, currDepth ,false,false,ghostPos, pruning, alpha, beta);

            Debug.Log("ITEMS: " + items[0]);
            Debug.Log("Type check: "+ (items[0] is double));

            score = Mathf.Round((float)items[0]);
            
            Debug.Log("ITEMS 2: " + score);


            if(score >= value){
                value = score;
                score2 += score;
                chosenAction = (string)n.actions[0];
            }

            if(pruning){
                alpha = Mathf.Max(alpha,value);
                if(alpha > beta)
                {
                    break;
                }
            }
        }

        ArrayList result = new ArrayList();
        result.Add(value);
        result.Add(chosenAction);

        return result;

        
    }

    //0 to represent pacman
    //1 to represent ghost
    public ArrayList minimize (SuccessorNode stateNode, int depth, int player,Vector3 ghostPos, bool pruning, float alpha, float beta){
        Debug.Log("Minimize in");
        string chosenAction = "";
        float value = 9999f;
        int nextAgent = 0;
        float score = 0;
        ArrayList items;

        ArrayList successorsTemp = generateSuccessor(ghostPos);
        foreach(SuccessorNode n in successorsTemp){
            items = minimax(stateNode, nextAgent, depth+1,true,false,n.state, pruning, alpha, beta);

            score = Mathf.Round((float)items[0]);

            if(score <= value){
                value = score;
                score2 += score;
                chosenAction = (string)n.actions[0];
            }
            if(pruning){

                beta = Mathf.Min(beta,value);
                if(beta < alpha)
                {
                    break;
                }
            }
        }

        ArrayList result = new ArrayList();
        result.Add(value);
        result.Add(chosenAction);

        return result;

    }

    
    private void Start()
    {
        ResetState();
    }

    public void ResetState()
    {
        speedMultiplier = 1f;
        direction = initialDirection;
        nextDirection = Vector2.zero;
        transform.position = startingPosition;
        rigidbody.isKinematic = false;
        enabled = true;
    }

    private void Update()
    {
        // Try to move in the next direction while it's queued to make movements
        // more responsive
        if (nextDirection != Vector2.zero) {
            SetDirection(nextDirection);
        }
    }

    private void FixedUpdate()
    {
        /*
        Vector2 position = rigidbody.position;
        Vector2 translation = direction * speed * speedMultiplier * Time.fixedDeltaTime;

        rigidbody.MovePosition(position + translation);
        */
    }

        public void SetDirection(Vector2 direction, bool forced = false)
    {
        // Only set the direction if the tile in that direction is available
        // otherwise we set it as the next direction so it'll automatically be
        // set when it does become available
        if (forced || !Occupied(direction))
        {
            this.direction = direction;
            nextDirection = Vector2.zero;

        }
        else
        {
            nextDirection = direction;
        }

        Vector2 position = rigidbody.position;
        Vector2 translation = direction;


        rigidbody.MovePosition(position + translation);

        // for(int i = 0; i< eaten.Length; i++){
        //     float xCoordCurrent = Mathf.Round((position + translation).x);
        //     float yCoordCurrent = Mathf.Round((position + translation).y);
        //     float xCoordGoal = Mathf.Round(foodPellete[i].x);
        //     float yCoordGoal = Mathf.Round(foodPellete[i].y);
        //     if(Mathf.Approximately(xCoordCurrent, xCoordGoal) && Mathf.Approximately(yCoordCurrent, yCoordGoal))
        //     {
        //         eaten[i] = true;
        //     }

        //     Debug.Log("Food pelette "+ eaten[i]);
        // }

    }


    public bool Occupied(Vector2 direction)
    {
        // If no collider is hit then there is no obstacle in that direction
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, Vector2.one * 0.75f, 0f, direction, 1.5f, obstacleLayer);
        return hit.collider != null;
    }

}