using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using System;

[RequireComponent(typeof(Rigidbody2D))]
public class Movement : MonoBehaviour
{
    public float speed = 1f;
    public float speedMultiplier = 2f;
    public Vector2 initialDirection;
    public LayerMask obstacleLayer;

    public new Rigidbody2D rigidbody { get; private set; }
    public Vector2 direction { get; private set; }
    public Vector2 nextDirection { get; private set; }
    public Vector3 startingPosition { get; private set; }

    public Vector2 goalPosition;

    public Tilemap tilemap;
    public TileBase[] allTiles { get; private set; }

    public Vector2 goalState = new Vector2(-1.5f, 3.1f);

    public GameObject rightPref;
    public GameObject leftPref;
    public GameObject downPref;
    public GameObject upPref;

    public ArrayList visitedCoordinates;
    public int expandedNodes;
    public float score2;

    public Vector3[] foodPellete;
    public bool[] eaten = new bool[9];
    public GameObject pellet;
    public int miniMaxExpandedNodes;


    Vector2[] corners = { new Vector2(-6, 6), new Vector2(6, 6), new Vector2(-6, -6), new Vector2(6, -6) };

    public Vector3 getPacmanPosition(){
        return rigidbody.position;
    }

    public struct SuccessorNode
    {
        public Vector2 state;
        public ArrayList actions;
        public ArrayList actionCoordinates;
        public int cost;
        public float successorScore;

        public bool[] visited;
    }

    SuccessorNode startNode;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        startingPosition = transform.position;
        BoundsInt bounds = tilemap.cellBounds;
        allTiles = tilemap.GetTilesBlock(bounds);
        visitedCoordinates = new ArrayList();
        startNode = new SuccessorNode();
        startNode.state = startingPosition;
        startNode.actions = new ArrayList();
        startNode.actionCoordinates = new ArrayList();
        startNode.cost = 0;
        startNode.successorScore = 0f;
        expandedNodes = 0;
        score2 = 0;
        miniMaxExpandedNodes = 0;

