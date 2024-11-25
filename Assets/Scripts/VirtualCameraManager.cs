using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class VirtualCameraManager : Singleton<VirtualCameraManager>
{
    [SerializeField] private CinemachineBrain brain;
    [SerializeField] private GameObject TopViewCam;
    [HideInInspector] public GameObject currentVirtualCamera;

    public static bool isTopView = true;
    public void SetCurrentVC(GameObject currentVC, Data data)
    {
        StopAllCoroutines();
        StartCoroutine(SwitchVCCoroutine(currentVC, data));
    }
    private IEnumerator SwitchVCCoroutine(GameObject currentVC, Data data)
    {
        if (UIManager.instance.BikeUIEnabled())
        {
            UIManager.instance.HideBikeDestUIPanel();
        }
        if (UIManager.instance.DataUIEnabled())
        {
            UIManager.Instance.HideDataUIPanel();
            yield return new WaitForSeconds(1.5f);
        }
        Camera.main.orthographic = (currentVC == TopViewCam);
        if (currentVirtualCamera)
        {
            currentVirtualCamera.SetActive(false);
        }
        currentVirtualCamera = currentVC;
        currentVirtualCamera.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        while (brain.IsBlending)
        {
            yield return null;
        }
        if (data != null)
        {
            UIManager.Instance.UpdateDataUI(data);
            UIManager.Instance.ShowDataUIPanel();

        }
    }

    //UIManager.Instance.UpdateDataUI(JsonReader.LoadJsonFromFile<Data>(Application.dataPath + "/Data/statues_of_liberty.json"));
    public void UpdateVCDuringBikeTour()
    {
        currentVirtualCamera = BikeManager.instance.GetCurrentAIVC();
    }
    public void SwitchToCityTour()
    {
        StartCoroutine(switchToCityTour());
    }
    private IEnumerator switchToCityTour()
    {
        if (UIManager.instance.DataUIEnabled())
        {
            UIManager.Instance.HideDataUIPanel();
            yield return new WaitForSeconds(1.5f);
        }
        currentVirtualCamera.SetActive(false);
        Camera.main.orthographic = false;
        BikeManager.instance.GetCurrentDestVC().SetActive(true);
        currentVirtualCamera = BikeManager.instance.GetCurrentDestVC();
        yield return new WaitForSeconds(0.1f);
        while (brain.IsBlending)
        {
            yield return null;
        }
        UIManager.Instance.UpdateDataUI(MainManager.instance.Interact_Point[BikeManager.instance.CurrentDest].GetComponent<InteractivePoint>().Data);
        UIManager.Instance.ShowDataUIPanel();
        UIManager.Instance.ShowBikeDestUIPanel();
        yield return new WaitForSeconds(1.5f);
    }
    public void ChangeCameraView()
    {
        Debug.Log(currentVirtualCamera.transform.parent.name);
        int CamCount = currentVirtualCamera.transform.childCount;
        if (CamCount <= 1)
        {
            return;
        }
        for (int i = 0; i < CamCount; i++)
        {
            if (currentVirtualCamera.transform.GetChild(i).gameObject.activeSelf)
            {
                currentVirtualCamera.transform.GetChild(i).gameObject.SetActive(false);
                i++;
                if (i == CamCount)
                {
                    i = 0;
                }
                currentVirtualCamera.transform.GetChild(i).gameObject.SetActive(true);
                break;
            }
        }
    }
    public void ChangeCameraViewToDefault()
    {
        int CamCount = currentVirtualCamera.transform.childCount;
        if (CamCount <= 1)
        {
            return;
        }
        for (int i = 0; i < CamCount; i++)
        {
            if (currentVirtualCamera.transform.GetChild(i).gameObject.activeSelf)
            {
                currentVirtualCamera.transform.GetChild(i).gameObject.SetActive(false);
                break;
            }
        }
        currentVirtualCamera.transform.GetChild(0).gameObject.SetActive(true);
    }
}
