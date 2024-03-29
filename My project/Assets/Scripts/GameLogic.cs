using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CheckersPiece {
    public bool IsWhite;
    public bool IsKing;
    public bool IsAlive;
    public Vector2Int position;
    public bool JustCaptured;
    public CheckersPiece(bool isWhite, bool isKing, bool isAlive, bool justCaptured, Vector2Int position) 
    {
        this.IsWhite = isWhite;
        this.IsKing = isKing;
        this.IsAlive = isAlive;
        this.JustCaptured= justCaptured;
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
        empty.JustCaptured = false;
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
        if (turnWhite != current.IsWhite)
        {
            return null;
        }
        if (mustCapture == true)
        {
            Debug.Log("Player must capture!");
            return null;
        }
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
        if(mustCapture)
        {
            return;
        }
        //checks if current piece can move to the desired location
        bool canMove = Array.Exists(CheckMovement(current), element => element == position);
        //if it can AND if the color matches the player turn, move the piece
        if (canMove && current.IsWhite == turnWhite)
        {
            board[position.x, position.y] = current;
            board[current.position.x, current.position.y] = empty;
            
            board[position.x, position.y].position = position;

        }

        turnWhite = !turnWhite;
        if (current.position.y == 7 || current.position.y == 0)
        {
            current.IsKing = true;
        }
        CheckCapturableBoard();
        return;
    }

    public (CheckersPiece, CheckersPiece, Vector2Int)[] CheckCapturablePiece(CheckersPiece piece)
    {
        int numberCapturable = 0;
        List<(CheckersPiece, CheckersPiece, Vector2Int)> output = new List<(CheckersPiece, CheckersPiece, Vector2Int)>();
        int flipVert = 1;
        if (!turnWhite) flipVert = -1; // flips which way is forward based on if a piece is white or black

        if (turnWhite)
                {
                    if (piece.position.x - 2 >= 0 && piece.position.y + 2*flipVert <= 7)
                    {
                        if (board[piece.position.x - 1, piece.position.y + flipVert].position != empty.position && board[piece.position.x - 1, piece.position.y + flipVert].IsWhite != piece.IsWhite && board[piece.position.x-2,piece.position.y + 2*flipVert].position == empty.position)
                        {
                            //CAN CAPTURE UP LEFT PIECE
                            output.Add((piece, board[piece.position.x - 1, piece.position.y + flipVert], new Vector2Int (piece.position.x-2,piece.position.y + 2*flipVert)));
                            mustCapture = true;
                            numberCapturable += 1;
                        }
                    }
                    if (piece.position.x + 2 <= 7 && piece.position.y + 2*flipVert <= 7)
                    {
                        if (board[piece.position.x + 1, piece.position.y + flipVert].position != empty.position && board[piece.position.x + 1, piece.position.y + flipVert].IsWhite != piece.IsWhite && board[piece.position.x+2,piece.position.y + 2*flipVert].position == empty.position)
                        {
                            //CAN CAPTURE UP RIGHT PIECE
                            Debug.Log(piece.position.x + " " + piece.position.y);
                            Debug.Log(board[piece.position.x + 1, piece.position.y + flipVert].position.x + " " + board[piece.position.x + 1, piece.position.y + flipVert].position.y);
                            Debug.Log(piece);
                            Debug.Log(board[piece.position.x + 1, piece.position.y + flipVert]);
                            output.Add((piece, board[piece.position.x + 1, piece.position.y + flipVert], new Vector2Int (piece.position.x+2,piece.position.y + 2*flipVert)));
                            mustCapture = true;
                            numberCapturable += 1;
                        }
                    }

                    //CHECK KING DIRECTION
                    if ((piece.IsKing || piece.JustCaptured) && piece.position.x - 2 <= 0 && piece.position.y - 2*flipVert >= 0)
                    {
                        if (board[piece.position.x - 1, piece.position.y - flipVert].position != empty.position && board[piece.position.x - 1, piece.position.y - flipVert].IsWhite != piece.IsWhite && board[piece.position.x-2,piece.position.y - 2*flipVert].position == empty.position)
                        {
                            //CAN CAPTURE BACK LEFT PIECE
                            output.Add((piece, board[piece.position.x - 1, piece.position.y - flipVert], new Vector2Int(piece.position.x-2,piece.position.y - 2*flipVert)));
                            mustCapture = true;
                            numberCapturable += 1;
                        }
                    }
                    if ((piece.IsKing || piece.JustCaptured) && piece.position.x + 2 <= 7 && piece.position.y + 2*flipVert >= 0)
                    {
                        if (board[piece.position.x + 1, piece.position.y - flipVert].position != empty.position && board[piece.position.x + 1, piece.position.y - flipVert].IsWhite != piece.IsWhite && board[piece.position.x+2,piece.position.y - 2*flipVert].position == empty.position)
                        {
                            //CAN CAPTURE UP RIGHT PIECE
                            output.Add((piece, board[piece.position.x + 1, piece.position.y - flipVert], new Vector2Int(piece.position.x+2,piece.position.y - 2*flipVert)));
                            mustCapture = true;
                            numberCapturable += 1;
                        }
                    }

                    
                } else { //NOT WHITE TURN
                    if (piece.position.x - 2 >= 0 && piece.position.y + 2*flipVert >= 0)
                    {
                        if (board[piece.position.x - 1, piece.position.y + flipVert].position != empty.position && board[piece.position.x - 1, piece.position.y + flipVert].IsWhite != piece.IsWhite && board[piece.position.x-2,piece.position.y + 2*flipVert].position == empty.position)
                        {
                            //CAN CAPTURE UP LEFT PIECE
                            output.Add((piece, board[piece.position.x - 1, piece.position.y + flipVert], new Vector2Int(piece.position.x-2,piece.position.y + 2*flipVert)));
                            mustCapture = true;
                            numberCapturable += 1;
                        }
                    }
                    if (piece.position.x + 2 <= 7 && piece.position.y + 2*flipVert >= 0)
                    {
                        if (board[piece.position.x + 1, piece.position.y + flipVert].position != empty.position && board[piece.position.x + 1, piece.position.y + flipVert].IsWhite != piece.IsWhite && board[piece.position.x+2,piece.position.y + 2*flipVert].position == empty.position)
                        {
                            //CAN CAPTURE UP RIGHT PIECE
                            output.Add((piece, board[piece.position.x + 1, piece.position.y + flipVert], new Vector2Int(piece.position.x+2,piece.position.y + 2*flipVert)));
                            mustCapture = true;
                            numberCapturable += 1;
                        }
                    }

                    //CHECK KING DIRECTION
                    if ((piece.IsKing || piece.JustCaptured) && piece.position.x - 2 >= 0 && piece.position.y - 2*flipVert <= 7)
                    {
                        if (board[piece.position.x - 1, piece.position.y - flipVert].position != empty.position && board[piece.position.x - 1, piece.position.y - flipVert].IsWhite != piece.IsWhite && board[piece.position.x-2,piece.position.y - 2*flipVert].position == empty.position)
                        {
                            //CAN CAPTURE BACK LEFT PIECE
                            output.Add((piece, board[piece.position.x - 1, piece.position.y - flipVert], new Vector2Int(piece.position.x-2,piece.position.y - 2*flipVert)));
                            mustCapture = true;
                            numberCapturable += 1;
                        }
                    }
                    if ((piece.IsKing || piece.JustCaptured) && piece.position.x + 2 <= 7 && piece.position.y + 2*flipVert <= 7)
                    {
                        if (board[piece.position.x + 1, piece.position.y - flipVert].position != empty.position && board[piece.position.x + 1, piece.position.y - flipVert].IsWhite != piece.IsWhite && board[piece.position.x+2,piece.position.y - 2*flipVert].position == empty.position)
                        {
                            //CAN CAPTURE UP RIGHT PIECE
                            output.Add((piece, board[piece.position.x + 1, piece.position.y - flipVert], new Vector2Int(piece.position.x+2,piece.position.y - 2*flipVert)));
                            mustCapture = true;
                            numberCapturable += 1;
                        }
                    }

                }
        //OUTPUT IS AN ARRAY OF TUPLES IN FORM (CURRENT PIECE, CAPTURED PIECE, POSITION TO MOVE TO)
        return output.ToArray();

    }
    public void CheckCapturableBoard()
    {
        foreach (CheckersPiece piece in board)
        {
            if (piece.IsWhite == turnWhite && piece.position != empty.position)
            {
                CheckCapturablePiece(piece);
            }
        }
    }



    public void CapturePiece(CheckersPiece current, CheckersPiece captured, Vector2Int position)
    {
        
        board[current.position.x, current.position.y] = empty;
        board[position.x, position.y] = current;
        board[position.x, position.y].position = position;

        if (captured.IsWhite)
        {
            whitePieces -=1;
        } else {
            blackPieces -=1;
        }
        board[captured.position.x, captured.position.y] = empty;

        board[position.x, position.y].JustCaptured = true;

        if (CheckCapturablePiece(board[position.x, position.y]).Length == 0)
        {
            board[position.x, position.y].JustCaptured = false;
            turnWhite = !turnWhite;
            mustCapture = false;
            if (current.position.y == 7 || current.position.y == 0)
            {
                current.IsKing = true;
            }
            CheckCapturableBoard();
        }
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
                    current.JustCaptured = false;
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
                    current.JustCaptured = false;
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
