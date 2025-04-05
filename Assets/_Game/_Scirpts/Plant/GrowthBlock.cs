using UnityEngine;

public class GrowthBlock : MonoBehaviour
{
    public enum GrowthStage
    {
        barren,
        ploughed,
        planted,
        growing1,
        growing2,
        ripe
    }

    public GrowthStage currentStage;
    [SerializeField] SpriteRenderer spriteRenderer;

    [Header("Sprite block")]
    [SerializeField] Sprite SoilTiled, WaterTiled;

    [Header("Crops/Seeds")]
    [SerializeField] SpriteRenderer cropRenderer;
    [SerializeField] Sprite planted, grow1, grow2, ripe;

    [Header("Grid Setup")]
    Vector2Int gridPosition;

    public bool isWatered { get; set; } = false;
    public bool isPlough { get; set; } = false;

    void Update()
    {
#if UNITY_EDITOR
        if(Input.GetKeyUp(KeyCode.N))
            GrowPlant();
#endif
    }

    void ChangeGrowthStage()
    {
        if (Input.GetKeyUp(KeyCode.E))
        {
            currentStage = currentStage + 1;
            if ((int)currentStage >= 6)
            {
                currentStage = GrowthStage.barren;
            }
        }
    }

    public void SetSoilSprite()
    {
        if (currentStage == GrowthStage.barren)
        {
            spriteRenderer.sprite = null;
        }
        else
        {
            if (isWatered)
                spriteRenderer.sprite = WaterTiled;
            else
                spriteRenderer.sprite = SoilTiled;
        }
    }

    public void PloughSoil()
    {
        if (currentStage == GrowthStage.barren)
        {
            currentStage = GrowthStage.ploughed;
            isPlough = true;
            SetSoilSprite();
            Debug.Log("Plough Soil");
        }
    }

    public void WaterSoil()
    {
        if (isPlough)
        {
            isWatered = true;
            SetSoilSprite();
            Debug.Log("Water Soil");
        }
    }

    public void UpdateCropSprite()
    {
        switch(currentStage)
        {
            case GrowthStage.planted:
                cropRenderer.sprite = planted;
                break;
            case GrowthStage.growing1:
                cropRenderer.sprite = grow1;
                break;
            case GrowthStage.growing2:
                cropRenderer.sprite = grow2;
                break;
            case GrowthStage.ripe:
                cropRenderer.sprite = ripe;
                break;
        }
    }

    public void PlantCrop()
    {
        if(currentStage == GrowthStage.ploughed && isWatered)
        {
            currentStage = GrowthStage.planted;
            UpdateCropSprite();
        }
    }

    public void HarvestCrop()
    {
        if(currentStage == GrowthStage.ripe)
        {
            currentStage = GrowthStage.ploughed;
            SetSoilSprite();
            cropRenderer.sprite = null;
        }
    }

    void GrowPlant()
    {
        if (isWatered)
        {
            if (currentStage == GrowthStage.planted || currentStage == GrowthStage.growing1 || currentStage == GrowthStage.growing2)
            {
                currentStage++;
                isWatered = false;
                SetSoilSprite();
                UpdateCropSprite();
            }
        }
    }

    public SpriteRenderer GetSpriteRenderer()
    {
        return spriteRenderer;
    }

    public void SetGridPosition(int x, int y)
    {
        gridPosition = new Vector2Int(x, y);
    }

    void UpdateGridInfo()
    {
        GridInfo.instance.UpdateInfo(this, gridPosition.x, gridPosition.y);
    }
}