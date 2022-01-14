using UnityEngine;

public class TennisBall : MonoBehaviour
{
    public Rigidbody RB;
    public bool Goal = false;
    public float Timer = 0;
    private int G;

    void Start()
    {
        G = 0;
        RB = gameObject.GetComponent<Rigidbody>();
        RB.velocity = new Vector2(Random.Range(0, 2) * 2 - 1, Random.Range(-0.5F, 0.5F) / 2F);
        transform.position = Vector3.zero;
    }

    void Update()
    {
        if (Goal)
        {
            transform.position = Vector3.zero;
            Timer += Time.deltaTime;
            if (Timer > 1)
            {
                G++;
                RB.velocity = new Vector2(Random.Range(0, 2) * 2 - 1, Random.Range(-0.5F, 0.5F) / 2F);
                Timer = 0;
                Goal = false;
            }
        }
        else
            RB.velocity = RB.velocity.normalized * 10F;
    }

    private void OnCollisionEnter(Collision collision)
    {
        OnCollision(collision);
    }

    private void OnCollisionStay(Collision collision)
    {
        OnCollision(collision);
    }

    private void OnCollision(Collision collision)
    {
        TennisControl TC = collision.gameObject.GetComponent<TennisControl>();
        if (TC != null)
            RB.velocity = new Vector3(Mathf.Sign((transform.position - TC.transform.position).x), (transform.position - TC.transform.position).y);
        else
            RB.velocity = new Vector3(Mathf.Sign(RB.velocity.x), -Mathf.Sign(collision.transform.position.y));
    }
}