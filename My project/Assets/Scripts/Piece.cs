using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    // Start is called before the first frame update
    private int x, y;
    private GameObject piece;
    public Piece(GameObject piece, int x, int y)
    { 
        this.piece = piece;
        this.x = x; 
        this.y = y;
    }
}
