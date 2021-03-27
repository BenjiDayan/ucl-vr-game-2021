using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class LineRendererSettings : MonoBehaviour
{
    //Declare a LineRenderer to store the component attached to the GameObject.
    [SerializeField] LineRenderer rend;

    //This will describe the path of the line
    Vector3[] points;

    public Button button;

    public LayerMask layerMask;

    InputDevice device;
    [SerializeField] InputFeatureUsage<bool> ClickButtonKeyVR = CommonUsages.gripButton;

    void Start() {
        rend = gameObject.GetComponent<LineRenderer>();

        //initialize the line renderer - 2 3D points describes a line
        points = new Vector3[2];
        points[0] = Vector3.zero;
        points[1] = transform.position + new Vector3(0, 0, 20); // pointing forwards
        rend.SetPositions(points);
        rend.enabled = true;

        var rightHandDevices = new List<InputDevice>();   
        InputDevices.GetDevicesAtXRNode(XRNode.RightHand, rightHandDevices);
        if(rightHandDevices.Count == 1)
        {
            device = rightHandDevices[0];
            Debug.Log("Found right hand device");

            var inputFeatures = new List<InputFeatureUsage>();
            if (device.TryGetFeatureUsages(inputFeatures)) {
                foreach (var feature in inputFeatures) {
                    if (feature.type == typeof(bool)) {
                        bool featureValue;
                        if (device.TryGetFeatureValue(feature.As<bool>(), out featureValue))
                        {
                            Debug.Log(string.Format("Bool feature '{0}''s value is '{1}'", feature.name, featureValue.ToString()));
                        }
                    }
                    else {
                        Debug.Log(string.Format("Non bool feature '{0}''s has type is '{1}'", feature.name, feature.type));
                    }
                }
            }
        }
        else {
            Debug.Log("No right hand devices!!");
        }
    }

    public bool AlignLineRenderer(LineRenderer rend) {
        Ray ray;
        ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        bool hitButton = false;

        if (Physics.Raycast(ray, out hit, layerMask)) {
            Debug.Log("line renderer hit something!");
            points[1] = transform.forward + new Vector3(0, 0, hit.distance);
            rend.startColor = Color.red;
            rend.endColor = Color.red;
            button = hit.collider.gameObject.GetComponent<Button>();
            hitButton = true;
        }
        else{
            Debug.Log("line renderer hitting nothing");
            points[1] = transform.forward + new Vector3(0, 0, 20);
            rend.startColor = Color.green;
            rend.endColor = Color.green;
            hitButton = false;
        }
        rend.SetPositions(points);
        rend.material.color = rend.startColor;
        return hitButton;
    }

    // Update is called once per frame
    void Update()
    {

        bool triggerValue;

        if (
            AlignLineRenderer(rend) && 
            (   
                Input.GetButtonDown("Fire1") ||
                (device.TryGetFeatureValue(ClickButtonKeyVR, out triggerValue) && triggerValue)
            )
        ) {
            button.onClick.Invoke();
        }
    }
}
