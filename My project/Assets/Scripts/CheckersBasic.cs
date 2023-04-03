using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;

public class CheckersBasic : MonoBehaviour
{
    public GameObject whiteQueen, blackQueen, whitePiece, blackPiece, visualizePiece;
    GameLogic game = new GameLogic();
    private GameObject[,] pieces = new GameObject[8, 8];

    public GameObject visualization;
    private Vector3 boardCornerleftbottom = new Vector3(-0.210f, 0.1f, 0.16f);
    private Vector3 boardCornerrightupper = new Vector3(-0.216f, 0.1f, -0.259f);
    private Vector3[,] postionCoordinates = new Vector3[8, 8];

    //Two arrrays used for determing the possibale piece move
    private GameObject[] visualizationPosPiece = new GameObject[0];
    Vector2Int[] visualizationPosCoordinates = new Vector2Int[0];

    Vector2Int pickedPiece = new Vector2Int(-1,-1);
    void Start()
    {
        boardInitialize();
        game.InitBoard();
        game.PrintBoard();
        //game.GetPieceAt(new Vector2Int(1,1));
        //Initialize visualization GameObject
        //______________________________________________________________
        visualization = GameObject.Instantiate(visualizePiece);
        visualization.transform.SetParent(transform);
        visualization.layer = LayerMask.NameToLayer("Ignore Raycast");
        //______________________________________________________________

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Debug.Log("Cursor position at " + cursorPosition());

            //Moving the selected piece if possiable
            if (!movePiece(cursorPosition()))
            {
                //Shows the user where piece is able to move
                showPossiblePositions(cursorPosition());
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
        Vector2 position = cursorPosition();
        if (position == new Vector2(-1, -1) || pieces[(int)position.x, (int)position.y] == null)
        {
            visualization.transform.position = Vector3.zero;
            return;
        }

        visualization.transform.localPosition = postionCoordinates[(int)position.x, (int)position.y];

        //Debug.Log("Guide position is " + convertToVector3(position));
    }

    private void showPossiblePositions(Vector2Int pos)
    {
        //cleun up visualizationPosPiece
        for (int i = 0; i < visualizationPosPiece.Length; i++)
        {
            Destroy(visualizationPosPiece[i]);
        }

        if (game.GetPieceAt(pos).position == game.empty.position)
        {
            Debug.Log("Selected empty piece.");
            pickedPiece = new Vector2Int(-1, -1);
            return;
        }

        pickedPiece = pos;
        visualizationPosCoordinates = game.CheckMovement(game.GetPieceAt(pos));
        Debug.Log("Available move positions is " + visualizationPosCoordinates.Length);

        
        visualizationPosPiece = new GameObject[visualizationPosCoordinates.Length];
        for (int i = 0; i < visualizationPosCoordinates.Length; i++)
        {
            visualizationPosPiece[i] = GameObject.Instantiate(visualizePiece);
            visualizationPosPiece[i].transform.SetParent(transform);
            visualizationPosPiece[i].layer = LayerMask.NameToLayer("Ignore Raycast");
        }

        for (int i = 0; i < visualizationPosCoordinates.Length; i++)
        {
            visualizationPosPiece[i].transform.localPosition = postionCoordinates[visualizationPosCoordinates[i].x, visualizationPosCoordinates[i].y];
            Debug.Log("Can move to " + visualizationPosCoordinates[i]);
        }

        return;
    }


    private bool movePiece(Vector2Int pos)
    {
        for (int i = 0; i < visualizationPosPiece.Length; i++)
        {
            if (visualizationPosCoordinates[i] == pos)
            {
                Destroy(pieces[pickedPiece.x, pickedPiece.y]);
                generatePiece(blackPiece, pos.x, pos.y);

                game.MovePiece(game.GetPieceAt(pickedPiece), pos);

                for (int j = 0; j < visualizationPosPiece.Length; j++)
                {
                    Destroy(visualizationPosPiece[j]);
                }
                visualizationPosPiece = new GameObject[0];
                return true;
            }
        }
        return false;
    }
    private Vector3 convertToVector3(Vector2 v)
    {
        return new Vector3(v.x, boardCornerleftbottom.y, v.y);
    }
}

    
