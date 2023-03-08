using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEngine;

public class TheBasics : MonoBehaviour
{
    float[] primitiveTrasl = { 0.5f, 1f, 0.5f, 0.5f };
    int primitiveIndex = 0;
    public Material[] materialsArray = new Material[5];
    GameObject visualization;

    int materialIndex = 0;
    // Start is called before the first frame update

    private void ObjDestroy(GameObject obj)
    { 
    
    }
    private void OnEnable()
    {
        BasicUIHandler.OnPrimitiveChange += BasicUIHandler_OnPrimitiveChange;
        BasicUIHandler.OnMaterialChange += BasicUIHandler_OnMaterialChange;
    }



    private void OnDisable()
    {
        BasicUIHandler.OnPrimitiveChange -= BasicUIHandler_OnPrimitiveChange;
        BasicUIHandler.OnMaterialChange -= BasicUIHandler_OnMaterialChange;
    }

    private void BasicUIHandler_OnMaterialChange(int index)
    {
        //throw new System.NotImplementedException();
        Debug.Log($"The  Material index is " + index);
        materialIndex = index;
    }

    private void BasicUIHandler_OnPrimitiveChange(int index)
    {
        //throw new System.NotImplementedException();
        //Debug.Log($"The  Basic class the index is " + index);
        primitiveIndex = index;
        var temp = visualization;
        visualization = GameObject.CreatePrimitive((PrimitiveType)primitiveIndex);
        visualization.layer = LayerMask.NameToLayer("Ignore Raycast");
        Destroy(temp);
    }
    void Start()
    {
        visualization = GameObject.CreatePrimitive((PrimitiveType)primitiveIndex);
        //visualization.GetComponent<MeshRenderer>().material = materialsArray[materialIndex];
        visualization.layer = LayerMask.NameToLayer("Ignore Raycast");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(1))
        {
            RaycastHit hitInfo = new RaycastHit();
            bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);

