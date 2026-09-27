using UnityEngine;

public class Key : MonoBehaviour
{
    [SerializeField] private Sprite[] allGateSprites;
    [SerializeField] private SpriteRenderer thisSpriteRen;


    private int gateIndex;
    private GateType thisGateType = GateType.None;
    private Spawner spawnerObj = null;

    public void SetUp(int index, GateType type, Spawner spawner)
    {
        if (type != GateType.None)
        {
            gateIndex = index;
            thisGateType = type;
            spawnerObj = spawner;
            int num = (int)thisGateType - 1;
            thisSpriteRen.sprite = allGateSprites[num];
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Objects"))
        {
            ShapeMovement shape = collision.gameObject.GetComponent<ShapeMovement>();
            if (spawnerObj && shape && shape.startMovement)
            {
                spawnerObj.KeyCollected(gateIndex);
                Destroy(gameObject);
            }
        }
    }
}
