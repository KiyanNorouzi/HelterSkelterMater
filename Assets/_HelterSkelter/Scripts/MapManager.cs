using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapManager : MonoBehaviour
{
    public GameObject policePrefab;

    public Waypoint alarmWaypoint;

    public Waypoint policeStartWaypoint;

    public static List<string> playerWeaponTargetTags = new List<string> { "Civil", "Police" };

    public static MapManager Instance;

    public LayerMask rayCastLayer_Corpse;

    public static string corpseRayColTag = "CorpseRayCol";

    public LayerMask rayCastLayer_Player;

    public static string playerRayColTag = "PlayerRayCol";

    public delegate void CorpseDetected(Corpse _corpse);
    public event CorpseDetected Event_CorpseDetected;

    public delegate void CorpseAdded(Corpse _corpse);
    public event CorpseAdded Event_CorpseAdded;

    public delegate void CorpseRemoved(Corpse _corpse);
    public event CorpseRemoved Event_CorpseRemoved;

    public GUIControl guiCtrl_Busted;

    [HideInInspector]
    public List<MapSection> sections = new List<MapSection>();

    [HideInInspector]
    public Waypoint[] allWaypoints;

    [HideInInspector]
    public List<Waypoint> civilWaypoints = new List<Waypoint>();

    [HideInInspector]
    public List<Waypoint> policeWaypoints = new List<Waypoint>();

    [HideInInspector]
    public List<Corpse> allCorpses = new List<Corpse>();

    [HideInInspector]
    public List<Corpse> NotReportedIdentifiedCorpses = new List<Corpse>();

    bool isPolicePresent = false;

    bool isAlarmBusy = false;

    bool isPlayerDetected = false;

    float showBustedTextSpeed = 0.7f;
    bool shouldShowBustedText = false;
    float bustedTextMaxAlpha = 1f;

    private float restartLevelTimeCounter = 3f;

    [HideInInspector]
    public bool isLevelFailed = false;

    bool shouldRecreatePolice = false;
    float recreatePoliceDelayTime = 2f;
    float recreatePoliceTimeCounter = 0;

    void Awake()
    {
        Instance = this;

        allWaypoints = GameObject.FindObjectsOfType(typeof(Waypoint)) as Waypoint[];

        for (int i = 0; i < allWaypoints.Length; i++)
        {
            switch (allWaypoints[i].waypointType)
            {
                case WaypointType.POLICE:
                    policeWaypoints.Add(allWaypoints[i]);
                    break;

                case WaypointType.CIVIL:
                    civilWaypoints.Add(allWaypoints[i]);
                    break;
            }
        }
    }

    // Use this for initialization
    void Start()
    {
        AudioManager.Instance.Play(SoundType.Ambient_InGame, true);
    }

    // Update is called once per frame
    void Update()
    {
        if (shouldRecreatePolice)
        {
            recreatePoliceTimeCounter = MathFPlus.DecDeltaTimeToZero(recreatePoliceTimeCounter);

            if (recreatePoliceTimeCounter == 0)
            {
                shouldRecreatePolice = false;

                GameObject.Instantiate(policePrefab, policeStartWaypoint.transform.position, Quaternion.identity);
            }
        }

        if (shouldShowBustedText)
        {
            if (guiCtrl_Busted.alpha < bustedTextMaxAlpha)
            {
                guiCtrl_Busted.alpha += showBustedTextSpeed * Time.deltaTime;

                if (guiCtrl_Busted.alpha > bustedTextMaxAlpha)
                {
                    guiCtrl_Busted.alpha = bustedTextMaxAlpha;
                }

                guiCtrl_Busted.ChangeAlpha();
            }
        }

        if (isLevelFailed)
        {
            restartLevelTimeCounter = MathFPlus.DecDeltaTimeToZero(restartLevelTimeCounter);

            if (restartLevelTimeCounter == 0)
            {
                restartLevelTimeCounter = 1000000f;
                Application.LoadLevel(1); //<<< for test
            }
        }
    }

    public void AddCorpse(Corpse _corpse)
    {
        Corpse corpse = _corpse;

        if (allCorpses.Contains(corpse))
            return;

        allCorpses.Add(corpse);

        if (Event_CorpseAdded != null)
            Event_CorpseAdded(corpse);
    }

    public void AddCorpseToNotReportedIdentifiedList(Corpse _corpse)
    {
        Corpse corpse = _corpse;

        if (!allCorpses.Contains(corpse))
        {
            Debug.LogError("Keyfin user?! Sani corpsan moshkeliddih. Corpsin yokhde in all corpse listun.");

            return;
        }

        if (NotReportedIdentifiedCorpses.Contains(corpse))
            return;

        NotReportedIdentifiedCorpses.Add(corpse);

        if (Event_CorpseDetected != null)
            Event_CorpseDetected(corpse);

    }

    public void RemoveCorpse(Corpse _corpse)
    {
        Corpse corpse = _corpse;

        if (corpse == null)
            return;

        if (!allCorpses.Contains(corpse))
        {
            Debug.LogError("Are you freaking kidding me?! That damn corpse doesn't exist in the list.");
            return;
        }

        allCorpses.Remove(corpse);

        if (NotReportedIdentifiedCorpses.Contains(corpse))
        {
            NotReportedIdentifiedCorpses.Remove(corpse);
        }

        if (Event_CorpseRemoved != null)
            Event_CorpseRemoved(corpse);

        Destroy(corpse.gameObject);

        corpse = null;
    }

    public bool IsAlarmBusy()
    {
        return isAlarmBusy;
    }

    public void SetAlarmIsBusy(bool _val)
    {
        isAlarmBusy = _val;
    }

    public bool IsPolicePresent()
    {
        return isPolicePresent;
    }

    public void SetPoliceIsPresent(bool _val)
    {
        isPolicePresent = _val;
    }

    public void SetPlayerIsDetected()
    {
        if (isPlayerDetected)
            return;

        isPlayerDetected = true;

        MultiTouchManager.Instance.SetInGameButtonsSikiminEnabled(false);
    }

    public bool IsPlayerDetected()
    {
        return isPlayerDetected;
    }

    public void SetLevelIsFailed()
    {
        isLevelFailed = true;

        StartShowingBustedText();
    }

    public void StartShowingBustedText()
    {
        shouldShowBustedText = true;

        guiCtrl_Busted.alpha = 0;
        guiCtrl_Busted.ChangeAlpha();

        guiCtrl_Busted.isVisible = true;
        guiCtrl_Busted.ChangeVisibility();
    }

    public void ShouldRecreatePolice()
    {
        if (shouldRecreatePolice)
            return;

        shouldRecreatePolice = true;

        recreatePoliceTimeCounter = recreatePoliceDelayTime;
    }
}
