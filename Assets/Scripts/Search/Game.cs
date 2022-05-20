using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent
{
    int index;
    public Agent(int index)
    {
        this.index = index;
    }

    public void getAction()
    {

    }
}

public class Directions
{
    public Directions()
    {

    }
}

public class Configuration
{
    public Configuration()
    {

    }

    public void getPosition()
    {

    }

    public void getDirection()
    {

    }

    public void isInteger()
    {

    }

    public void checkEquality()
    {

    }

    public void hash()
    {

    }

    public void print()
    {

    }

    public void generateSuccessor()
    {

    }
}
public class AgentState
{
    public AgentState()
    {

    }

    public void print()
    {

    }
    public void checkEquality()
    {

    }

    public void hash()
    {

    }

    public void copy()
    {

    }
    public void getPosition()
    {

    }
    public void getDirection()
    {

    }
}

public class Grid
{
    public Grid()
    {

    }

    public void getItem()
    {

    }
    public void setItem()
    {

    }
    public void print()
    {

    }
    public void checkEquality()
    {

    }

    public void hash()
    {

    }

    public void copy()
    {

    }
    public void deepCopy()
    {

    }
    public void shallowCopy()
    {

    }
    public void count()
    {

    }
    public void asList()
    {

    }
    public void packBits()
    {

    }
    public void cellIndexToPosition()
    {

    }
    public void unpackBits()
    {

    }
}
/*def reconstituteGrid(bitRep):
    if type(bitRep) is not type((1, 2)):
        return bitRep
    width, height = bitRep[:2]
    return Grid(width, height, bitRepresentation=bitRep[2:])
    */

public class Actions
{
    public Actions()
    {

    }

    public void reverseDirection()
    {

    }

    public void vectorToDirection()
    {

    }

    public void directionToVector()
    {

    }
    public void getPossibleActions()
    {

    }
    public void getLegalNeighbors()
    {

    }
    public void getSuccessor()
    {

    }
}

public class GameStateDate
{
    public GameStateDate()
    {

    }
    public void print()
    {

    }
    public void checkEquality()
    {

    }

    public void hash()
    {

    }

    public void copy()
    {

    }
    public void deepCopy()
    {

    }
    public void copyAgentStates()
    {

    }
    public void foodWallStr()
    {

    }
    public void pacStr()
    {

    }
    public void ghostStr()
    {

    }
}

public class Game
{
    public Game()
    {

    }

    public void getProgress()
    {

    }

    public void agentCrash()
    {

    }
    public void mute()
    {

    }
    public void unmute()
    {

    }
    public void run()
    {

    }
}
