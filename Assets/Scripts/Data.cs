
using System;

[Serializable]
public class Data
{
    public string title;
    public float Electricity_Consumption;
    public float Water_Consumption;
    public float Scope_1_emission;
    public float Scope_2_emission;
    public float Scope_3_emission;
    public float CO;
    public float CO2;
    public float NOx;
    public float SO2;
    public float PM10;
    public float PM2point5;
    public Data() 
    {
        title = "Loading";
        Electricity_Consumption = 0;
        Water_Consumption = 0;
        Scope_1_emission = 0;
        Scope_2_emission = 0;
        Scope_3_emission = 0;
        CO = 0;
        CO2 = 0;
        NOx = 0;
        SO2 = 0;
        PM10 = 0;
        PM2point5 = 0;
    }
}
