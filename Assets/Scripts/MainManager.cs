using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class MainManager : Singleton<MainManager>
{
    [SerializeField] private GameObject[] interacable_buildings;
    public GameObject[] Interact_Point => interacable_buildings;
    [SerializeField] private GameObject TopViewCam;
    [SerializeField] private GameObject TopBtnCanvas;
    public static Dictionary<Status, Color> statusColorPair = new Dictionary<Status, Color> {
        {Status.None, new Color(0.5f, 0.5f, 0.5f, 1f) },
        {Status.Low, new Color(0,1,0,1f) },
        {Status.Moderate, new Color(1f,1f,0,1f) },
        {Status.High, new Color(1f,0.5f,0,1f) },
        {Status.Extreme, new Color(1,0,0,1f) }
        };

    public delegate void StandardUpdateEvent();
    public static event StandardUpdateEvent OnStandardUpdate;
    public delegate void TimeUpdateEvent();
    public static event TimeUpdateEvent OnTimeUpdate;

    public static Standard currentStandard = Standard.CO;
    [SerializeField] private GameObject playerController;
    
    public void UpdateBuildingData(int buildingID, string dataInJson)
    {
        Data data = JsonReader.LoadJsonFromString<Data>(dataInJson);
        interacable_buildings[buildingID].GetComponent<InteractivePoint>().SetData(data);
    }
    public void UpdateTime(string time)
    {
        UIManager.instance.SetTime(time);
        OnTimeUpdate?.Invoke();
    }
    public void TopViewSwitch()
    {
        VirtualCameraManager.Instance.SetCurrentVC(TopViewCam, null);
        playerController.SetActive(false);
        UIManager.instance.DisableChangeTransportation();
        UIManager.instance.DisableSwitchVirtualCamera();
        UIManager.instance.DisableTopViewBtn();
        UIManager.instance.EnableStandard();
        UIManager.instance.EnableCityTourBtn();
    }
    public void CityTourSwitch()
    {
        VirtualCameraManager.instance.SwitchToCityTour();
        UIManager.instance.SetSwitchControlBtnAvaliablity(BikeManager.instance.SwitchControlAvaliable());
        UIManager.instance.DisableStandard();
        UIManager.instance.DisableCityTourBtn();
        UIManager.instance.EnableChangeTransportation();
        UIManager.instance.DisableSwitchVirtualCamera();
        UIManager.instance.EnableTopViewBtn();
    }
    public void UpdateStandard(int option)
    {
        currentStandard = (Standard)option;
        OnStandardUpdate?.Invoke();
    }
    private Ray ray;
    private RaycastHit hit;
    private GameObject obj;
    private void Click()
    {
        Vector3 mousePos = Input.mousePosition;
        ray = Camera.main.ScreenPointToRay(mousePos);
        if (Physics.Raycast(ray, out hit))
        {
            obj = hit.collider.gameObject;
            if (obj.tag == "InteractivePoint")
            {
                obj.transform.parent.GetComponent<InteractivePoint>().Click();
            }
            else if(obj.tag == "InteractiveBuilding" && playerController.activeSelf)
            {
                BikeManager.instance.ChangeTourPoint(obj.GetComponent<InteractivePoint>().ID);
            }
        } 
    }
    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Click();
        }
    }

    #region react test
    [SerializeField] private GameObject debugWindow;
    [SerializeField] private TMP_Text debugText;
    public void OpenDebugWindow()
    {
        debugWindow.SetActive(true);
    }
    public void TestString(string msg)
    {
        debugText.text = "TestString: " + msg;
    }
    public void TestFloat(float msg)
    {
        debugText.text = "TestFloat: " + msg.ToString();
    }
    private void Start()
    {
        StartCoroutine(FakeDataUpdate());
        TopViewSwitch();
    }
    public IEnumerator FakeDataUpdate()
    {
        UpdateTime("2024/08/11 08:00");
        yield return new WaitForSeconds(1f);
        UpdateBuildingData(0,
            @"{
                ""title"" : ""Statue Of Liberty"",
                ""Electricity_Consumption"" : ""1123.42"",
                ""Water_Consumption"" : ""12372.12"",
                ""Scope_1_emission"" : ""122.12"",
                ""Scope_2_emission"" : ""3124.21"",
                ""Scope_3_emission"" : ""8765.87"",
                ""CO"" : ""0.2"",
                ""CO2"" : ""0.2"",
                ""NOx"" : ""45.82"",
                ""SO2"" : ""51.84"",
                ""PM10"" : ""11.25"",
                ""PM2point5"" : ""25.21""
            }");
        yield return new WaitForSeconds(1f);
        UpdateBuildingData(1,
            @"{
                    ""title"" : ""Empire State Building"",
                    ""Electricity_Consumption"" : ""2123.42"",
                    ""Water_Consumption"" : ""1272.12"",
                    ""Scope_1_emission"" : ""412.12"",
                    ""Scope_2_emission"" : ""224.21"",
                    ""Scope_3_emission"" : ""1076.87"",
                    ""CO"" : ""12.68"",
                    ""CO2"" : ""0.2"",
                    ""NOx"" : ""55.82"",
                    ""SO2"" : ""31.84"",
                    ""PM10"": ""22.25"",
                    ""PM2point5"" : ""12.21""
            }");
    }
    #endregion
}