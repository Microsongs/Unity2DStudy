using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingObject : MonoBehaviour
{
    //오브젝트를 움직이게 할 시간 단위
    public float moveTime = 0.1f;

    // 이동할 공간이 열려있고, 그 곳으로 이동하려 할 때 충돌이 일어났는지 체크할 뱐스
    public LayerMask blockingLayer;

    // BoxCollider2D 변수
    private BoxCollider2D boxCollider;

    // RIgidbody2D 변수
    private Rigidbody2D rb2D;

    // 움직임을 더 효율적으로 계산하는데 사용될 변수
    private float inverseMoveTime;

     
    // 자식클래스에서 오버라이드 할 수 있도록 변경
    // Start is called before the first frame update
    protected virtual void Start()
    {
        // 박스 콜라이더2D 컴포넌트의 레퍼런스를 가져옴
        boxCollider = GetComponent<BoxCollider2D>();
        // rigidbody2D 컴포넌트의 레퍼런스를 가져와 저장
        rb2D = GetComponent<Rigidbody2D>();
        //inverseMoveTime에 1 / moveTIme으로 지정, 곱하기를 사용할 수 있어 편해진다.
        inverseMoveTime = 1f / moveTime;
    }

    // SmoothMovement 코루틴을 선언한다. 이 코루틴은 유닛들을 한 공간에서 다른 곳으로 옮기는 데 사용
    protected IEnumerator SmoothMoveMent (Vector3 end)
    {
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

        while(sqrRemainingDistance > float.Epsilon)
        {
            Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);
            rb2D.MovePosition(newPosition);
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;
            yield return null;
        }
    }

    // Generic입력 T를 매개변수로 가져와 자식 클래스에서 오버라이드하도록 만듦
    protected abstract void OnCantMove<T>(T component)
        where T : Component;

    // 이동 지점에 장애물이 있는지 파악하고 없을 경우 이동하는 함수 Move
    protected bool Move(int xDir, int yDir, out RaycastHit2D hit)
    {
        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(xDir, yDir);

        boxCollider.enabled = false;
        hit = Physics2D.Linecast(start, end, blockingLayer);
        boxCollider.enabled = true;

        if(hit.transform == null)
        {
            StartCoroutine(SmoothMoveMent(end));
            return true;
        }
        return false;
    }

    protected virtual void AttemptMove<T>(int xDir, int yDir)
        where T : Component
    {
        RaycastHit2D hit;
        bool canMove = Move(xDir, yDir, out hit);

        if (hit.transform == null)
            return;
        T hitComponent = hit.transform.GetComponent<T>();

        if (!canMove && hitComponent != null)
            OnCantMove(hitComponent);
    }
}
