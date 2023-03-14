using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct CheckersPiece {
    public bool IsWhite;
    public bool IsKing;
    public bool IsAlive;
    public Vector2Int position;
    public CheckersPiece(bool isWhite, bool isKing, bool isAlive, Vector2Int position) 
    {
        this.IsWhite = isWhite;
        this.IsKing = isKing;
        this.IsAlive = isAlive;
        this.position = position;
    }
}

public class GameLogic : MonoBehaviour
{
    CheckersPiece[,] board;
    // Start is called before the first frame update
    void Start()
    {
        board = new CheckersPiece[8, 8];
        InitBoard();
        PrintBoard();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    CheckersPiece GetPieceAt(Vector2Int position)
    {
        return board[position.x, position.y];
    }
    Vector2Int[] CheckMovement(CheckersPiece current)
    {
        Vector2Int currentPos = current.position;

        CheckersPiece empty; //set the empty piece
        empty.IsAlive = false;
        empty.IsKing = false;
        empty.IsWhite = false;
        empty.position = new Vector2Int(-1, -1);

        bool includesCapture = false; // used to force player to capture if there's a capture
        List<Vector2Int> movePositions = new List<Vector2Int>(); // set of normal directional moves that a piece can make
        List<Vector2Int> capturePositions = new List<Vector2Int>(); // set of capturing moves a piece can make

        Vector2Int enemyRight = new Vector2Int(-1,-1); // used to store an up-right enemy's piece's position
        Vector2Int enemyLeft = new Vector2Int(-1, -1); // used to store an up-left enemy's piece's position

        int flipVert = 1;
        if (!current.IsWhite) flipVert = -1; // flips which way is forward based on if a piece is white or black

        //check if diagonals are empty and not beyond the edge
        if (currentPos.x + 1 < 7 && currentPos.y + (1*flipVert) < 8 && currentPos.y + (1*flipVert) >= 0)// right side not on edge
        { 
            if (board[(currentPos.x + 1), (currentPos.y + (1 * flipVert))].position == empty.position) //up right empty
            {
                //The position up-right of current piece is empty
                movePositions.Add(new Vector2Int((currentPos.x + 1), (currentPos.y + (1 * flipVert))));

            } else { // up right has enemy
                if (board[(currentPos.x + 1), (currentPos.y + (1 * flipVert))].IsWhite != current.IsWhite)
                {
                    enemyRight = new Vector2Int(currentPos.x+1, currentPos.y + (1 * flipVert));
                }
            }
        }
        if(currentPos.x - 1 > 0 && currentPos.y + (1*flipVert) < 8 && currentPos.y + (1*flipVert) >= 0) //left side not on edge
        {
            if (board[(currentPos.x - 1), (currentPos.y + (1 * flipVert))].position == empty.position)
            {
                //The position up-left of current piece is empty
                movePositions.Add(new Vector2Int((currentPos.x - 1), (currentPos.y + (1 * flipVert))));

            } else {
                if (board[(currentPos.x - 1), (currentPos.y + (1 * flipVert))].IsWhite != current.IsWhite)
                {
                    enemyLeft = new Vector2Int(currentPos.x-1, currentPos.y + (1 * flipVert));
                }
            }
        }
        if (enemyRight != new Vector2Int(-1,-1))
        {
            while (getEnemyCapturable("right", enemyRight))
            {
                includesCapture = true;
                capturePositions.Add(enemyRight + new Vector2Int(1, flipVert));
                enemyRight = (enemyRight + new Vector2Int(2, (2*flipVert)));
            }
        }
        if (enemyLeft != new Vector2Int(-1,-1))
        {
            while (getEnemyCapturable("left", enemyLeft))
            {
                includesCapture = true;
                capturePositions.Add(enemyLeft + new Vector2Int(-1, flipVert));
                enemyLeft = (enemyLeft + new Vector2Int(-2, (2*flipVert)));
            }
        }

        bool getEnemyCapturable(string direction, Vector2Int position)
        {
            if (board[position.x, position.y].position == empty.position || board[position.x, position.y].IsWhite == current.IsWhite)
            {
                return false;
            }

            int directionVal = 1;
            if (direction == "left") directionVal = -1;

            if (position != new Vector2Int(-1,-1) && position.y + (1*flipVert) < 8 && position.y + (1*flipVert) >= 0)
            {
                if (board[(position.x + (1*directionVal)), (position.y + (1 * flipVert))].position == empty.position)
                {
                    //The position through enemy piece is empty
                    return true;
                }
            }
            return false;
        }
        
        if (!includesCapture)
        {
            return movePositions.ToArray();
        } else {
            return capturePositions.ToArray();
        }

    }
    void PrintBoard()
    {
        CheckersPiece empty;
        empty.IsAlive = false;
        empty.IsKing = false;
        empty.IsWhite = false;
        empty.position = new Vector2Int(-1, -1);
        string toPrint = "";
        
        for (int i=0; i<8; i++)
        {
            for (int j=0; j<8; j++)
            {
                if (board[i,j].position == empty.position)
                {
                    toPrint += "[_]";
                    continue;
                }

                if (board[i,j].IsWhite == true)
                {
                    toPrint += "[W]";
                } else {
                    toPrint += "[B]";
                }
            }
            toPrint += "\n";
        }
        Debug.Log(toPrint);

        return;
    }
    void InitBoard()
    {
        CheckersPiece current;
        CheckersPiece empty;
        empty.IsAlive = false;
        empty.IsKing = false;
        empty.IsWhite = false;
        empty.position = new Vector2Int(-1, -1);
        for (int i=0; i<2; i++)
        {
            for (int j=0; j<8; j++)
            {   
                if (i%2 == 0 && j%2 == 0 || i%2 == 1 && j%2 ==1)
                {
                    current.IsWhite = true;
                    current.IsAlive = true;
                    current.IsKing = false;
                    current.position = new Vector2Int(i,j);

                    board[i,j] = current;
                } else {
                    board[i,j] = empty;
                }
            }
        }
        for (int i=2; i<5; i++)
        {
            for (int j=0; j<8; j++)
            {
                board[i,j] = empty;
            }
        }
        for (int i=5; i<8; i++)
        {
            for (int j=0; j<8; j++)
            {   
                if (i%2 == 1 && j%2 == 0 || i%2 == 0 && j%2 ==1)
                {
                    current.IsWhite = false;
                    current.IsAlive = true;
                    current.IsKing = true;
                    current.position = new Vector2Int(i,j);

                    board[i,j] = current;
                } else {
                    board[i,j] = empty;
                }
            }
        }
        return;
    }
}
