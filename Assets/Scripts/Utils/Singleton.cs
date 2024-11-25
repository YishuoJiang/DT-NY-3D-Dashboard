using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    protected static T instance;
    public static T Instance
    {
        get { return instance; }
    }

    protected virtual void Awake()
    {       
        if (instance == null)
        {
            instance = (T)this;
        }
        else
        {
            Debug.Log(this.gameObject.name);
            Debug.LogError("More Than One Instance of Singleton!");
        }
    }
}
