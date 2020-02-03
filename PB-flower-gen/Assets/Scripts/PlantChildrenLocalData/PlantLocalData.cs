using UnityEngine;

public class PlantLocalData : MonoBehaviour
{
    #region Basic Properties

    public string PlantName { get; set; }
    public string Description { get; set; }
    public float Rotation { get; set; }
    public PlantFormType PlantFromType { get; set; }



    #endregion

    #region Constructor

    PlantLocalData(PlantFormData data)
    {
        PlantName = data.plantName;
    }

    #endregion
}
