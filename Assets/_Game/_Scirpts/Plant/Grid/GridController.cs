using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class GridController : MonoBehaviour
{
    public static GridController instance;
    [CanBeNull]
    [SerializeField] Transform minPoint, maxPoint;
    [SerializeField] GrowthBlock baseBlock;
    private Vector2Int mGridSize;
    public List<BlockRow> blockRows = new List<BlockRow>();
    [SerializeField] LayerMask gridBlockers;

    void Awake()
    {
        instance = this;
        baseBlock = FindFirstObjectByType<GrowthBlock>();
    }

    void Start()
    {
        GenarateGrid();
    }

    void GenarateGrid()
    {
        minPoint.position = new Vector3(Mathf.Round(minPoint.position.x), Mathf.Round(minPoint.position.y), 0f);
        maxPoint.position = new Vector3(Mathf.Round(maxPoint.position.x), Mathf.Round(maxPoint.position.y), 0f);

        Vector3 startPoint = minPoint.position - new Vector3(0.5f, 0.5f, 0f);
        // Instantiate(baseBlock, startPoint, Quaternion.identity);

        mGridSize = new Vector2Int(Mathf.RoundToInt(maxPoint.position.x - minPoint.position.x),
         Mathf.RoundToInt(maxPoint.position.y - minPoint.position.y));
        for (int y = 0; y < mGridSize.y; y++)
        {
            blockRows.Add(new BlockRow());
            for (int x = 0; x < mGridSize.x; x++)
            {
                GrowthBlock newBlock = Instantiate(this.baseBlock, startPoint + new Vector3(x, y, 0f), Quaternion.identity);
                newBlock.transform.SetParent(transform);
                newBlock.GetSpriteRenderer().sprite = null;
                newBlock.SetGridPosition(x,y);
                blockRows[y].blocks.Add(newBlock);

                //Kiểm tra layer (gridBlockers) nếu trùng layer thì sprite = null
                if(Physics2D.OverlapBox((Vector2)newBlock.transform.position, new Vector2(0.9f, 0.9f), 0f, gridBlockers))
                {
                    newBlock.GetSpriteRenderer().sprite = null;
                    // newBlock.isPreventUse = true;
                }
                if(GridInfo.instance.hasGrid == true)
                {
                    BlockInfo storedBlock = GridInfo.instance.theGrid[y].blocks[x];
                    newBlock.currentStage = storedBlock.currentStage;
                    newBlock.isWatered = storedBlock.isWatered;

                    newBlock.SetSoilSprite();
                    newBlock.UpdateCropSprite();
                }
            }
        }

        if(GridInfo.instance.hasGrid == false)
        {
            GridInfo.instance.CreateGrid();
        }
        this.baseBlock.gameObject.SetActive(false);
    }

    public GrowthBlock GetBlock(float x, float y)
    {
        x = Mathf.RoundToInt(x);
        y = Mathf.RoundToInt(y);

        x -= minPoint.position.x;
        y -= minPoint.position.y;

        int intX = Mathf.RoundToInt(x);
        int intY = Mathf.RoundToInt(y);

        if(intX < mGridSize.x && intY < mGridSize.y)
        {
            return blockRows[intY].blocks[intX];
        }

        return null;
    }
}