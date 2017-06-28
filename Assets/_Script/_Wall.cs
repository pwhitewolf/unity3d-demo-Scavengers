using UnityEngine;
using System.Collections;

public class _Wall : MonoBehaviour {

    public Sprite dmgSprite;   //攻击墙体后的图片
    public int hp = 4;
    public AudioClip chopSound1;
    public AudioClip chopSound2;

    private SpriteRenderer spriteRenderer;

	// Use this for initialization
	void Awake () {

        spriteRenderer = GetComponent<SpriteRenderer>();
	}
	
	public void DamageWall(int loss)
    {
        spriteRenderer.sprite = dmgSprite;     //攻击墙体后替换图片
        _SoundManager.instance.RandomizeSfx(chopSound1, chopSound2);

        hp -= loss;
        if (hp <= 0)
            gameObject.SetActive(false);
    }
}
