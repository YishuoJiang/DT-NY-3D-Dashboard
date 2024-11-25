using KikiNgao.SimpleBikeControl;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BikeManager : Singleton<BikeManager>
{
    [SerializeField] private GameObject AJ;
    [SerializeField] private GameObject Bike;
    [SerializeField] private GameObject Car;
    [SerializeField] private GameObject PlayerController;
    public bool inCityTourModel => GetCurrentDestVC().activeSelf;
    private GameObject currentAI;
    private int currentDest;
    public int CurrentDest => currentDest;
    // Start is called before the first frame update
    void Start()
    {
        currentDest = 0;
        currentAI = AJ;
        UIManager.Instance.UpdateBikeDestUIPanel(currentDest);
    }

    public void ChangeTourPoint(int option)
    {
        SetPlayerController(false);
        VirtualCameraManager.instance.currentVirtualCamera.SetActive(false);
        VirtualCameraManager.instance.currentVirtualCamera = GetCurrentAIVC();
        Transform dest = MainManager.instance.Interact_Point[option].transform.Find("Dest");
        currentAI.GetComponent<NavMeshAgent>().isStopped = false;
        if (currentAI.GetComponent<Animator>())
        {
            currentAI.GetComponent<Animator>().SetFloat("Speed", 10);
        }
        currentAI.GetComponent<NavMeshAgent>().SetDestination(dest.Find("DestLoc").position);
        dest.Find("DestVC").gameObject.SetActive(false);
        UIManager.Instance.HideBikeDestUIPanel();
        UIManager.instance.HideDataUIPanel();
        UIManager.instance.DisableChangeTransportation();
        UIManager.instance.DisableCityTourBtn();
        UIManager.instance.DisableTopViewBtn();
        currentDest = option;
        StartCoroutine(StopRunning(dest.gameObject));
    }
    public void ChangeTransportation(int option)
    {
        switch (option)
        {
            case 0:
                AJ.transform.position = currentAI.transform.position;
                AJ.transform.rotation = currentAI.transform.rotation;
                PlayerController.SetActive(true);
                AJ.GetComponent<Biker>().ExitBike();
                currentAI.SetActive(false);
                currentAI = AJ;
                AJ.GetComponent<NavMeshAgent>().enabled = true;
                currentAI.SetActive(true);
                break;
            case 1:
                Bike.transform.position = currentAI.transform.position;
                Bike.transform.rotation = currentAI.transform.rotation;
                currentAI.SetActive(false);
                AJ.SetActive(true);
                AJ.GetComponent<Biker>().EnterBike();
                AJ.GetComponent<NavMeshAgent>().enabled = false;
                currentAI = Bike;
                currentAI.SetActive(true);
                break;
            case 2:
                Car.transform.position = currentAI.transform.position;
                Car.transform.rotation = currentAI.transform.rotation;
                currentAI.SetActive(false);
                currentAI = Car;
                currentAI.SetActive(true);
                break;
        }
        UIManager.instance.SetSwitchControlBtnAvaliablity(SwitchControlAvaliable());
        SetPlayerController(false);
        VirtualCameraManager.instance.UpdateVCDuringBikeTour();
    }
    private IEnumerator StopRunning(GameObject dest)
    {
        currentAI.transform.Find("Virtual Cameras").gameObject.SetActive(true);
        Vector3 destPosition = dest.transform.Find("DestLoc").position;
        while (Vector3.Distance(currentAI.transform.position, destPosition) > 0.5f)
        {
            Debug.Log(Vector3.Distance(currentAI.transform.position, dest.transform.Find("DestLoc").position));
            yield return null;
        }
        if (currentAI.GetComponent<Animator>())
        {
            currentAI.GetComponent<Animator>().SetFloat("Speed", 0);
        }
        currentAI.GetComponent<NavMeshAgent>().isStopped = true;
        UIManager.Instance.UpdateBikeDestUIPanel(currentDest);
        UIManager.Instance.ShowBikeDestUIPanel();
        currentAI.transform.Find("Virtual Cameras").gameObject.SetActive(false);
        dest.transform.Find("DestVC").gameObject.SetActive(true);
        UIManager.Instance.UpdateDataUI(MainManager.instance.Interact_Point[currentDest].GetComponent<InteractivePoint>().Data);
        UIManager.Instance.ShowDataUIPanel();
        UIManager.instance.EnableChangeTransportation();
        UIManager.instance.DisableCityTourBtn();
        UIManager.Instance.EnableTopViewBtn();
    }
    public GameObject GetCurrentAIVC()
    {
        return currentAI.transform.Find("Virtual Cameras").gameObject;
    }
    public GameObject GetCurrentDestVC()
    {
        return MainManager.instance.Interact_Point[currentDest].transform.Find("Dest").Find("DestVC").gameObject;
    }
    public bool SwitchControlAvaliable()
    {
        return currentAI == AJ;
    }
    public void SetPlayerController(bool enable)
    {
        if(enable)
        {
            UIManager.instance.HideDataUIPanel();
            UIManager.instance.DisableTopViewBtn();
        }
        else
        {
            if (currentAI.GetComponent<Animator>())
            {
                currentAI.GetComponent<Animator>().SetFloat("Speed", 0);
            }
            UIManager.instance.EnableTopViewBtn();
        }
        GetCurrentDestVC().SetActive(!enable);
        PlayerController.SetActive(enable);
    }
    public void OnSwitchControl()
    {
        SetPlayerController(!PlayerController.activeSelf);
        if(PlayerController.activeSelf)
        {
            UIManager.instance.DisableChangeTransportation();
        }
        else
        {
            UIManager.instance.ShowDataUIPanel();
            UIManager.instance.EnableChangeTransportation();
        }
    }
}
