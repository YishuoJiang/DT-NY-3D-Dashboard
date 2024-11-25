using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class InteractivePoint : MonoBehaviour
{
    [SerializeField] private int id;
    public int ID => id;
    private Data data = new Data();
    public Data Data => data;
    public string Name => this.gameObject.name;
    private GameObject interactivePoint;
    private GameObject virtualCameras;
    private void Awake()
    {
        interactivePoint = GameObject.Instantiate(Resources.Load<GameObject>("InteractivePoint"), 
            new Vector3(this.transform.position.x, 700, this.transform.position.z), Quaternion.identity, this.transform);
        interactivePoint.GetComponent<MeshRenderer>().material = new Material(Resources.Load<Shader>("Shader/Wave"));
        interactivePoint.transform.localScale = new Vector3(100f/this.transform.lossyScale.x, 
            1f/this.transform.lossyScale.y, 100f/this.transform.lossyScale.z);
        UpdateColor();
        virtualCameras = transform.Find("Virtual Cameras").gameObject;
        MainManager.OnStandardUpdate += UpdateColor;
        MainManager.OnTimeUpdate += InitializeData;
    }
    public void SetData(Data data)
    {
        this.data = data;
        UpdateColor();
    }
    public void InitializeData()
    {
        this.data = new Data();
        UpdateColor();
    }
    public void UpdateColor()
    {
        Status state = Status.None;
        if(data.title != "Loading")
        {
            switch(MainManager.currentStandard)
            {
                case Standard.CO2:
                    state = AirQualityStandard.COQuality(data.CO2);
                    break;
                case Standard.CO:
                    state = AirQualityStandard.COQuality(data.CO);
                    break;
                case Standard.SO2:
                    state = AirQualityStandard.SO2Quality(data.SO2);
                    break;
                case Standard.NOx:
                    state = AirQualityStandard.NOxQuality(data.NOx);
                    break;
                case Standard.PM2point5:
                    state = AirQualityStandard.PM2point5Quality(data.PM2point5);
                    break;
                case Standard.PM10:
                    state = AirQualityStandard.PM10Quality(data.PM10);
                    break;
            }
        }
        interactivePoint.GetComponent<MeshRenderer>().material.color = MainManager.statusColorPair[state];
    }
    public void Click()
    {
        VirtualCameraManager.Instance.SetCurrentVC(virtualCameras, data);
        UIManager.Instance.EnableTopViewBtn();
        UIManager.Instance.EnableSwitchVirtualCamera();
        UIManager.Instance.DisableStandard();
    }
}

public enum Status
{
    None,
    Low,
    Moderate,
    High,
    Extreme
}
