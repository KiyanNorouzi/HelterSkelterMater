using UnityEngine;
using System.Collections;

public class SpriteCutTesr : MonoBehaviour {

	// Use this for initialization

    public SpriteRenderer sprRend;
    public Sprite spr;
    float val = 0;
    public float x = 0;
    public float y = 0;
    public float w = 900;
    public float h = 200;

	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        //val += Time.deltaTime * 10f;
        ////sprRend.sprite = Sprite.Create(spr.texture, new Rect(spr.rect.xMin, spr.rect.yMin + val, spr.rect.width, spr.rect.height), new Vector2(spr.rect.xMin + spr.rect.width / 2, spr.rect.yMin + spr.rect.height / 2));

        sprRend.sprite = Sprite.Create(spr.texture, new Rect(x, y, w, h), new Vector2(0.5f, 0.5f),  100);
	}
}
