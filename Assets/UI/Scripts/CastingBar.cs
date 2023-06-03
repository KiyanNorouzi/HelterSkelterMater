using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class CastingBar : MonoBehaviour
{
    private Vector3 startPos;
    private Vector3 endPos;

    private Image castImage;

    private RectTransform castTransform;

    private bool casting;

    public float fadeSpeed;


    private Rage angry = new Rage("Angry", 2f, Color.red);
    private Rage normal = new Rage("Normal", 2f, Color.yellow);
    private Rage easy = new Rage("Easy", 2f, Color.green);

    void Start()
    {
        casting = false;
        castTransform = GetComponent<RectTransform>();
        castImage = GetComponent<Image>();

        endPos = castTransform.position;
        startPos = new Vector3(castTransform.position.x - castTransform.rect.width, castTransform.position.y, castTransform.position.z);

    }


    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            StartCoroutine(CastRage(angry));
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            StartCoroutine(CastRage(normal));
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            StartCoroutine(CastRage(easy));
        }

    }




    private IEnumerator CastRage(Rage rage)
    {
        if (!casting)
        {
            casting = true;
            castImage.color = rage.rageColor;
            castTransform.position = startPos;
            float timeLeft = Time.deltaTime;
            float rate = 1.0f / rage.castTime;
            float progress = 0.0f;


            while (progress <= 1.0)
            {
                castTransform.position = Vector3.Lerp(startPos, endPos, progress);
                progress += rate * Time.deltaTime;
                timeLeft += Time.deltaTime;

                yield return null;
            }
            castTransform.position = endPos;
            casting = false;

        }


       
    }



}