            if (hit)
            {
                if (!hitInfo.transform.name.Equals("Plane"))
                {
                    var exp = hitInfo.transform.AddComponent<TriangleExplosion>();
                    StartCoroutine(exp.SplitMesh(true));
                }
            }
        }
        else if (Input.GetMouseButtonUp(0))  // check if left button is pressed
        {
            // take mouse position, convert from screen space to world space, do a raycast, store output of raycast into 
            // hitInfo object ...

            #region Screen To World
            RaycastHit hitInfo = new RaycastHit();
            bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
            if (hit)
            {

                #region HIDE

                var obj = GameObject.CreatePrimitive((PrimitiveType)primitiveIndex);
                obj.GetComponent<MeshRenderer>().material = materialsArray[materialIndex];


                /* switch(primitiveIndex)
                 {
                     case 3: obj.tag = "MyCube"; break;
                     case 0: obj.tag = "MySphere"; break ;
                     case 1: obj.tag = "MyCapsule"; break;
                 }*/
                //obj.tag = "Myobj";
                //obj.GetComponent<BoxCollider>().isTrigger = true;
                //obj.GetComponent<Renderer>().material = blockMaterial;
                #endregion

                //obj.transform.position = new Vector3(hitInfo.point.x, hitInfo.point.y + 0.5f, hitInfo.point.z);
                #region HIDE
                if (hitInfo.transform.tag.Equals("Base"))
                {
                    obj.transform.position = new Vector3(hitInfo.point.x, hitInfo.point.y + (primitiveTrasl[primitiveIndex]), hitInfo.point.z);
                }
                #region HIDE
                else
                {
                    //int max;
                    if (hitInfo.normal.x > hitInfo.normal.y && hitInfo.normal.x > hitInfo.normal.z)
                        hitInfo.normal = new Vector3(1, 0, 0);
                    else if (hitInfo.normal.y > hitInfo.normal.z && hitInfo.normal.y > hitInfo.normal.x)
                        hitInfo.normal = new Vector3(0, 1, 0);
                    else
                        hitInfo.normal = new Vector3(0, 0, 1);

                    int transition = primitiveIndex;

                    if (hitInfo.normal == new Vector3(0, 0, 1)) // z+
                    {
                        if (primitiveIndex == 1)
                        {
                            transition = 2;
                        }
                        obj.transform.position = new Vector3(hitInfo.transform.position.x, hitInfo.transform.position.y, hitInfo.point.z + (primitiveTrasl[transition]));
                    }
                    #region HIDE
                    if (hitInfo.normal == new Vector3(1, 0, 0)) // x+
                    {
                        if (primitiveIndex == 1)
                        {
                            transition = 2;
                        }
                        obj.transform.position = new Vector3(hitInfo.point.x + (primitiveTrasl[transition]), hitInfo.transform.position.y, hitInfo.transform.position.z);
                    }
                    if (hitInfo.normal == new Vector3(0, 1, 0)) // y+
                    {
                        obj.transform.position = new Vector3(hitInfo.transform.position.x, hitInfo.point.y + (primitiveTrasl[transition]), hitInfo.transform.position.z);
                    }
                    if (hitInfo.normal == new Vector3(0, 0, -1)) // z-
                    {
                        if (primitiveIndex == 1)
                        {
                            transition = 2;
                        }
                        obj.transform.position = new Vector3(hitInfo.transform.position.x, hitInfo.transform.position.y, hitInfo.point.z - (primitiveTrasl[transition]));
                    }
                    if (hitInfo.normal == new Vector3(-1, 0, 0)) // x-
                    {
                        if (primitiveIndex == 1)
                        {
                            transition = 2;
                        }
                        obj.transform.position = new Vector3(hitInfo.point.x - (primitiveTrasl[transition]), hitInfo.transform.position.y, hitInfo.transform.position.z);
                    }
                    if (hitInfo.normal == new Vector3(0, -1, 0)) // y-
                    {
                        obj.transform.position = new Vector3(hitInfo.transform.position.x, hitInfo.point.y - (primitiveTrasl[transition]), hitInfo.transform.position.z);
                    }
                    #endregion
                }
                #endregion

                //Debug.DrawRay(hitInfo.point, hitInfo.normal, Color.red, 2, false);
                //Debug.Log(hitInfo.normal);
                #endregion


            }
            #endregion
        }
        else
        {
            // take mouse position, convert from screen space to world space, do a raycast, store output of raycast into 
            // hitInfo object ...

            #region Screen To World
            RaycastHit hitInfo = new RaycastHit();
            bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
            if (hit)
            {
                #region HIDE

                /* switch(primitiveIndex)
                 {
                     case 3: obj.tag = "MyCube"; break;
                     case 0: obj.tag = "MySphere"; break ;
                     case 1: obj.tag = "MyCapsule"; break;
                 }*/
                //obj.tag = "Myobj";
                //obj.GetComponent<BoxCollider>().isTrigger = true;
                //obj.GetComponent<Renderer>().material = blockMaterial;
                #endregion

                //obj.transform.position = new Vector3(hitInfo.point.x, hitInfo.point.y + 0.5f, hitInfo.point.z);
                #region HIDE
                if (hitInfo.transform.tag.Equals("Base"))
                {
                    visualization.transform.position = new Vector3(hitInfo.point.x, hitInfo.point.y + (primitiveTrasl[primitiveIndex]), hitInfo.point.z);
                    visualization.GetComponent<MeshRenderer>().material = materialsArray[4];
                }
                #region HIDE
                else
                {
                    visualization.GetComponent<MeshRenderer>().material = materialsArray[3];
                    //int max;
                    if (hitInfo.normal.x > hitInfo.normal.y && hitInfo.normal.x > hitInfo.normal.z)
                        hitInfo.normal = new Vector3(1, 0, 0);
                    else if (hitInfo.normal.y > hitInfo.normal.z && hitInfo.normal.y > hitInfo.normal.x)
                        hitInfo.normal = new Vector3(0, 1, 0);
                    else
                        hitInfo.normal = new Vector3(0, 0, 1);

                    int transition = primitiveIndex;

                    if (hitInfo.normal == new Vector3(0, 0, 1)) // z+
                    {
                        if (primitiveIndex == 1)
                        {
                            transition = 2;
                        }
                        visualization.transform.position = new Vector3(hitInfo.transform.position.x, hitInfo.transform.position.y, hitInfo.point.z + (primitiveTrasl[transition]));
                    }
                    #region HIDE
                    if (hitInfo.normal == new Vector3(1, 0, 0)) // x+
                    {
                        if (primitiveIndex == 1)
                        {
                            transition = 2;
                        }
                        visualization.transform.position = new Vector3(hitInfo.point.x + (primitiveTrasl[transition]), hitInfo.transform.position.y, hitInfo.transform.position.z);
                    }
                    if (hitInfo.normal == new Vector3(0, 1, 0)) // y+
                    {
                        visualization.transform.position = new Vector3(hitInfo.transform.position.x, hitInfo.point.y + (primitiveTrasl[transition]), hitInfo.transform.position.z);
                    }
                    if (hitInfo.normal == new Vector3(0, 0, -1)) // z-
                    {
                        if (primitiveIndex == 1)
                        {
                            transition = 2;
                        }
                        visualization.transform.position = new Vector3(hitInfo.transform.position.x, hitInfo.transform.position.y, hitInfo.point.z - (primitiveTrasl[transition]));
                    }
                    if (hitInfo.normal == new Vector3(-1, 0, 0)) // x-
                    {
                        if (primitiveIndex == 1)
                        {
                            transition = 2;
                        }
                        visualization.transform.position = new Vector3(hitInfo.point.x - (primitiveTrasl[transition]), hitInfo.transform.position.y, hitInfo.transform.position.z);
                    }
                    if (hitInfo.normal == new Vector3(0, -1, 0)) // y-
                    {
                        visualization.transform.position = new Vector3(hitInfo.transform.position.x, hitInfo.point.y - (primitiveTrasl[transition]), hitInfo.transform.position.z);
                    }
                    #endregion
                }
                #endregion

                //Debug.DrawRay(hitInfo.point, hitInfo.normal, Color.red, 2, false);
                //Debug.Log(hitInfo.normal);
                #endregion
                //yield return new WaitForSeconds(1.0f);
                //while (timeRemaining - Time.deltaTime > 0)
                //{ }
                //Destroy(obj);

            }
            #endregion
        }
    }
}
