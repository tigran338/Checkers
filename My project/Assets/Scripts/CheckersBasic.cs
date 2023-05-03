using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;

public class CheckersBasic : MonoBehaviour
{
    public GameObject whiteKing, blackKing, whitePiece, blackPiece, visualizePiece;
    private GameObject[,] pieces = new GameObject[8, 8];

    private bool isWhiteTurn = true;
    private bool onlyCapture = false;

    public GameObject visualization;
    private Vector3 boardCornerleftbottom = new Vector3(-0.210f, 0.1f, 0.16f);
    private Vector3 boardCornerrightupper = new Vector3(-0.216f, 0.1f, -0.259f);
    private Vector3[,] postionCoordinates = new Vector3[8, 8];

    //Two arrrays used for determing the possibale piece move
    private GameObject[] visualizationPosPiece = new GameObject[0];
    List<(Vector2Int, Vector2Int?)> visualizationPosCoordinates = null;

    Vector2Int pickedPiece = new Vector2Int(-1,-1);
    void Start()
    {
        boardInitialize();
        //game.GetPieceAt(new Vector2Int(1,1));
        //Initialize visualization GameObject
        //______________________________________________________________
        visualization = GameObject.Instantiate(visualizePiece);
        visualization.transform.SetParent(transform);
        visualization.layer = LayerMask.NameToLayer("Ignore Raycast");
        //______________________________________________________________
        isWhiteTurn = true;

    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetMouseButtonUp(0))
        {
            //onlyCapture = OnlyCapture();
            //Debug.Log("Cursor position at " + cursorPosition());
            var piecePos = cursorPosition();

            if (IsInBounds(piecePos))
            {
                /*
                List<(Vector2Int, Vector2Int?)> moves = pieceAbleMove(piecePos);
                Debug.Log("Tigran checks " + moves.Count);
                foreach ((Vector2Int move, Vector2Int? capture) in moves)
                {
                    if (capture.HasValue)
                    {
                        Debug.Log(string.Format("Move from ({0}, {1}) to ({2}, {3}), capturing ({4}, {5}).",
                            piecePos.x, piecePos.y, move.x, move.y, capture.Value.x, capture.Value.y));
                    }
                    else
                    {
                        Debug.Log(string.Format("Move from ({0}, {1}) to ({2}, {3}).",
                            piecePos.x, piecePos.y, move.x, move.y));
                    }
                }
                */

                if (!movePiece(piecePos))
                {
                    //Shows the user where piece is able to move
                    showPossiblePositions(piecePos);
                }

            }
        }
        userGuide();


    }



    private void generatePiece(GameObject pieceType, int x, int y)
    {
        GameObject go = GameObject.Instantiate(pieceType);
        go.transform.SetParent(transform);
        go.transform.localPosition = postionCoordinates[x, y];
        go.layer = LayerMask.NameToLayer("Ignore Raycast");
        pieces[x, y] = go;
    }

    //Puting pieces on there places to start the game
    private void boardInitialize()
    {
        //Fill up postionCoordinates array
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                pieces[i, j] = null;
                postionCoordinates[i, j] = boardCornerleftbottom + new Vector3(i * (Math.Abs(boardCornerleftbottom.x) + Math.Abs(boardCornerrightupper.x)) / 7, 0, 0) - new Vector3(0, 0, j * (Math.Abs(boardCornerleftbottom.z) + Math.Abs(boardCornerrightupper.z)) / 7);
            }
        }

        //Spawn White Pieces
        for (int j = 0; j < 3; j++)
        {
            for (int i = 0; i < 8; i++)
            {
                if ((i + j) % 2 == 1)
                    generatePiece(whitePiece, i, j);
            }
        }

        //Spawn Black Pieces

        for (int j = 7; j > 4; j--)
        {
            for (int i = 0; i < 8; i++)
            {
                if ((i + j) % 2 == 1)
                    generatePiece(blackPiece, i, j);
            }
        }
    }

    //Return the board cell where cursor is pointing
    private Vector2Int cursorPosition()
    {
        Vector2Int closestPosition = new Vector2Int(-1, -1);
        RaycastHit hitInfo = new RaycastHit();
        bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);

        if (hitInfo.transform.tag.Equals("Base"))
        {
            float mindist = float.MaxValue;
            //Debug.Log(transform.InverseTransformPoint(hitInfo.point));
            for (int i = 0; i < 8; i++)
            {


                for (int j = 0; j < 8; j++)
                {
                    if (Math.Abs(Vector3.Distance(transform.InverseTransformPoint(hitInfo.point), postionCoordinates[i, j] + new Vector3(0, 0, (Math.Abs(boardCornerleftbottom.x) + Math.Abs(boardCornerrightupper.x)) / 7))) < mindist)
                    {
                        mindist = Math.Abs(Vector3.Distance(transform.InverseTransformPoint(hitInfo.point), postionCoordinates[i, j] + new Vector3(0, 0, (Math.Abs(boardCornerleftbottom.x) + Math.Abs(boardCornerrightupper.x)) / 7)));
                        closestPosition = new Vector2Int(i, j);
                    }
                }
            }
        }

        return closestPosition;
    }

    private void userGuide()
    {
        Vector2Int position = cursorPosition();
        


        if (position == new Vector2(-1, -1) || pieces[position.x, position.y] == null || !IsInBounds(position) || isWhiteTurn != pieceType(position).Item1)
        {
            visualization.transform.position = Vector3.zero;
            return;
        }

        visualization.transform.localPosition = postionCoordinates[position.x, position.y];

        //Debug.Log("Guide position is " + convertToVector3(position));
    }

    private void showPossiblePositions(Vector2Int pos)
    {
        //cleun up visualizationPosPiece
        for (int i = 0; i < visualizationPosPiece.Length; i++)
        {
            Destroy(visualizationPosPiece[i]);
        }

        pickedPiece = pos;

        //_____________________
        visualizationPosCoordinates = pieceAbleMoveMain(pickedPiece);

        if (visualizationPosCoordinates == null)
            return;

        
        visualizationPosPiece = new GameObject[visualizationPosCoordinates.Count];
        for (int i = 0; i < visualizationPosCoordinates.Count; i++)
        {
            visualizationPosPiece[i] = GameObject.Instantiate(visualizePiece);
            visualizationPosPiece[i].transform.SetParent(transform);
            visualizationPosPiece[i].layer = LayerMask.NameToLayer("Ignore Raycast");
        }

        for (int i = 0; i < visualizationPosCoordinates.Count; i++)
        {
            visualizationPosPiece[i].transform.localPosition = postionCoordinates[visualizationPosCoordinates[i].Item1.x, visualizationPosCoordinates[i].Item1.y];
        }

        return;
    }



    
    private bool movePiece(Vector2Int pos)
    {
        if (pickedPiece == new Vector2Int(-1, -1))
        {
            return false;
        }

        


        foreach (var (whereMove, capture) in pieceAbleMoveMain(pickedPiece))
        {
            if (pos == whereMove)
            {

                var (isWhite, isKing) = pieceType(pickedPiece);
                if (isWhite)
                {
                    if (isKing || pos.y == 7)
                        generatePiece(whiteKing, pos.x, pos.y);
                    else
                        generatePiece(whitePiece, pos.x, pos.y);
                }
                else
                {
                    if (isKing || pos.y == 0)
                        generatePiece(blackKing, pos.x, pos.y);
                    else
                        generatePiece(blackPiece, pos.x, pos.y);
                }

                Destroy(pieces[pickedPiece.x, pickedPiece.y]);
                pieces[pickedPiece.x, pickedPiece.y] = null;


                if (capture != null)
                {
                    var temp = capture ?? new Vector2Int(-1, -1);
                    Destroy(pieces[temp.x, temp.y]);
                    pieces[temp.x, temp.y] = null;
                }


                for (int i = 0; i < visualizationPosPiece.Length; i++)
                {
                    Destroy(visualizationPosPiece[i]);
                }
                pickedPiece = new Vector2Int(-1, -1);
                isWhiteTurn = !isWhiteTurn;
                return true;
            }
        }

        pickedPiece = new Vector2Int(-1, -1);
        return false;
    }






    /// <summary>
    /// Determines the position where can move the piece and the pieces must be captured. Takes into account the game situation
    /// </summary>
    /// <param name="piecePos">The position of the piece to check.</param>
    /// <returns>The List of tuple of (Vector2Int, Vector2Int?) values representing (NewPosition, MustCapture).  If MustCapture is null no need to capture</returns>
    private List<(Vector2Int, Vector2Int?)> pieceAbleMoveMain(Vector2Int piecePos)
    {
        if (OnlyCapture())
        {
            foreach (var (_, capture) in pieceAbleMove(piecePos))
            {
                if (capture != null)
                {
                    return pieceAbleMove(piecePos);
                }
            }
            return new List<(Vector2Int, Vector2Int?)>(); ;
        }
        return pieceAbleMove(piecePos);
    }





    /// <summary>
    /// Determines the position where can move the piece and the pieces must be captured. Dont take ino account game situation
    /// </summary>
    /// <param name="piecePos">The position of the piece to check.</param>
    /// <returns>The List of tuple of (Vector2Int, Vector2Int?) values representing (NewPosition, MustCapture).  If MustCapture is null no need to capture</returns>
    private List<(Vector2Int, Vector2Int?)> pieceAbleMove(Vector2Int piecePos)
    {
        List<(Vector2Int, Vector2Int?)> moves = new List<(Vector2Int, Vector2Int?)>();
        List<(Vector2Int, Vector2Int?)> captureMoves = new List<(Vector2Int, Vector2Int?)>();
        var (isWhite, isKing) = pieceType(piecePos);


        if (pieces[piecePos.x, piecePos.y] == null || isWhiteTurn != pieceType(piecePos).Item1)
            return moves;


        int numSteps;
        Vector2Int[] moveSteps;
        Vector2Int[] captureSteps = new Vector2Int[] {
            new Vector2Int(1, 1), new Vector2Int(-1, 1),
            new Vector2Int(1, -1), new Vector2Int(-1, -1) 
        };

        if (isKing)
        {
            numSteps = 8;
            moveSteps = new Vector2Int[] {
            new Vector2Int(1, 1), new Vector2Int(-1, 1),
            new Vector2Int(1, -1), new Vector2Int(-1, -1)
        };
         
        }
        else
        {
            numSteps = 1;
            moveSteps = isWhite
                ? new Vector2Int[] { new Vector2Int(1, 1), new Vector2Int(-1, 1) }
                : new Vector2Int[] { new Vector2Int(1, -1), new Vector2Int(-1, -1) };
        }

        /*if (OnlyCapture())
        {
            moveSteps = new Vector2Int[] { };
        }*/
        
        foreach (Vector2Int move in moveSteps)
        {
            int n = numSteps;
            Vector2Int newPosition = piecePos;
            while (n > 0)
            {
                newPosition += move;
                if (IsInBounds(newPosition))
                {
                    var (targetIsWhite, _) = pieceType(newPosition);

                    if (isWhite != targetIsWhite && pieces[newPosition.x, newPosition.y] != null)
                    {
                        break;
                    }
                    else if (pieces[newPosition.x, newPosition.y] == null)
                    {
                        moves.Add((newPosition, null));
                    }
                }
                n--;
            }
        }


        foreach (Vector2Int move in captureSteps)
        {
            int n = numSteps;
            Vector2Int capturePosition = piecePos;
            while (n > 0)
            {
                capturePosition += move;
                if (IsInBounds(capturePosition))
                {
                    var (targetIsWhite, _) = pieceType(capturePosition);

                    if (isWhite != targetIsWhite && pieces[capturePosition.x, capturePosition.y] != null)
                    {
                        var newPosition = capturePosition + move;
                        if (IsInBounds(newPosition) && pieces[newPosition.x, newPosition.y] == null)
                        {
                            captureMoves.Add((newPosition, capturePosition));
                        }
                        break;
                    }
                }
                n--;
            }
        }

        

        return captureMoves.Count > 0 ? captureMoves : moves;
    }


    private bool IsInBounds(Vector2Int position)
    {
        return position.x >= 0 && position.x < 8 && position.y >= 0 && position.y < 8;
    }




    /// <summary>
    /// Determines the type of a piece at a given position and returns a tuple of two boolean values.
    /// </summary>
    /// <param name="pospiece">The position of the piece to check.</param>
    /// <returns>A tuple of two boolean values representing (isWhite, isKing).</returns>
    private (bool,bool) pieceType(Vector2Int pospiece)
    {
        
        var piece = pieces[pospiece.x, pospiece.y];
        bool isWhite = false;
        bool isKing = false;
        if (piece == null)
            return (false,false);
        //Debug.Log("                                 " + piece.name);
        if (piece.name.Contains("White"))
        {
            isWhite = true;
            //Debug.Log("White");
        }

        if (piece.name.Contains("King"))
        {
            isKing = true;
            //Debug.Log("King");
        }



        return (isWhite, isKing);
    }

    private bool OnlyCapture()
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                var v = new Vector2Int(i, j);
                if (pieces[i,j] != null && isWhiteTurn == pieceType(v).Item1)
                {
                    foreach (var (_,capture) in pieceAbleMove(v))
                    {
                        if (capture != null)
                        {
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }
}

    
