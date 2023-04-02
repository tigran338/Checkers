using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CheckersPiece {
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

public class GameLogic
{
    CheckersPiece[,] board;
    public CheckersPiece empty;
    public bool turnWhite; //indicates if it's white's turn
    public int whitePieces; //# white pieces
    public int blackPieces;//# black pieces
    public bool mustCapture;// does the player have to capture?
    
    // Start is called before the first frame update
    public GameLogic()
    {
        //setup board
        board = new CheckersPiece[8, 8];
        InitBoard();
        PrintBoard();

        //setup empty piece
        empty.IsAlive = false;
        empty.IsKing = false;
        empty.IsWhite = false;
        empty.position = new Vector2Int(-3, -3);

        //setup game state
        turnWhite = true;
        mustCapture = false;
        whitePieces = 12;
        blackPieces = 12;
        
    }

    // Update is called once per frame
    public CheckersPiece GetPieceAt(Vector2Int position)
    {
        return board[position.x, position.y];
    }
    public Vector2Int[] CheckMovement(CheckersPiece current)
    {
        
        List<Vector2Int> list = new List<Vector2Int>();
        Vector2Int currentPos = current.position;
        

        bool canLeft = true;
        bool canRight = true;

        int flipVert = 1;
        if (!current.IsWhite) flipVert = -1; // flips which way is forward based on if a piece is white or black

        if (currentPos.x-1 < 0) { //Checks if it's against the left edge
            canLeft = false;
        }
        if (currentPos.x+1 > 7) { //Checks if it's against the right edge
            canRight = false;
        }

        //Checks if it can move left and if the forward left space is empty
            // if so, add to possible spaces array
        if (canLeft)
        {
            Debug.Log(GetPieceAt(new Vector2Int(currentPos.x-1, currentPos.y + flipVert)).position);
            if(GetPieceAt(new Vector2Int(currentPos.x-1, currentPos.y + flipVert)).position == empty.position)
            {
                Debug.Log("Adding Left");
                list.Add(new Vector2Int (currentPos.x-1, currentPos.y + flipVert));
            }
        }
        //Checks if it can move right and if the forward right space is empty
            // if so, add to possible spaces array
        if (canRight)
        {
            if (GetPieceAt(new Vector2Int(currentPos.x+1, currentPos.y + flipVert)).position == empty.position)
            {
                Debug.Log("Adding Right");
                list.Add(new Vector2Int (currentPos.x+1, currentPos.y + flipVert));
            }
        }

        //If piece is king, checks spaces behind for movement, adds them to the list
        if(current.IsKing)
        {
            if (canLeft && GetPieceAt(new Vector2Int(currentPos.x-1, currentPos.y + flipVert*-1)).position == empty.position)
            {
                list.Add(new Vector2Int (currentPos.x-1, currentPos.y + flipVert*-1));
            }
            if (canRight && GetPieceAt(new Vector2Int(currentPos.x+1, currentPos.y + flipVert*-1)).position == empty.position)
            {
                list.Add(new Vector2Int (currentPos.x+1, currentPos.y + flipVert*-1));
            }
        }
        Debug.Log(list);
        return list.ToArray();

    }

    public void MovePiece(CheckersPiece current, Vector2Int position)
    {
        
        //checks if current piece can move to the desired location
        bool canMove = Array.Exists(CheckMovement(current), element => element == position);
        //if it can AND if the color matches the player turn, move the piece
        if (canMove && current.IsWhite == turnWhite)
        {
            board[current.position.x, current.position.y] = empty;
            board[position.x, position.y] = current;
            current.position = position;

        }

        turnWhite = !turnWhite;
        return;
    }

    public void CheckCapturable()
    {
        int flipVert = 1;
        if (!turnWhite) flipVert = -1; // flips which way is forward based on if a piece is white or black
        foreach (CheckersPiece piece in board)
        {
            if (piece.IsWhite == turnWhite)
            {
                if (turnWhite)
                {
                    if (piece.position.x - 2 <= 0 && piece.position.y + 2*flipVert <= 7)
                    {
                        if (board[piece.position.x - 1, piece.position.y + flipVert].position != empty.position && board[piece.position.x - 1, piece.position.y + flipVert].IsWhite != piece.IsWhite && board[piece.position.x-2,piece.position.y + 2*flipVert].position == empty.position)
                        {
                            //CAN CAPTURE UP LEFT PIECE
                        }
                    }
                    if (piece.position.x + 2 <= 7 && piece.position.y + 2*flipVert <= 7)
                    {
                        if (board[piece.position.x + 1, piece.position.y + flipVert].position != empty.position && board[piece.position.x + 1, piece.position.y + flipVert].IsWhite != piece.IsWhite && board[piece.position.x+2,piece.position.y + 2*flipVert].position == empty.position)
                        {
                            //CAN CAPTURE UP RIGHT PIECE
                        }
                    }

                    //CHECK KING DIRECTION
                    if (piece.IsKing && piece.position.x - 2 <= 0 && piece.position.y - 2*flipVert >= 0)
                    {
                        if (board[piece.position.x - 1, piece.position.y - flipVert].position != empty.position && board[piece.position.x - 1, piece.position.y - flipVert].IsWhite != piece.IsWhite && board[piece.position.x-2,piece.position.y - 2*flipVert].position == empty.position)
                        {
                            //CAN CAPTURE BACK LEFT PIECE
                        }
                    }
                    if (piece.IsKing && piece.position.x + 2 <= 7 && piece.position.y + 2*flipVert >= 0)
                    {
                        if (board[piece.position.x + 1, piece.position.y - flipVert].position != empty.position && board[piece.position.x + 1, piece.position.y - flipVert].IsWhite != piece.IsWhite && board[piece.position.x+2,piece.position.y - 2*flipVert].position == empty.position)
                        {
                            //CAN CAPTURE UP RIGHT PIECE
                        }
                    }

                    
                } else { //NOT WHITE TURN
                    if (piece.position.x - 2 <= 0 && piece.position.y + 2*flipVert >= 0)
                    {
                        if (board[piece.position.x - 1, piece.position.y + flipVert].position != empty.position && board[piece.position.x - 1, piece.position.y + flipVert].IsWhite != piece.IsWhite && board[piece.position.x-2,piece.position.y + 2*flipVert].position == empty.position)
                        {
                            //CAN CAPTURE UP LEFT PIECE
                        }
                    }
                    if (piece.position.x + 2 <= 7 && piece.position.y + 2*flipVert >= 0)
                    {
                        if (board[piece.position.x + 1, piece.position.y + flipVert].position != empty.position && board[piece.position.x + 1, piece.position.y + flipVert].IsWhite != piece.IsWhite && board[piece.position.x+2,piece.position.y + 2*flipVert].position == empty.position)
                        {
                            //CAN CAPTURE UP RIGHT PIECE
                        }
                    }

                    //CHECK KING DIRECTION
                    if (piece.IsKing && piece.position.x - 2 <= 0 && piece.position.y - 2*flipVert <= 7)
                    {
                        if (board[piece.position.x - 1, piece.position.y - flipVert].position != empty.position && board[piece.position.x - 1, piece.position.y - flipVert].IsWhite != piece.IsWhite && board[piece.position.x-2,piece.position.y - 2*flipVert].position == empty.position)
                        {
                            //CAN CAPTURE BACK LEFT PIECE
                        }
                    }
                    if (piece.IsKing && piece.position.x + 2 <= 7 && piece.position.y + 2*flipVert <= 7)
                    {
                        if (board[piece.position.x + 1, piece.position.y - flipVert].position != empty.position && board[piece.position.x + 1, piece.position.y - flipVert].IsWhite != piece.IsWhite && board[piece.position.x+2,piece.position.y - 2*flipVert].position == empty.position)
                        {
                            //CAN CAPTURE UP RIGHT PIECE
                        }
                    }

                }
            }
        }
    }
    public void CapturePiece(CheckersPiece current, string direction)
    {
        return;
    }

    public void PrintBoard()
    {
        string toPrint = "";
        
        for (int j=0; j<8; j++)
        {
            for (int i=0; i<8; i++)
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
    public void InitBoard()
    {
        CheckersPiece current;
        for (int j=0; j<3; j++)
        {
            for (int i=0; i<8; i++)
            {   
                if (i%2 == 0 && j%2 == 1 || i%2 == 1 && j%2 == 0)
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
        for (int j=3; j<5; j++)
        {
            for (int i=0; i<8; i++)
            {
                board[i,j] = empty;
            }
        }
        for (int j=5; j<8; j++)
        {
            for (int i=0; i<8; i++)
            {   
                if (i%2 == 1 && j%2 == 0 || i%2 == 0 && j%2 ==1)
                {
                    current.IsWhite = false;
                    current.IsAlive = true;
                    current.IsKing = false;
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
