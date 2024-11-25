using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq.Expressions;

public class UIManager : Singleton<UIManager>
{
    [Header("Data UI")]
    [SerializeField] private TMP_Text Title;
    [SerializeField] private TMP_Text Electricity_Consumption;
    [SerializeField] private TMP_Text Water_Consumption;
    [SerializeField] private TMP_Text Scope_1_emission;
    [SerializeField] private TMP_Text Scope_2_emission;
    [SerializeField] private TMP_Text Scope_3_emission;
    [SerializeField] private TMP_Text CO;
    [SerializeField] private TMP_Text CO2;
    [SerializeField] private TMP_Text NOx;
    [SerializeField] private TMP_Text SO2;
    [SerializeField] private TMP_Text PM10;
    [SerializeField] private TMP_Text PM2point5;
    [SerializeField] private Animator DataUIAnimator;
    [SerializeField] private Animator BikeUIAnimator;
    [SerializeField] private TMP_Text currentLocText;
    [SerializeField] private TMP_Dropdown destDropDown;
    [SerializeField] private Button SwitchCamBtn;
    [SerializeField] private Button TopViewBtn;
    [SerializeField] private Button CityTourBtn;
    [SerializeField] private TMP_Dropdown changeTransportationDropDown;
    [SerializeField] private TMP_Dropdown standardDropDown;
    [SerializeField] private TMP_Text Time;
    [SerializeField] private Button SwitchControlBtn;

    private void Start()
    {
        changeTransportationDropDown.onValueChanged.AddListener(BikeManager.instance.ChangeTransportation);
        standardDropDown.onValueChanged.AddListener(MainManager.instance.UpdateStandard);
        List<string> destList = new List<string>();
        for(int i = 0; i < MainManager.instance.Interact_Point.Length; i++)
        {
            destList.Add(MainManager.instance.Interact_Point[i].GetComponent<InteractivePoint>().Name);
        }
        destDropDown.AddOptions(destList);
    }
    public void UpdateDataUI(Data data)
    {
        Title.text = data.title;
        Electricity_Consumption.text = data.Electricity_Consumption.ToString();
        Water_Consumption.text = data.Water_Consumption.ToString();
        Scope_1_emission.text = data.Scope_1_emission.ToString();
        Scope_2_emission.text = data.Scope_2_emission.ToString();
        Scope_3_emission.text = data.Scope_3_emission.ToString();
        CO2.text = data.CO2.ToString() + " " + AirQualityStandard.COQuality(data.CO2);
        CO.text = data.CO.ToString() + " " + AirQualityStandard.COQuality(data.CO);
        NOx.text = data.NOx.ToString() + " " + AirQualityStandard.NOxQuality(data.NOx);
        SO2.text = data.SO2.ToString() + " " + AirQualityStandard.SO2Quality(data.SO2);
        PM10.text = data.PM10.ToString() + " " + AirQualityStandard.PM10Quality(data.PM10);
        PM2point5.text = data.PM2point5.ToString() + " " + AirQualityStandard.PM2point5Quality(data.PM2point5);
    }
    public void ShowDataUIPanel()
    {
        if(DataUIEnabled())
        {
            return;
        }
        DataUIAnimator.SetTrigger("FadeIn");
    }
    public void HideDataUIPanel()
    {
        if (!DataUIEnabled())
        {
            return;
        }
        DataUIAnimator.SetTrigger("FadeOut");
    }
    public bool DataUIEnabled()
    {
        return DataUIAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Idle" || DataUIAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "FadeIn";
    }
    public bool BikeUIEnabled()
    {
        return BikeUIAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Idle" || BikeUIAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "FadeIn";
    }
    public void ShowBikeDestUIPanel()
    {
        BikeUIAnimator.SetTrigger("FadeIn");
        DisableSwitchVirtualCamera();
        VirtualCameraManager.instance.ChangeCameraViewToDefault();
    }
    public void HideBikeDestUIPanel()
    {
        BikeUIAnimator.SetTrigger("FadeOut");
        EnableSwitchVirtualCamera();
        VirtualCameraManager.instance.ChangeCameraViewToDefault();
    }
    public void UpdateBikeDestUIPanel(int dest)
    {
        currentLocText.text = MainManager.instance.Interact_Point[dest].GetComponent<InteractivePoint>().Name;
    }
    public string GetCurrentOption(int option)
    {
        return destDropDown.options[option].text;
    }
    public void EnableChangeTransportation()
    {
        changeTransportationDropDown.interactable = true;
    }
    public void DisableChangeTransportation()
    {
        changeTransportationDropDown.interactable = false;
    }
    public void DisableStandard()
    {
        standardDropDown.interactable = false;
    }
    public void EnableStandard()
    {
        standardDropDown.interactable = true;
    }
    public void DisableTopViewBtn()
    {
        TopViewBtn.interactable = false;
    }
    public void EnableTopViewBtn()
    {
        TopViewBtn.interactable = true;
    }
    public void DisableCityTourBtn()
    {
        CityTourBtn.interactable = false;
    }
    public void EnableCityTourBtn()
    {
        CityTourBtn.interactable = true;
    }
    public void DisableSwitchVirtualCamera()
    {
        SwitchCamBtn.interactable = false;
    }
    public void EnableSwitchVirtualCamera()
    {
        SwitchCamBtn.interactable = true;
    }
    public void SetTime(string time)
    {
        Time.text = time;
    }
    public void SetSwitchControlBtnAvaliablity(bool enable)
    {
        SwitchControlBtn.gameObject.SetActive(enable);
    }
}
