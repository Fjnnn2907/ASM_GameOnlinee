using UnityEngine;

public class TempolaryPlayer : MonoBehaviour
{
    #region Copy hết đống này vào script player
    [SerializeField] GrowthBlock block;
    [SerializeField] ActionType type;

    void Update()
    {
        UseTool();   
    }

    /// <summary>
    /// Thay đống này bằng tương tác với enum của Inventory <see cref="ActionType"/>
    /// </summary>
    void UseTool()
    {
        switch(type)
        {
            case ActionType.Plough:
                if(Input.GetKeyUp(KeyCode.E))
                    block.PloughSoil();
                break;
            case ActionType.Water:
                if(Input.GetKeyUp(KeyCode.R))
                    block.WaterSoil();
                break;
            case ActionType.Seed:
                if(Input.GetKeyUp(KeyCode.Q))
                    block.PlantCrop();
                break;
            case ActionType.Basket:
                if(Input.GetKeyUp(KeyCode.T))
                    block.HarvestCrop();
                break;
        }
    }
    #endregion
}