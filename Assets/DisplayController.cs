using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayController : MonoBehaviour {

    byte[] lcd7segPattern = { 0x3F, 0x06, 0x5B, 0x4F, 0x66, 0x6D, 0x7D, 0x07, 0x7F, 0x6F, 0x77, 0x7C, 0x39, 0x5E, 0x79, 0x71, 0x07, 0x7F, 0x6F };

    byte[] lcd7segBigDigitOne = {0x06, 0x3B, 0x2F};


    void setSegmentState(Transform reference, bool active)
    {
        if ( active )
        {
            reference.GetChild(0).GetComponent<MeshRenderer>().material.EnableKeyword("_EMISSION");
            return;
        }
        reference.transform.GetChild(0).GetComponent<MeshRenderer>().material.DisableKeyword("_EMISSION");
    }

    bool getSegmentState(Transform reference)
    {
        return reference.transform.GetChild(0).GetComponent<MeshRenderer>().material.IsKeywordEnabled("_EMISSION");
    }





    void setDigitData(Transform reference, byte data)
    {
        for (int i = 0; i < 7; i++)
        {
            setSegmentState(reference.GetChild(i), (data & 1) == 1);
            data >>= 1;
        }
    }
    




    void setIndicatorABS(bool active){
        setSegmentState(GameObject.Find("indicatorsContainer/abs").transform, active);
    }
    void setIndicatorBattery(bool active)
    {
        setSegmentState(GameObject.Find("indicatorsContainer/battery").transform, active);
    }
    void setIndicatorInjection(bool active)
    {
        setSegmentState(GameObject.Find("indicatorsContainer/injection").transform, active);
    }
    void setIndicatorLeftArrow(bool active)
    {
        setSegmentState(GameObject.Find("indicatorsContainer/leftArrow").transform, active);
    }
    void setIndicatorLightHigh(bool active)
    {
        setSegmentState(GameObject.Find("indicatorsContainer/lightHigh").transform, active);
    }
    void setIndicatorLightLow(bool active)
    {
        setSegmentState(GameObject.Find("indicatorsContainer/lightLow").transform, active);
    }
    void setIndicatorNeutral(bool active)
    {
        setSegmentState(GameObject.Find("indicatorsContainer/neutral").transform, active);
    }
    void setIndicatorRightArrow(bool active)
    {
        setSegmentState(GameObject.Find("indicatorsContainer/rightArrow").transform, active);
    }
    void setIndicatorTemperature(bool active)
    {
        setSegmentState(GameObject.Find("indicatorsContainer/temperature").transform, active);
    }




    void setFuelIcon(bool active){
        setSegmentState(GameObject.Find("fuelIcon").transform, active);
    }

    void setFuelLevel(int level)
    {
        Transform fuelLevel = GameObject.Find("fuelLevel").transform;
        for (int i = 0; i < fuelLevel.childCount; i++){
            setSegmentState(fuelLevel.GetChild(i), i < level);
        }
    }









    void setGearIcon(bool active){
        setSegmentState(GameObject.Find("gearIndicator").transform, active);
    }

    void setGearValue(int value)
    {
        value = Mathf.Clamp(value, 0, 9);
        setDigitData(GameObject.Find("gearDigit").transform, value < 0? (byte)0 : lcd7segPattern[value]);
    }




    // isMetric (true == KM/H)
    void setSpeedUnit(bool isMetric, bool visible = true)
    {
        setSegmentState(GameObject.Find("indicatorTextContainer/kmh").transform, isMetric && visible);
        setSegmentState(GameObject.Find("indicatorTextContainer/mph").transform, !isMetric && visible);
    }


    void setDistanceUnit(bool isMetric, bool visible = true)
    {
        setSegmentState(GameObject.Find("indicatorTextContainer/km").transform, isMetric && visible);
        setSegmentState(GameObject.Find("indicatorTextContainer/miles").transform, !isMetric && visible);
    }



    void setTripOdometer(bool isTrip, bool visible = true)
    {
        setSegmentState(GameObject.Find("indicatorTextContainer/trip").transform, isTrip && visible);
        setSegmentState(GameObject.Find("indicatorTextContainer/odo").transform, !isTrip && visible);
    }

     


    void setDisplayGroove(bool active){
        setSegmentState(GameObject.Find("groove").transform, active);
    }

    void setDisplayDP(bool active){
        setSegmentState(GameObject.Find("miscDisplayDot").transform, active);
    }


    void setDisplayData(int digit, byte data){
        setDigitData(GameObject.Find("miscDisplayContainer/digitList").transform.GetChild(digit), data);
    }




    void setRPMLevel(int level)
    {
        Transform rpmMarkList = GameObject.Find("rpmMarkList").transform;
        for (int i = 0; i < rpmMarkList.childCount; i++)
        {
            setSegmentState(rpmMarkList.GetChild(i), i < level);
        }
    }


    void setRPMValue(int value)
    {
        value = Mathf.Clamp(value, 0, 12000); 
        setRPMLevel(value * 48 / 12000); 
    }






     



    void setBigDisplayData(int digit, byte data)
    {
        Transform speedContainer = GameObject.Find("speedContainer").transform;
        if ( digit == 0 ){
            for (int i = 0; i < speedContainer.GetChild(digit).childCount; i++)
            {
                setSegmentState(speedContainer.GetChild(digit).GetChild(i), (data & 0x1) == 1);
                data >>= 1;
            }
            return;
        }

        setDigitData(speedContainer.GetChild(digit), data);
    }



    void setBigDisplayValue(int value)
    {
        value = Mathf.Clamp(value, 0, 399);

        for (int i = 0; i < 3; i++)
        {
            int offset = (int)Math.Pow(10, 2-i);
            int digtiValue = value / offset % 10;

            if ( i == 0 )
            {
                digtiValue = Mathf.Clamp(digtiValue, 0, 3);
                setBigDisplayData(i, digtiValue == 0? (byte) 0x00 : lcd7segBigDigitOne[digtiValue-1]);
                continue;
            }

            if ( i == 2 )
            {
                setBigDisplayData(i, lcd7segPattern[digtiValue]);
                continue;
            }

            setBigDisplayData(i, digtiValue == 0 && value < offset? (byte)0x00 : lcd7segPattern[digtiValue]);

        } 

    }






    void setMiniDisplayValue(int value)
    {

        for (int i = 0; i < 6; i++)
        {
            int offset = (int)Math.Pow(10, 5 - i);
            int digtiValue = value / offset % 10;
            if ( i == 5)
            {
                setDisplayData(i, lcd7segPattern[digtiValue]);
                continue;
            }
            setDisplayData(i, digtiValue == 0 && value < offset ? (byte)0x00 : lcd7segPattern[digtiValue]);
        } 

    }


     




    void setBacklight(bool active)
    {
        setSegmentState(GameObject.Find("fuelMisc").transform, active);
        setSegmentState(GameObject.Find("gearBox").transform, active);
        setSegmentState(GameObject.Find("rpmMarkLine").transform, active);
    }






    IEnumerator LoadDefault()
    {
        setBacklight(true);

        

        // FUEL
        setFuelIcon(false);
        setFuelLevel(3);

        // GEAR
        setGearIcon(false);
        setGearValue(0);


        setSpeedUnit(true, true); // SPEED UNIT
        setDistanceUnit(true, true); // DISTENCE UNIT
        setTripOdometer(false, true); // TRIP or ODOMETER



        // MINI DISPLAY
        setDisplayGroove(false);
        setDisplayDP(true);

        setBigDisplayValue(0);
        setMiniDisplayValue(87506);



        //setRPMLevel(4);
        setRPMValue(1300);
        setFuelLevel(3);


        yield return new WaitForSeconds(1f);
        // CLUSTER INDICATORS
        setIndicatorABS(false);
        setIndicatorBattery(false);
        setIndicatorInjection(false);
        setIndicatorLeftArrow(false);
        setIndicatorLightHigh(false);
        setIndicatorLightLow(false);
        setIndicatorNeutral(true);
        setIndicatorRightArrow(false);
        setIndicatorTemperature(false);

    }









    IEnumerator fuelTest()
    {
        for (int i = 0; i <= 5; i++)
        {
            setFuelLevel(i);
            yield return new WaitForSeconds(0.01f);
        }
    }




    IEnumerator SelfTest()
    {

        setBacklight(true);

        // CLUSTER INDICATORS
        setIndicatorABS(true);
        setIndicatorBattery(true);
        setIndicatorInjection(true);
        setIndicatorLeftArrow(true);
        setIndicatorLightHigh(true);
        setIndicatorLightLow(true);
        setIndicatorNeutral(true);
        setIndicatorRightArrow(true);
        setIndicatorTemperature(true);

        // FUEL
        setFuelIcon(true);
        setFuelLevel(0);

        // GEAR
        setGearIcon(true);
        setGearValue(8);


        
        setSegmentState(GameObject.Find("indicatorTextContainer/kmh").transform, true);
        setSegmentState(GameObject.Find("indicatorTextContainer/mph").transform, true);
        setSegmentState(GameObject.Find("indicatorTextContainer/km").transform, true);
        setSegmentState(GameObject.Find("indicatorTextContainer/miles").transform, true);
        setSegmentState(GameObject.Find("indicatorTextContainer/trip").transform, true);
        setSegmentState(GameObject.Find("indicatorTextContainer/odo").transform, true);



        // MINI DISPLAY
        setDisplayGroove(true);
        setDisplayDP(true);

        setBigDisplayValue(88);
        setBigDisplayData(0, 0xFF);
        setMiniDisplayValue(888888);



        StartCoroutine(fuelTest());


        for (int i = 0; i<=48; i++)
        {
            setRPMLevel(i);
            yield return new WaitForSeconds(0.001f); 
        }

        yield return new WaitForSeconds(0.3f);

        for (int i = 0; i <= 48; i++)
        {
            setRPMLevel(48 - i);
            yield return new WaitForSeconds(0.001f);
        }

        yield return new WaitForSeconds(0.3f);
        StartCoroutine(LoadDefault()); 

        


    }




    int currentArrowState = 0;
    int currentLightState = 0;


    // Use this for initialization
    void Start () {

         

        StartCoroutine(SelfTest());

    }
	
	// Update is called once per frame
	void Update () {
		
        if (Input.GetKeyDown(KeyCode.LeftArrow)){
            currentArrowState = (currentArrowState == -1) ? 0 : -1;
        }else if (Input.GetKeyDown(KeyCode.RightArrow)){
            currentArrowState = (currentArrowState == 1) ? 0 : 1;
        }

        if (Input.GetKeyDown(KeyCode.F)){
            currentLightState = (currentLightState + 1) % 3;
        }
        
        setIndicatorLeftArrow(currentArrowState == -1);
        setIndicatorRightArrow(currentArrowState == 1);

        setIndicatorLightLow(currentLightState == 1);
        setIndicatorLightHigh(currentLightState == 2);




    }
}
