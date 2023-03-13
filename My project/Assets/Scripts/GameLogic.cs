using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct CheckersPiece {
    public bool IsWhite;
    public bool IsAlive;
    public Vector2Int position;
    public CheckersPiece(bool isWhite, bool IsAlive, Vector2Int position) 
    {
        this.IsWhite = isWhite;
        this.IsAlive = IsAlive;
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
    void PrintBoard()
    {
        CheckersPiece empty;
        empty.IsAlive = false;
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
