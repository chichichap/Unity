using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController main;
    private VRInteractable pointedObject;
    private OVRScreenFade ovrScreenFade;

    public VRInteractable PointedObject
    {
        get
        {
            return pointedObject;
        }

        set
        {
            if (pointedObject == null && value != null) {
                value.Highlight();
            } else if (pointedObject != null && value == null && pointedObject.IsInteractable) {
                pointedObject.Unhighlight();
            }

            pointedObject = value;

            DebugLogController.main.Log("pointed = " + pointedObject);
        }
    }
    public OVRScreenFade OvrScreenFade
    {
        get
        {
            return ovrScreenFade;
        }

        set
        {
            ovrScreenFade = value;
        }
    }

    private void Awake()
    {
        main = this;
        DontDestroyOnLoad(this.gameObject);

        OvrScreenFade = transform.Find("TrackingSpace/CenterEyeAnchor").GetComponent<OVRScreenFade>();
    }

    private void Update()
    {
        if (pointedObject != null && OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger)) {
            if (pointedObject is VROption)
                ConversationController.main.SelectOption(((VROption)pointedObject).OptionIndex);
        }
    }
}
