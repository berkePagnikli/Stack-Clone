using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class objController : MonoBehaviour
{
    //scripts
    [SerializeField] gameMaster masterScript;

    //transforms
    [SerializeField] private Transform baseObjTrans;
    [SerializeField] private Transform lastObjTrans;

    //GameObjects
    [SerializeField] private GameObject cutObjPref;
    [SerializeField] private GameObject remainingObjPref;
    [SerializeField] private GameObject gameOverText;

    //MeshRenderers
    [SerializeField] private MeshRenderer baseObjMesh;

    //Variables
    [SerializeField] private float limit;
    [SerializeField] private float speed;
    private bool isForward;
    private bool isMoveX;
    public bool gameStop;
    private bool fail(Vector3 distance)
    {
        var origin = isMoveX ? transform.localScale.x : transform.localScale.z;
        var current = isMoveX ? Mathf.Abs(distance.x) : Mathf.Abs(distance.z);

        return current >= origin;
    }
    enum Direction{ Left, Right, Front, Back }



    /*
     * Basicly, Update moves the object but it does other stuff too such as checking: 
     * If the game has stoped. It is necessary for cutting process.
     * If player has used touch screen
     */ 
    private void Update()
    {
        if (gameStop) return;

        if (Input.GetMouseButtonDown(0)) OnClicked();

        var position = transform.position;
        var direction = isForward ? 1 : -1;
        var move = speed * Time.deltaTime * direction;

        if (isMoveX)
        {
            position.x += move;

            if(position.x < -limit ||  position.x > limit)
            {
                position.x = Mathf.Clamp(position.x, -limit, limit);
                isForward = !isForward;
            }
        }
        else
        {
            position.z += move;

            if (position.z < -limit || position.z > limit)
            {
                position.z = Mathf.Clamp(position.z, -limit, limit);
                isForward = !isForward;
            }
        }

        transform.position = position;
    }


    /*
     * OnClicked():
     * Stops the game and checks the distance between last object and current object
     * In the beginning, the last object is assigned as the base object
     * If the distance between the last object and the current object is not negative, the game can continue
     * CutObject() initiates. The information of movement axis is given.
     * If the object has been cut successfully, the new position is assigned and the score is updated
     */
    private void OnClicked()
    {
        gameStop = true;
        var distance = lastObjTrans.position - transform.position;

        if (fail(distance))
        {
            masterScript.Fail();
            return;
        }

        CutObject(isMoveX, isMoveX ? distance.x : distance.z);

        isMoveX = !isMoveX;
        var newPos = lastObjTrans.position;
        newPos.y += transform.localScale.y;
        if (!isMoveX) newPos.z = limit;
        else newPos.x = limit;
        transform.position = newPos;

        transform.localScale = lastObjTrans.localScale;
        masterScript.Scored();
        gameStop = false;
    }


    /*
     * CutObject(): 
     * Checks if the left or right side of the object will be cut
     * Instantiates two prefabs one being the one that cut out the other being the one that remains 
     * Scales and positions new objects
     * Starts coroutine to destroy the object that has cut out
     */
    private void CutObject(bool isCutZ, float value)
    {
        bool isCutLeft = value > 0;

        var cutObjTrans = Instantiate(cutObjPref).transform;
        var remainingObjTrans = Instantiate(remainingObjPref).transform;

        //SCALING
        var cutSize = baseObjTrans.localScale;
        if(isCutZ) cutSize.x = Mathf.Abs(value);
        else cutSize.z = Mathf.Abs(value);
        cutObjTrans.localScale = cutSize;

        var remainingSize = baseObjTrans.localScale;
        if(isCutZ) remainingSize.x = baseObjTrans.localScale.x - Mathf.Abs(value);
        else remainingSize.z = baseObjTrans.localScale.z - Mathf.Abs(value);
        remainingObjTrans.localScale = remainingSize;

        //POSITIONING
        var minusD = isCutZ ? Direction.Left : Direction.Back;
        var plusD = isCutZ ? Direction.Right : Direction.Front;

        var cutPosition = GetEdgePosition(baseObjMesh, isCutLeft ? minusD: plusD);
        if(isCutZ) cutPosition.x += cutSize.x / 2 * (isCutLeft ? 1 : -1);
        else cutPosition.z += cutSize.z / 2 * (isCutLeft ? 1 : -1);
        cutObjTrans.position = cutPosition; 

        var remainingPosition = GetEdgePosition(baseObjMesh, !isCutLeft ? minusD : plusD);
        if(isCutZ) remainingPosition.x += (remainingSize.x / 2) * (!isCutLeft ? 1: -1);
        else remainingPosition.z += (remainingSize.z / 2) * (!isCutLeft ? 1 : -1);
        remainingObjTrans.position = remainingPosition;

        lastObjTrans = remainingObjTrans;
        StartCoroutine(destroyCut(cutObjTrans.gameObject));
    }


    /*
     * GetEdgePosition gets the extents of object using its mesh
     */
    private Vector3 GetEdgePosition(MeshRenderer mesh, Direction direction)
    {
        var extents = mesh.bounds.extents;
        var position = mesh.transform.position;

        switch (direction)
        {
            case Direction.Left:
                position.x -= extents.x;
                break;
            case Direction.Right:
                position.x += extents.x;
                break;
            case Direction.Front:
                position.z += extents.z;
                break;
            case Direction.Back:
                position.z -= extents.z;
                break;
        }

        return position;
    }


    /*
     * destroyCut destroys the object that has been cut out
     */
    private IEnumerator destroyCut(GameObject lastObj)
    {
        yield return new WaitForSeconds(2);
        Destroy(lastObj);
    }
    

}
