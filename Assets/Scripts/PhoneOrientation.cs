using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoneOrientation : MonoBehaviour
{
    [SerializeField]
    GameObject target;
    [SerializeField]
    Vector3 startRot = new Vector3(0, 0, 0);
    [SerializeField]
    Vector3 endRot = new Vector3(0, 0, 0);

    public bool invertX = false;
    public bool invertY = false;
    public bool invertZ = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //if (SensorInput.instance.isInitialized)
        //{
        //    Quaternion qat = Quaternion.Euler(-90, 0,0);
        //    if (SensorInput.instance.DeviceFound(SensorInput.attituteSensorLayout))
        //        //qat *= Quaternion.Inverse(SensorInput.instance.GetControlValue(SensorInput.instance.attitudeSensor.attitude));
        //        qat *= SensorInput.instance.GetControlValue(SensorInput.instance.attitudeSensor.attitude);

        //    Vector3 dir = 5 * (qat * Vector3.forward);
        //    transform.rotation = Quaternion.LookRotation(-dir, Vector3.up);
        //    transform.position = target.transform.position + dir;
        //}

        if (SensorInput.instance.isInitialized)
        {
            Quaternion qat = Quaternion.Euler(startRot);
            if (SensorInput.instance.DeviceFound(SensorInput.attituteSensorLayout))
                //qat *= Quaternion.Inverse(SensorInput.instance.GetControlValue(SensorInput.instance.attitudeSensor.attitude));
                qat *= SensorInput.instance.GetControlValue(SensorInput.instance.attitudeSensor.attitude);

            Vector3 euler = qat.eulerAngles;
            euler.x = -euler.x; // Invert the X-axis rotation

            if (invertX)
                euler.x = -euler.x;
            if (invertY)
                euler.y = -euler.y;
            if (invertZ)
                euler.z = -euler.z;

            qat = Quaternion.Euler(euler);

            qat *= Quaternion.Euler(endRot);
            transform.rotation = qat;
        }
    }
}
