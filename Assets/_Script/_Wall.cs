using UnityEngine;
using System.Collections;

public class _Wall : MonoBehaviour {

    public Sprite dmgSprite;   //攻击墙体后的图片
    public int hp = 4;

    private SpriteRenderer spriteRenderer;

	// Use this for initialization
	void Awake () {

        spriteRenderer = GetComponent<SpriteRenderer>();
	}
	
	public void DamageWall(int loss)
    {
        spriteRenderer.sprite = dmgSprite;     //攻击墙体后替换图片

        hp -= loss;
        if (hp <= 0)
            gameObject.SetActive(false);
    }
}
