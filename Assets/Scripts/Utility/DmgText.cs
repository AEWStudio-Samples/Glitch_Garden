using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DmgText : MonoBehaviour
{
    // con fig vars //
    public float despawnTime = 1f;
    public float velocity = 0.3f;
    public TextMeshPro text = null;

    // state vars //
    Rigidbody2D myBody;
    Vector2 startPos;

    private void Awake()
    {
        myBody = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        velocity = Random.Range(0.1f, velocity);
        myBody.velocity = new Vector2(0, velocity);
        StartCoroutine(DespawnTimer());
    }

    public void SetText(int dmg)
    {
        text.text = dmg.ToString();
    }

    private IEnumerator DespawnTimer()
    {
        while (despawnTime > 0)
        {
            despawnTime -= Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }
}
