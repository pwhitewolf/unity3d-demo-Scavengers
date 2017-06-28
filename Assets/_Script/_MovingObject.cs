using UnityEngine;
using System.Collections;

//抽象类，可以创建不完整的类及类成员,在派生类中实现
public abstract class _MovingObject : MonoBehaviour {


    public float moveTime = 0.1f;
    public LayerMask blockingLayer;

    private BoxCollider2D boxCollider;
    private Rigidbody2D rb2D;
    private float inverseMoveTime;    //让移动计算更有效率,乘法


	// 可被继承类重写
	protected virtual void Start ()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();
        inverseMoveTime = 1f / moveTime;
        
	}

    protected bool Move(int xDir,int yDir,out RaycastHit2D hit)
    {
        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(xDir, yDir);

        boxCollider.enabled = false;   //禁用自身碰撞器，防止射线碰撞到

        hit = Physics2D.Linecast(start, end, blockingLayer);   //从start到end的射线，在blockingLayer层检测碰撞

        boxCollider.enabled = true;

        if (hit.transform == null)
        {
            StartCoroutine(SmoothMovement(end));    //路线无碰撞则运行协程
            return true;
        }

        return false;
    }

    //协程，让单位从一个位置移动到下一个位置
    protected IEnumerator SmoothMovement(Vector3 end)
    {
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

        while (sqrRemainingDistance > float.Epsilon)
        {
            Vector3 newPosition = Vector3.MoveTowards(transform.position, end, inverseMoveTime * Time.deltaTime);
            rb2D.MovePosition(newPosition);
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;

            yield return null;  //等一帧后，再重新判断是否满足循环条件
        }
    }

    protected virtual void AttempMove<T>(int xDir,int yDir) where T:Component    //使用泛型是因为主角与敌人均继承该脚本，两者交互的对象不同。这里不知道hit后的Component类型。使用泛型可获取其引用并传递给OnCantMove()
    {
        RaycastHit2D hit;
        bool canMove = Move(xDir, yDir, out hit);

        if (hit.transform == null)
            return;

        T hitComponent = hit.transform.GetComponent<T>();  //获取碰撞组件

        if (!canMove && hit.transform != null)
        {
            OnCantMove<T>(hitComponent);
        }
    }

    protected abstract void OnCantMove<T>(T component) where T : Component;  //抽象类需在继承类重写

}
