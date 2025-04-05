using UnityEngine;

public class TempolaryPlayer : MonoBehaviour
{
    [SerializeField] GrowthBlock block;
    [SerializeField] ActionType type;

    void Update()
    {
        UseTool();   
    }

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
}