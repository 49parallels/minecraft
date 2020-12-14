using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class BuildController : MonoBehaviour
{
    public World world;
    private Ray ray;
    private RaycastHit hit;

    private GameObject currentObject;
    private GameObject lastObject;

    private BlockType blockType = BlockType.yellow;

    [SerializeField] private GameObject placeHolderObject;
    private GameObject placeHolder;

    private BlockController blockController;

    private bool buildMode;
    
    


    // Start is called before the first frame update
    void Start()
    {
        placeHolder = Instantiate(placeHolderObject);
        placeHolder.transform.parent = world.transform;
    }

    // Update is called once per frame
    void Update()
    {
        CheckBuildModeToggle();
        BuildOrDestroy();
    }

    private void FixedUpdate()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
        //Debug.DrawRay(ray.origin, ray.direction * 10, Color.red);

        if (Physics.Raycast(ray, out hit, 5f))
        {
            currentObject = hit.transform.gameObject;
            if (!currentObject) return;
            currentObject.GetComponent<BlockController>().Select();;
            
            if (!lastObject) lastObject = currentObject;

            if (!lastObject.Equals(currentObject))
            {
                lastObject.GetComponent<BlockController>().Unselect();
                lastObject = currentObject;
            }
        }
    }

    private void LateUpdate()
    {
        if (currentObject)
            ShowBlockPlaceholder(currentObject.GetComponent<BlockController>().block.GetDefinitionData());
    }

    void BuildOrDestroy()
    {
        if (!currentObject) return;
        var currentBlockController = currentObject.GetComponent<BlockController>();

        if (Input.GetMouseButtonDown(0) && buildMode)
        {
            currentBlockController.AddBlock(placeHolder.transform.position, blockType);
        }

        if (Input.GetMouseButton(0) && !buildMode)
        {
            StartCoroutine("DestroyInTime");
        }
    }

    void CheckBuildModeToggle()
    {
        if (Input.GetKeyUp(KeyCode.B))
        {
            buildMode = !buildMode;
        }

        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            blockType = BlockType.red;
            buildMode = true;
        }

        if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            blockType = BlockType.green;
            buildMode = true;
        }

        if (Input.GetKeyUp(KeyCode.Alpha3))
        {
            blockType = BlockType.blue;
            buildMode = true;
        }

        if (Input.GetKeyUp(KeyCode.Alpha4))
        {
            blockType = BlockType.yellow;
            buildMode = true;
        }
    }

    void ShowBlockPlaceholder(Vector3 atPosition)
    {
        if (buildMode)
        {
            placeHolder.SetActive(true);
            placeHolder.transform.position = atPosition + Vector3.up;
        }
        else
        {
            placeHolder.SetActive(false);
        }
    }

    IEnumerator DestroyInTime()
    {
        yield return new WaitForSeconds(1.0f);
        if (currentObject) currentObject.GetComponent<BlockController>().GraduallyDestroyBlock();
    }
}