        for(int i = 0; i < foodPellete.Length; i++){
            Instantiate(pellet, foodPellete[i],Quaternion.identity);
            eaten[i] = false;
        }

    }

    private void Start()
    {
        ResetState();
    }

    public void createPath(ArrayList path, ArrayList actions, string expansion, float gradient)
    {
        string coordinates = "";
        string actionsss = "";
        GameObject pathObj = null;
        pathObj = Instantiate(rightPref, new Vector3(startingPosition.x, startingPosition.y, 0), Quaternion.identity);
        pathObj.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 1);

        for (int i = 0; i < path.Count; i++)
        {
            coordinates += "(" + path[i] + "), ";
            actionsss += "(" + actions[i] + "), ";
            Vector2 coordinate = (Vector2)path[i];
            expandedNodes += 1;

            if ((string)actions[i] == "e" && (!visitedCoordinates.Contains(coordinate) || expansion == "last"))
            {
                pathObj = Instantiate(rightPref, new Vector3(coordinate.x, coordinate.y, 0), Quaternion.identity);
                if (expansion == "last")
                {
                    pathObj.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 1);
                }
                else
                {
                    pathObj.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, gradient);
                    Debug.Log("COLORRR");
                }
                visitedCoordinates.Add(coordinate);
            }
            else if ((string)actions[i] == "w" && (!visitedCoordinates.Contains(coordinate) || expansion == "last"))
            {
                pathObj = Instantiate(leftPref, new Vector3(coordinate.x, coordinate.y, 0), Quaternion.identity);
                if (expansion == "last")
                {
                    pathObj.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 1);
                }
                else
                {
                    pathObj.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, gradient);
                    Debug.Log("COLORRR");
                }
                visitedCoordinates.Add(coordinate);
            }
            else if ((string)actions[i] == "n" && (!visitedCoordinates.Contains(coordinate) || expansion == "last"))
            {
                pathObj = Instantiate(upPref, new Vector3(coordinate.x, coordinate.y, 0), Quaternion.identity);
                if (expansion == "last")
                {
                    pathObj.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 1);
                }
                else
                {
                    pathObj.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, gradient);
                    Debug.Log("COLORRR");
                }
                visitedCoordinates.Add(coordinate);
            }
            else if ((string)actions[i] == "s" && (!visitedCoordinates.Contains(coordinate) || expansion == "last"))
            {
                pathObj = Instantiate(downPref, new Vector3(coordinate.x, coordinate.y, 0), Quaternion.identity);
                if (expansion == "last")
                {
                    pathObj.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 1);
                }
                else
                {
                    pathObj.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, gradient);
                    Debug.Log("COLORRR");
                }
                visitedCoordinates.Add(coordinate);
            }

        }
        Debug.Log("==========");
        Debug.Log("COORDINATES: " + coordinates);
        Debug.Log("Actions: " + actionsss);
        Debug.Log("No Of Coordinates: " + path.Count);
        Debug.Log("No Of Actions: " + actions.Count);
        Debug.Log("==========");
        if (expansion == "last"){
            Debug.Log("Expanded Nodes: " + expandedNodes);
        }
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
            node.successorScore = -2f;
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
            node.successorScore = -2f;
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
            node.successorScore = -2f;
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
            node.successorScore = -2f;
            successors.Add(node);
        }

        return successors;
    }

    public ArrayList depthFirstSearch()
    {
        Stack<SuccessorNode> myStack = new Stack<SuccessorNode>();
        ArrayList seen = new ArrayList();

        Vector2 current = new Vector2((float)startingPosition.x, (float)startingPosition.y);
        ArrayList actions = new ArrayList();
        ArrayList pathCoordinates = new ArrayList();

        int cost = 0;
        SuccessorNode node = new SuccessorNode();
        node.state = current;
        node.actions = actions;
        node.cost = cost;
        float gradient = 0;

        myStack.Push(node);
        int countW = 0;

        while (myStack.Count != 0)
        {
            countW++;
            gradient += 0.003f; //Small maze icin
            //gradient += 0.001f; //Medium maze icin
            SuccessorNode successors = myStack.Pop();
            actions = new ArrayList();
            pathCoordinates = new ArrayList();
            current = successors.state;
            Debug.Log("actions count " + successors.actions.Count);
            for (int i = 0; i < successors.actions.Count; i++)
            {
                actions.Add((string)successors.actions[i]);
                pathCoordinates.Add((Vector2)successors.actionCoordinates[i]);
            }

            cost = successors.cost;

            Debug.Log("Before seen");
            Debug.Log("Stack count: " + myStack.Count);


            if (!seen.Contains(current))
            {
                Debug.Log("not In seen");
                seen.Add(current);


                bool equalityX = Mathf.Approximately(current.x, goalPosition.x);
                bool equalityY = Mathf.Approximately(current.y, goalPosition.y);

                bool equality = equalityX && equalityY;

                Debug.Log("x coor: " + Mathf.Round(current.x * 10.0f) * 0.1f);

                float xCoordCurrent = Mathf.Round(current.x);
                float yCoordCurrent = Mathf.Round(current.y);
                float xCoordGoal = Mathf.Round(goalPosition.x);
                float yCoordGoal = Mathf.Round(goalPosition.y);

                Debug.Log("x coordinates check" + (Mathf.Approximately(xCoordCurrent, xCoordGoal)));

                if (!(Mathf.Approximately(xCoordCurrent, xCoordGoal) && Mathf.Approximately(yCoordCurrent, yCoordGoal)))
                {
                    Debug.Log("In if statement");
                    ArrayList successorsTemp = generateSuccessor(current);
                    foreach (SuccessorNode n in successorsTemp)
                    {
                        Debug.Log("in foreach statement");
                        ArrayList nodeActions = new ArrayList();
                        SuccessorNode tempNode = new SuccessorNode();
                        tempNode.actions = new ArrayList();
                        tempNode.actionCoordinates = new ArrayList();

                        for (int i = 0; i < actions.Count; i++)
                        {
                            tempNode.actions.Add((string)actions[i]);
                            tempNode.actionCoordinates.Add((Vector2)pathCoordinates[i]);
                        }
                        tempNode.state = new Vector2((float)n.state.x, (float)n.state.y);
                        tempNode.actions.Add((string)n.actions[0]);
                        tempNode.actionCoordinates.Add((Vector2)n.actionCoordinates[0]);
                        tempNode.cost = 0;
                        myStack.Push(tempNode);
                        countW++;


                    }
                    Debug.Log("Current pos: " + current);
                    Debug.Log("Goal pos: " + goalPosition);

                }
                else
                {
                    Debug.Log("End");
                    actions.Add("f");
                    createPath(pathCoordinates, actions, "last", 255);
                    return actions;
                }
            }
            else
            {
                Debug.Log("in seen");
            }
            createPath(pathCoordinates, actions, "not", gradient);
        }

        Debug.Log("out of loop");
        createPath(pathCoordinates, actions, "not", gradient);
        return actions;

    }

    public ArrayList breadthFirstSearch()
    {
        Queue<SuccessorNode> myQueue = new Queue<SuccessorNode>();
        ArrayList seen = new ArrayList();

        Vector2 current = new Vector2((float)startingPosition.x, (float)startingPosition.y);
        ArrayList actions = new ArrayList();
        ArrayList pathCoordinates = new ArrayList();
        float gradient = 0;
        int cost = 0;
        SuccessorNode node = new SuccessorNode();
        node.state = current;
        node.actions = actions;
        node.cost = cost;

        myQueue.Enqueue(node);
        int countW = 0;

        while (myQueue.Count != 0)
        {
            countW++;
            gradient += 0.003f; //Small maze icin
            //gradient += 0.001f; //Medium maze icin
            SuccessorNode successors = myQueue.Dequeue();
            actions = new ArrayList();
            pathCoordinates = new ArrayList();
            current = successors.state;
            Debug.Log("actions count " + successors.actions.Count);
            for (int i = 0; i < successors.actions.Count; i++)
            {
                actions.Add((string)successors.actions[i]);
                pathCoordinates.Add((Vector2)successors.actionCoordinates[i]);
            }

            cost = successors.cost;

            Debug.Log("Before seen");
            Debug.Log("Stack count: " + myQueue.Count);


            if (!seen.Contains(current))
            {
                Debug.Log("not In seen");
                seen.Add(current);


                bool equalityX = Mathf.Approximately(current.x, goalPosition.x);
                bool equalityY = Mathf.Approximately(current.y, goalPosition.y);

                bool equality = equalityX && equalityY;

                Debug.Log("x coor: " + Mathf.Round(current.x * 10.0f) * 0.1f);

                float xCoordCurrent = Mathf.Round(current.x);
                float yCoordCurrent = Mathf.Round(current.y);
                float xCoordGoal = Mathf.Round(goalPosition.x);
                float yCoordGoal = Mathf.Round(goalPosition.y);

                Debug.Log("x coordinates check" + (Mathf.Approximately(xCoordCurrent, xCoordGoal)));

                if (!(Mathf.Approximately(xCoordCurrent, xCoordGoal) && Mathf.Approximately(yCoordCurrent, yCoordGoal)))
                {
                    Debug.Log("In if statement");
                    ArrayList successorsTemp = generateSuccessor(current);
                    foreach (SuccessorNode n in successorsTemp)
                    {
                        Debug.Log("in foreach statement");
                        ArrayList nodeActions = new ArrayList();
                        SuccessorNode tempNode = new SuccessorNode();
                        tempNode.actions = new ArrayList();
                        tempNode.actionCoordinates = new ArrayList();

                        for (int i = 0; i < actions.Count; i++)
                        {
                            tempNode.actions.Add((string)actions[i]);
                            tempNode.actionCoordinates.Add((Vector2)pathCoordinates[i]);
                        }
                        tempNode.state = new Vector2((float)n.state.x, (float)n.state.y);
                        tempNode.actions.Add((string)n.actions[0]);
                        tempNode.actionCoordinates.Add((Vector2)n.actionCoordinates[0]);
                        tempNode.cost = 0;
                        myQueue.Enqueue(tempNode);
                        countW++;


                    }
                    Debug.Log("Current pos: " + current);
                    Debug.Log("Goal pos: " + goalPosition);

                }
                else
                {
                    Debug.Log("End");
                    actions.Add("f");
                    createPath(pathCoordinates, actions, "last", 255);
                    return actions;
                }
            }
            else
            {
                Debug.Log("in seen");
            }
            createPath(pathCoordinates, actions, "not", gradient);
        }

        Debug.Log("out of loop");
        createPath(pathCoordinates, actions, "not", gradient);
        return actions;

    }

    public ArrayList UniformCostSearch()
    {
        PriorityQueue<SuccessorNode> myStack = new PriorityQueue<SuccessorNode>(true);
        ArrayList seen = new ArrayList();

        Vector2 current = new Vector2((float)startingPosition.x, (float)startingPosition.y);
        ArrayList actions = new ArrayList();
        ArrayList pathCoordinates = new ArrayList();
        float gradient = 0;
        int cost = 0;
        SuccessorNode node = new SuccessorNode();
        node.state = current;
        node.actions = actions;
        node.cost = cost;

        myStack.Enqueue(node.cost, node);
        int countW = 0;

        while (myStack.Count != 0)
        {
            countW++;
            gradient += 0.003f; //Small maze icin
            //gradient += 0.001f; //Medium maze icin
            SuccessorNode successors = myStack.Dequeue();
            actions = new ArrayList();
            pathCoordinates = new ArrayList();
            current = successors.state;
            cost = 0;
            Debug.Log("actions count " + successors.actions.Count);
            for (int i = 0; i < successors.actions.Count; i++)
            {
                actions.Add((string)successors.actions[i]);
                cost += cost + successors.cost;
                pathCoordinates.Add((Vector2)successors.actionCoordinates[i]);
            }

            Debug.Log("Before seen");
            Debug.Log("Stack count: " + myStack.Count);


            if (!seen.Contains(current))
            {
                Debug.Log("not In seen");
                seen.Add(current);


                bool equalityX = Mathf.Approximately(current.x, goalPosition.x);
                bool equalityY = Mathf.Approximately(current.y, goalPosition.y);

                bool equality = equalityX && equalityY;

                Debug.Log("x coor: " + Mathf.Round(current.x * 10.0f) * 0.1f);

                float xCoordCurrent = Mathf.Round(current.x);
                float yCoordCurrent = Mathf.Round(current.y);
                float xCoordGoal = Mathf.Round(goalPosition.x);
                float yCoordGoal = Mathf.Round(goalPosition.y);

                Debug.Log("x coordinates check" + (Mathf.Approximately(xCoordCurrent, xCoordGoal)));

                if (!(Mathf.Approximately(xCoordCurrent, xCoordGoal) && Mathf.Approximately(yCoordCurrent, yCoordGoal)))
                {
                    Debug.Log("In if statement");
                    ArrayList successorsTemp = generateSuccessor(current);
                    foreach (SuccessorNode n in successorsTemp)
                    {
                        Debug.Log("in foreach statement");
                        ArrayList nodeActions = new ArrayList();
                        SuccessorNode tempNode = new SuccessorNode();
                        tempNode.actions = new ArrayList();
                        tempNode.actionCoordinates = new ArrayList();

                        for (int i = 0; i < actions.Count; i++)
                        {
                            tempNode.actions.Add((string)actions[i]);
                            tempNode.actionCoordinates.Add((Vector2)pathCoordinates[i]);
                            tempNode.cost += 1;
                        }
                        tempNode.state = new Vector2((float)n.state.x, (float)n.state.y);
                        tempNode.actions.Add((string)n.actions[0]);
                        tempNode.actionCoordinates.Add((Vector2)n.actionCoordinates[0]);
                        tempNode.cost += n.cost;
                        myStack.Enqueue(tempNode.cost, tempNode);
                        countW++;


                    }
                    Debug.Log("Current pos: " + current);
                    Debug.Log("Goal pos: " + goalPosition);

                }
                else
                {
                    Debug.Log("End");
                    actions.Add("f");
                    createPath(pathCoordinates, actions, "last", 255);
                    return actions;
                }
            }
            else
            {
                Debug.Log("in seen");
            }
            createPath(pathCoordinates, actions, "not", gradient);
        }

        Debug.Log("out of loop");
        createPath(pathCoordinates, actions, "not", gradient);
        return actions;
    }

    public ArrayList AStarSearch()
    {
        PriorityQueue<SuccessorNode> myStack = new PriorityQueue<SuccessorNode>(true);
        ArrayList seen = new ArrayList();

        Vector2 current = new Vector2((float)startingPosition.x, (float)startingPosition.y);
        ArrayList actions = new ArrayList();
        ArrayList pathCoordinates = new ArrayList();
        float gradient = 0;
        int cost = 0;
        SuccessorNode node = new SuccessorNode();
        node.state = current;
        node.actions = actions;
        node.cost = cost;

        myStack.Enqueue(node.cost, node);
        int countW = 0;

        while (myStack.Count != 0)
        {
            countW++;
            gradient += 0.003f; //Small maze icin
            //gradient += 0.001f; //Medium maze icin
            SuccessorNode successors = myStack.Dequeue();
            actions = new ArrayList();
            pathCoordinates = new ArrayList();
            current = successors.state;
            cost = 0;
            Debug.Log("actions count " + successors.actions.Count);
            for (int i = 0; i < successors.actions.Count; i++)
            {
                actions.Add((string)successors.actions[i]);
                cost += cost + successors.cost;
                pathCoordinates.Add((Vector2)successors.actionCoordinates[i]);
            }

            Debug.Log("Before seen");
            Debug.Log("Stack count: " + myStack.Count);


            if (!seen.Contains(current))
            {
                Debug.Log("not In seen");
                seen.Add(current);


                bool equalityX = Mathf.Approximately(current.x, goalPosition.x);
                bool equalityY = Mathf.Approximately(current.y, goalPosition.y);

                bool equality = equalityX && equalityY;

                Debug.Log("x coor: " + Mathf.Round(current.x * 10.0f) * 0.1f);

                float xCoordCurrent = Mathf.Round(current.x);
                float yCoordCurrent = Mathf.Round(current.y);
                float xCoordGoal = Mathf.Round(goalPosition.x);
                float yCoordGoal = Mathf.Round(goalPosition.y);

                Debug.Log("x coordinates check" + (Mathf.Approximately(xCoordCurrent, xCoordGoal)));

                if (!(Mathf.Approximately(xCoordCurrent, xCoordGoal) && Mathf.Approximately(yCoordCurrent, yCoordGoal)))
                {
                    Debug.Log("In if statement");
                    ArrayList successorsTemp = generateSuccessor(current);
                    foreach (SuccessorNode n in successorsTemp)
                    {
                        Debug.Log("in foreach statement");
                        ArrayList nodeActions = new ArrayList();
                        SuccessorNode tempNode = new SuccessorNode();
                        tempNode.actions = new ArrayList();
                        tempNode.actionCoordinates = new ArrayList();

                        for (int i = 0; i < actions.Count; i++)
                        {
                            tempNode.actions.Add((string)actions[i]);
                            tempNode.actionCoordinates.Add((Vector2)pathCoordinates[i]);
                            tempNode.cost += 1;
                        }
                        tempNode.state = new Vector2((float)n.state.x, (float)n.state.y);
                        tempNode.actions.Add((string)n.actions[0]);
                        tempNode.actionCoordinates.Add((Vector2)n.actionCoordinates[0]);
                        tempNode.cost += n.cost;
                        tempNode.cost += manhattanDistance(n.state, goalPosition);
                        myStack.Enqueue(tempNode.cost, tempNode);
                        countW++;


                    }
                    Debug.Log("Current pos: " + current);
                    Debug.Log("Goal pos: " + goalPosition);

                }
                else
                {
                    Debug.Log("End");
                    actions.Add("f");
                    createPath(pathCoordinates, actions, "last", 255);
                    return actions;
                }
            }
            else
            {
                Debug.Log("in seen");
            }
            createPath(pathCoordinates, actions, "not", gradient);
        }

        Debug.Log("out of loop");
        createPath(pathCoordinates, actions, "not", gradient);
        return actions;
    }

    public int nullheuristic(){
        return 0;
    }

    public int eucledianHeuristic(Vector2 currentPos, Vector2 goalPosition){
        Debug.Log("Man distance: " + Math.Round(Math.Sqrt((Math.Pow(currentPos.x - goalPosition.x, 2) + Math.Pow(currentPos.y - goalPosition.y, 2)))));
        return (int) Math.Round(Math.Sqrt((Math.Pow(currentPos.x - goalPosition.x, 2) + Math.Pow(currentPos.y - goalPosition.y, 2))));
    }

    public int cornerHeuristic(Vector2 state, bool[] unvisitedCorners)
    {
        //corners: Top Left: -6,6; Top Right: 6,6; Bottom Left: -6,-6; Bottom Right: 6,-6

        int heuristic = 0;
        int closestPoint = 0;
        int closestPointCost = 0;
        int lenghtToCorner = 0;
        Vector2 cornerLocation = new Vector2(0, 0);
        Vector2 closestNode = new Vector2(0, 0);
        Stack visited = new Stack();
        visited.Push(0);
        visited.Push(1);
        visited.Push(2);
        visited.Push(3);
        for (int i = 0; i < 4; i++)
        {
            visited.Pop();
            if (unvisitedCorners[i])
            {
                closestPointCost = manhattanDistance(state, corners[i]);
                closestPoint = 1;

                for (int j = 0; j < visited.Count; j++)
                {
                    if (unvisitedCorners[j])
                    {
                        cornerLocation = corners[j];
                        lenghtToCorner = manhattanDistance(state, cornerLocation);

                        if (lenghtToCorner <= closestPointCost)
                        {
                            closestPoint = j;
                            closestPointCost = lenghtToCorner;
                        }
                    }
                }
                if (closestPointCost != 0)
                {
                    closestNode = corners[closestPoint];
                    heuristic += mazeDistance(closestNode, state); //needs work probably
                    unvisitedCorners[closestPoint] = false;

                    state = closestNode;
                }
            }
        }
        return heuristic;
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

    public int mazeDistance(Vector2 a, Vector2 b)
    {
        return 0;
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

    

     //isWin()
    public bool isWin(){
        float xCoordCurrent = Mathf.Round(rigidbody.position.x);
        float yCoordCurrent = Mathf.Round(rigidbody.position.y);
        float xCoordGoal = Mathf.Round(goalPosition.x);
        float yCoordGoal = Mathf.Round(goalPosition.y);
        if((Mathf.Approximately(xCoordCurrent, xCoordGoal) && Mathf.Approximately(yCoordCurrent, yCoordGoal))){
            return true;
        }
        return false;
    }

    public bool isAllFoodEaten(){
        bool foodFound = true;
        
        for(int i = 0; i < eaten.Length; i++){
            if(!eaten[i]){
                foodFound = false;
            }
        }

        Debug.Log("Minimax Expanded nodes: " + miniMaxExpandedNodes);
        return foodFound;
    }


     //isWin()
    public bool isInGoal(Vector3 pos){
        float xCoordCurrent = Mathf.Round(pos.x);
        float yCoordCurrent = Mathf.Round(pos.y);
        float xCoordGoal = Mathf.Round(goalPosition.x);
        float yCoordGoal = Mathf.Round(goalPosition.y);
        if((Mathf.Approximately(xCoordCurrent, xCoordGoal) && Mathf.Approximately(yCoordCurrent, yCoordGoal))){
            return true;
        }
        return false;
    }

    //isLose()
    public bool isLose(Vector3 ghostPos){
        float xCoordCurrent = Mathf.Round(rigidbody.position.x);
        float yCoordCurrent = Mathf.Round(rigidbody.position.y);
        float xCoordGoal = Mathf.Round(ghostPos.x);
        float yCoordGoal = Mathf.Round(ghostPos.y);

        if((Mathf.Approximately(xCoordCurrent, xCoordGoal) && Mathf.Approximately(yCoordCurrent, yCoordGoal))){
            return true;
        }
        return false;
    }

    
    public float evaluationFunction(Vector3 pacmanPos, Vector3 ghostPos,float score)
    {
        miniMaxExpandedNodes++;
        Vector2 pacman = new Vector2(pacmanPos.x,pacmanPos.y);
        Vector2 ghost = new Vector2(ghostPos.x,ghostPos.y);
        float distance = manhattanDistance(pacman,ghost);
        float goalDistance = manhattanDistance(pacman,goalPosition);
        float minFoodDistance = 9999f;
        bool foundFood = false; 
        for(int i = 0; i < eaten.Length; i++){
            if(eaten[i] != true)
            {
                foundFood = true;
                float tempFoodDistance = manhattanDistance(new Vector2(foodPellete[i].x, foodPellete[i].y), pacman);
                if(tempFoodDistance != 0){
                    if( tempFoodDistance <= minFoodDistance){
                        minFoodDistance = tempFoodDistance;
                        score -= 5f; 
                    }  
                }else{
                    return 10f;
                }
            }
        } 
        if(!foundFood){
            return 500f;
        }
        if(distance < 2){
            return -9999f;
        }


        return (float)(score + (-3/distance) + (3/minFoodDistance));
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
            state.successorScore = evaluationFunction(state.state, ghostPos,state.successorScore);
            r.Add(state.successorScore);
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

     public ArrayList globalExpectimize(int player,int depth, bool maximizing, bool start, Vector3 ghostPos,bool pruning, float alpha, float beta)
    {
        SuccessorNode currentNode;
        currentNode = new SuccessorNode();
        currentNode.state = rigidbody.position;
        currentNode.actions = new ArrayList();
        score2 = -9999f;

        return expectimax(currentNode,player,depth,maximizing,start,ghostPos, pruning, alpha, beta);

    }

    

    public ArrayList expectimax(SuccessorNode state, int player,int depth, bool maximizing, bool start, Vector3 ghostPos, bool pruning, float alpha, float beta){
        //startNode var
        if(depth < 3){
            if(start){
                if( maximizing){
                    return maximizeValue(state,depth,0,ghostPos, pruning, alpha, beta);
                }else{
                    return average(state,depth,1,ghostPos, pruning, alpha, beta);
                }
            }else{
                if( maximizing){
                    return maximizeValue(state,depth,0,ghostPos, pruning, alpha, beta);
                }else{
                    return average(state,depth,1,ghostPos, pruning, alpha, beta);
                }
            }
        }
        else{
            ArrayList r = new ArrayList();
            state.successorScore = evaluationFunction(state.state, ghostPos,state.successorScore);
            r.Add(state.successorScore);
            r.Add("f");
            return r;
        }
        
    }
    public ArrayList maximizeValue (SuccessorNode stateNode, int depth, int player, Vector3 ghostPos, bool pruning, float alpha, float beta)
    {
        Debug.Log("in maximizeValue");
        string chosenAction = "";
        float value = -9999f;
        int nextAgent = 1;
        float score;
        ArrayList items;
        int currDepth = depth+1;

        ArrayList successorsTemp = generateSuccessor(stateNode.state);
        foreach(SuccessorNode n in successorsTemp){
            items = expectimax(n, nextAgent, currDepth ,false,false,ghostPos, pruning, alpha, beta);

            Debug.Log("ITEMS: " + items[0]);
            Debug.Log("Type check: "+ (items[0] is double));

            score = Mathf.Round((float)items[0]);
            
            Debug.Log("ITEMS 2: " + score);


            if(score >= value){
                value = score;
                score2 += score;
                chosenAction = (string)n.actions[0];
            }
        }

        ArrayList result = new ArrayList();
        result.Add(value);
        result.Add(chosenAction);

        return result;

        
    }

    //0 to represent pacman
    //1 to represent ghost
    public ArrayList average (SuccessorNode stateNode, int depth, int player,Vector3 ghostPos, bool pruning, float alpha, float beta){
        Debug.Log("In average");
       string chosenAction = "";
        float value = 9999f;
        int nextAgent = 0;
        float score = 0;
        ArrayList items;

        ArrayList successorsTemp = generateSuccessor(ghostPos);
        foreach(SuccessorNode n in successorsTemp){
            items = expectimax(stateNode, nextAgent, depth+1,true,false,n.state, pruning, alpha, beta);
            score += Mathf.Round((float)items[0]);
        }

        value = score / successorsTemp.Count;

        ArrayList result = new ArrayList();
        result.Add(value);
        result.Add(chosenAction);

        return result;

    }

    private void Update()
    {
        // Try to move in the next direction while it's queued to make movements
        // more responsive
        if (nextDirection != Vector2.zero)
        {
            SetDirection(nextDirection);
        }
    }

    private void FixedUpdate()
    {
        /*
        Vector2 position = rigidbody.position;
        Vector2 translation = direction * 0.05f;



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

        score2 -= 2f;



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

    public void setEaten(Vector3 eatenFoodPos){
        for(int i = 0; i< eaten.Length; i++){
            float xCoordCurrent = Mathf.Round(eatenFoodPos.x);
            float yCoordCurrent = Mathf.Round(eatenFoodPos.y);
            float xCoordGoal = Mathf.Round(foodPellete[i].x);
            float yCoordGoal = Mathf.Round(foodPellete[i].y);
            if(Mathf.Approximately(xCoordCurrent, xCoordGoal) && Mathf.Approximately(yCoordCurrent, yCoordGoal))
            {
                eaten[i] = true;
            }

            Debug.Log("Food pelette "+ eaten[i]);
        }
    }

    public bool Occupied(Vector2 direction)
    {
        //Debug.Log("obstacle: " + direction);
        // If no collider is hit then there is no obstacle in that direction
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, Vector2.one * 0.75f, 0f, direction, 1.5f, obstacleLayer);
        return hit.collider != null;
    }

    public bool OccupiedTried(Vector2 nextPos, Vector2 direction)
    {
        //Debug.Log("obstacle: " + direction);
        // If no collider is hit then there is no obstacle in that direction
        Vector3 pos = new Vector3(nextPos.x, nextPos.y, 0);
        RaycastHit2D hit = Physics2D.BoxCast(pos, Vector2.one * 0.5f, 0f, direction, 1f, obstacleLayer);
        return hit.collider != null;
    }

    public bool willCollide(Vector2 pos)
    {
        Vector3 direc = new Vector3((float)pos.x, (float)pos.y, 0);
        Vector3Int tilePos = tilemap.WorldToCell(pos);
        Vector3Int d = Vector3Int.FloorToInt(direc);

        if (tilemap.GetTile(tilePos))
        {
            Debug.Log("tile");
        }
        else
        {
            Debug.Log("not tile");

        }


        return tilemap.GetTile(tilePos) != null;
    }

}