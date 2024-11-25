using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirQualityStandard : MonoBehaviour
{
    public static Status CO2Quality(float data)
    {
        if (data < 0.5)
        {
            return Status.Low;
        }
        else if (data < 1)
        {
            return Status.Moderate;
        }
        else if (data < 1.5)
        {
            return Status.High;
        }
        else
        {
            return Status.Extreme;
        }
    }
    public static Status COQuality(float data)
    {
        if(data < 0.5)
        {
            return Status.Low;
        }
        else if(data < 1)
        {
            return Status.Moderate;
        }
        else if(data < 1.5)
        {
            return Status.High;
        }
        else
        {
            return Status.Extreme;
        }
    }
    public static Status NOxQuality(float data)
    {
        if (data <= 10)
        {
            return Status.Low;
        }
        else if (data < 50)
        {
            return Status.Moderate;
        }
        else if (data < 100)
        {
            return Status.High;
        }
        else
        {
            return Status.Extreme;
        }
    }
    public static Status SO2Quality(float data)
    {
        if (data <= 10)
        {
            return Status.Low;
        }
        else if (data < 50)
        {
            return Status.Moderate;
        }
        else if (data < 100)
        {
            return Status.High;
        }
        else
        {
            return Status.Extreme;
        }
    }
    public static Status PM10Quality(float data)
    {
        if (data <= 50)
        {
            return Status.Low;
        }
        else if (data < 100)
        {
            return Status.Moderate;
        }
        else if (data < 150)
        {
            return Status.High;
        }
        else
        {
            return Status.Extreme;
        }
    }
    public static Status PM2point5Quality(float data)
    {
        if (data <= 25)
        {
            return Status.Low;
        }
        else if (data < 50)
        {
            return Status.Moderate;
        }
        else if (data < 75)
        {
            return Status.High;
        }
        else
        {
            return Status.Extreme;
        }
    }
}

public enum Standard
{
    CO2,
    CO,
    NOx,
    SO2,
    PM2point5,
    PM10
}