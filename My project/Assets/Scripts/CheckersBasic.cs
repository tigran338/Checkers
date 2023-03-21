using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CheckersBasic : MonoBehaviour
{
    public GameObject whiteQueen, blackQueen, whitePiece, blackPiece;
    GameObject[,] pieces = new GameObject[8, 8];
    private Vector3 boardCornerleftbottom = new Vector3(-0.210f, 0.1f, 0.16f);
    private Vector3 boardCornerrightupper = new Vector3(-0.216f, 0.1f, -0.259f);
    private Vector3[,] postionCoordinates = new Vector3[8, 8];


    void Start()
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
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if ((i + j) % 2 == 1)
                    GeneratePiece(whitePiece, i, j);
            }
        }

        //Spawn Black Pieces

        for (int i = 7; i > 4; i--)
        {
            for (int j = 0; j < 8; j++)
            {
                if ((i + j) % 2 == 1)
                    GeneratePiece(blackPiece, i, j);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Debug.Log(clickedPosition());
        }
    }
    private Vector2 clickedPosition()
    {
        Vector2 closestPosition = new Vector2(-1, -1);
        RaycastHit hitInfo = new RaycastHit();
        bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);

        if (hitInfo.transform.tag.Equals("Base"))
        {
            float mindist = float.MaxValue;
            Debug.Log(transform.InverseTransformPoint(hitInfo.point));
            for (int i = 0; i < 8; i++)
            {
               
                
                for (int j = 0; j < 8; j++)
                {
                    if (Math.Abs(Vector3.Distance(transform.InverseTransformPoint(hitInfo.point), postionCoordinates[i, j] + new Vector3(0, 0, (Math.Abs(boardCornerleftbottom.x) + Math.Abs(boardCornerrightupper.x)) / 7))) < mindist)
                    {
                        mindist = Math.Abs(Vector3.Distance(transform.InverseTransformPoint(hitInfo.point), postionCoordinates[i, j] + new Vector3(0, 0, (Math.Abs(boardCornerleftbottom.x) + Math.Abs(boardCornerrightupper.x)) / 7)));
                        closestPosition = new Vector2(i, j);
                    }
                }
            }
        }

        return closestPosition;
    }


    private void GeneratePiece(GameObject pieceType, int x, int y)
    {
        GameObject go = GameObject.Instantiate(pieceType);
        go.transform.SetParent(transform);
        go.transform.localPosition = postionCoordinates[x, y];
        go.layer = LayerMask.NameToLayer("Ignore Raycast");
        pieces[x,y] = go;
    }
}

    
